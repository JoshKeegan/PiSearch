using StringSearch.Legacy.Collections;

namespace StringSearch.NamedDigits.Factories
{
    public interface IDigitsFactory
    {
        ObjectWithStream<IBigArray<byte>> Create(string namedDigits);
    }
}
