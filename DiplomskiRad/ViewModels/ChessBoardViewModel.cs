using DiplomskiRad.Commands;
using DiplomskiRad.Models;
using DiplomskiRad.Models.Enums;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using DiplomskiRad.Helper;

namespace DiplomskiRad.ViewModels
{
    public class ChessBoardViewModel : ViewModelBase
    {
        public Color PlayerColor { get; set; }
        public int EngineStrength { get; set; }
        public FlipBoard FlipBoard {get; set; }

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
            FlipBoard = new FlipBoard();

            ClickCommand = new Command(ExecuteClickCommand, CanExecuteClickCommand);
            MoveCommand = new Command(ExecuteMoveCommand, CanExecuteMoveCommand);

            HighlightedSquares = new List<ushort>();
            ChessSquares = new ObservableCollection<ChessSquare>();
            Puzzles = new List<ChessPuzzle>();
        }

        public void BoardSetUp(RatingEvaluation evaluation)
        {
            if (evaluation == RatingEvaluation.UserSelected)
            {
                ChessSquares.Clear();
                ChessSquares = SetUpBoard();
                if (PlayerColor == Color.Black) FlipBoard.Flip();
            }
            else // puzzles
            {
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

                        chessSquares.Add(square);
                    }
                }
                ChessSquares = chessSquares;

                Puzzles = Parser.ParseFile();

                PuzzleRush(1);
            }
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
                        var color = row switch
                        {
                            0 or 1 => Color.Black,
                            6 or 7 => Color.White,
                            _ => Color.White,
                        };
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

            return chessSquares;
        }

        #endregion

        #region Puzzles

        public List<ChessPuzzle> Puzzles { get; set; }

        private void PuzzleRush(int ordinalNumber)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var targetFolder = Path.Combine(currentDirectory, "..", "..", "..", "Images");
            foreach (var wPos in Puzzles[ordinalNumber].WhitePos)
            {
                var coordinates = Mapping.CoordinateToDoubleIndex[wPos.Substring(1)];
                var piece = wPos[0] switch
                {
                    'K' => new King(Color.White, coordinates.Key, coordinates.Value),
                    'Q' => new Queen(Color.White, coordinates.Key, coordinates.Value),
                    'R' => new Rook(Color.White, coordinates.Key, coordinates.Value),
                    'B' => new Bishop(Color.White, coordinates.Key, coordinates.Value),
                    'N' => new Knight(Color.White, coordinates.Key, coordinates.Value),
                    'P' => new Pawn(Color.White, coordinates.Key, coordinates.Value),
                    _ => new Piece("ime", 4, Color.Black, PieceType.Bishop, 0, 0),
                };
                ChessSquares[Mapping.CoordinateToIndex[wPos.Substring(1)]].Piece = piece;
                ChessSquares[Mapping.CoordinateToIndex[wPos.Substring(1)]].ImagePath = Path.Combine(targetFolder, $"{piece.Name}_W.png");
            }

            foreach (var bPos in Puzzles[ordinalNumber].BlackPos)
            {
                var coordinates = Mapping.CoordinateToDoubleIndex[bPos.Substring(1)];
                var piece = bPos[0] switch
                {
                    'K' => new King(Color.Black, coordinates.Key, coordinates.Value),
                    'Q' => new Queen(Color.Black, coordinates.Key, coordinates.Value),
                    'R' => new Rook(Color.Black, coordinates.Key, coordinates.Value),
                    'B' => new Bishop(Color.Black, coordinates.Key, coordinates.Value),
                    'N' => new Knight(Color.Black, coordinates.Key, coordinates.Value),
                    'P' => new Pawn(Color.Black, coordinates.Key, coordinates.Value),
                    _ => new Piece("ime", 4, Color.Black, PieceType.Bishop, 0, 0),
                };
                ChessSquares[Mapping.CoordinateToIndex[bPos.Substring(1)]].Piece = piece;
                ChessSquares[Mapping.CoordinateToIndex[bPos.Substring(1)]].ImagePath = Path.Combine(targetFolder, $"{piece.Name}_B.png");
            }

            if (Puzzles[ordinalNumber].FirstMove == Color.White) FlipBoard.Flip();
        }

        private void SetUpPuzzle()
        {

        }

        #endregion




        #region MovingLogic

        public ICommand ClickCommand { get; private set; }
        public ICommand MoveCommand { get; private set; }
        private List<ushort> HighlightedSquares { get; set; }

        private ChessSquare? _selectedSquare;
        public ChessSquare? SelectedSquare
        {
            get => _selectedSquare;
            set
            {
                _selectedSquare = value;
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
            HighlightedSquares.AddRange(SelectedSquare.Piece.GetPossibleMoves(SelectedSquare, ChessSquares.ToList()));
            foreach (var t in HighlightedSquares) ChessSquares[t].Color = "Black";
        }

        #endregion
    }
}
