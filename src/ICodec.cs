namespace Cfth;

public interface ICodec
{
    string Encode(ulong id);

    bool TryDecode(string encodedId, out ulong id);
}
