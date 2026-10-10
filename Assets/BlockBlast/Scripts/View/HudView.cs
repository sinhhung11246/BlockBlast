using UnityEngine;

namespace BlockBlast.View
{
    /// <summary>Placeholder HUD drawn with IMGUI. Replace with the Figma UI later; game logic does not depend on it.</summary>
    public sealed class HudView : MonoBehaviour
    {
        public int Score;
        public int Best;
        public int Combo;
        public bool GameOver;
        public bool NewRecord;
        public System.Action OnRestart;

        GUIStyle style;

        void OnGUI()
        {
            float unit = Screen.height / 20f;
            if (style == null) style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
            style.fontSize = Mathf.RoundToInt(unit * 0.6f);
            style.normal.textColor = Color.white;

            float w = Screen.width;
            GUI.Label(new Rect(0, unit * 0.5f, w, unit), $"BEST {Best}", style);
            GUI.Label(new Rect(0, unit * 1.5f, w, unit), $"{Score}", style);
            if (Combo > 1) GUI.Label(new Rect(0, unit * 2.5f, w, unit), $"COMBO x{Combo}", style);

            if (!GameOver) return;

            GUI.Box(new Rect(0, 0, w, Screen.height), GUIContent.none);
            GUI.Label(new Rect(0, Screen.height * 0.4f, w, unit), NewRecord ? "NEW RECORD!" : "GAME OVER", style);
            var button = new Rect(w * 0.3f, Screen.height * 0.5f, w * 0.4f, unit * 1.2f);
            if (GUI.Button(button, "RESTART")) OnRestart?.Invoke();
        }
    }
}
