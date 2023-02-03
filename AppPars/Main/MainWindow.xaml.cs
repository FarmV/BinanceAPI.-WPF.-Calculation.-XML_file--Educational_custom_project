using MahApps.Metro.Controls;

using Microsoft.VisualBasic;

using ReactiveUI;
using DynamicData;
using System.Reactive.Disposables;
using System.Reactive.Linq;

using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Documents;
using System;

namespace AppPars
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow, IViewFor<MainWindowViewModel>
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel), typeof(MainWindowViewModel), typeof(MainWindow));
        public GridLength GridLengthWorkPanelHide;
        public GridLength GridLengthWorkPanelOpen;


        public static readonly DependencyProperty MyPropCol1OpenProperty =
            DependencyProperty.Register("MyProp1", typeof(GridLength), typeof(MainWindow), new PropertyMetadata(new GridLength(30)));


        public GridLength MyProp1
        {
            get { return (GridLength)GetValue(MyPropCol1OpenProperty); }
            set { SetValue(MyPropCol1OpenProperty, value); }
        }




        // public static readonly GridLength GridLengthWorkPanelHide22 = new GridLength(30);




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
            GridLengthWorkPanelHide = new GridLength(30);
            GridLengthWorkPanelOpen = GridLength.Auto;
            InitializeComponent();
            ViewModel = viewModel;
            this.DataContext = viewModel;


            

            this.Loaded += async (_, _) =>
            {
                await this.Dispatcher.InvokeAsync(async () =>
                {
                    
                    if (ViewModel.Init is false)
                    {
                        this.ShowMinButton = false;
                        this.IsMaxRestoreButtonEnabled = false;
                        this.ShowCloseButton = false;

                        GlobalWorkGrid.Visibility = Visibility.Hidden;
                        await ViewModel.GetDataHTTP();


                        IReactiveBinding<MainWindow, ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit>> commandOverwrite = this.BindCommand(this.ViewModel,
                        vm => vm.OverwriteAllFilesCommand,
                        v => v.ButtonOverwriteAllFiles);
                        commandOverwrite.Changed.Subscribe(x =>
                        {
                            x.IsExecuting.Subscribe(x =>
                            {
                                if (x is true) ((ContentControl)this.ButtonOverwriteAllFiles.Content).Content = new Run("Обработка");
                                else ((ContentControl)this.ButtonOverwriteAllFiles.Content).Content = new Run("Выгрузить");
                            });

                        });
                        IReactiveBinding<MainWindow, ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit>> commandNewDirectory = this.BindCommand(this.ViewModel,
                        vm => vm.SetNewDirectoryCommand,
                        v => v.ButtonNewDirectory);


                        IReactiveBinding<MainWindow, ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit>> commandOpenDirectory = this.BindCommand(this.ViewModel,
                        vm => vm.OpenDirectoryCommand,
                        v => v.ButtonOpenDirectory);


                        IReactiveBinding<MainWindow, ReactiveCommand<(double?, double?, double?, double?, double?), System.Reactive.Unit>> commandSetNewTemaplateCommandAll = this.BindCommand(this.ViewModel,
                        vm => vm.SetNewTemaplateCommand,
                        v => v.ButtonTemplateALL,
                        withParameter: this.WhenAnyValue(x => x.NumericUpDownWork1.Value, x => x.NumericUpDownWork2.Value, x => x.NumericUpDownWork3.Value, x => x.NumericUpDownWork4.Value, x => x.NumericUpDownWork5.Value));



                        ViewboxPlug.Visibility = Visibility.Collapsed;
                        GlobalWorkGrid.Visibility = Visibility.Visible;



                        this.ShowMinButton = true;
                        this.IsMaxRestoreButtonEnabled = true;
                        this.ShowCloseButton = true;
                    }
                });
               
            };

            //commandOverwrite.Changed.Subscribe(x =>
            //{
            //    x.IsExecuting.Subscribe(x =>
            //    {
            //        if (x is true) ((ContentControl)this.ButtonOverwriteAllFiles.Content).Content = new Run("Обработка");
            //        else ((ContentControl)this.ButtonOverwriteAllFiles.Content).Content = new Run("Выгрузить");
            //    });

            //});


            //IReactiveBinding<MainWindow, ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit>> commandNewDirectory = this.BindCommand(this.ViewModel,
            //vm => vm.SetNewDirectoryCommand,
            //v => v.ButtonNewDirectory);


            //IReactiveBinding<MainWindow, ReactiveCommand<System.Reactive.Unit, System.Reactive.Unit>> commandOpenDirectory = this.BindCommand(this.ViewModel,
            //vm => vm.OpenDirectoryCommand,
            //v => v.ButtonOpenDirectory);




            //var commandSetNewTemaplateCommandAll = this.BindCommand(this.ViewModel,
            //vm => vm.SetNewTemaplateCommand,
            //v => v.ButtonTemplateALL,
            //withParameter:

            //this.WhenAnyValue(x => x.NumericUpDownWork1.Value, x => x.NumericUpDownWork2.Value, x => x.NumericUpDownWork3.Value, x => x.NumericUpDownWork4.Value, x => x.NumericUpDownWork5.Value));


        }

        SetTemplateCommandParams _setTemplateCommand = new SetTemplateCommandParams();
        public SetTemplateCommandParams Property
        {
            get => _setTemplateCommand;

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
            CanvasTopWorkPanel.PreviewMouseLeftButtonDown += (_, e) =>
            {
                if (e.ButtonState is not MouseButtonState.Pressed) return;
                else { this.DragMove(); e.Handled = true; }
            };

            BackgroundDrag.PreviewMouseLeftButtonDown += (_, e) =>
            {
                if (e.ButtonState is not MouseButtonState.Pressed) return;
                else { this.DragMove(); e.Handled = true; }
            };



        }



        public double ColumnWidthDuplicate
        {
            get => this.OneColumn.ActualWidth;
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



        private void NumericUpDownO1_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            var res = ((MahApps.Metro.Controls.NumericUpDown)e.OriginalSource).ToolTip;
            if (res is double result)
            {
                CultureInfo culture = new CultureInfo("ru-RU");
                culture.NumberFormat.NumberGroupSeparator = " ";

                var test = result.ToString("N2", culture);

                ((MahApps.Metro.Controls.NumericUpDown)e.OriginalSource).ToolTip = result.ToString(test);
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


        private void MainWindowObject_StateChanged(object sender, EventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {

                Storyboard storyboard = new Storyboard();


                DoubleAnimation animation = new DoubleAnimation();
                animation.From = 0.7;
                animation.To = 0.79;
                animation.Duration = new Duration(TimeSpan.FromMilliseconds(280));

                Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity"));
                storyboard.Children.Add(animation);


                storyboard.Begin(this.ViewboxBackgroundObject);

                return;
            }

            if (this.WindowState == WindowState.Normal)
            {

                Storyboard storyboard = new Storyboard();
                DoubleAnimation animation = new DoubleAnimation();
                animation.From = 0.79;

                animation.To = 0.37;
                animation.Duration = new Duration(TimeSpan.FromMilliseconds(280));

                Storyboard.SetTargetProperty(animation, new PropertyPath("Opacity"));
                storyboard.Children.Add(animation);

                storyboard.Begin(this.ViewboxBackgroundObject);

                return;


            }
        }

        double MinHidePanel = 30;
        private void TestB_Click(object sender, RoutedEventArgs e)
        {
            if (this.WorkPanelGrid.Visibility == Visibility.Visible)
            {
                GridLengthWorkPanelOpen = OneColumn.Width;
                MinHidePanel = OneColumn.MinWidth;
                OneColumn.MinWidth = 30;
                OneColumn.Width = GridLengthWorkPanelHide;
                return;
            }
            if (this.WorkPanelGrid.Visibility == Visibility.Collapsed)
            {
                OneColumn.MinWidth = MinHidePanel;
                OneColumn.Width = GridLengthWorkPanelOpen;

                return;
            }
        }


        private void MainWindowObject_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (ButtonOpenDirectory.Visibility == Visibility.Collapsed) return;
            if (ButtonOpenDirectory.IsMouseOver is true) return;
            else
            {
                Storyboard reverseStoryboard = new Storyboard();
                ObjectAnimationUsingKeyFrames reverseVisibilityAnimation = new ObjectAnimationUsingKeyFrames();

                DiscreteObjectKeyFrame reverseVisibilityKeyFrame = new DiscreteObjectKeyFrame();
                reverseVisibilityKeyFrame.KeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero);
                reverseVisibilityKeyFrame.Value = Visibility.Collapsed;
                reverseVisibilityAnimation.KeyFrames.Add(reverseVisibilityKeyFrame);
                reverseStoryboard.Children.Add(reverseVisibilityAnimation);

                Storyboard.SetTargetProperty(reverseVisibilityAnimation, new PropertyPath("(UIElement.Visibility)"));
                Storyboard.SetTargetName(this.ButtonOpenDirectory, "ButtonOpenDirectory");

                reverseStoryboard.Begin(this.ButtonOpenDirectory);
            }


        }
    }

    public class MyCulture : CultureInfo
    {
        public MyCulture() : base($"{CultureInfo.InvariantCulture.ToString()}")
        {
        }
        public override NumberFormatInfo NumberFormat
        {
            get
            {
                NumberFormatInfo nfi = (NumberFormatInfo)base.NumberFormat.Clone();
                nfi.NumberGroupSeparator = " ";
                nfi.NumberDecimalDigits = 4;
                return nfi;
            }
        }

        //private string _myStringFormat;

        //public string MyStringFormat
        //{
        //    get => string.Format(this, "{0:N2}", number)
        //    set { myStringFormat = value; }
        //}

    }

    public class ConverterNumericCultureValue : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null || Information.IsNumeric(value) is false) return null;


            CultureInfo newCulture = new CultureInfo("ru-RU");
            newCulture.NumberFormat.NumberGroupSeparator = " ";
            return ((double)value).ToString("N2", newCulture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public class ConverterStringValue : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null || Information.IsNumeric(value) is false) return null;


            CultureInfo newCulture = new CultureInfo("ru-RU");
            newCulture.NumberFormat.NumberGroupSeparator = " ";
            return ((double)value).ToString("N2", newCulture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class InverseVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((Visibility)value == Visibility.Visible)
                return Visibility.Collapsed;
            else
                return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture);
        }
    }


}


