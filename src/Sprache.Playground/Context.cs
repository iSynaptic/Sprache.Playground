using System;

namespace Sprache.Playground
{
    public class Context
    {
        public readonly int Index;
        public readonly string Input;
        public readonly char Current;
        public readonly string Name;
        public readonly bool AtEnd;

        public readonly Parser<object> Interleaving;

        public Context(string input, string name)
            : this(input, name, null, 0)
        {
        }

        public Context(string input, string name, Parser<object> interleaving)
            : this(input, name, interleaving, 0)
        {
        }

        private Context(string input, string name, Parser<object> interleaving, int index)
        {
            if (input == null) throw new ArgumentNullException("input");

            Input = input;
            AtEnd = input.Length == index;

            if (!AtEnd)
                Current = input[index];

            Name = name ?? "";
            Interleaving = interleaving;
            Index = index;
        }

        public Context Advance() { return Advance(1); }

        public Context Advance(int count)
        {
            if (count <= 0)
                throw new ArgumentOutOfRangeException("count");

            int newIndex = Index + count;

            if (AtEnd || newIndex > Input.Length)
                throw new ArgumentException(String.Format("Advancing {0} charaters exceeds the length of the input.", count), "count");

            return new Context(Input, Name, Interleaving, newIndex);
        }

        public Context WithInterleave(Parser<Object> interleaving)
        {
            return new Context(Input, Name, interleaving, Index);
        }

        public static implicit operator Context(string content)
        {
            return new Context(content, null);
        }
    }
}