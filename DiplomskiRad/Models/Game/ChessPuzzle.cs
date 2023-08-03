using DiplomskiRad.Models.Enums;
using System.Collections.Generic;

namespace DiplomskiRad.Models.Game
{
    public class ChessPuzzle
    {
        public List<string> WhitePos { get; set; }
        public List<string> BlackPos { get; set; }
        public Color FirstMove { get; set; }
        public List<string> MoveOrder { get; set; }

        public ChessPuzzle(List<string> whitePos, List<string> blackPos, Color firstMove, List<string> moveOrder)
        {
            WhitePos = whitePos;
            BlackPos = blackPos;
            FirstMove = firstMove;
            MoveOrder = moveOrder;
        }
    }
}
