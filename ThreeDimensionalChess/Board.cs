using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeDimensionalChess
{
    class Board
    {
        //one dimensional list used to store board - used modular arithmetic on vectors to navigate it
        private List<Square> board = new List<Square>();
        private List<Piece> pieces = new List<Piece>();

        public Board()
        {
            //intialise all 512 squares of the 3D board
            for(int x = 0; x < 512; x++)
            {
                //adds a new square to the board, performs mod on x to get colour, Colours enum is stored in Square.cs even squares are black(with 0 also being black), odd are white
                board.Add(new Square(x % 2));
            }
        }
        public void addPiece(string type, int pos, int colour)
        {
            Piece p = null;
            switch (type)
            {
                case "Rook":
                    p = new Rook(pos, colour);
                    break;
            }

            pieces.Add(p);

            board[pos].setPiecePointer(pieces.Count() - 1);
        }
    }
}
