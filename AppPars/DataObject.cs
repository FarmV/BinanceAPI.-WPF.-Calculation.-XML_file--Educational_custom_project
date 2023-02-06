using ReactiveUI;

using static AppPars.DataManagement;

namespace AppPars
{
    public class DataObject: ReactiveObject
    {
        private readonly CurrencyObject _currencyObject;
        private readonly IEnumerable<DataVolumeTmp> _dataVolume;

        public DataObject(CurrencyObject currencyObject, IEnumerable<DataVolumeTmp> dataVolume)
        {
            _currencyObject = currencyObject;
            _dataVolume = dataVolume;
        }     
        public CurrencyObject CurrencyObject
        {
            get => _currencyObject;
        }
        public IEnumerable<DataVolumeTmp>DataVolumeTemp
        {
            get => _dataVolume;
        }




        private decimal _first_WorkAmount;
        public decimal First_WorkAmount
        {
            get => _first_WorkAmount;
            set
            {
                if (value == _first_WorkAmount) return;
                if (value < 0) return;
                _first_WorkAmount = AdjustTheStep(value);
                this.RaisePropertyChanged();
            }
        }
        private decimal _second_WorkAmount;
        public decimal Second_WorkAmount
        {
            get => _second_WorkAmount;
            set
            {
                if (value == _second_WorkAmount) return;
                if (value < 0) return;
                _second_WorkAmount = AdjustTheStep(value);
                this.RaisePropertyChanged();
            }
        }
        private decimal _third_WorkAmount;
        public decimal Third_WorkAmount
        {
            get => _third_WorkAmount;
            set
            {
                if (value == _third_WorkAmount) return;
                if (value < 0) return;
                _third_WorkAmount = AdjustTheStep(value);
                this.RaisePropertyChanged();
            }
        }
        private decimal _fourth_WorkAmount;
        public decimal Fourth_WorkAmount
        {
            get => _fourth_WorkAmount;
            set
            {
                if (value == _fourth_WorkAmount) return;
                if (value < 0) return;
                _fourth_WorkAmount = AdjustTheStep(value);
                this.RaisePropertyChanged();
            }
        }
        private decimal _fifth_WorkAmount;
        public decimal Fifth_WorkAmount
        {
            get => _fifth_WorkAmount;
            set
            {
                if (value == _fifth_WorkAmount) return;
                if (value < 0) return;
                _fifth_WorkAmount = AdjustTheStep(value);
                this.RaisePropertyChanged();
            }
        }
        private decimal AdjustTheStep(decimal volume)
        {
            return Math.Floor(volume / CurrencyObject.StepSize) * CurrencyObject.StepSize;
        }
        
    }

    //public class DataVolume : ReactiveObject
    //{
    //    decimal _stepSize;
    //    public DataVolume(decimal stepSize)
    //    {
    //        _stepSize = stepSize;
    //    }
    //    private decimal _first_WorkAmount;
    //    public decimal First_WorkAmount
    //    {
    //        get => _first_WorkAmount;
    //        set
    //        {
    //            if (value == _first_WorkAmount) return;
    //            if (value < 0) return;
    //            _first_WorkAmount = AdjustTheStep(value);
    //            this.RaisePropertyChanged();
    //        }
    //    }
    //    private decimal _second_WorkAmount;
    //    public decimal Second_WorkAmount
    //    {
    //        get => _second_WorkAmount;
    //        set
    //        {
    //            if (value == _second_WorkAmount) return;
    //            if (value < 0) return;
    //            _second_WorkAmount = AdjustTheStep(value);
    //            this.RaisePropertyChanged();
    //        }
    //    }
    //    private decimal _third_WorkAmount;
    //    public decimal Third_WorkAmount
    //    {
    //        get => _third_WorkAmount;
    //        set
    //        {
    //            if (value == _third_WorkAmount) return;
    //            if (value < 0) return;
    //            _third_WorkAmount = AdjustTheStep(value);
    //            this.RaisePropertyChanged();
    //        }
    //    }
    //    private decimal _fourth_WorkAmount;
    //    public decimal Fourth_WorkAmount
    //    {
    //        get => _fourth_WorkAmount;
    //        set
    //        {
    //            if (value == _fourth_WorkAmount) return;
    //            if (value < 0) return;
    //            _fourth_WorkAmount = AdjustTheStep(value);
    //            this.RaisePropertyChanged();
    //        }
    //    }
    //    private decimal _fifth_WorkAmount;



    //    public decimal Fifth_WorkAmount
    //    {
    //        get => _fifth_WorkAmount;
    //        set
    //        {
    //            if (value == _fifth_WorkAmount) return;
    //            if (value < 0) return;
    //            _fifth_WorkAmount = AdjustTheStep(value);
    //            this.RaisePropertyChanged();
    //        }
    //    }
    //    private decimal AdjustTheStep(decimal volume)
    //    {
    //        return Math.Floor(volume / StepSize) * StepSize;
    //    }
    //}
}