using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeDimensionalChess
{
    class Board
    {
        //one dimensional list used to store board - used modular arithmetic on vectors to navigate it
        private List<Square> board = new List<Square>();
        private List<Piece> pieces = new List<Piece>();
        //storage of selected piece and its moves
        private List<int> currentPossibleMoves = new List<int>();
        private int currentPieceIndex;
        private List<int> pointersMovedToFrom = new List<int>();

        public Board(bool custom = false)
        {
            int colourCtr = 0;
            //intialise all 512 squares of the 3D board
            for (int x = 0; x < Constants.boardSize; x++)
            {
                //ADD ONE FOR EACH NEW LINE
                if (x % 8 == 0 && x != 0) { colourCtr++; }
                //ADD ONE FOR EACH NEW BOARD
                if (x % 64 == 0 && x != 0) { colourCtr++; }
                //adds a new square to the board, performs mod on x to get colour, Colours enum is stored in Square.cs even squares are black(with 0 also being black), odd are white
                board.Add(new Square(colourCtr % 2));
                colourCtr++;
            }
            //always place kings so that they are first in list
            //only place pieces in default positions if not custom setup, otherwise force user to place kings first
            if (!custom)
            {
                addPiece("K", 508, 0);
                addPiece("K", 4, 1);
                //place the rest of the pieces in order of complexity (facilitates efficient stalemate checking later)
                //white pawns
                for (int i = 0; i < 2; i++)
                {
                    for (int x = 0; x < Constants.boardDimensions; x++)
                    {
                        int pos = 8 + x + (64 * i);
                        addPiece("P", pos, 1);
                    }
                }
                //black pawns
                for (int i = 0; i < 2; i++)
                {
                    for (int x = 0; x < Constants.boardDimensions; x++)
                    {
                        int pos = 496 + x - (64 * i);
                        addPiece("P", pos, 0);
                    }
                }
                //rooks
                addPiece("R", 0, 1);
                addPiece("R", 7, 1);
                addPiece("R", 511, 0);
                addPiece("R", 504, 0);
                //knights
                addPiece("N", 1, 1);
                addPiece("N", 6, 1);
                addPiece("N", 510, 0);
                addPiece("N", 505, 0);
                //bishops
                addPiece("B", 2, 1);
                addPiece("B", 5, 1);
                addPiece("B", 509, 0);
                addPiece("B", 506, 0);
                //queens
                addPiece("Q", 507, 0);
                addPiece("Q", 3, 1);
            }
            currentPieceIndex = -1;
        }

        public bool isPieceSelected()
        {
            bool ret = false;
            if (currentPieceIndex != -1) { ret = true; }
            return ret;
        }

        public Piece getSelectedPiece()
        {
            return pieces[currentPieceIndex];
        }

        //takes a square index as input
        public Piece getPiece(int inp)
        {
            Piece tmp = null;
            int ptr = board[inp].getPiecePointer();
            //if there is a piece on square returns piece otherwise returns null
            if (ptr != -1) { tmp = pieces[ptr]; }
            return tmp;
        }

        public Piece getPieceDirect(int ptr)
        {
            Piece tmp = null;
            if(ptr != -1)
            {
                tmp = pieces[ptr];
            }
            else { throw new ArgumentOutOfRangeException(); }
            return tmp;
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

        //this needs to be triggered when a square is clicked on, parameter is index of square
        public void selectPiece(int squarePtr, int currentPlayer)
        {
            //do nothing if square is empty
            //list is setup here in case needed
            int piecePtr = board[squarePtr].getPiecePointer();
            List<int> moveList = new List<int>();
            if (piecePtr != -1)
            {
                //deselects piece if its already selected
                if (piecePtr != currentPieceIndex)
                {
                    //clear colours on squares first
                    for (int x = 0; x < currentPossibleMoves.Count(); x++) 
                    { 
                        board[currentPossibleMoves[x]].notUnderThreat();
                        if (pointersMovedToFrom.Contains(currentPossibleMoves[x])) { board[currentPossibleMoves[x]].pieceMoved(); }
                    }

                    currentPieceIndex = piecePtr;
                    moveList = pieces[piecePtr].generatePossibleMoves(board, pieces); //this only returns physically possible moves
                    //should show moves protecting a piece????? highlight different colour : yellow

                    //filter out null moves
                    List<int> filteredMoves = new List<int>();
                    for (int x = 0; x < moveList.Count(); x++)
                    {
                        //FILTER OUT SELF CHECK MOVES HERE
                        //this may slow down everything a lot - creatre siumulatred board and try the move here
                        SimulatedBoard tmp = new SimulatedBoard(board, pieces);
                        bool legal = tmp.legalMove(moveList[x], currentPieceIndex, pieces[currentPieceIndex].getColour());
                        if (legal && moveList[x] != -1) { filteredMoves.Add(moveList[x]); }
                    }

                    currentPossibleMoves = filteredMoves;
                    bool friendly = true;
                    if (pieces[currentPieceIndex].getColour() != currentPlayer) { friendly = false; }
                    displayMoves(currentPossibleMoves, friendly);
                }
            }
            else
            {
                //resets selection data
                currentPieceIndex = -1;
                for (int x = 0; x < currentPossibleMoves.Count(); x++) 
                { 
                    board[currentPossibleMoves[x]].notUnderThreat();
                    if (pointersMovedToFrom.Contains(currentPossibleMoves[x])) { board[currentPossibleMoves[x]].pieceMoved(); }
                }
                currentPossibleMoves = new List<int>();
            }

        }

        //subroutine for moving piece, no need to take piece in parameter - since currently selected piece is already stored
        public string movePiece(int targetSquarePtr, int currentPlayer)
        {
            //LA3DN move will be returned in this string
            string move = null;

            int startSquarePtr = pieces[currentPieceIndex].getCurrentPosition();
            //target move should be in possible move list and also check that piece belongs to current player
            if (currentPossibleMoves.Contains(targetSquarePtr) && pieces[currentPieceIndex].getColour() == currentPlayer)
            {
                //reset colours of last moves - make sure that list isn't empty first
                if (pointersMovedToFrom.Count() > 0)
                {
                    board[pointersMovedToFrom[0]].notUnderThreat();
                    board[pointersMovedToFrom[1]].notUnderThreat();
                    pointersMovedToFrom.RemoveAt(0);
                    pointersMovedToFrom.RemoveAt(0);
                }

                int targetPiecePtr = board[targetSquarePtr].getPiecePointer();

                //store first half of move data here (origin point and piece type)
                string moveDataFirstHalf = pieces[currentPieceIndex].getPieceType() + pieces[currentPieceIndex].getCurrentPosAsStr();

                //effects the move, shades squares yellow, store squares to reset them later
                string moveDataSecondHalf = pieces[currentPieceIndex].movePiece(targetSquarePtr, board, pieces);
                board[startSquarePtr].setPiecePointer(-1);
                board[startSquarePtr].pieceMoved();
                pointersMovedToFrom.Add(startSquarePtr);
                //added logic here to prevent ptr misalignment - if a piece is taken and its pointer is less than the current piece, we need to offset current piece ptr by one
                if (moveDataSecondHalf.Contains("X"))
                {
                    if (targetPiecePtr < currentPieceIndex) { currentPieceIndex--; }
                    for (int x = targetPiecePtr; x < pieces.Count(); x++)
                    {
                        board[pieces[x].getCurrentPosition()].decrementPiecePointer();
                    }
                }
                board[targetSquarePtr].setPiecePointer(currentPieceIndex);
                board[targetSquarePtr].pieceMoved();
                pointersMovedToFrom.Add(targetSquarePtr);
                //resets selection data
                currentPieceIndex = -1;
                for (int x = 0; x < currentPossibleMoves.Count(); x++) 
                { 
                    board[currentPossibleMoves[x]].notUnderThreat();
                    if (pointersMovedToFrom.Contains(currentPossibleMoves[x])) { board[currentPossibleMoves[x]].pieceMoved(); }
                }
                currentPossibleMoves = new List<int>();
                //check if a pawn has reached end rank  
                string promotion = checkLastRank();

                //aggregate string parts - don't forget to add promotion back here
                move = moveDataFirstHalf + moveDataSecondHalf + promotion;

            }
            else
            {
                //FIXME: this needs to be changed I think - just deselect piece?
                //issue has been confirmed
                //fix has been attempted
                selectPiece(targetSquarePtr, currentPlayer);
            }

            return move;
        }

        public int getGamestate(int currentPlayer)
        {
            int ret = 0;

            for (int x = 0; x < pieces.Count(); x++)
            {
                if (pieces[x].getColour() == currentPlayer)
                {
                    List<int> moveList = pieces[x].generatePossibleMoves(board, pieces);
                    //filter out null moves
                    List<int> filteredMoves = new List<int>();
                    for (int y = 0; y < moveList.Count(); y++)
                    {
                        if (moveList[y] != -1)
                        {
                            //FILTER OUT SELF CHECK MOVES HERE
                            //this may slow down everything a lot - creatre siumulatred board and try the move here
                            SimulatedBoard tmp = new SimulatedBoard(board, pieces);
                            bool legal = tmp.legalMove(moveList[y], x, pieces[x].getColour());
                            if (legal) { filteredMoves.Add(moveList[y]); }
                        }
                    }
                    //if this is greater than zero, games is neither stale nor checkmate
                    if (filteredMoves.Count() > 0)
                    {
                        ret = (int)Gamestates.Ongoing;
                        x = pieces.Count();
                    }
                }
            }
            // if game has not been set to ongoing here, then it is either stale or checkmate
            if (ret != (int)Gamestates.Ongoing)
            {
                bool checkMate = checkThreat(pieces[currentPlayer].getCurrentPosition(), currentPlayer);
                if (checkMate && currentPlayer == (int)Colours.Black) { ret = (int)Gamestates.WhiteW; }
                else if (checkMate && currentPlayer == (int)Colours.White) { ret = (int)Gamestates.BlackW; }
            }

            return ret;
        }

        private void displayMoves(List<int> possibleMoves, bool friendly)
        {
            for (int x = 0; x < possibleMoves.Count(); x++)
            {
                board[possibleMoves[x]].underThreat(friendly);
            }
        }

        private string checkLastRank()
        {
            string ret = "";
            //check promotion linearly
            for(int x = 0; x < 8; x++)
            {
                //white pieces
                int piecePtr = board[x + (7 * 64) + (7 * 8)].getPiecePointer();
                if(piecePtr != -1 && pieces[piecePtr].getPieceType() == "P" && pieces[piecePtr].getColour() == (int)Colours.White)
                {
                    ret = "=";
                }

                //black pieces
                piecePtr = board[x].getPiecePointer();
                if(piecePtr != -1 && pieces[piecePtr].getPieceType() == "P" && pieces[piecePtr].getColour() == (int)Colours.Black)
                {
                    ret = "=";
                }
            }
            return ret;
        }

        public bool promotePawn(string pieceType, int squarePtr)
        {
            int piecePtr = board[squarePtr].getPiecePointer();
            bool success = false;
            ///rereference piece in list to a new piece of promoted variant
            switch (pieceType)
            {
                case "Q":
                    pieces[piecePtr] = new Queen(squarePtr, pieces[piecePtr].getColour());
                    success = true;
                    break;
                case "B":
                    pieces[piecePtr] = new Bishop(squarePtr, pieces[piecePtr].getColour());
                    success = true;
                    break;
                case "R":
                    pieces[piecePtr] = new Rook(squarePtr, pieces[piecePtr].getColour());
                    success = true;
                    break;
                case "N":
                    pieces[piecePtr] = new Knight(squarePtr, pieces[piecePtr].getColour());
                    success = true;
                    break;
            }
            return success;
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

        public bool checkCheck(int player)
        {
            bool check = checkThreat(pieces[player].getCurrentPosition(), player);
            return check;
        }

        public void printPieces()
        {
            for (int x = 0; x < pieces.Count(); x++)
            {
                Console.WriteLine(pieces[x].getPieceType() + " :" + pieces[x].getCurrentPosition());
            }
        }

        public void parseMove(string move)
        {
            throw new NotImplementedException();
        }

        public void undoMove(string move)
        {
            bool captureFlag = false;
            bool promotionFlag = false;
            if (move.Contains("X")) { captureFlag = true; }
            if (move.Contains("=")) { promotionFlag = true; }
            string[] halves = move.Split('X', '-');

            //WIP
        }

        public string getPossibleMoveListRep()
        {
            return currentPossibleMoves.ConvertToString();
        }

        public void printMoves(int piecePtr)
        {
            List<int> tmp = new List<int>();
            tmp = pieces[piecePtr].generatePossibleMoves(board, pieces);
            for (int x = 0; x < tmp.Count(); x++)
            {
                if (tmp[x] != -1)
                {
                    Console.WriteLine(tmp[x]);
                }

            }
        }

        public void printSelectedPiece()
        {
            if (currentPieceIndex != -1)
            {
                Piece p = pieces[currentPieceIndex];
                string col = "";
                if (p.getColour() == (int)Colours.White)
                {
                    col = "White";
                }
                else { col = "Black"; }
                Console.WriteLine(col + " " + p.getPieceType() + " at " + p.getCurrentPosition());
            }
            else { Console.WriteLine("No selected piece"); }
        }

        public Square getSquare(int ptr)
        {
            return board[ptr];
        }

        public void setSquareBlue(int ptr) { board[ptr].setSquareBlue(); }

        public int getSquareColour(int ptr)
        {
            return board[ptr].getColour();
        }
    }

}
