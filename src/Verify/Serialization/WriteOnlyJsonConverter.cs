namespace VerifyTests;

public abstract class WriteOnlyJsonConverter :
    JsonConverter
{
    public sealed override void WriteJson(
        JsonWriter writer,
        object value,
        JsonSerializer serializer)
    {
        Write((VerifyJsonWriter)writer, value);
    }

    public abstract void Write(VerifyJsonWriter writer, object value);
}