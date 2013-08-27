using System;
using System.Collections.Generic;

namespace Sprache.Playground
{
    public interface IResult<out T>
    {
        T Value { get; }
        Boolean HasValue { get; }
        String Description { get; }

        Context Context { get; }

        IEnumerable<ParseError> Errors { get; }
    }
}