using System;
using System.Globalization;
using System.Windows.Data;

namespace PL.Converters
{
    public class BoolToSimulatorButtonTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (bool)value ? "stop simulator" : "start simulator";

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
