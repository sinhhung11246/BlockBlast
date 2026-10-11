using UnityEngine;

namespace BlockBlast.View
{
    /// <summary>One Daily Tasks row: reward, description, progress bar and a Claim button (grey until done, a check once claimed).</summary>
    public sealed class TaskRowView : MonoBehaviour
    {
        /// <summary>What a row shows; filled by the controller from DailyTasks.</summary>
        public struct State
        {
            public string text;
            public int reward, progress, target;
            public bool claimed;
            public bool Done => progress >= target;
        }

        [SerializeField] TextMesh text, reward, claimText;
        [Tooltip("The fill grows from the bar's left edge, inset by fillInset on every side.")]
        [SerializeField] SpriteRenderer bar, fill;
        [SerializeField] float fillInset = 0.03f;
        [SerializeField] SpriteRenderer claim, done;
        [SerializeField] Sprite claimOn, claimOff;

        public void Set(State state)
        {
            gameObject.SetActive(true);
            text.text = state.text;
            reward.text = state.reward.ToString();

            // Row-local sizes: the bar's sprite times its scale.
            float p = state.target > 0 ? Mathf.Clamp01((float)state.progress / state.target) : 1f;
            Vector2 barSize = Vector2.Scale(bar.sprite.bounds.size, bar.transform.localScale);
            float width = barSize.x - 2f * fillInset;
            Vector2 fillSprite = fill.sprite.bounds.size;
            fill.enabled = p > 0f;
            fill.transform.localScale = new Vector3(width * p / fillSprite.x, (barSize.y - 2f * fillInset) / fillSprite.y, 1f);
            Vector3 barPos = bar.transform.localPosition;
            fill.transform.localPosition = new Vector3(barPos.x - width * 0.5f + width * p * 0.5f, barPos.y, barPos.z);

            done.enabled = state.claimed;
            claim.enabled = !state.claimed;
            claimText.gameObject.SetActive(!state.claimed);
            claim.sprite = state.Done ? claimOn : claimOff;
        }

        public void Hide() => gameObject.SetActive(false);

        public bool HitsClaim(Vector2 world) => gameObject.activeSelf && claim.enabled && SpriteFactory.Hits(claim, world);
    }
}
