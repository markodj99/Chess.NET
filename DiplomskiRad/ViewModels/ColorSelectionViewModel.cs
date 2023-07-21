using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DiplomskiRad.Commands;
using DiplomskiRad.Models.Enums;

namespace DiplomskiRad.ViewModels
{
    public class ColorSelectionViewModel : ViewModelBase
    {
        public ICommand WhiteCommand { get; private set; }
        public ICommand BlackCommand { get; private set; }
        private Color SelectedColor { get; set; }

        public ColorSelectionViewModel()
        {
            WhiteCommand = new Command(ExecuteWhiteCommand, CanExecuteWhiteCommand);
            BlackCommand = new Command(ExecuteBlackCommand, CanExecuteBlackCommand);
        }

        private bool CanExecuteWhiteCommand(object parameter) => true;
        private void ExecuteWhiteCommand(object parameter) => SelectedColor = Color.White;

        private bool CanExecuteBlackCommand(object parameter) => true;
        private void ExecuteBlackCommand(object parameter) => SelectedColor = Color.Black;
    }
}
