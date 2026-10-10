using UnityEngine;

namespace BlockBlast.Data
{
    /// <summary>
    /// Every tunable value of the game. The project asset lives at Resources/GameConfig.
    /// Layout is in world units where 1 unit = 1 board cell; the Figma screen (1440x2960 px,
    /// 167 px per cell) is about 8.6 x 17.7 units, centered on the origin.
    /// Positions come from the Figma play / game over mockups next to "Destroy 1 block".
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
        public Sprite gameOverTitle;
        public Sprite gameOverPanel;
        public Sprite replayButton;
        public Sprite homeButton;
        [Tooltip("Home gradient; covers the whole screen.")]
        public Sprite homeBackground;
        [Tooltip("Home logo, icons and event card at design size, so they stay aligned with the buttons on any screen.")]
        public Sprite homeArt;
        public Sprite adventureButton;
        public Sprite classicButton;
        public Sprite homeGear;
        public Sprite playGear;
        [Tooltip("Settings panel with its labels and icons; toggles and row buttons are drawn on top.")]
        public Sprite settingsPanel;
        public Sprite toggleOn;
        public Sprite toggleOff;
        public Sprite settingsReplayButton;
        public Sprite settingsHomeButton;
        [Tooltip("Glow drawn over the cells of lines the dragged piece would clear.")]
        public Sprite cellHighlight;
        public Sprite comboLabel;
        [Tooltip("Index = digit (0-9). A missing digit makes the combo number fall back to the font.")]
        public Sprite[] comboDigits = new Sprite[10];
        public Font font;
        [Range(0f, 1f)] public float previewAlpha = 0.5f;

        [Header("Text colors")]
        public Color bestColor = new Color(1f, 1f, 0f);
        public Color scoreLabelColor = new Color(0.055f, 0.75f, 0.92f);
        public Color scoreColor = new Color(0.75f, 0.86f, 0.96f);

        [Header("Layout: play screen")]
        public Vector2 designSize = new Vector2(8.62f, 17.72f);
        public float boardCenterY = 1.68f;
        [Tooltip("Frame size relative to the cell grid, so the border sits outside the cells.")]
        public float boardFrameScale = 1.01f;
        public float handY = -4.75f;
        public float handSpacing = 2.75f;
        public float handPieceScale = 0.45f;
        [Tooltip("Dragged piece floats this far above the pointer so the finger does not hide it.")]
        public float dragLift = 2f;
        public Vector2 crownPos = new Vector2(-3.16f, 8.09f);
        public Vector2 crownSize = new Vector2(0.69f, 0.56f);
        public Vector2 bestPos = new Vector2(-3.18f, 7.52f);
        public float bestTextHeight = 0.38f;
        public Vector2 scoreLabelPos = new Vector2(0f, 7.17f);
        public float scoreLabelHeight = 0.32f;
        public Vector2 scorePos = new Vector2(0f, 6.47f);
        public float scoreTextHeight = 0.69f;
        public Vector2 outOfSpacePos = new Vector2(0f, -6.6f);
        public float outOfSpaceTextHeight = 0.45f;
        public Vector2 gearPos = new Vector2(3.68f, 8.11f);
        public float gearWidth = 0.56f;

        [Header("Layout: game over")]
        public Vector2 titlePos = new Vector2(0f, 4.81f);
        public float titleWidth = 5.5f;
        public Vector2 panelPos = new Vector2(0f, 0.97f);
        public float panelWidth = 6.51f;
        public Vector2 resultLabelPos = new Vector2(0f, 1.86f);
        public float resultLabelHeight = 0.28f;
        public Vector2 resultScorePos = new Vector2(0f, 0.8f);
        public float resultScoreHeight = 0.93f;
        public Vector2 replayPos = new Vector2(1.65f, -3.17f);
        public float replayWidth = 2.93f;
        public Vector2 homePos = new Vector2(-1.81f, -3.16f);
        public float homeWidth = 2.99f;

        [Header("Layout: home")]
        [Tooltip("World width of homeArt, the full Figma home frame (1446 px).")]
        public float homeArtWidth = 8.66f;
        [Tooltip("Adventure has no mode of its own yet, so it starts Classic too.")]
        public Vector2 adventurePos = new Vector2(0.03f, -4.24f);
        public Vector2 classicPos = new Vector2(0.03f, -7.13f);
        public float modeButtonWidth = 4.38f;
        public Vector2 homeGearPos = new Vector2(3.55f, 7.52f);
        public float homeGearWidth = 0.68f;

        [Header("Layout: settings")]
        public float settingsPanelWidth = 7.36f;
        [Tooltip("Screen dim behind the panel; tapping it closes the panel.")]
        public Color settingsDim = new Color(0f, 0f, 0f, 0.6f);
        [Tooltip("Sound, BGM, Vibration.")]
        public Vector2[] togglePos = { new Vector2(1.94f, 2.92f), new Vector2(1.94f, 1.76f), new Vector2(1.94f, 0.59f) };
        public float toggleWidth = 1.28f;
        public Vector2 settingsReplayPos = new Vector2(1.72f, -0.65f);
        public Vector2 settingsHomePos = new Vector2(1.72f, -1.8f);
        public float settingsButtonWidth = 1.77f;

        [Header("VFX")]
        [Tooltip("Highlight size relative to a cell; 1.08 puts its ring on the cell edge.")]
        public float highlightScale = 1.08f;
        [Tooltip("Highlight alpha pulses between these values while dragging.")]
        public Vector2 highlightAlpha = new Vector2(0.55f, 1f);
        public float highlightPulseSpeed = 6f;
        [Tooltip("Placed cells start at this scale and settle to 1.")]
        public float placePopScale = 1.2f;
        public float placePopTime = 0.15f;
        [Tooltip("Cleared cells grow to this scale while fading out.")]
        public float clearScale = 1.3f;
        public float clearTime = 0.3f;
        [Tooltip("Delay per cell of distance from the placed piece, so lines burst outward.")]
        public float clearStagger = 0.03f;
        public int shardsPerCell = 3;
        public float shardSize = 0.3f;
        public float shardSpeed = 5f;
        public float shardGravity = 15f;
        public float shardTime = 0.6f;
        [Tooltip("Camera shake per line beyond the first cleared in one placement.")]
        public float shakePerExtraLine = 0.06f;
        public float shakeTime = 0.25f;
        [Tooltip("Smallest combo that shows the Combo popup.")]
        public int comboPopupMin = 2;
        public Vector2 comboPopupPos = new Vector2(0f, 1f);
        public float comboLabelHeight = 0.58f;
        public float comboDigitHeight = 0.94f;
        public float comboSpacing = 0.1f;
        public float comboPopupTime = 1f;
        public float comboRise = 0.6f;
        [Tooltip("Seconds for the score text to roll up to a new value.")]
        public float scoreRollTime = 0.4f;

        public int ColorCount => Mathf.Max(1, blockSprites != null ? blockSprites.Length : 0);

        /// <summary>Sprite for a color id (1-based); null when the art is not assigned.</summary>
        public Sprite BlockSprite(int colorId) =>
            blockSprites != null && colorId - 1 < blockSprites.Length ? blockSprites[colorId - 1] : null;

        /// <summary>Digit sprite (0-9); null when the art is not assigned.</summary>
        public Sprite ComboDigit(int digit) =>
            comboDigits != null && digit < comboDigits.Length ? comboDigits[digit] : null;
    }
}
