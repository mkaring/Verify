using System.Numerics;

public class Yaml
{
    Dictionary<>
    public static string Serialize(object? target)
    {
        var culture = CultureInfo.InvariantCulture;
        switch (target)
        {
            case null:
                return "null";
            case string o:
                return o;
            case StringBuilder o:
                return o.ToString();
            case StringWriter o:
                return o.ToString();
            case bool i:
                return i.ToString(culture);
            case short i:
                return i.ToString(culture);
            case ushort i:
                return i.ToString(culture);
            case int i:
                return i.ToString(culture);
            case uint i:
                return i.ToString(culture);
            case long i:
                return i.ToString(culture);
            case ulong i:
                return i.ToString(culture);
            case decimal i:
                return i.ToString(culture);
            case double i:
                return i.ToString(culture);
            case DateTimeOffset i:
                return i.ToString("yyyy-MM-ddTHH:mm:ss.FFFFFFFz", culture);
            case DateTime i:
                return i.ToString("yyyy-MM-ddTHH:mm:ss.FFFFFFFz", culture);
            case float i:
                return i.ToString(culture);
            case BigInteger i:
                return i.ToString(culture);
#if NET5_0_OR_GREATER
            case Half i:
                return i.ToString(culture);
#endif
#if NET6_0_OR_GREATER
            case DateOnly i:
                return i.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            case TimeOnly i:
                return i.ToString("h:mm tt", CultureInfo.InvariantCulture);
#endif
        }
    }
}