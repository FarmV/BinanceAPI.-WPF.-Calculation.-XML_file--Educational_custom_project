

using Microsoft.VisualBasic.ApplicationServices;

using ReactiveUI;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace AppPars
{

    public class SetTemplateCommandParams
    {
        public double? a1 { get; set; }
        public double? a2 { get; set; }
        public double? a3 { get; set; }
        public double? a4 { get; set; }
        public double? a5 { get; set; }
    }
    public class MainWindowViewModel : ReactiveUI.ReactiveObject
    {
        private DataManagement _dataManagement;
        private IEnumerable<DataObject> _dataObjects;
        public bool _saveProccesinng = false;
        private double[] _valueTemaplete = new double[] { 0, 0, 0, 0, 0 };  
        public ReactiveCommand<Unit, Unit> OverwriteAllFilesCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> SetNewDirectoryCommand { get; private set; }
        public ReactiveCommand<Unit, Unit> OpenDirectoryCommand { get; private set; }
        public ReactiveCommand<(double?, double?, double?, double?, double?), Unit> SetNewTemaplateCommand { get; private set; }
   


        public DataManagement DataManagement
        {
            get => _dataManagement;
            private set
            {
                this.RaiseAndSetIfChanged(ref _dataManagement, value);
            }
        }

        private void SetNewTemaplate(decimal first_WorkAmount, decimal second_WorkAmount, decimal third_WorkAmount, decimal fourth_WorkAmount, decimal fifth_WorkAmount)
        {
            foreach (DataObject data in _dataObjects)
            {
                data.First_WorkAmount = _dataManagement.CalculateAdjustTheStep(_dataManagement.CalculateVolume(first_WorkAmount,data.CurrencyObject.Price), data.CurrencyObject.StepSize);
                data.Second_WorkAmount = _dataManagement.CalculateAdjustTheStep(_dataManagement.CalculateVolume(second_WorkAmount, data.CurrencyObject.Price), data.CurrencyObject.StepSize);
                data.Third_WorkAmount = _dataManagement.CalculateAdjustTheStep(_dataManagement.CalculateVolume(third_WorkAmount, data.CurrencyObject.Price), data.CurrencyObject.StepSize);
                data.Fourth_WorkAmount = _dataManagement.CalculateAdjustTheStep(_dataManagement.CalculateVolume(fourth_WorkAmount, data.CurrencyObject.Price), data.CurrencyObject.StepSize); ;
                data.Fifth_WorkAmount = _dataManagement.CalculateAdjustTheStep(_dataManagement.CalculateVolume(fifth_WorkAmount, data.CurrencyObject.Price), data.CurrencyObject.StepSize); ;
            }
            ValueTemaplete = new double[] { Convert.ToDouble(first_WorkAmount), Convert.ToDouble(second_WorkAmount), Convert.ToDouble(third_WorkAmount), Convert.ToDouble(fourth_WorkAmount), Convert.ToDouble(fifth_WorkAmount) };
        }

        private DataObject _firstData;
        private DataObject FirstData
        {
            get => _firstData;
            set => this.RaiseAndSetIfChanged(ref _firstData,value);
        }

        private bool _init = false;
        public bool Init
        {
            get => _init;
            set => this.RaiseAndSetIfChanged(ref _init, value);
        }
        public Task<DataManagement> CreateData { get; set; }

        public MainWindowViewModel(Task<DataManagement> dataManagement)
        {
            CreateData = dataManagement;            
        }

        public async Task GetDataHTTP()
        {
            DataManagement data = await CreateData;
            DataObjects = data.CalculateMultiplier();
            _firstData = DataObjects.First();
            this.DataManagement = data;


            this.WhenAnyValue(x => x.FirstData).
                Subscribe(x => ValueTemaplete = new double[] {
                    Convert.ToDouble(x.Fifth_WorkAmount),
                    Convert.ToDouble(x.Second_WorkAmount),
                    Convert.ToDouble(x.Third_WorkAmount),
                    Convert.ToDouble(x.Fourth_WorkAmount),
                    Convert.ToDouble(x.Fourth_WorkAmount) });

            OverwriteAllFilesCommand = ReactiveCommand.CreateFromTask(cen => DataManagement.OverwriteAllFiles(DataObjects, cen));
            SetNewDirectoryCommand = ReactiveCommand.Create(DataManagement.SetNewDirectory);
            OpenDirectoryCommand = ReactiveCommand.CreateFromTask(() =>
            {
                if(Directory.Exists(_dataManagement.SavePath) is not true)
                {
                    System.Windows.MessageBox.Show($"Вероятно отсутствует директория{Environment.NewLine}{_dataManagement.SavePath}{Environment.NewLine}Проверте наличие директории", "Ошибка",
                       System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                    return Task.CompletedTask;
                }
                string argument = "/n, \"" + $"{_dataManagement.SavePath}" + "\"";
                System.Diagnostics.Process.Start("explorer.exe", argument);
                return Task.CompletedTask;
            });

            SetNewTemaplateCommand = ReactiveCommand.Create(((double?, double?, double?, double?, double?) o) => SetNewTemaplate(Convert.ToDecimal(o.Item1 ?? 0), Convert.ToDecimal(o.Item2 ?? 0), Convert.ToDecimal(o.Item3 ?? 0), Convert.ToDecimal(o.Item4 ?? 0), Convert.ToDecimal(o.Item5 ?? 0)));

            Init = true;            
        }
        private async void CreateDisainer()
        {
            DataManagement dataManagement = await DataManagement.Create();
            _dataManagement = dataManagement;
            DataObjects = _dataManagement.CalculateMultiplier();

            
        }
        public MainWindowViewModel()
        {
            if (Program.StartAppDesigner is not true) throw new InvalidOperationException("Пустой конструтор только для дизайнера");


            CreateDisainer();
        }



        public double[] ValueTemaplete
        {
            get => _valueTemaplete;
            set
            {
                this.RaiseAndSetIfChanged(ref _valueTemaplete, value);
            }
        }


        public async Task OverwriteAllFiles()
        {
            if (SaveProccesinng is true) return;
            else
            {
                SaveProccesinng = true;
                await _dataManagement.OverwriteAllFiles(DataObjects);
                SaveProccesinng = false;
                return;
            }
        }

        public bool SaveProccesinng
        {
            get => _saveProccesinng;
            private set => this.RaiseAndSetIfChanged(ref _saveProccesinng, value);
        }

        public IEnumerable<DataObject> DataObjects
        {
            get => _dataObjects;
            set => this.RaiseAndSetIfChanged(ref _dataObjects, value);
        }

    }
}
