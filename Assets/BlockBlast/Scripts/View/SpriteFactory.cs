using UnityEngine;

namespace BlockBlast.View
{
    /// <summary>Creates sprite renderers sized in world units. A missing sprite falls back to a white square.</summary>
    public static class SpriteFactory
    {
        static Sprite placeholder, pixel;

        /// <param name="width">World width; height follows the sprite's aspect ratio.</param>
        public static SpriteRenderer Create(string name, Transform parent, Sprite sprite, Vector2 localPos, float width, int sortingOrder)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sortingOrder = sortingOrder;
            SetSprite(sr, sprite, width);
            return sr;
        }

        /// <summary>Swaps the sprite and rescales so it keeps the given world width.</summary>
        public static void SetSprite(SpriteRenderer sr, Sprite sprite, float width)
        {
            sr.sprite = sprite != null ? sprite : Placeholder();
            float scale = width / sr.sprite.bounds.size.x;
            sr.transform.localScale = new Vector3(scale, scale, 1f);
        }

        /// <summary>Scales a renderer so its sprite covers the whole camera view.</summary>
        public static void Cover(SpriteRenderer sr, Camera cam)
        {
            Vector2 view = new Vector2(cam.orthographicSize * 2f * cam.aspect, cam.orthographicSize * 2f);
            Vector2 size = sr.sprite.bounds.size;
            float scale = Mathf.Max(view.x / size.x, view.y / size.y);
            sr.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, 0f);
            sr.transform.localScale = new Vector3(scale, scale, 1f);
        }

        /// <summary>Full-screen tinted quad, e.g. to dim what is behind a popup.</summary>
        public static SpriteRenderer Dim(string name, Transform parent, Color color, int sortingOrder, Camera cam)
        {
            var sr = Create(name, parent, Pixel(), Vector2.zero, 1f, sortingOrder);
            sr.color = color;
            Cover(sr, cam);
            return sr;
        }

        /// <summary>True when a world point lies inside the renderer's sprite bounds.</summary>
        public static bool Hits(SpriteRenderer sr, Vector2 world) =>
            sr.bounds.Contains(new Vector3(world.x, world.y, sr.bounds.center.z));

        static Sprite Pixel()
        {
            if (pixel != null) return pixel;
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            pixel = Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1);
            return pixel;
        }

        static Sprite Placeholder()
        {
            if (placeholder != null) return placeholder;

            const int size = 32, gap = 2;
            var tex = new Texture2D(size, size) { filterMode = FilterMode.Point };
            var pixels = new Color32[size * size];
            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                {
                    bool inside = x >= gap && y >= gap && x < size - gap && y < size - gap;
                    pixels[y * size + x] = inside ? new Color32(255, 255, 255, 255) : new Color32(0, 0, 0, 0);
                }
            tex.SetPixels32(pixels);
            tex.Apply();

            placeholder = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
            return placeholder;
        }
    }
}
