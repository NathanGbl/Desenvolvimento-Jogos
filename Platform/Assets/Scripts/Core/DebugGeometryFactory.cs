using System.Collections.Generic;
using UnityEngine;

namespace OCaminhoDoPeregrino.Core
{
    public static class DebugGeometryFactory
    {
        private static readonly Dictionary<string, Sprite> SpriteCache = new();

        public static Sprite CreateSquare(Color color)
        {
            return GetSprite($"square_{ColorToKey(color)}", texture => CreateFilledTexture(texture, color));
        }

        public static Sprite CreateCircle(Color color)
        {
            return GetSprite($"circle_{ColorToKey(color)}", texture => CreateCircleTexture(texture, color));
        }

        public static Sprite CreateDiamond(Color color)
        {
            return GetSprite($"diamond_{ColorToKey(color)}", texture => CreateDiamondTexture(texture, color));
        }

        public static Sprite CreateRing(Color color)
        {
            return GetSprite($"ring_{ColorToKey(color)}", texture => CreateRingTexture(texture, color));
        }

        private static Sprite GetSprite(string key, System.Func<Texture2D, Texture2D> builder)
        {
            if (SpriteCache.TryGetValue(key, out Sprite cachedSprite))
            {
                return cachedSprite;
            }

            Texture2D texture = new Texture2D(64, 64, TextureFormat.RGBA32, false);
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Point;
            texture = builder(texture);
            texture.Apply();

            Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 32f);
            SpriteCache[key] = sprite;
            return sprite;
        }

        private static Texture2D CreateFilledTexture(Texture2D texture, Color color)
        {
            Color32[] pixels = new Color32[texture.width * texture.height];
            Color32 pixelColor = color;

            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = pixelColor;
            }

            texture.SetPixels32(pixels);
            return texture;
        }

        private static Texture2D CreateCircleTexture(Texture2D texture, Color color)
        {
            Color32[] pixels = new Color32[texture.width * texture.height];
            float radius = texture.width * 0.5f - 2f;
            Vector2 center = new Vector2(texture.width * 0.5f, texture.height * 0.5f);
            Color32 fill = color;
            Color32 transparent = new Color32(0, 0, 0, 0);

            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), center);
                    pixels[y * texture.width + x] = distance <= radius ? fill : transparent;
                }
            }

            texture.SetPixels32(pixels);
            return texture;
        }

        private static Texture2D CreateDiamondTexture(Texture2D texture, Color color)
        {
            Color32[] pixels = new Color32[texture.width * texture.height];
            Vector2 center = new Vector2(texture.width * 0.5f, texture.height * 0.5f);
            float maxDistance = texture.width * 0.45f;
            Color32 fill = color;
            Color32 transparent = new Color32(0, 0, 0, 0);

            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    float dx = Mathf.Abs(x + 0.5f - center.x);
                    float dy = Mathf.Abs(y + 0.5f - center.y);
                    pixels[y * texture.width + x] = dx + dy <= maxDistance ? fill : transparent;
                }
            }

            texture.SetPixels32(pixels);
            return texture;
        }

        private static Texture2D CreateRingTexture(Texture2D texture, Color color)
        {
            Color32[] pixels = new Color32[texture.width * texture.height];
            Vector2 center = new Vector2(texture.width * 0.5f, texture.height * 0.5f);
            float outerRadius = texture.width * 0.45f;
            float innerRadius = texture.width * 0.26f;
            Color32 fill = color;
            Color32 transparent = new Color32(0, 0, 0, 0);

            for (int y = 0; y < texture.height; y++)
            {
                for (int x = 0; x < texture.width; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), center);
                    pixels[y * texture.width + x] = distance <= outerRadius && distance >= innerRadius ? fill : transparent;
                }
            }

            texture.SetPixels32(pixels);
            return texture;
        }

        private static string ColorToKey(Color color)
        {
            return $"{Mathf.RoundToInt(color.r * 255f)}_{Mathf.RoundToInt(color.g * 255f)}_{Mathf.RoundToInt(color.b * 255f)}_{Mathf.RoundToInt(color.a * 255f)}";
        }
    }
}
