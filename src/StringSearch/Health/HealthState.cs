namespace StringSearch.Health
{
    public class HealthState
    {
        public readonly bool Healthy;
        public readonly string Message;

        public HealthState(bool healthy, string message)
        {
            Healthy = healthy;
            Message = message;
        }
    }
}