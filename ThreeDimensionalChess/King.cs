using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeDimensionalChess
{
    class King : Piece
    {
        public King(int startPosition, int colour) : base(startPosition, colour) { }

        public override string GetPieceType() { return "K"; }

        public override List<int> GeneratePossibleMoves(List<Square> board, List<Piece> pieces)
        {
            List<int> moves = new List<int>();

            //process up down and across moves here
            for (int dir = (int)Directions.Right; dir <= (int)Directions.Backwards; dir++)
            {
                int pos = currentPosition;
                int[] vect = ConvertPtrToVect(pos);
                //switch transforms position
                switch (dir)
                {
                    case (int)Directions.Right:
                        //moves right one square
                        pos++;
                        vect[0]++;
                        break;
                    case (int)Directions.Left:
                        //moves left one square
                        pos--;
                        vect[0]--;
                        break;
                    case (int)Directions.Up:
                        //moves up one square
                        pos += 8;
                        vect[1]++;
                        break;
                    case (int)Directions.Down:
                        //moves down one square
                        pos -= 8;
                        vect[1]--;
                        break;
                    case (int)Directions.Forwards:
                        //moves deeper on z axis by one square
                        pos += 64;
                        vect[2]++;
                        break;
                    case (int)Directions.Backwards:
                        //moves back on z axis by on square
                        pos -= 64;
                        vect[2]--;
                        break;
                }

                //check if piece has gone off edge here
                if (EdgeCheck(vect)) { moves.Add(pos); }
            }

            //process diagonal moves here
            for (int dir = 0; dir < 12; dir++)
            {
                int pos = currentPosition;
                int[] vect = ConvertPtrToVect(pos);
                //switch transforms position, collapse for readability, switch is in terms of 8 sided board, change later?
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

                //check if piece has gone off edge before adding to move of lists
                if (EdgeCheck(vect)) { moves.Add(pos); }
            }


            //loop to check if pieces on squares
            for (int x = 0; x < moves.Count(); x++)
            {
                int targetPos = board[moves[x]].GetPiecePointer();
                if (targetPos != -1)
                {
                    Piece targetPiece = pieces[targetPos];
                    //if piece of same colour on square remove move
                    if (targetPiece.GetColour() == colour)
                    {
                        moves.RemoveAt(x);
                        x--;
                    }
                }
            }

            return moves;
        }

        private bool EdgeCheck(int[] vect)
        {
            //returns true if move is safe
            bool safe = false;

            if (vect[0] > -1 && vect[0] < Constants.boardDimensions && vect[1] > -1 && vect[1] < Constants.boardDimensions && vect[2] > -1 && vect[2] < Constants.boardDimensions) { safe = true; }
            return safe;
        }
    }
}
