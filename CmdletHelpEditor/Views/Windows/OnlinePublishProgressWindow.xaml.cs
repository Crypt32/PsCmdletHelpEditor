using System;
using System.Windows;
using CmdletHelpEditor.Abstract;
using CmdletHelpEditor.API.Models;
using CmdletHelpEditor.API.ViewModels;
using Unity;

namespace CmdletHelpEditor.Views.Windows {
    /// <summary>
    /// Interaction logic for OnlinePublishProgressWindow.xaml
    /// </summary>
    public partial class OnlinePublishProgressWindow : IScrollToView {
        public OnlinePublishProgressWindow(ModuleObject module) {
            InitializeComponent();
            IOnlinePublishProgressVM dc = App.Container.Resolve<IOnlinePublishProgressVM>();
            dc.SetModule(module);
            DataContext = dc;
        }

        void CloseClick(Object sender, RoutedEventArgs e) {
            Close();
        }
        public void ScrollIntoView(Object item) {
            lv.ScrollIntoView(item);
        }
    }
}
