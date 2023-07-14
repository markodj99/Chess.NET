using DiplomskiRad.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiplomskiRad.ViewModels
{
    public class ChessBoardViewModel
    {
        public ObservableCollection<ChessSquare> ChessSquares { get; set; }
        //private string[] initialPieceOrder = { "Rook", "Knight", "Bishop", "Queen", "King", "Bishop", "Knight", "Rook" };

        public ChessBoardViewModel()
        {
            ChessSquares = new ObservableCollection<ChessSquare>();
            for (int row = 0; row < 8; row++)
            {
                for (int column = 0; column < 8; column++)
                {
                    var square = new ChessSquare
                    {
                        Row = row,
                        Column = column,
                        Color = (row + column) % 2 == 0 ? "White" : "Black",
                        Piece = null
                    };

                    ChessSquares.Add(square);
                }
            }
        }

        private Piece GetInitialPiece(int column)
        {
            return null;
        }
    }
}
