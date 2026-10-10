using UnityEngine;

namespace BlockBlast.Data
{
    /// <summary>
    /// Every tunable value of the game. Create one via Assets > Create > BlockBlast > Game Config
    /// and name it "GameConfig" inside a Resources folder, or assign it on GameController.
    /// Without an asset, these defaults are used.
    /// </summary>
    [CreateAssetMenu(menuName = "BlockBlast/Game Config", fileName = "GameConfig")]
    public sealed class GameConfig : ScriptableObject
    {
        [Header("Board")]
        public int boardSize = 8;
        public int handSize = 3;

        [Header("Score")]
        public int pointsPerCell = 1;
        public int pointsPerLine = 10;
        [Tooltip("Extra points for each line beyond the first in one placement (before combo).")]
        public int multiLineBonus = 10;
        [Tooltip("Placements in a row without a clear before combo resets to 0.")]
        public int comboBreakAfterMisses = 1;

        [Header("Shapes")]
        [Tooltip("Rows separated by '/', top row first. '#' filled, '.' empty.")]
        public string[] shapes =
        {
            "#",
            "##", "#/#",
            "###", "#/#/#",
            "####", "#/#/#/#",
            "#####", "#/#/#/#/#",
            "##/##",
            "###/###/###",
            "##/#.", "##/.#", "#./##", ".#/##",
            "#../#../###", "###/..#/..#", "..#/..#/###", "###/#../#..",
            "#./#./##", "##/.#/.#", "###/#..", "..#/###",
            ".#/.#/##", "##/#./#.", "#../###", "###/..#",
            "###/.#.", ".#./###", "#./##/#.", ".#/##/.#",
            "##./.##", ".##/##.", "#./##/.#", ".#/##/#.",
        };

        [Header("Visuals (replace with Figma assets)")]
        [Tooltip("Sprite for one cell. Empty = generated square.")]
        public Sprite cellSprite;
        public Color emptyCellColor = new Color(0.16f, 0.2f, 0.36f);
        public Color backgroundColor = new Color(0.1f, 0.13f, 0.27f);
        public Color previewValidColor = new Color(1f, 1f, 1f, 0.35f);
        public Color[] blockColors =
        {
            new Color(0.95f, 0.3f, 0.3f),
            new Color(0.98f, 0.62f, 0.2f),
            new Color(0.98f, 0.85f, 0.25f),
            new Color(0.35f, 0.8f, 0.4f),
            new Color(0.3f, 0.65f, 0.98f),
            new Color(0.6f, 0.4f, 0.95f),
        };

        [Header("Layout (world units)")]
        public float cellSize = 1f;
        public float boardCenterY = 1.5f;
        public float handY = -4.5f;
        public float handSpacing = 3f;
        public float handPieceScale = 0.5f;
        [Tooltip("Dragged piece floats this far above the pointer so the finger does not hide it.")]
        public float dragLift = 2f;

        public Color ColorOf(int colorId) => blockColors[colorId - 1];
    }
}
