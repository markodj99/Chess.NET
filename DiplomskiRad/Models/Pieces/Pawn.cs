using DiplomskiRad.Helper;
using DiplomskiRad.Models.Enums;
using DiplomskiRad.Models.Game;
using System.Collections.Generic;
using System.Linq;

namespace DiplomskiRad.Models.Pieces
{
    public class Pawn : Piece
    {
        public bool IsFirstMove { get; set; }

        public Pawn(Color color, bool isFirstMove) : base("Pawn", 1, color, PieceType.Pawn) => IsFirstMove = isFirstMove;

        public override List<int> GetPossibleMoves(ChessSquare chessSquare, List<ChessSquare> board, int enPassantPosibleSquare = -1)
        {
            int row = chessSquare.Row;
            int column = chessSquare.Column;

            var moves = new List<int>(5);

            var allPossibleAdvances = GetAllPossibleAdvances(row, column);

            if (allPossibleAdvances.Count == 2)
            {
                if (board[allPossibleAdvances[0]].Piece == null)
                {
                    moves.Add(allPossibleAdvances[0]);
                    if (board[allPossibleAdvances[1]].Piece == null)
                    {
                        moves.Add(allPossibleAdvances[1]);
                    }
                }
            }
            else if (board[allPossibleAdvances[0]].Piece == null)
            {
                moves.Add(allPossibleAdvances[0]);
            }

            if (enPassantPosibleSquare > 0)
            {
                int move = PossibleEnPassant(chessSquare, board, enPassantPosibleSquare);
                if (move != 70)
                {
                    moves.Add(move);
                }
            }

            moves.AddRange(GetAllPossibleCaptures(row, column).Where(pos => board[pos].Piece != null && board[pos].Piece.Color != chessSquare.Piece.Color));
            return moves;
        }

        private List<int> GetAllPossibleAdvances(int row, int column)
        {
            var retValCoord = new List<string>(2);
            int rowOffset = Color == Color.White ? -1 : 1;

            retValCoord.Add(Mapping.DoubleIndexToCoordinate[new KeyValuePair<int, int>(row + rowOffset, column)]);
            if (IsFirstMove)
            {
                retValCoord.Add(Mapping.DoubleIndexToCoordinate[new KeyValuePair<int, int>(row + rowOffset * 2, column)]);
            }

            var retValIndex = new List<int>(retValCoord.Count);
            retValIndex.AddRange(retValCoord.Select(x => Mapping.CoordinateToIndex[x]));

            return retValIndex;
        }

        private List<int> GetAllPossibleCaptures(int row, int column)
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

            var retValIndex = new List<int>(retValCoord.Count);
            retValIndex.AddRange(retValCoord.Select(x => Mapping.CoordinateToIndex[x]));

            return retValIndex;
        }

        private int PossibleEnPassant(ChessSquare chessSquare, List<ChessSquare> board, int enPassantPosibleSquare)
        {
            if (board[enPassantPosibleSquare].Piece.Color != chessSquare.Piece.Color)
            {
                if (chessSquare.Row == board[enPassantPosibleSquare].Row)
                {
                    int rowOffset = Color == Color.White ? -1 : 1;

                    return Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(chessSquare.Row + rowOffset, board[enPassantPosibleSquare].Column)];
                }
            }

            return 70; 
        }
    }
}
