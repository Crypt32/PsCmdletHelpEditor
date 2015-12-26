using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CmdletHelpEditor.API.Converters {
	class BooleanToVisibilityReverse : IValueConverter {
		public object Convert(Object value, Type targetType, Object parameter, CultureInfo culture) {
			return (Boolean)value
				? Visibility.Collapsed
				: Visibility.Visible;
		}
		public object ConvertBack(Object value, Type targetType, Object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
