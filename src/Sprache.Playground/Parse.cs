using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace Sprache.Playground
{
    public static class Parse
    {
        public static Parser<char> AnyChar = context =>
        {
            if (context.AtEnd)
                return Result.WithoutValue<char>("any character", context, new ParseError(context.Index, "Expected: any character"));

            return Result.WithValue(context.Current, "any character", context.Advance());
        };

        public static Parser<string> String(string expected)
        {
            if (expected == null) throw new ArgumentNullException("expected");

            return context =>
            {
                if (expected.Length == 0)
                    return Result.WithValue("", expected, context);

                var input = context.Input;
                if(input.Length - context.Index < expected.Length)
                    return Result.WithoutValue<String>(expected, context, new ParseError(context.Index, "Unexpected end of input"));

                string sample = input.Substring(context.Index, expected.Length);
                if (sample == expected)
                    return Result.WithValue(expected, expected, context.Advance(sample.Length));

                return Result.WithoutValue<String>(expected, context, new ParseError(context.Index, string.Format("Expected string: \"{0}\"", expected)));
            };
        }

        public static Parser<T> InterleaveWith<T>(this Parser<T> @this, Parser<Object> interleaving)
        {
            return context =>
            {
                interleaving = context.Interleaving != null
                                   ? context.Interleaving.Or(interleaving)
                                   : interleaving;

                var newContext = context.WithInterleave(interleaving);

                return @this(newContext);
            };
        }

        public static Parser<T> Interleave<T>(this Parser<T> @this)
        {
            return context =>
            {
                if (context.Interleaving != null)
                {
                    var result = context.Interleaving(context);
                    while (result.HasValue)
                    {
                        context = result.Context;
                        result = context.Interleaving(context);
                    }
                }

                return @this(context);
            };
        }

        private class OrParser<T>
        {
            public static OrParser<T> Or(Parser<T> left, Parser<T> right)
            {
                var l = left.Target as OrParser<T>;
                var r = right.Target as OrParser<T>;

                if (l != null && r != null)
                    return new OrParser<T>(l.Parsers.Concat(r.Parsers));

                if(l != null)
                    return new OrParser<T>(l.Parsers.Concat(new []{right}));

                if(r != null)
                    return new OrParser<T>(new []{left}.Concat(r.Parsers));

                return new OrParser<T>(new []{left, right});
            }

            private OrParser(IEnumerable<Parser<T>> parsers)
            {
                Parsers = parsers.ToArray();
            }

            public IResult<T> Parse(Context context)
            {
                var results = new List<IResult<T>>();

                foreach (var parser in Parsers)
                {
                    var result = parser(context);
                    if (result.HasValue)
                        return result;

                    results.Add(result);
                }

                string expected = string.Join(" or ", results.Select(x => x.Description));

                return Result.WithoutValue<T>("or",
                    context,
                    new ParseError(
                        context.Index,
                        string.Format("Expected: {0}", expected),
                        results.SelectMany(x => x.Errors)));
            }

            public IEnumerable<Parser<T>> Parsers { get; private set; }
        }

        public static Parser<T> Or<T>(this Parser<T> first, Parser<T> second)
        {
            return OrParser<T>.Or(first, second).Parse;
        }

        public static Parser<U> Select<T, U>(this Parser<T> @this, Func<T, U> selector)
        {
            return context =>
            {
                var result = @this(context);
                if (result.HasValue)
                    return Result.WithValue(selector(result.Value), result.Description, result.Context, result.Errors);

                return Result.WithoutValue<U>(result.Description, result.Context, result.Errors);
            };
        }

        public static Parser<U> SelectMany<T, U>(this Parser<T> @this, Func<T, Parser<U>> selector)
        {
            return context =>
            {
                var result = @this.Interleave()(context);
                if (result.HasValue)
                {
                    var nextResult = selector(result.Value).Interleave()(result.Context);
                    return nextResult;
                }

                return Result.WithoutValue<U>(result.Description, result.Context, result.Errors);
            };
        }

        public static Parser<T> DescribeAs<T>(this Parser<T> @this, string description)
        {
            return context =>
            {
                var result = @this(context);
                return result.HasValue 
                    ? Result.WithValue(result.Value, description, result.Context, result.Errors) 
                    : Result.WithoutValue<T>(description, result.Context, result.Errors);
            };
        }

        public static Parser<TResult> SelectMany<T, TIntermediate, TResult>(this Parser<T> @this,
                                                                            Func<T, Parser<TIntermediate>> selector,
                                                                            Func<T, TIntermediate, TResult> combiner)
        {
            return SelectMany(@this, x => selector(x).Select(y => combiner(x, y)));
        }
    }
}