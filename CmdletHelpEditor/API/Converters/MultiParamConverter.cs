using System;
using System.Globalization;
using System.Windows.Data;

namespace CmdletHelpEditor.API.Converters;
public class MultiParamConverter : IMultiValueConverter {
    public Object Convert(Object[] value, Type targetType, Object parameter, CultureInfo culture) {
        return value.Clone();
    }

    public Object[] ConvertBack(Object value, Type[] targetTypes, Object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}
