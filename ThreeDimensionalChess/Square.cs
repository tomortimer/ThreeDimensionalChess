using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeDimensionalChess
{
    enum Colours
    {
        Black, // 0
        White, // 1
        BlackBlue, // 2 - Used for when a friendly move may go onto a black square
        WhiteBlue, // 3 - Used for when a friendly move may go onto a white square
        BlackRed, // 4 - Used for when an enemy move may go onto a black square
        WhiteRed // 5 - Used for when an enemy move may go onto a white square  
    }

    class Square
    {
        private int squareColour;
        //points to current piece in list
        private int piecePointer;

        //constructor of board should intialise squares in for loop and pass colour values
        public Square(int colour)
        {
            squareColour = colour;
            //-1 represents a lack of piece - converter for data binding will handle the displaying of this
            piecePointer = -1;
        }

        public int getPiecePointer() { return piecePointer;}
        public void setPiecePointer(int ptr) { piecePointer = ptr; }
    }
}
