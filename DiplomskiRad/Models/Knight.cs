using DiplomskiRad.Models.Enums;

namespace DiplomskiRad.Models
{
    public class Knight : Piece
    {
        public Knight(Color color, int row, int column) : base("Knight", 3, color, PieceType.Knight, row, column) { }
    }
}
