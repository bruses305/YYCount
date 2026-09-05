using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace YYCount.Infrastructure
{
    public class BooleanToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool flag = value is bool && (bool)value;
            string colorName = parameter as string ?? "DarkGreen";
            return flag ? new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorName)) : new SolidColorBrush(Colors.Black);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}