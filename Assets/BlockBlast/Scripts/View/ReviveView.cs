using UnityEngine;

namespace BlockBlast.View
{
    /// <summary>"Continue?" popup after the board runs out of space: pay coins, watch an ad, or close.</summary>
    public sealed class ReviveView : MonoBehaviour
    {
        public enum Action { None, Coins, Ad, Decline }

        [Tooltip("Button backgrounds; their parent holds the button's icon and text.")]
        [SerializeField] SpriteRenderer coinButton, adButton;
        [SerializeField] TextMesh price;
        [SerializeField] CoinBadge coins;
        [Tooltip("Center of the X baked into the panel art.")]
        [SerializeField] Transform close;
        [SerializeField] float closeRadius = 0.4f;
        [SerializeField] GameObject adOverlay;
        [Tooltip("Alpha of the coin button when the player cannot afford it.")]
        [Range(0f, 1f)] [SerializeField] float disabledAlpha = 0.45f;

        public void Show(int coinCount, int cost)
        {
            gameObject.SetActive(true);
            coins.Set(coinCount);
            price.text = cost.ToString();
            SetAlpha(coinButton.transform.parent, coinCount >= cost ? 1f : disabledAlpha);
            ShowAd(false);
        }

        public void Hide() => gameObject.SetActive(false);

        public void ShowAd(bool playing) => adOverlay.SetActive(playing);

        public Action Tap(Vector2 world)
        {
            if (adOverlay.activeSelf) return Action.None;
            if (SpriteFactory.Hits(coinButton, world)) return Action.Coins;
            if (SpriteFactory.Hits(adButton, world)) return Action.Ad;
            return Vector2.Distance(world, close.position) <= closeRadius ? Action.Decline : Action.None;
        }

        static void SetAlpha(Transform root, float alpha)
        {
            foreach (var sr in root.GetComponentsInChildren<SpriteRenderer>()) sr.color = new Color(1f, 1f, 1f, alpha);
            foreach (var text in root.GetComponentsInChildren<TextMesh>()) text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
        }
    }
}
