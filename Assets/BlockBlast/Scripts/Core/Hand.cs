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

        public void Refill()
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
