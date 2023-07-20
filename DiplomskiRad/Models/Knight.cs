using DiplomskiRad.Helper;
using DiplomskiRad.Models.Enums;
using System.Collections.Generic;
using System.Linq;

namespace DiplomskiRad.Models
{
    public class Knight : Piece
    {
        public Knight(Color color, int row, int column) : base("Knight", 3, color, PieceType.Knight, row, column) { }

        public override List<ushort> GetPossibleMoves(ChessSquare chessSquare, List<ChessSquare> board)
        {
            int[] rows = new int[4] { Row - 2, Row - 1, Row + 1, Row + 2 };
            int[] columns = new int[4] { Column - 2, Column - 1, Column + 1, Column + 2 };

            var allPossibleMoves = new List<ushort>(8);

            foreach (var row in rows)
                foreach (var t2 in columns)
                    if (Mapping.DoubleIndexToCoordinate.ContainsKey(new KeyValuePair<int, int>(row, t2)))
                        allPossibleMoves.Add(Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(row, t2)]);

            RemoveDiagonals(ref allPossibleMoves);

            var moves = new List<ushort>(8);

            foreach (var pos in allPossibleMoves)
                if (board[pos].Piece == null) moves.Add(pos);
                else if (board[pos].Piece.Color != Color) moves.Add(pos);
            
            return moves;
        }

        private void RemoveDiagonals(ref List<ushort> allPossibleMoves)
        {
            var allDiags = new List<KeyValuePair<int, int>>(8)
            {
                new(Row - 1, Column - 1), new(Row - 2, Column - 2), // gore levo
                new(Row - 1, Column + 1), new(Row - 2, Column + 2), // gore desno
                new(Row + 1, Column - 1), new(Row + 2, Column - 2), // dole levo
                new(Row + 1, Column + 1), new(Row + 2, Column + 2), // dole desno
            };

            foreach (var diag in allDiags.Where(diag => Mapping.DoubleIndexToIndex.ContainsKey(diag)))
                if (allPossibleMoves.Contains(Mapping.DoubleIndexToIndex[diag]))
                    allPossibleMoves.Remove(Mapping.DoubleIndexToIndex[diag]);
        }
    }
}
