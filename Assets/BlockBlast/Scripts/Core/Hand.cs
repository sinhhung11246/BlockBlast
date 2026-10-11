using System.Collections.Generic;
using UnityEngine;

namespace BlockBlast.Core
{
    /// <summary>The pieces offered to the player. A used slot is null until the hand is refilled.</summary>
    public sealed class Hand
    {
        public struct Piece
        {
            public Shape Shape;
            public int ColorId;
        }

        readonly Piece?[] slots;
        readonly Shape[] pool;
        readonly int colorCount;
        readonly System.Random random;

        public int Count => slots.Length;
        public Piece? this[int i] => slots[i];

        /// <param name="colorCount">Color ids are 1..colorCount (0 means empty on the board).</param>
        public Hand(int size, Shape[] pool, int colorCount, System.Random random)
        {
            slots = new Piece?[size];
            this.pool = pool;
            this.colorCount = colorCount;
            this.random = random;
        }

        public bool IsEmpty
        {
            get
            {
                foreach (var s in slots) if (s.HasValue) return false;
                return true;
            }
        }

        /// <summary>
        /// Deals a new hand, rerolling up to <paramref name="attempts"/> times until the whole hand can be
        /// placed in some order (clears included). Failing that, keeps the first roll where a piece fits,
        /// else the last roll (the board is truly out of space).
        /// </summary>
        /// <returns>True when the dealt hand is solvable.</returns>
        public bool Refill(Board board, int attempts)
        {
            Piece?[] fallback = null;
            for (int a = 0; a < attempts; a++)
            {
                Roll();
                if (Solvable(board)) return true;
                if (fallback == null && AnyFits(board)) fallback = (Piece?[])slots.Clone();
            }
            if (fallback != null) System.Array.Copy(fallback, slots, slots.Length);
            return false;
        }

        /// <summary>Deals copies of the smallest shape, e.g. after a revive found no solvable roll.</summary>
        public void DealSmallest()
        {
            Shape smallest = pool[0];
            foreach (var s in pool) if (s.Cells.Length < smallest.Cells.Length) smallest = s;
            for (int i = 0; i < slots.Length; i++)
                slots[i] = new Piece { Shape = smallest, ColorId = random.Next(colorCount) + 1 };
        }

        /// <summary>True when every remaining piece can be placed in some order, clearing lines along the way.</summary>
        public bool Solvable(Board board)
        {
            var left = new List<Shape>();
            foreach (var s in slots) if (s.HasValue) left.Add(s.Value.Shape);
            return Solvable(board, left);
        }

        static bool Solvable(Board board, List<Shape> left)
        {
            if (left.Count == 0) return true;
            if (left.Count == 1) return board.FitsAnywhere(left[0]);

            for (int i = 0; i < left.Count; i++)
            {
                Shape shape = left[i];
                if (left.IndexOf(shape) < i) continue; // same shape already tried first
                var rest = new List<Shape>(left);
                rest.RemoveAt(i);

                for (int x = 0; x <= board.Size - shape.Width; x++)
                    for (int y = 0; y <= board.Size - shape.Height; y++)
                    {
                        var origin = new Vector2Int(x, y);
                        if (!board.CanPlace(shape, origin)) continue;
                        Board next = board.Clone();
                        next.Place(shape, origin, 1);
                        next.ClearLines(next.FindFullLines());
                        if (Solvable(next, rest)) return true;
                    }
            }
            return false;
        }

        void Roll()
        {
            for (int i = 0; i < slots.Length; i++)
                slots[i] = new Piece
                {
                    Shape = pool[random.Next(pool.Length)],
                    ColorId = random.Next(colorCount) + 1,
                };
        }

        public void Take(int i) => slots[i] = null;

        /// <summary>True when at least one remaining piece fits on the board.</summary>
        public bool AnyFits(Board board)
        {
            foreach (var s in slots)
                if (s.HasValue && board.FitsAnywhere(s.Value.Shape)) return true;
            return false;
        }
    }
}
