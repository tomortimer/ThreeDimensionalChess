using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeDimensionalChess
{
    class Rook : Piece
    {
        //constructor
        public Rook(bool negation, int startPosition, int colour) : base(negation, startPosition, colour) { }

        //implementation of generate possible moves, doing this recursively because rooks can move to the edge of the board
        public override List<int> generatePossibleMoves(List<Square> board, List<Piece> pieces)
        {
            List<int> moves = new List<int>();

            for(int direction = 0; direction <= (int)Directions.Backwards; direction++)
            {

            }

            return moves;
        }

        private List<int> generateNextMove(int dir, List<Square> board, int pos, List<Piece> pieces)
        {
            //switch to transform position in direction specified - will be using base 8 things
            switch (dir)
            {
                case (int)Directions.Right:
                    //moves right one square
                    pos++;
                    break;
                case (int)Directions.Left:
                    //moves left one square
                    pos--;
                    break;
                case (int)Directions.Up:
                    //moves up one square
                    pos += 8;
                    break;
                case (int)Directions.Down:
                    //moves down one square
                    pos -= 8;
                    break;
                case (int)Directions.Forwards:
                    //moves deeper on z axis by one square
                    pos += 64;
                    break;
                case (int)Directions.Backwards:
                    //moves back on z axis by on square
                    pos -= 64;
                    break;
            }

            //converts direction into an index that addresses the relevant part of a 3-Length Vector Array: (0&1)-> 0, (2&3)-> 1, (4&5) -> 2
            int arrayIndex = (dir / 2) % 3;

            int[] vect = convertPtrToVect(pos);

            //checks that piece hasn't gone off the edge of the board
            if(vect[arrayIndex] < 8 && vect[arrayIndex] > -1)
            {
                //checks if there is a piece on the square
                int targetPtr = board[pos].getPiecePointer();
                if(targetPtr != -1)
                {
                    //now check if piece is friendly or enemy
                    Piece target = pieces[targetPtr];
                    if(target.getColour)
                }
            }
        }
    }
}
