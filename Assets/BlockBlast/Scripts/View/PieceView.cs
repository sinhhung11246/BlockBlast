using BlockBlast.Core;
using BlockBlast.Data;
using UnityEngine;

namespace BlockBlast.View
{
    /// <summary>One hand piece, drawn centered on its transform from Block prefab cells.</summary>
    public sealed class PieceView : MonoBehaviour
    {
        const int HandOrder = 5, DragOrder = 10;

        GameConfig cfg;
        SpriteRenderer[] renderers;

        public Hand.Piece Piece { get; private set; }

        public static PieceView Create(SpriteRenderer block, Transform parent, Hand.Piece piece, GameConfig cfg)
        {
            var view = new GameObject("Piece").AddComponent<PieceView>();
            view.transform.SetParent(parent, false);
            view.cfg = cfg;
            view.Piece = piece;

            Shape shape = piece.Shape;
            Sprite sprite = cfg.BlockSprite(piece.ColorId);
            view.renderers = new SpriteRenderer[shape.Cells.Length];
            for (int i = 0; i < shape.Cells.Length; i++)
                view.renderers[i] = SpriteFactory.Spawn(block, view.transform, sprite, (Vector2)shape.Cells[i] - view.HalfExtent(), 1f, HandOrder);
            return view;
        }

        public void SetDragging(bool dragging)
        {
            float s = dragging ? 1f : cfg.handPieceScale;
            transform.localScale = new Vector3(s, s, 1f);
            foreach (var r in renderers) r.sortingOrder = dragging ? DragOrder : HandOrder;
        }

        /// <summary>Swaps every cell to the given sprite, e.g. grey when the piece cannot be placed.</summary>
        public void SetSprite(Sprite sprite)
        {
            foreach (var r in renderers) SpriteFactory.SetSprite(r, sprite, 1f);
        }

        /// <summary>World center of the shape's bottom-left cell; used to find the drop cell.</summary>
        public Vector2 BottomLeftCellCenter() => (Vector2)transform.position - HalfExtent() * transform.localScale.x;

        Vector2 HalfExtent() => new Vector2(Piece.Shape.Width - 1, Piece.Shape.Height - 1) * 0.5f;
    }
}
