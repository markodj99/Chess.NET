using System;
using DiplomskiRad.Commands;
using DiplomskiRad.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using DiplomskiRad.Helper;

namespace DiplomskiRad.ViewModels
{
    public class ChessBoardViewModel : INotifyPropertyChanged
    {
        #region BooardSetUp

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

            ClickCommand = new Command(ExecuteClickCommand, CanExecuteClickCommand);
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
                        Color = (row + column) % 2 == 0 ? "#CCCCCC" : "#3a9cce" // oke boje za sad
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

        #endregion

        public ICommand ClickCommand { get; private set; }
        public ICommand MoveCommand { get; private set; }
        private ChessSquare _selectedSquare;
        public ChessSquare? SelectedSquare
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

        private bool CanExecuteClickCommand(object parameter)
        {
            var selectedSquare = parameter as ChessSquare;
            if (selectedSquare?.ImagePath == null) return false;
            return SelectedSquare is null || SelectedSquare.Equals(selectedSquare);
        }

        private void ExecuteClickCommand(object parameter)
        {
            var selectedSquare = parameter as ChessSquare;

            if (SelectedSquare is null) // nista nije izabrano
            {
                SelectedSquare = new ChessSquare(selectedSquare);
                ChessSquares.First(x => x.Row == SelectedSquare.Row && x.Column == SelectedSquare.Column).Color = "Black";
                //ChessSquares.First(x => x is { Row: 2, Column: 0 }).ImagePath = ChessSquares.First(x => x is { Row: 0, Column: 0 }).ImagePath; test samo
            }
            else if (SelectedSquare.Equals(selectedSquare)) // ista izabrana 2 puta
            {
                var color = (SelectedSquare.Row + SelectedSquare.Column) % 2 == 0 ? "#CCCCCC" : "#3a9cce";
                ChessSquares.First(x => x.Row == SelectedSquare.Row && x.Column == SelectedSquare.Column).Color = color;
                SelectedSquare = null;
            }
        }

        private bool CanExecuteMoveCommand(object parameter)
        {
            return false;
        }

        private void ExecuteMoveCommand(object parameter)
        {

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

            switch (chessSquare.Piece.Name)
            {
                case "King":
                    break;
                case "Queen":
                    break;
                case "Rook":
                    break;
                case "Bishop":
                    break;
                case "Knight":
                    break;
                case "Pawn":
                    HighlightPawnMoves(chessSquare, retVal);
                    break;
            }

            return retVal;
        }


        private void HighlightPawnMoves(ChessSquare chessSquare, List<ChessSquare> retVal)
        {
            var pos = Mapping.DoubleIndexToCoordinate[new KeyValuePair<int, int>(chessSquare.Row, chessSquare.Column)];
            int file = Convert.ToInt32(pos.Substring(1));
            //dodaj ono sto si zamislio can move zbog saha da li je vezan i ta sranja da li moze da uzme figuru i na kraju promocija bla bla

            if (chessSquare.Color.Equals("White"))
            {
                if (((Pawn)(chessSquare.Piece)).IsFirstMove)
                {
                    string firstSquare = $"{pos[0]}{file + 1}", secondSquare = $"{pos[0]}{file + 2}";
                }
                else
                {
                    string firstSquare = $"{pos[0]}{file + 1}";
                }
            }
            else
            {
                if (((Pawn)(chessSquare.Piece)).IsFirstMove)
                {
                    string firstSquare = $"{pos[0]}{file - 1}", secondSquare = $"{pos[0]}{file - 2}";
                }
                else
                {

                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
