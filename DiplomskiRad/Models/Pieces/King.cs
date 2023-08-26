using DiplomskiRad.Helper;
using DiplomskiRad.Models.Enums;
using DiplomskiRad.Models.Game;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace DiplomskiRad.Models.Pieces
{
    public class King : Piece
    {
        public bool CastlingRight { get; set; }

        public King(Color color, bool castlingRight) : base("King", ushort.MaxValue, color, PieceType.King) => CastlingRight = castlingRight;

        public override List<ushort> GetPossibleMoves(ChessSquare chessSquare, List<ChessSquare> board)
        {
            int row = chessSquare.Row;
            int column = chessSquare.Column;

            var allHorizontalsAndVerticals = GetAllHorizontalsAndVerticals(board, row, column);
            var allDiagonals = GetAllDiagonals(board, row, column);
            var castleMoves = GetCastleSquares(board, chessSquare, row, column);

            var allMoves = new List<KeyValuePair<int, int>>(allHorizontalsAndVerticals.Count + allDiagonals.Count + castleMoves.Count);
            allMoves.AddRange(allHorizontalsAndVerticals);
            allMoves.AddRange(allDiagonals);
            allMoves.AddRange(castleMoves);

            var moves = new List<ushort>(8);
            moves.AddRange(allMoves.Select(move => Mapping.DoubleIndexToIndex[move]));

            return moves;
        }

        private List<KeyValuePair<int, int>> GetAllDiagonals(List<ChessSquare> board, int row, int column)
        {
            var allDiags = new List<KeyValuePair<int, int>>(4);

            if (row - 1 >= 0 && column - 1 >= 0) // gore levo
                if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(row - 1, column - 1)]].Piece == null)
                    allDiags.Add(new(row - 1, column - 1));
                else if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(row - 1, column - 1)]].Piece
                             .Color != Color)
                    allDiags.Add(new(row - 1, column - 1));

            if (row - 1 >= 0 && column + 1 <= 7) // gore desno
                if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(row - 1, column + 1)]].Piece == null)
                    allDiags.Add(new(row - 1, column + 1));
                else if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(row - 1, column + 1)]].Piece
                             .Color != Color)
                    allDiags.Add(new(row - 1, column + 1));

            if (row + 1 <= 7 && column - 1 >= 0) // dole levo
                if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(row + 1, column - 1)]].Piece == null)
                    allDiags.Add(new(row + 1, column - 1));
                else if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(row + 1, column - 1)]].Piece
                             .Color != Color)
                    allDiags.Add(new(row + 1, column - 1));

            if (row + 1 <= 7 && column + 1 <= 7) // dole desno
                if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(row + 1, column + 1)]].Piece == null)
                    allDiags.Add(new(row + 1, column + 1));
                else if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(row + 1, column + 1)]].Piece
                             .Color != Color)
                    allDiags.Add(new(row + 1, column + 1));

            return allDiags;
        }

        private List<KeyValuePair<int, int>> GetAllHorizontalsAndVerticals(List<ChessSquare> board, int row, int column)
        {
            var allDiags = new List<KeyValuePair<int, int>>(4);

            if (row - 1 >= 0) // gore
                if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(row - 1, column)]].Piece == null)
                    allDiags.Add(new(row - 1, column));
                else if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(row - 1, column)]].Piece
                             .Color != Color)
                    allDiags.Add(new(row - 1, column));

            if (row + 1 <= 7) // dole
                if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(row + 1, column)]].Piece == null)
                    allDiags.Add(new(row + 1, column));
                else if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(row + 1, column)]].Piece
                             .Color != Color)
                    allDiags.Add(new(row + 1, column));

            if (column + 1 <= 7) // desno
                if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(row, column + 1)]].Piece == null)
                    allDiags.Add(new(row, column + 1));
                else if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(row, column + 1)]].Piece
                             .Color != Color)
                    allDiags.Add(new(row, column + 1));

            if (column - 1 >= 0) // levo
                if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(row, column - 1)]].Piece == null)
                    allDiags.Add(new(row, column - 1));
                else if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(row, column - 1)]].Piece
                             .Color != Color)
                    allDiags.Add(new(row, column - 1));

            return allDiags;
        }

        private List<KeyValuePair<int, int>> GetCastleSquares(List<ChessSquare> board, ChessSquare chessSquare, int row, int column)
        {
            if (((King)chessSquare.Piece).CastlingRight)
            {
                var retVal = new List<KeyValuePair<int, int>>();

                if (ShortCastle(board, chessSquare, row, column))
                {
                    retVal.Add(new KeyValuePair<int, int>(row, column + 2));
                }

                if (LongCastle(board, chessSquare, row, column))
                {
                    retVal.Add(new KeyValuePair<int, int>(row, column - 2));
                }

                return retVal;
            }

            return new List<KeyValuePair<int, int>>(0);
        }

        private bool ShortCastle(List<ChessSquare> board, ChessSquare chessSquare, int row, int column)
        {
            for (int i = 1; i < 3; i++)
            {
                if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(row, column + i)]].Piece != null) return false;
            }

            var piece = board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(row, column + 3)]].Piece;
            return piece switch
            {
                null => false,
                Rook r when r.CastlingRight && r.Color == chessSquare.Piece.Color => true,
                _ => false
            };
        }

        private bool LongCastle(List<ChessSquare> board, ChessSquare chessSquare, int row, int column)
        {
            for (int i = 1; i < 4; i++)
            {
                if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(row, column - i)]].Piece != null) return false;
            }

            var piece = board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(row, column - 4)]].Piece;
            return piece switch
            {
                null => false,
                Rook r when r.CastlingRight && r.Color == chessSquare.Piece.Color => true,
                _ => false
            };
        }
    }
}
