using DiplomskiRad.Models.Enums;
using DiplomskiRad.Stores;

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
        private readonly EngineStrengthStore _engineStrengthStore;

        public MainWindowViewModel(ColorSelectionStore colorSelectionStore, EngineStrengthStore engineStrengthStore, ColorSelectionViewModel colorSelectionViewModel,
            ChessBoardViewModel chessBoardViewModel, EngineStrengthViewModel engineStrengthViewModel)
        {
            _colorSelectionStore = colorSelectionStore;
            _engineStrengthStore = engineStrengthStore;

            _colorSelectionViewModel = colorSelectionViewModel;
            _engineStrengthViewModel = engineStrengthViewModel;
            _chessBoardViewModel = chessBoardViewModel;

            _colorSelectionStore.ColorSelected += ColorSelected;
            _engineStrengthStore.StrengthSelected += StrengthSelected;

            CurrentViewModel = _colorSelectionViewModel;
        }

        private void ColorSelected(Color color )
        {
            _chessBoardViewModel.PlayerColor = color;
            CurrentViewModel = _engineStrengthViewModel;
        }

        private void StrengthSelected(int selectedStrength, RatingEvaluation evaluation)
        {
            _chessBoardViewModel.EngineStrength = selectedStrength;
            _chessBoardViewModel.BoardSetUp(evaluation);
            CurrentViewModel = _chessBoardViewModel;
        }

        public override void Dispose()
        {
            _colorSelectionStore.ColorSelected -= ColorSelected;
            _engineStrengthStore.StrengthSelected -= StrengthSelected;
        }
    }
}
