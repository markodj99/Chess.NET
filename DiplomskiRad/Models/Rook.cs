using DiplomskiRad.Models.Enums;
using System.Collections.Generic;

namespace DiplomskiRad.Models
{
    public class Rook : Piece
    {
        public Rook(Color color, int row, int column) : base("Rook", 5, color, PieceType.Rook, row, column) { }

        public override List<ushort> GetPossibleMoves(ChessSquare chessSquare, List<ChessSquare> board)
        {
            return base.GetPossibleMoves(chessSquare, board);
        }
    }
}
