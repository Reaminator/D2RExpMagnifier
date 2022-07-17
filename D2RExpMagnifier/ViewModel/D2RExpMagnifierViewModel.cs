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
        /*[DllImport("User32.Dll")]
        public static extern long SetCursorPos(int x, int y);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int BitBlt(IntPtr hDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);*/

        private System.Timers.Timer refreshTimer;


        public ResolutionPreset SelectedResolution { get; set; }

        public D2RExpMagnifierViewModel()
        {
            ResolutionPresets.Add(new ResolutionPreset() { Name = "2560x1440", Left = 778, Right = 1770, Height = 1327 });
            ResolutionPresets.Add(new ResolutionPreset() { Name = "1920x1080", Left = 589, Right = 1325, Height = 994 });
            SelectedResolution = ResolutionPresets.First();

            TestButtonCommand = new DelegateCommand<object>(RefreshExp);
            ResetStatsCommand = new DelegateCommand<object>(ResetStats);
            refreshTimer = new System.Timers.Timer(1000);
            refreshTimer.Elapsed += TimedRefresh;
            refreshTimer.Enabled = true;
            GetScreens();
        }

        private void TimedRefresh(object source, ElapsedEventArgs e)
        {
            refreshTimer.Enabled = false;
            RefreshExp(null);
            refreshTimer.Enabled = true;
        }

        private void GetScreens()
        {
            foreach (Screen screen in Screen.AllScreens)
            {
                Screens.Add(screen);
            }

            SelectedScreen = Screens.FirstOrDefault();
        }

        private List<Color> GetColorsBetween(int startx, int endx, int y)
        {
            List<Color> returnValue = new List<Color>();

            try
            {
                Bitmap bitmap = new Bitmap(endx - startx, 1, PixelFormat.Format32bppArgb);
                Graphics destination = Graphics.FromImage(bitmap);

                destination.CopyFromScreen(startx, y, 0, 0, new Size(endx - startx, 1), CopyPixelOperation.SourceCopy);

                for (int i = 0; i < (endx - startx); i++)
                {
                    returnValue.Add(bitmap.GetPixel(i, 0));
                }
            }
            catch { }

            return returnValue;
        }

        public Screen? SelectedScreen 
        { 
            get => selectedScreen;
            set
            {
                selectedScreen = value;
                RaisePropertyChanged();
            }
        }

        private Screen? selectedScreen = null;

        public ObservableCollection<ResolutionPreset> ResolutionPresets { get; } = new ObservableCollection<ResolutionPreset>();


        public event PropertyChangedEventHandler? PropertyChanged;
        
        public DelegateCommand<object> TestButtonCommand { get; }
        public DelegateCommand<object> ResetStatsCommand { get; }

        public List<Screen> Screens { get; } = new List<Screen>();

        public void ViewLoaded()
        {
        }

        private bool IsExpForeground(Color color)
        {
            return ((int)color.R + (int)color.G + (int)color.B) > 720;
        }

        private bool IsExpBackground(Color color)
        {
            return ((int)color.R + (int)color.G + (int)color.B) < 40;
        }

        private int? GetLeftBound()
        {
            int? returnValue = null;
            bool foundBar = false;

            List<Color> getCheckPixels = GetColorsBetween((int)SelectedResolution.Left - 100, (int)SelectedResolution.Left, (int)SelectedResolution.Height);
            getCheckPixels.Reverse();

            foreach(Color color in getCheckPixels)
            {
                if (IsExpForeground(color) || IsExpBackground(color)) foundBar = true;

                if (foundBar && !IsExpBackground(color) && !IsExpForeground(color))
                {
                    returnValue = (int)SelectedResolution.Left - getCheckPixels.IndexOf(color);
                    break;
                }
            }

            return returnValue;
        }

        private int? GetRightBound()
        {
            int? returnValue = null;
            bool foundBar = false;

            List<Color> getCheckPixels = GetColorsBetween((int)SelectedResolution.Right, (int)SelectedResolution.Right + 100, (int)SelectedResolution.Left);

            foreach (Color color in getCheckPixels)
            {
                if (IsExpForeground(color) || IsExpBackground(color)) foundBar = true;

                if (foundBar && !IsExpBackground(color) && !IsExpForeground(color))
                {
                    returnValue = (int)SelectedResolution.Right + getCheckPixels.IndexOf(color);
                    break;
                }
            }

            return returnValue;
        }

        private double percentage = 0;
        public double Percentage
        {
            get => percentage;
            set
            {
                percentage = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(BarPercentage));
                RaisePropertyChanged(nameof(Bar));
                RaisePropertyChanged(nameof(TimeToLevel));
                RaisePropertyChanged(nameof(PercentPerHour));
                RaisePropertyChanged(nameof(TimeToBar));
            }
        }

        public double BarPercentage => Math.Round((percentage % 10)*1000)/100;

        public int Bar => ((int)(percentage / 10))+1;

        private double startPercentage = -999;
        private DateTime startTime = DateTime.Now;

        public TimeSpan TimeToLevel => CalculateTimeToLevel();

        public TimeSpan TimeToBar => TimeSpan.FromHours((100 - BarPercentage) / (PercentPerHour * 10));

        private TimeSpan CalculateTimeToLevel()
        {
            TimeSpan returnValue = new TimeSpan(0, 0, 1);
            
            if (Percentage > startPercentage)
            {
                double gain = Percentage - startPercentage;
                TimeSpan time = DateTime.Now - startTime;

                returnValue = time*((100 - Percentage) / gain);
            }

            return returnValue;
        }

        public double PercentPerHour => Math.Round( ((percentage - startPercentage) / (DateTime.Now - startTime).TotalHours)*10 )/10;

        private void ResetStats(object parameter)
        {
            startTime = DateTime.Now;
            startPercentage = Percentage;
            RaisePropertyChanged(nameof(TimeToLevel));
        }

        private void RefreshExp(object parameter)
        {
            int? startX = GetLeftBound();
            int? endX = GetRightBound();

            int foregroundCount = 0;

            if (startX != null && endX != null)
            {
                if (startPercentage == -999)
                {
                    startPercentage = Percentage;
                    startTime = DateTime.Now;
                }

                List<Color> getCheckPixels = GetColorsBetween((int)startX, (int)endX, (int)SelectedResolution.Height);
                foregroundCount = getCheckPixels.Where(o => IsExpForeground(o)).Count();
                double calculatedPercentage = Math.Round( ((double)foregroundCount / 900) * 1000*100)/1000;

                if (calculatedPercentage > startPercentage) Percentage = calculatedPercentage;
            }
        }

        private void AddDebugText(string text)
        {
            debugText.Insert(0,
               String.Format(
                   "{0}: {1}",
                   DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    text));

            RaisePropertyChanged(nameof(DebugText));
        }

        private List<string> debugText = new List<string>();

        public string DebugText => String.Join("\r\n", debugText);

        private void RaisePropertyChanged([CallerMemberName] string memberName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
    }
}
