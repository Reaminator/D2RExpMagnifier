using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace D2RExpMagnifier.Model
{
    public class ExpTracker
    {
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [DllImport("User32.Dll")]
        public static extern long SetCursorPos(int x, int y);

        [DllImport("User32.Dll")]
        public static extern bool ClientToScreen(IntPtr hWnd, ref POINT point);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;

            public POINT(int X, int Y)
            {
                x = X;
                y = Y;
            }
        }

        public ExpTracker() 
        {
            AddPresets();

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


        private void AddPresets()
        {
            ResolutionPresets.Add(new ResolutionPreset()
            {
                Name = "2560x1440_Full_Medium",
                Left = 790,
                Right = 1770,
                Height = 1327,
                ForegroundCount = 900,
                WindowMode = false,
                WindowModeXOffset = 0,
                WindowModeYOffset = 0,
                ExpForegroundBrightness = 720,
                ExpBackgroundBrightness = 45
            });

            ResolutionPresets.Add(new ResolutionPreset()
            {
                Name = "2560x1440_Full_Bright",
                Left = 790,
                Right = 1770,
                Height = 1327,
                ForegroundCount = 900,
                WindowMode = false,
                WindowModeXOffset = 0,
                WindowModeYOffset = 0,
                ExpForegroundBrightness = 720,
                ExpBackgroundBrightness = 65
            });

            ResolutionPresets.Add(new ResolutionPreset()
            {
                Name = "2560x1440_Window_Medium",
                Left = 790,
                Right = 1770,
                Height = 1327,
                ForegroundCount = 900,
                WindowMode = true,
                WindowModeXOffset = 10,
                WindowModeYOffset = 33,
                ExpForegroundBrightness = 720,
                ExpBackgroundBrightness = 45
            });

            ResolutionPresets.Add(new ResolutionPreset()
            {
                Name = "2560x1440_Window_Bright",
                Left = 790,
                Right = 1770,
                Height = 1327,
                ForegroundCount = 900,
                WindowMode = true,
                WindowModeXOffset = 10,
                WindowModeYOffset = 33,
                ExpForegroundBrightness = 720,
                ExpBackgroundBrightness = 65
            });

            ResolutionPresets.Add(new ResolutionPreset()
            {
                Name = "1920x1080_Full_Medium",
                Left = 596,
                Right = 1333,
                Height = 996,
                ForegroundCount = 672,
                WindowMode = false,
                WindowModeXOffset = 0,
                WindowModeYOffset = 0,
                ExpForegroundBrightness = 720,
                ExpBackgroundBrightness = 45
            });

            ResolutionPresets.Add(new ResolutionPreset()
            {
                Name = "1920x1080_Full_Bright",
                Left = 596,
                Right = 1333,
                Height = 996,
                ForegroundCount = 672,
                WindowMode = false,
                WindowModeXOffset = 0,
                WindowModeYOffset = 0,
                ExpForegroundBrightness = 720,
                ExpBackgroundBrightness = 65
            });

            ResolutionPresets.Add(new ResolutionPreset()
            {
                Name = "1920x1080_Window_Medium",
                Left = 596,
                Right = 1333,
                Height = 996,
                ForegroundCount = 672,
                WindowMode = true,
                WindowModeXOffset = 10,
                WindowModeYOffset = 33,
                ExpForegroundBrightness = 720,
                ExpBackgroundBrightness = 45
            });

            ResolutionPresets.Add(new ResolutionPreset()
            {
                Name = "1920x1080_Window_Bright",
                Left = 596,
                Right = 1333,
                Height = 996,
                ForegroundCount = 672,
                WindowMode = true,
                WindowModeXOffset = 10,
                WindowModeYOffset = 33,
                ExpForegroundBrightness = 720,
                ExpBackgroundBrightness = 65
            });
        }

        public double StartPercentage { get; set; } = 0.1337;
        
        //public double StartBarPercentage { get; set; } = 0;

        public double StartBarPercentage => (int)(Percentage / 10) > (int)(StartPercentage / 10) ? 0 : (StartPercentage % 10) * 10;

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
            Percentage = 0;
            StartPercentage = Percentage;
            RefreshExp();
            startTime = DateTime.Now;
            StartPercentage = Percentage;
            //StartBarPercentage = BarPercentage;
        }

        public void ResetBarPercentage()
        {
            //StartBarPercentage = BarPercentage;
        }

        //public bool WindowMode { get; set; } = false;

        public void RefreshExp()
        {
            int? startX = GetLeftBound();
            int? endX = GetRightBound();

            int foregroundCount = 0;
            int backgroundCount = 0;
            int unknownCount = 0;

            string startNo = startX != null ? startX.ToString() : "Start not found";
            string endNo = endX != null ? endX.ToString() : "End not found";

            string debugString = startNo + " " + endNo + " Count: ";

            if (startX != null && endX != null)
            {
                List<Color> getCheckPixels = GetColorsBetween((int)startX, (int)endX, (int)SelectedResolution.Height);
                foregroundCount = getCheckPixels.Where(o => IsExpForeground(o)).Count();
                backgroundCount = getCheckPixels.Where(o => IsExpBackground(o)).Count();
                debugString += foregroundCount.ToString();

                double calculatedPercentage = Math.Round(((double)foregroundCount / SelectedResolution.ForegroundCount) * 1000 * 100) / 1000;
                unknownCount = getCheckPixels.Count - (foregroundCount + backgroundCount);

                if(unknownCount < 75)
                {
                    Status = true;

                    if (StartPercentage == 0.1337) ResetStats();

                    Percentage = calculatedPercentage;
                }
                else
                {
                    Status = false;
                }
            }
            else
            {
                Status = false;
            }

            AddDebugText(debugString);
        }

        public void TestLeftCoord() { SetCursorPos((int)(GetWindowOffset().x + SelectedResolution.Left), (int)(GetWindowOffset().y + SelectedResolution.Height)); }
        public void TestRightCoord() { SetCursorPos((int)(GetWindowOffset().x + SelectedResolution.Right), (int)(GetWindowOffset().y + SelectedResolution.Height)); }
        public void TestWindowLeftOffset() { SetCursorPos((int)(GetWindowOffset().x + SelectedResolution.WindowModeXOffset), (int)(GetWindowOffset().y + SelectedResolution.WindowModeYOffset)); }
        public void TestWindowTopOffset() => TestWindowLeftOffset();

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

        public bool Status { get; set; } = false;


        private (int x, int y) GetWindowOffset()
        {
            (int, int) returnValue = (0, 0);

            if (SelectedResolution.WindowMode)
            {
                Process[] processes = Process.GetProcessesByName("D2R");

                if (processes.FirstOrDefault()?.MainWindowHandle is IntPtr d2rHandle)
                {
                    RECT windowPos = new RECT();

                    if (GetWindowRect(d2rHandle, ref windowPos))
                    {
                        returnValue = (windowPos.Left + SelectedResolution.WindowModeXOffset,
                            windowPos.Top + SelectedResolution.WindowModeYOffset);
                    }
                }
            }

            return returnValue;
        }

        private List<Color> GetColorsBetween(int startx, int endx, int y)
        {
            List<Color> returnValue = new List<Color>();

            int xOffset = GetWindowOffset().x;
            int yOffset = GetWindowOffset().y;

            try
            {
                Bitmap bitmap = new Bitmap(endx - startx, 1, PixelFormat.Format32bppArgb);
                Graphics destination = Graphics.FromImage(bitmap);

                destination.CopyFromScreen(xOffset+startx, yOffset+y, 0, 0, new Size(endx - startx, 1), CopyPixelOperation.SourceCopy);

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
                if (IsExpForeground(color) || IsExpBackground(color))
                {
                    foundBar = true;
                }
                else if (!foundBar) break;

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
                if (IsExpForeground(color) || IsExpBackground(color))
                {
                    foundBar = true;
                }
                else if (!foundBar) break;

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
            return ((int)color.R + (int)color.G + (int)color.B) > SelectedResolution.ExpForegroundBrightness;
        }

        private bool IsExpBackground(Color color)
        {
            return ((int)color.R + (int)color.G + (int)color.B) < SelectedResolution.ExpBackgroundBrightness;
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
