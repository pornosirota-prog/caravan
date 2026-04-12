using UnityEngine;
using UnityEngine.UI;

namespace CaravanRoguelite.UI
{
    public static class UiFactory
    {
        private static Font _font;

        public static Font Font => _font ??= Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        public static Text MakeText(Transform parent, string value, int size, TextAnchor anchor)
        {
            var go = new GameObject("Text", typeof(RectTransform), typeof(Text));
            go.transform.SetParent(parent, false);
            var text = go.GetComponent<Text>();
            text.font = Font;
            text.text = value;
            text.color = Color.white;
            text.fontSize = size;
            text.alignment = anchor;
            return text;
        }

        public static Button MakeButton(Transform parent, string label)
        {
            var buttonGo = new GameObject(label + "Button", typeof(RectTransform), typeof(Image), typeof(Button));
            buttonGo.transform.SetParent(parent, false);
            var image = buttonGo.GetComponent<Image>();
            image.sprite = ProceduralSpriteFactory.CreateRect(new Color(0.16f, 0.18f, 0.24f), 16, 16);
            image.type = Image.Type.Sliced;

            var text = MakeText(buttonGo.transform, label, 18, TextAnchor.MiddleCenter);
            var rect = text.rectTransform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            return buttonGo.GetComponent<Button>();
        }
    }
}
