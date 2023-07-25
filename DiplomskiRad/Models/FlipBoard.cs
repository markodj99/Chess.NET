using System.ComponentModel;

namespace DiplomskiRad.Models
{
    public class FlipBoard : INotifyPropertyChanged
    {
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
