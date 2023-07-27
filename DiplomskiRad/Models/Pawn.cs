using DiplomskiRad.Helper;
using DiplomskiRad.Models.Enums;
using System.Collections.Generic;
using System.Linq;

namespace DiplomskiRad.Models
{
    public class Pawn : Piece
    {
        public bool IsFirstMove {get; set; }

        public Pawn(Color color, int row, int column) : base("Pawn", 1, color, PieceType.Pawn, row, column) => IsFirstMove = true;

        public override List<ushort> GetPossibleMoves(ChessSquare chessSquare, List<ChessSquare> board)
        {
            var moves = new List<ushort>(4);

            var allPossibleAdvances = GetAllPossibleAdvances();

            if (allPossibleAdvances.Count == 2)
            {
                if (board[allPossibleAdvances[0]].Piece == null)
                {
                    moves.Add(allPossibleAdvances[0]);
                    if (board[allPossibleAdvances[1]].Piece == null) moves.Add(allPossibleAdvances[1]);
                }
            }
            else if (board[allPossibleAdvances[0]].Piece == null) moves.Add(allPossibleAdvances[0]);

            moves.AddRange(GetAllPossibleCaptures().Where(pos => board[pos].Piece != null && board[pos].Piece.Color != chessSquare.Piece.Color));

            IsKingInCheck(moves, board, chessSquare, Color);







            //kasnije uradi za en passant i proveru za sah al ima do toga vremena

            return moves;
        }

        private List<ushort> GetAllPossibleAdvances()
        {
            var retValCoord = new List<string>(2);
            int rowOffset = (Color == Color.White) ? -1 : 1;

            retValCoord.Add(Mapping.DoubleIndexToCoordinate[new KeyValuePair<int, int>(Row + rowOffset, Column)]);
            if (IsFirstMove) retValCoord.Add(Mapping.DoubleIndexToCoordinate[new KeyValuePair<int, int>(Row + rowOffset * 2, Column)]);

            var retValIndex = new List<ushort>(retValCoord.Count);
            retValIndex.AddRange(retValCoord.Select(x => Mapping.CoordinateToIndex[x]));

            return retValIndex;
        }

        private List<ushort> GetAllPossibleCaptures()
        {
            var retValCoord = new List<string>(2);
            int rowOffset = (Color == Color.White) ? -1 : 1;

            switch (Column)
            {
                case 0:
                    retValCoord.Add(Mapping.DoubleIndexToCoordinate[new KeyValuePair<int, int>(Row + rowOffset, Column + 1)]);
                    break;
                case 7:
                    retValCoord.Add(Mapping.DoubleIndexToCoordinate[new KeyValuePair<int, int>(Row + rowOffset, Column - 1)]);
                    break;
                default:
                    retValCoord.Add(Mapping.DoubleIndexToCoordinate[new KeyValuePair<int, int>(Row + rowOffset, Column + 1)]);
                    retValCoord.Add(Mapping.DoubleIndexToCoordinate[new KeyValuePair<int, int>(Row + rowOffset, Column - 1)]);
                    break;
            }

            var retValIndex = new List<ushort>(retValCoord.Count);
            retValIndex.AddRange(retValCoord.Select(x => Mapping.CoordinateToIndex[x]));

            return retValIndex;
        }
    }
}
