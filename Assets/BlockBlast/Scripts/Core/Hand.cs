using System.Collections.Generic;
using UnityEngine;

namespace BlockBlast.Core
{
    /// <summary>
    /// The pieces offered to the player. A used slot is null until the hand is refilled.
    /// Shapes are picked by weight; on a crowded board the dealer keeps the friendliest of its rolls.
    /// </summary>
    public sealed class Hand
    {
        public struct Piece
        {
            public Shape Shape;
            public int ColorId;
        }

        /// <summary>
        /// Mercy deal: once the board is more than MercyFillStart full, a solvable roll scores
        /// pressure * (ClearWeight * lines its pieces can clear - CellWeight * its cell count),
        /// where pressure rises from 0 at MercyFillStart to 1 on a full board, and the best roll is kept.
        /// </summary>
        public struct Rules
        {
            public float MercyFillStart;
            public float ClearWeight;
            public float CellWeight;
        }

        readonly Piece?[] slots;
        readonly Shape[] pool;
        readonly float[] cumulativeWeights;
        readonly int colorCount;
        readonly System.Random random;
        readonly Rules rules;

        public int Count => slots.Length;
        public Piece? this[int i] => slots[i];

        /// <param name="weights">Relative chance of each pool shape.</param>
        /// <param name="colorCount">Color ids are 1..colorCount (0 means empty on the board).</param>
        public Hand(int size, Shape[] pool, float[] weights, int colorCount, System.Random random, Rules rules)
        {
            slots = new Piece?[size];
            this.pool = pool;
            this.colorCount = colorCount;
            this.random = random;
            this.rules = rules;

            cumulativeWeights = new float[weights.Length];
            float total = 0f;
            for (int i = 0; i < weights.Length; i++) cumulativeWeights[i] = total += Mathf.Max(0f, weights[i]);
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
        /// Deals a new hand from up to <paramref name="attempts"/> rolls. Only rolls the whole hand can be placed from
        /// (in some order, clears included) count; below the mercy fill the first such roll is kept, above it the best
        /// scoring one. With none, keeps the first roll where a piece fits, else the last roll (the board is truly out of space).
        /// </summary>
        /// <returns>True when the dealt hand is solvable.</returns>
        public bool Refill(Board board, int attempts)
        {
            float pressure = Mathf.InverseLerp(rules.MercyFillStart, 1f, FillRatio(board));
            Piece?[] best = null, fallback = null;
            float bestScore = float.MinValue;
            for (int a = 0; a < attempts; a++)
            {
                Roll();
                if (!Solvable(board))
                {
                    if (fallback == null && AnyFits(board)) fallback = (Piece?[])slots.Clone();
                    continue;
                }
                if (pressure <= 0f) return true;

                float score = pressure * (rules.ClearWeight * ClearableLines(board) - rules.CellWeight * CellCount());
                if (score <= bestScore) continue;
                bestScore = score;
                best = (Piece?[])slots.Clone();
            }

            Piece?[] keep = best ?? fallback;
            if (keep != null) System.Array.Copy(keep, slots, slots.Length);
            return best != null;
        }

        /// <summary>Deals copies of the smallest shape (a single cell), e.g. after a revive found no solvable roll.</summary>
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

        /// <summary>Sum over the pieces of the most lines each could clear with one placement on the current board.</summary>
        int ClearableLines(Board board)
        {
            int total = 0;
            foreach (var s in slots)
            {
                if (!s.HasValue) continue;
                Shape shape = s.Value.Shape;
                int most = 0;
                for (int x = 0; x <= board.Size - shape.Width; x++)
                    for (int y = 0; y <= board.Size - shape.Height; y++)
                    {
                        var origin = new Vector2Int(x, y);
                        if (board.CanPlace(shape, origin)) most = Mathf.Max(most, board.LinesCompletedBy(shape, origin).Count);
                    }
                total += most;
            }
            return total;
        }

        int CellCount()
        {
            int total = 0;
            foreach (var s in slots) if (s.HasValue) total += s.Value.Shape.Cells.Length;
            return total;
        }

        static float FillRatio(Board board)
        {
            int filled = 0;
            for (int x = 0; x < board.Size; x++)
                for (int y = 0; y < board.Size; y++)
                    if (board[x, y] != 0) filled++;
            return (float)filled / (board.Size * board.Size);
        }

        void Roll()
        {
            for (int i = 0; i < slots.Length; i++)
                slots[i] = new Piece { Shape = pool[PickShape()], ColorId = random.Next(colorCount) + 1 };
        }

        /// <summary>Index of a pool shape, picked by weight.</summary>
        int PickShape()
        {
            float r = (float)random.NextDouble() * cumulativeWeights[cumulativeWeights.Length - 1];
            for (int i = 0; i < cumulativeWeights.Length - 1; i++)
                if (r < cumulativeWeights[i]) return i;
            return cumulativeWeights.Length - 1;
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
