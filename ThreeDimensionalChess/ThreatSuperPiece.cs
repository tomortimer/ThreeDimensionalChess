using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeDimensionalChess
{
    //the threat super piece is capable of making every possible move, but only returns the capture moves - because these capture moves will give the squarePointers of threatening pieces
    class ThreatSuperPiece : Piece
    {
        public ThreatSuperPiece(int startPos, int col) : base(startPos, col) { }
        public override string GetPieceType() { return "TSP"; }

        public override List<int> GeneratePossibleMoves(List<Square> board, List<Piece> pieces)
        {
            List<int> moves = new List<int>();

            //process rook moves here
            for (int direction = (int)Directions.Right; direction <= (int)Directions.Backwards; direction++)
            {
                List<int> tmpR = new List<int>();
                //calls recursive directional move generator
                tmpR = GenerateNextMoveRook(direction, board, currentPosition, pieces);
                //appends all moves Generated into moves list
                for (int x = 0; x < tmpR.Count(); x++)
                {
                    //filter out when invalid moves have been reached
                    if (tmpR[x] != -1) { moves.Add(tmpR[x]); }
                }
            }

            //process bishop moves here
            for (int direction = 0; direction < 12; direction++)
            {
                List<int> tmpB = new List<int>();
                tmpB = GenerateNextMoveBishop(direction, board, currentPosition, pieces);

                //append Generate moves to main list
                for (int x = 0; x < tmpB.Count(); x++)
                {
                    //filter out invalid moves
                    if (tmpB[x] != -1) { moves.Add(tmpB[x]); }
                }
            }

            //process knight moves here
            List<int> tmpN = new List<int>();
            tmpN = GenerateKnightMoves(board, pieces);
            //append generated moves to main list
            for (int x = 0; x < tmpN.Count(); x++)
            {
                moves.Add(tmpN[x]);
            }

            //process pawn moves here - only need to check captures, from reverse
            List<int> tmpP = new List<int>();
            tmpP = GeneratePawnCaptures(board, pieces);
            for (int x = 0; x < tmpP.Count(); x++)
            {
                moves.Add(tmpP[x]);
            }

            //process King moves here
            List<int> tmpK = new List<int>();
            tmpK = GenerateKingMoves(board, pieces);
            for (int x = 0; x < tmpK.Count(); x++)
            {
                moves.Add(tmpK[x]);
            }

            return moves;
        }

        public List<int> GenerateKingMoves(List<Square> board, List<Piece> pieces)
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

            List<int> finalMoves = new List<int>();
            //loop to check if pieces on squares
            for (int x = 0; x < moves.Count(); x++)
            {
                int targetPos = board[moves[x]].GetPiecePointer();
                if (targetPos != -1)
                {
                    //only add move if its capturing
                    if (pieces[targetPos].GetColour() != colour && pieces[targetPos].GetPieceType() == "K")
                    {
                        finalMoves.Add(moves[x]);
                    }
                }
            }

            return finalMoves;
        }

        private List<int> GenerateNextMoveRook(int dir, List<Square> board, int pos, List<Piece> pieces)
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
                    //also check if piece could make capturing moves
                    if (target.GetColour() != colour && (target.GetPieceType() == "R" || target.GetPieceType() == "Q"))
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
                    //moves.Add(pos);
                    //don't add moves unless its a capture
                    List<int> newMoves = new List<int>();
                    newMoves = GenerateNextMoveRook(dir, board, pos, pieces);
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

        private List<int> GenerateNextMoveBishop(int dir, List<Square> board, int pos, List<Piece> pieces)
        {
            int[] vect = ConvertPtrToVect(pos);

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
                int targetPtr = board[pos].GetPiecePointer();
                if (targetPtr != -1)
                {
                    Piece target = pieces[targetPtr];
                    //check piece could make move also
                    if (target.GetColour() != colour && (target.GetPieceType() == "Q" || target.GetPieceType() == "B"))
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
                    //commented out so that only capture moves are returned
                    //moves.Add(pos);
                    List<int> newMoves = new List<int>();
                    newMoves = GenerateNextMoveBishop(dir, board, pos, pieces);
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

        private List<int> GenerateKnightMoves(List<Square> board, List<Piece> pieces)
        {
            List<int> moves = new List<int>();

            //possible moves of knight
            int[] xMoves = { 1, 1, -1, -1, 2, 2, -2, -2, 1, 1, -1, -1, 2, 2, -2, -2, 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] yMoves = { 2, -2, 2, -2, 1, -1, 1, -1, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, -1, -1, 2, 2, -2, -2 };
            int[] zMoves = { 0, 0, 0, 0, 0, 0, 0, 0, 2, -2, 2, -2, 1, -1, 1, -1, 2, -2, 2, -2, 1, -1, 1, -1 };

            //iterate through and check moves
            for (int i = 0; i < xMoves.Length; i++)
            {
                int[] pos = ConvertPtrToVect(currentPosition);
                pos[0] += xMoves[i];
                pos[1] += yMoves[i];
                pos[2] += zMoves[i];

                //check move hasn't gone off board
                if (pos[0] > -1 && pos[0] < Constants.boardDimensions && pos[1] > -1 && pos[1] < Constants.boardDimensions && pos[2] > -1 && pos[2] < Constants.boardDimensions)
                {
                    int targetPiecePtr = board[ConvertVectToPtr(pos)].GetPiecePointer();
                    //add pointer version of move to list if its valid
                    //modified to only return capture moves
                    if (targetPiecePtr != -1 && pieces[targetPiecePtr].GetColour() != this.colour && pieces[targetPiecePtr].GetPieceType() == "N")
                    {
                        moves.Add(ConvertVectToPtr(pos));
                    }

                }
            }

            return moves;

        }

        private List<int> GeneratePawnCaptures(List<Square> board, List<Piece> pieces)
        {
            List<int> moves = new List<int>();
            int[] vect = ConvertPtrToVect(currentPosition);

            //might need to add extra conditions e.g. for black check that vect[1] != 7
            if (colour == (int)Colours.White)
            {
                //Generate black captures here
                //front view captures:
                if (vect[0] != 0 && vect[1] != 7)
                {
                    int pos = currentPosition + 8 - 1;
                    moves.Add(pos);
                }
                if (vect[0] != 7 && vect[1] != 7)
                {
                    int pos = currentPosition + 8 + 1;
                    moves.Add(pos);
                }
                //top down captures:
                if (vect[0] != 0 && vect[2] != 7)
                {
                    int pos = currentPosition + 64 - 1;
                    moves.Add(pos);
                }
                if (vect[0] != 7 && vect[2] != 7)
                {
                    int pos = currentPosition + 64 + 1;
                    moves.Add(pos);
                }
                //side view capture
                if (vect[1] != 7 && vect[2] != 7)
                {
                    int pos = currentPosition + 64 + 8;
                    moves.Add(pos);
                }
            }
            else
            {
                //Generate white captures here
                //front view captures
                if (vect[0] != 0 && vect[1] != 0)
                {
                    int pos = currentPosition - 8 - 1;
                    moves.Add(pos);
                }
                if (vect[0] != 7 && vect[1] != 0)
                {
                    int pos = currentPosition - 8 + 1;
                    moves.Add(pos);
                }
                //top down captures
                if (vect[0] != 0 && vect[2] != 0)
                {
                    int pos = currentPosition - 64 - 1;
                    moves.Add(pos);
                }
                if (vect[0] != 7 && vect[2] != 0)
                {
                    int pos = currentPosition - 64 + 1;
                    moves.Add(pos);
                }
                //side view capture
                if (vect[1] != 0 && vect[2] != 0)
                {
                    int pos = currentPosition - 64 - 8;
                    moves.Add(pos);
                }
            }

            List<int> finalMoves = new List<int>();

            for(int x = 0; x < moves.Count(); x++)
            {
                //filter moves so its only pawns of opposite colour
                int targetPtr = board[moves[x]].GetPiecePointer();
                if(targetPtr != -1)
                {
                    if (pieces[targetPtr].GetColour() != colour && pieces[targetPtr].GetPieceType() == "P")
                    {
                        finalMoves.Add(moves[x]);
                    }
                }
            }

            return finalMoves;
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
