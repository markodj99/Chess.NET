using System.ComponentModel;
using DiplomskiRad.Models.Pieces;

namespace DiplomskiRad.Models.Game
{
    public class ChessSquare : INotifyPropertyChanged
    {
        public int Row { get; set; }
        public int Column { get; set; }

        private string _color;
        public string Color
        {
            get => _color;
            set
            {
                if (_color == value) return;
                _color = value;
                OnPropertyChanged(nameof(Color));
            }
        }
        public Piece Piece { get; set; }
        private string _imagePath;
        public string ImagePath
        {
            get => _imagePath;
            set
            {
                if (_imagePath == value) return;
                _imagePath = value;
                OnPropertyChanged(nameof(ImagePath));
            }
        }

        public ChessSquare(ChessSquare other)
        {
            Row = other.Row;
            Column = other.Column;
            Color = other.Color;
            Piece = other.Piece;
            ImagePath = other.ImagePath;
        }

        public ChessSquare() { }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;

            var other = (ChessSquare)obj;
            return Row == other.Row && Column == other.Column;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
