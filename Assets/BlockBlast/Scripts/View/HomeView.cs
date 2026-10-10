using BlockBlast.Data;
using UnityEngine;

namespace BlockBlast.View
{
    /// <summary>Home screen: background with the mode buttons and the settings gear on top.</summary>
    public sealed class HomeView : MonoBehaviour
    {
        SpriteRenderer adventure, classic, gear;

        public void Build(GameConfig cfg, Camera cam)
        {
            var bg = SpriteFactory.Create("Background", transform, cfg.homeBackground, Vector2.zero, 1f, 40);
            SpriteFactory.Cover(bg, cam);
            adventure = SpriteFactory.Create("Adventure", transform, cfg.adventureButton, cfg.adventurePos, cfg.modeButtonWidth, 41);
            classic = SpriteFactory.Create("Classic", transform, cfg.classicButton, cfg.classicPos, cfg.modeButtonWidth, 41);
            gear = SpriteFactory.Create("Settings", transform, cfg.homeGear, cfg.homeGearPos, cfg.homeGearWidth, 41);
            gameObject.SetActive(false);
        }

        public void Show(bool visible) => gameObject.SetActive(visible);

        public bool HitsPlay(Vector2 world) => SpriteFactory.Hits(adventure, world) || SpriteFactory.Hits(classic, world);
        public bool HitsSettings(Vector2 world) => SpriteFactory.Hits(gear, world);
    }
}
