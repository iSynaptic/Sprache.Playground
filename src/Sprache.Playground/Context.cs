using System;

namespace Sprache.Playground
{
    public class Context
    {
        public readonly Position ConsumedTo;
        public readonly Position ReadTo;

        public readonly string Input;
        public readonly string Name;

        public bool AtEnd => Input.Length == ReadTo.Index;
        public char Current => !AtEnd ? Input[ReadTo.Index] : default(char);

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
            if (input == null) throw new ArgumentNullException(nameof(input));

            Input = input;
            Name = name ?? "";
            Interleaving = interleaving;

            ConsumedTo = consumedTo;
            ReadTo = readTo;
        }

        public Context Read(int count) => Move(count, false);
        public Context ReadAndConsume(int count) => Move(count, true);
        public Context Consume() => Move(0, true);

        private Context Move(int count, bool consume)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            if (count == 0 && consume && ConsumedTo == ReadTo)
                return this;

            int newIndex = ReadTo.Index + count;

            if (AtEnd || newIndex > Input.Length)
                throw new ArgumentException(String.Format("Advancing {0} charaters exceeds the length of the input.", count), nameof(count));

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
            => new Context(Input, Name, interleaving, ConsumedTo, ReadTo);

        public static implicit operator Context(string content) => new Context(content, null);
    }
}