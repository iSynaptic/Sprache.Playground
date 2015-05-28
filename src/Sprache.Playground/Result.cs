using System.Collections.Generic;

namespace Sprache.Playground
{
    public static class Result
    {
        public static IResult<T> WithValue<T>(T value, string description, Context context, params ParseError[] errors) 
            => WithValue(value, description, context, (IEnumerable<ParseError>)errors);

        public static IResult<T> WithValue<T>(T value, string description, Context context, IEnumerable<ParseError> errors) 
            => new Result<T>(value, true, description, context, errors);

        public static IResult<T> WithoutValue<T>(string description, Context context, params ParseError[] errors) 
            => WithoutValue<T>(description, context, (IEnumerable<ParseError>)errors);

        public static IResult<T> WithoutValue<T>(string description, Context context, IEnumerable<ParseError> errors) 
            => new Result<T>(default(T), false, description, context, errors);
    }

    internal class Result<T> : IResult<T>
    {
        public Result(T value, bool hasValue, string description, Context context, IEnumerable<ParseError> errors)
        {
            Value = value;
            HasValue = hasValue;
            Description = description;
            Context = context;
            Errors = errors ?? new ParseError[0];
        }

        public T Value { get; }
        public bool HasValue { get; }
        public string Description { get; }

        public Context Context { get; }

        public IEnumerable<ParseError> Errors { get; }
    }

}