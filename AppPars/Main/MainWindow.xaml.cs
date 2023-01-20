

using ControlzEx.Standard;

using MahApps.Metro.Controls;

using Microsoft.VisualBasic;

using ReactiveUI;

using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.TextFormatting;
using System.Windows.Resources;
using System.Windows.Shapes;



namespace AppPars
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow, IViewFor<MainWindowViewModel>
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel), typeof(MainWindowViewModel), typeof(MainWindow));

        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = value is MainWindowViewModel mvm ? mvm : throw new NotImplementedException();
        }
        public MainWindowViewModel? ViewModel
        {
            get => (MainWindowViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        [DllImport("dwmapi.dll", PreserveSig = false)]
        private static extern void DwmEnableBlurBehindWindow(IntPtr hwnd, ref DwmBlurbehind blurBehind);

        public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
            this.Data.DataContext = viewModel;

            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;
            this.MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;


         

        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            var bb = new DwmBlurbehind
            {
                dwFlags = CoreNativeMethods.DwmBlurBehindDwFlags.DwmBbEnable,
                Enabled = true,
                TransitionOnMaximized = false
            };

            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Background = System.Windows.Media.Brushes.Transparent;


            Focus();
            nint hwnd = new WindowInteropHelper(this).Handle;
            HwndSource.FromHwnd(hwnd).CompositionTarget.BackgroundColor = new System.Windows.Media.Color() { A = 100, R = 4, G = 4, B = 1 };
            DwmEnableBlurBehindWindow(hwnd, ref bb);


            HeaderColumn.PreviewMouseLeftButtonDown += (_, e) =>
            {
                if (e.ButtonState is not MouseButtonState.Pressed) return;
                else { this.DragMove(); e.Handled = true; }
            };
            BottomColumn.PreviewMouseLeftButtonDown += (_, e) =>
            {
                if (e.ButtonState is not MouseButtonState.Pressed) return;
                else { this.DragMove(); e.Handled = true; }
            };
            HamburgerMenu.MouseDown += (_, e) =>
            {
                if (e.ButtonState is not MouseButtonState.Pressed) return;
                if (HamburgerMenu.IsPaneOpen is true) return;
                else { this.DragMove(); }
            };

           

            

        }


        [StructLayout(LayoutKind.Sequential)]
        public struct DwmBlurbehind
        {
            public CoreNativeMethods.DwmBlurBehindDwFlags dwFlags;
            public bool Enabled;
            public IntPtr BlurRegion;
            public bool TransitionOnMaximized;
        }

        public static class CoreNativeMethods
        {
            public enum DwmBlurBehindDwFlags
            {
                DwmBbEnable = 1,
                DwmBbBlurRegion = 2,
                DwmBbTransitionOnMaximized = 4
            }
        }
        private void OtherPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.OriginalSource is FrameworkElement { BindingGroup.Name: "ComboBoxGroup" })
            {
                var fe = (FrameworkElement)e.OriginalSource;
                if (fe.BindingGroup.Owner is System.Windows.Controls.ComboBox { IsDropDownOpen: false })
                {
                    ScrollViewerData.ScrollToVerticalOffset(ScrollViewerData.VerticalOffset - e.Delta);
                    e.Handled = true;
                    return;
                }
                e.Handled = true;
                return;
            }
            ScrollViewerData.ScrollToVerticalOffset(ScrollViewerData.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void HamburgerMenu_HamburgerButtonClick(object sender, RoutedEventArgs e)
        {

            //if (HeaderColumn.Margin != new Thickness(0))
            //{
            //    HeaderColumn.Margin = default;
            //    return;
            //}
            //else
            //{
            //    HeaderColumn.Margin = new Thickness(-150, 0, 0, 0);
            //    return;
            //}
        }

        private void NumericUpDownO1_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            var res = ((MahApps.Metro.Controls.NumericUpDown)e.OriginalSource).ToolTip;
            if (res is double result)
            {
                CultureInfo culture = new CultureInfo("ru-RU");               
                culture.NumberFormat.NumberGroupSeparator = " ";

                var test = result.ToString("N2", culture);

                ((MahApps.Metro.Controls.NumericUpDown)e.OriginalSource).ToolTip = result.ToString(test) ;
            }



        }
        private DataGridCell _currentCell;
        private void DataGridCell_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_currentCell != null)
            {
                _currentCell.BorderBrush = null;
                _currentCell.BorderThickness = new Thickness(0);
            }
            var cell = (DataGridCell)sender;
            cell.BorderBrush = new SolidColorBrush(Colors.Red);
            cell.BorderThickness = new Thickness(1);
            _currentCell = cell;
        }

       
    }
    public class ConverterStringValuse : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
            if (value is null || Information.IsNumeric(value) is false ) return null;

                         
                CultureInfo newCulture = new CultureInfo("ru-RU");
                newCulture.NumberFormat.NumberGroupSeparator = " ";
                return ((double)value).ToString("N2",  newCulture);
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

}


