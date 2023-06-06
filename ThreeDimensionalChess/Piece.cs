using System;
using System.Linq;

namespace ThreeDimensionalChess
{
    abstract class Piece
    {
        //store movement as a three dimensional vector
        int[] movementVect = new int[3];
        //boolean to store if piece can move backwards
        bool vectCanBeNegated;
        //stores current position on board
        int currentPosition;

        //constructor for piece
        public Piece(int[] vector, bool negation, int startPos)
        {
            movementVect = vector;
            vectCanBeNegated = negation;
            currentPosition = startPos;
        }

        //method to calculate all possible moves by a piece - returns a list, must be implemented on a per piece basis
        abstract public List<int> generatePossibleMoves();

        private int convertVectToPtr(int[] arr)
        {
            // if this method doesn't receive a three dimensional vector in array form, it will throw an argument exception - thankfully this should never happen
            if(arr.Length != 3){ throw new ArgumentException(); }

            // uses base 8 system to convert three dimensional vectors into pointers for the list of board squares
            int movePtr = 0;
            movePtr += arr[0];
            movePtr += arr[1] * 8;
            movePtr += arr[2] * 64;
            return movePtr;
        }
    }
}
