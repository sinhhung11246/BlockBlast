using BlockBlast.Core;
using BlockBlast.Data;
using UnityEngine;

namespace BlockBlast.View
{
    /// <summary>Draws the board frame, placed blocks and the drop preview. Holds no game state.</summary>
    public sealed class BoardView : MonoBehaviour
    {
        GameConfig cfg;
        SpriteRenderer[,] cells;
        Vector2 min; // world position of the grid's bottom-left corner

        public void Build(GameConfig config)
        {
            cfg = config;
            int n = cfg.boardSize;
            float half = n * 0.5f;
            transform.position = new Vector3(0f, cfg.boardCenterY, 0f);
            min = new Vector2(-half, cfg.boardCenterY - half);

            SpriteFactory.Create("Frame", transform, cfg.boardFrame, Vector2.zero, n * cfg.boardFrameScale, 0);

            cells = new SpriteRenderer[n, n];
            for (int x = 0; x < n; x++)
                for (int y = 0; y < n; y++)
                    cells[x, y] = SpriteFactory.Create("Cell", transform, null, new Vector2(x + 0.5f - half, y + 0.5f - half), 1f, 1);
        }

        /// <summary>Cell containing a world point (may be out of bounds).</summary>
        public Vector2Int WorldToCell(Vector2 world)
        {
            Vector2 p = world - min;
            return new Vector2Int(Mathf.FloorToInt(p.x), Mathf.FloorToInt(p.y));
        }

        /// <summary>Draws the board, plus the preview piece and the lines it would clear when given.</summary>
        public void Render(Board board, Hand.Piece? preview = null, Vector2Int origin = default)
        {
            for (int x = 0; x < board.Size; x++)
                for (int y = 0; y < board.Size; y++)
                    Paint(cells[x, y], board[x, y], 1f);

            if (!preview.HasValue || !board.CanPlace(preview.Value.Shape, origin)) return;

            var piece = preview.Value;
            foreach (var c in piece.Shape.Cells)
            {
                var p = origin + c;
                Paint(cells[p.x, p.y], piece.ColorId, cfg.previewAlpha);
            }

            // Lines this placement would complete take the piece's color.
            LineSet lines = board.LinesCompletedBy(piece.Shape, origin);
            foreach (int y in lines.Rows)
                for (int x = 0; x < board.Size; x++) Paint(cells[x, y], piece.ColorId, 1f);
            foreach (int x in lines.Columns)
                for (int y = 0; y < board.Size; y++) Paint(cells[x, y], piece.ColorId, 1f);
        }

        void Paint(SpriteRenderer sr, int colorId, float alpha)
        {
            sr.enabled = colorId != 0;
            if (!sr.enabled) return;
            Sprite sprite = cfg.BlockSprite(colorId);
            if (sprite != null) sr.sprite = sprite;
            sr.color = new Color(1f, 1f, 1f, alpha);
        }
    }
}
