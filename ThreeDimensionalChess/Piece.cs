using System;
using System.Linq;

namespace ThreeDimensionalChess
{
    //neat little enum to help with generating moves
    enum Directions
    {
        Right, // 0- Positive movement on x
        Left, // 1 - Negative Movement on X
        Up, // 2 - Positive movement on Y
        Down, // 3 - Negative movement on y
        Forwards, // 4 - Positive movement on z
        Backwards, // 5 - Negative movement on z
    }

    abstract class Piece
    {
        //store movement as a three dimensional vector [x, y, z] - can be implemented in constructor of a piece, some don't need this
        int[] movementVect = new int[3];
        //stores current position on board
        int currentPosition;
        int colour;

        //constructor for piece
        public Piece(int startPos, int col)
        {
            currentPosition = startPos;
            colour = col;
        }

        //method to calculate all possible moves by a piece - returns a list, must be implemented on a per piece basis
        public abstract List<int> generatePossibleMoves(List<Square> board);

        //self explanatory names, useful for eachother
        public int[] convertPtrToVect(int inp)
        {
            //uses modular arithmetic around base 8 to convert a position in the list of squares to a 3D coordinate/vector of the piece's position
            int[] vect = { 0, 0, 0 };
            int remainder = inp % 64;
            vect[2] = inp / 64;
            vect[1] = remainder / 8;
            vect[0] = remainder % 8;

            return vect;
        }

        public int convertVectToPtr(int[] arr)
        {
            // if this method doesn't receive a three dimensional vector in array form, it will throw an argument exception - thankfully this should never happen
            if(arr.Length != 3){ throw new ArgumentException(); }
            int[] currentPosVect = convertPtrToVect(currentPosition);
            int movePtr = 0;
            
            // uses base 8 system to convert three dimensional vectors into pointers for the list of board squares
            movePtr += arr[0];
            movePtr += arr[1] * 8;
            movePtr += arr[2] * 64;

            return movePtr;
        }

        public int getColour()
        {
            return colour;
        }
    }
}
