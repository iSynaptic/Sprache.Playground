namespace Sprache.Playground
{
    public delegate IResult<T> Parser<out T>(Context context);
}