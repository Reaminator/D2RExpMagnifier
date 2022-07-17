using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace D2RExpMagnifier.UI.ValueConverter
{
    public class TimeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string returnValue = "";

            if(parameter is string stringParameter)
            {
                if (value is TimeSpan timeValue)
                {
                    if (stringParameter == "H") returnValue = Math.Floor(timeValue.TotalHours).ToString();
                    if (stringParameter == "M") returnValue = timeValue.Minutes.ToString();
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
