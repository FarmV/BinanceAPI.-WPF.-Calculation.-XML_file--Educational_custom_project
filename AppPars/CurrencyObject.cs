

using ReactiveUI;

using System.Text.Json.Serialization;

namespace AppPars
{
    public class CurrencyObject: ReactiveObject
    {
        private string _symbol;
        private decimal _price;
        private long _time;
        private DateTime _dateTime;

        [JsonConstructor]
        public CurrencyObject(string symbol, decimal price, long time)
        {
            _symbol = symbol;
            _price = price;
            Time = time;
        }
        public string Symbol
        {
            get => _symbol;
            set => _symbol = value;
        }
        public decimal Price
        {
            get => _price;
            set => _price =  value;
        }
        public long Time // unix timestamp milisecinds
        {
            get => _time;
            set
            {
                _time = value;
                _dateTime = ConvertUnixTimestamp();
            }
        }
        public DateTime LocalTime 
        {
            get => _dateTime;
        }
        public DateTime ConvertUnixTimestamp(bool isLocalTime = true)
        {
            if (isLocalTime is true) return DateTimeOffset.FromUnixTimeMilliseconds(_time).ToLocalTime().DateTime;
            else { return DateTimeOffset.FromUnixTimeMilliseconds(_time).DateTime; }
        }
    }
}