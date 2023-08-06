using DiplomskiRad.Helper;
using DiplomskiRad.Models.Enums;
using DiplomskiRad.Models.Game;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace DiplomskiRad.Models.Pieces
{
    public class Pawn : Piece
    {
        public bool IsFirstMove { get; set; }

        public Pawn(Color color, bool isFirstMove) : base("Pawn", 1, color, PieceType.Pawn) => IsFirstMove = isFirstMove;

        public override List<ushort> GetPossibleMoves(ChessSquare chessSquare, List<ChessSquare> board)
        {
            int row = chessSquare.Row;
            int column = chessSquare.Column;

            var moves = new List<ushort>(4);

            var allPossibleAdvances = GetAllPossibleAdvances(row, column);

            if (allPossibleAdvances.Count == 2)
            {
                if (board[allPossibleAdvances[0]].Piece == null)
                {
                    moves.Add(allPossibleAdvances[0]);
                    if (board[allPossibleAdvances[1]].Piece == null) moves.Add(allPossibleAdvances[1]);
                }
            }
            else if (board[allPossibleAdvances[0]].Piece == null) moves.Add(allPossibleAdvances[0]);

            moves.AddRange(GetAllPossibleCaptures(row, column).Where(pos => board[pos].Piece != null && board[pos].Piece.Color != chessSquare.Piece.Color));
            return moves;
        }

        private List<ushort> GetAllPossibleAdvances(int row, int column)
        {
            var retValCoord = new List<string>(2);
            int rowOffset = Color == Color.White ? -1 : 1;

            retValCoord.Add(Mapping.DoubleIndexToCoordinate[new KeyValuePair<int, int>(row + rowOffset, column)]);
            if (IsFirstMove) retValCoord.Add(Mapping.DoubleIndexToCoordinate[new KeyValuePair<int, int>(row + rowOffset * 2, column)]);

            var retValIndex = new List<ushort>(retValCoord.Count);
            retValIndex.AddRange(retValCoord.Select(x => Mapping.CoordinateToIndex[x]));

            return retValIndex;
        }

        private List<ushort> GetAllPossibleCaptures(int row, int column)
        {
            var retValCoord = new List<string>(2);
            int rowOffset = Color == Color.White ? -1 : 1;

            switch (column)
            {
                case 0:
                    retValCoord.Add(Mapping.DoubleIndexToCoordinate[new KeyValuePair<int, int>(row + rowOffset, column + 1)]);
                    break;                                                                    
                case 7:                                                                       
                    retValCoord.Add(Mapping.DoubleIndexToCoordinate[new KeyValuePair<int, int>(row + rowOffset, column - 1)]);
                    break;                                                                  
                default:                                                                    
                    retValCoord.Add(Mapping.DoubleIndexToCoordinate[new KeyValuePair<int, int>(row + rowOffset, column + 1)]);
                    retValCoord.Add(Mapping.DoubleIndexToCoordinate[new KeyValuePair<int, int>(row + rowOffset, column - 1)]);
                    break;
            }

            var retValIndex = new List<ushort>(retValCoord.Count);
            retValIndex.AddRange(retValCoord.Select(x => Mapping.CoordinateToIndex[x]));

            return retValIndex;
        }
    }
}
