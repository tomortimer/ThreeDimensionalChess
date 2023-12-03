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
        WhiteRed, // 5 - Used for when an enemy move may go onto a white square
        BlackYellow, // 6 - Used for when a move has just gone to or from a black square
        WhiteYellow // 7 - Used for when a move has just gone to or from a white square
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

        public void SetSquareBlue() { squareColour = (int)Colours.BlackBlue; }

        public int GetPiecePointer() { return piecePointer; }
        public void SetPiecePointer(int ptr) { piecePointer = ptr; }

        public int GetColour() { return squareColour; }

        //change colour values of square
        public void UnderThreat(bool friendly)
        {
            //reset colour first
            NotUnderThreat();
            if (friendly)
            {
                squareColour += 2;
            }
            else
            {
                squareColour += 4;
            }
        }
        public void NotUnderThreat()
        {
            //returns square colour to normal
            //uses mod, since black squares are even, white squares are odd
            if (squareColour % 2 == 0)
            {
                squareColour = (int)Colours.Black;
            }
            else
            {
                squareColour = (int)Colours.White;
            }
        }

        public void PieceMoved()
        {
            //black squares are even, white squares are odd, shade yellow accordingly
            if (squareColour % 2 == 0)
            {
                squareColour = (int)Colours.BlackYellow;
            }
            else
            {
                squareColour = (int)Colours.WhiteYellow;
            }
        }

        public void DecrementPiecePointer()
        {
            piecePointer--;
        }
    }
}
