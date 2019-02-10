namespace StringSearch.Api.Contracts.Searches
{
    public class SurroundingDigitsDto
    {
        public string Before { get; }
        public string After { get; }

        public SurroundingDigitsDto(string before, string after)
        {
            Before = before;
            After = after;
        }
    }
}
