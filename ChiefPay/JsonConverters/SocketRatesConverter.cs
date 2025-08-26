using Newtonsoft.Json;
using ChiefPay.Models;

public class SocketRatesConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(SocketRates);
    }

    public override bool CanWrite => false;

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var rates = serializer.Deserialize<List<RateData>>(reader) ?? new List<RateData>();

        return new SocketRates { Rates = rates };
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException("Not using this method.");
    }
}