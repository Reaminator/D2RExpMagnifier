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

            refreshTimer = new System.Timers.Timer(1000);
            refreshTimer.Elapsed += TimedRefresh;
            refreshTimer.Enabled = true;
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
        }

        //UI Private methods

        private void TimedRefresh(object source, ElapsedEventArgs e)
        {
            try
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    System.Windows.Application.Current.MainWindow.Topmost = KeepWindowTopMost;
                });
            }
            catch { }

            refreshTimer.Enabled = false;
            Model.RefreshExp();
            RefreshAllProperties();
            refreshTimer.Enabled = true;
        }

        public bool Status => Model.Status;

        private void RefreshAllProperties()
        {
            RaisePropertyChanged(nameof(Status));
            RaisePropertyChanged(nameof(WindowMode));
            RaisePropertyChanged(nameof(Screens));
            RaisePropertyChanged(nameof(SelectedScreen));
            RaisePropertyChanged(nameof(ResolutionPresets));
            RaisePropertyChanged(nameof(SelectedResolution));
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
        public double Percentage => Model.Percentage;
        public double BarPercentage => Model.BarPercentage;
        public int Bar => Model.Bar;
        public double AddedPercentage => Model.AddedPercentage;
        public List<Screen> Screens => Model.Screens;
        public double StartPercentage => Model.StartPercentage;
        public double AddedBarPercentage => Model.AddedBarPercentage;
        public double StartBarPercentage => Model.StartBarPercentage;
        public TimeSpan TimeToLevel => Model.TimeToLevel;
        public TimeSpan TimeToBar => Model.TimeToBar;
        public double PercentPerHour => Model.PercentPerHour;
        public string DebugText => Model.DebugText;

        public bool WindowMode
        {
            get => Model.WindowMode;
            set
            {
                Model.WindowMode = value;
                RaisePropertyChanged();
            }
        }

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
