using BlockBlast.Data;
using UnityEngine;

namespace BlockBlast.View
{
    /// <summary>Creates cell renderers. Uses GameConfig.cellSprite, or a generated square as placeholder.</summary>
    public static class CellSprite
    {
        static Sprite placeholder;

        public static SpriteRenderer Create(Transform parent, GameConfig cfg, Vector3 localPos, int sortingOrder)
        {
            var go = new GameObject("Cell");
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = cfg.cellSprite != null ? cfg.cellSprite : Placeholder();
            sr.sortingOrder = sortingOrder;

            // Fit any sprite to exactly one cell.
            Vector2 size = sr.sprite.bounds.size;
            go.transform.localScale = new Vector3(cfg.cellSize / size.x, cfg.cellSize / size.y, 1f);
            return sr;
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
