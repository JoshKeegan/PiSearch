namespace StringSearch.Api.Health
{
    public class HealthState
    {
        public readonly bool Healthy;
        public readonly string Message;

        internal HealthState(bool healthy, string message)
        {
            Healthy = healthy;
            Message = message;
        }
    }
}