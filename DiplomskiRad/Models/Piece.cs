using System.Data.Common;

namespace DiplomskiRad.Models
{
    public class Piece
    {
        public string Name { get; }
        public ushort Value { get; }
        public string Color { get; }

        public Piece(string name, ushort value, string color)
        {
            this.Name = name;
            this.Value = value;
            this.Color = color;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            var other = (Piece)obj;
            return Name.Equals(other.Name) && Value == other.Value && Color.Equals(other.Color);
        }
    }
}
