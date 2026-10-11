using BlockBlast.Data;
using UnityEngine;

namespace BlockBlast.View
{
    /// <summary>"Continue?" popup after the board runs out of space: pay coins, watch an ad, or close.</summary>
    public sealed class ReviveView : MonoBehaviour
    {
        public enum Action { None, Coins, Ad, Decline }

        GameConfig cfg;
        Transform popup;
        SpriteRenderer coinButton, adButton;
        CoinBadge coins;
        GameObject adOverlay;

        public bool IsOpen => gameObject.activeSelf;

        public void Build(GameConfig config, Camera cam)
        {
            cfg = config;
            SpriteFactory.Dim("Dim", transform, cfg.settingsDim, 60, cam);
            popup = new GameObject("Popup").transform;
            popup.SetParent(transform, false);
            popup.localPosition = cfg.revivePos;
            SpriteFactory.Create("Panel", popup, cfg.revivePanel, Vector2.zero, cfg.revivePanelWidth, 61);
            coins = CoinBadge.Create(popup, cfg, cfg.reviveCoinsPos, 62);
            Text(popup, "Continue?", cfg.reviveTitle, 62);

            coinButton = SpriteFactory.Create("CoinButton", popup, cfg.coinButton, cfg.reviveCoinButton.pos, cfg.reviveCoinButton.size, 62);
            Text(coinButton.transform, cfg.reviveCost.ToString(), cfg.revivePrice, 63);

            adButton = SpriteFactory.Create("AdButton", popup, cfg.adButton, cfg.reviveAdButton.pos, cfg.reviveAdButton.size, 62);
            Child(adButton.transform, "Video", cfg.videoIcon, cfg.reviveVideoIcon, 63);
            Text(adButton.transform, "Free", cfg.reviveAdText, 63);

            adOverlay = new GameObject("AdOverlay");
            adOverlay.transform.SetParent(transform, false);
            SpriteFactory.Dim("Dim", adOverlay.transform, Color.black, 70, cam);
            SpriteFactory.Text("Label", adOverlay.transform, cfg.font, "Ad playing...", Vector2.zero, cfg.reviveTitle.size, cfg.popupTextColor, 71);

            gameObject.SetActive(false);
        }

        public void Show(bool visible, int coinCount = 0)
        {
            gameObject.SetActive(visible);
            if (!visible) return;
            coins.Set(coinCount);
            SetAlpha(coinButton.transform, coinCount >= cfg.reviveCost ? 1f : cfg.disabledAlpha);
            ShowAd(false);
        }

        public void ShowAd(bool playing) => adOverlay.SetActive(playing);

        public Action Tap(Vector2 world)
        {
            if (adOverlay.activeSelf) return Action.None;
            if (SpriteFactory.Hits(coinButton, world)) return Action.Coins;
            if (SpriteFactory.Hits(adButton, world)) return Action.Ad;
            Vector2 close = (Vector2)popup.position + cfg.reviveClose.pos;
            return Vector2.Distance(world, close) <= cfg.reviveClose.size ? Action.Decline : Action.None;
        }

        /// <summary>Children of a scaled button are placed in world units relative to the button center.</summary>
        void Text(Transform parent, string text, Place place, int order)
        {
            var mesh = SpriteFactory.Text(text, popup, cfg.font, text, Vector2.zero, place.size, cfg.popupTextColor, order);
            mesh.transform.position = parent.position + (Vector3)place.pos;
            mesh.transform.SetParent(parent, true);
        }

        void Child(Transform parent, string name, Sprite sprite, Place place, int order)
        {
            var sr = SpriteFactory.Create(name, popup, sprite, Vector2.zero, place.size, order);
            sr.transform.position = parent.position + (Vector3)place.pos;
            sr.transform.SetParent(parent, true);
        }

        static void SetAlpha(Transform root, float alpha)
        {
            foreach (var sr in root.GetComponentsInChildren<SpriteRenderer>()) sr.color = new Color(1f, 1f, 1f, alpha);
            foreach (var text in root.GetComponentsInChildren<TextMesh>()) text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
        }
    }
}
