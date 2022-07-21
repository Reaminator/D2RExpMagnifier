using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2RExpMagnifier.Model
{
    public class ResolutionPreset
    {
        public string Name { get; set; } = "undefined";
        public double Left { get; set; }
        public double Right { get; set; }
        public double Height { get; set; }

        public int ExpForegroundBrightness { get; set; }
        public int ExpBackgroundBrightness { get; set; }

        public bool WindowMode { get; set; }

        public int WindowModeXOffset { get; set; }
        public int WindowModeYOffset { get; set; }


        public double ForegroundCount { get; set; }
    }
}
