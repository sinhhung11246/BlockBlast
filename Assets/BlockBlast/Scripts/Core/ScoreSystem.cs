namespace BlockBlast.Core
{
    /// <summary>
    /// Score per placement:
    ///   cells * PointsPerCell
    /// + (lines * PointsPerLine + (lines - 1) * MultiLineBonus) * Combo   (only when lines > 0)
    /// Combo rises by 1 on every placement that clears, and resets after
    /// ComboBreakAfterMisses placements in a row that clear nothing.
    /// </summary>
    public sealed class ScoreSystem
    {
        public struct Rules
        {
            public int PointsPerCell;
            public int PointsPerLine;
            public int MultiLineBonus;
            public int ComboBreakAfterMisses;
        }

        readonly Rules rules;
        int misses;

        public int Score { get; private set; }
        public int Combo { get; private set; }

        public ScoreSystem(Rules rules) => this.rules = rules;

        public void Reset()
        {
            Score = 0;
            Combo = 0;
            misses = 0;
        }

        /// <returns>Points gained by this placement.</returns>
        public int AddPlacement(int cellCount, int linesCleared)
        {
            int gained = cellCount * rules.PointsPerCell;

            if (linesCleared > 0)
            {
                Combo++;
                misses = 0;
                gained += (linesCleared * rules.PointsPerLine + (linesCleared - 1) * rules.MultiLineBonus) * Combo;
            }
            else if (++misses >= rules.ComboBreakAfterMisses)
            {
                Combo = 0;
            }

            Score += gained;
            return gained;
        }
    }
}
