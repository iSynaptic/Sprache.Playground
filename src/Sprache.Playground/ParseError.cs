using System.Collections.Generic;
using System.Linq;

namespace Sprache.Playground
{
    public class ParseError
    {
        public ParseError(Position position, string message)
            : this(position, message, null)
        {
        }

        public ParseError(Position position, string message, IEnumerable<ParseError> errors)
        {
            Position = position;
            Message = message;
            Errors = (errors ?? new ParseError[0]).ToArray();
        }

        public Position Position { get; private set; }
        public string Message { get; private set; }

        public IEnumerable<ParseError> Errors { get; private set; }
    }
}