using DiplomskiRad.Models.Enums;

namespace DiplomskiRad.Models
{
    public class Queen : Piece
    {
        public Queen(Color color, int row, int column) : base("Queen", 8, color, PieceType.Pawn, row, column) { }
    }
}
