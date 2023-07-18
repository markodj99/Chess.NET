namespace DiplomskiRad.Models
{
    public class Piece
    {
        protected string Name { get; }
        protected ushort Value { get; }
        protected string Color { get; }

        public Piece(string name, ushort value, string color)
        {
            this.Name = name;
            this.Value = value;
            this.Color = color;
        }
    }
}
