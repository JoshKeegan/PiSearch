using System.Collections.Generic;

namespace StringSearch.Health.Factories
{
    public interface IDigitsHealthChecksFactory
    {
        IDictionary<string, IHealthResource[]> Create();
    }
}
