using System;

namespace Sprache.Playground
{
    public static partial class Parse
    {
        public static Parser<char> AnyChar = context =>
        {
            if (context.AtEnd)
                return Result.WithoutValue<char>("any character", context, new ParseError(context.ReadTo, "Unexpected: end of input"));

            return Result.WithValue(context.Current, "any character", context.ReadAndConsume(1));
        };

        public static readonly Parser<string> Empty =
            ctx => Result.WithValue("", "empty", ctx);

        public static Parser<string> Literal(string expected)
        {
            return Literal(expected, "literal");
        }

        public static Parser<string> Literal(string expected, string description)
        {
            if (expected == null) throw new ArgumentNullException("expected");
            if (expected == string.Empty) return Empty;

            return context =>
            {
                if(context.AtEnd)
                    return Result.WithoutValue<String>(description, context, new ParseError(context.ConsumedTo, "Unexpected: end of input"));

                string input = context.Input;
                int offset = context.ConsumedTo.Index;
                int length = expected.Length;

                for (int i = 0; i < length; i++)
                {
                    if (i == input.Length)
                    {
                        var newContext = context.Read(i);
                        return Result.WithoutValue<String>(description, newContext, new ParseError(newContext.ReadTo, "Unexpected: end of input"));
                    }

                    if (input[offset + i] != expected[i])
                    {
                        var newContext = context.Read(i);
                        return Result.WithoutValue<String>(description, newContext, new ParseError(newContext.ReadTo, string.Format("Unexpected: character '{0}'", input[offset + i])));
                    }
                }

                return Result.WithValue(expected, description, context.ReadAndConsume(length));
            };
        }
    }
}