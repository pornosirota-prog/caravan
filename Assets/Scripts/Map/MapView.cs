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
            foreach (Transform child in _root)
            {
                UnityEngine.Object.Destroy(child.gameObject);
            }
            _buttons.Clear();

            foreach (var node in graph.Nodes)
            {
                foreach (int target in node.Links)
                {
                    var to = graph.Get(target);
                    DrawLink(node.Position, to.Position);
                }
            }

            foreach (var node in graph.Nodes)
            {
                var button = CreateNode(node);
                int captured = node.Id;
                button.onClick.AddListener(() => onClick(captured));
                if (node.Id == currentNodeId)
                {
                    button.GetComponent<Image>().color = Color.cyan;
                }
                _buttons[node.Id] = button;
            }
        }

        public void SetInteractable(HashSet<int> allowed)
        {
            foreach (var pair in _buttons)
            {
                pair.Value.interactable = allowed.Contains(pair.Key);
            }
        }

        private Button CreateNode(MapNode node)
        {
            var button = UiFactory.MakeButton(_root, "");
            var image = button.GetComponent<Image>();
            image.sprite = ProceduralSpriteFactory.CreateCircle(ColorByType(node.Type));
            image.type = Image.Type.Simple;

            var rect = button.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(36, 36);
            rect.anchoredPosition = node.Position * 56f;

            return button;
        }

        private void DrawLink(Vector2 from, Vector2 to)
        {
            var line = new GameObject("Link", typeof(RectTransform), typeof(Image));
            line.transform.SetParent(_root, false);
            var rect = line.GetComponent<RectTransform>();
            var delta = (to - from) * 56f;
            rect.sizeDelta = new Vector2(delta.magnitude, 3f);
            rect.anchoredPosition = from * 56f + delta / 2f;
            rect.localRotation = Quaternion.Euler(0, 0, Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg);
            var image = line.GetComponent<Image>();
            image.sprite = ProceduralSpriteFactory.CreateRect(new Color(0.35f, 0.35f, 0.38f), 4, 4);
        }

        private Color ColorByType(NodeType type)
        {
            return type switch
            {
                NodeType.Start => Color.white,
                NodeType.Event => new Color(0.2f, 0.6f, 1f),
                NodeType.City => new Color(0.2f, 0.9f, 0.3f),
                NodeType.Boss => new Color(1f, 0.2f, 0.2f),
                _ => new Color(1f, 0.65f, 0.1f)
            };
        }
    }
}
