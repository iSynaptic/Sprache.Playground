namespace Sprache.Playground
{
    public class ParseError
    {
        public ParseError(int index, string message)
        {
            Index = index;
            Message = message;
        }

        public int Index { get; private set; }
        public string Message { get; private set; }
    }
}