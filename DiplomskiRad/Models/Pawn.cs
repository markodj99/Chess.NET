using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomskiRad.Models
{
    public class Pawn : Piece
    {
        public Pawn(string color) : base("Pawn", 1, color) { }
    }
}
