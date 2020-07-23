using System.Windows.Input;
using CmdletHelpEditor.Abstract;

namespace CmdletHelpEditor.API.ViewModels {
    public class FileOperations {
        readonly IDataSource _dataSource;

        public FileOperations(IDataSource dataSource) {
            _dataSource = dataSource;
        }

        public ICommand NewCommand { get; set; }
        public ICommand OpenCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand CloseAppCommand { get; set; }
    }
}