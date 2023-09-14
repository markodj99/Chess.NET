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
using System.Windows;
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
        public ICommand NewGameCommand { get; private set; }

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
        public bool CanPlayerMove { get; set; }

        #endregion

        #region Initialization

        public ChessBoardGameViewModel()
        {
            FlipBoard = new FlipBoard();

            ClickCommand = new Command(ExecuteClickCommand, CanExecuteClickCommand);
            MoveCommand = new Command(ExecuteMoveCommand, CanExecuteMoveCommand);
            NewGameCommand = new Command(ExecuteNewGameCommand, CanExecuteNewGameCommand);
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
                CanPlayerMove = false;
                FlipBoard.Orientation = Color.Black;

                await GetEngineMoveAsync(string.Empty);
            }
            else
            {
                CanPlayerMove = true;
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

        private bool CanExecuteNewGameCommand(object parameter) => CanPlayerMove;

        private void ExecuteNewGameCommand(object parameter)
        {

        }

        private bool CanExecuteClickCommand(object parameter)
        {
            if (!CanPlayerMove) return false;

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

            GameStatus status = IsCheckMateOrStaleMate(PlayerColor);
            if (status == GameStatus.CheckMate) MessageBox.Show("You've lost.");
            if (status == GameStatus.StaleMate) MessageBox.Show("Stale Mate.");
            if (IsDrawByInsufficientMaterial()) MessageBox.Show("Draw by insufficient material.");

            SelectedSquare = null;
        }

        #endregion

        #region Private Methods

        private void UpdateAvailableMoves()
        {
            if (SelectedSquare?.Piece == null) return;
            var original = EnPassantPossibilty ? SelectedSquare.Piece.GetPossibleMoves(SelectedSquare, ChessSquares.ToList(), EnPassantSquare) : SelectedSquare.Piece.GetPossibleMoves(SelectedSquare, ChessSquares.ToList());
            var final = AreMovesValid(original, SelectedSquare);
            HighlightedSquares.AddRange(final);
            foreach (var t in HighlightedSquares)
            {
                ChessSquares[t].Color = "Black";
            }
        }

        private List<int> AreMovesValid(List<int> possibleMoves, ChessSquare selectedSquare)
        {
            if (selectedSquare.Piece is King { CastlingRight: true }) return AreKingMovesValid(possibleMoves, selectedSquare);

            var retVal = new List<int>(possibleMoves);

            var initialPiecePosition = selectedSquare.Index;
            foreach (var move in possibleMoves)
            {
                var boardCopy = new List<ChessSquare>(ChessSquares.Count);
                foreach (var square in ChessSquares) boardCopy.Add(new ChessSquare(square));
                
                boardCopy[move].Piece = boardCopy[initialPiecePosition].Piece;
                boardCopy[initialPiecePosition].Piece = null; // zamena pozicija

                int kingPos = 100;
                foreach (var square in boardCopy.Where(x => x.Piece != null && x.Piece.Color == selectedSquare.Piece.Color))
                {
                    if (square.Piece.Type == PieceType.King)
                    {
                        kingPos = square.Index;
                        break;
                    }
                }

                foreach (var square in boardCopy.Where(x => x.Piece != null && x.Piece.Color != selectedSquare.Piece.Color))
                {
                    var pieceMoves = square.Piece.GetPossibleMoves(square, boardCopy);
                    if (pieceMoves.Contains(kingPos)) retVal.Remove(move);
                }

                boardCopy[initialPiecePosition].Piece = boardCopy[move].Piece;
                boardCopy[move].Piece = null; // vracanje pozicija
            }

            return retVal;
        }

        private List<int> AreKingMovesValid(List<int> possibleMoves, ChessSquare selectedSquare)
        {
            var retVal = new List<int>(possibleMoves);

            int initialKingPos = selectedSquare.Index;
            foreach (var move in possibleMoves)
            {
                var boardCopy = new List<ChessSquare>(ChessSquares.Count);
                foreach (var square in ChessSquares) boardCopy.Add(new ChessSquare(square));
                
                boardCopy[move].Piece = boardCopy[initialKingPos].Piece;
                boardCopy[initialKingPos].Piece = null; // zamena pozicija

                int newKingPos = boardCopy[move].Index;

                foreach (var square in boardCopy.Where(x => x.Piece != null && x.Piece.Color != selectedSquare.Piece.Color))
                {
                    var pieceMoves = square.Piece.GetPossibleMoves(square, boardCopy);
                    if (pieceMoves.Contains(newKingPos))
                    {
                        if (newKingPos == initialKingPos + 1) retVal.Remove(initialKingPos + 2);
                        if (newKingPos == initialKingPos - 1) retVal.Remove(initialKingPos - 2);
                        retVal.Remove(move);
                    }

                    if (!pieceMoves.Contains(initialKingPos)) continue;
                    retVal.Remove(initialKingPos + 2);
                    retVal.Remove(initialKingPos - 2);
                }

                boardCopy[initialKingPos].Piece = boardCopy[move].Piece;
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

            return targetSquare.Piece is Knight ? "N" : targetSquare.Piece.Name.Substring(0, 1);
        }

        private void OrdinaryMove(ChessSquare targetSquare)
        {
            int start = SelectedSquare.Index;
            int end = targetSquare.Index;

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

            ChessSquares[targetSquare.Index].Piece = SelectedSquare.Piece;
            ChessSquares[targetSquare.Index].ImagePath = SelectedSquare.ImagePath;

            if (ChessSquares[targetSquare.Index].Piece is Pawn) ((Pawn)(ChessSquares[targetSquare.Index].Piece)).IsFirstMove = false;
            if (ChessSquares[targetSquare.Index].Piece is King) ((King)(ChessSquares[targetSquare.Index].Piece)).CastlingRight = false;
            if (ChessSquares[targetSquare.Index].Piece is Rook) ((Rook)(ChessSquares[targetSquare.Index].Piece)).CastlingRight = false;

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

            foreach (var move in LastMove)
            {
                ChessSquares[move].Color = "Yellow";
            }
        }

        #endregion

        #region Engine

        private async Task GetEngineMoveAsync(string playerMove)
        {
            CanPlayerMove = false;

            string engineMove;

            if (playerMove.Equals(string.Empty)) // prvi potez igra engine
            {
                engineMove = await Task.Run(() => StockfishManager.GetBestMove());
            }
            else // svi ostali potezi
            {
                engineMove = await Task.Run(() => StockfishManager.GetBestMove(playerMove));
            }

            if (engineMove.Equals("(none)"))
            {
                MessageBox.Show(IsStockfishKingAttacked() ? "You've won." : "Stale mate.");
                return;
            }

            MakeEngineMove(engineMove);

            int start = Mapping.CoordinateToIndex[engineMove.Substring(0, 2)];
            int end = Mapping.CoordinateToIndex[engineMove.Substring(2, 2)];

            HighlightSquares(start, end);

            CanPlayerMove = true;
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

                int rowStart = Mapping.IndexToDoubleIndex[start].Key;
                int rowEnd = Mapping.IndexToDoubleIndex[end].Key;

                if (ChessSquares[end].Piece is Pawn && Math.Abs(rowStart - rowEnd) == 2)
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

        #region Game Status

        private GameStatus IsCheckMateOrStaleMate(Color playerColor)
        {
            int kingPos = 100;
            foreach (var chessSquare in ChessSquares.Where(x => x.Piece != null && x.Piece.Color == playerColor))
            {
                if (chessSquare.Piece is King) kingPos = chessSquare.Index;
                var original = EnPassantPossibilty ? chessSquare.Piece.GetPossibleMoves(chessSquare, ChessSquares.ToList(), EnPassantSquare) : chessSquare.Piece.GetPossibleMoves(chessSquare, ChessSquares.ToList());
                var final = AreMovesValid(original, chessSquare);
                if (final.Count > 0) return GameStatus.Continue;
            }

            foreach (var chessSquare in ChessSquares.Where(x => x.Piece != null && x.Piece.Color != playerColor))
            {
                var original = chessSquare.Piece.GetPossibleMoves(chessSquare, ChessSquares.ToList());
                if(original.Contains(kingPos)) return GameStatus.CheckMate;
            }

            return GameStatus.StaleMate;
        }

        private bool IsStockfishKingAttacked()
        {
            int stockfishKingPos = 100;

            foreach (var chessSquare in ChessSquares.Where(x => x.Piece != null && x.Piece.Color != PlayerColor))
            {
                if (chessSquare.Piece is not King) continue;
                stockfishKingPos = chessSquare.Index;
                break;
            }

            foreach (var chessSquare in ChessSquares.Where(x => x.Piece != null && x.Piece.Color == PlayerColor))
            {
                if (chessSquare.Piece.GetPossibleMoves(chessSquare, ChessSquares.ToList()).Contains(stockfishKingPos)) return true;
            }

            return false;
        }

        private bool IsDrawByInsufficientMaterial()
        {
            var whitePieces = new List<PieceType>();
            var blackPieces = new List<PieceType>();

            foreach (var chessSquare in ChessSquares.Where(x => x.Piece != null))
            {
                var piece = chessSquare.Piece switch
                {
                    King => PieceType.King,
                    Queen => PieceType.Queen,
                    Rook => PieceType.Rook,
                    Bishop => PieceType.Bishop,
                    Knight => PieceType.Knight,
                    Pawn => PieceType.Pawn,
                    _ => PieceType.Pawn
                };
                if(chessSquare.Piece.Color == PlayerColor) whitePieces.Add(piece);
                else blackPieces.Add(piece);
            }

            if (whitePieces.Count > 3 || blackPieces.Count > 3) return false;

            if (whitePieces.Count == 1 && blackPieces.Count == 1) return true; // kralj vs kralj
            if (KingBishopVsKing(whitePieces, blackPieces)) return true; // kralj lovac vs kralj
            if (KingKnightVsKing(whitePieces, blackPieces)) return true; // kralj skakac vs kralj
            if (MinorPieceDraw(whitePieces, blackPieces)) return true; // kralj skakac vs kralj lovac || kralj skakac vs kralj skakac || kralj lovac vs kralj lovac
            if (KingTwoKnightsVsKing(whitePieces, blackPieces)) return true; // kralj skakac skakac vs kralj -> poseban slucaj u sahu

            return false;
        }

        private bool KingBishopVsKing(List<PieceType> whitePieces, List<PieceType> blackPieces)
        {
            if (whitePieces.Count == 2 && blackPieces.Count == 1)
            {
                if (whitePieces[0] is (PieceType.King or PieceType.Bishop) && whitePieces[1] is (PieceType.King or PieceType.Bishop)) return true;
            }

            if (whitePieces.Count == 1 && blackPieces.Count == 2)
            {
                if (blackPieces[0] is (PieceType.King or PieceType.Bishop) && blackPieces[1] is (PieceType.King or PieceType.Bishop)) return true;
            }

            return false;
        }

        private bool KingKnightVsKing(List<PieceType> whitePieces, List<PieceType> blackPieces)
        {
            if (whitePieces.Count == 2 && blackPieces.Count == 1)
            {
                if (whitePieces[0] is (PieceType.King or PieceType.Knight) && whitePieces[1] is (PieceType.King or PieceType.Knight)) return true;
            }

            if (whitePieces.Count == 1 && blackPieces.Count == 2)
            {
                if (blackPieces[0] is (PieceType.King or PieceType.Knight) && blackPieces[1] is (PieceType.King or PieceType.Knight)) return true;
            }

            return false;
        }

        private bool MinorPieceDraw(List<PieceType> whitePieces, List<PieceType> blackPieces)
        {
            if (whitePieces.Count == 2 && blackPieces.Count == 2)
            {
                if ((whitePieces[0] is (PieceType.King or PieceType.Knight or PieceType.Bishop) && whitePieces[1] is (PieceType.King or PieceType.Knight or PieceType.Bishop))
                    && (blackPieces[0] is (PieceType.King or PieceType.Knight or PieceType.Bishop) && blackPieces[1] is (PieceType.King or PieceType.Knight or PieceType.Bishop))) 
                    return true;
            }

            return false;
        }

        private bool KingTwoKnightsVsKing(List<PieceType> whitePieces, List<PieceType> blackPieces)
        {
            if (whitePieces.Count == 3 && blackPieces.Count == 1)
            {
                if (whitePieces[0] is (PieceType.King or PieceType.Knight) && whitePieces[1] is (PieceType.King or PieceType.Knight)
                                                                           && whitePieces[2] is (PieceType.King or PieceType.Knight)) return true;
            }

            if (whitePieces.Count == 1 && blackPieces.Count == 3)
            {
                if (blackPieces[0] is (PieceType.King or PieceType.Knight) && blackPieces[1] is (PieceType.King or PieceType.Knight)
                                                                           && blackPieces[2] is (PieceType.King or PieceType.Knight)) return true;
            }

            return false;
        }

        #endregion
    }
}
