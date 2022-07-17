using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace D2RExpMagnifier.UI.ValueConverter
{
    public class InputCoordinateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string returnValue = String.Empty;

            if (value is double doubleValue) returnValue = doubleValue.ToString();

            return returnValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double returnValue = -999;

            if (value is string stringValue && double.TryParse(stringValue, out double result))
            {
                returnValue = result;
            }

            return returnValue;
        }
    }
}
