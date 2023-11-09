using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeDimensionalChess
{
    class Queen : Piece
    {
        //create internal pieces that can be used to generate moves
        Bishop internalBishop;
        Rook internalRook;

        public Queen(int startPosition, int col) : base(startPosition, col)
        {
            internalBishop = new Bishop(startPosition, col);
            internalRook = new Rook(startPosition, col);
        }

        public override string getPieceType() { return "Q"; }

        //override virtual move piece method to move internal pieces with piece
        public override string movePiece(int endPosition, List<Square> board, List<Piece> pieces)
        {
            //setup string to return LA3DN data
            string data = "";
            //dereference captured piece here
            int targetPiecePtr = board[endPosition].getPiecePointer();
            if (targetPiecePtr != -1)
            {
                data += "X" + pieces[targetPiecePtr].getPieceType();
                pieces.RemoveAt(targetPiecePtr);
            }
            else { data += "-"; }
            data += convertPosToStr(endPosition);

            currentPosition = endPosition;
            internalBishop.forceMove(endPosition);
            internalRook.forceMove(endPosition);

            return data;
        }

        public override List<int> generatePossibleMoves(List<Square> board, List<Piece> pieces)
        {
            List<int> moves = new List<int>();

            //generate moves using internal pieces and then append to main list of moves
            List<int> tmp = internalRook.generatePossibleMoves(board, pieces);
            for (int x = 0; x < tmp.Count(); x++) { moves.Add(tmp[x]); }
            tmp = internalBishop.generatePossibleMoves(board, pieces);
            for (int x = 0; x < tmp.Count(); x++) { moves.Add(tmp[x]); }

            return moves;
        }
    }
}
