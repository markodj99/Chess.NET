using DiplomskiRad.Commands;
using DiplomskiRad.Models;
using DiplomskiRad.Models.Enums;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

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

            ClickCommand = new Command(ExecuteClickCommand, CanExecuteClickCommand);
            MoveCommand = new Command(ExecuteMoveCommand, CanExecuteMoveCommand);

            HighlightedSquares = new List<ushort>();
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
                        Color color;
                        switch (row)
                        {
                            case 0 or 1:
                                color = Color.Black;
                                break;
                            case 6 or 7:
                                color = Color.White;
                                break;
                            default:
                                color = Color.White;
                                break;
                        }

                        Piece piece = pieceName switch
                        {
                            "Pawn" => new Pawn(color, row, column),
                            "Rook" => new Rook(color, row, column),
                            "Knight" => new Knight(color, row, column),
                            "Bishop" => new Bishop(color, row, column),
                            "Queen" => new Queen(color, row, column),
                            _ => new King(color, row, column)
                        };

                        var imagePath = Path.Combine(targetFolder, $"{pieceName}_{c}.png");
                        square.Piece = piece;
                        square.ImagePath = imagePath;
                    }
                    else square.ImagePath = null;

                    chessSquares.Add(square);
                }
            }

            //test
            //chessSquares[35] = new ChessSquare
            //{
            //    Row = 4,
            //    Column = 3,
            //    Color = "#3a9cce",
            //    Piece = new Pawn(Color.White, 4, 3),
            //    ImagePath = Path.Combine(targetFolder, "Pawn_W.png")
            //};
            //((Pawn)chessSquares[35].Piece).IsFirstMove = false;
            //chessSquares[36] = new ChessSquare
            //{
            //    Row = 4,
            //    Column = 4,
            //    Color = "#CCCCCC",
            //    Piece = new Pawn(Color.Black, 4, 4),
            //    ImagePath = Path.Combine(targetFolder, "Pawn_B.png")
            //};
            //((Pawn)chessSquares[36].Piece).IsFirstMove = false;
            //chessSquares[43] = new ChessSquare
            //{
            //    Row = 5,
            //    Column = 3,
            //    Color = "#3a9cce",
            //    Piece = new Queen(Color.White, 2, 3),
            //    ImagePath = Path.Combine(targetFolder, "Queen_W.png")
            //};
            chessSquares[27] = new ChessSquare
            {
                Row = 3,
                Column = 3,
                Color = "#CCCCCC",
                Piece = new Knight(Color.Black, 3, 3),
                ImagePath = Path.Combine(targetFolder, "Knight_B.png")
            };
            chessSquares[36] = new ChessSquare
            {
                Row = 4,
                Column = 4,
                Color = "#CCCCCC",
                Piece = new Knight(Color.Black, 4, 4),
                ImagePath = Path.Combine(targetFolder, "Knight_B.png")
            };

            chessSquares[28] = new ChessSquare
            {
                Row = 3,
                Column = 4,
                Color = "#3a9cce",
                Piece = new Knight(Color.White, 3, 4),
                ImagePath = Path.Combine(targetFolder, "Knight_W.png")
            };
            chessSquares[35] = new ChessSquare
            {
                Row = 4,
                Column = 3,
                Color = "#3a9cce",
                Piece = new Knight(Color.White, 4, 3),
                ImagePath = Path.Combine(targetFolder, "Knight_W.png")
            };

            return chessSquares;
        }

        #endregion

        public ICommand ClickCommand { get; private set; }
        public ICommand MoveCommand { get; private set; }
        private ChessSquare _selectedSquare;
        private List<ushort> HighlightedSquares { get; set; }

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
                int index = ChessSquares.ToList()
                    .FindIndex(x => x.Row == SelectedSquare.Row && x.Column == SelectedSquare.Column);
                HighlightedSquares.Add((ushort)index);
                ChessSquares[index].Color = "Black";
                //ChessSquares.First(x => x is { Row: 2, Column: 0 }).ImagePath = ChessSquares.First(x => x is { Row: 0, Column: 0 }).ImagePath; test samo
            }
            else if (SelectedSquare.Equals(selectedSquare)) // ista izabrana 2 puta
            {
                foreach (var s in HighlightedSquares) ChessSquares[s].Color = (ChessSquares[s].Row + ChessSquares[s].Column) % 2 == 0 ? "#CCCCCC" : "#3a9cce";
                HighlightedSquares.Clear();
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
            if (SelectedSquare?.Piece == null) return;
            HighlightedSquares.AddRange(GetValidMoves(SelectedSquare));

            foreach (var t in HighlightedSquares) ChessSquares[t].Color = "Black";
        }

        private List<ushort> GetValidMoves(ChessSquare chessSquare)
        {

            switch (chessSquare.Piece.Type)
            {
                case PieceType.King:
                    break;
                case PieceType.Queen:
                    break;
                case PieceType.Rook:
                    break;
                case PieceType.Bishop:
                    break;
                case PieceType.Knight:
                    return chessSquare.Piece.GetPossibleMoves(chessSquare, ChessSquares.ToList());
                    break;
                case PieceType.Pawn:
                    return chessSquare.Piece.GetPossibleMoves(chessSquare, ChessSquares.ToList());
            }

            return new List<ushort>();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
