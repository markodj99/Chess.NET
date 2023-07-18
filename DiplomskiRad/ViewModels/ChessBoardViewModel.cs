using DiplomskiRad.Commands;
using DiplomskiRad.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace DiplomskiRad.ViewModels
{
    public class ChessBoardViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ChessSquare> ChessSquares { get; set; }

        private readonly string[,] _initialPieceOrder = new string[,]
        {
            { "Rook", "Knight", "Bishop", "Queen", "King", "Bishop", "Knight", "Rook" },
            { "Pawn", "Pawn", "Pawn", "Pawn", "Pawn", "Pawn", "Pawn", "Pawn" },
            { "", "", "", "", "", "", "", "" },
            { "", "", "", "", "", "", "", "" },
            { "", "", "", "", "", "", "", "" },
            { "", "", "", "", "", "", "", "" },
            { "Pawn", "Pawn", "Pawn", "Pawn", "Pawn", "Pawn", "Pawn", "Pawn" },
            { "Rook", "Knight", "Bishop", "Queen", "King", "Bishop", "Knight", "Rook" }
        };

        public ChessBoardViewModel()
        {
            ChessSquares = SetUpBoard();
            AvailableMoves = new ObservableCollection<ChessSquare>();

            MoveCommand = new Command(ExecuteMoveCommand, CanExecuteMoveCommand);
        }

        private ObservableCollection<ChessSquare> SetUpBoard()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var targetFolder = Path.Combine(currentDirectory, "..", "..", "..", "Images");

            var chessSquares = new ObservableCollection<ChessSquare>();
            for (int row = 0; row < 8; row++)
            {
                for (int column = 0; column < 8; column++)
                {
                    var square = new ChessSquare
                    {
                        Row = row,
                        Column = column,
                        Color = (row + column) % 2 == 0 ? "White" : "Black"
                    };

                    var c = row is not (0 or 1) ? "W" : "B";

                    var pieceName = _initialPieceOrder[row, column];
                    if (!string.IsNullOrEmpty(pieceName))
                    {
                        Piece piece = pieceName switch
                        {
                            "Pawn" => new Pawn(square.Color),
                            "Rook" => new Rook(square.Color),
                            "Knight" => new Knight(square.Color),
                            "Bishop" => new Bishop(square.Color),
                            "Queen" => new Queen(square.Color),
                            _ => new King(square.Color)
                        };

                        var imagePath = Path.Combine(targetFolder, $"{pieceName}_{c}.png");
                        square.Piece = piece;
                        square.ImagePath = imagePath;
                    }
                    else square.ImagePath = null;

                    chessSquares.Add(square);
                }
            }

            return chessSquares;
        }

        public ICommand MoveCommand { get; private set; }

        private ChessSquare _selectedSquare;
        public ChessSquare SelectedSquare
        {
            get => _selectedSquare; 
            set
            {
                _selectedSquare = value;
                OnPropertyChanged(nameof(SelectedSquare));
                UpdateAvailableMoves();
            }
        }

        public ObservableCollection<ChessSquare> AvailableMoves { get; private set; }

        public event PropertyChangedEventHandler? PropertyChanged;


        private bool CanExecuteMoveCommand(object parameter)
        {
            var selectedSquare = parameter as ChessSquare;

            if (selectedSquare?.ImagePath == null)
            {
                MessageBox.Show("Ne moze");
                return false;
            }
            else
            {
                MessageBox.Show("moze");
                return true;
            }
        }

        private void ExecuteMoveCommand(object parameter)
        {
            MessageBox.Show("moze");
        }

        private void UpdateAvailableMoves()
        {
            AvailableMoves.Clear();

            if (SelectedSquare?.Piece == null) return;
            var validMoves = GetValidMoves(SelectedSquare);
            foreach (var move in validMoves) AvailableMoves.Add(move);
        }

        private List<ChessSquare> GetValidMoves(ChessSquare chessSquare)
        {
            var retVal = new List<ChessSquare>();

            return retVal;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
