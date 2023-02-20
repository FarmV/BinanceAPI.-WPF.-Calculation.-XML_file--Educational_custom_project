using ReactiveUI;

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq.Expressions;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Linq;

namespace AppPars
{
    public partial class DataManagement : ReactiveObject
    {
        private readonly string _fileMask = "BINAD.CCUR_FUT.{price.symbol}_Settings_55555.xml";
        private readonly SaveXML _saveXML;
        private readonly IEnumerable<decimal> _multiplier;
        private readonly IEnumerable<CurrencyObject> _currencies;
        private static decimal[] _defoltMultiplier = new decimal[] { 100, 250, 500, 1000, 3000 };
        private string _savePath;        
        public string SavePath
        {
            get => _savePath;
            set
            {
                this.RaiseAndSetIfChanged(ref _savePath, value);
            }
        }
        public void SetNewDirectory()
        {
            if (SaveFileDirectory("Выбор директории") is not string newPath) return;
            if (Path.GetDirectoryName(newPath) is null)
            {
                if (Path.GetPathRoot(newPath) == newPath) SavePath = newPath;
                else return;
            }
            else SavePath = newPath; 
        }
        public async Task OverwriteAllFiles(IEnumerable<DataObject> data, CancellationToken cancellationToken = default)
        {                   
            IEnumerable<Task> tasks = data.Select(data => _saveXML.SaveDataFile(Path.Combine(SavePath, _fileMask.Replace("{price.symbol}", data.CurrencyObject.Symbol)), data, cancellationToken));
            await Task.WhenAll(tasks);
        }
        private string? SaveFileDirectory(string Title)
        {
            System.Windows.Forms.FolderBrowserDialog DirectoryDialog = new System.Windows.Forms.FolderBrowserDialog()
            {
                AddToRecent = true,
                Description = Title,
                InitialDirectory = _savePath,
                ShowNewFolderButton = true,
                ShowPinnedPlaces =true,
                UseDescriptionForTitle = true,
            };
            if (DirectoryDialog.ShowDialog() is not DialogResult.OK) return null;
            return DirectoryDialog.SelectedPath;
        }
        private DataManagement(IEnumerable<CurrencyObject> currencies, IEnumerable<decimal> multiplier)
        {
            _savePath = $"{Directory.GetParent(Environment.ProcessPath ?? throw new NullReferenceException())}\\output\\";
            _currencies = currencies;
            _multiplier = multiplier;
            _saveXML = new SaveXML();
            if(Directory.Exists(_savePath) is false) Directory.CreateDirectory(_savePath);
        }
        public static async Task<DataManagement> Create(IEnumerable<decimal>? multiplier = null)
        {
            ReqestLogic webLogic = new ReqestLogic();
            CurrencyObject[] currencies = await webLogic.GetObjectData();
            IEnumerable<decimal> multiplierSet = multiplier ?? _defoltMultiplier;
            return new DataManagement(currencies, multiplierSet);
        }
        internal decimal CalculateVolume(decimal volume,decimal price) => volume / price;
        internal decimal CalculateAdjustTheStep(decimal volume, decimal stepSize) => Math.Floor(volume / stepSize) * stepSize;       
        public IEnumerable<DataObject> CalculateMultiplier()
        {
            Dictionary<CurrencyObject, List<DataVolumeTmp>> tempDictionary = new();

            List<DataObject> retuenList = new List<DataObject>();
            foreach (CurrencyObject objectOpration in _currencies)
            {
                tempDictionary.Add(objectOpration, new List<DataVolumeTmp>());
                foreach (decimal item in _multiplier)
                {
                    tempDictionary[objectOpration].Add(new DataVolumeTmp(item, objectOpration.Price, CalculateAdjustTheStep(CalculateVolume(item, objectOpration.Price), objectOpration.StepSize)));
                }
            }
            foreach (KeyValuePair<CurrencyObject, List<DataVolumeTmp>> item in tempDictionary)
            {
                retuenList.Add(new DataObject(item.Key, item.Value));
            }
            return retuenList;
        }





    }



    internal class SaveXML
    {
        public static string _defaultXML = """
            <?xml version="1.0" encoding="utf-8"?>
            <Settings>
              <DOM>
                <FilledAt Value="1500000" />
                <AutoScroll Value="True" />
                <ScrollStepTurbo Value="100" />
                <StringHeight Value="10" />
                <BigAmount Value="1500000" />
                <HugeAmount Value="3000000" />
                <PlayBigAmount Value="False" />
                <PlayHugeAmount Value="False" />
                <RulerDataType Value="2" />
                <FindIcebergOrders Value="False" />
                <PlayIcebergOrders Value="False" />
                <IcebergSuspectedFrom Value="999999" />
                <IcebergFactor Value="2" />
                <Focus_TSpreadFTick Value="True" />
                <FatLevelsFactor Value="50" />
                <SlimLevelsFactor Value="10" />
                <PanelWidth Value="126" />
                <UserLevels Value="" />
                <UserSignalPriceLevels Value="" />
                <PlayUserSignalPriceLevels Value="True" />
              </DOM>
              <TICK_PANEL>
                <ShowTicksFrom Value="8500" />
                <HideFilteredTicks Value="True" />
                <TicksStyle Value="DotsLines" />
                <TicksWeight Value="1" />
                <SumTicks_Period Value="250" />
                <PanelWidth Value="81" />
                <BigAmount Value="999999" />
                <PlayBigAmount Value="False" />
              </TICK_PANEL>
              <CLUSTER_PANEL>
                <FilledAt Value="4000000" />
                <ShowClusterPanel Value="True" />
                <TimeFrame Value="5" />
                <GridWidth Value="288" />
                <ClusterStyleText Value="Summ" />
                <ClusterStyleColor Value="BlackBalance" />
                <PanelWidth Value="30" />
              </CLUSTER_PANEL>
              <COMMON_PANEL>
                <PriceAggregationStep Value="2" />
                <PriceAggregationFactor Value="1" />
                <MaxAmountZeroDigits Value="0" />
                <ReduceAmountThousands Value="True" />
                <ShowAdditiveCursor Value="True" />
              </COMMON_PANEL>
              <TRADING>
                <First_WorkAmount Value="2 978" /> 
                <Second_WorkAmount Value="1 489" />
                <Third_WorkAmount Value="744" />
                <Fourth_WorkAmount Value="297" />
                <Fifth_WorkAmount Value="8 936" />
                <ActiveWorkAmountKey Value="5" />
                <IsWorkAmountInMoneyMode Value="True" />
                <ThrowLimitTo Value="100" />
                <StopLoss_Steps Value="0" />
                <TakeProfit_Steps Value="0" />
                <StopLimitOrdersThrowRange Value="0" />
                <StopOrdersThrowRange Value="0" />
                <IsServerStopOrders Value="True" />
                <StopOrdersMethod_TPricesFTick Value="False" />
                <AveragingMethod Value="Dispatcher" />
                <PlaySoundOnTrade Value="True" />
                <ShowProfitType Value="2" />
              </TRADING>
            </Settings>
            """;

        public async Task SaveDataFile(string path, DataObject data, CancellationToken cancellationToken = default)
        {
            if (File.Exists(path) is not true)
            {
                if (Directory.Exists(Path.GetDirectoryName(path)) is not true)
                {
                    System.Windows.MessageBox.Show($"Вероятно отсутствует директория{Environment.NewLine}{Path.GetDirectoryName(path)}{Environment.NewLine}Проверте наличие директории{Environment.NewLine}Программа будет закрыта","Ошибка",
                        System.Windows.MessageBoxButton.OK,System.Windows.MessageBoxImage.Error);
                    Environment.Exit(-1);                    
                }
                
                if (cancellationToken.IsCancellationRequested) cancellationToken.ThrowIfCancellationRequested();
                await OverwriteTextFile(path, ConvertDataToXML(_defaultXML, data), cancellationToken);
                return;
            }
            if (File.Exists(path) is true)
            {
                if (cancellationToken.IsCancellationRequested) cancellationToken.ThrowIfCancellationRequested();
                await OverwriteTextFile(path, ConvertDataToXML(await File.ReadAllTextAsync(path, Encoding.UTF8, cancellationToken), data), cancellationToken);
                return;
            }
        }

        private class Utf8StringWriter : StringWriter { public override Encoding Encoding { get => Encoding.UTF8; } }
        private string ConvertDataToXML(string soureceXML, DataObject data)
        {
            static string NameOf<T>(Expression<Func<T>> expr) => ((MemberExpression)expr.Body).Member.Name;

            string valueAttribut = "Value";
            XDocument doc = XDocument.Parse(soureceXML, LoadOptions.PreserveWhitespace);
            doc.Declaration = new XDeclaration("1.0", "utf-8", null);
            using StringWriter writer = new Utf8StringWriter();
            XElement nodeTRADING = doc.Element(XName.Get("Settings"))!.Element("TRADING") ?? throw new NullReferenceException();
            nodeTRADING!.Element(NameOf(() => data.First_WorkAmount))!.Attribute(XName.Get(valueAttribut))!.Value = data.First_WorkAmount.ToString(CultureInfo.InvariantCulture);
            nodeTRADING!.Element(NameOf(() => data.Second_WorkAmount))!.Attribute(XName.Get(valueAttribut))!.Value = data.Second_WorkAmount.ToString(CultureInfo.InvariantCulture);
            nodeTRADING!.Element(NameOf(() => data.Third_WorkAmount))!.Attribute(XName.Get(valueAttribut))!.Value = data.Third_WorkAmount.ToString(CultureInfo.InvariantCulture);
            nodeTRADING!.Element(NameOf(() => data.Fourth_WorkAmount))!.Attribute(XName.Get(valueAttribut))!.Value = data.Fourth_WorkAmount.ToString(CultureInfo.InvariantCulture);
            nodeTRADING!.Element(NameOf(() => data.Fifth_WorkAmount))!.Attribute(XName.Get(valueAttribut))!.Value = data.Fifth_WorkAmount.ToString(CultureInfo.InvariantCulture);
            doc.Save(writer, SaveOptions.None);
            return writer.ToString();
        }

        private async Task OverwriteTextFile(string path, string data, CancellationToken cancellationToken = default) => await File.WriteAllTextAsync(path, data, Encoding.UTF8, cancellationToken);
    }
}