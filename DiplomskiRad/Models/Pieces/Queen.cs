using DiplomskiRad.Helper;
using DiplomskiRad.Models.Enums;
using DiplomskiRad.Models.Game;
using System.Collections.Generic;
using System.Linq;

namespace DiplomskiRad.Models.Pieces
{
    public class Queen : Piece
    {
        public Queen(Color color) : base("Queen", 8, color, PieceType.Pawn) { }

        public override List<ushort> GetPossibleMoves(ChessSquare chessSquare, List<ChessSquare> board)
        {
            int row = chessSquare.Row;
            int column = chessSquare.Column;

            var allHorizontalsAndVerticals = GetAllHorizontalsAndVerticals(board, row, column);
            var allDiagonals = GetAllDiagonals(board, row, column);

            var allMoves = new List<KeyValuePair<int, int>>(allHorizontalsAndVerticals.Count + allDiagonals.Count);
            allMoves.AddRange(allHorizontalsAndVerticals);
            allMoves.AddRange(allDiagonals);

            var moves = new List<ushort>(13);
            moves.AddRange(allMoves.Select(move => Mapping.DoubleIndexToIndex[move]));

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
                        allDiags.Add(new(row - i, column - i));
                    else if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(row - i, column - i)]].Piece
                                 .Color != Color)
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
                        allDiags.Add(new(row - i, column + i));
                    else if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(row - i, column + i)]].Piece
                                 .Color != Color)
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
                        allDiags.Add(new(row + i, column - i));
                    else if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(row + i, column - i)]].Piece
                                 .Color != Color)
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
                        allDiags.Add(new(row + i, column + i));
                    else if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(row + i, column + i)]].Piece
                                 .Color != Color)
                    {
                        allDiags.Add(new(row + i, column + i));
                        break;
                    }
                    else break;
                }
            }

            return allDiags;
        }

        private List<KeyValuePair<int, int>> GetAllHorizontalsAndVerticals(List<ChessSquare> board, int row, int column)
        {
            var allDiags = new List<KeyValuePair<int, int>>(14);

            for (int i = 1; i <= 7; i++) // gore
            {
                if (row - i >= 0)
                {
                    if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(row - i, column)]].Piece == null)
                        allDiags.Add(new(row - i, column));
                    else if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(row - i, column)]].Piece
                                 .Color != Color)
                    {
                        allDiags.Add(new(row - i, column));
                        break;
                    }
                    else break;
                }
            }

            for (int i = 1; i <= 7; i++) // dole
            {
                if (row + i <= 7)
                {
                    if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(row + i, column)]].Piece == null)
                        allDiags.Add(new(row + i, column));
                    else if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(row + i, column)]].Piece
                                 .Color != Color)
                    {
                        allDiags.Add(new(row + i, column));
                        break;
                    }
                    else break;
                }
            }

            for (int i = 1; i <= 7; i++) // desno
            {
                if (column + i <= 7)
                {
                    if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(row, column + i)]].Piece == null)
                        allDiags.Add(new(row, column + i));
                    else if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(row, column + i)]].Piece
                                 .Color != Color)
                    {
                        allDiags.Add(new(row, column + i));
                        break;
                    }
                    else break;
                }
            }

            for (int i = 1; i <= 7; i++) // levo
            {
                if (column - i >= 0)
                {
                    if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(row, column - i)]].Piece == null)
                        allDiags.Add(new(row, column - i));
                    else if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(row, column - i)]].Piece
                                 .Color != Color)
                    {
                        allDiags.Add(new(row, column - i));
                        break;
                    }
                    else break;
                }
            }

            return allDiags;
        }
    }
}
