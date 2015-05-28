using System;

namespace Sprache.Playground
{
    public struct PositionRange : IEquatable<PositionRange>
    {
        public PositionRange(Position start, Position end)
        {
            if (end < start)
                throw new ArgumentOutOfRangeException(nameof(end), "End position must not be before the start position.");

            Start = start;
            End = end;
        }

        public bool Equals(PositionRange other) 
            => Start.Equals(other.Start) &&
               End.Equals(other.End);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null)) return false;
            if (GetType() != obj.GetType()) return false;

            return Equals((PositionRange)obj);
        }

        public override int GetHashCode()
        {
            int hash = 1;

            hash = Hashing.MixJenkins32(hash + Start.GetHashCode());
            hash = Hashing.MixJenkins32(hash + End.GetHashCode());

            return hash;
        }

        public static Boolean operator ==(PositionRange left, PositionRange right)
            => left.Equals(right);

        public static Boolean operator !=(PositionRange left, PositionRange right)
            => !(left.Equals(right));

        public Position Start { get; }
        public Position End { get; }
    }
}
