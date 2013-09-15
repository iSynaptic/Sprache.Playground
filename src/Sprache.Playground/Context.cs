using System;

namespace Sprache.Playground
{
    public class Context
    {
        public readonly Position ConsumedTo;
        public readonly Position ReadTo;

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
            : this(input, name, interleaving, default(Position), default(Position))
        {
        }

        private Context(string input, string name, Parser<object> interleaving, Position consumedTo, Position readTo)
        {
            if (input == null) throw new ArgumentNullException("input");

            Input = input;
            AtEnd = input.Length == readTo.Index;

            if (!AtEnd)
                Current = input[readTo.Index];

            Name = name ?? "";
            Interleaving = interleaving;

            ConsumedTo = consumedTo;
            ReadTo = readTo;
        }

        public Context Read(int count)
        {
            return Move(count, false);
        }

        public Context ReadAndConsume(int count)
        {
            return Move(count, true);
        }

        public Context Consume()
        {
            return Move(0, true);
        }

        private Context Move(int count, bool consume)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException("count");

            if (count == 0 && consume && ConsumedTo == ReadTo)
                return this;

            int newIndex = ReadTo.Index + count;

            if (AtEnd || newIndex > Input.Length)
                throw new ArgumentException(String.Format("Advancing {0} charaters exceeds the length of the input.", count), "count");

            var newReadTo = ReadTo;
            while (newReadTo.Index < newIndex)
            {
                newReadTo = Input[newReadTo.Index] == '\n'
                    ? newReadTo.AdvanceWithNewLine()
                    : newReadTo.Advance();
            }

            return new Context(Input, Name, Interleaving, consume ? newReadTo : ConsumedTo, newReadTo);
        }

        public Context WithInterleave(Parser<Object> interleaving)
        {
            return new Context(Input, Name, interleaving, ConsumedTo, ReadTo);
        }

        public static implicit operator Context(string content)
        {
            return new Context(content, null);
        }
    }
}