namespace StringSearch.Api.Contracts.Searches
{
    public class SurroundingDigits
    {
        public string Before { get; }
        public string After { get; }

        public SurroundingDigits(string before, string after)
        {
            Before = before;
            After = after;
        }
    }
}
