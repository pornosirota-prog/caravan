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
            text.color = VisualTheme.TextPrimary;
            text.fontSize = size;
            text.alignment = anchor;
            return text;
        }

        public static Button MakeButton(Transform parent, string label)
        {
            var buttonGo = new GameObject(label + "Button", typeof(RectTransform), typeof(Image), typeof(Button));
            buttonGo.transform.SetParent(parent, false);
            var image = buttonGo.GetComponent<Image>();
            image.sprite = ProceduralSpriteFactory.CreateRoundedRect(new Color(0.09f, 0.14f, 0.2f, 0.98f), new Color(0.34f, 0.56f, 0.86f, 1f));
            image.type = Image.Type.Sliced;

            var button = buttonGo.GetComponent<Button>();
            var colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(1f, 1f, 1f, 1f);
            colors.pressedColor = new Color(0.88f, 0.94f, 1f, 1f);
            colors.disabledColor = new Color(0.5f, 0.56f, 0.64f, 0.7f);
            colors.fadeDuration = 0.15f;
            button.colors = colors;

            var glowGo = new GameObject("Glow", typeof(RectTransform), typeof(Image));
            glowGo.transform.SetParent(buttonGo.transform, false);
            var glowImage = glowGo.GetComponent<Image>();
            glowImage.raycastTarget = false;
            glowImage.sprite = ProceduralSpriteFactory.CreateSoftCircle(new Color(0.3f, 0.64f, 1f, 0.22f), 128, 2f);
            var glowRect = glowGo.GetComponent<RectTransform>();
            glowRect.anchorMin = new Vector2(-0.08f, -0.3f);
            glowRect.anchorMax = new Vector2(1.08f, 1.3f);
            glowRect.offsetMin = Vector2.zero;
            glowRect.offsetMax = Vector2.zero;

            var text = MakeText(buttonGo.transform, label, 18, TextAnchor.MiddleCenter);
            text.color = VisualTheme.TextPrimary;
            var rect = text.rectTransform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            return button;
        }
    }
}
