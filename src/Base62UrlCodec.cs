using Base62;

namespace Cfth;

public class Base62Codec : ICodec
{
    private readonly Base62Converter codec;

    public Base62Codec()
    {
        this.codec = new Base62Converter(Base62Converter.CharacterSet.DEFAULT);
    }

    public string Encode(ulong id)
    {
        return this.codec.Encode(id.ToString());
    }

    public bool TryDecode(string encodedId, out ulong id)
    {
        return ulong.TryParse(this.codec.Decode(encodedId), out id);
    }
}
