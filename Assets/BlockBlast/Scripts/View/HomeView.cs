using UnityEngine;

namespace BlockBlast.View
{
    /// <summary>
    /// Home screen: gradient covering the screen, then the logo / icons / event card and the buttons,
    /// all in design coordinates so wider or taller screens never shift one against the other.
    /// </summary>
    public sealed class HomeView : MonoBehaviour
    {
        [Tooltip("Adventure has no mode of its own yet, so it starts Classic too.")]
        [SerializeField] SpriteRenderer adventure;
        [SerializeField] SpriteRenderer classic, gear, tasks;
        [SerializeField] CoinBadge coins;

        public void SetCoins(int amount) => coins.Set(amount);

        public bool HitsPlay(Vector2 world) => SpriteFactory.Hits(adventure, world) || SpriteFactory.Hits(classic, world);
        public bool HitsSettings(Vector2 world) => SpriteFactory.Hits(gear, world);
        public bool HitsTasks(Vector2 world) => SpriteFactory.Hits(tasks, world);
    }
}
