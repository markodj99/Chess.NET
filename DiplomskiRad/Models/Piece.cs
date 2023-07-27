using DiplomskiRad.Models.Enums;
using System.Collections.Generic;
using System.Linq;
using DiplomskiRad.Helper;

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

        public bool IsKingInCheck(List<ushort> moves, List<ChessSquare> board, ChessSquare chessSquare, Color playerColor)
        {
            var initialPiecePosition = Mapping.DoubleIndexToIndex[new KeyValuePair<int, int>(chessSquare.Row, chessSquare.Column)];
            foreach (var move in moves)
            {
                board[move] = board[initialPiecePosition];

                ChessSquare kingSquare;
                foreach (var piece in board.Where(piece => piece.Piece.Type == PieceType.King && piece.Color.Equals(playerColor.ToString()))) { kingSquare = piece; break;}

                foreach (var piece in board)
                {
                    if (!piece.Color.Equals(playerColor.ToString()) && piece.Piece != null)
                    {
                        piece.Piece.Type switch
                        {
                            PieceType.King => new King(Color.White, coordinates.Key, coordinates.Value),
                            PieceType.Queen => new Queen(Color.White, coordinates.Key, coordinates.Value),
                            PieceType.Rook => new Rook(Color.White, coordinates.Key, coordinates.Value),
                            PieceType.Bishop => new Bishop(Color.White, coordinates.Key, coordinates.Value),
                            PieceType.Knight => new Knight(Color.White, coordinates.Key, coordinates.Value),
                            PieceType.Pawn => new Pawn(Color.White, coordinates.Key, coordinates.Value),
                            _ => new Piece("ime", 4, Color.Black, PieceType.Bishop, 0, 0),
                        };
                    }
                }

            }

            return true;
        }



    }
}
