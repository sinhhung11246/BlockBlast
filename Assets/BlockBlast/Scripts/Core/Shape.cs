using System.Collections.Generic;
using UnityEngine;

namespace BlockBlast.Core
{
    /// <summary>Immutable block shape. Cells are relative to its bottom-left corner.</summary>
    public sealed class Shape
    {
        public readonly Vector2Int[] Cells;
        public readonly int Width;
        public readonly int Height;

        Shape(Vector2Int[] cells, int width, int height)
        {
            Cells = cells;
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Parses a pattern: rows separated by '/', top row first, '#' = filled, '.' = empty.
        /// Example: "#./#./##" is an L shape.
        /// </summary>
        public static Shape Parse(string pattern)
        {
            string[] rows = pattern.Split('/');
            int height = rows.Length;
            int width = 0;
            var cells = new List<Vector2Int>();

            for (int r = 0; r < height; r++)
            {
                string row = rows[r];
                if (row.Length > width) width = row.Length;
                for (int x = 0; x < row.Length; x++)
                    if (row[x] == '#') cells.Add(new Vector2Int(x, height - 1 - r));
            }

            return new Shape(cells.ToArray(), width, height);
        }
    }
}
