using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomskiRad.Models
{
    public class ChessSquare
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public string Color { get; set; }
        public Piece Piece { get; set; }
    }
}
