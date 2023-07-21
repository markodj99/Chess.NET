using DiplomskiRad.Helper;
using DiplomskiRad.Models.Enums;
using System.Collections.Generic;
using System.Linq;

namespace DiplomskiRad.Models
{
    public class Queen : Piece
    {
        public Queen(Color color, int row, int column) : base("Queen", 8, color, PieceType.Pawn, row, column) { }

        public override List<ushort> GetPossibleMoves(ChessSquare chessSquare, List<ChessSquare> board)
        {
            var allHorizontalsAndVerticals = GetAllHorizontalsAndVerticals(board);
            var allDiagonals = GetAllDiagonals(board);

            var allMoves = new List<KeyValuePair<int, int>>(allHorizontalsAndVerticals.Count + allDiagonals.Count);
            allMoves.AddRange(allHorizontalsAndVerticals);
            allMoves.AddRange(allDiagonals);

            var moves = new List<ushort>(13);
            moves.AddRange(allMoves.Select(move => Mapping.DoubleIndexToIndex[move]));

            return moves;
        }

        private List<KeyValuePair<int, int>> GetAllDiagonals(List<ChessSquare> board)
        {
            var allDiags = new List<KeyValuePair<int, int>>(13);

            for (int i = 1; i <= 7; i++) // gore levo
            {
                if (Row - i >= 0 && Column - i >= 0)
                {
                    if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(Row - i, Column - i)]].Piece == null)
                        allDiags.Add(new(Row - i, Column - i));
                    else if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(Row - i, Column - i)]].Piece
                                 .Color != Color)
                    {
                        allDiags.Add(new(Row - i, Column - i));
                        break;
                    }
                    else break;
                }
            }

            for (int i = 1; i <= 7; i++) // gore desno
            {
                if (Row - i >= 0 && Column + i <= 7)
                {
                    if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(Row - i, Column + i)]].Piece == null)
                        allDiags.Add(new(Row - i, Column + i));
                    else if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(Row - i, Column + i)]].Piece
                                 .Color != Color)
                    {
                        allDiags.Add(new(Row - i, Column + i));
                        break;
                    }
                    else break;
                }
            }

            for (int i = 1; i <= 7; i++) // dole levo
            {
                if (Row + i <= 7 && Column - i >= 0)
                {
                    if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(Row + i, Column - i)]].Piece == null)
                        allDiags.Add(new(Row + i, Column - i));
                    else if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(Row + i, Column - i)]].Piece
                                 .Color != Color)
                    {
                        allDiags.Add(new(Row + i, Column - i));
                        break;
                    }
                    else break;
                }
            }

            for (int i = 1; i <= 7; i++) // dole desno
            {
                if (Row + i <= 7 && Column + i <= 7)
                {
                    if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(Row + i, Column + i)]].Piece == null)
                        allDiags.Add(new(Row + i, Column + i));
                    else if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(Row + i, Column + i)]].Piece
                                 .Color != Color)
                    {
                        allDiags.Add(new(Row + i, Column + i));
                        break;
                    }
                    else break;
                }
            }

            return allDiags;
        }

        private List<KeyValuePair<int, int>> GetAllHorizontalsAndVerticals(List<ChessSquare> board)
        {
            var allDiags = new List<KeyValuePair<int, int>>(14);

            for (int i = 1; i <= 7; i++) // gore
            {
                if (Row - i >= 0)
                {
                    if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(Row - i, Column)]].Piece == null)
                        allDiags.Add(new(Row - i, Column));
                    else if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(Row - i, Column)]].Piece
                                 .Color != Color)
                    {
                        allDiags.Add(new(Row - i, Column));
                        break;
                    }
                    else break;
                }
            }

            for (int i = 1; i <= 7; i++) // dole
            {
                if (Row + i <= 7)
                {
                    if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(Row + i, Column)]].Piece == null)
                        allDiags.Add(new(Row + i, Column));
                    else if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(Row + i, Column)]].Piece
                                 .Color != Color)
                    {
                        allDiags.Add(new(Row + i, Column));
                        break;
                    }
                    else break;
                }
            }

            for (int i = 1; i <= 7; i++) // desno
            {
                if (Column + i <= 7)
                {
                    if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(Row, Column + i)]].Piece == null)
                        allDiags.Add(new(Row, Column + i));
                    else if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(Row, Column + i)]].Piece
                                 .Color != Color)
                    {
                        allDiags.Add(new(Row, Column + i));
                        break;
                    }
                    else break;
                }
            }

            for (int i = 1; i <= 7; i++) // levo
            {
                if (Column - i >= 0)
                {
                    if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(Row, Column - i)]].Piece == null)
                        allDiags.Add(new(Row, Column - i));
                    else if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(Row, Column - i)]].Piece
                                 .Color != Color)
                    {
                        allDiags.Add(new(Row, Column - i));
                        break;
                    }
                    else break;
                }
            }

            return allDiags;
        }
    }
}
