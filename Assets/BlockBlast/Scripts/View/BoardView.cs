using BlockBlast.Core;
using BlockBlast.Data;
using UnityEngine;

namespace BlockBlast.View
{
    /// <summary>
    /// Draws placed blocks and the drop preview with its line highlight on the cells laid out in the scene.
    /// The board is centered on this transform, one world unit per cell. Holds no game state.
    /// </summary>
    public sealed class BoardView : MonoBehaviour
    {
        [Tooltip("Size x Size cells, index = x * Size + y; all block sprites share one pixel size, so swapping keeps the scale.")]
        [SerializeField] SpriteRenderer[] cells;
        [Tooltip("Glow over each cell of a line the dragged piece would clear; same indexing as cells.")]
        [SerializeField] SpriteRenderer[] highlights;

        GameConfig cfg;

        public int Size => Mathf.RoundToInt(Mathf.Sqrt(cells.Length));

        /// <summary>World position of the grid's bottom-left corner.</summary>
        Vector2 Min => (Vector2)transform.position - Vector2.one * (Size * 0.5f);

        public void Init(GameConfig config) => cfg = config;

        public Vector2 CellCenter(Vector2Int cell) => Min + cell + Vector2.one * 0.5f;

        /// <summary>Cell containing a world point (may be out of bounds).</summary>
        public Vector2Int WorldToCell(Vector2 world)
        {
            Vector2 p = world - Min;
            return new Vector2Int(Mathf.FloorToInt(p.x), Mathf.FloorToInt(p.y));
        }

        /// <summary>Draws the board, plus the preview piece and the lines it would clear when given.</summary>
        public void Render(Board board, Hand.Piece? preview = null, Vector2Int origin = default)
        {
            for (int x = 0; x < board.Size; x++)
                for (int y = 0; y < board.Size; y++)
                {
                    Paint(x, y, board[x, y], 1f);
                    Highlight(x, y).enabled = false;
                }

            if (!preview.HasValue || !board.CanPlace(preview.Value.Shape, origin)) return;

            var piece = preview.Value;
            foreach (var c in piece.Shape.Cells)
            {
                var p = origin + c;
                Paint(p.x, p.y, piece.ColorId, cfg.previewAlpha);
            }

            // Lines this placement would complete take the piece's color and a pulsing glow.
            LineSet lines = board.LinesCompletedBy(piece.Shape, origin);
            float t = 0.5f + 0.5f * Mathf.Sin(Time.time * cfg.highlightPulseSpeed);
            var glow = new Color(1f, 1f, 1f, Mathf.Lerp(cfg.highlightAlpha.x, cfg.highlightAlpha.y, t));
            foreach (int y in lines.Rows)
                for (int x = 0; x < board.Size; x++) Glow(x, y, piece.ColorId, glow);
            foreach (int x in lines.Columns)
                for (int y = 0; y < board.Size; y++) Glow(x, y, piece.ColorId, glow);
        }

        void Glow(int x, int y, int colorId, Color glow)
        {
            Paint(x, y, colorId, 1f);
            SpriteRenderer h = Highlight(x, y);
            h.enabled = true;
            h.color = glow;
        }

        void Paint(int x, int y, int colorId, float alpha)
        {
            SpriteRenderer sr = cells[x * Size + y];
            sr.enabled = colorId != 0;
            if (!sr.enabled) return;
            sr.sprite = cfg.BlockSprite(colorId);
            sr.color = new Color(1f, 1f, 1f, alpha);
        }

        SpriteRenderer Highlight(int x, int y) => highlights[x * Size + y];
    }
}
