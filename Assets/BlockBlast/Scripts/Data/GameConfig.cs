using UnityEngine;

namespace BlockBlast.Data
{
    /// <summary>
    /// Gameplay tunables shared by the Home and Game scenes. The project asset lives at Resources/GameConfig.
    /// Layout and screen art live in the scenes and prefabs; world units are board cells.
    /// </summary>
    [CreateAssetMenu(menuName = "BlockBlast/Game Config", fileName = "GameConfig")]
    public sealed class GameConfig : ScriptableObject
    {
        [Header("Dealing")]
        [Tooltip("Rolls tried per new hand. Only hands whose pieces can all be placed count; 1 = pure random.")]
        public int refillAttempts = 20;
        [Tooltip("Board fill (0-1) above which the dealer keeps its friendliest roll instead of the first; 1 = never.")]
        [Range(0f, 1f)] public float mercyFillStart = 0.5f;
        [Tooltip("Mercy score per line the hand's pieces can clear right away.")]
        public float mercyClearWeight = 2f;
        [Tooltip("Mercy score lost per cell in the hand, so crowded boards get smaller pieces.")]
        public float mercyCellWeight = 0.5f;
        [Tooltip("Seconds the board stays visible, with unplaceable pieces greyed, before the revive offer or Game Over.")]
        public float gameOverDelay = 1.5f;
        [Tooltip("Hand pieces are drawn at this scale; a dragged piece is full size.")]
        public float handPieceScale = 0.45f;
        [Tooltip("Dragged piece floats this far above the pointer so the finger does not hide it.")]
        public float dragLift = 2f;

        [Header("Score")]
        public int pointsPerCell = 1;
        public int pointsPerLine = 10;
        [Tooltip("Extra points for each line beyond the first in one placement (before combo).")]
        public int multiLineBonus = 10;
        [Tooltip("Placements in a row without a clear before combo resets to 0.")]
        public int comboBreakAfterMisses = 1;
        [Tooltip("Seconds for the score text to roll up to a new value.")]
        public float scoreRollTime = 0.4f;

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
        [Tooltip("At most the number of rows in the DailyTasksPopup prefab.")]
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
        [Tooltip("Every piece shape and its relative chance: small and line pieces common, big ones rare.")]
        public ShapeDef[] shapes =
        {
            new ShapeDef("#", 1f),
            new ShapeDef("##", 1.2f), new ShapeDef("#/#", 1.2f),
            new ShapeDef("###", 1.2f), new ShapeDef("#/#/#", 1.2f),
            new ShapeDef("####", 0.8f), new ShapeDef("#/#/#/#", 0.8f),
            new ShapeDef("#####", 0.3f), new ShapeDef("#/#/#/#/#", 0.3f),
            new ShapeDef("##/##", 1f),
            new ShapeDef("###/###/###", 0.25f),
            new ShapeDef("##/#.", 1f), new ShapeDef("##/.#", 1f), new ShapeDef("#./##", 1f), new ShapeDef(".#/##", 1f),
            new ShapeDef("#../#../###", 0.3f), new ShapeDef("###/..#/..#", 0.3f), new ShapeDef("..#/..#/###", 0.3f), new ShapeDef("###/#../#..", 0.3f),
            new ShapeDef("#./#./##", 0.6f), new ShapeDef("##/.#/.#", 0.6f), new ShapeDef("###/#..", 0.6f), new ShapeDef("..#/###", 0.6f),
            new ShapeDef(".#/.#/##", 0.6f), new ShapeDef("##/#./#.", 0.6f), new ShapeDef("#../###", 0.6f), new ShapeDef("###/..#", 0.6f),
            new ShapeDef("###/.#.", 0.6f), new ShapeDef(".#./###", 0.6f), new ShapeDef("#./##/#.", 0.6f), new ShapeDef(".#/##/.#", 0.6f),
            new ShapeDef("##./.##", 0.5f), new ShapeDef(".##/##.", 0.5f), new ShapeDef("#./##/.#", 0.5f), new ShapeDef(".#/##/#.", 0.5f),
        };

        [Header("Blocks")]
        [Tooltip("One sprite per block color; each piece gets a random one.")]
        public Sprite[] blockSprites;
        [Tooltip("Shown on hand pieces that no longer fit when the game ends.")]
        public Sprite noFitBlock;
        [Tooltip("Alpha of the drop preview on the board.")]
        [Range(0f, 1f)] public float previewAlpha = 0.5f;

        [Header("VFX")]
        [Tooltip("Line highlight alpha pulses between these values while dragging.")]
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
        [Tooltip("Praise words by lines cleared in one placement: 1 = Good, 2 = Great... the last one repeats.")]
        public Sprite[] praiseSprites;
        public Vector2 praisePos = new Vector2(0f, 2.6f);
        [Tooltip("World units per sprite unit, so every praise word shares one font size.")]
        public float praiseScale = 0.33f;
        [Tooltip("Smallest combo that shows the Combo popup.")]
        public int comboPopupMin = 2;
        public Vector2 comboPopupPos = new Vector2(0f, 1f);
        public Sprite comboLabel;
        [Tooltip("Index = digit (0-9). A missing digit makes the combo number fall back to the font.")]
        public Sprite[] comboDigits = new Sprite[10];
        public Font font;
        public Color comboNumberColor = new Color(1f, 1f, 0f);
        public float comboLabelHeight = 0.58f;
        public float comboDigitHeight = 0.94f;
        public float comboSpacing = 0.1f;
        [Tooltip("Seconds a praise word or Combo popup stays on screen.")]
        public float comboPopupTime = 1f;
        public float comboRise = 0.6f;

        public int ColorCount => Mathf.Max(1, blockSprites != null ? blockSprites.Length : 0);

        /// <summary>Sprite for a color id (1-based); null when the art is not assigned.</summary>
        public Sprite BlockSprite(int colorId) =>
            blockSprites != null && colorId - 1 < blockSprites.Length ? blockSprites[colorId - 1] : null;

        /// <summary>Digit sprite (0-9); null when the art is not assigned.</summary>
        public Sprite ComboDigit(int digit) =>
            comboDigits != null && digit < comboDigits.Length ? comboDigits[digit] : null;
    }
}
