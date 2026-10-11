using System.Collections.Generic;
using System.Linq;
using BlockBlast.Core;
using BlockBlast.Data;
using BlockBlast.View;
using UnityEngine;

namespace BlockBlast.Game
{
    /// <summary>
    /// Game scene: runs the core loop (deal hand -> drag piece -> place -> clear lines -> score -> refill / game over)
    /// plus the revive offer and Game Over. Board size and hand size come from the cells and slots laid out in the scene.
    /// </summary>
    public sealed class GameController : ScreenController
    {
        [SerializeField] BoardView boardView;
        [SerializeField] HudView hud;
        [SerializeField] ReviveView reviveView;
        [SerializeField] GameOverView gameOverView;
        [SerializeField] VfxView vfx;
        [Tooltip("Where each hand piece rests; their count is the hand size.")]
        [SerializeField] Transform[] slots;
        [SerializeField] SpriteRenderer block;

        Board board;
        Hand hand;
        ScoreSystem score;
        DailyTasks tasks;
        IRewardedAds ads;
        PieceView[] pieceViews;

        enum State { Playing, OutOfSpace, Revive, GameOver }

        int dragIndex = -1;
        Vector2 dragOffset;
        State state;
        float outOfSpaceTimer;
        int revivesUsed;

        protected override void Awake()
        {
            base.Awake();
            board = new Board(boardView.Size);
            hand = new Hand(slots.Length, config.shapes.Select(Shape.Parse).ToArray(), config.ColorCount, new System.Random());
            score = new ScoreSystem(new ScoreSystem.Rules
            {
                PointsPerCell = config.pointsPerCell,
                PointsPerLine = config.pointsPerLine,
                MultiLineBonus = config.multiLineBonus,
                ComboBreakAfterMisses = config.comboBreakAfterMisses,
            });
            tasks = new DailyTasks(config);
            var fakeAds = gameObject.AddComponent<FakeRewardedAds>();
            fakeAds.seconds = config.fakeAdSeconds;
            ads = fakeAds;
            pieceViews = new PieceView[slots.Length];

            boardView.Init(config);
            hud.Init(config.scoreRollTime);
            vfx.Init(config, cam);
        }

        void Start() => Restart();

        protected override void Replay() => Restart();

        /// <summary>Leaves the current game (its score still counts for Best) for the Home scene.</summary>
        protected override void GoHome()
        {
            HighScoreStore.Submit(score.Score);
            Load(HomeScene);
        }

        void Restart()
        {
            HighScoreStore.Submit(score.Score); // a run left mid-game still counts for Best
            board.Clear();
            score.Reset();
            vfx.Clear();
            revivesUsed = 0;
            state = State.Playing;
            gameOverView.Hide();
            reviveView.Hide();
            DealHand();
            SyncHud();
            boardView.Render(board);
        }

        void DealHand()
        {
            hand.Refill(board, config.refillAttempts);
            ShowHand();
        }

        void ShowHand()
        {
            for (int i = 0; i < hand.Count; i++)
            {
                if (pieceViews[i] != null) Destroy(pieceViews[i].gameObject);
                pieceViews[i] = PieceView.Create(block, transform, hand[i].Value, config);
                ReturnToSlot(i);
            }
        }

        protected override void Tick(Vector2 pointer)
        {
            switch (state)
            {
                case State.OutOfSpace:
                    outOfSpaceTimer -= Time.deltaTime;
                    if (outOfSpaceTimer > 0f) return;
                    if (revivesUsed < config.maxRevives) OfferRevive();
                    else ShowGameOver();
                    return;
                case State.Revive:
                    if (PointerInput.Pressed) TapRevive(pointer);
                    return;
                case State.GameOver:
                    if (!PointerInput.Pressed) return;
                    if (gameOverView.HitsReplay(pointer)) Restart();
                    else if (gameOverView.HitsHome(pointer)) GoHome();
                    return;
            }

            if (dragIndex < 0)
            {
                if (!PointerInput.Pressed) return;
                if (hud.HitsSettings(pointer)) OpenSettings();
                else BeginDrag(pointer);
                return;
            }

            PieceView piece = pieceViews[dragIndex];
            piece.transform.position = pointer + dragOffset;
            Vector2Int origin = boardView.WorldToCell(piece.BottomLeftCellCenter());

            if (PointerInput.Released) EndDrag(origin);
            else boardView.Render(board, piece.Piece, origin);
        }

        void OfferRevive()
        {
            state = State.Revive;
            reviveView.Show(CoinStore.Coins, config.reviveCost);
        }

        void TapRevive(Vector2 pointer)
        {
            switch (reviveView.Tap(pointer))
            {
                case ReviveView.Action.Coins:
                    if (CoinStore.Spend(config.reviveCost)) Revive();
                    return;
                case ReviveView.Action.Ad:
                    reviveView.ShowAd(true);
                    ads.Show(rewarded =>
                    {
                        if (state != State.Revive) return;
                        reviveView.ShowAd(false);
                        if (rewarded) Revive();
                    });
                    return;
                case ReviveView.Action.Decline:
                    reviveView.Hide();
                    ShowGameOver();
                    return;
            }
        }

        /// <summary>Continues the lost game with a fresh hand that can all be placed.</summary>
        void Revive()
        {
            revivesUsed++;
            reviveView.Hide();
            if (!hand.Refill(board, config.reviveRefillAttempts)) hand.DealSmallest();
            ShowHand();
            state = State.Playing;
            SyncHud();
            if (!hand.AnyFits(board)) EnterOutOfSpace();
        }

        void BeginDrag(Vector2 pointer)
        {
            float pickRadius = slots.Length > 1 ? Vector2.Distance(SlotPosition(0), SlotPosition(1)) * 0.5f : float.MaxValue;
            for (int i = 0; i < pieceViews.Length; i++)
            {
                if (pieceViews[i] == null) continue;
                if (Vector2.Distance(pointer, SlotPosition(i)) > pickRadius) continue;

                dragIndex = i;
                dragOffset = Vector2.up * config.dragLift;
                pieceViews[i].SetDragging(true);
                return;
            }
        }

        void EndDrag(Vector2Int origin)
        {
            int i = dragIndex;
            dragIndex = -1;

            Hand.Piece piece = pieceViews[i].Piece;
            if (!board.CanPlace(piece.Shape, origin))
            {
                ReturnToSlot(i);
                boardView.Render(board);
                return;
            }

            board.Place(piece.Shape, origin, piece.ColorId);
            vfx.Place(piece.Shape.Cells.Select(c => boardView.CellCenter(origin + c)), config.BlockSprite(piece.ColorId));

            LineSet lines = board.FindFullLines();
            if (lines.Count > 0)
            {
                vfx.ClearCells(ClearedCells(lines), PieceCenter(piece, origin), lines.Count);
                SettingsStore.Vibrate();
            }
            board.ClearLines(lines);
            int gained = score.AddPlacement(piece.Shape.Cells.Length, lines.Count);
            if (lines.Count > 0) vfx.Praise(lines.Count);
            if (lines.Count > 0 && score.Combo >= config.comboPopupMin) vfx.Combo(score.Combo);

            tasks.Report(DailyTaskType.PlacePieces, 1);
            tasks.Report(DailyTaskType.ClearLines, lines.Count);
            tasks.Report(DailyTaskType.ScorePoints, gained);
            tasks.Report(DailyTaskType.ReachCombo, score.Combo);

            hand.Take(i);
            Destroy(pieceViews[i].gameObject);
            pieceViews[i] = null;
            if (hand.IsEmpty) DealHand();

            boardView.Render(board);
            if (!hand.AnyFits(board)) EnterOutOfSpace();
            SyncHud();
        }

        /// <summary>Every cell of the lines with its block sprite; read before the lines are removed.</summary>
        IEnumerable<(Vector2, Sprite)> ClearedCells(LineSet lines)
        {
            var cells = new HashSet<Vector2Int>();
            foreach (int y in lines.Rows)
                for (int x = 0; x < board.Size; x++) cells.Add(new Vector2Int(x, y));
            foreach (int x in lines.Columns)
                for (int y = 0; y < board.Size; y++) cells.Add(new Vector2Int(x, y));
            return cells.Select(c => (boardView.CellCenter(c), config.BlockSprite(board[c.x, c.y]))).ToList();
        }

        Vector2 PieceCenter(Hand.Piece piece, Vector2Int origin) =>
            boardView.CellCenter(origin) + new Vector2(piece.Shape.Width - 1, piece.Shape.Height - 1) * 0.5f;

        /// <summary>Greys out the remaining pieces so the player sees why the game ends, then waits.</summary>
        void EnterOutOfSpace()
        {
            state = State.OutOfSpace;
            outOfSpaceTimer = config.gameOverDelay;
            foreach (var view in pieceViews)
                if (view != null) view.SetSprite(config.noFitBlock);
        }

        void ShowGameOver()
        {
            state = State.GameOver;
            int earned = score.Score / Mathf.Max(1, config.pointsPerCoin);
            CoinStore.Add(earned);
            tasks.Report(DailyTaskType.PlayGames, 1);
            gameOverView.Show(score.Score, HighScoreStore.Submit(score.Score), earned);
            SyncHud();
        }

        void SyncHud()
        {
            hud.Score = score.Score;
            hud.Best = Mathf.Max(HighScoreStore.Load(), score.Score);
            hud.OutOfSpace = state == State.OutOfSpace || state == State.Revive;
        }

        void ReturnToSlot(int i)
        {
            pieceViews[i].SetDragging(false);
            pieceViews[i].transform.position = SlotPosition(i);
        }

        Vector2 SlotPosition(int i) => slots[i].position;
    }
}
