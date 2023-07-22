using DiplomskiRad.Stores;
using DiplomskiRad.Models.Enums;

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
        private EngineStrengthViewModel _engineStrengthViewModel;
        private ChessBoardViewModel _chessBoardViewModel;
        private readonly ColorSelectionStore _colorSelectionStore;

        private Color _color = Color.White;


        public MainWindowViewModel(ColorSelectionStore colorSelectionStore, ColorSelectionViewModel colorSelectionViewModel,
            ChessBoardViewModel chessBoardViewModel, EngineStrengthViewModel engineStrengthViewModel)
        {
            _colorSelectionStore = colorSelectionStore;

            _colorSelectionViewModel = colorSelectionViewModel;
            _engineStrengthViewModel = engineStrengthViewModel;
            _chessBoardViewModel = chessBoardViewModel;

            _colorSelectionStore.ColorSelected += ColorSelected;

            CurrentViewModel = _colorSelectionViewModel;
        }


        private void ColorSelected(Color color )
        {
            _color = color;
            CurrentViewModel = _chessBoardViewModel;
        }
    }
}
