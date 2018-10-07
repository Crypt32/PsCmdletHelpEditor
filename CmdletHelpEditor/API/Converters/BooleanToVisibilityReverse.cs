using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CmdletHelpEditor.API.Converters {
    class BooleanToVisibilityReverse : IValueConverter {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture) {
            return (Boolean)value
                ? Visibility.Collapsed
                : Visibility.Visible;
        }
        public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
