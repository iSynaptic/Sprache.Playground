using System;

namespace Sprache.Playground
{
    public class Context
    {
        public readonly Position Position;
        public readonly string Input;
        public readonly char Current;
        public readonly string Name;
        public readonly bool AtEnd;

        public readonly Parser<object> Interleaving;

        public Context(string input)
            : this(input, null)
        {
        }

        public Context(string input, string name)
            : this(input, name, null)
        {
        }

        public Context(string input, string name, Parser<object> interleaving)
            : this(input, name, interleaving, default(Position))
        {
        }

        private Context(string input, string name, Parser<object> interleaving, Position position)
        {
            if (input == null) throw new ArgumentNullException("input");

            Input = input;
            AtEnd = input.Length == position.Index;

            if (!AtEnd)
                Current = input[position.Index];

            Name = name ?? "";
            Interleaving = interleaving;
            Position = position;
        }

        public Context Advance() { return Advance(1); }

        public Context Advance(int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException("count");

            if (count == 0)
                return this;

            int newIndex = Position.Index + count;

            if (AtEnd || newIndex > Input.Length)
                throw new ArgumentException(String.Format("Advancing {0} charaters exceeds the length of the input.", count), "count");

            var position = Position;
            while (position.Index < newIndex)
            {
                position = Input[position.Index] == '\n' 
                    ? position.AdvanceWithNewLine() 
                    : position.Advance();
            }

            return new Context(Input, Name, Interleaving, position);
        }

        public Context WithInterleave(Parser<Object> interleaving)
        {
            return new Context(Input, Name, interleaving, Position);
        }

        public static implicit operator Context(string content)
        {
            return new Context(content, null);
        }
    }
}