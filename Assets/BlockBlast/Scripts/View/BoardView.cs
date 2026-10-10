using BlockBlast.Core;
using BlockBlast.Data;
using UnityEngine;

namespace BlockBlast.View
{
    /// <summary>Draws the board and the drop preview. Holds no game state.</summary>
    public sealed class BoardView : MonoBehaviour
    {
        GameConfig cfg;
        SpriteRenderer[,] cells;
        Vector2 min; // world position of the board's bottom-left corner

        public void Build(GameConfig config)
        {
            cfg = config;
            int n = cfg.boardSize;
            float half = n * cfg.cellSize * 0.5f;
            transform.position = new Vector3(0f, cfg.boardCenterY, 0f);
            min = new Vector2(-half, cfg.boardCenterY - half);

            cells = new SpriteRenderer[n, n];
            for (int x = 0; x < n; x++)
                for (int y = 0; y < n; y++)
                {
                    var local = new Vector3((x + 0.5f) * cfg.cellSize - half, (y + 0.5f) * cfg.cellSize - half, 0f);
                    cells[x, y] = CellSprite.Create(transform, cfg, local, 1);
                }
        }

        /// <summary>Cell containing a world point (may be out of bounds).</summary>
        public Vector2Int WorldToCell(Vector2 world)
        {
            Vector2 p = (world - min) / cfg.cellSize;
            return new Vector2Int(Mathf.FloorToInt(p.x), Mathf.FloorToInt(p.y));
        }

        /// <summary>Draws the board, plus the preview piece and the lines it would clear when given.</summary>
        public void Render(Board board, Hand.Piece? preview = null, Vector2Int origin = default)
        {
            for (int x = 0; x < board.Size; x++)
                for (int y = 0; y < board.Size; y++)
                    cells[x, y].color = board[x, y] == 0 ? cfg.emptyCellColor : cfg.ColorOf(board[x, y]);

            if (!preview.HasValue || !board.CanPlace(preview.Value.Shape, origin)) return;

            var piece = preview.Value;
            Color color = cfg.ColorOf(piece.ColorId);
            foreach (var c in piece.Shape.Cells)
            {
                var p = origin + c;
                cells[p.x, p.y].color = Color.Lerp(cfg.emptyCellColor, color, 0.5f);
            }

            // Tint lines that this placement would complete.
            LineSet lines = board.LinesCompletedBy(piece.Shape, origin);

            foreach (int y in lines.Rows)
                for (int x = 0; x < board.Size; x++) cells[x, y].color = color;
            foreach (int x in lines.Columns)
                for (int y = 0; y < board.Size; y++) cells[x, y].color = color;
        }
    }
}
