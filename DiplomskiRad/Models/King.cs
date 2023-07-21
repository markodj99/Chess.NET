using DiplomskiRad.Helper;
using DiplomskiRad.Models.Enums;
using System.Collections.Generic;
using System.Linq;

namespace DiplomskiRad.Models
{
    public class King : Piece
    {
        public King(Color color, int row, int column) : base("King", ushort.MaxValue, color, PieceType.King, row, column) { }

        public override List<ushort> GetPossibleMoves(ChessSquare chessSquare, List<ChessSquare> board)
        {
            var allHorizontalsAndVerticals = GetAllHorizontalsAndVerticals(board);
            var allDiagonals = GetAllDiagonals(board);

            var allMoves = new List<KeyValuePair<int, int>>(allHorizontalsAndVerticals.Count + allDiagonals.Count);
            allMoves.AddRange(allHorizontalsAndVerticals);
            allMoves.AddRange(allDiagonals);

            var moves = new List<ushort>(8);
            moves.AddRange(allMoves.Select(move => Mapping.DoubleIndexToIndex[move]));

            return moves;
        }

        private List<KeyValuePair<int, int>> GetAllDiagonals(List<ChessSquare> board)
        {
            var allDiags = new List<KeyValuePair<int, int>>(4);

            if (Row - 1 >= 0 && Column - 1 >= 0) // gore levo
                if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(Row - 1, Column - 1)]].Piece == null)
                    allDiags.Add(new(Row - 1, Column - 1));
                else if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(Row - 1, Column - 1)]].Piece
                             .Color != Color)
                    allDiags.Add(new(Row - 1, Column - 1));

            if (Row - 1 >= 0 && Column + 1 <= 7) // gore desno
                if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(Row - 1, Column + 1)]].Piece == null)
                    allDiags.Add(new(Row - 1, Column + 1));
                else if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(Row - 1, Column + 1)]].Piece
                             .Color != Color)
                    allDiags.Add(new(Row - 1, Column + 1));

            if (Row + 1 <= 7 && Column - 1 >= 0) // dole levo
                if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(Row + 1, Column - 1)]].Piece == null)
                    allDiags.Add(new(Row + 1, Column - 1));
                else if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(Row + 1, Column - 1)]].Piece
                             .Color != Color)
                    allDiags.Add(new(Row + 1, Column - 1));

            if (Row + 1 <= 7 && Column + 1 <= 7) // dole desno
                if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(Row + 1, Column + 1)]].Piece == null)
                    allDiags.Add(new(Row + 1, Column + 1));
                else if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(Row + 1, Column + 1)]].Piece
                             .Color != Color)
                    allDiags.Add(new(Row + 1, Column + 1));

            return allDiags;
        }

        private List<KeyValuePair<int, int>> GetAllHorizontalsAndVerticals(List<ChessSquare> board)
        {
            var allDiags = new List<KeyValuePair<int, int>>(4);

            if (Row - 1 >= 0) // gore
                if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(Row - 1, Column)]].Piece == null)
                    allDiags.Add(new(Row - 1, Column));
                else if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(Row - 1, Column)]].Piece
                             .Color != Color)
                    allDiags.Add(new(Row - 1, Column));

            if (Row + 1 <= 7) // dole
                if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(Row + 1, Column)]].Piece == null)
                    allDiags.Add(new(Row + 1, Column));
                else if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(Row + 1, Column)]].Piece
                             .Color != Color)
                    allDiags.Add(new(Row + 1, Column));

            if (Column + 1 <= 7) // desno
                if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(Row, Column + 1)]].Piece == null)
                    allDiags.Add(new(Row, Column + 1));
                else if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(Row, Column + 1)]].Piece
                             .Color != Color)
                    allDiags.Add(new(Row, Column + 1));

            if (Column - 1 >= 0) // levo
                if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(Row, Column - 1)]].Piece == null)
                    allDiags.Add(new(Row, Column - 1));
                else if (board[Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(Row, Column - 1)]].Piece
                             .Color != Color)
                    allDiags.Add(new(Row, Column - 1));

            return allDiags;
        }
    }
}
