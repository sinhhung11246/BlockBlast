using System.Collections.Generic;
using UnityEngine;

namespace BlockBlast.Core
{
    /// <summary>Square grid. A cell holds 0 when empty, otherwise the color id of its block.</summary>
    public sealed class Board
    {
        public readonly int Size;
        readonly int[,] cells;

        public Board(int size)
        {
            Size = size;
            cells = new int[size, size];
        }

        public int this[int x, int y] => cells[x, y];

        public Board Clone()
        {
            var copy = new Board(Size);
            System.Array.Copy(cells, copy.cells, cells.Length);
            return copy;
        }

        public bool InBounds(Vector2Int p) => p.x >= 0 && p.y >= 0 && p.x < Size && p.y < Size;

        public void Clear() => System.Array.Clear(cells, 0, cells.Length);

        public bool CanPlace(Shape shape, Vector2Int origin)
        {
            foreach (var c in shape.Cells)
            {
                var p = origin + c;
                if (!InBounds(p) || cells[p.x, p.y] != 0) return false;
            }
            return true;
        }

        public void Place(Shape shape, Vector2Int origin, int colorId)
        {
            foreach (var c in shape.Cells)
            {
                var p = origin + c;
                cells[p.x, p.y] = colorId;
            }
        }

        public bool FitsAnywhere(Shape shape)
        {
            for (int x = 0; x <= Size - shape.Width; x++)
                for (int y = 0; y <= Size - shape.Height; y++)
                    if (CanPlace(shape, new Vector2Int(x, y))) return true;
            return false;
        }

        /// <summary>Full rows and columns, found together before anything is removed.</summary>
        public LineSet FindFullLines()
        {
            var lines = new LineSet();
            for (int i = 0; i < Size; i++)
            {
                if (IsRowFull(i)) lines.Rows.Add(i);
                if (IsColumnFull(i)) lines.Columns.Add(i);
            }
            return lines;
        }

        /// <summary>Lines a placement would complete, without changing the board. Placement must be valid.</summary>
        public LineSet LinesCompletedBy(Shape shape, Vector2Int origin)
        {
            Place(shape, origin, -1);
            LineSet lines = FindFullLines();
            Place(shape, origin, 0);
            return lines;
        }

        public void ClearLines(LineSet lines)
        {
            foreach (int y in lines.Rows)
                for (int x = 0; x < Size; x++) cells[x, y] = 0;
            foreach (int x in lines.Columns)
                for (int y = 0; y < Size; y++) cells[x, y] = 0;
        }

        bool IsRowFull(int y)
        {
            for (int x = 0; x < Size; x++) if (cells[x, y] == 0) return false;
            return true;
        }

        bool IsColumnFull(int x)
        {
            for (int y = 0; y < Size; y++) if (cells[x, y] == 0) return false;
            return true;
        }
    }

    public sealed class LineSet
    {
        public readonly List<int> Rows = new List<int>();
        public readonly List<int> Columns = new List<int>();
        public int Count => Rows.Count + Columns.Count;
    }
}
