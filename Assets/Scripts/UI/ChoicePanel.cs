using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CaravanRoguelite.UI
{
    public class ChoicePanel
    {
        private readonly RectTransform _root;
        private readonly Text _title;
        private readonly Text _body;
        private readonly RectTransform _choices;

        public ChoicePanel(Transform parent)
        {
            _root = new GameObject("ChoicePanel", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
            _root.SetParent(parent, false);
            _root.anchorMin = new Vector2(0.2f, 0.2f);
            _root.anchorMax = new Vector2(0.8f, 0.78f);
            _root.offsetMin = Vector2.zero;
            _root.offsetMax = Vector2.zero;
            _root.GetComponent<Image>().color = new Color(0.08f, 0.09f, 0.12f, 0.95f);

            _title = UiFactory.MakeText(_root, "", 26, TextAnchor.UpperCenter);
            _title.rectTransform.anchorMin = new Vector2(0f, 0.83f);
            _title.rectTransform.anchorMax = new Vector2(1f, 1f);

            _body = UiFactory.MakeText(_root, "", 18, TextAnchor.UpperLeft);
            _body.rectTransform.anchorMin = new Vector2(0.06f, 0.42f);
            _body.rectTransform.anchorMax = new Vector2(0.94f, 0.8f);

            _choices = new GameObject("Choices", typeof(RectTransform)).GetComponent<RectTransform>();
            _choices.SetParent(_root, false);
            _choices.anchorMin = new Vector2(0.08f, 0.08f);
            _choices.anchorMax = new Vector2(0.92f, 0.38f);
            _choices.offsetMin = Vector2.zero;
            _choices.offsetMax = Vector2.zero;

            Hide();
        }

        public void Show(string title, string body, List<string> choices, Action<int> onPick)
        {
            _root.gameObject.SetActive(true);
            _title.text = title;
            _body.text = body;

            foreach (Transform child in _choices)
            {
                UnityEngine.Object.Destroy(child.gameObject);
            }

            for (int i = 0; i < choices.Count; i++)
            {
                var button = UiFactory.MakeButton(_choices, choices[i]);
                int captured = i;
                button.onClick.AddListener(() => onPick(captured));
                var rect = button.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0f, 1f - (i + 1) * 0.3f);
                rect.anchorMax = new Vector2(1f, 1f - i * 0.3f);
                rect.offsetMin = new Vector2(0, -6);
                rect.offsetMax = new Vector2(0, -6);
            }
        }

        public void Hide()
        {
            _root.gameObject.SetActive(false);
        }
    }
}
