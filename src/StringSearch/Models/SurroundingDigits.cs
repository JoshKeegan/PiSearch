namespace StringSearch.Models
{
    public class SurroundingDigits
    {
        public readonly string Before;
        public readonly string After;

        public SurroundingDigits(string before, string after)
        {
            Before = before;
            After = after;
        }
    }
}
