using System;

namespace ThreeDimensionalChess
{
    class Knight : Piece
    {
        public Knight(int startPosition, int colour) : base(startPosition, colour) { }

        public override string GetPieceType()
        {
            return "N";
        }

        public override List<int> GeneratePossibleMoves(List<Square> board, List<Piece> pieces)
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
                    //filter empty and capture moves here
                    if (targetPiecePtr == -1)
                    {
                        moves.Add(ConvertVectToPtr(pos));
                    }
                    else if (pieces[targetPiecePtr].GetColour() != this.colour)
                    {
                        moves.Add(ConvertVectToPtr(pos));
                    }
                }
            }

            return moves;

        }
    }
}
