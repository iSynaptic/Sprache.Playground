﻿using System;
using System.Linq;

namespace Sprache.Playground
{
    public static partial class Parse
    {
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

        public static Parser<T> Or<T>(this Parser<T> first, Parser<T> second)
        {
            return context =>
            {
                var firstResult = first(context);
                if (firstResult.HasValue)
                    return firstResult;

                var secondResult = second(context);
                if (secondResult.HasValue)
                    return secondResult;

                return Result.WithoutValue<T>("or",
                    context,
                    new ParseError(
                        context.ReadTo,
                        string.Format("Expected: {0} or {1}", firstResult.Description, secondResult.Description),
                        firstResult.Errors.Concat(secondResult.Errors)));
            };
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