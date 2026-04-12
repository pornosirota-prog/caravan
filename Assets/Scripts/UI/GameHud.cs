using CaravanRoguelite.Data;
using UnityEngine;
using UnityEngine.UI;

namespace CaravanRoguelite.UI
{
    public class GameHud
    {
        private readonly Text _top;
        private readonly Text _log;

        public GameHud(Transform parent)
        {
            var topPanel = new GameObject("TopPanel", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
            topPanel.SetParent(parent, false);
            topPanel.anchorMin = new Vector2(0f, 0.905f);
            topPanel.anchorMax = new Vector2(1f, 0.995f);
            topPanel.offsetMin = new Vector2(16, 0);
            topPanel.offsetMax = new Vector2(-16, 0);
            var topImage = topPanel.GetComponent<Image>();
            topImage.sprite = ProceduralSpriteFactory.CreateRoundedRect(VisualTheme.PanelFill, VisualTheme.PanelBorder, 96, 18, 6);
            topImage.type = Image.Type.Sliced;

            _top = UiFactory.MakeText(topPanel, "", 20, TextAnchor.MiddleLeft);
            _top.rectTransform.anchorMin = new Vector2(0.02f, 0f);
            _top.rectTransform.anchorMax = new Vector2(0.98f, 1f);
            _top.color = VisualTheme.TextPrimary;

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
            _top.text = $"День {day}  HP {stats.Health}/{stats.MaxHealth}  Еда {stats.Food}  Золото {stats.Gold}  Мораль {stats.Morale}  Атака {stats.Attack}";
        }

        public void Log(string message)
        {
            _log.text = message;
        }
    }
}
