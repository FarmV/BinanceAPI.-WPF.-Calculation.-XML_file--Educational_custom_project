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
        private readonly Uri _webReqest = new Uri(@"https://www.binance.com/fapi/v1/ticker/price");
        private readonly Uri _webReqest2 = new Uri(@"https://www.binance.com/fapi/v1/exchangeInfo");
        public ReqestLogic()
        {

        }


        public async Task<CurrencyObject[]> GetObjectData()
        {
            (string Symbol, decimal StepSize)[]? StepSize = null;
            CurrencyObjectBase[]? currencyObject = null; 
         
            Task task1 = Task.Run(async () =>
            {
                JsonSerializerOptions optionJson = new JsonSerializerOptions();
                optionJson.Converters.Add(new StepSizeJsonConverter());
                using HttpClient httpClient = new HttpClient();
                StepSize = await httpClient.GetFromJsonAsync<(string Symbol, decimal StepSize)[]>(requestUri: _webReqest2, options: optionJson) ?? throw new InvalidOperationException();
            });
            Task task2 = Task.Run(async () =>
            {
                JsonSerializerOptions optionJson = new JsonSerializerOptions();
                optionJson.Converters.Add(new CurrencyObjectJsonConverter());
                using HttpClient httpClient = new HttpClient();
                currencyObject = await httpClient.GetFromJsonAsync<CurrencyObjectBase[]>(requestUri: _webReqest, options: optionJson) ?? throw new InvalidOperationException();
            });

            await Task.WhenAll(task1,task2);


            IEnumerable<CurrencyObject> joinResult = DataUnion(currencyObject ?? throw new InvalidOperationException(), StepSize ?? throw new InvalidOperationException());

            return joinResult?.ToArray() ?? throw new NullReferenceException();
        }


        private static IEnumerable<CurrencyObject> DataUnion(IEnumerable<CurrencyObjectBase> list1, IEnumerable<(string Symbol, decimal Step)> list2)
        {         
            var joinAnonim = list1.Join(list2, x => x.Symbol, y => y.Symbol,(cur, y) => new { cur, y.Step });
            return joinAnonim.Select(x => new CurrencyObject(x.Step,x.cur.Symbol,x.cur.Price,x.cur.Time));
        }

        public class CurrencyObjectJsonConverter : JsonConverter<CurrencyObjectBase>
        {
            public override CurrencyObjectBase? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.Null) return null;

                Type currencyObjectType = typeof(CurrencyObjectBase);

                var constuctors = currencyObjectType.GetConstructors();
                ConstructorInfo? constructorInfo = null;

                foreach (ConstructorInfo item in constuctors)
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
                return new CurrencyObjectBase(itemSymbol ?? throw new NullReferenceException(), itemPrice ?? throw new NullReferenceException(), itemTime ?? throw new NullReferenceException());
            }

            public override void Write(Utf8JsonWriter writer, CurrencyObjectBase value, JsonSerializerOptions options) => throw new NotImplementedException();

        }

        public class StepSizeJsonConverter : JsonConverter<(string Symbol, decimal StepSize)[]>
        {
            public override (string Symbol, decimal StepSize)[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                using  JsonDocument jsonDoc = JsonDocument.ParseValue(ref reader);
                (string symbol, decimal stepSize)[] tuple = jsonDoc.RootElement.GetProperty("symbols").EnumerateArray().Select(x =>
                {
                    string symbol = x.GetProperty("symbol").GetString() ?? throw new NullReferenceException();
                    string stepSizeStr = x.GetProperty("filters").EnumerateArray().Single(s => s.GetProperty("filterType").GetString() == "LOT_SIZE").GetProperty("stepSize").GetString() ?? throw new NullReferenceException();

                    decimal stepSize = Convert.ToDecimal(stepSizeStr, CultureInfo.InvariantCulture);

                    return (symbol, stepSize);

                }).ToArray();

                return tuple;
            }

            public override void Write(Utf8JsonWriter writer, (string Symbol, decimal StepSize)[] value, JsonSerializerOptions options)
            {
                throw new NotImplementedException();
            }
        }


    }
}