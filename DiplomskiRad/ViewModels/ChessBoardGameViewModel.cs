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
using System.Threading.Tasks;
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

        public async Task Start()
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

                await GetEngineMoveAsync(string.Empty);
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

        private async void ExecuteMoveCommand(object parameter)
        {
            var targetSquare = parameter as ChessSquare;
            int selectedColumn = SelectedSquare.Column;
            int targetRow = targetSquare.Row, targetColumn = targetSquare.Column;

            int start = SelectedSquare.Index, end = targetSquare.Index;
            bool promotion = false;
            string promotionPiece = "k";

            if (SelectedSquare.Piece is King && Math.Abs(targetColumn - selectedColumn) > 1) // rokada
            {
                CastlingMove(targetSquare, SelectedSquare);
            }
            else if (SelectedSquare.Piece is Pawn && targetRow is (0 or 7)) // promocija
            {
                promotion = true;
                promotionPiece = PromotionMove(targetSquare).ToLower();
            }
            else
            {
                OrdinaryMove(targetSquare);
            }

            EnPassantPossibilty = false;

            HighlightSquares(start, end);

            string a = Mapping.IndexToCoordinate[LastMove[0]];
            string b = Mapping.IndexToCoordinate[LastMove[1]];
            string playerMove = a + b;
            playerMove += promotion ? $"{promotionPiece} " : " ";

            await GetEngineMoveAsync(playerMove);
        }

        #endregion

        #region Private Methods

        private void UpdateAvailableMoves()
        {
            if (SelectedSquare?.Piece == null) return;
            var original = EnPassantPossibilty ? SelectedSquare.Piece.GetPossibleMoves(SelectedSquare, ChessSquares.ToList(), EnPassantSquare) : SelectedSquare.Piece.GetPossibleMoves(SelectedSquare, ChessSquares.ToList());
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

        private void CastlingMove(ChessSquare targetSquare, ChessSquare selectedSquare)
        {
            int selectedRow = selectedSquare.Row;
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

            ChessSquares[targetSquare.Index].Piece = selectedSquare.Piece;
            ChessSquares[targetSquare.Index].ImagePath = selectedSquare.ImagePath;

            ChessSquares[selectedSquare.Index].Piece = null;
            ChessSquares[selectedSquare.Index].ImagePath = null;

            int start = Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(selectedRow, origin)];
            int finish = Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(selectedRow, destenation)];

            ChessSquares[finish].Piece = ChessSquares[start].Piece;
            ChessSquares[finish].ImagePath = ChessSquares[start].ImagePath;

            ChessSquares[start].Piece = null;
            ChessSquares[start].ImagePath = null;

            ((King)ChessSquares[targetSquare.Index].Piece).CastlingRight = false;
            ((Rook)ChessSquares[finish].Piece).CastlingRight = false;
        }

        private string PromotionMove(ChessSquare targetSquare)
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

            return targetSquare.Piece.Name.Substring(0, 1);
        }

        private void OrdinaryMove(ChessSquare targetSquare)
        {
            ChessSquares[targetSquare.Index].Piece = SelectedSquare.Piece;
            ChessSquares[targetSquare.Index].ImagePath = SelectedSquare.ImagePath;

            if (ChessSquares[targetSquare.Index].Piece is Pawn) ((Pawn)(ChessSquares[targetSquare.Index].Piece)).IsFirstMove = false;
            if (ChessSquares[targetSquare.Index].Piece is King) ((King)(ChessSquares[targetSquare.Index].Piece)).CastlingRight = false;
            if (ChessSquares[targetSquare.Index].Piece is Rook) ((Rook)(ChessSquares[targetSquare.Index].Piece)).CastlingRight = false;

            int start = targetSquare.Index;
            int end = SelectedSquare.Index;

            if (ChessSquares[start].Piece is Pawn && Math.Abs(start - end) is (7 or 9) && ChessSquares[end].Piece is null) // en passant
            {
                var origin = Mapping.IndexToDoubleIndex[start];
                var destenation = Mapping.IndexToDoubleIndex[end];

                int targetRow = origin.Key;
                int targetColumn = destenation.Value;

                int pawnIndexToRemove = Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(targetRow, targetColumn)];
                ChessSquares[pawnIndexToRemove].Piece = null;
                ChessSquares[pawnIndexToRemove].ImagePath = null;

                EnPassantPossibilty = false;
            }

            ChessSquares[SelectedSquare.Index].Piece = null;
            ChessSquares[SelectedSquare.Index].ImagePath = null;
        }

        private void HighlightSquares(int start, int end)
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

            LastMove.Add(start);
            LastMove.Add(end);
            SelectedSquare = null;

            foreach (var move in LastMove)
            {
                ChessSquares[move].Color = "Yellow";
            }
        }

        #endregion

        #region Engine

        private async Task GetEngineMoveAsync(string playerMove, bool promotion = false)
        {
            string engineMove;

            if (playerMove.Equals(string.Empty)) // prvi potez igra engine
            {
                engineMove = await Task.Run(() => StockfishManager.GetBestMove());
            }
            else // svi ostali potezi
            {
                engineMove = await Task.Run(() => StockfishManager.GetBestMove(playerMove));
            }

            MakeEngineMove(engineMove);

            int start = Mapping.CoordinateToIndex[engineMove.Substring(0, 2)];
            int end = Mapping.CoordinateToIndex[engineMove.Substring(2, 2)];

            HighlightSquares(start, end);
        }

        private void MakeEngineMove(string engineMove)
        {
            if (ChessSquares[Mapping.CoordinateToIndex[engineMove.Substring(0, 2)]].Piece is King)
            {
                switch (engineMove)
                {
                    case "e1g1": // mala rokada beli
                        CastlingMove(ChessSquares[Mapping.CoordinateToIndex[engineMove.Substring(2)]],
                            ChessSquares[Mapping.CoordinateToIndex[engineMove.Substring(0, 2)]]);
                        return;
                    case "e1c1": // velika rokada beli
                        CastlingMove(ChessSquares[Mapping.CoordinateToIndex[engineMove.Substring(2)]],
                            ChessSquares[Mapping.CoordinateToIndex[engineMove.Substring(0, 2)]]);
                        return;
                    case "e8g8": // mala rokada crni
                        CastlingMove(ChessSquares[Mapping.CoordinateToIndex[engineMove.Substring(2)]],
                            ChessSquares[Mapping.CoordinateToIndex[engineMove.Substring(0, 2)]]);
                        return;
                    case "e8c8": // velika rokada crni
                        CastlingMove(ChessSquares[Mapping.CoordinateToIndex[engineMove.Substring(2)]],
                            ChessSquares[Mapping.CoordinateToIndex[engineMove.Substring(0, 2)]]);
                        return;
                }

                EnPassantPossibilty = false;
            }

            if (engineMove.Length == 5) // promocija
            {
                int origin = Mapping.CoordinateToIndex[engineMove.Substring(0, 2)];
                int destenation = Mapping.CoordinateToIndex[engineMove.Substring(2, 2)];
                string promotionChar = engineMove.Substring(4);

                ChessSquares[origin].Piece = null;
                ChessSquares[origin].ImagePath = null;

                ChessSquares[destenation].Piece = GetPromotionPiece(promotionChar);
                ChessSquares[destenation].ImagePath = GetPromotionPieceImage(promotionChar);

                EnPassantPossibilty = false;
            }
            else // obican potez i en passant
            {
                int start = Mapping.CoordinateToIndex[engineMove.Substring(0, 2)];
                int end = Mapping.CoordinateToIndex[engineMove.Substring(2)];

                if (ChessSquares[start].Piece is Pawn && Math.Abs(start - end) is (7 or 9) && ChessSquares[end].Piece is null) // en passant
                {
                    var origin = Mapping.IndexToDoubleIndex[start];
                    var destenation = Mapping.IndexToDoubleIndex[end];

                    int targetRow = origin.Key;
                    int targetColumn = destenation.Value;

                    int pawnIndexToRemove = Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(targetRow, targetColumn)];
                    ChessSquares[pawnIndexToRemove].Piece = null;
                    ChessSquares[pawnIndexToRemove].ImagePath = null;

                    EnPassantPossibilty = false;
                }

                ChessSquares[end].Piece = ChessSquares[start].Piece;
                ChessSquares[end].ImagePath = ChessSquares[start].ImagePath;

                ChessSquares[start].Piece = null;
                ChessSquares[start].ImagePath = null;

                if (ChessSquares[end].Piece is Pawn) ((Pawn)ChessSquares[end].Piece).IsFirstMove = false;
                if (ChessSquares[end].Piece is King) ((King)ChessSquares[end].Piece).CastlingRight = false;
                if (ChessSquares[end].Piece is Rook) ((Rook)ChessSquares[end].Piece).CastlingRight = false;

                if (ChessSquares[end].Piece is Pawn && Math.Abs(end - start) == 2)
                {
                    EnPassantPossibilty = true;

                    var targetSquare = Mapping.IndexToDoubleIndex[end];
                    EnPassantSquare = Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(targetSquare.Key, targetSquare.Value)];
                }
                else
                {
                    EnPassantPossibilty = false;
                }
            }
        }

        private Piece GetPromotionPiece(string promotionChar)
        {
            var color = PlayerColor == Color.White ? Color.Black : Color.White;

            return promotionChar switch
            {
                "q" => new Queen(color),
                "n" => new Knight(color),
                "r" => new Rook(color, false),
                "b" => new Bishop(color),
                _ => new Piece("King", ushort.MaxValue, Color.White, PieceType.King)
            };
        }

        private string GetPromotionPieceImage(string promotionChar)
        {
            var color = PlayerColor == Color.White ? "B" : "W";
            var currentDirectory = Directory.GetCurrentDirectory();
            var targetFolder = Path.Combine(currentDirectory, "..", "..", "..", "Images");

            return promotionChar switch
            {
                "q" => Path.Combine(targetFolder, $"Queen_{color}.png"),
                "n" => Path.Combine(targetFolder, $"Knight_{color}.png"),
                "r" => Path.Combine(targetFolder, $"Rook_{color}.png"),
                "b" => Path.Combine(targetFolder, $"Bishop_{color}.png"),
                _ => Path.Combine(targetFolder, "King_W.png")
            };
        }

        #endregion
    }
}
