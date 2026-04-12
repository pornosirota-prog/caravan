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
            _root.anchorMin = new Vector2(0.19f, 0.18f);
            _root.anchorMax = new Vector2(0.81f, 0.82f);
            _root.offsetMin = Vector2.zero;
            _root.offsetMax = Vector2.zero;

            var panelImage = _root.GetComponent<Image>();
            panelImage.sprite = ProceduralSpriteFactory.CreateRoundedRect(VisualTheme.PanelFill, VisualTheme.PanelBorder, 128, 24, 7);
            panelImage.type = Image.Type.Sliced;

            var highlight = new GameObject("PanelHighlight", typeof(RectTransform), typeof(Image)).GetComponent<RectTransform>();
            highlight.SetParent(_root, false);
            highlight.anchorMin = new Vector2(-0.12f, 0.58f);
            highlight.anchorMax = new Vector2(1.12f, 1.08f);
            highlight.offsetMin = Vector2.zero;
            highlight.offsetMax = Vector2.zero;
            var highlightImage = highlight.GetComponent<Image>();
            highlightImage.raycastTarget = false;
            highlightImage.sprite = ProceduralSpriteFactory.CreateSoftCircle(new Color(0.35f, 0.68f, 1f, 0.16f), 160, 2.2f);

            _title = UiFactory.MakeText(_root, "", 28, TextAnchor.UpperCenter);
            _title.rectTransform.anchorMin = new Vector2(0.06f, 0.82f);
            _title.rectTransform.anchorMax = new Vector2(0.94f, 0.98f);
            _title.color = VisualTheme.TextPrimary;

            _body = UiFactory.MakeText(_root, "", 19, TextAnchor.UpperLeft);
            _body.rectTransform.anchorMin = new Vector2(0.07f, 0.42f);
            _body.rectTransform.anchorMax = new Vector2(0.93f, 0.79f);
            _body.color = VisualTheme.TextDim;

            _choices = new GameObject("Choices", typeof(RectTransform)).GetComponent<RectTransform>();
            _choices.SetParent(_root, false);
            _choices.anchorMin = new Vector2(0.08f, 0.08f);
            _choices.anchorMax = new Vector2(0.92f, 0.35f);
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

            float slot = 1f / Mathf.Max(choices.Count, 1);
            for (int i = 0; i < choices.Count; i++)
            {
                var button = UiFactory.MakeButton(_choices, choices[i]);
                int captured = i;
                button.onClick.AddListener(() => onPick(captured));
                var rect = button.GetComponent<RectTransform>();
                rect.anchorMin = new Vector2(0f, 1f - (i + 1) * slot + 0.02f);
                rect.anchorMax = new Vector2(1f, 1f - i * slot - 0.02f);
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
            }
        }

        public void Hide()
        {
            _root.gameObject.SetActive(false);
        }
    }
}
