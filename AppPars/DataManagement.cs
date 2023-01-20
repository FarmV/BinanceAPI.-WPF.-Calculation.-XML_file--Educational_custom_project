using System.Collections.Generic;

namespace AppPars
{
    public partial class DataManagement
    {
        private IEnumerable<decimal> _multiplier;
        private ReqestLogic _webLogic;
        private IEnumerable<CurrencyObject> _currencies;

        private DataManagement(IEnumerable<CurrencyObject> currencies, IEnumerable<decimal> multiplier, ReqestLogic webLogic)
        {
            _currencies = currencies;
            _multiplier = multiplier;
            _webLogic = webLogic;
        }
        public static async Task<DataManagement> Create(string webReqest = @"https://www.binance.com/fapi/v1/ticker/price", IEnumerable<decimal>? multiplier = null)
        {
            ReqestLogic webLogic = new ReqestLogic(webReqest);
            CurrencyObject[] currencies = await webLogic.GetObjectData();
            decimal[] defoltMultiplier = new decimal[] { 100, 250, 500, 1000, 3000 };
            IEnumerable<decimal> multiplierSet = multiplier ?? defoltMultiplier;
            return new DataManagement(currencies, multiplierSet, webLogic);
        }
        public static decimal CalculateVolume(decimal volume, decimal price) => volume / price;
        public IEnumerable<DataObject> CalculateMultiplier()
        {
            Dictionary<CurrencyObject, List<DataVolume>> tempDictionary = new();

            List<DataObject> retuenList = new List<DataObject>();
            foreach (CurrencyObject objectOpratin in _currencies)
            {
                tempDictionary.Add(objectOpratin, new List<DataVolume>());
                foreach (decimal item in _multiplier)
                {
                    tempDictionary[objectOpratin].Add(new DataVolume(item, objectOpratin.Price, CalculateVolume(item, objectOpratin.Price)));
                }
            }
            foreach (KeyValuePair<CurrencyObject, List<DataVolume>> item in tempDictionary)
            {
                retuenList.Add(new DataObject(item.Key, item.Value, new SaveOneXmlFile($"{item.Key.Symbol}.xml")));
            }
            return retuenList;
        }




        public static void Print(IEnumerable<DataObject> dataList)
        {
            Console.WriteLine("==============================================================================================");
            foreach (DataObject item in dataList)
            {
                Console.WriteLine($"Наименование {item.CurrencyObject.Symbol} Прайс - {item.DataVolume.First().Quantity}");
                item.DataVolume.ToList().ForEach(x => Console.WriteLine($"объём - {x.Volume}, расчётная цена - {Math.Round(x.Result, 2)}"));
                Console.WriteLine("==============================================================================================");
            }
        }
        public void SaveData(DataObject dataObject) => dataObject.OneXmlFile.DataWrite();
        public void SaveData(IEnumerable<DataObject> dataObject) => dataObject.AsParallel().ForAll(SaveData);





    }
}