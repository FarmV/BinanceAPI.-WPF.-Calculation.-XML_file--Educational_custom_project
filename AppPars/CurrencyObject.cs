

using ReactiveUI;

using System.Runtime.Serialization;
using System.Text.Json.Serialization;

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AppPars
{
    [DataContract]
    public class CurrencyObjectBase : ReactiveObject
    {
        private string _symbol;
        private decimal _price;
        [JsonIgnore]
        [IgnoreDataMember]
        private long _time;
        [JsonIgnore]
        [IgnoreDataMember]
        private DateTime? _dateTime;

        [JsonConstructor]
        public CurrencyObjectBase(string symbol, decimal price, long? time = null)
        {
            _symbol = symbol;
            _price = price;
            Time = time ?? default;
        }
        public string Symbol
        {
            get => _symbol;
            set 
            {
                this.RaiseAndSetIfChanged(ref _symbol, value);
            }
        }
        public decimal Price
        {
            get => _price;
            set 
            {
                this.RaiseAndSetIfChanged(ref _price, value);           
            }
        }
        [JsonIgnore]
        [IgnoreDataMember]
        public long Time // unix timestamp milisecinds
        {
            get => _time;
            set
            {
                if (value == _time) return;
                _time = value;
                if (value is not 0)
                {
                    _dateTime = ConvertUnixTimestamp();
                     this.RaisePropertyChanged(nameof(LocalTime));
                }
                this.RaisePropertyChanged();
            }
        }
        [JsonIgnore]
        [IgnoreDataMember]
        public DateTime? LocalTime
        {
            get => _dateTime;
        }
        public DateTime? ConvertUnixTimestamp(bool isLocalTime = true)
        {
            if(_time is 0) return null;
            if (isLocalTime is true) return DateTimeOffset.FromUnixTimeMilliseconds(_time).ToLocalTime().DateTime;
            else { return DateTimeOffset.FromUnixTimeMilliseconds(_time).DateTime; }
        }
    }

    public class CurrencyObject : CurrencyObjectBase
    {
        private decimal _stepSize;
        [JsonConstructor]
        public CurrencyObject(decimal stepSize, string symbol, decimal price, long time) : base(symbol, price, time)
        {
            _stepSize = stepSize;
        }
        public decimal StepSize
        {
            get => _stepSize;
            set 
            {
                _stepSize = value;
            }
        }
      


    }

}