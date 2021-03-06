﻿using System;

namespace Sprache.Playground
{
    public struct Position : IEquatable<Position>, IComparable<Position>
    {
        public static readonly Position Beginning = new Position(0, 1, 1);

        private readonly int _index;
        private readonly int _line;
        private readonly int _column;

        public Position(int index, int line, int column)
            : this()
        {
            if (index < 0) throw new ArgumentOutOfRangeException(nameof(index), "Index must not be negative.");
            if (line <= 0) throw new ArgumentOutOfRangeException(nameof(line), "Line must be greater than or equal to one.");
            if (column <= 0) throw new ArgumentOutOfRangeException(nameof(column), "Column must be greater than or equal to one.");

            _index = index;
            _line = line;
            _column = column;
        }

        public Position Advance() 
            => new Position(Index + 1, Line, Column + 1);

        public Position AdvanceWithNewLine()
            => new Position(Index + 1, Line + 1, 1);

        public bool Equals(Position other)
            => Index == other.Index &&
               Line == other.Line &&
               Column == other.Column;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null)) return false;
            if (GetType() != obj.GetType()) return false;

            return Equals((Position)obj);
        }

        public override int GetHashCode()
        {
            int hash = 1;

            hash = Hashing.MixJenkins32(hash + Index);
            hash = Hashing.MixJenkins32(hash + Line);
            hash = Hashing.MixJenkins32(hash + Column);

            return hash;
        }

        public PositionRange ToRange() => new PositionRange(this, this);

        public int CompareTo(Position other) => Index.CompareTo(other.Index);

        public static Boolean operator ==(Position left, Position right) 
            => left.Equals(right);

        public static Boolean operator !=(Position left, Position right) 
            => !(left.Equals(right));

        public static Boolean operator <(Position left, Position right)
            => left.Index < right.Index;

        public static Boolean operator >(Position left, Position right)
            => left.Index < right.Index;

        public static Boolean operator <=(Position left, Position right)
            => left.Index <= right.Index;

        public static Boolean operator >=(Position left, Position right)
            => left.Index <= right.Index;

        public int Index => _index;
        public int Line => _line == 0 ? 1 : _line;
        public int Column => _column == 0 ? 1 : _column;
    }
}