using System.Collections.Generic;
using System.Linq;
using BlockBlast.Core;
using BlockBlast.Data;
using BlockBlast.View;
using UnityEngine;

namespace BlockBlast.Game
{
    /// <summary>
    /// Runs the core loop: deal hand -> drag piece -> place -> clear lines -> score -> refill / game over,
    /// plus Home, Settings, Daily Tasks, coins and the revive offer. Auto-created when the scene has none, so an empty scene is playable.
    /// </summary>
    public sealed class GameController : MonoBehaviour
    {
        [Tooltip("Empty = Resources/GameConfig, else built-in defaults.")]
        [SerializeField] GameConfig config;

        Board board;
        Hand hand;
        ScoreSystem score;
        DailyTasks tasks;
        IRewardedAds ads;

        Camera cam;
        BoardView boardView;
        HudView hud;
        GameOverView gameOverView;
        HomeView homeView;
        SettingsView settingsView;
        DailyTasksView tasksView;
        ReviveView reviveView;
        VfxView vfx;
        PieceView[] pieceViews;

        enum State { Home, Playing, OutOfSpace, Revive, GameOver }

        int dragIndex = -1;
        Vector2 dragOffset;
        State state;
        float outOfSpaceTimer;
        int revivesUsed;
        float fittedAspect;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Bootstrap()
        {
            if (FindAnyObjectByType<GameController>() == null)
                new GameObject("BlockBlast").AddComponent<GameController>();
        }

        void Awake()
        {
            if (config == null) config = Resources.Load<GameConfig>("GameConfig");
            if (config == null) config = ScriptableObject.CreateInstance<GameConfig>();

            board = new Board(config.boardSize);
            hand = new Hand(config.handSize, config.shapes.Select(Shape.Parse).ToArray(),
                config.ColorCount, new System.Random());
            score = new ScoreSystem(new ScoreSystem.Rules
            {
                PointsPerCell = config.pointsPerCell,
                PointsPerLine = config.pointsPerLine,
                MultiLineBonus = config.multiLineBonus,
                ComboBreakAfterMisses = config.comboBreakAfterMisses,
            });

            SetupCamera();
            var bg = SpriteFactory.Create("Background", transform, config.background, Vector2.zero, 1f, -10);
            SpriteFactory.Cover(bg, cam, 1.05f); // margin so camera shake never shows the edge
            boardView = Child<BoardView>("Board");
            boardView.Build(config);
            hud = Child<HudView>("Hud");
            hud.Init(config, cam);
            gameOverView = Child<GameOverView>("GameOver");
            gameOverView.Build(config, cam);
            vfx = Child<VfxView>("Vfx");
            vfx.Init(config, cam);
            homeView = Child<HomeView>("Home");
            homeView.Build(config, cam);
            settingsView = Child<SettingsView>("Settings");
            settingsView.Build(config, cam);
            tasksView = Child<DailyTasksView>("DailyTasks");
            tasksView.Build(config, cam);
            reviveView = Child<ReviveView>("Revive");
            reviveView.Build(config, cam);
            tasks = new DailyTasks(config);
            var fakeAds = gameObject.AddComponent<FakeRewardedAds>();
            fakeAds.seconds = config.fakeAdSeconds;
            ads = fakeAds;
            pieceViews = new PieceView[config.handSize];

            GoHome();
        }

        void SetupCamera()
        {
            cam = Camera.main;
            if (cam == null)
            {
                cam = new GameObject("Main Camera") { tag = "MainCamera" }.AddComponent<Camera>();
                cam.transform.position = new Vector3(0f, 0f, -10f);
            }
            cam.orthographic = true;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Color.black;

            cam.transform.position = new Vector3(0f, 0f, -10f);
            FitCamera();
        }

        /// <summary>Shows the whole Figma screen; wider or taller devices see extra background.</summary>
        void FitCamera()
        {
            fittedAspect = cam.aspect;
            cam.orthographicSize = Mathf.Max(config.designSize.y, config.designSize.x / cam.aspect) * 0.5f;
        }

        T Child<T>(string name) where T : Component
        {
            var go = new GameObject(name);
            go.transform.SetParent(transform, false);
            return go.AddComponent<T>();
        }

        void Restart()
        {
            if (state != State.Home) HighScoreStore.Submit(score.Score); // a run left mid-game still counts for Best
            board.Clear();
            score.Reset();
            vfx.Clear();
            revivesUsed = 0;
            state = State.Playing;
            gameOverView.Show(false);
            reviveView.Show(false);
            homeView.Show(false);
            DealHand();
            SyncHud();
            boardView.Render(board);
        }

        /// <summary>Leaves the current game (its score still counts for Best) and shows the Home screen.</summary>
        void GoHome()
        {
            if (state != State.Home) HighScoreStore.Submit(score.Score);
            vfx.Clear();
            state = State.Home;
            gameOverView.Show(false);
            homeView.SetCoins(CoinStore.Coins);
            homeView.Show(true);
            SyncHud();
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
                pieceViews[i] = PieceView.Create(transform, hand[i].Value, config);
                ReturnToSlot(i);
            }
        }

        void Update()
        {
            if (cam.aspect != fittedAspect) FitCamera(); // screen resized or rotated
            Vector2 pointer = PointerInput.WorldPosition(cam);

            if (settingsView.IsOpen)
            {
                if (PointerInput.Pressed) TapSettings(pointer);
                return;
            }
            if (tasksView.IsOpen)
            {
                if (PointerInput.Pressed) TapTasks(pointer);
                return;
            }

            switch (state)
            {
                case State.Home:
                    if (!PointerInput.Pressed) return;
                    if (homeView.HitsPlay(pointer)) Restart();
                    else if (homeView.HitsSettings(pointer)) OpenSettings();
                    else if (homeView.HitsTasks(pointer)) OpenTasks();
                    return;
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

        void OpenSettings()
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
                    Restart();
                    return;
                case SettingsView.Action.Home:
                    settingsView.Show(false);
                    GoHome();
                    return;
            }
        }

        void OpenTasks()
        {
            for (int i = 0; i < tasks.Count; i++)
                tasksView.SetRow(i, new DailyTasksView.RowState
                {
                    text = tasks[i].Description,
                    reward = tasks[i].reward,
                    progress = tasks.Progress(i),
                    target = tasks[i].target,
                    claimed = tasks.Claimed(i),
                });
            tasksView.HideRowsFrom(tasks.Count);
            tasksView.Show(true);
        }

        void TapTasks(Vector2 pointer)
        {
            switch (tasksView.Tap(pointer, out int row))
            {
                case DailyTasksView.Action.Close:
                    tasksView.Show(false);
                    return;
                case DailyTasksView.Action.Claim:
                    if (tasks.Claim(row) == 0) return;
                    homeView.SetCoins(CoinStore.Coins);
                    OpenTasks(); // refresh rows
                    return;
            }
        }

        void OfferRevive()
        {
            state = State.Revive;
            reviveView.Show(true, CoinStore.Coins);
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
                    reviveView.Show(false);
                    ShowGameOver();
                    return;
            }
        }

        /// <summary>Continues the lost game with a fresh hand that can all be placed.</summary>
        void Revive()
        {
            revivesUsed++;
            reviveView.Show(false);
            if (!hand.Refill(board, config.reviveRefillAttempts) && !hand.AnyFits(board)) hand.DealSmallest();
            ShowHand();
            state = State.Playing;
            SyncHud();
            if (!hand.AnyFits(board)) EnterOutOfSpace();
        }

        void BeginDrag(Vector2 pointer)
        {
            float pickRadius = config.handSpacing * 0.5f;
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
            gameOverView.SetCoinsEarned(earned);
            gameOverView.Show(true);
            SyncHud(HighScoreStore.Submit(score.Score));
        }

        void SyncHud(bool newRecord = false)
        {
            hud.gameObject.SetActive(state != State.Home);
            hud.Score = score.Score;
            hud.Best = Mathf.Max(HighScoreStore.Load(), score.Score);
            hud.OutOfSpace = state == State.OutOfSpace;
            hud.GameOver = state == State.GameOver;
            hud.NewRecord = newRecord;
        }

        void ReturnToSlot(int i)
        {
            pieceViews[i].SetDragging(false);
            pieceViews[i].transform.position = SlotPosition(i);
        }

        Vector2 SlotPosition(int i)
        {
            float x = (i - (config.handSize - 1) * 0.5f) * config.handSpacing;
            return new Vector2(x, config.handY);
        }
    }
}
