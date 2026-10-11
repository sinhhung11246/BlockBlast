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

        [Tooltip("A new hand is rerolled up to this many times until all its pieces can be placed. 1 = pure random.")]
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

        [Header("Coins and revive")]
        [Tooltip("Coins earned at game over = score / this.")]
        public int pointsPerCoin = 10;
        [Tooltip("Coins to continue a lost game.")]
        public int reviveCost = 200;
        [Tooltip("Continues allowed per game, by coins or by ad.")]
        public int maxRevives = 1;
        [Tooltip("A revive rerolls the hand up to this many times for pieces that can all be placed.")]
        public int reviveRefillAttempts = 200;
        [Tooltip("Length of the placeholder ad used until an ads SDK is added.")]
        public float fakeAdSeconds = 2f;

        [Header("Daily tasks")]
        public int dailyTaskCount = 5;
        [Tooltip("Each day picks dailyTaskCount different tasks from this pool.")]
        public DailyTaskDef[] dailyTaskPool =
        {
            new DailyTaskDef { type = DailyTaskType.PlayGames, target = 3, reward = 20 },
            new DailyTaskDef { type = DailyTaskType.PlacePieces, target = 50, reward = 20 },
            new DailyTaskDef { type = DailyTaskType.ClearLines, target = 20, reward = 30 },
            new DailyTaskDef { type = DailyTaskType.ClearLines, target = 50, reward = 60 },
            new DailyTaskDef { type = DailyTaskType.ScorePoints, target = 500, reward = 30 },
            new DailyTaskDef { type = DailyTaskType.ScorePoints, target = 2000, reward = 60 },
            new DailyTaskDef { type = DailyTaskType.ReachCombo, target = 3, reward = 30 },
            new DailyTaskDef { type = DailyTaskType.ReachCombo, target = 5, reward = 50 },
        };

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
        public Sprite coin;
        public Sprite coinPill;
        public Sprite homeTasksButton;
        [Tooltip("Revive popup with its title bar, close X and icon; title, buttons and texts are drawn on top.")]
        public Sprite revivePanel;
        public Sprite coinButton;
        public Sprite adButton;
        public Sprite videoIcon;
        public Sprite tasksPanel;
        public Sprite tasksIcon;
        public Sprite closeButton;
        public Sprite clockIcon;
        public Sprite taskRow;
        public Sprite taskRewardTile;
        public Sprite taskBar;
        public Sprite taskBarFill;
        public Sprite claimButton;
        public Sprite claimButtonOff;
        public Sprite taskDone;
        [Tooltip("Praise words by lines cleared in one placement: 1 = Good, 2 = Great... the last one repeats.")]
        public Sprite[] praiseSprites;
        [Tooltip("Glow drawn over the cells of lines the dragged piece would clear.")]
        public Sprite cellHighlight;
        public Sprite comboLabel;
        [Tooltip("Index = digit (0-9). A missing digit makes the combo number fall back to the font.")]
        public Sprite[] comboDigits = new Sprite[10];
        public Font font;
        [Range(0f, 1f)] public float previewAlpha = 0.5f;

        [Header("Text colors")]
        public Color bestColor = new Color(1f, 1f, 0f);
        public Color popupTextColor = Color.white;
        [Tooltip("Task descriptions and reward numbers on the light task rows.")]
        public Color taskTextColor = new Color(0.36f, 0.2f, 0.62f);
        public Color timerColor = new Color(1f, 0.84f, 0.2f);
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
        [Tooltip("Coins earned this game, inside the panel under the score.")]
        public Vector2 coinsEarnedPos = new Vector2(0f, -0.3f);

        [Header("Layout: home")]
        [Tooltip("World width of homeArt, the full Figma home frame (1446 px).")]
        public float homeArtWidth = 8.66f;
        [Tooltip("Adventure has no mode of its own yet, so it starts Classic too.")]
        public Vector2 adventurePos = new Vector2(0.03f, -4.24f);
        public Vector2 classicPos = new Vector2(0.03f, -7.13f);
        public float modeButtonWidth = 4.38f;
        public Vector2 homeGearPos = new Vector2(3.55f, 7.52f);
        public float homeGearWidth = 0.68f;
        [Tooltip("The baked tasks icon on the home art, replaced by a button.")]
        public Place homeTasks = new Place(-3.52f, 4.84f, 1.01f);
        public Vector2 homeCoinsPos = new Vector2(0f, 7.49f);

        [Header("Layout: coin badge (relative to its center)")]
        public Place badgePill = new Place(0.1f, 0.03f, 1.71f);
        public Place badgeCoin = new Place(-0.63f, 0f, 0.65f);
        public Place badgeText = new Place(0.29f, 0.03f, 0.22f);
        public Color badgeTextColor = new Color(0.36f, 0.2f, 0.62f);

        [Header("Layout: revive (relative to the popup center)")]
        public Vector2 revivePos = new Vector2(0f, 1.2f);
        public float revivePanelWidth = 6.77f;
        [Tooltip("Coin balance, inside the panel left of the icon.")]
        public Vector2 reviveCoinsPos = new Vector2(-1.95f, 1.85f);
        public Place reviveTitle = new Place(0f, 2.9f, 0.45f);
        [Tooltip("Hit circle of the X baked into the panel; size = radius.")]
        public Place reviveClose = new Place(2.63f, 2.91f, 0.4f);
        public Place reviveCoinButton = new Place(-1.53f, -1.95f, 2.9f);
        [Tooltip("Relative to the coin button.")]
        public Place revivePrice = new Place(0.24f, 0.08f, 0.3f);
        public Place reviveAdButton = new Place(1.53f, -1.95f, 2.9f);
        [Tooltip("Relative to the ad button.")]
        public Place reviveVideoIcon = new Place(-0.86f, -0.04f, 0.6f);
        [Tooltip("Relative to the ad button.")]
        public Place reviveAdText = new Place(0.3f, 0.01f, 0.28f);
        [Range(0f, 1f)] public float disabledAlpha = 0.45f;

        [Header("Layout: daily tasks (relative to the panel center)")]
        public float tasksPanelWidth = 7.62f;
        public Place tasksIconPlace = new Place(-2.6f, 6.17f, 1.62f);
        public Place tasksTitle = new Place(0.3f, 6.14f, 0.45f);
        public Place tasksClose = new Place(2.87f, 6.11f, 0.53f);
        public Place tasksClock = new Place(-0.89f, 4.92f, 0.53f);
        public Place tasksTimer = new Place(0.25f, 4.95f, 0.22f);
        public Vector2 firstRowPos = new Vector2(0.04f, 2.45f);
        public float rowPitch = 1.96f;
        public float rowWidth = 6.84f;
        [Tooltip("Row parts below are relative to the row center.")]
        public Place rowTile = new Place(-2.47f, 0f, 1.24f);
        public Place rowCoin = new Place(-2.47f, 0.12f, 0.86f);
        public Place rowReward = new Place(-2.47f, -0.35f, 0.24f);
        public Place rowText = new Place(0.03f, 0.33f, 0.21f);
        [Tooltip("Bar background; the fill grows from its left edge.")]
        public Place rowBar = new Place(0f, -0.17f, 2.92f);
        [Tooltip("Gap between the bar's edge and its fill, on every side.")]
        public float rowBarFillInset = 0.03f;
        public Place rowButton = new Place(2.4f, -0.08f, 1.52f);
        public Place rowButtonText = new Place(2.4f, -0.07f, 0.24f);
        public Place rowDone = new Place(2.48f, 0f, 0.98f);

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
        public Vector2 praisePos = new Vector2(0f, 2.6f);
        [Tooltip("World units per sprite unit, so every praise word shares one font size.")]
        public float praiseScale = 0.33f;
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
