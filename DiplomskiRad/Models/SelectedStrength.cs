using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DiplomskiRad.Models
{
    public class SelectedStrength : INotifyPropertyChanged
    {
        private int _selectedLevel;

        public int SelectedLevel
        {
            get => _selectedLevel;
            set
            {
                if (_selectedLevel == value) return;
                _selectedLevel = value;
                OnPropertyChanged(nameof(_selectedLevel));
            }
        }

        public SelectedStrength() => _selectedLevel = 1;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
