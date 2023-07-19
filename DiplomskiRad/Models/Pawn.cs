namespace DiplomskiRad.Models
{
    public class Pawn : Piece
    {
        public bool IsFirstMove {get; set; }
        public Pawn(string color) : base("Pawn", 1, color) => IsFirstMove = true;
    }
}
