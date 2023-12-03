using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeDimensionalChess
{
    class Rook : Piece
    {
        //constructor
        public Rook(int startPosition, int colour) : base(startPosition, colour) { }

        public override string GetPieceType() { return "R"; }

        //implementation of generate possible moves, doing this recursively because rooks can move to the edge of the board
        public override List<int> GeneratePossibleMoves(List<Square> board, List<Piece> pieces)
        {
            List<int> moves = new List<int>();

            for (int direction = (int)Directions.Right; direction <= (int)Directions.Backwards; direction++)
            {
                List<int> tmp = new List<int>();
                //calls recursive directional move generator
                tmp = GenerateNextMove(direction, board, currentPosition, pieces);
                //appends all moves generated into moves list
                for (int x = 0; x < tmp.Count(); x++)
                {
                    //filter out when invalid moves have been reached
                    if (tmp[x] != -1) { moves.Add(tmp[x]); }
                }
            }

            return moves;
        }

        private List<int> GenerateNextMove(int dir, List<Square> board, int pos, List<Piece> pieces)
        {
            int[] vect = ConvertPtrToVect(pos);
            //converts direction into an index that addresses the relevant part of a 3-Length Vector Array: (0&1)-> 0, (2&3)-> 1, (4&5) -> 2
            int arrayIndex = (dir / 2) % 3;


            //switch to transform position in direction specified - will be using base 8 things
            switch (dir)
            {
                case (int)Directions.Right:
                    //moves right one square
                    pos++;
                    vect[arrayIndex]++;
                    break;
                case (int)Directions.Left:
                    //moves left one square
                    pos--;
                    vect[arrayIndex]--;
                    break;
                case (int)Directions.Up:
                    //moves up one square
                    pos += 8;
                    vect[arrayIndex]++;
                    break;
                case (int)Directions.Down:
                    //moves down one square
                    pos -= 8;
                    vect[arrayIndex]--;
                    break;
                case (int)Directions.Forwards:
                    //moves deeper on z axis by one square
                    pos += 64;
                    vect[arrayIndex]++;
                    break;
                case (int)Directions.Backwards:
                    //moves back on z axis by on square
                    pos -= 64;
                    vect[arrayIndex]--;
                    break;
            }

            List<int> moves = new List<int>();

            //checks that piece hasn't gone off the edge of the board
            if (vect[arrayIndex] < Constants.boardDimensions && vect[arrayIndex] > -1)
            {
                //checks if there is a piece on the square
                int targetPtr = board[pos].GetPiecePointer();
                if (targetPtr != -1)
                {
                    //now check if piece is friendly or enemy
                    Piece target = pieces[targetPtr];
                    if (target.GetColour() != colour)
                    {
                        //unwind recursion from here
                        moves.Add(pos);
                        return moves;
                    } // return a -1 to be filtered out later
                    else
                    {
                        moves.Add(-1);
                        return moves;
                    }
                }
                else
                {
                    moves.Add(pos);
                    List<int> newMoves = new List<int>();
                    newMoves = GenerateNextMove(dir, board, pos, pieces);
                    //go deeper in recursion here
                    for (int x = 0; x < newMoves.Count(); x++)
                    {
                        moves.Add(newMoves[x]);
                    }
                }

            } // return a -1 to be filtered out later, prevent null errors
            else { moves.Add(-1); }

            return moves;
        }

        //used for queen when handling internal move
        public void ForceMove(int endPosition)
        {
            currentPosition = endPosition;
        }
    }
}
