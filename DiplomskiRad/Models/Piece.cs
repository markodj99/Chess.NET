using DiplomskiRad.Models.Enums;
using System.Collections.Generic;
using System.Linq;
using DiplomskiRad.Helper;
using System.Data.Common;
using System.Xml.Linq;

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

        public Piece(Piece other)
        {
            this.Name = other.Name;
            this.Value = other.Value;
            this.Color = other.Color;
            this.Type = other.Type;
            this.Row = other.Row;
            this.Column = other.Column;
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
