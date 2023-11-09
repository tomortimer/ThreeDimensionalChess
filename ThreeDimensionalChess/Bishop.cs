using Microsoft.VisualBasic;
using System;

namespace ThreeDimensionalChess
{
    class Bishop : Piece
    {
        public Bishop(int startPosition, int colour) : base(startPosition, colour) { }

        public override string getPieceType() { return "B"; }

        public override List<int> generatePossibleMoves(List<Square> board, List<Piece> pieces)
        {
            List<int> moves = new List<int>();

            //loop around recursive move generator, bishops can move in 12 directions
            for (int direction = 0; direction < 12; direction++)
            {
                List<int> tmp = new List<int>();
                tmp = generateNextMove(direction, board, currentPosition, pieces);

                //append generate moves to main list
                for (int x = 0; x < tmp.Count(); x++)
                {
                    //filter out invalid moves
                    if (tmp[x] != -1) { moves.Add(tmp[x]); }
                }
            }

            return moves;
        }

        private List<int> generateNextMove(int dir, List<Square> board, int pos, List<Piece> pieces)
        {
            int[] vect = convertPtrToVect(pos);

            //large switch to transform piece, going clockwise around each board, front -> top -> side
            switch (dir)
            {
                case 0:
                    pos += 9;
                    vect[0]++;
                    vect[1]++;
                    break;
                case 1:
                    pos -= 7;
                    vect[0]++;
                    vect[1]--;
                    break;
                case 2:
                    pos -= 9;
                    vect[0]--;
                    vect[1]--;
                    break;
                case 3:
                    pos += 7;
                    vect[0]--;
                    vect[1]++;
                    break;
                //moves from top view
                case 4:
                    pos += 65;
                    vect[0]++;
                    vect[2]++;
                    break;
                case 5:
                    pos -= 63;
                    vect[0]++;
                    vect[2]--;
                    break;
                case 6:
                    pos -= 65;
                    vect[0]--;
                    vect[2]--;
                    break;
                case 7:
                    pos += 63;
                    vect[0]--;
                    vect[2]++;
                    break;
                //moves from side view
                case 8:
                    pos += 72;
                    vect[1]++;
                    vect[2]++;
                    break;
                case 9:
                    pos += 56;
                    vect[1]--;
                    vect[2]++;
                    break;
                case 10:
                    pos -= 72;
                    vect[1]--;
                    vect[2]--;
                    break;
                case 11:
                    pos -= 56;
                    vect[1]++;
                    vect[2]--;
                    break;
            }

            List<int> moves = new List<int>();

            //check that the piece hasn't gone off the board
            if (vect[0] < Constants.boardDimensions && vect[0] > -1 && vect[1] < Constants.boardDimensions && vect[1] > -1 && vect[2] < Constants.boardDimensions && vect[2] > -1)
            {
                //checks if there is a piece on the square
                int targetPtr = board[pos].getPiecePointer();
                if (targetPtr != -1)
                {
                    Piece target = pieces[targetPtr];
                    if (target.getColour() != colour)
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
                    newMoves = generateNextMove(dir, board, pos, pieces);
                    //go deeper in recursion here
                    for (int x = 0; x < newMoves.Count(); x++)
                    {
                        moves.Add(newMoves[x]);
                    }
                }
            }
            else { moves.Add(-1); }

            return moves;
        }

        //used for queen when handling internal move
        public void forceMove(int endPosition)
        {
            currentPosition = endPosition;
        }
    }
}
