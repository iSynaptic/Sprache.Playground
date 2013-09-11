using System.Collections.Generic;
using System.Linq;

namespace Sprache.Playground
{
    public class ParseError
    {
        public ParseError(int index, string message) 
            : this(index, message, null)
        {
        }

        public ParseError(int index, string message, IEnumerable<ParseError> errors)
        {
            Index = index;
            Message = message;
            Errors = (errors ?? new ParseError[0]).ToArray();
        }

        public int Index { get; private set; }
        public string Message { get; private set; }

        public IEnumerable<ParseError> Errors { get; private set; }
    }
}