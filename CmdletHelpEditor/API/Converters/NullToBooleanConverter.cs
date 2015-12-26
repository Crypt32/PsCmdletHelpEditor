using System;
using System.Globalization;
using System.Windows.Data;

namespace CmdletHelpEditor.API.Converters {
	public class NullToBooleanConverter : IValueConverter {
		public object Convert(Object value, Type targetType, Object parameter, CultureInfo culture) {
			return value != null;
		}
		public object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
