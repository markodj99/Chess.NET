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
        public King(Color color) : base("King", ushort.MaxValue, color, PieceType.King) { }

        public override List<ushort> GetPossibleMoves(ChessSquare chessSquare, List<ChessSquare> board)
        {
            int row = chessSquare.Row;
            int column = chessSquare.Column;

            var allHorizontalsAndVerticals = GetAllHorizontalsAndVerticals(board, row, column);
            var allDiagonals = GetAllDiagonals(board, row, column);

            var allMoves = new List<KeyValuePair<int, int>>(allHorizontalsAndVerticals.Count + allDiagonals.Count);
            allMoves.AddRange(allHorizontalsAndVerticals);
            allMoves.AddRange(allDiagonals);

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
    }
}
