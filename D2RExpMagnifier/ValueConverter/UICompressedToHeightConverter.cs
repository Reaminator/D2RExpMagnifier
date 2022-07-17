using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace D2RExpMagnifier.UI.ValueConverter
{
    public class UICompressedToHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Visibility returnValue = Visibility.Visible;

            if (value is bool boolValue && boolValue)
            {
                returnValue = Visibility.Collapsed;
            }


            return returnValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
