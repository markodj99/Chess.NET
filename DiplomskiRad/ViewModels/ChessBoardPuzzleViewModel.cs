using DiplomskiRad.Commands;
using DiplomskiRad.Helper;
using DiplomskiRad.Models.Enums;
using DiplomskiRad.Models.Game;
using DiplomskiRad.Models.Pieces;
using DiplomskiRad.Stores;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace DiplomskiRad.ViewModels
{
    public class ChessBoardPuzzleViewModel : ViewModelBase
    {
        #region Fields

        private readonly EngineStrengthEvaluatedStore _engineStrengthEvaluated;
        public FlipBoard FlipBoard { get; set; }
        public ObservableCollection<ChessSquare> ChessSquares { get; set; }

        public List<int> LastMove { get; set; }
        public List<int> HighlightedSquares { get; set; }

        public PuzzleManager PuzzleManager { get; set; }

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
        public ICommand ClickCommand { get; private set; }
        public ICommand MoveCommand { get; private set; }

        #endregion

        #region Initialization

        public ChessBoardPuzzleViewModel(EngineStrengthEvaluatedStore engineStrengthEvaluated)
        {
            FlipBoard = new FlipBoard();
            ChessSquares = SetUpBoard();

            ClickCommand = new Command(ExecuteClickCommand, CanExecuteClickCommand);
            MoveCommand = new Command(ExecuteMoveCommand, CanExecuteMoveCommand);

            _engineStrengthEvaluated = engineStrengthEvaluated;
        }

        private ObservableCollection<ChessSquare> SetUpBoard()
        {
            var chessSquares = new ObservableCollection<ChessSquare>();

            int index = 0;
            for (int row = 0; row < 8; row++)
            {
                for (int column = 0; column < 8; column++)
                {
                    var color = (row + column) % 2 == 0 ? "#CCCCCC" : "#3a9cce"; // oke boje za sad
                    chessSquares.Add(new ChessSquare(row, column, index, Mapping.IndexToCoordinate[index], null, color, null));
                    index++;
                }
            }

            return chessSquares;
        }

        public void Start()
        {
            PuzzleManager = new PuzzleManager();
            PuzzleManager.Initialize();

            HighlightedSquares = new List<int>();
            LastMove = new List<int>(2);
            SelectedSquare = null;

            PuzzleRush();
        }

        #endregion

        #region Commands

        private bool CanExecuteClickCommand(object parameter)
        {
            var selectedSquare = parameter as ChessSquare;

            if (selectedSquare.Piece != null)
            {
                if (selectedSquare.Piece.Color == PuzzleManager.GetCurrentPuzzle().FirstMove) return false;
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
                int index = SelectedSquare.Index;
                HighlightedSquares.Add(index);
                ChessSquares[index].Color = "Black";
                //ChessSquares.First(x => x is { Row: 2, Column: 0 }).ImagePath = ChessSquares.First(x => x is { Row: 0, Column: 0 }).ImagePath; test samo
            }
            else if (SelectedSquare.Equals(selectedSquare)) // ista izabrana 2 puta
            {
                foreach (var s in HighlightedSquares)
                {
                    ChessSquares[s].Color = (ChessSquares[s].Row + ChessSquares[s].Column) % 2 == 0 ? "#CCCCCC" : "#3a9cce";
                }
                HighlightedSquares.Clear();

                SelectedSquare = null;

                foreach (var move in LastMove)
                {
                    ChessSquares[move].Color = "Yellow";
                }
            }
        }

        private bool CanExecuteMoveCommand(object parameter)
        {
            var targetSquare = parameter as ChessSquare;
            return SelectedSquare != null && !SelectedSquare.Equals(targetSquare) && HighlightedSquares.Contains(targetSquare.Index);
        }

        private void ExecuteMoveCommand(object parameter)
        {
            var targetSquare = parameter as ChessSquare;

            ChessSquares[targetSquare.Index].Piece = SelectedSquare.Piece;
            ChessSquares[targetSquare.Index].ImagePath = SelectedSquare.ImagePath;

            if (ChessSquares[targetSquare.Index].Piece is Pawn) ((Pawn)ChessSquares[targetSquare.Index].Piece).IsFirstMove = false;
            if (ChessSquares[targetSquare.Index].Piece is King) ((King)ChessSquares[targetSquare.Index].Piece).CastlingRight = false;
            if (ChessSquares[targetSquare.Index].Piece is Rook) ((Rook)ChessSquares[targetSquare.Index].Piece).CastlingRight = false;

            ChessSquares[SelectedSquare.Index].Piece = null;
            ChessSquares[SelectedSquare.Index].ImagePath = null;

            foreach (var s in HighlightedSquares)
            {
                ChessSquares[s].Color = (ChessSquares[s].Row + ChessSquares[s].Column) % 2 == 0 ? "#CCCCCC" : "#3a9cce";
            }
            HighlightedSquares.Clear();

            foreach (var move in LastMove)
            {
                ChessSquares[move].Color = (ChessSquares[move].Row + ChessSquares[move].Column) % 2 == 0 ? "#CCCCCC" : "#3a9cce";
            }
            LastMove.Clear();

            LastMove.Add(targetSquare.Index);
            LastMove.Add(SelectedSquare.Index);

            SelectedSquare = null;

            foreach (var move in LastMove)
            {
                ChessSquares[move].Color = "Yellow";
            }

            NextMoveOrNextPuzzle(targetSquare.Index);
        }

        #endregion

        #region Puzzles

        private void PuzzleRush()
        {
            if (PuzzleManager.Condition())
            {
                //MessageBox.Show($"{PuzzleManager.Rating}");
                _engineStrengthEvaluated.EvaulatedRating(PuzzleManager.Rating);
                return;
            }

            var puzzle = PuzzleManager.GetCurrentPuzzle();
            SetUpPiecesPosition(puzzle.WhitePos, Color.White);
            SetUpPiecesPosition(puzzle.BlackPos, Color.Black);

            var firstMove = puzzle.MoveOrder[0].Split("-");
            int origin = Mapping.CoordinateToIndex[firstMove[0]];
            int destenation = Mapping.CoordinateToIndex[firstMove[1]];

            ChessSquares[destenation].Piece = ChessSquares[origin].Piece;
            ChessSquares[destenation].ImagePath = ChessSquares[origin].ImagePath;
            ChessSquares[origin].ImagePath = null;
            ChessSquares[origin].Piece = null;

            if (ChessSquares[destenation].Piece is Pawn) ((Pawn)ChessSquares[destenation].Piece).IsFirstMove = false;
            if (ChessSquares[destenation].Piece is King) ((King)ChessSquares[destenation].Piece).CastlingRight = false;
            if (ChessSquares[destenation].Piece is Rook) ((Rook)ChessSquares[destenation].Piece).CastlingRight = false;

            foreach (var move in LastMove)
            {
                ChessSquares[move].Color = (ChessSquares[move].Row + ChessSquares[move].Column) % 2 == 0 ? "#CCCCCC" : "#3a9cce";
            }
            LastMove.Clear();

            LastMove.Add(origin);
            LastMove.Add(destenation);
            foreach (var move in LastMove)
            {
                ChessSquares[move].Color = "Yellow";
            }
            
            PuzzleManager.OrdinalMoveNumber = 1;
            FlipBoard.Orientation = puzzle.FirstMove == Color.White ? Color.Black : Color.White;
        }

        private void SetUpPiecesPosition(List<string> positions, Color color)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var targetFolder = Path.Combine(currentDirectory, "..", "..", "..", "Images");

            var imageColor = color == Color.White ? "W" : "B";

            foreach (var pos in positions)
            {
                Piece? piece;
                switch (pos[0])
                {
                    case 'K':
                        bool castleKing = false;
                        if (color == Color.Black && Mapping.CoordinateToIndex[pos.Substring(1)] == 4) castleKing = true;
                        if (color == Color.White && Mapping.CoordinateToIndex[pos.Substring(1)] == 60) castleKing = true;
                        piece = new King(color, castleKing);
                        break;
                    case 'Q':
                        piece = new Queen(color);
                        break;
                    case 'R':
                        bool castleRook = false;
                        if (color == Color.Black && Mapping.CoordinateToIndex[pos.Substring(1)] is (0 or 7)) castleRook = true;
                        if (color == Color.White && Mapping.CoordinateToIndex[pos.Substring(1)] is (56 or 63)) castleRook = true;
                        piece = new Rook(color, castleRook); //popravi kasnije
                        break;
                    case 'B':
                        piece = new Bishop(color);
                        break;
                    case 'N':
                        piece = new Knight(color);
                        break;
                    case 'P':
                        bool isfirstMove = false;
                        if (color == Color.Black && Mapping.CoordinateToDoubleIndex[pos.Substring(1)].Key == 1) isfirstMove = true;
                        if (color == Color.White && Mapping.CoordinateToDoubleIndex[pos.Substring(1)].Key == 6) isfirstMove = true;
                        piece = new Pawn(color, isfirstMove);
                        break;
                    default:
                        piece = new Piece("ime", 4, color, PieceType.Bishop);
                        break;
                }

                ChessSquares[Mapping.CoordinateToIndex[pos.Substring(1)]].Piece = piece;
                ChessSquares[Mapping.CoordinateToIndex[pos.Substring(1)]].ImagePath = Path.Combine(targetFolder, $"{piece.Name}_{imageColor}.png");
            }
        }

        private void SetUpNextPuzzleMove(int index)
        {
            var puzzle = PuzzleManager.GetCurrentPuzzle();

            var previousMove = puzzle.MoveOrder[PuzzleManager.OrdinalMoveNumber].Split("-");
            if (Mapping.CoordinateToIndex[previousMove[1]] != index)
            {
                if (++PuzzleManager.ErrorCount == 3)
                {
                    //MessageBox.Show($"{PuzzleManager.Rating}");
                    _engineStrengthEvaluated.EvaulatedRating(PuzzleManager.Rating);
                    return;
                }

                CleanBoardForNextPuzzle();
                return;
            }

            var moves = puzzle.MoveOrder[PuzzleManager.OrdinalMoveNumber + 1].Split("-");
            int origin = Mapping.CoordinateToIndex[moves[0]];
            int destenation = Mapping.CoordinateToIndex[moves[1]];

            ChessSquares[destenation].Piece = ChessSquares[origin].Piece;
            ChessSquares[destenation].ImagePath = ChessSquares[origin].ImagePath;
            ChessSquares[origin].ImagePath = null;
            ChessSquares[origin].Piece = null;

            if (ChessSquares[destenation].Piece is Pawn) ((Pawn)ChessSquares[destenation].Piece).IsFirstMove = false;
            if (ChessSquares[destenation].Piece is King) ((King)ChessSquares[destenation].Piece).CastlingRight = false;
            if (ChessSquares[destenation].Piece is Rook) ((Rook)ChessSquares[destenation].Piece).CastlingRight = false;

            foreach (var move in LastMove)
            {
                ChessSquares[move].Color = (ChessSquares[move].Row + ChessSquares[move].Column) % 2 == 0 ? "#CCCCCC" : "#3a9cce";
            }
            LastMove.Clear();

            LastMove.Add(Mapping.CoordinateToIndex[moves[0]]);
            LastMove.Add(Mapping.CoordinateToIndex[moves[1]]);

            foreach (var move in LastMove)
            {
                ChessSquares[move].Color = "Yellow";
            }
            
            PuzzleManager.OrdinalMoveNumber += 2;
        }

        private void NextMoveOrNextPuzzle(int index)
        {
            var puzzle = PuzzleManager.GetCurrentPuzzle();
            if ((PuzzleManager.OrdinalMoveNumber + 1) >= puzzle.MoveOrder.Count) // poslednji potez
            {
                var previousMove = puzzle.MoveOrder[PuzzleManager.OrdinalMoveNumber].Split("-");
                if (Mapping.CoordinateToIndex[previousMove[1]] != index) // los poslednji potez
                {
                    if (++PuzzleManager.ErrorCount == 3)
                    {
                        //MessageBox.Show($"{PuzzleManager.Rating}");
                        _engineStrengthEvaluated.EvaulatedRating(PuzzleManager.Rating);
                        return;
                    }
                }
                else // dobar poslednji potez
                {
                    PuzzleManager.Rating += 50;
                }

                CleanBoardForNextPuzzle();
            }
            else
            {
                SetUpNextPuzzleMove(index);
            }
        }

        private void CleanBoardForNextPuzzle()
        {
            PuzzleManager.IncrementOrdinalNumber();
            foreach (var chessSquare in ChessSquares)
            {
                chessSquare.Piece = null;
                chessSquare.ImagePath = null;
            }
            PuzzleRush();
        }

        #endregion

        #region Private Methods

        private void UpdateAvailableMoves()
        {
            if (SelectedSquare?.Piece == null) return;
            var original = SelectedSquare.Piece.GetPossibleMoves(SelectedSquare, ChessSquares.ToList());
            var final = AreMovesValid(original);
            HighlightedSquares.AddRange(final);
            foreach (var t in HighlightedSquares)
            {
                ChessSquares[t].Color = "Black";
            }
        }

        private List<int> AreMovesValid(List<int> possibleMoves)
        {
            var retVal = new List<int>(possibleMoves);

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

                int kingPos = 100;
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

        #endregion
    }
}
