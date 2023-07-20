using DiplomskiRad.Models.Enums;

namespace DiplomskiRad.Models
{
    public class Bishop : Piece
    {
        public Bishop(Color color, int row, int column) : base("Bishop", 3, color, PieceType.Bishop, row, column) { }
    }
}
