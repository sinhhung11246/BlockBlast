using BlockBlast.Data;
using BlockBlast.View;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BlockBlast.Game
{
    /// <summary>
    /// Base of the Home and Game scene controllers: the shared config, the pointer in world space and the
    /// Settings popup, which takes every tap while it is open.
    /// </summary>
    public abstract class ScreenController : MonoBehaviour
    {
        protected const string HomeScene = "Home", GameScene = "Game";

        [SerializeField] protected GameConfig config;
        [SerializeField] SettingsView settingsView;

        protected Camera cam;

        protected virtual void Awake() => cam = Camera.main;

        protected void Update()
        {
            Vector2 pointer = PointerInput.WorldPosition(cam);
            if (settingsView.IsOpen)
            {
                if (PointerInput.Pressed) TapSettings(pointer);
                return;
            }
            Tick(pointer);
        }

        /// <summary>Per-frame input and state while no Settings popup is open.</summary>
        protected abstract void Tick(Vector2 pointer);

        /// <summary>Settings "Play" row.</summary>
        protected abstract void Replay();

        /// <summary>Settings "Back" row.</summary>
        protected abstract void GoHome();

        protected static void Load(string scene) => SceneManager.LoadScene(scene);

        protected void OpenSettings()
        {
            foreach (SettingsView.Toggle t in System.Enum.GetValues(typeof(SettingsView.Toggle)))
                settingsView.SetToggle(t, SettingsStore.Get(t));
            settingsView.Show(true);
        }

        void TapSettings(Vector2 pointer)
        {
            switch (settingsView.Tap(pointer, out SettingsView.Toggle toggle))
            {
                case SettingsView.Action.Toggle:
                    bool on = !SettingsStore.Get(toggle);
                    SettingsStore.Set(toggle, on);
                    settingsView.SetToggle(toggle, on);
                    return;
                case SettingsView.Action.Close:
                    settingsView.Show(false);
                    return;
                case SettingsView.Action.Replay:
                    settingsView.Show(false);
                    Replay();
                    return;
                case SettingsView.Action.Home:
                    settingsView.Show(false);
                    GoHome();
                    return;
            }
        }
    }
}
