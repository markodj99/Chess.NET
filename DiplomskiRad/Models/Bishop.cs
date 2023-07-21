using DiplomskiRad.Helper;
using DiplomskiRad.Models.Enums;
using System.Collections.Generic;
using System.Linq;

namespace DiplomskiRad.Models
{
    public class Bishop : Piece
    {
        public Bishop(Color color, int row, int column) : base("Bishop", 3, color, PieceType.Bishop, row, column) { }

        public override List<ushort> GetPossibleMoves(ChessSquare chessSquare, List<ChessSquare> board)
        {
            var allDiags = GetAllDiagonals(board);  

            var moves = new List<ushort>(13);
            moves.AddRange(allDiags.Select(diag => Mapping.DoubleIndexToIndex[diag]));

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
    }
}
