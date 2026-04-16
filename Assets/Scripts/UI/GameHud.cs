using System.Collections.Generic;
using CaravanRoguelite.Data;
using UnityEngine;
using UnityEngine.UI;

namespace CaravanRoguelite.UI
{
    public class GameHud
    {
        private readonly RectTransform _root;
        private readonly Text _top;
        private readonly Text _log;
        private readonly Dictionary<string, StatWidget> _widgets = new();
        private readonly Button _strategyButton;

        public GameHud(Transform parent)
        {
            _root = new GameObject("HudRoot", typeof(RectTransform)).GetComponent<RectTransform>();
            _root.SetParent(parent, false);
            _root.anchorMin = Vector2.zero;
            _root.anchorMax = Vector2.one;
            _root.offsetMin = Vector2.zero;
            _root.offsetMax = Vector2.zero;

            var topPanel = new GameObject("TopPanel", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
            topPanel.SetParent(_root, false);
            topPanel.anchorMin = new Vector2(0f, 0.915f);
            topPanel.anchorMax = new Vector2(1f, 0.995f);
            topPanel.offsetMin = new Vector2(16, 0);
            topPanel.offsetMax = new Vector2(-16, 0);
            var topImage = topPanel.GetComponent<Image>();
            topImage.sprite = ProceduralSpriteFactory.CreateRoundedRect(VisualTheme.PanelFill, VisualTheme.PanelBorder, 96, 18, 6);
            topImage.type = Image.Type.Sliced;

            _top = UiFactory.MakeText(topPanel, "", 22, TextAnchor.MiddleLeft);
            _top.rectTransform.anchorMin = new Vector2(0.02f, 0.08f);
            _top.rectTransform.anchorMax = new Vector2(0.78f, 0.92f);
            _top.color = VisualTheme.TextPrimary;

            _strategyButton = UiFactory.MakeButton(topPanel, "Стратегия");
            var strategyRect = _strategyButton.GetComponent<RectTransform>();
            strategyRect.anchorMin = new Vector2(0.8f, 0.15f);
            strategyRect.anchorMax = new Vector2(0.98f, 0.85f);
            strategyRect.offsetMin = Vector2.zero;
            strategyRect.offsetMax = Vector2.zero;

            var statsPanel = new GameObject("StatsPanel", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
            statsPanel.SetParent(_root, false);
            statsPanel.anchorMin = new Vector2(0.02f, 0.18f);
            statsPanel.anchorMax = new Vector2(0.29f, 0.9f);
            statsPanel.offsetMin = Vector2.zero;
            statsPanel.offsetMax = Vector2.zero;
            var statsImage = statsPanel.GetComponent<Image>();
            statsImage.sprite = ProceduralSpriteFactory.CreateRoundedRect(new Color(0.06f, 0.09f, 0.13f, 0.92f), new Color(0.25f, 0.4f, 0.58f, 0.98f), 128, 18, 6);
            statsImage.type = Image.Type.Sliced;

            var title = UiFactory.MakeText(statsPanel, "СТАТЫ КАРАВАНА", 15, TextAnchor.UpperCenter);
            title.rectTransform.anchorMin = new Vector2(0.06f, 0.92f);
            title.rectTransform.anchorMax = new Vector2(0.94f, 0.99f);
            title.color = new Color(0.88f, 0.95f, 1f, 1f);

            CreateWidget(statsPanel, "hp", "HP", 0.82f, new Color(1f, 0.36f, 0.4f, 1f));
            CreateWidget(statsPanel, "food", "Еда", 0.69f, new Color(1f, 0.82f, 0.36f, 1f));
            CreateWidget(statsPanel, "gold", "Золото", 0.56f, new Color(1f, 0.9f, 0.4f, 1f));
            CreateWidget(statsPanel, "morale", "Мораль", 0.43f, new Color(0.48f, 0.95f, 0.75f, 1f));
            CreateWidget(statsPanel, "attack", "Атака", 0.30f, new Color(0.6f, 0.82f, 1f, 1f));

            var logPanel = new GameObject("LogPanel", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
            logPanel.SetParent(_root, false);
            logPanel.anchorMin = new Vector2(0f, 0f);
            logPanel.anchorMax = new Vector2(1f, 0.145f);
            logPanel.offsetMin = new Vector2(16, 10);
            logPanel.offsetMax = new Vector2(-16, 0);
            var logImage = logPanel.GetComponent<Image>();
            logImage.sprite = ProceduralSpriteFactory.CreateRoundedRect(new Color(0.04f, 0.07f, 0.1f, 0.88f), new Color(0.22f, 0.34f, 0.46f, 0.9f), 96, 14, 5);
            logImage.type = Image.Type.Sliced;

            _log = UiFactory.MakeText(logPanel, "Добро пожаловать в пустошь.", 16, TextAnchor.UpperLeft);
            _log.rectTransform.anchorMin = new Vector2(0.02f, 0.06f);
            _log.rectTransform.anchorMax = new Vector2(0.98f, 0.92f);
            _log.color = VisualTheme.TextDim;
        }

        public void Refresh(CaravanStats stats, int day)
        {
            _top.text = $"Караван в пути — День {day}";

            SetWidget("hp", stats.Health, stats.MaxHealth, $"{stats.Health}/{stats.MaxHealth}");
            SetWidget("food", stats.Food, 20, stats.Food.ToString());
            SetWidget("gold", stats.Gold, 30, stats.Gold.ToString());
            SetWidget("morale", stats.Morale, 10, stats.Morale.ToString());
            SetWidget("attack", stats.Attack, 12, stats.Attack.ToString());
        }

        public void Log(string message)
        {
            _log.text = message;
        }

        public void BindOpenStrategy(System.Action onOpenStrategy)
        {
            _strategyButton.onClick.RemoveAllListeners();
            _strategyButton.onClick.AddListener(() => onOpenStrategy?.Invoke());
        }

        public void SetStrategyNavigationEnabled(bool enabled)
        {
            _strategyButton.interactable = enabled;
        }

        public void SetVisible(bool visible)
        {
            _root.gameObject.SetActive(visible);
        }

        private void CreateWidget(Transform parent, string id, string label, float yAnchor, Color color)
        {
            var holder = new GameObject($"{id}_Widget", typeof(RectTransform)).GetComponent<RectTransform>();
            holder.SetParent(parent, false);
            holder.anchorMin = new Vector2(0.08f, yAnchor - 0.11f);
            holder.anchorMax = new Vector2(0.92f, yAnchor + 0.02f);
            holder.offsetMin = Vector2.zero;
            holder.offsetMax = Vector2.zero;

            var name = UiFactory.MakeText(holder, label, 14, TextAnchor.MiddleLeft);
            name.rectTransform.anchorMin = new Vector2(0f, 0.58f);
            name.rectTransform.anchorMax = new Vector2(0.58f, 1f);
            name.color = new Color(0.84f, 0.91f, 0.99f, 1f);

            var value = UiFactory.MakeText(holder, "", 14, TextAnchor.MiddleRight);
            value.rectTransform.anchorMin = new Vector2(0.58f, 0.58f);
            value.rectTransform.anchorMax = new Vector2(1f, 1f);
            value.color = Color.white;

            var barBg = new GameObject("BarBg", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
            barBg.SetParent(holder, false);
            barBg.anchorMin = new Vector2(0f, 0f);
            barBg.anchorMax = new Vector2(1f, 0.5f);
            barBg.offsetMin = Vector2.zero;
            barBg.offsetMax = Vector2.zero;
            var barBgImage = barBg.GetComponent<Image>();
            barBgImage.sprite = ProceduralSpriteFactory.CreateRoundedRect(new Color(0.08f, 0.12f, 0.18f, 1f), new Color(0.19f, 0.27f, 0.38f, 1f), 96, 10, 3);
            barBgImage.type = Image.Type.Sliced;

            var fill = new GameObject("Fill", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
            fill.SetParent(barBg, false);
            fill.anchorMin = new Vector2(0.01f, 0.12f);
            fill.anchorMax = new Vector2(0.99f, 0.88f);
            fill.offsetMin = Vector2.zero;
            fill.offsetMax = Vector2.zero;
            var fillImage = fill.GetComponent<Image>();
            fillImage.sprite = ProceduralSpriteFactory.CreateRoundedRect(color, Color.white, 96, 8, 3);
            fillImage.type = Image.Type.Sliced;
            fill.pivot = new Vector2(0f, 0.5f);

            _widgets[id] = new StatWidget(value, fill, fillImage, color);
        }

        private void SetWidget(string id, int value, int targetRange, string valueLabel)
        {
            if (!_widgets.TryGetValue(id, out var widget))
            {
                return;
            }

            widget.ValueLabel.text = valueLabel;
            float normalized = Mathf.Clamp01(targetRange <= 0 ? 0f : value / (float)targetRange);
            widget.Fill.localScale = new Vector3(Mathf.Lerp(0.06f, 1f, normalized), 1f, 1f);

            float lowWarning = normalized < 0.25f ? 0.35f : 0f;
            widget.FillImage.color = Color.Lerp(widget.BaseColor, Color.red, lowWarning);
        }

        private readonly struct StatWidget
        {
            public readonly Text ValueLabel;
            public readonly RectTransform Fill;
            public readonly Image FillImage;
            public readonly Color BaseColor;

            public StatWidget(Text valueLabel, RectTransform fill, Image fillImage, Color baseColor)
            {
                ValueLabel = valueLabel;
                Fill = fill;
                FillImage = fillImage;
                BaseColor = baseColor;
            }
        }
    }
}
