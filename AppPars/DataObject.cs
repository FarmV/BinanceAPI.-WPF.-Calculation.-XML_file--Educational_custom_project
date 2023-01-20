using ReactiveUI;

using static AppPars.DataManagement;

namespace AppPars
{
    public class DataObject: ReactiveObject
    {
        private readonly CurrencyObject _currencyObject;
        private readonly IEnumerable<DataVolume> _dataVolume;
        private readonly SaveOneXmlFile _saveOneXmlFile;

        public DataObject(CurrencyObject currencyObject, IEnumerable<DataVolume> dataVolume, SaveOneXmlFile saveOneXmlFile)
        {
            _currencyObject = currencyObject;
            _dataVolume = dataVolume;
            _saveOneXmlFile = saveOneXmlFile;
        }
        public SaveOneXmlFile OneXmlFile
        {
            get => _saveOneXmlFile;
        }
        public CurrencyObject CurrencyObject
        {
            get => _currencyObject;
        }
        public IEnumerable<DataVolume>DataVolume
        {
            get => _dataVolume;
        }
    }
}