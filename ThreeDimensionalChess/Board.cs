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

        public Board()
        {
            //intialise all 512 squares of the 3D board
            for(int x = 0; x < 512; x++)
            {
                //adds a new square to the board, performs mod on x to get colour, Colours enum is stored in Square.cs even squares are black, odd are white
                board.Add(new Square(x % 2));
            }
        }
    }
}
