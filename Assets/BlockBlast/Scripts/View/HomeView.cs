using BlockBlast.Data;
using UnityEngine;

namespace BlockBlast.View
{
    /// <summary>
    /// Home screen: gradient covering the screen, then the logo / icons / event card and the buttons,
    /// both in design coordinates so wider or taller screens never shift one against the other.
    /// </summary>
    public sealed class HomeView : MonoBehaviour
    {
        SpriteRenderer adventure, classic, gear, tasks;
        CoinBadge coins;

        public void Build(GameConfig cfg, Camera cam)
        {
            var bg = SpriteFactory.Create("Background", transform, cfg.homeBackground, Vector2.zero, 1f, 40);
            SpriteFactory.Cover(bg, cam);
            SpriteFactory.Create("Art", transform, cfg.homeArt, Vector2.zero, cfg.homeArtWidth, 41);
            adventure = SpriteFactory.Create("Adventure", transform, cfg.adventureButton, cfg.adventurePos, cfg.modeButtonWidth, 42);
            classic = SpriteFactory.Create("Classic", transform, cfg.classicButton, cfg.classicPos, cfg.modeButtonWidth, 42);
            gear = SpriteFactory.Create("Settings", transform, cfg.homeGear, cfg.homeGearPos, cfg.homeGearWidth, 42);
            tasks = SpriteFactory.Create("Tasks", transform, cfg.homeTasksButton, cfg.homeTasks.pos, cfg.homeTasks.size, 42);
            coins = CoinBadge.Create(transform, cfg, cfg.homeCoinsPos, 42);
            gameObject.SetActive(false);
        }

        public void Show(bool visible) => gameObject.SetActive(visible);
        public void SetCoins(int amount) => coins.Set(amount);

        public bool HitsPlay(Vector2 world) => SpriteFactory.Hits(adventure, world) || SpriteFactory.Hits(classic, world);
        public bool HitsSettings(Vector2 world) => SpriteFactory.Hits(gear, world);
        public bool HitsTasks(Vector2 world) => SpriteFactory.Hits(tasks, world);
    }
}
