using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace D2RExpMagnifier.UI.ValueConverter
{
    public class PercentageToBarConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string returnValue = "*";

            if(parameter is string stringParameter)
            {
                if (value is double doubleValue)
                {
                    returnValue = stringParameter == "Foreground" ? String.Format("{0}*", ((int)doubleValue).ToString()) : String.Format("{0}*", ((int)(100-doubleValue)).ToString());
                }
            }


            return returnValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
