using BlockBlast.View;
using UnityEngine;

namespace BlockBlast.Game
{
    /// <summary>Saved on/off settings, all on by default. Sound and BGM are read by audio once the game has it.</summary>
    public static class SettingsStore
    {
        public static bool Get(SettingsView.Toggle toggle) => PlayerPrefs.GetInt(Key(toggle), 1) == 1;

        public static void Set(SettingsView.Toggle toggle, bool on)
        {
            PlayerPrefs.SetInt(Key(toggle), on ? 1 : 0);
            PlayerPrefs.Save();
        }

        /// <summary>Short buzz on phones when Vibration is on.</summary>
        public static void Vibrate()
        {
#if UNITY_ANDROID || UNITY_IOS
            if (Get(SettingsView.Toggle.Vibration)) Handheld.Vibrate();
#endif
        }

        static string Key(SettingsView.Toggle toggle) => "BlockBlast." + toggle;
    }
}
