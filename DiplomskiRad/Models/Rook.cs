using DiplomskiRad.Models.Enums;

namespace DiplomskiRad.Models
{
    public class Rook : Piece
    {
        public Rook(Color color, int row, int column) : base("Rook", 5, color, PieceType.Rook, row, column) { }
    }
}
