using UnityEngine;

namespace BlockBlast.Data
{
    /// <summary>
    /// Every tunable value of the game. The project asset lives at Resources/GameConfig.
    /// Layout is in world units where 1 unit = 1 board cell; the Figma screen (1440x2960 px,
    /// 167 px per cell) is about 8.6 x 17.7 units, centered on the origin.
    /// </summary>
    [CreateAssetMenu(menuName = "BlockBlast/Game Config", fileName = "GameConfig")]
    public sealed class GameConfig : ScriptableObject
    {
        [Header("Board")]
        public int boardSize = 8;
        public int handSize = 3;

        [Tooltip("A new hand is rerolled up to this many times until one piece fits. 1 = pure random.")]
        public int refillAttempts = 20;
        [Tooltip("Seconds the board stays visible, with unplaceable pieces greyed, before Game Over.")]
        public float gameOverDelay = 1.5f;

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

        [Header("Art")]
        public Sprite background;
        public Sprite boardFrame;
        [Tooltip("One sprite per block color; each piece gets a random one.")]
        public Sprite[] blockSprites;
        [Tooltip("Shown on hand pieces that no longer fit when the game ends.")]
        public Sprite noFitBlock;
        public Sprite crown;
        public Sprite gameOverBackground;
        public Sprite gameOverPanel;
        public Sprite replayButton;
        public Font font;
        [Range(0f, 1f)] public float previewAlpha = 0.5f;

        [Header("Text colors")]
        public Color bestColor = new Color(1f, 1f, 0f);
        public Color scoreLabelColor = new Color(0.055f, 0.75f, 0.92f);
        public Color scoreColor = new Color(0.75f, 0.86f, 0.96f);

        [Header("Layout: play screen")]
        public Vector2 designSize = new Vector2(8.62f, 17.72f);
        public float boardCenterY = 1.37f;
        [Tooltip("Frame size relative to the cell grid, so the border sits outside the cells.")]
        public float boardFrameScale = 1.002f;
        public float handY = -4.31f;
        public float handSpacing = 2.75f;
        public float handPieceScale = 0.44f;
        [Tooltip("Dragged piece floats this far above the pointer so the finger does not hide it.")]
        public float dragLift = 2f;
        public Vector2 crownPos = new Vector2(-3.06f, 7.96f);
        public Vector2 crownSize = new Vector2(0.69f, 0.56f);
        public Vector2 bestPos = new Vector2(-3.08f, 7.4f);
        public float bestTextHeight = 0.38f;
        public Vector2 scoreLabelPos = new Vector2(0f, 7.04f);
        public float scoreLabelHeight = 0.32f;
        public Vector2 scorePos = new Vector2(0f, 6.16f);
        public float scoreTextHeight = 0.7f;
        public Vector2 comboPos = new Vector2(0f, 5.5f);
        public float comboTextHeight = 0.4f;
        public Vector2 outOfSpacePos = new Vector2(0f, -6.2f);
        public float outOfSpaceTextHeight = 0.45f;

        [Header("Layout: game over")]
        public Vector2 panelPos = new Vector2(0f, 1.55f);
        public float panelWidth = 7.2f;
        public Vector2 resultLabelPos = new Vector2(0f, 0.6f);
        public float resultLabelHeight = 0.5f;
        public Vector2 resultScorePos = new Vector2(0f, -0.5f);
        public float resultScoreHeight = 1.3f;
        public Vector2 replayPos = new Vector2(0f, -3.57f);
        public float replayWidth = 2.23f;

        public int ColorCount => Mathf.Max(1, blockSprites != null ? blockSprites.Length : 0);

        /// <summary>Sprite for a color id (1-based); null when the art is not assigned.</summary>
        public Sprite BlockSprite(int colorId) =>
            blockSprites != null && colorId - 1 < blockSprites.Length ? blockSprites[colorId - 1] : null;
    }
}
