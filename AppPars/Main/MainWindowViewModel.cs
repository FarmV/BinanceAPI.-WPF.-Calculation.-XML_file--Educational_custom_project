using ReactiveUI;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppPars
{
    public class MainWindowViewModel : ReactiveUI.ReactiveObject
    {
        private DataManagement _dataManagement;
        private IEnumerable<DataObject> _dataObjects;
        
        public MainWindowViewModel(DataManagement dataManagement)
        {
            _dataManagement = dataManagement;
            _dataObjects = _dataManagement.CalculateMultiplier();
        }
        private async void CreateDisainer()
        {

            DataManagement dataManagement = await DataManagement.Create();
            _dataManagement = dataManagement;
            DataObjects = _dataManagement.CalculateMultiplier();
           // System.Windows.MessageBox.Show("test");
        }
        public MainWindowViewModel()
        {
            if (Program.StartAppDesigner is not true) throw new InvalidOperationException("Пустой конструтор только для дизайнера");
            CreateDisainer();
            //_dataObjects = new DataObject[] { new DataObject(new CurrencyObject("FFFF",24,1673662956),Enumerable.Range(0,1).Select(x=> new DataManagement.DataVolume(55,87, 90)),null)};
        }
        public IEnumerable<DataObject> DataObjects
        {
            get => _dataObjects;
            set => this.RaiseAndSetIfChanged(ref _dataObjects, value);
        }
        //public IEnumerable<DataObject> DataObjects
        //{
        //    get => _dataObjects;
        //    set => this.RaiseAndSetIfChanged(ref _dataObjects, value);
        //}

    }
}
