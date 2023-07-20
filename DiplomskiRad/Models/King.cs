using DiplomskiRad.Models.Enums;
using System.Collections.Generic;

namespace DiplomskiRad.Models
{
    public class King : Piece
    {
        public King(Color color, int row, int column) : base("King", ushort.MaxValue, color, PieceType.King, row, column) { }

        public override List<ushort> GetPossibleMoves(ChessSquare chessSquare, List<ChessSquare> board)
        {
            return base.GetPossibleMoves(chessSquare, board);
        }
    }
}
