using BlockBlast.Data;
using UnityEngine;

namespace BlockBlast.View
{
    /// <summary>Game Over screen: background, panel and replay button. Score text is drawn by HudView.</summary>
    public sealed class GameOverView : MonoBehaviour
    {
        SpriteRenderer replay;

        public void Build(GameConfig cfg, Camera cam)
        {
            var bg = SpriteFactory.Create("Background", transform, cfg.gameOverBackground, Vector2.zero, 1f, 20);
            SpriteFactory.Cover(bg, cam);
            SpriteFactory.Create("Panel", transform, cfg.gameOverPanel, cfg.panelPos, cfg.panelWidth, 21);
            replay = SpriteFactory.Create("Replay", transform, cfg.replayButton, cfg.replayPos, cfg.replayWidth, 22);
            gameObject.SetActive(false);
        }

        public void Show(bool visible) => gameObject.SetActive(visible);

        public bool HitsReplay(Vector2 world) => replay.bounds.Contains(new Vector3(world.x, world.y, replay.bounds.center.z));
    }
}
