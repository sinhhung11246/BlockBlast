using BlockBlast.Data;
using UnityEngine;

namespace BlockBlast.View
{
    /// <summary>Game Over screen: background, title, panel, home and replay buttons. Score text is drawn by HudView.</summary>
    public sealed class GameOverView : MonoBehaviour
    {
        SpriteRenderer replay, home;

        public void Build(GameConfig cfg, Camera cam)
        {
            var bg = SpriteFactory.Create("Background", transform, cfg.gameOverBackground, Vector2.zero, 1f, 20);
            SpriteFactory.Cover(bg, cam);
            SpriteFactory.Create("Title", transform, cfg.gameOverTitle, cfg.titlePos, cfg.titleWidth, 21);
            SpriteFactory.Create("Panel", transform, cfg.gameOverPanel, cfg.panelPos, cfg.panelWidth, 21);
            replay = SpriteFactory.Create("Replay", transform, cfg.replayButton, cfg.replayPos, cfg.replayWidth, 22);
            home = SpriteFactory.Create("Home", transform, cfg.homeButton, cfg.homePos, cfg.homeWidth, 22);
            gameObject.SetActive(false);
        }

        public void Show(bool visible) => gameObject.SetActive(visible);

        public bool HitsReplay(Vector2 world) => SpriteFactory.Hits(replay, world);
        public bool HitsHome(Vector2 world) => SpriteFactory.Hits(home, world);
    }
}
