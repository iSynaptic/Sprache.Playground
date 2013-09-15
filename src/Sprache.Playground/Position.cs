using System;

namespace Sprache.Playground
{
    public struct Position
    {
        private readonly int _index;
        private readonly int _line;
        private readonly int _column;

        public Position(int index, int line, int column)
            : this()
        {
            if(index < 0) throw new ArgumentOutOfRangeException("index", "Index must not be negative.");
            if (line <= 0) throw new ArgumentOutOfRangeException("line", "Line must be greater than or equal to one.");
            if (column <= 0) throw new ArgumentOutOfRangeException("column", "Column must be greater than or equal to one.");

            _index = index;
            _line = line;
            _column = column;
        }

        public Position Advance()
        {
            return new Position(Index + 1, Line, Column + 1);
        }

        public Position AdvanceWithNewLine()
        {
            return new Position(Index + 1, Line + 1, 1);
        }

        public int Index { get { return _index; } }

        public int Line { get { return _line == 0 ? 1 : _line; } }
        public int Column { get { return _column == 0 ? 1 : _column; } }
    }
}