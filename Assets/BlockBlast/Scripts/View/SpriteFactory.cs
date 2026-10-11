using UnityEngine;

namespace BlockBlast.View
{
    /// <summary>Sprite and text helpers for objects spawned at runtime (pieces, effects) and for tap tests.</summary>
    public static class SpriteFactory
    {
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

        /// <summary>Instantiates a sprite prefab (e.g. Block) at a local position, scaled to the given world width.</summary>
        public static SpriteRenderer Spawn(SpriteRenderer prefab, Transform parent, Sprite sprite, Vector2 localPos, float width, int sortingOrder)
        {
            var sr = Object.Instantiate(prefab, parent);
            sr.transform.localPosition = localPos;
            sr.sortingOrder = sortingOrder;
            SetSprite(sr, sprite, width);
            return sr;
        }

        /// <summary>Swaps the sprite and rescales so it keeps the given world width.</summary>
        public static void SetSprite(SpriteRenderer sr, Sprite sprite, float width)
        {
            sr.sprite = sprite;
            float scale = width / sprite.bounds.size.x;
            sr.transform.localScale = new Vector3(scale, scale, 1f);
        }

        /// <summary>World-space text in the game font.</summary>
        /// <param name="capHeight">Height of a capital letter in world units.</param>
        public static TextMesh Text(string name, Transform parent, Font font, string text, Vector2 localPos,
            float capHeight, Color color, int sortingOrder)
        {
            const int fontSize = 100;
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;

            var mesh = go.AddComponent<TextMesh>();
            mesh.font = font;
            mesh.fontSize = fontSize;
            mesh.characterSize = capHeight * 10f / (fontSize * 0.7f); // cap height ~0.7 em
            mesh.anchor = TextAnchor.MiddleCenter;
            mesh.alignment = TextAlignment.Center;
            mesh.color = color;
            mesh.text = text;

            var mr = go.GetComponent<MeshRenderer>();
            if (font != null) mr.sharedMaterial = font.material;
            mr.sortingOrder = sortingOrder;
            return mesh;
        }

        /// <summary>True when a world point lies inside the renderer's sprite bounds.</summary>
        public static bool Hits(SpriteRenderer sr, Vector2 world) =>
            sr.bounds.Contains(new Vector3(world.x, world.y, sr.bounds.center.z));
    }
}
