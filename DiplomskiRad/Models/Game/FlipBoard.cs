using DiplomskiRad.Models.Enums;
using System.ComponentModel;

namespace DiplomskiRad.Models.Game
{
    public class FlipBoard : INotifyPropertyChanged
    {
        private Color _oriantation;

        public Color Orientation
        {
            get => _oriantation;
            set
            {
                if (value != _oriantation)
                {
                    Flip();
                    _oriantation = value;
                }
            }
        }

        private double _scaleX;
        public double ScaleX
        {
            get => _scaleX;
            set
            {
                if (_scaleX == value) return;
                _scaleX = value;
                OnPropertyChanged(nameof(ScaleX));
            }
        }
        private double _scaleY;
        public double ScaleY
        {
            get => _scaleY;
            set
            {
                if (_scaleY == value) return;
                _scaleY = value;
                OnPropertyChanged(nameof(ScaleY));
            }
        }

        public FlipBoard()
        {
            Orientation = Color.White;
            ScaleX = 1f;
            ScaleY = 1f;
        }

        public void Flip()
        {
            ScaleX = -ScaleX;
            ScaleY = -ScaleY;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
