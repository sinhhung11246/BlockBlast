using BlockBlast.Data;
using UnityEngine;

namespace BlockBlast.View
{
    /// <summary>
    /// Settings popup over any screen: Sound / BGM / Vibration toggles, Replay and Back to home.
    /// Only draws and hit-tests; the controller owns the saved values.
    /// </summary>
    public sealed class SettingsView : MonoBehaviour
    {
        public enum Toggle { Sound, Music, Vibration }
        public enum Action { None, Close, Replay, Home, Toggle }

        GameConfig cfg;
        SpriteRenderer panel, replay, home;
        readonly SpriteRenderer[] toggles = new SpriteRenderer[3];

        public bool IsOpen => gameObject.activeSelf;

        public void Build(GameConfig config, Camera cam)
        {
            cfg = config;
            SpriteFactory.Dim("Dim", transform, cfg.settingsDim, 50, cam);
            panel = SpriteFactory.Create("Panel", transform, cfg.settingsPanel, Vector2.zero, cfg.settingsPanelWidth, 51);
            for (int i = 0; i < toggles.Length; i++)
                toggles[i] = SpriteFactory.Create(((Toggle)i).ToString(), transform, cfg.toggleOn, cfg.togglePos[i], cfg.toggleWidth, 52);
            replay = SpriteFactory.Create("Replay", transform, cfg.settingsReplayButton, cfg.settingsReplayPos, cfg.settingsButtonWidth, 52);
            home = SpriteFactory.Create("Home", transform, cfg.settingsHomeButton, cfg.settingsHomePos, cfg.settingsButtonWidth, 52);
            gameObject.SetActive(false);
        }

        public void Show(bool visible) => gameObject.SetActive(visible);

        public void SetToggle(Toggle toggle, bool on) =>
            SpriteFactory.SetSprite(toggles[(int)toggle], on ? cfg.toggleOn : cfg.toggleOff, cfg.toggleWidth);

        /// <summary>What a tap at <paramref name="world"/> does; tapping outside the panel closes it.</summary>
        public Action Tap(Vector2 world, out Toggle toggle)
        {
            toggle = default;
            for (int i = 0; i < toggles.Length; i++)
            {
                if (!SpriteFactory.Hits(toggles[i], world)) continue;
                toggle = (Toggle)i;
                return Action.Toggle;
            }
            if (SpriteFactory.Hits(replay, world)) return Action.Replay;
            if (SpriteFactory.Hits(home, world)) return Action.Home;
            return SpriteFactory.Hits(panel, world) ? Action.None : Action.Close;
        }
    }
}
