using DiplomskiRad.Commands;
using DiplomskiRad.Engine;
using DiplomskiRad.Helper;
using DiplomskiRad.Models.Enums;
using DiplomskiRad.Models.Game;
using DiplomskiRad.Models.Pieces;
using DiplomskiRad.Views;
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
        #region Fields

        public Color PlayerColor { get; set; }
        public int EngineStrength { get; set; }
        public FlipBoard FlipBoard { get; set; }
        
        public ObservableCollection<ChessSquare> ChessSquares { get; set; }
        public StockfishManager StockfishManager { get; set; }

        public ICommand ClickCommand { get; private set; }
        public ICommand MoveCommand { get; private set; }

        private List<int> HighlightedSquares { get; set; }
        public List<int> LastMove { get; set; }

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

        public bool EnPassantPossibilty { get; set; }
        public int EnPassantSquare { get; set; }

        #endregion

        #region Initialization

        public ChessBoardGameViewModel()
        {
            FlipBoard = new FlipBoard();

            ClickCommand = new Command(ExecuteClickCommand, CanExecuteClickCommand);
            MoveCommand = new Command(ExecuteMoveCommand, CanExecuteMoveCommand);
        }

        public void Start()
        {
            ChessSquares = SetUpBoard();

            StockfishManager = new StockfishManager(EngineStrength);

            HighlightedSquares = new List<int>();
            LastMove = new List<int>(2);

            SelectedSquare = null;

            EnPassantPossibilty = false;
            EnPassantSquare = -1;

            if (PlayerColor == Color.Black) // posle za engine
            {
                FlipBoard.Orientation = Color.Black;

                var firstmove = StockfishManager.GetBestMove();

                int start = Mapping.CoordinateToIndex[firstmove.Substring(0, 2)];
                int end = Mapping.CoordinateToIndex[firstmove.Substring(2)];

                ChessSquares[end].Piece = ChessSquares[start].Piece;
                ChessSquares[end].ImagePath = ChessSquares[start].ImagePath;

                ChessSquares[start].Piece = null;
                ChessSquares[start].ImagePath = null;

                if (ChessSquares[end].Piece is Pawn) ((Pawn)(ChessSquares[end].Piece)).IsFirstMove = false;
                if (ChessSquares[end].Piece is King) ((King)(ChessSquares[end].Piece)).CastlingRight = false;
                if (ChessSquares[end].Piece is Rook) ((Rook)(ChessSquares[end].Piece)).CastlingRight = false;
            }
        }

        private ObservableCollection<ChessSquare> SetUpBoard()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var targetFolder = Path.Combine(currentDirectory, "..", "..", "..", "Images");

            var chessSquares = new ObservableCollection<ChessSquare>();
            int index = 0;
            for (int row = 0; row < 8; row++)
            {
                for (int column = 0; column < 8; column++)
                {
                    Piece piece = null;
                    string imagePath = null;

                    var c = row is not (0 or 1) ? "W" : "B";
                    var pieceName = Mapping.InitialPieceOrder[row, column];
                    if (!string.IsNullOrEmpty(pieceName))
                    {
                        var pieceColor = row switch
                        {
                            0 or 1 => Color.Black,
                            6 or 7 => Color.White,
                            _ => Color.White,
                        };
                        piece = pieceName switch
                        {
                            "Pawn" => new Pawn(pieceColor, true),
                            "Rook" => new Rook(pieceColor, true),
                            "Knight" => new Knight(pieceColor),
                            "Bishop" => new Bishop(pieceColor),
                            "Queen" => new Queen(pieceColor),
                            _ => new King(pieceColor, true)
                        };
                        imagePath = Path.Combine(targetFolder, $"{pieceName}_{c}.png");
                    }

                    var color = (row + column) % 2 == 0 ? "#CCCCCC" : "#3a9cce"; // oke boje za sad
                    chessSquares.Add(new ChessSquare(row, column, index, Mapping.IndexToCoordinate[index], piece, color, imagePath));
                    index++;
                }
            }

            return chessSquares;
        }

        #endregion

        #region Commands

        private bool CanExecuteClickCommand(object parameter)
        {
            var selectedSquare = parameter as ChessSquare;

            if (selectedSquare.Piece != null)
            {
                if (selectedSquare.Piece.Color != PlayerColor) return false;
            }

            if (selectedSquare?.Piece == null) return false;
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
                ChessSquares[index].Color = "Black"; // boje
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
            if (SelectedSquare != null && !SelectedSquare.Equals(targetSquare) && HighlightedSquares.Contains(targetSquare.Index)) return true;
            return false;
        }

        private void ExecuteMoveCommand(object parameter)
        {
            var targetSquare = parameter as ChessSquare;
            int selectedColumn = SelectedSquare.Column;
            int targetRow = targetSquare.Row, targetColumn = targetSquare.Column;

            if (SelectedSquare.Piece is King k && Math.Abs(targetColumn - selectedColumn) > 1) // rokada
            {
                k.CastlingRight = false;
                EnPassantPossibilty = false;
                CastlingMove(targetSquare);
            }
            else if (SelectedSquare.Piece is Pawn && targetRow is (0 or 7)) // promocija
            {
                EnPassantPossibilty = false;
                PromotionMove(targetSquare);
            }
            else
            {
                OrdinaryMove(targetSquare);
            }

            HighlightSquares(targetSquare);

            string a = Mapping.IndexToCoordinate[LastMove[1]];
            string b = Mapping.IndexToCoordinate[LastMove[0]];

            string x = a + b + " ";

            var movex = StockfishManager.GetBestMove(x);

            int start = Mapping.CoordinateToIndex[movex.Substring(0, 2)];
            int end = Mapping.CoordinateToIndex[movex.Substring(2)];

            ChessSquares[end].Piece = ChessSquares[start].Piece;
            ChessSquares[end].ImagePath = ChessSquares[start].ImagePath;

            ChessSquares[start].Piece = null;
            ChessSquares[start].ImagePath = null;

            if (ChessSquares[end].Piece is Pawn) ((Pawn)(ChessSquares[end].Piece)).IsFirstMove = false;
            if (ChessSquares[end].Piece is King) ((King)(ChessSquares[end].Piece)).CastlingRight = false;
            if (ChessSquares[end].Piece is Rook) ((Rook)(ChessSquares[end].Piece)).CastlingRight = false;
        }

        private void CastlingMove(ChessSquare targetSquare)
        {
            int selectedRow = SelectedSquare.Row;
            int targetColumn = targetSquare.Column;

            int origin = -1, destenation = -1;

            if (targetColumn == 6) // mala rokada
            {
                origin = 7;
                destenation = 5;
            }
            else // velika rokada (targetColumn == 2)
            {
                origin = 0;
                destenation = 3;
            }

            ChessSquares[targetSquare.Index].Piece = SelectedSquare.Piece;
            ChessSquares[targetSquare.Index].ImagePath = SelectedSquare.ImagePath;

            ChessSquares[SelectedSquare.Index].Piece = null;
            ChessSquares[SelectedSquare.Index].ImagePath = null;

            int start = Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(selectedRow, origin)];
            int finish = Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(selectedRow, destenation)];

            ChessSquares[finish].Piece = ChessSquares[start].Piece;
            ChessSquares[finish].ImagePath = ChessSquares[start].ImagePath;

            ChessSquares[start].Piece = null;
            ChessSquares[start].ImagePath = null;

            ((Rook)ChessSquares[finish].Piece).CastlingRight = false;
        }

        private void PromotionMove(ChessSquare targetSquare)
        {
            var promotionView = new PromotionWindowView();
            var promotionVM = new PromotionWindowViewModel(promotionView);
            promotionView.DataContext = promotionVM;

            string imageColor = "";

            if (targetSquare.Row == 0) // beli
            {
                promotionVM.SetUpPromotionPieces(Color.White);
                imageColor = "W";
            }
            else if (targetSquare.Row == 7) // crni
            {
                promotionVM.SetUpPromotionPieces(Color.Black);
                imageColor = "B";
            }

            promotionView.ShowDialog();

            var currentDirectory = Directory.GetCurrentDirectory();
            var targetFolder = Path.Combine(currentDirectory, "..", "..", "..", "Images");

            targetSquare.Piece = promotionVM.GetPiece();
            targetSquare.ImagePath = Path.Combine(targetFolder, $"{targetSquare.Piece.Name}_{imageColor}.png");

            ChessSquares[SelectedSquare.Index].Piece = null;
            ChessSquares[SelectedSquare.Index].ImagePath = null;
        }

        private void OrdinaryMove(ChessSquare targetSquare)
        {
            ChessSquares[targetSquare.Index].Piece = SelectedSquare.Piece;
            ChessSquares[targetSquare.Index].ImagePath = SelectedSquare.ImagePath;

            if (ChessSquares[targetSquare.Index].Piece is Pawn) ((Pawn)(ChessSquares[targetSquare.Index].Piece)).IsFirstMove = false;
            if (ChessSquares[targetSquare.Index].Piece is King) ((King)(ChessSquares[targetSquare.Index].Piece)).CastlingRight = false;
            if (ChessSquares[targetSquare.Index].Piece is Rook) ((Rook)(ChessSquares[targetSquare.Index].Piece)).CastlingRight = false;

            int origin = SelectedSquare.Row;
            int target = targetSquare.Row;

            if (ChessSquares[targetSquare.Index].Piece is Pawn && Math.Abs(target - origin) == 2)
            {
                EnPassantPossibilty = true;
                EnPassantSquare = Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(targetSquare.Row, targetSquare.Column)];
            }
            else
            {
                EnPassantPossibilty = false;
            }

            ChessSquares[SelectedSquare.Index].Piece = null;
            ChessSquares[SelectedSquare.Index].ImagePath = null;
        }

        private void HighlightSquares(ChessSquare targetSquare)
        {
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
        }

        #endregion

        #region Private Methods

        private void UpdateAvailableMoves()
        {
            if (SelectedSquare?.Piece == null) return;

            List<int> original = EnPassantPossibilty ? SelectedSquare.Piece.GetPossibleMoves(SelectedSquare, ChessSquares.ToList(), EnPassantSquare) : SelectedSquare.Piece.GetPossibleMoves(SelectedSquare, ChessSquares.ToList());
            var final = AreMovesValid(original);
            HighlightedSquares.AddRange(final);
            foreach (var t in HighlightedSquares) ChessSquares[t].Color = "Black";
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
