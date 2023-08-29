using DiplomskiRad.Helper;
using DiplomskiRad.Models.Enums;
using DiplomskiRad.Models.Game;
using System.Collections.Generic;
using System.Linq;

namespace DiplomskiRad.Models.Pieces
{
    public class Knight : Piece
    {
        public Knight(Color color) : base("Knight", 3, color, PieceType.Knight) { }

        public override List<ushort> GetPossibleMoves(ChessSquare chessSquare, List<ChessSquare> board, int enPassantPosibleSquare = -1)
        {
            int row = chessSquare.Row;
            int column = chessSquare.Column;

            int[] rows = new int[4] { row - 2, row - 1, row + 1, row + 2 };
            int[] columns = new int[4] { column - 2, column - 1, column + 1, column + 2 };

            var allPossibleMoves = new List<ushort>(8);

            foreach (var roww in rows)
                foreach (var t2 in columns)
                    if (Mapping.DoubleIndexToCoordinate.ContainsKey(new KeyValuePair<int, int>(roww, t2)))
                        allPossibleMoves.Add(Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(roww, t2)]);

            RemoveDiagonals(ref allPossibleMoves, row, column);

            var moves = new List<ushort>(8);

            foreach (var pos in allPossibleMoves)
                if (board[pos].Piece == null) moves.Add(pos);
                else if (board[pos].Piece.Color != Color) moves.Add(pos);

            return moves;
        }

        private void RemoveDiagonals(ref List<ushort> allPossibleMoves, int row, int column)
        {
            var allDiags = new List<KeyValuePair<int, int>>(8)
            {
                new(row - 1, column - 1), new(row - 2, column - 2), // gore levo
                new(row - 1, column + 1), new(row - 2, column + 2), // gore desno
                new(row + 1, column - 1), new(row + 2, column - 2), // dole levo
                new(row + 1, column + 1), new(row + 2, column + 2), // dole desno
            };

            foreach (var diag in allDiags.Where(diag => Mapping.DoubleIndexToIndex.ContainsKey(diag)))
                if (allPossibleMoves.Contains(Mapping.DoubleIndexToIndex[diag]))
                    allPossibleMoves.Remove(Mapping.DoubleIndexToIndex[diag]);
        }
    }
}
