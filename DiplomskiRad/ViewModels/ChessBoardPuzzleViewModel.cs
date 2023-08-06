using DiplomskiRad.Commands;
using DiplomskiRad.Helper;
using DiplomskiRad.Models.Enums;
using DiplomskiRad.Models.Game;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using DiplomskiRad.Models.Pieces;

namespace DiplomskiRad.ViewModels
{
    public class ChessBoardPuzzleViewModel : ViewModelBase
    {
        public FlipBoard FlipBoard { get; set; }
        public List<ushort> LastMove { get; set; }
        public List<ushort> HighlightedSquares { get; set; }
        public ObservableCollection<ChessSquare> ChessSquares { get; set; }

        public List<ChessPuzzle> Puzzles { get; set; }
        public int OrdinalNumber { get; set; }
        public int OrdinalMoveNumber { get; set; }

        public ChessBoardPuzzleViewModel()
        {
            FlipBoard = new FlipBoard();
            LastMove = new List<ushort>(2);
            ChessSquares = SetUpBoard();

            ClickCommand = new Command(ExecuteClickCommand, CanExecuteClickCommand);
            MoveCommand = new Command(ExecuteMoveCommand, CanExecuteMoveCommand);

            HighlightedSquares = new List<ushort>();
            Puzzles = new List<ChessPuzzle>();
            OrdinalNumber = 0;
            OrdinalMoveNumber = 0;
        }

        private ObservableCollection<ChessSquare> SetUpBoard()
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
                        Color = (row + column) % 2 == 0 ? "#CCCCCC" : "#3a9cce", // oke boje za sad
                        Piece = null,
                        ImagePath = null
                    };

                    chessSquares.Add(square);
                }
            }

            return chessSquares;
        }

        public void Start()
        {
            Puzzles = Parser.ParseFile();

            OrdinalNumber = 0;
            PuzzleRush();
        }

        #region Commands

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

        public ushort SelectedMove { get; set; } = 0;
        public ICommand ClickCommand { get; private set; }
        public ICommand MoveCommand { get; private set; }

        private bool CanExecuteClickCommand(object parameter)
        {
            var selectedSquare = parameter as ChessSquare;
            if (selectedSquare.Piece != null)
                if (selectedSquare.Piece.Color == Puzzles[OrdinalNumber].FirstMove) return false;
            
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

                foreach (var move in LastMove) ChessSquares[move].Color = "Yellow";
            }
        }

        private bool CanExecuteMoveCommand(object parameter)
        {
            var selectedSquare = parameter as ChessSquare;
            SelectedMove = Mapping.DoubleIndexToIndex[
                new KeyValuePair<int, int>(selectedSquare.Row, selectedSquare.Column)];
            return SelectedSquare != null && !SelectedSquare.Equals(selectedSquare) && HighlightedSquares.Contains(SelectedMove);
        }

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

            foreach (var move in LastMove) ChessSquares[move].Color = (ChessSquares[move].Row + ChessSquares[move].Column) % 2 == 0 ? "#CCCCCC" : "#3a9cce";
            LastMove.Clear();

            LastMove.Add(Mapping.DoubleIndexToIndex[
                new KeyValuePair<int, int>(SelectedSquare.Row, SelectedSquare.Column)]);
            LastMove.Add(Mapping.DoubleIndexToIndex[
                new KeyValuePair<int, int>(SelectedSquare.Row, SelectedSquare.Column)]);
            SelectedSquare = null;
            foreach (var move in LastMove) ChessSquares[move].Color = "Yellow";

             NextMoveOrNextPuzzle();
        }

        private void UpdateAvailableMoves()
        {
            if (SelectedSquare?.Piece == null) return;
            var a = SelectedSquare.Piece.GetPossibleMoves(SelectedSquare, ChessSquares.ToList());
            var b = AreMovesValid(a);
            HighlightedSquares.AddRange(b);
            foreach (var t in HighlightedSquares) ChessSquares[t].Color = "Black";
        }

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

        #endregion

        #region Puzzles

        private void PuzzleRush()
        {
            SetUpPiecesPosition(Puzzles[OrdinalNumber].WhitePos, Color.White);
            SetUpPiecesPosition(Puzzles[OrdinalNumber].BlackPos, Color.Black);

            var firstMove = Puzzles[OrdinalNumber].MoveOrder[0].Split("-");
            ChessSquares[Mapping.CoordinateToIndex[firstMove[1]]].Piece = ChessSquares[Mapping.CoordinateToIndex[firstMove[0]]].Piece;
            ChessSquares[Mapping.CoordinateToIndex[firstMove[1]]].ImagePath = ChessSquares[Mapping.CoordinateToIndex[firstMove[0]]].ImagePath;
            ChessSquares[Mapping.CoordinateToIndex[firstMove[0]]].ImagePath = null;
            ChessSquares[Mapping.CoordinateToIndex[firstMove[0]]].Piece = null;


            foreach (var move in LastMove) ChessSquares[move].Color = (ChessSquares[move].Row + ChessSquares[move].Column) % 2 == 0 ? "#CCCCCC" : "#3a9cce";
            LastMove.Clear();

            LastMove.Add(Mapping.CoordinateToIndex[firstMove[0]]);
            LastMove.Add(Mapping.CoordinateToIndex[firstMove[1]]);
            foreach (var move in LastMove) ChessSquares[move].Color = "Yellow";
            
            OrdinalMoveNumber = 1;

            FlipBoard.Orientation = Puzzles[OrdinalNumber].FirstMove == Color.White ? Color.Black : Color.White;
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
                        piece = new King(color);
                        break;
                    case 'Q':
                        piece = new Queen(color);
                        break;
                    case 'R':
                        piece = new Rook(color);
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

        private void SetUpNextPuzzleMove()
        {
            var previousMove = Puzzles[OrdinalNumber].MoveOrder[OrdinalMoveNumber].Split("-");
            if (Mapping.CoordinateToIndex[previousMove[1]] != SelectedMove)
            {
                // nije dobar potez izabrao
                return;
            }

            var moves = Puzzles[OrdinalNumber].MoveOrder[OrdinalMoveNumber + 1].Split("-");

            ChessSquares[Mapping.CoordinateToIndex[moves[1]]].Piece = ChessSquares[Mapping.CoordinateToIndex[moves[0]]].Piece;
            ChessSquares[Mapping.CoordinateToIndex[moves[1]]].ImagePath = ChessSquares[Mapping.CoordinateToIndex[moves[0]]].ImagePath;
            ChessSquares[Mapping.CoordinateToIndex[moves[0]]].ImagePath = null;
            ChessSquares[Mapping.CoordinateToIndex[moves[0]]].Piece = null;

            foreach (var move in LastMove) ChessSquares[move].Color = (ChessSquares[move].Row + ChessSquares[move].Column) % 2 == 0 ? "#CCCCCC" : "#3a9cce";
            LastMove.Clear();

            LastMove.Add(Mapping.CoordinateToIndex[moves[0]]);
            LastMove.Add(Mapping.CoordinateToIndex[moves[1]]);

            foreach (var move in LastMove) ChessSquares[move].Color = "Yellow";
            
            OrdinalMoveNumber += 2;
        }

        private void NextMoveOrNextPuzzle()
        {
            if ((OrdinalMoveNumber + 1) >= Puzzles[OrdinalNumber].MoveOrder.Count)
            {
                //next puzzle
                OrdinalNumber++;
                foreach (var chessSquare in ChessSquares)
                {
                    chessSquare.Piece = null;
                    chessSquare.ImagePath = null;
                }
                PuzzleRush();
            }
            else
            {
                SetUpNextPuzzleMove();
            }
        }

        #endregion
    }
}
