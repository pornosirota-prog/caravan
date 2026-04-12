using UnityEngine;

namespace CaravanRoguelite.UI
{
    public static class VisualTheme
    {
        public static readonly Color BackgroundTop = new(0.09f, 0.13f, 0.2f, 1f);
        public static readonly Color BackgroundBottom = new(0.03f, 0.05f, 0.08f, 1f);
        public static readonly Color Accent = new(0.42f, 0.75f, 1f, 1f);
        public static readonly Color TextPrimary = new(0.89f, 0.93f, 1f, 1f);
        public static readonly Color TextDim = new(0.64f, 0.72f, 0.84f, 1f);

        public static readonly Color PanelFill = new(0.05f, 0.08f, 0.12f, 0.92f);
        public static readonly Color PanelBorder = new(0.22f, 0.34f, 0.5f, 0.98f);

        public static readonly Color RouteLine = new(0.52f, 0.68f, 0.82f, 0.78f);
        public static readonly Color RouteGlow = new(0.35f, 0.61f, 0.92f, 0.32f);

        public static readonly Color BiomeRuins = new(0.25f, 0.29f, 0.4f, 0.14f);
        public static readonly Color BiomeWilds = new(0.17f, 0.36f, 0.29f, 0.14f);
        public static readonly Color BiomeAsh = new(0.4f, 0.23f, 0.2f, 0.14f);

        public static Color ByNodeType(CaravanRoguelite.Data.NodeType type)
        {
            return type switch
            {
                CaravanRoguelite.Data.NodeType.Start => new Color(0.84f, 0.95f, 1f, 1f),
                CaravanRoguelite.Data.NodeType.Event => new Color(0.4f, 0.78f, 1f, 1f),
                CaravanRoguelite.Data.NodeType.City => new Color(0.47f, 0.9f, 0.58f, 1f),
                CaravanRoguelite.Data.NodeType.Boss => new Color(1f, 0.43f, 0.45f, 1f),
                _ => new Color(1f, 0.75f, 0.42f, 1f)
            };
        }
    }
}
