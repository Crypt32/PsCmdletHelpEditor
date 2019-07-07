using System;
using System.Windows;
using System.Windows.Controls;
using PsCmdletHelpEditor.BLL.Abstraction.Controls;

namespace PsCmdletHelpEditor.Wpf.Views.Controls {
    class FormattableTextBox : TextBox, IFormattableTextBox {
        public static readonly DependencyProperty AllowFormatProperty = DependencyProperty.Register(
            nameof(AllowFormat),
            typeof(Boolean),
            typeof(FormattableTextBox),
            new PropertyMetadata(default(Boolean)));

        public Boolean AllowFormat {
            get => (Boolean)GetValue(AllowFormatProperty);
            set => SetValue(AllowFormatProperty, value);
        }
    }
}
