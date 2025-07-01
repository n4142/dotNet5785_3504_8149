using System.Globalization;
using System.Windows.Data;
using System.Windows;

namespace PL.Converters
{
    public class ManagerAndNotNewVolunteerToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2)
                return Visibility.Collapsed;

            bool isManager = values[0] is bool manager && manager;
            bool hasValidId = values[1] is int id && id != 0;

            return isManager && hasValidId ? Visibility.Visible : Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}