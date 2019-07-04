using System;
using System.Globalization;
using System.Windows.Data;

namespace PsCmdletHelpEditor.Wpf.Views.Converters {
    public class NullToBooleanConverter : IValueConverter {
        public Object Convert(Object value, Type targetType, Object parameter, CultureInfo culture) {
            return value != null;
        }
        public Object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
