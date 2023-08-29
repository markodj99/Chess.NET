using DiplomskiRad.Helper;
using DiplomskiRad.Models.Enums;
using DiplomskiRad.Models.Game;
using System.Collections.Generic;
using System.Linq;

namespace DiplomskiRad.Models.Pieces
{
    public class Rook : Piece
    {
        public bool CastlingRight { get; set; }

        public Rook(Color color, bool castlingRight) : base("Rook", 5, color, PieceType.Rook) => CastlingRight = castlingRight;

        public override List<ushort> GetPossibleMoves(ChessSquare chessSquare, List<ChessSquare> board, int enPassantPosibleSquare = -1)
        {
            int row = chessSquare.Row;
            int column = chessSquare.Column;

            var allMoves = GetAllHorizontalsAndVerticals(board, row, column);

            var moves = new List<ushort>(13);
            moves.AddRange(allMoves.Select(move => Mapping.DoubleIndexToIndex[move]));

            return moves;
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
