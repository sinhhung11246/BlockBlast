using BlockBlast.Core;
using BlockBlast.Data;
using UnityEngine;

namespace BlockBlast.View
{
    /// <summary>Draws the board frame, placed blocks and the drop preview with its line highlight. Holds no game state.</summary>
    public sealed class BoardView : MonoBehaviour
    {
        GameConfig cfg;
        SpriteRenderer[,] cells;
        SpriteRenderer[,] highlights;
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
            highlights = new SpriteRenderer[n, n];
            for (int x = 0; x < n; x++)
                for (int y = 0; y < n; y++)
                {
                    var pos = new Vector2(x + 0.5f - half, y + 0.5f - half);
                    cells[x, y] = SpriteFactory.Create("Cell", transform, null, pos, 1f, 1);
                    highlights[x, y] = SpriteFactory.Create("Highlight", transform, cfg.cellHighlight, pos, cfg.highlightScale, 4);
                    highlights[x, y].enabled = false;
                }
        }

        public Vector2 CellCenter(Vector2Int cell) => min + cell + Vector2.one * 0.5f;

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
                {
                    Paint(cells[x, y], board[x, y], 1f);
                    highlights[x, y].enabled = false;
                }

            if (!preview.HasValue || !board.CanPlace(preview.Value.Shape, origin)) return;

            var piece = preview.Value;
            foreach (var c in piece.Shape.Cells)
            {
                var p = origin + c;
                Paint(cells[p.x, p.y], piece.ColorId, cfg.previewAlpha);
            }

            // Lines this placement would complete take the piece's color and a pulsing glow.
            LineSet lines = board.LinesCompletedBy(piece.Shape, origin);
            float t = 0.5f + 0.5f * Mathf.Sin(Time.time * cfg.highlightPulseSpeed);
            var glow = new Color(1f, 1f, 1f, Mathf.Lerp(cfg.highlightAlpha.x, cfg.highlightAlpha.y, t));
            foreach (int y in lines.Rows)
                for (int x = 0; x < board.Size; x++) Highlight(x, y, piece.ColorId, glow);
            foreach (int x in lines.Columns)
                for (int y = 0; y < board.Size; y++) Highlight(x, y, piece.ColorId, glow);
        }

        void Highlight(int x, int y, int colorId, Color glow)
        {
            Paint(cells[x, y], colorId, 1f);
            highlights[x, y].enabled = true;
            highlights[x, y].color = glow;
        }

        void Paint(SpriteRenderer sr, int colorId, float alpha)
        {
            sr.enabled = colorId != 0;
            if (!sr.enabled) return;
            SpriteFactory.SetSprite(sr, cfg.BlockSprite(colorId), 1f);
            sr.color = new Color(1f, 1f, 1f, alpha);
        }
    }
}
