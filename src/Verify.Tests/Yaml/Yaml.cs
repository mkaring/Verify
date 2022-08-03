public class Yaml
{
    Dictionary<>
    public static string Serialize(object? target)
    {
        if (target is null)
        {
            return "null";
        }

        if (target is int i)
        {
            return i.ToString();
        }
    }
}