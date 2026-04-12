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
            var topPanel = new GameObject("TopPanel", typeof(RectTransform)).GetComponent<RectTransform>();
            topPanel.SetParent(parent, false);
            topPanel.anchorMin = new Vector2(0f, 0.92f);
            topPanel.anchorMax = new Vector2(1f, 1f);
            topPanel.offsetMin = new Vector2(20, 0);
            topPanel.offsetMax = new Vector2(-20, 0);
            _top = UiFactory.MakeText(topPanel, "", 20, TextAnchor.MiddleLeft);
            _top.rectTransform.anchorMin = Vector2.zero;
            _top.rectTransform.anchorMax = Vector2.one;

            var logPanel = new GameObject("LogPanel", typeof(RectTransform)).GetComponent<RectTransform>();
            logPanel.SetParent(parent, false);
            logPanel.anchorMin = new Vector2(0f, 0f);
            logPanel.anchorMax = new Vector2(1f, 0.14f);
            logPanel.offsetMin = new Vector2(20, 10);
            logPanel.offsetMax = new Vector2(-20, 0);
            _log = UiFactory.MakeText(logPanel, "Добро пожаловать в пустошь.", 16, TextAnchor.UpperLeft);
            _log.rectTransform.anchorMin = Vector2.zero;
            _log.rectTransform.anchorMax = Vector2.one;
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
