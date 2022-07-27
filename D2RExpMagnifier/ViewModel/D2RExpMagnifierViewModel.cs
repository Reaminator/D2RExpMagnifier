using D2RExpMagnifier.Model;
using Microsoft.VisualStudio.PlatformUI;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace D2RExpMagnifier.UI.ViewModel
{
    public class D2RExpMagnifierViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public ExpTracker Model { get; } = new ExpTracker();

        public D2RExpMagnifierViewModel()
        {
            TestButtonCommand = new DelegateCommand<object>(RefreshExp);
            ResetStatsCommand = new DelegateCommand<object>(ResetStats);
            CloseApplicationCommand = new DelegateCommand<object>(CloseApplication);
            ShrinkWidthCommand = new DelegateCommand<object>(ShrinkWidth);
            ExpandWidthCommand = new DelegateCommand<object>(ExpandWidth);
            ResetBarPercentageCommand = new DelegateCommand<object>(ResetBarPercentage);
            TestLeftCoordCommand = new DelegateCommand<object>(TestLeftCoord);
            TestRightCoordCommand = new DelegateCommand<object>(TestRightCoord);
            TestWindowLeftOffsetCommand = new DelegateCommand<object>(TestWindowLeftOffset);
            TestWindowTopOffsetCommand = new DelegateCommand<object>(TestWindowTopOffset);
        }

        //UI Fields
        private bool uicompressed = false;
        private System.Timers.Timer refreshTimer;
        private List<Double> windowWidths = new List<Double> { 200, 300, 400, 600, 800, 1000, 1200 };
        private double selectedWidth = 400;

        //UI Properties
        public DelegateCommand<object> TestButtonCommand { get; }
        public DelegateCommand<object> ResetStatsCommand { get; }
        public DelegateCommand<object> ResetBarPercentageCommand { get; }
        public DelegateCommand<object> ShrinkWidthCommand { get; }
        public DelegateCommand<object> ExpandWidthCommand { get; }
        public DelegateCommand<object> CloseApplicationCommand { get; }
        public DelegateCommand<object> TestLeftCoordCommand { get; }
        public DelegateCommand<object> TestRightCoordCommand { get; }
        public DelegateCommand<object> TestWindowLeftOffsetCommand { get; }
        public DelegateCommand<object> TestWindowTopOffsetCommand { get; }

        public bool KeepWindowTopMost { get; set; } = false;

        public bool UICompressed
        {
            get => uicompressed;
            set
            {
                uicompressed = value;
                RaisePropertyChanged();
            }
        }

        public double SelectedWidth
        {
            get => selectedWidth;
            set
            {
                selectedWidth = value;
                RaisePropertyChanged();
            }
        }

        //UI Public methods
        public void GridClicked()
        {
            UICompressed = !UICompressed;
        }

        public void ViewLoaded()
        {
            RefreshAllProperties();
            RaisePropertyChanged(nameof(SelectedResolution));
            refreshTimer = new System.Timers.Timer(1000);
            refreshTimer.Elapsed += TimedRefresh;
            refreshTimer.Enabled = true;
        }

        //UI Private methods

        private void TimedRefresh(object source, ElapsedEventArgs e)
        {
            refreshTimer.Enabled = false;

            try
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    Model.RefreshExp();
                    RefreshAllProperties();
                    System.Windows.Application.Current.MainWindow.Topmost = KeepWindowTopMost;
                });
            }
            finally { refreshTimer.Enabled = true; }
        }

        public bool Status => Model.Status;

        private void RefreshAllProperties()
        {
            RaisePropertyChanged(nameof(Status));
            RaisePropertyChanged(nameof(Screens));
            RaisePropertyChanged(nameof(SelectedScreen));
            RaisePropertyChanged(nameof(ResolutionPresets));
            RaisePropertyChanged(nameof(Percentage));
            RaisePropertyChanged(nameof(BarPercentage));
            RaisePropertyChanged(nameof(Bar));
            RaisePropertyChanged(nameof(TimeToLevel));
            RaisePropertyChanged(nameof(PercentPerHour));
            RaisePropertyChanged(nameof(TimeToBar));
            RaisePropertyChanged(nameof(AddedPercentage));
            RaisePropertyChanged(nameof(AddedBarPercentage));
            RaisePropertyChanged(nameof(DebugText));
            RaisePropertyChanged(nameof(StartPercentage));
            RaisePropertyChanged(nameof(StartBarPercentage));
            RaisePropertyChanged(nameof(GameTime));
            RaisePropertyChanged(nameof(AverageGameTime));
            RaisePropertyChanged(nameof(GameCount));
            RaisePropertyChanged(nameof(PercentPerGame));
            RaisePropertyChanged(nameof(GamesToLevel));
            RaisePropertyChanged(nameof(TotalGameTime));
        }

        private void CloseApplication(object parameter)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void ShrinkWidth(object parameter)
        {
            if (windowWidths.IndexOf(SelectedWidth) > 0)
            {
                SelectedWidth = windowWidths[windowWidths.IndexOf(SelectedWidth) - 1];
            }
        }

        private void ExpandWidth(object parameter)
        {
            if (windowWidths.IndexOf(SelectedWidth) < windowWidths.Count - 1)
            {
                SelectedWidth = windowWidths[windowWidths.IndexOf(SelectedWidth) + 1];
            }
        }

        public int GameCount => Model.GameCount;

        public string TotalGameTime => String.Format("{0}:{1}:{2}", pad(Model.TotalGameTime.Hours), pad(Model.TotalGameTime.Minutes), pad(Model.TotalGameTime.Seconds));

        public string GameTime => Status ? String.Format("{0}:{1}:{2}", pad(Model.GameTime.Hours), pad(Model.GameTime.Minutes), pad(Model.GameTime.Seconds)) : "?";

        public string AverageGameTime => String.Format("{0}:{1}:{2}", pad(Model.AverageGameTime.Hours), pad(Model.AverageGameTime.Minutes), pad(Model.AverageGameTime.Seconds));

        private string pad(int number)
        {
            string returnValue = number.ToString();

            while(returnValue.Length < 2)
            {
                returnValue = string.Format("0{0}", returnValue);
            }

            return returnValue;
        }

        private void RaisePropertyChanged([CallerMemberName] string memberName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));

        //Model properties
        public ResolutionPreset SelectedResolution
        {
            get => Model.SelectedResolution;
            set
            {
                Model.SelectedResolution = value;
                RaisePropertyChanged();
            }
        }

        public Screen? SelectedScreen 
        {
            get => Model.SelectedScreen;
            set
            {
                Model.SelectedScreen = value;
                RaisePropertyChanged();
            }
        }

        public List<ResolutionPreset> ResolutionPresets => Model.ResolutionPresets;
        public double Percentage => round(Model.Percentage, 1);
        public double BarPercentage => round(Model.BarPercentage, 0);
        public int Bar => Model.Bar;
        public double AddedPercentage => round(Model.AddedPercentage, 1);
        public List<Screen> Screens => Model.Screens;
        public double StartPercentage => round(Model.StartPercentage, 1);
        public double AddedBarPercentage => round(Model.AddedBarPercentage, 0);
        public double StartBarPercentage => round(Model.StartBarPercentage, 0);
        public TimeSpan TimeToLevel => Model.TimeToLevel;
        public TimeSpan TimeToBar => Model.TimeToBar;
        public double PercentPerHour => round(Model.PercentPerHour,2);

        public double PercentPerGame => round(Model.PercentPerGame,2);

        private double round(double input, int decimals)
        {
            return Math.Round(input * Math.Pow(10, decimals)) / Math.Pow(10, decimals);
        }

        public int GamesToLevel => Model.GamesToLevel;

        public string DebugText => Model.DebugText;

        //UI Relayed Methods
        private void ResetStats(object parameter)
        {
            Model.ResetStats();
            RefreshAllProperties();
        }

        private void ResetBarPercentage(object parameter)
        {
            Model.ResetBarPercentage();
            RefreshAllProperties();
        }

        private void TestLeftCoord(object parameter) => Model.TestLeftCoord();
        private void TestRightCoord(object parameter) => Model.TestRightCoord();
        private void TestWindowLeftOffset(object parameter) => Model.TestWindowLeftOffset();
        private void TestWindowTopOffset(object parameter) => Model.TestWindowTopOffset();

        private void RefreshExp(object parameter)
        {
            Model.RefreshExp();
            RefreshAllProperties();
        }

        private void AddDebugText(string text)
        {
            Model.AddDebugText(text);
            RaisePropertyChanged(nameof(DebugText));
        }
    }
}
