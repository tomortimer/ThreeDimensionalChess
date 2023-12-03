using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeDimensionalChess
{
    class Pawn : Piece
    {

        public Pawn(int position, int colour) : base(position, colour) { }

        public override string GetPieceType()
        {
            return "P";
        }

        public override List<int> GeneratePossibleMoves(List<Square> board, List<Piece> pieces)
        {
            List<int> moves = new List<int>();
            //move in opposite directions based on colour
            //uses private method, makes move simpler to program
            moves = GenerateNextMove(currentPosition, board, pieces);

            //add capture moves here
            //RULE: pawns can make both advancing captures from top and front view, only one option for advancing capture is available on side view
            int[] posVect = ConvertPtrToVect(currentPosition);
            int pos = currentPosition;
            if (colour == (int)Colours.White)
            {
                //edge check for top down advancing captures
                if (posVect[0] != 0 && posVect[2] != 7)
                {
                    int target = pos + 64 - 1;
                    int targetPiecePtr = board[target].GetPiecePointer();
                    if (targetPiecePtr != -1)
                    {
                        if (pieces[targetPiecePtr].GetColour() != colour) { moves.Add(target); }
                    }
                }
                if (posVect[0] != 7 && posVect[2] != 7)
                {
                    int target = pos + 64 + 1;
                    int targetPiecePtr = board[target].GetPiecePointer();
                    if (targetPiecePtr != -1)
                    {
                        if (pieces[targetPiecePtr].GetColour() != colour) { moves.Add(target); }
                    }
                }

                //edge check for front view advancing captures
                if (posVect[0] != 0 && posVect[1] != 7)
                {
                    int target = pos + 8 - 1;
                    int targetPiecePtr = board[target].GetPiecePointer();
                    if (targetPiecePtr != -1)
                    {
                        if (pieces[targetPiecePtr].GetColour() != colour) { moves.Add(target); }
                    }
                }
                if (posVect[0] != 7 && posVect[1] != 7)
                {
                    int target = pos + 8 + 1;
                    int targetPiecePtr = board[target].GetPiecePointer();
                    if (targetPiecePtr != -1)
                    {
                        if (pieces[targetPiecePtr].GetColour() != colour) { moves.Add(target); }
                    }
                }

                //edge check for side view advancing capture (singular)
                if (posVect[1] != 7 && posVect[2] != 7)
                {
                    int target = pos + 8 + 64;
                    int targetPiecePtr = board[target].GetPiecePointer();
                    if (targetPiecePtr != -1)
                    {
                        if (pieces[targetPiecePtr].GetColour() != colour) { moves.Add(target); }
                    }
                }
            }
            else
            {
                //if this doesn't work may need to switch signs on x transformations (y trans for last statement) becuase its hard to conceptualise
                //edge check for top down captures (black)
                if (posVect[0] != 7 && posVect[2] != 0)
                {
                    int target = pos - 64 + 1;
                    int targetPiecePtr = board[target].GetPiecePointer();
                    if (targetPiecePtr != -1)
                    {
                        if (pieces[targetPiecePtr].GetColour() != colour) { moves.Add(target); }
                    }
                }
                if (posVect[0] != 0 && posVect[2] != 0)
                {
                    int target = pos - 64 - 1;
                    int targetPiecePtr = board[target].GetPiecePointer();
                    if (targetPiecePtr != -1)
                    {
                        if (pieces[targetPiecePtr].GetColour() != colour) { moves.Add(target); }
                    }
                }

                //edge check for front view captures (black)
                if (posVect[0] != 7 && posVect[1] != 0)
                {
                    int target = pos - 8 + 1;
                    int targetPiecePtr = board[target].GetPiecePointer();
                    if (targetPiecePtr != -1)
                    {
                        if (pieces[targetPiecePtr].GetColour() != colour) { moves.Add(target); }
                    }
                }
                if (posVect[0] != 0 && posVect[1] != 0)
                {
                    int target = pos - 8 - 1;
                    int targetPiecePtr = board[target].GetPiecePointer();
                    if (targetPiecePtr != -1)
                    {
                        if (pieces[targetPiecePtr].GetColour() != colour) { moves.Add(target); }
                    }
                }

                //edge check for side view captures
                if (posVect[1] != 0 && posVect[2] != 0)
                {
                    int target = pos - 8 - 64;
                    int targetPiecePtr = board[target].GetPiecePointer();
                    if (targetPiecePtr != -1)
                    {
                        if (pieces[targetPiecePtr].GetColour() != colour) { moves.Add(target); }
                    }
                }
            }

            return moves;
        }

        private List<int> GenerateNextMove(int pos, List<Square> board, List<Piece> pieces)
        {
            List<int> ret = new List<int>();
            List<int[]> vects = new List<int[]>();

            //transform position, by one across and one up individually
            //direction depends on piece colour
            if (colour == (int)Colours.White)
            {
                //across move
                ret.Add(pos + 64);
                int[] tmp1 = ConvertPtrToVect(pos);
                tmp1[2]++;
                vects.Add(tmp1);

                //up move
                ret.Add(pos + 8);
                int[] tmp2 = ConvertPtrToVect(pos);
                tmp2[1]++;
                vects.Add(tmp2);
            }
            else
            {
                //across move
                ret.Add(pos - 64);
                int[] tmp1 = ConvertPtrToVect(pos);
                tmp1[2]--;
                vects.Add(tmp1);

                //up move
                ret.Add(pos - 8);
                int[] tmp2 = ConvertPtrToVect(pos);
                tmp2[1]--;
                vects.Add(tmp2);
            }

            List<int> retMoves = new List<int>();
            //edge check
            for (int i = 0; i < ret.Count(); i++)
            {
                //use statement add valid moves into another list, was having an error where lists were no longer parallel after removing nodes
                if (vects[i][2] > -1 && vects[i][2] < Constants.boardDimensions && vects[i][1] > -1 && vects[i][1] < Constants.boardDimensions)
                {
                    retMoves.Add(ret[i]);
                }
            }

            //occupied space check
            //written out twice because of same issue as above for loop
            if (retMoves.Count() == 2)
            {
                int piecePtr = board[retMoves[1]].GetPiecePointer();
                if (piecePtr != -1)
                {
                    //remove move because can't move forward onto piece
                    retMoves.RemoveAt(1);
                }
            }
            if (retMoves.Count() >= 1)
            {
                int piecePtr = board[retMoves[0]].GetPiecePointer();
                if (piecePtr != -1)
                {
                    //remove move because can't move forward onto piece
                    retMoves.RemoveAt(0);
                }
            }




            return retMoves;
        }
    }
}
