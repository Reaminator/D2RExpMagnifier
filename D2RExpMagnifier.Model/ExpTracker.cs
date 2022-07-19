using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;

namespace D2RExpMagnifier.Model
{
    public class ExpTracker
    {
        public ExpTracker() 
        {
            ResolutionPresets.Add(new ResolutionPreset() { Name = "2560x1440", Left = 790, Right = 1770, Height = 1327, ForegroundCount = 900 });
            ResolutionPresets.Add(new ResolutionPreset() { Name = "1920x1080", Left = 596, Right = 1333, Height = 996, ForegroundCount = 672 });

            GetScreens();

            if (Screens.FirstOrDefault(p => p.Primary)?.Bounds.Width.ToString() is string screenWidth && !String.IsNullOrEmpty(screenWidth))
            {
                SelectedResolution = ResolutionPresets.Where(o => o.Name.StartsWith(screenWidth)).FirstOrDefault() ?? ResolutionPresets.First();
            }
            else
            {
                SelectedResolution = ResolutionPresets.First();
            }
        }

        public double StartPercentage { get; set; } = 0;
        public double StartBarPercentage { get; set; } = 0;

        public double Percentage { get; set; } = 0;

        public double BarPercentage => Math.Round((Percentage % 10) * 1000) / 100;

        public int Bar => ((int)(Percentage / 10)) + 1;

        public double AddedBarPercentage => Math.Round((BarPercentage - StartBarPercentage) * 100) / 100;
        public double AddedPercentage => Math.Round((Percentage - StartPercentage) * 100) / 100;

        public double PercentPerHour => Math.Round(((Percentage - StartPercentage) / (DateTime.Now - startTime).TotalHours) * 10) / 10;

        public TimeSpan TimeToLevel => CalculateTimeToLevel();

        public TimeSpan TimeToBar => TimeSpan.FromHours((100 - BarPercentage) / (0.01 + (PercentPerHour * 10)));

        private TimeSpan CalculateTimeToLevel()
        {
            TimeSpan returnValue = new TimeSpan(0, 0, 1);

            if (Percentage > StartPercentage)
            {
                double gain = Percentage - StartPercentage;
                TimeSpan time = DateTime.Now - startTime;

                returnValue = time * ((100 - Percentage) / gain);
            }

            return returnValue;
        }

        public void ResetStats()
        {
            startTime = DateTime.Now;
            StartPercentage = Percentage;
            StartBarPercentage = BarPercentage;
        }

        public void ResetBarPercentage()
        {
            StartBarPercentage = BarPercentage;
        }


        public void RefreshExp()
        {
            int? startX = GetLeftBound();
            int? endX = GetRightBound();

            int foregroundCount = 0;

            string startNo = startX != null ? startX.ToString() : "Start not found";
            string endNo = endX != null ? endX.ToString() : "End not found";

            string debugString = startNo + " " + endNo + " Count: ";

            if (startX != null && endX != null)
            {
                if (StartPercentage == -999)
                {
                    StartPercentage = Percentage;
                    startTime = DateTime.Now;
                }

                List<Color> getCheckPixels = GetColorsBetween((int)startX, (int)endX, (int)SelectedResolution.Height);
                foregroundCount = getCheckPixels.Where(o => IsExpForeground(o)).Count();
                debugString += foregroundCount.ToString();

                double calculatedPercentage = Math.Round(((double)foregroundCount / SelectedResolution.ForegroundCount) * 1000 * 100) / 1000;

                Percentage = calculatedPercentage;
            }

            AddDebugText(debugString);
        }

        public ResolutionPreset SelectedResolution { get; set; }

        public List<ResolutionPreset> ResolutionPresets { get; } = new List<ResolutionPreset>();
        public List<Screen> Screens { get; } = new List<Screen>();

        public Screen? SelectedScreen { get; set; }

        private DateTime startTime = DateTime.Now;


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

        private int? GetLeftBound()
        {
            int? returnValue = null;
            bool foundBar = false;

            List<Color> getCheckPixels = GetColorsBetween((int)SelectedResolution.Left - 100, (int)SelectedResolution.Left, (int)SelectedResolution.Height);
            getCheckPixels.Reverse();

            foreach (Color color in getCheckPixels)
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

            List<Color> getCheckPixels = GetColorsBetween((int)SelectedResolution.Right, (int)SelectedResolution.Right + 100, (int)SelectedResolution.Height);

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

        private bool IsExpForeground(Color color)
        {
            return ((int)color.R + (int)color.G + (int)color.B) > 720;
        }

        private bool IsExpBackground(Color color)
        {
            return ((int)color.R + (int)color.G + (int)color.B) < 65;
        }

        private bool debugOn = false;

        public void AddDebugText(string text)
        {
            if (debugOn)
            {
                debugText.Insert(0,
               String.Format(
                   "{0}: {1}",
                   DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    text));
            }

        }

        private List<string> debugText = new List<string>();

        public string DebugText => String.Join("\r\n", debugText);
    }
}
