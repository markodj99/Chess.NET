using DiplomskiRad.Models.Enums;
using System.Collections.Generic;

namespace DiplomskiRad.Models
{
    public class Queen : Piece
    {
        public Queen(Color color, int row, int column) : base("Queen", 8, color, PieceType.Pawn, row, column) { }

        public override List<ushort> GetPossibleMoves(ChessSquare chessSquare, List<ChessSquare> board)
        {
            return base.GetPossibleMoves(chessSquare, board);
        }
    }
}
