using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PL.Converters
{
    public class MultiConditionToVisibilityConverter : IMultiValueConverter
    {
        // Visible רק אם: CurrentCall == null && ManagerMode == false
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2)
                return Visibility.Collapsed;

            var currentCall = values[0];
            var isManager = values[1] as bool?;

            bool shouldShow = currentCall == null && isManager == false;

            return shouldShow ? Visibility.Visible : Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
