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

        //method to calculate all possible moves by a piece - returns a list
        public List<int> generatePossibleMoves()
        {
            List<int> moves = new List<int>();


            return moves;
        }
    }
}
