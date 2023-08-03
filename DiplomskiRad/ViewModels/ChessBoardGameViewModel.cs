using DiplomskiRad.Commands;
using DiplomskiRad.Helper;
using DiplomskiRad.Models.Enums;
using DiplomskiRad.Models.Game;
using DiplomskiRad.Models.Pieces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace DiplomskiRad.ViewModels
{
    public class ChessBoardGameViewModel : ViewModelBase
    {
        public Color PlayerColor { get; set; }
        public int EngineStrength { get; set; }
        public FlipBoard FlipBoard {get; set; }
        public List<ushort> LastMove { get; set; } = new List<ushort>(2);

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

        public ChessBoardGameViewModel()
        {
            FlipBoard = new FlipBoard();

            ClickCommand = new Command(ExecuteClickCommand, CanExecuteClickCommand);
            MoveCommand = new Command(ExecuteMoveCommand, CanExecuteMoveCommand);

            HighlightedSquares = new List<ushort>();
            ChessSquares = new ObservableCollection<ChessSquare>();
        }

        public void Start()
        {
            ChessSquares = SetUpBoard();
            if (PlayerColor == Color.Black) FlipBoard.Flip();
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
                            "Pawn" => new Pawn(color),
                            "Rook" => new Rook(color),
                            "Knight" => new Knight(color),
                            "Bishop" => new Bishop(color),
                            "Queen" => new Queen(color),
                            _ => new King(color)
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

        public int OrdinalNumber { get; set; } = 0;

        private bool CanExecuteClickCommand(object parameter)
        {
            var selectedSquare = parameter as ChessSquare;

            if (selectedSquare.Piece != null)
            {
                if (selectedSquare.Piece.Color != PlayerColor)
                {
                    return false;
                }
            }

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
                foreach (var move in LastMove)
                {
                    ChessSquares[move].Color = "Yellow";
                }
            }
        }

        public ushort SelectedMove { get; set; } = 0;

        private bool CanExecuteMoveCommand(object parameter)
        {
            var selectedSquare = parameter as ChessSquare;
            SelectedMove = Mapping.DoubleIndexToIndex[
                new KeyValuePair<int, int>(selectedSquare.Row, selectedSquare.Column)];
            if (SelectedSquare != null && !SelectedSquare.Equals(selectedSquare) && HighlightedSquares.Contains(SelectedMove)) return true;

            return false;
        }

        public int MoveOrder { get; set; } = 0;

        private void ExecuteMoveCommand(object parameter)
        {
            var selectedSquare = parameter as ChessSquare;


            ChessSquares[SelectedMove].Piece = SelectedSquare.Piece;
            ChessSquares[SelectedMove].ImagePath = SelectedSquare.ImagePath;

            ChessSquares[
                    Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(SelectedSquare.Row, SelectedSquare.Column)]]
                .Piece = null;
            ChessSquares[
                    Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(SelectedSquare.Row, SelectedSquare.Column)]]
                .ImagePath = null;

            foreach (var s in HighlightedSquares) ChessSquares[s].Color = (ChessSquares[s].Row + ChessSquares[s].Column) % 2 == 0 ? "#CCCCCC" : "#3a9cce";
            HighlightedSquares.Clear();

            foreach (var move in LastMove)
            {
                ChessSquares[move].Color = (ChessSquares[move].Row + ChessSquares[move].Column) % 2 == 0 ? "#CCCCCC" : "#3a9cce";
            }
            LastMove.Clear();

            LastMove.Add(Mapping.DoubleIndexToIndex[
                new KeyValuePair<int, int>(selectedSquare.Row, selectedSquare.Column)]);
            LastMove.Add(Mapping.DoubleIndexToIndex[
                new KeyValuePair<int, int>(SelectedSquare.Row, SelectedSquare.Column)]);

            SelectedSquare = null;

            foreach (var move in LastMove)
            {
                ChessSquares[move].Color = "Yellow";
            }
        }

        private void UpdateAvailableMoves()
        {
            if (SelectedSquare?.Piece == null) return;
            var a = SelectedSquare.Piece.GetPossibleMoves(SelectedSquare, ChessSquares.ToList());
            var b = AreMovesValid(a);
            HighlightedSquares.AddRange(b);
            foreach (var t in HighlightedSquares) ChessSquares[t].Color = "Black";
        }

        #endregion

        private List<ushort> AreMovesValid(List<ushort> possibleMoves)
        {
            var retVal = new List<ushort>(possibleMoves);

            var initialPiecePosition = Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(SelectedSquare.Row, SelectedSquare.Column)];
            foreach (var move in possibleMoves)
            {
                var boardCopy = new List<ChessSquare>(ChessSquares.Count);

                foreach (var square in ChessSquares)
                {
                    boardCopy.Add(new ChessSquare(square));
                }

                boardCopy[move].Piece = boardCopy[initialPiecePosition].Piece;
                boardCopy[initialPiecePosition].Piece = null; // zamena pozicija

                ushort kingPos = 100;
                foreach (var square in boardCopy)
                {
                    if (square.Piece != null)
                    {
                        if (square.Piece.Type == PieceType.King && square.Piece.Color == SelectedSquare.Piece.Color)
                        {
                            kingPos = Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(square.Row, square.Column)];
                            break;
                        }
                    }
                }

                foreach (var square in boardCopy)
                {
                    if (square.Piece != null)
                    {
                        if (square.Piece.Color != SelectedSquare.Piece.Color)
                        {
                            var pieceMoves = square.Piece.GetPossibleMoves(square, boardCopy);
                            if (pieceMoves.Contains(kingPos))
                            {
                                retVal.Remove(move);
                            }
                        }
                    }
                }

                boardCopy[initialPiecePosition].Piece = boardCopy[move].Piece;
                boardCopy[move].Piece = null; // vracanje pozicija
            }

            return retVal;
        }
    }
}
