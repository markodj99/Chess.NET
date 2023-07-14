using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomskiRad.Models
{
    public class King : Piece
    {
        public King(string color) : base("King", ushort.MaxValue, color) { }
    }
}
