using System.Collections.Generic;
using DiplomskiRad.Commands;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.IO;
using System.Windows;
using Accessibility;
using DiplomskiRad.Models.Enums;
using DiplomskiRad.Models.Pieces;

namespace DiplomskiRad.ViewModels
{
    public class PromotionWindowViewModel : INotifyPropertyChanged
    {
        public ICommand FigureCommand { get; private set; }

        private string _imagePathQueen;
        public string ImagePathQueen
        {
            get => _imagePathQueen;
            set
            {
                if (_imagePathQueen == value) return;
                _imagePathQueen = value;
                OnPropertyChanged(nameof(ImagePathQueen));
            }
        }

        private string _imagePathKnight;
        public string ImagePathKnight
        {
            get => _imagePathKnight;
            set
            {
                if (_imagePathKnight == value) return;
                _imagePathKnight = value;
                OnPropertyChanged(nameof(ImagePathKnight));
            }
        }

        private string _imagePathRook;
        public string ImagePathRook
        {
            get => _imagePathRook;
            set
            {
                if (_imagePathRook == value) return;
                _imagePathRook = value;
                OnPropertyChanged(nameof(ImagePathRook));
            }
        }

        private string _imagePathBishop;
        public string ImagePathBishop
        {
            get => _imagePathBishop;
            set
            {
                if (_imagePathBishop == value) return;
                _imagePathBishop = value;
                OnPropertyChanged(nameof(ImagePathBishop));
            }
        }

        private readonly Window _promotionWindow;

        public Color Color { get; set; }

        public Piece Piece { get; set; }

        public PromotionWindowViewModel(Window promotionWindow)
        {
            FigureCommand = new Command(ExecuteFigureCommand, (o) => true);

            _promotionWindow = promotionWindow;
        }

        public void SetUpPromotionPieces(Color color)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var targetFolder = Path.Combine(currentDirectory, "..", "..", "..", "Images");
            Color = color;

            if (color == Color.White)
            {
                ImagePathQueen = Path.Combine(targetFolder, "Queen_W.png");
                ImagePathKnight = Path.Combine(targetFolder, "Knight_W.png");
                ImagePathRook = Path.Combine(targetFolder, "Rook_W.png");
                ImagePathBishop = Path.Combine(targetFolder, "Bishop_W.png");
            }
            else if (color == Color.Black)
            {
                ImagePathQueen = Path.Combine(targetFolder, "Queen_B.png");
                ImagePathKnight = Path.Combine(targetFolder, "Knight_B.png");
                ImagePathRook = Path.Combine(targetFolder, "Rook_B.png");
                ImagePathBishop = Path.Combine(targetFolder, "Bishop_B.png");
            }
        }

        private void ExecuteFigureCommand(object parameter)
        {
            switch ((string)parameter)
            {
                case "queen":
                    Piece = new Queen(Color);
                    break;
                case "knight":
                    Piece = new Knight(Color);
                    break;
                case "rook":
                    Piece = new Rook(Color, false);
                    break;
                case "bishop":
                    Piece = new Bishop(Color);
                    break;
            }

            _promotionWindow.Close();
        }

        public Piece GetPiece() => Piece;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
