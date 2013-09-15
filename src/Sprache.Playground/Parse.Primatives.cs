using System;
using System.Collections.Generic;
using System.Linq;

namespace Sprache.Playground
{
    public static partial class Parse
    {
        public static Parser<char> AnyChar = context =>
        {
            if (context.AtEnd)
                return Result.WithoutValue<char>("any character", context, new ParseError(context.ReadTo, "Expected: any character"));

            return Result.WithValue(context.Current, "any character", context.ReadAndConsume(1));
        };

        public static Parser<string> String(string expected)
        {
            if (expected == null) throw new ArgumentNullException("expected");

            return context =>
            {
                if (expected.Length == 0)
                    return Result.WithValue("", expected, context);

                var input = context.Input;
                if(input.Length - context.ReadTo.Index < expected.Length)
                    return Result.WithoutValue<String>(expected, context, new ParseError(context.ReadTo, "Unexpected: end of input"));

                if (string.Compare(context.Input, context.ReadTo.Index, expected, 0, expected.Length) == 0)
                    return Result.WithValue(expected, expected, context.ReadAndConsume(expected.Length));

                return Result.WithoutValue<String>(expected, context, new ParseError(context.ReadTo, string.Format("Expected string: \"{0}\"", expected)));
            };
        }
    }
}