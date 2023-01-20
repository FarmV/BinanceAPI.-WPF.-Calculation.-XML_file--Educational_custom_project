using System.Globalization;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AppPars
{
    internal class ReqestLogic
    {
        private readonly Uri _webReqest;
        public ReqestLogic(string webReqest = @"https://www.binance.com/fapi/v1/ticker/price") => _webReqest = new Uri(webReqest);
        

        public async Task<CurrencyObject[]> GetObjectData()
        {
            using HttpClient httpClient = new HttpClient();
            List<CurrencyObject> data = new List<CurrencyObject>();

            JsonSerializerOptions optionJson = new JsonSerializerOptions();
            optionJson.Converters.Add(new CurrencyObjectJsonConverter());

            return await httpClient.GetFromJsonAsync<CurrencyObject[]>(requestUri: _webReqest, options: optionJson) ?? throw new InvalidOperationException();
        }
        public class CurrencyObjectJsonConverter : JsonConverter<CurrencyObject>
        {
            public override CurrencyObject? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.Null) return null;

                Type currencyObjectType = typeof(CurrencyObject);

                var res22 = currencyObjectType.GetConstructors();
                ConstructorInfo? constructorInfo = null;

                foreach (ConstructorInfo item in res22)
                {
                    if (item.GetCustomAttribute(typeof(JsonConstructorAttribute)) is not JsonConstructorAttribute) continue;
                    else
                    {
                        constructorInfo = item;
                        break;
                    }
                }
                if (constructorInfo is null) throw new InvalidOperationException("The constructor marked with JsonConstructor is not found");

                List<ParameterInfo> parametersConstructor = constructorInfo.GetParameters().ToList();

                JsonElement jsonElement = JsonDocument.ParseValue(ref reader).RootElement;
                if (jsonElement.ValueKind != JsonValueKind.Object) throw new InvalidOperationException();
               
                string? itemSymbol = null;
                decimal? itemPrice = null;
                long? itemTime = null;

                foreach (JsonProperty jsonElementItem in jsonElement.EnumerateObject())
                {
                    foreach (ParameterInfo construcParam in parametersConstructor)
                    {
                        if (construcParam.Name == jsonElementItem.Name)
                        {
                            if (jsonElementItem.Name == "symbol")
                            {
                                itemSymbol = jsonElementItem.Value.Deserialize<string>();
                            }
                            else if (jsonElementItem.Name == "price")
                            {
                                itemPrice = decimal.Parse(jsonElementItem.Value.Deserialize<string>() ?? throw new NullReferenceException(construcParam.Name), new CultureInfo("en-US"));
                            }
                            else if (jsonElementItem.Name == "time")
                            {
                                itemTime = jsonElementItem.Value.Deserialize<long>();
                            }
                            else { throw new InvalidOperationException("Unknown constructor signature"); }
                            parametersConstructor.Remove(construcParam);
                            break;
                        }
                        else { throw new InvalidOperationException("Unknown constructor signature"); }
                    }
                }
                return new CurrencyObject(itemSymbol ?? throw new NullReferenceException(), itemPrice ?? throw new NullReferenceException(), itemTime ?? throw new NullReferenceException());
            }

            public override void Write(Utf8JsonWriter writer, CurrencyObject value, JsonSerializerOptions options) => throw new NotImplementedException();

        }

    }
}