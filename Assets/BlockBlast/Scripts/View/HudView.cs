using BlockBlast.Data;
using UnityEngine;

namespace BlockBlast.View
{
    /// <summary>
    /// Score texts, placed at world positions from GameConfig and drawn with IMGUI on top of the sprites.
    /// Reads only the public fields the controller sets.
    /// </summary>
    public sealed class HudView : MonoBehaviour
    {
        public int Score;
        public int Best;
        public int Combo;
        public bool GameOver;
        public bool NewRecord;
        public bool OutOfSpace;

        GameConfig cfg;
        Camera cam;
        GUIStyle style;

        public void Init(GameConfig config, Camera camera)
        {
            cfg = config;
            cam = camera;
            SpriteFactory.Create("Crown", transform, cfg.crown, cfg.crownPos, cfg.crownSize.x, 2);
        }

        void OnGUI()
        {
            if (cfg == null) return;
            if (style == null) style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, font = cfg.font };

            if (GameOver)
            {
                Text(NewRecord ? "NEW BEST SCORE" : "SCORE", cfg.resultLabelPos, cfg.resultLabelHeight, cfg.scoreLabelColor);
                Text(Score.ToString(), cfg.resultScorePos, cfg.resultScoreHeight, cfg.scoreColor);
                return;
            }

            Text(Best.ToString(), cfg.bestPos, cfg.bestTextHeight, cfg.bestColor);
            Text("SCORE", cfg.scoreLabelPos, cfg.scoreLabelHeight, cfg.scoreLabelColor);
            Text(Score.ToString(), cfg.scorePos, cfg.scoreTextHeight, cfg.scoreColor);
            if (Combo > 1) Text($"COMBO x{Combo}", cfg.comboPos, cfg.comboTextHeight, cfg.bestColor);
            if (OutOfSpace) Text("NO MORE SPACE", cfg.outOfSpacePos, cfg.outOfSpaceTextHeight, cfg.scoreColor);
        }

        /// <param name="height">Cap height in world units.</param>
        void Text(string text, Vector2 worldPos, float height, Color color)
        {
            float pixelsPerUnit = Screen.height / (cam.orthographicSize * 2f);
            Vector3 screen = cam.WorldToScreenPoint(worldPos);
            float h = height * pixelsPerUnit;

            style.fontSize = Mathf.RoundToInt(h * 1.4f); // font size ~ 1.4x cap height
            style.normal.textColor = color;
            GUI.Label(new Rect(screen.x - Screen.width, Screen.height - screen.y - h, Screen.width * 2f, h * 2f), text, style);
        }
    }
}
