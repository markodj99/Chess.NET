using DiplomskiRad.Helper;
using DiplomskiRad.Models.Enums;
using DiplomskiRad.Models.Game;
using System.Collections.Generic;
using System.Linq;

namespace DiplomskiRad.Models.Pieces
{
    public class Bishop : Piece
    {
        public Bishop(Color color) : base("Bishop", 3, color, PieceType.Bishop) { }

        public override List<int> GetPossibleMoves(ChessSquare chessSquare, List<ChessSquare> board, int enPassantPosibleSquare = -1)
        {
            int row = chessSquare.Row;
            int column = chessSquare.Column;

            var allDiags = GetAllDiagonals(board, row, column);

            var moves = new List<int>(13);
            moves.AddRange(allDiags.Select(diag => Mapping.DoubleIndexToIndex[diag]));

            return moves;
        }

        private List<KeyValuePair<int, int>> GetAllDiagonals(List<ChessSquare> board, int row, int column)
        {
            var allDiags = new List<KeyValuePair<int, int>>(13);

            for (int i = 1; i <= 7; i++) // gore levo
            {
                if (row - i >= 0 && column - i >= 0)
                {
                    if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(row - i, column - i)]].Piece == null)
                    {
                        allDiags.Add(new(row - i, column - i));
                    }
                    else if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(row - i, column - i)]].Piece.Color != Color)
                    {
                        allDiags.Add(new(row - i, column - i));
                        break;
                    }
                    else break;
                }
            }

            for (int i = 1; i <= 7; i++) // gore desno
            {
                if (row - i >= 0 && column + i <= 7)
                {
                    if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(row - i, column + i)]].Piece == null)
                    {
                        allDiags.Add(new(row - i, column + i));
                    }
                    else if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(row - i, column + i)]].Piece.Color != Color)
                    {
                        allDiags.Add(new(row - i, column + i));
                        break;
                    }
                    else break;
                }
            }

            for (int i = 1; i <= 7; i++) // dole levo
            {
                if (row + i <= 7 && column - i >= 0)
                {
                    if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(row + i, column - i)]].Piece == null)
                    {
                        allDiags.Add(new(row + i, column - i));
                    }
                    else if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(row + i, column - i)]].Piece.Color != Color)
                    {
                        allDiags.Add(new(row + i, column - i));
                        break;
                    }
                    else break;
                }
            }

            for (int i = 1; i <= 7; i++) // dole desno
            {
                if (row + i <= 7 && column + i <= 7)
                {
                    if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(row + i, column + i)]].Piece == null)
                    {
                        allDiags.Add(new(row + i, column + i));
                    }
                    else if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(row + i, column + i)]].Piece.Color != Color)
                    {
                        allDiags.Add(new(row + i, column + i));
                        break;
                    }
                    else break;
                }
            }

            return allDiags;
        }
    }
}
