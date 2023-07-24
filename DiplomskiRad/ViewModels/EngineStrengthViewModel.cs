using DiplomskiRad.Commands;
using DiplomskiRad.Models.Enums;
using DiplomskiRad.Stores;
using System.Windows.Input;

namespace DiplomskiRad.ViewModels
{
    public class EngineStrengthViewModel : ViewModelBase
    {
        private int _selectedStrength = 1;
        public int SelectedStrength
        {
            get => _selectedStrength;
            set
            {
                if (_selectedStrength == value) return;
                _selectedStrength = value;
                OnPropertyChanged(nameof(_selectedStrength));
            }
        }

        public ICommand ConfirmCommand { get; private set; }
        public ICommand PuzzleCommand { get; private set; }

        private readonly EngineStrengthStore _engineStrengthStore;

        public EngineStrengthViewModel(EngineStrengthStore engineStrengthStore)
        {
            _engineStrengthStore = engineStrengthStore;

            ConfirmCommand = new Command((o) => _engineStrengthStore.SelectedStrength(SelectedStrength, RatingEvaluation.UserSelected), (o) => true);
            PuzzleCommand = new Command((o) => _engineStrengthStore.SelectedStrength(-1, RatingEvaluation.AutoCalculated), (o) => true);
        }
    }
}
