using DiplomskiRad.Models.Enums;
using System.Collections.Generic;
using System.Windows.Controls;

namespace DiplomskiRad.Models
{
    public class Piece
    {
        public string Name { get; }
        public PieceType Type { get;}
        public ushort Value { get; }
        public Color Color { get; }
        public int Row { get; set; }
        public int Column { get; set; }

        public Piece(string name, ushort value, Color color, PieceType type, int row, int column)
        {
            this.Name = name;
            this.Value = value;
            this.Color = color;
            this.Type = type;
            this.Row = row;
            this.Column = column;
        }

        public virtual List<ushort> GetPossibleMoves(ChessSquare chessSquare, List<ChessSquare> board) => new(0);

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            var other = (Piece)obj;
            return Name.Equals(other.Name) && Value == other.Value && Color.Equals(other.Color);
        }
    }
}
