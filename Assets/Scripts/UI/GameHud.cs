using CaravanRoguelite.Data;
using UnityEngine;
using UnityEngine.UI;

namespace CaravanRoguelite.UI
{
    public class GameHud
    {
        private readonly Text _top;
        private readonly Text _stats;
        private readonly Text _log;

        public GameHud(Transform parent)
        {
            var topPanel = new GameObject("TopPanel", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
            topPanel.SetParent(parent, false);
            topPanel.anchorMin = new Vector2(0f, 0.915f);
            topPanel.anchorMax = new Vector2(1f, 0.995f);
            topPanel.offsetMin = new Vector2(16, 0);
            topPanel.offsetMax = new Vector2(-16, 0);
            var topImage = topPanel.GetComponent<Image>();
            topImage.sprite = ProceduralSpriteFactory.CreateRoundedRect(VisualTheme.PanelFill, VisualTheme.PanelBorder, 96, 18, 6);
            topImage.type = Image.Type.Sliced;

            _top = UiFactory.MakeText(topPanel, "", 22, TextAnchor.MiddleLeft);
            _top.rectTransform.anchorMin = new Vector2(0.02f, 0.08f);
            _top.rectTransform.anchorMax = new Vector2(0.98f, 0.92f);
            _top.color = VisualTheme.TextPrimary;

            var statsPanel = new GameObject("StatsPanel", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
            statsPanel.SetParent(parent, false);
            statsPanel.anchorMin = new Vector2(0.02f, 0.18f);
            statsPanel.anchorMax = new Vector2(0.26f, 0.9f);
            statsPanel.offsetMin = Vector2.zero;
            statsPanel.offsetMax = Vector2.zero;
            var statsImage = statsPanel.GetComponent<Image>();
            statsImage.sprite = ProceduralSpriteFactory.CreateRoundedRect(new Color(0.06f, 0.09f, 0.13f, 0.9f), new Color(0.25f, 0.4f, 0.58f, 0.98f), 128, 18, 6);
            statsImage.type = Image.Type.Sliced;

            _stats = UiFactory.MakeText(statsPanel, "", 17, TextAnchor.UpperLeft);
            _stats.rectTransform.anchorMin = new Vector2(0.08f, 0.05f);
            _stats.rectTransform.anchorMax = new Vector2(0.92f, 0.95f);
            _stats.color = VisualTheme.TextDim;

            var logPanel = new GameObject("LogPanel", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
            logPanel.SetParent(parent, false);
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
            _stats.text =
                $"┌ Статы каравана\n" +
                $"│ HP: {stats.Health}/{stats.MaxHealth}\n" +
                $"│ Еда: {stats.Food}\n" +
                $"│ Золото: {stats.Gold}\n" +
                $"│ Мораль: {stats.Morale}\n" +
                $"│ Атака: {stats.Attack}\n" +
                "└────────────";
        }

        public void Log(string message)
        {
            _log.text = message;
        }
    }
}
