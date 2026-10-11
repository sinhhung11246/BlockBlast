using System;
using System.Collections;
using System.Collections.Generic;
using BlockBlast.Data;
using UnityEngine;

namespace BlockBlast.View
{
    /// <summary>
    /// Fire-and-forget effects drawn over the board: placement pop, line clear burst with shards,
    /// praise word, Combo popup and camera shake. Every effect spawns its own sprites from the Block and Praise prefabs,
    /// so BoardView stays untouched.
    /// </summary>
    public sealed class VfxView : MonoBehaviour
    {
        const int PopOrder = 2, ClearOrder = 3, ShardOrder = 6, ComboOrder = 15;

        [SerializeField] SpriteRenderer block;
        [Tooltip("Root that pops and fades; its SpriteRenderer child shows the word.")]
        [SerializeField] Transform praise;

        GameConfig cfg;
        Camera cam;
        Vector3 camHome;
        Coroutine shake;

        public void Init(GameConfig config, Camera camera)
        {
            cfg = config;
            cam = camera;
            camHome = cam.transform.position;
        }

        /// <summary>Removes every running effect, e.g. on restart.</summary>
        public void Clear()
        {
            StopAllCoroutines();
            shake = null;
            cam.transform.position = camHome;
            for (int i = transform.childCount - 1; i >= 0; i--) Destroy(transform.GetChild(i).gameObject);
        }

        /// <summary>Placed cells start a bit larger and settle into the grid.</summary>
        public void Place(IEnumerable<Vector2> cells, Sprite sprite)
        {
            foreach (var pos in cells)
            {
                var sr = SpriteFactory.Spawn(block, transform, sprite, pos, 1f, PopOrder);
                Vector3 baseScale = sr.transform.localScale;
                StartCoroutine(Tween(cfg.placePopTime, 0f, t =>
                    sr.transform.localScale = baseScale * Mathf.Lerp(cfg.placePopScale, 1f, EaseOut(t)), sr.gameObject));
            }
        }

        /// <summary>Cleared cells swell and fade, bursting outward from <paramref name="from"/>, and throw shards.</summary>
        public void ClearCells(IEnumerable<(Vector2 pos, Sprite sprite)> cells, Vector2 from, int lineCount)
        {
            foreach (var (pos, sprite) in cells)
            {
                float delay = Vector2.Distance(pos, from) * cfg.clearStagger;
                var sr = SpriteFactory.Spawn(block, transform, sprite, pos, 1f, ClearOrder);
                Vector3 baseScale = sr.transform.localScale;
                StartCoroutine(Tween(cfg.clearTime, delay, t =>
                {
                    sr.transform.localScale = baseScale * Mathf.Lerp(1f, cfg.clearScale, EaseOut(t));
                    sr.color = new Color(1f, 1f, 1f, 1f - t);
                }, sr.gameObject));

                for (int i = 0; i < cfg.shardsPerCell; i++) StartCoroutine(Shard(pos, sprite, delay));
            }

            if (lineCount > 1) Shake(cfg.shakePerExtraLine * (lineCount - 1));
        }

        /// <summary>"Combo" + number, popping in with overshoot, then rising and fading.</summary>
        public void Combo(int combo)
        {
            var root = new GameObject("Combo").transform;
            root.SetParent(transform, false);

            var parts = new List<Transform>();
            parts.Add(SpriteFactory.Create("Label", root, cfg.comboLabel, Vector2.zero, Width(cfg.comboLabel, cfg.comboLabelHeight), ComboOrder).transform);
            parts.AddRange(Number(combo, root));
            Layout(parts);
            PopUp(root, cfg.comboPopupPos);
        }

        /// <summary>Praise word for the lines cleared by one placement: Good, Great, Excellent...</summary>
        public void Praise(int lineCount)
        {
            if (cfg.praiseSprites == null || cfg.praiseSprites.Length == 0) return;
            Sprite sprite = cfg.praiseSprites[Mathf.Min(lineCount, cfg.praiseSprites.Length) - 1];
            if (sprite == null) return;

            var root = Instantiate(praise, transform);
            var word = root.GetComponentInChildren<SpriteRenderer>();
            SpriteFactory.SetSprite(word, sprite, sprite.bounds.size.x * cfg.praiseScale);
            PopUp(root, cfg.praisePos);
        }

        /// <summary>Pops in with overshoot at <paramref name="pos"/>, then rises and fades.</summary>
        void PopUp(Transform root, Vector2 pos)
        {
            root.localPosition = pos;
            var renderers = root.GetComponentsInChildren<Renderer>();
            StartCoroutine(Tween(cfg.comboPopupTime, 0f, t =>
            {
                const float popEnd = 0.2f, fadeStart = 0.6f;
                float scale = t < popEnd ? EaseOutBack(t / popEnd) : 1f;
                float alpha = t < fadeStart ? 1f : 1f - (t - fadeStart) / (1f - fadeStart);
                root.localScale = Vector3.one * scale;
                root.localPosition = pos + Vector2.up * (cfg.comboRise * t);
                foreach (var r in renderers) SetAlpha(r, alpha);
            }, root.gameObject));
        }

        void Shake(float amount)
        {
            if (shake != null) StopCoroutine(shake);
            shake = StartCoroutine(Tween(cfg.shakeTime, 0f, t =>
                cam.transform.position = camHome + (Vector3)(UnityEngine.Random.insideUnitCircle * amount * (1f - t)), null));
        }

        IEnumerator Shard(Vector2 pos, Sprite sprite, float delay)
        {
            yield return new WaitForSeconds(delay);

            var sr = SpriteFactory.Spawn(block, transform, sprite, pos, cfg.shardSize, ShardOrder);
            Vector2 velocity = (Vector2.up + UnityEngine.Random.insideUnitCircle) * cfg.shardSpeed * UnityEngine.Random.Range(0.5f, 1f);
            float spin = UnityEngine.Random.Range(-720f, 720f);

            for (float time = 0f; time < cfg.shardTime; time += Time.deltaTime)
            {
                velocity.y -= cfg.shardGravity * Time.deltaTime;
                sr.transform.localPosition += (Vector3)(velocity * Time.deltaTime);
                sr.transform.Rotate(0f, 0f, spin * Time.deltaTime);
                sr.color = new Color(1f, 1f, 1f, 1f - time / cfg.shardTime);
                yield return null;
            }
            Destroy(sr.gameObject);
        }

        /// <summary>Digit sprites when every digit has art, otherwise one text mesh in the font.</summary>
        IEnumerable<Transform> Number(int value, Transform root)
        {
            string digits = value.ToString();
            bool allArt = true;
            foreach (char c in digits) allArt &= cfg.ComboDigit(c - '0') != null;

            if (allArt)
            {
                foreach (char c in digits)
                {
                    Sprite s = cfg.ComboDigit(c - '0');
                    yield return SpriteFactory.Create("Digit", root, s, Vector2.zero, Width(s, cfg.comboDigitHeight), ComboOrder).transform;
                }
                yield break;
            }

            yield return SpriteFactory.Text("Number", root, cfg.font, digits, Vector2.zero, cfg.comboDigitHeight, cfg.comboNumberColor, ComboOrder).transform;
        }

        /// <summary>Lays parts left to right, centered on the root.</summary>
        void Layout(List<Transform> parts)
        {
            var widths = new float[parts.Count];
            float total = cfg.comboSpacing * (parts.Count - 1);
            for (int i = 0; i < parts.Count; i++) total += widths[i] = parts[i].GetComponent<Renderer>().bounds.size.x;

            float x = -total * 0.5f;
            for (int i = 0; i < parts.Count; i++)
            {
                parts[i].localPosition = new Vector2(x + widths[i] * 0.5f, 0f);
                x += widths[i] + cfg.comboSpacing;
            }
        }

        /// <summary>Runs step(0..1) over the duration after a delay, then destroys the owner.</summary>
        IEnumerator Tween(float duration, float delay, Action<float> step, GameObject owner)
        {
            if (delay > 0f) yield return new WaitForSeconds(delay);
            for (float time = 0f; time < duration; time += Time.deltaTime)
            {
                step(time / duration);
                yield return null;
            }
            step(1f);
            if (owner != null) Destroy(owner);
        }

        static float Width(Sprite sprite, float height) =>
            sprite != null ? height * sprite.bounds.size.x / sprite.bounds.size.y : height;

        static void SetAlpha(Renderer r, float alpha)
        {
            if (r is SpriteRenderer sr) sr.color = new Color(1f, 1f, 1f, alpha);
            else if (r.TryGetComponent(out TextMesh text)) text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
        }

        static float EaseOut(float t) => 1f - (1f - t) * (1f - t);

        static float EaseOutBack(float t)
        {
            const float c = 1.7f;
            t -= 1f;
            return 1f + (c + 1f) * t * t * t + c * t * t;
        }
    }
}
