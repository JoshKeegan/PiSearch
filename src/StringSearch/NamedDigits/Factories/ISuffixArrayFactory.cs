using StringSearch.Legacy.Collections;

namespace StringSearch.NamedDigits.Factories
{
    public interface ISuffixArrayFactory
    {
        ObjectWithStream<IBigArray<ulong>> Create(string namedDigits);
    }
}
