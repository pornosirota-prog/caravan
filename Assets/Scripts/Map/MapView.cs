using System;
using System.Collections.Generic;
using CaravanRoguelite.Data;
using CaravanRoguelite.UI;
using UnityEngine;
using UnityEngine.UI;

namespace CaravanRoguelite.Map
{
    public class MapView
    {
        private readonly RectTransform _root;
        private readonly Dictionary<int, Button> _buttons = new();
        private readonly Dictionary<int, RectTransform> _nodeVisuals = new();
        private readonly Dictionary<int, Image> _nodeGlows = new();
        private readonly Dictionary<int, RectTransform> _moveArrows = new();
        private readonly Dictionary<int, Vector2> _nodePositions = new();
        private readonly List<RectTransform> _particles = new();
        private float _uiScale = 1f;
        private RectTransform _caravan;
        private int _currentNodeId;

        public MapView(Transform parent)
        {
            _root = new GameObject("MapPanel", typeof(RectTransform)).GetComponent<RectTransform>();
            _root.SetParent(parent, false);
            _root.anchorMin = new Vector2(0.04f, 0.18f);
            _root.anchorMax = new Vector2(0.96f, 0.9f);
            _root.offsetMin = Vector2.zero;
            _root.offsetMax = Vector2.zero;
        }

        public void Render(MapGraph graph, int currentNodeId, Action<int> onClick)
        {
            _currentNodeId = currentNodeId;
            foreach (Transform child in _root)
            {
                UnityEngine.Object.Destroy(child.gameObject);
            }

            _buttons.Clear();
            _nodeVisuals.Clear();
            _nodeGlows.Clear();
            _moveArrows.Clear();
            _nodePositions.Clear();
            _particles.Clear();
            _caravan = null;

            DrawBiomeBands(graph);
            CreateGuideOverlay();
            BuildNodeLayout(graph);

            foreach (var node in graph.Nodes)
            {
                foreach (int target in node.Links)
                {
                    DrawLink(node.Id, target, graph);
                }
            }

            foreach (var node in graph.Nodes)
            {
                var button = CreateNode(node);
                int captured = node.Id;
                button.onClick.AddListener(() => onClick(captured));
                _buttons[node.Id] = button;
            }

            CreateAmbientParticles(28);
            CreateCaravanMarker();
        }

        public void Tick(float time)
        {
            foreach (var pair in _nodeVisuals)
            {
                float phase = time * 2.1f + pair.Key * 0.31f;
                float basePulse = 1f + Mathf.Sin(phase) * 0.05f;
                float currentBoost = pair.Key == _currentNodeId ? 0.11f + Mathf.Sin(time * 4f) * 0.05f : 0f;
                pair.Value.localScale = Vector3.one * (basePulse + currentBoost);

                if (_nodeGlows.TryGetValue(pair.Key, out var glow))
                {
                    float alpha = pair.Key == _currentNodeId ? 0.5f : 0.18f;
                    glow.color = new Color(glow.color.r, glow.color.g, glow.color.b, alpha + Mathf.Sin(phase) * 0.08f);
                }

                if (_moveArrows.TryGetValue(pair.Key, out var arrow))
                {
                    float bounce = Mathf.Sin((time * 6f) + pair.Key * 0.9f) * 4f;
                    arrow.anchoredPosition = new Vector2(0f, (41f * _uiScale) + bounce);
                }
            }

            if (_caravan != null && _nodePositions.TryGetValue(_currentNodeId, out var markerTarget))
            {
                float wagonBounce = Mathf.Sin(time * 3.2f) * 3.2f;
                _caravan.anchoredPosition = Vector2.Lerp(_caravan.anchoredPosition, markerTarget + new Vector2(0f, (54f * _uiScale) + wagonBounce), 0.14f);
            }

            for (int i = 0; i < _particles.Count; i++)
            {
                var particle = _particles[i];
                var pos = particle.anchoredPosition;
                pos.y += Mathf.Sin((time * 0.9f) + i * 0.43f) * 0.16f;
                pos.x += Mathf.Cos((time * 0.6f) + i * 0.33f) * 0.12f;
                particle.anchoredPosition = pos;
            }
        }

        public void SetInteractable(HashSet<int> allowed)
        {
            foreach (var pair in _buttons)
            {
                bool enabled = allowed.Contains(pair.Key);
                pair.Value.interactable = enabled;
                var image = pair.Value.GetComponent<Image>();
                image.color = enabled || pair.Key == _currentNodeId ? Color.white : new Color(0.62f, 0.65f, 0.7f, 0.72f);

                if (_moveArrows.TryGetValue(pair.Key, out var arrow))
                {
                    arrow.gameObject.SetActive(enabled);
                }
            }
        }

        private Button CreateNode(MapNode node)
        {
            var button = UiFactory.MakeButton(_root, "");
            var rect = button.GetComponent<RectTransform>();
            rect.sizeDelta = Vector2.one * (42f * _uiScale);
            rect.anchoredPosition = GetNodePosition(node.Id);

            var buttonImage = button.GetComponent<Image>();
            buttonImage.sprite = ProceduralSpriteFactory.CreateCircle(new Color(0.08f, 0.11f, 0.15f, 0.96f), 64);
            buttonImage.type = Image.Type.Simple;

            var glow = new GameObject("NodeGlow", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
            glow.SetParent(button.transform, false);
            glow.anchorMin = new Vector2(-0.4f, -0.4f);
            glow.anchorMax = new Vector2(1.4f, 1.4f);
            glow.offsetMin = Vector2.zero;
            glow.offsetMax = Vector2.zero;
            var glowImage = glow.GetComponent<Image>();
            glowImage.raycastTarget = false;
            glowImage.sprite = ProceduralSpriteFactory.CreateSoftCircle(VisualTheme.ByNodeType(node.Type), 128, 2f);
            glowImage.color = new Color(1f, 1f, 1f, node.Id == _currentNodeId ? 0.52f : 0.18f);
            glow.SetAsFirstSibling();

            var icon = new GameObject("Silhouette", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
            icon.SetParent(button.transform, false);
            icon.anchorMin = new Vector2(0.12f, 0.12f);
            icon.anchorMax = new Vector2(0.88f, 0.88f);
            icon.offsetMin = Vector2.zero;
            icon.offsetMax = Vector2.zero;
            var iconImage = icon.GetComponent<Image>();
            iconImage.raycastTarget = false;
            iconImage.sprite = SpriteByType(node.Type);
            iconImage.color = VisualTheme.ByNodeType(node.Type);

            var typeLabel = UiFactory.MakeText(button.transform, LabelByType(node.Type), Mathf.RoundToInt(10f * _uiScale), TextAnchor.MiddleCenter);
            typeLabel.rectTransform.anchorMin = new Vector2(-0.7f, -0.62f);
            typeLabel.rectTransform.anchorMax = new Vector2(1.7f, -0.1f);
            typeLabel.rectTransform.offsetMin = Vector2.zero;
            typeLabel.rectTransform.offsetMax = Vector2.zero;
            typeLabel.color = new Color(0.88f, 0.94f, 1f, 0.95f);

            var moveArrow = new GameObject("MoveArrow", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
            moveArrow.SetParent(button.transform, false);
            moveArrow.sizeDelta = Vector2.one * (18f * _uiScale);
            moveArrow.anchoredPosition = new Vector2(0f, 41f * _uiScale);
            var arrowImage = moveArrow.GetComponent<Image>();
            arrowImage.raycastTarget = false;
            arrowImage.sprite = ProceduralSpriteFactory.CreatePolygon(3, new Color(0.93f, 0.99f, 1f, 0.98f), 48, 90f);

            var moveText = UiFactory.MakeText(moveArrow, "ЖМИ", Mathf.RoundToInt(8f * _uiScale), TextAnchor.LowerCenter);
            moveText.rectTransform.anchorMin = new Vector2(-1.2f, -1.2f);
            moveText.rectTransform.anchorMax = new Vector2(2.2f, -0.2f);
            moveText.rectTransform.offsetMin = Vector2.zero;
            moveText.rectTransform.offsetMax = Vector2.zero;
            moveText.color = new Color(0.95f, 0.98f, 1f, 0.98f);

            if (node.Id == _currentNodeId)
            {
                var marker = UiFactory.MakeText(button.transform, "ВЫ ЗДЕСЬ", Mathf.RoundToInt(10f * _uiScale), TextAnchor.UpperCenter);
                marker.rectTransform.anchorMin = new Vector2(-0.8f, 1.02f);
                marker.rectTransform.anchorMax = new Vector2(1.8f, 1.62f);
                marker.rectTransform.offsetMin = Vector2.zero;
                marker.rectTransform.offsetMax = Vector2.zero;
                marker.color = new Color(1f, 0.97f, 0.77f, 1f);
            }

            _nodeVisuals[node.Id] = rect;
            _nodeGlows[node.Id] = glowImage;
            _moveArrows[node.Id] = moveArrow;

            return button;
        }

        private void CreateGuideOverlay()
        {
            var panel = new GameObject("GuidePanel", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
            panel.SetParent(_root, false);
            panel.anchorMin = new Vector2(0.02f, 0.82f);
            panel.anchorMax = new Vector2(0.52f, 0.99f);
            panel.offsetMin = Vector2.zero;
            panel.offsetMax = Vector2.zero;

            var bg = panel.GetComponent<Image>();
            bg.sprite = ProceduralSpriteFactory.CreateRoundedRect(new Color(0.04f, 0.07f, 0.1f, 0.86f), new Color(0.3f, 0.48f, 0.7f, 0.9f), 96, 14, 4);
            bg.type = Image.Type.Sliced;
            bg.raycastTarget = false;

            var text = UiFactory.MakeText(panel, "1) Кликни по узлу со стрелкой ▲\n2) Посмотри прогноз и подтверди переход\n3) Серые узлы пока недоступны", Mathf.RoundToInt(13f * _uiScale), TextAnchor.UpperLeft);
            text.rectTransform.anchorMin = new Vector2(0.03f, 0.08f);
            text.rectTransform.anchorMax = new Vector2(0.97f, 0.94f);
            text.color = new Color(0.92f, 0.96f, 1f, 1f);
        }

        private void DrawBiomeBands(MapGraph graph)
        {
            Color[] biomes = { VisualTheme.BiomeRuins, VisualTheme.BiomeWilds, VisualTheme.BiomeAsh };
            for (int i = 0; i < biomes.Length; i++)
            {
                float start = i / (float)biomes.Length;
                float end = (i + 1) / (float)biomes.Length;
                var biome = new GameObject($"BiomeBand{i}", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
                biome.SetParent(_root, false);
                biome.anchorMin = new Vector2(start, 0f);
                biome.anchorMax = new Vector2(end, 1f);
                biome.offsetMin = Vector2.zero;
                biome.offsetMax = Vector2.zero;
                var image = biome.GetComponent<Image>();
                image.sprite = ProceduralSpriteFactory.CreateSoftCircle(biomes[i], 256, 2.4f);
                image.type = Image.Type.Simple;
                image.raycastTarget = false;
            }
        }

        private void DrawLink(int fromNodeId, int toNodeId, MapGraph graph)
        {
            var from = GetNodePosition(fromNodeId);
            var to = GetNodePosition(toNodeId);
            var delta = to - from;
            var position = from + delta / 2f;
            float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;

            var glow = new GameObject("LinkGlow", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
            glow.SetParent(_root, false);
            glow.sizeDelta = new Vector2(delta.magnitude + 10f, 11f);
            glow.anchoredPosition = position;
            glow.localRotation = Quaternion.Euler(0, 0, angle);
            var glowImage = glow.GetComponent<Image>();
            glowImage.sprite = ProceduralSpriteFactory.CreateLine(VisualTheme.RouteGlow, 128, 18);
            glowImage.raycastTarget = false;

            var line = new GameObject("Link", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
            line.SetParent(_root, false);
            line.sizeDelta = new Vector2(delta.magnitude, 4f);
            line.anchoredPosition = position;
            line.localRotation = Quaternion.Euler(0, 0, angle);
            var lineImage = line.GetComponent<Image>();
            lineImage.sprite = ProceduralSpriteFactory.CreateLine(VisualTheme.RouteLine, 128, 10);
            lineImage.raycastTarget = false;
        }

        private void CreateAmbientParticles(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var particle = new GameObject("Particle", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
                particle.SetParent(_root, false);
                particle.anchorMin = new Vector2(0.5f, 0.5f);
                particle.anchorMax = new Vector2(0.5f, 0.5f);
                particle.sizeDelta = Vector2.one * UnityEngine.Random.Range(3.5f, 7f);
                particle.anchoredPosition = new Vector2(UnityEngine.Random.Range(-520f, 520f), UnityEngine.Random.Range(-220f, 220f));
                var image = particle.GetComponent<Image>();
                image.raycastTarget = false;
                image.sprite = ProceduralSpriteFactory.CreateSoftCircle(new Color(0.72f, 0.85f, 1f, UnityEngine.Random.Range(0.08f, 0.2f)), 64, 2.6f);
                _particles.Add(particle);
            }
        }

        private void CreateCaravanMarker()
        {
            _caravan = new GameObject("CaravanMarker", typeof(RectTransform)).GetComponent<RectTransform>();
            _caravan.SetParent(_root, false);
            _caravan.sizeDelta = new Vector2(56f, 36f) * _uiScale;
            if (_nodePositions.TryGetValue(_currentNodeId, out var currentPos))
            {
                _caravan.anchoredPosition = currentPos + new Vector2(0f, 54f * _uiScale);
            }

            var body = new GameObject("CaravanBody", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
            body.SetParent(_caravan, false);
            body.anchorMin = new Vector2(0.12f, 0.32f);
            body.anchorMax = new Vector2(0.88f, 0.78f);
            body.offsetMin = Vector2.zero;
            body.offsetMax = Vector2.zero;
            var bodyImage = body.GetComponent<Image>();
            bodyImage.raycastTarget = false;
            bodyImage.sprite = ProceduralSpriteFactory.CreateRoundedRect(new Color(0.66f, 0.5f, 0.34f, 1f), new Color(0.88f, 0.74f, 0.52f, 1f), 80, 14, 4);
            bodyImage.type = Image.Type.Sliced;

            CreateWheel(new Vector2(0.28f, 0.18f));
            CreateWheel(new Vector2(0.72f, 0.18f));
        }

        private void CreateWheel(Vector2 anchor)
        {
            var wheel = new GameObject("Wheel", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
            wheel.SetParent(_caravan, false);
            wheel.anchorMin = anchor;
            wheel.anchorMax = anchor;
            wheel.sizeDelta = Vector2.one * (11f * _uiScale);
            wheel.anchoredPosition = Vector2.zero;
            var image = wheel.GetComponent<Image>();
            image.raycastTarget = false;
            image.sprite = ProceduralSpriteFactory.CreateCircle(new Color(0.14f, 0.17f, 0.22f, 1f), 64);
        }

        private Vector2 GetNodePosition(int nodeId)
        {
            return _nodePositions.TryGetValue(nodeId, out var value) ? value : Vector2.zero;
        }

        private void BuildNodeLayout(MapGraph graph)
        {
            float minX = float.MaxValue;
            float maxX = float.MinValue;
            float minY = float.MaxValue;
            float maxY = float.MinValue;

            foreach (var node in graph.Nodes)
            {
                minX = Mathf.Min(minX, node.Position.x);
                maxX = Mathf.Max(maxX, node.Position.x);
                minY = Mathf.Min(minY, node.Position.y);
                maxY = Mathf.Max(maxY, node.Position.y);
            }

            var size = _root.rect.size;
            float width = Mathf.Max(size.x, 900f);
            float height = Mathf.Max(size.y, 460f);
            _uiScale = Mathf.Clamp(Mathf.Min(width / 1280f, height / 720f), 0.82f, 1.2f);
            float spanX = Mathf.Max(0.01f, maxX - minX);
            float spanY = Mathf.Max(0.01f, maxY - minY);
            float usableWidth = width * 0.72f;
            float usableHeight = height * 0.54f;
            float scale = Mathf.Min(usableWidth / spanX, usableHeight / spanY);
            Vector2 center = new Vector2((minX + maxX) * 0.5f, (minY + maxY) * 0.5f);
            Vector2 mapOffset = new Vector2(width * 0.2f, -height * 0.05f);

            foreach (var node in graph.Nodes)
            {
                Vector2 centered = (node.Position - center) * scale;
                _nodePositions[node.Id] = centered + mapOffset;
            }
        }

        private Sprite SpriteByType(NodeType type)
        {
            return type switch
            {
                NodeType.Start => ProceduralSpriteFactory.CreatePolygon(4, Color.white, 72, 45f),
                NodeType.Event => ProceduralSpriteFactory.CreatePolygon(6, Color.white, 72, 0f),
                NodeType.City => ProceduralSpriteFactory.CreateRoundedRect(Color.white, Color.white, 72, 12, 0),
                NodeType.Boss => ProceduralSpriteFactory.CreatePolygon(3, Color.white, 72, 90f),
                _ => ProceduralSpriteFactory.CreatePolygon(5, Color.white, 72, 18f)
            };
        }

        private string LabelByType(NodeType type)
        {
            return type switch
            {
                NodeType.Start => "СТАРТ",
                NodeType.Event => "СОБЫТИЕ",
                NodeType.Combat => "БОЙ",
                NodeType.City => "ГОРОД",
                NodeType.Boss => "БОСС",
                _ => "УЗЕЛ"
            };
        }
    }
}
