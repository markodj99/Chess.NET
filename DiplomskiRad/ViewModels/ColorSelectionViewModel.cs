using DiplomskiRad.Commands;
using DiplomskiRad.Models.Enums;
using DiplomskiRad.Stores;
using System.Windows.Input;

namespace DiplomskiRad.ViewModels
{
    public class ColorSelectionViewModel : ViewModelBase
    {
        public ICommand WhiteCommand { get; private set; }
        public ICommand BlackCommand { get; private set; }
        private readonly ColorSelectionStore _colorSelectionStore;

        public ColorSelectionViewModel(ColorSelectionStore colorSelectionStore)
        {
            _colorSelectionStore = colorSelectionStore;

            WhiteCommand = new Command((o) => _colorSelectionStore.SelectColor(Color.White), (o) => true);
            BlackCommand = new Command((o) => _colorSelectionStore.SelectColor(Color.Black), (o) => true);
        }
    }
}
