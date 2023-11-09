using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeDimensionalChess
{
    internal class SimulatedBoard
    {
        //one dimensional list used to store board - used modular arithmetic on vectors to navigate it
        private List<Square> board = new List<Square>();
        private List<Piece> pieces = new List<Piece>();

        //cloning constructor
        public SimulatedBoard(List<Square> b, List<Piece> p)
        {
            for (int x = 0; x < Constants.boardSize; x++)
            {
                //adds a new square to the board, performs mod on x to get colour, Colours enum is stored in Square.cs even squares are black(with 0 also being black), odd are white
                board.Add(new Square(x % 2));
                //copy over positions
                board[x].setPiecePointer(b[x].getPiecePointer());
            }
            //copy over pieces
            for (int x = 0; x < p.Count(); x++)
            {
                addPiece(p[x].getPieceType(), p[x].getCurrentPosition(), p[x].getColour());
            }

        }
        public void addPiece(string type, int pos, int colour)
        {
            Piece p = null;
            switch (type)
            {
                case "R":
                    p = new Rook(pos, colour);
                    break;
                case "P":
                    p = new Pawn(pos, colour);
                    break;
                case "K":
                    p = new King(pos, colour);
                    break;
                case "B":
                    p = new Bishop(pos, colour);
                    break;
                case "N":
                    p = new Knight(pos, colour);
                    break;
                case "Q":
                    p = new Queen(pos, colour);
                    break;
                case "TSP":
                    p = new ThreatSuperPiece(pos, colour);
                    break;
            }

            pieces.Add(p);

            board[pos].setPiecePointer(pieces.Count() - 1);
        }

        public bool legalMove(int endPos, int piecePtr, int currentPlayer)
        {
            int targetPiecePtr = board[endPos].getPiecePointer();

            board[pieces[piecePtr].getCurrentPosition()].setPiecePointer(-1);
            string moveData = pieces[piecePtr].movePiece(endPos, board, pieces);
            //added logic here to prevent ptr misalignment - if a piece is taken and its pointer is less than the current piece, we need to offset current piece ptr by one
            if (moveData.Contains("X"))
            {
                if (targetPiecePtr < piecePtr) { piecePtr--; }
                for (int x = targetPiecePtr; x < pieces.Count(); x++)
                {
                    board[pieces[x].getCurrentPosition()].decrementPiecePointer();
                }
            }
            board[endPos].setPiecePointer(piecePtr);

            //use check threat method with piece index as currentplayer (points at their king)
            //takes negation of return value, since checkthreat returns true for threat but we want to know if move is safe
            bool ret = !checkThreat(pieces[currentPlayer].getCurrentPosition(), currentPlayer);
            return ret;
        }

        private bool checkThreat(int squarePtr, int currentPlayer)
        {
            bool threat = false;
            ThreatSuperPiece tmp = new ThreatSuperPiece(squarePtr, currentPlayer);
            //see if there are ANY pieces threatening current square
            List<int> threatMoves = tmp.generatePossibleMoves(board, pieces);
            if (threatMoves.Count() > 0) { threat = true; }

            return threat;
        }
    }
}
