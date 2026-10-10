using System.Linq;
using BlockBlast.Core;
using BlockBlast.Data;
using BlockBlast.View;
using UnityEngine;

namespace BlockBlast.Game
{
    /// <summary>
    /// Runs the core loop: deal hand -> drag piece -> place -> clear lines -> score -> refill / game over.
    /// Auto-created when the scene has none, so an empty scene is playable.
    /// </summary>
    public sealed class GameController : MonoBehaviour
    {
        [Tooltip("Empty = Resources/GameConfig, else built-in defaults.")]
        [SerializeField] GameConfig config;

        Board board;
        Hand hand;
        ScoreSystem score;

        Camera cam;
        BoardView boardView;
        HudView hud;
        GameOverView gameOverView;
        PieceView[] pieceViews;

        int dragIndex = -1;
        Vector2 dragOffset;
        bool gameOver;

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
            SpriteFactory.Cover(bg, cam);
            boardView = Child<BoardView>("Board");
            boardView.Build(config);
            hud = Child<HudView>("Hud");
            hud.Init(config, cam);
            gameOverView = Child<GameOverView>("GameOver");
            gameOverView.Build(config, cam);
            pieceViews = new PieceView[config.handSize];

            Restart();
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

            // Show the whole Figma screen; wider or taller devices see extra background.
            cam.orthographicSize = Mathf.Max(config.designSize.y, config.designSize.x / cam.aspect) * 0.5f;
            cam.transform.position = new Vector3(0f, 0f, -10f);
        }

        T Child<T>(string name) where T : Component
        {
            var go = new GameObject(name);
            go.transform.SetParent(transform, false);
            return go.AddComponent<T>();
        }

        void Restart()
        {
            board.Clear();
            score.Reset();
            gameOver = false;
            gameOverView.Show(false);
            DealHand();
            SyncHud(false);
            boardView.Render(board);
        }

        void DealHand()
        {
            hand.Refill();
            for (int i = 0; i < hand.Count; i++)
            {
                if (pieceViews[i] != null) Destroy(pieceViews[i].gameObject);
                pieceViews[i] = PieceView.Create(transform, hand[i].Value, config);
                ReturnToSlot(i);
            }
        }

        void Update()
        {
            Vector2 pointer = PointerInput.WorldPosition(cam);

            if (gameOver)
            {
                if (PointerInput.Pressed && gameOverView.HitsReplay(pointer)) Restart();
                return;
            }

            if (dragIndex < 0)
            {
                if (PointerInput.Pressed) BeginDrag(pointer);
                return;
            }

            PieceView piece = pieceViews[dragIndex];
            piece.transform.position = pointer + dragOffset;
            Vector2Int origin = boardView.WorldToCell(piece.BottomLeftCellCenter());

            if (PointerInput.Released) EndDrag(origin);
            else boardView.Render(board, piece.Piece, origin);
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
            LineSet lines = board.FindFullLines();
            board.ClearLines(lines);
            score.AddPlacement(piece.Shape.Cells.Length, lines.Count);

            hand.Take(i);
            Destroy(pieceViews[i].gameObject);
            pieceViews[i] = null;
            if (hand.IsEmpty) DealHand();

            boardView.Render(board);

            gameOver = !hand.AnyFits(board);
            gameOverView.Show(gameOver);
            SyncHud(gameOver && HighScoreStore.Submit(score.Score));
        }

        void SyncHud(bool newRecord)
        {
            hud.Score = score.Score;
            hud.Combo = score.Combo;
            hud.Best = Mathf.Max(HighScoreStore.Load(), score.Score);
            hud.GameOver = gameOver;
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
