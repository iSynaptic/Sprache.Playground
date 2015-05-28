using System;

namespace Sprache.Playground
{
    public static partial class Parse
    {
        public static Parser<char> AnyChar = context =>
        {
            return context.AtEnd
                ? Result.WithoutValue<char>("any character", context, new ParseError(context.ReadTo.ToRange(), "Unexpected: end of input"))
                : Result.WithValue(context.Current, "any character", context.ReadAndConsume(1));
        };

        public static readonly Parser<string> Empty =
            ctx => Result.WithValue("", "empty", ctx);

        public static Parser<string> Literal(string expected) => Literal(expected, "literal");

        public static Parser<string> Literal(string expected, string description)
        {
            if (expected == null) throw new ArgumentNullException(nameof(expected));
            if (expected == string.Empty) return Empty;

            return context =>
            {
                if(context.AtEnd)
                    return Result.WithoutValue<string>(description, context, new ParseError(context.ReadTo.ToRange(), "Unexpected: end of input"));

                string input = context.Input;
                int offset = context.ConsumedTo.Index;
                int length = expected.Length;

                for (int i = 0; i < length; i++)
                {
                    if (i == input.Length)
                    {
                        var newContext = context.Read(i);
                        return Result.WithoutValue<string>(description, newContext, new ParseError(newContext.ReadTo.ToRange(), "Unexpected: end of input"));
                    }

                    if (input[offset + i] != expected[i])
                    {
                        var newContext = context.Read(i);
                        return Result.WithoutValue<string>(description, newContext, new ParseError(newContext.ReadTo.ToRange(), string.Format("Unexpected: character '{0}'", input[offset + i])));
                    }
                }

                return Result.WithValue(expected, description, context.ReadAndConsume(length));
            };
        }
    }
}