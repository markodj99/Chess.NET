using DiplomskiRad.Models.Enums;

namespace DiplomskiRad.Models
{
    public class King : Piece
    {
        public King(Color color, int row, int column) : base("King", ushort.MaxValue, color, PieceType.King, row, column) { }
    }
}
