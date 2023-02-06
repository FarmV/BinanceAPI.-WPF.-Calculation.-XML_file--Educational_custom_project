using ReactiveUI;

namespace AppPars
{
    public partial class DataManagement 
    {
        public class DataVolumeTmp: ReactiveUI.ReactiveObject
        {
            private decimal _volume;
            private decimal _quantity;
            private decimal _result;
            public DataVolumeTmp(decimal volume, decimal quantity, decimal result)
            {
                Volume = volume;
                QuantityPrice = quantity;
                Result = result;
            }

            public decimal Volume
            {
                get => _volume;
                set => this.RaiseAndSetIfChanged(ref _volume, value);
            }
            public decimal QuantityPrice
            { 
                get => _quantity;
                set => this.RaiseAndSetIfChanged(ref _quantity, value);
            }
            public decimal Result 
            { 
                get=> _result;
                set => this.RaiseAndSetIfChanged(ref _result, value);
            }

        }
      
    }
}