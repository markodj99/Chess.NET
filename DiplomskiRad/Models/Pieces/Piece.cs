using DiplomskiRad.Models.Enums;
using System.Collections.Generic;
using System.Linq;
using DiplomskiRad.Helper;
using System.Data.Common;
using System.Xml.Linq;
using DiplomskiRad.Models.Game;

namespace DiplomskiRad.Models.Pieces
{
    public class Piece
    {
        public string Name { get; }
        public PieceType Type { get; }
        public ushort Value { get; }
        public Color Color { get; }

        public Piece(string name, ushort value, Color color, PieceType type)
        {
            Name = name;
            Value = value;
            Color = color;
            Type = type;
        }

        public Piece(Piece other)
        {
            Name = other.Name;
            Value = other.Value;
            Color = other.Color;
            Type = other.Type;
        }

        public virtual List<int> GetPossibleMoves(ChessSquare chessSquare, List<ChessSquare> board, int enPassantPosibleSquare = -1) => new(0);

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            var other = (Piece)obj;
            return Name.Equals(other.Name) && Value == other.Value && Color.Equals(other.Color);
        }
    }
}
