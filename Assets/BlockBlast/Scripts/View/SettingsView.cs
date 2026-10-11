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

        [SerializeField] SpriteRenderer panel, replay, home;
        [Tooltip("Sound, BGM, Vibration, in Toggle order.")]
        [SerializeField] SpriteRenderer[] toggles = new SpriteRenderer[3];
        [SerializeField] Sprite toggleOn, toggleOff;

        public bool IsOpen => gameObject.activeSelf;

        public void Show(bool visible) => gameObject.SetActive(visible);

        public void SetToggle(Toggle toggle, bool on) => toggles[(int)toggle].sprite = on ? toggleOn : toggleOff;

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
