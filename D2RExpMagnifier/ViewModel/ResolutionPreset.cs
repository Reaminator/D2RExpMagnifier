using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2RExpMagnifier.UI.ViewModel
{
    public class ResolutionPreset
    {
        public string Name { get; set; } = "undefined";
        public double Left { get; set; }
        public double Right { get; set; }
        public double Height { get; set; }

        public double ForegroundCount { get; set; }
    }
}
