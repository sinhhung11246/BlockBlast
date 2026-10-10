using BlockBlast.Core;
using BlockBlast.Data;
using UnityEngine;

namespace BlockBlast.View
{
    /// <summary>One hand piece, drawn centered on its transform.</summary>
    public sealed class PieceView : MonoBehaviour
    {
        GameConfig cfg;
        SpriteRenderer[] renderers;

        public Hand.Piece Piece { get; private set; }

        public static PieceView Create(Transform parent, Hand.Piece piece, GameConfig cfg)
        {
            var view = new GameObject("Piece").AddComponent<PieceView>();
            view.transform.SetParent(parent, false);
            view.cfg = cfg;
            view.Piece = piece;

            Shape shape = piece.Shape;
            Vector2 offset = new Vector2(shape.Width - 1, shape.Height - 1) * 0.5f;
            view.renderers = new SpriteRenderer[shape.Cells.Length];
            for (int i = 0; i < shape.Cells.Length; i++)
            {
                Vector2 local = ((Vector2)shape.Cells[i] - offset) * cfg.cellSize;
                view.renderers[i] = CellSprite.Create(view.transform, cfg, local, 5);
                view.renderers[i].color = cfg.ColorOf(piece.ColorId);
            }
            return view;
        }

        public void SetDragging(bool dragging)
        {
            float s = dragging ? 1f : cfg.handPieceScale;
            transform.localScale = new Vector3(s, s, 1f);
            foreach (var r in renderers) r.sortingOrder = dragging ? 10 : 5;
        }

        /// <summary>World center of the shape's bottom-left cell; used to find the drop cell.</summary>
        public Vector2 BottomLeftCellCenter()
        {
            Shape shape = Piece.Shape;
            Vector2 offset = new Vector2(shape.Width - 1, shape.Height - 1) * 0.5f * cfg.cellSize;
            return (Vector2)transform.position - offset * transform.localScale.x;
        }
    }
}
