using System.Collections.Generic;
using UnityEngine;

namespace CaravanRoguelite.UI
{
    public static class ProceduralSpriteFactory
    {
        private static readonly Dictionary<string, Sprite> Cache = new();

        public static Sprite CreateCircle(Color color, int size = 64)
        {
            string key = $"circle-{ColorUtility.ToHtmlStringRGBA(color)}-{size}";
            if (Cache.TryGetValue(key, out var cached))
            {
                return cached;
            }

            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            var center = new Vector2(size / 2f, size / 2f);
            float radius = size * 0.45f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), center);
                    texture.SetPixel(x, y, distance <= radius ? color : Color.clear);
                }
            }

            texture.Apply();
            return Cache[key] = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
        }

        public static Sprite CreateSoftCircle(Color color, int size = 128, float softness = 1.6f)
        {
            string key = $"soft-circle-{ColorUtility.ToHtmlStringRGBA(color)}-{size}-{softness:F2}";
            if (Cache.TryGetValue(key, out var cached))
            {
                return cached;
            }

            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            var center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);
            float radius = size * 0.5f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), center);
                    float normalized = Mathf.Clamp01(distance / radius);
                    float alpha = Mathf.Pow(1f - normalized, softness);
                    texture.SetPixel(x, y, new Color(color.r, color.g, color.b, color.a * alpha));
                }
            }

            texture.Apply();
            return Cache[key] = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
        }

        public static Sprite CreateRect(Color color, int width = 8, int height = 8)
        {
            string key = $"rect-{ColorUtility.ToHtmlStringRGBA(color)}-{width}x{height}";
            if (Cache.TryGetValue(key, out var cached))
            {
                return cached;
            }

            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            var pixels = new Color[width * height];
            for (int i = 0; i < pixels.Length; i++) pixels[i] = color;
            texture.SetPixels(pixels);
            texture.Apply();
            return Cache[key] = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 100f);
        }

        public static Sprite CreateRoundedRect(Color fill, Color border, int size = 96, int radius = 18, int borderThickness = 5)
        {
            string key = $"rounded-{ColorUtility.ToHtmlStringRGBA(fill)}-{ColorUtility.ToHtmlStringRGBA(border)}-{size}-{radius}-{borderThickness}";
            if (Cache.TryGetValue(key, out var cached))
            {
                return cached;
            }

            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            float half = (size - 1) * 0.5f;
            float max = half - Mathf.Max(1, radius);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float dx = Mathf.Abs(x - half);
                    float dy = Mathf.Abs(y - half);
                    float qx = Mathf.Max(dx - max, 0f);
                    float qy = Mathf.Max(dy - max, 0f);
                    float distance = Mathf.Sqrt(qx * qx + qy * qy);
                    bool inside = distance <= radius;
                    if (!inside)
                    {
                        texture.SetPixel(x, y, Color.clear);
                        continue;
                    }

                    bool inner = distance <= Mathf.Max(0, radius - borderThickness) && dx <= half - borderThickness && dy <= half - borderThickness;
                    texture.SetPixel(x, y, inner ? fill : border);
                }
            }

            texture.Apply();
            var sprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f, 0, SpriteMeshType.FullRect, new Vector4(radius, radius, radius, radius));
            Cache[key] = sprite;
            return sprite;
        }

        public static Sprite CreatePolygon(int sides, Color color, int size = 72, float rotationDegrees = 0f)
        {
            string key = $"poly-{sides}-{ColorUtility.ToHtmlStringRGBA(color)}-{size}-{rotationDegrees:F2}";
            if (Cache.TryGetValue(key, out var cached))
            {
                return cached;
            }

            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
            var center = new Vector2((size - 1) * 0.5f, (size - 1) * 0.5f);
            float radius = size * 0.42f;
            float angleOffset = rotationDegrees * Mathf.Deg2Rad;
            var vertices = new Vector2[sides];
            for (int i = 0; i < sides; i++)
            {
                float angle = angleOffset + i * Mathf.PI * 2f / sides;
                vertices[i] = center + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            }

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    Vector2 p = new Vector2(x, y);
                    texture.SetPixel(x, y, IsInsidePolygon(p, vertices) ? color : Color.clear);
                }
            }

            texture.Apply();
            return Cache[key] = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
        }

        public static Sprite CreateLine(Color color, int width = 128, int height = 14)
        {
            string key = $"line-{ColorUtility.ToHtmlStringRGBA(color)}-{width}x{height}";
            if (Cache.TryGetValue(key, out var cached))
            {
                return cached;
            }

            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            float half = (height - 1) * 0.5f;
            for (int y = 0; y < height; y++)
            {
                float ny = Mathf.Abs(y - half) / Mathf.Max(half, 1f);
                float alpha = Mathf.Clamp01(1f - ny * ny);
                for (int x = 0; x < width; x++)
                {
                    texture.SetPixel(x, y, new Color(color.r, color.g, color.b, color.a * alpha));
                }
            }

            texture.Apply();
            return Cache[key] = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 100f);
        }

        public static Sprite CreateProceduralBackdrop(Color top, Color bottom, Color accent, int width = 512, int height = 512, int seed = 0)
        {
            string key = $"backdrop-{ColorUtility.ToHtmlStringRGBA(top)}-{ColorUtility.ToHtmlStringRGBA(bottom)}-{ColorUtility.ToHtmlStringRGBA(accent)}-{width}x{height}-{seed}";
            if (Cache.TryGetValue(key, out var cached))
            {
                return cached;
            }

            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            float shiftX = (seed % 97) * 0.13f;
            float shiftY = (seed % 43) * 0.19f;

            for (int y = 0; y < height; y++)
            {
                float v = y / (height - 1f);
                for (int x = 0; x < width; x++)
                {
                    float u = x / (width - 1f);
                    float noise = Mathf.PerlinNoise((u * 3.6f) + shiftX, (v * 4.1f) + shiftY);
                    float dune = Mathf.Sin((u + noise * 0.25f) * 12f) * 0.04f;
                    Color baseColor = Color.Lerp(bottom, top, Mathf.Clamp01(v + dune));
                    float stars = Mathf.PerlinNoise((u * 26f) + shiftY, (v * 25f) + shiftX);
                    if (stars > 0.82f)
                    {
                        baseColor = Color.Lerp(baseColor, accent, (stars - 0.82f) * 1.6f);
                    }

                    float vignetteX = Mathf.Abs(u * 2f - 1f);
                    float vignetteY = Mathf.Abs(v * 2f - 1f);
                    float vignette = Mathf.Clamp01(1f - (vignetteX * vignetteX + vignetteY * vignetteY) * 0.45f);
                    texture.SetPixel(x, y, baseColor * vignette);
                }
            }

            texture.Apply();
            return Cache[key] = Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0.5f), 100f);
        }

        private static bool IsInsidePolygon(Vector2 point, Vector2[] polygon)
        {
            bool inside = false;
            for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
            {
                bool intersects = ((polygon[i].y > point.y) != (polygon[j].y > point.y)) &&
                                  (point.x < (polygon[j].x - polygon[i].x) * (point.y - polygon[i].y) / (polygon[j].y - polygon[i].y + 0.001f) + polygon[i].x);
                if (intersects)
                {
                    inside = !inside;
                }
            }

            return inside;
        }
    }
}
