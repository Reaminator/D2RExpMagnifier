﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace D2RExpMagnifier.UI.ValueConverter
{
    public class UICompressedToBackgroundConverter : IValueConverter
    {
        private UICompressedToBackgroundConverter() { }
        static UICompressedToBackgroundConverter() { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            WindowStyle returnValue = WindowStyle.SingleBorderWindow;

            if (value is bool boolValue && boolValue)
            {
                returnValue = WindowStyle.None;
            }

            return returnValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public static UICompressedToBackgroundConverter Instance { get; } = new UICompressedToBackgroundConverter();
    }
}
