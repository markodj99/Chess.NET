using DiplomskiRad.Commands;

namespace DiplomskiRad.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ViewModelBase _currentViewModel;
        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set  => SetProperty(ref _currentViewModel, value);

        }
        private ColorSelectionViewModel _colorSelectionViewModel;
        private ChessBoardViewModel _chessBoardViewModel;
        //public Command NavigationCommand { get; private set; }

        public MainWindowViewModel()
        {
            _colorSelectionViewModel = new ColorSelectionViewModel();
            _chessBoardViewModel = new ChessBoardViewModel();

            CurrentViewModel = _colorSelectionViewModel;
        }

        //private bool CanExecuteNavigationCommand(object parameter) => true;

        //private void ExecuteNavigationCommand(object parameter)
        //{

        //}
    }
}
