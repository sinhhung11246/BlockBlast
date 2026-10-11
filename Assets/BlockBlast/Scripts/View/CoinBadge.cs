using BlockBlast.Data;
using UnityEngine;

namespace BlockBlast.View
{
    /// <summary>Purple pill with a coin and the player's coin count.</summary>
    public sealed class CoinBadge : MonoBehaviour
    {
        TextMesh amount;

        public static CoinBadge Create(Transform parent, GameConfig cfg, Vector2 localPos, int sortingOrder)
        {
            var badge = new GameObject("Coins").AddComponent<CoinBadge>();
            badge.transform.SetParent(parent, false);
            badge.transform.localPosition = localPos;
            SpriteFactory.Create("Pill", badge.transform, cfg.coinPill, cfg.badgePill.pos, cfg.badgePill.size, sortingOrder);
            SpriteFactory.Create("Coin", badge.transform, cfg.coin, cfg.badgeCoin.pos, cfg.badgeCoin.size, sortingOrder + 1);
            badge.amount = SpriteFactory.Text("Amount", badge.transform, cfg.font, "0", cfg.badgeText.pos, cfg.badgeText.size,
                cfg.popupTextColor, sortingOrder + 1);
            return badge;
        }

        public void Set(int coins) => amount.text = coins.ToString();
        public void Set(string text) => amount.text = text;
    }
}
