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
                AddPiece("K", 508, 0);
                AddPiece("K", 4, 1);
                //place the rest of the pieces in order of complexity (facilitates efficient stalemate checking later)
                //white pawns
                for (int i = 0; i < 2; i++)
                {
                    for (int x = 0; x < Constants.boardDimensions; x++)
                    {
                        int pos = 8 + x + (64 * i);
                        AddPiece("P", pos, 1);
                    }
                }
                //black pawns
                for (int i = 0; i < 2; i++)
                {
                    for (int x = 0; x < Constants.boardDimensions; x++)
                    {
                        int pos = 496 + x - (64 * i);
                        AddPiece("P", pos, 0);
                    }
                }
                //rooks
                AddPiece("R", 0, 1);
                AddPiece("R", 7, 1);
                AddPiece("R", 511, 0);
                AddPiece("R", 504, 0);
                //knights
                AddPiece("N", 1, 1);
                AddPiece("N", 6, 1);
                AddPiece("N", 510, 0);
                AddPiece("N", 505, 0);
                //bishops
                AddPiece("B", 2, 1);
                AddPiece("B", 5, 1);
                AddPiece("B", 509, 0);
                AddPiece("B", 506, 0);
                //queens
                AddPiece("Q", 507, 0);
                AddPiece("Q", 3, 1);
            }
            currentPieceIndex = -1;
        }

        public bool IsPieceSelected()
        {
            bool ret = false;
            if (currentPieceIndex != -1) { ret = true; }
            return ret;
        }

        public Piece GetSelectedPiece()
        {
            return pieces[currentPieceIndex];
        }

        //takes a square index as input
        public Piece GetPiece(int inp)
        {
            Piece tmp = null;
            int ptr = board[inp].GetPiecePointer();
            //if there is a piece on square returns piece otherwise returns null
            if (ptr != -1) { tmp = pieces[ptr]; }
            return tmp;
        }

        public Piece GetPieceDirect(int ptr)
        {
            Piece tmp = null;
            if(ptr != -1)
            {
                tmp = pieces[ptr];
            }
            else { throw new ArgumentOutOfRangeException(); }
            return tmp;
        }

        public void AddPiece(string type, int pos, int colour)
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

            board[pos].SetPiecePointer(pieces.Count() - 1);
        }

        //this needs to be triggered when a square is clicked on, parameter is index of square
        public void SelectPiece(int squarePtr, int currentPlayer)
        {
            //do nothing if square is empty
            //list is setup here in case needed
            int piecePtr = board[squarePtr].GetPiecePointer();
            List<int> moveList = new List<int>();
            if (piecePtr != -1)
            {
                //deselects piece if its already selected
                if (piecePtr != currentPieceIndex)
                {
                    //clear colours on squares first
                    for (int x = 0; x < currentPossibleMoves.Count(); x++) 
                    { 
                        board[currentPossibleMoves[x]].NotUnderThreat();
                        if (pointersMovedToFrom.Contains(currentPossibleMoves[x])) { board[currentPossibleMoves[x]].PieceMoved(); }
                    }

                    currentPieceIndex = piecePtr;
                    moveList = pieces[piecePtr].GeneratePossibleMoves(board, pieces); //this only returns physically possible moves
                    //should show moves protecting a piece????? highlight different colour : yellow

                    //filter out null moves
                    List<int> filteredMoves = new List<int>();
                    for (int x = 0; x < moveList.Count(); x++)
                    {
                        //FILTER OUT SELF CHECK MOVES HERE
                        //this may slow down everything a lot - creatre siumulatred board and try the move here
                        SimulatedBoard tmp = new SimulatedBoard(board, pieces);
                        bool legal = tmp.LegalMove(moveList[x], currentPieceIndex, pieces[currentPieceIndex].GetColour());
                        if (legal && moveList[x] != -1) { filteredMoves.Add(moveList[x]); }
                    }

                    currentPossibleMoves = filteredMoves;
                    bool friendly = true;
                    if (pieces[currentPieceIndex].GetColour() != currentPlayer) { friendly = false; }
                    DisplayMoves(currentPossibleMoves, friendly);
                }
            }
            else
            {
                //resets selection data
                currentPieceIndex = -1;
                for (int x = 0; x < currentPossibleMoves.Count(); x++) 
                { 
                    board[currentPossibleMoves[x]].NotUnderThreat();
                    if (pointersMovedToFrom.Contains(currentPossibleMoves[x])) { board[currentPossibleMoves[x]].PieceMoved(); }
                }
                currentPossibleMoves = new List<int>();
            }

        }

        //subroutine for moving piece, no need to take piece in parameter - since currently selected piece is already stored
        public string MovePiece(int targetSquarePtr, int currentPlayer)
        {
            //LA3DN move will be returned in this string
            string move = null;

            int startSquarePtr = pieces[currentPieceIndex].GetCurrentPosition();
            //target move should be in possible move list and also check that piece belongs to current player
            if (currentPossibleMoves.Contains(targetSquarePtr) && pieces[currentPieceIndex].GetColour() == currentPlayer)
            {
                //reset colours of last moves - make sure that list isn't empty first
                if (pointersMovedToFrom.Count() > 0)
                {
                    board[pointersMovedToFrom[0]].NotUnderThreat();
                    board[pointersMovedToFrom[1]].NotUnderThreat();
                    pointersMovedToFrom.RemoveAt(0);
                    pointersMovedToFrom.RemoveAt(0);
                }

                int targetPiecePtr = board[targetSquarePtr].GetPiecePointer();

                //store first half of move data here (origin point and piece type)
                string moveDataFirstHalf = pieces[currentPieceIndex].GetPieceType() + pieces[currentPieceIndex].GetCurrentPosAsStr();

                //effects the move, shades squares yellow, store squares to reset them later
                string moveDataSecondHalf = pieces[currentPieceIndex].MovePiece(targetSquarePtr, board, pieces);
                board[startSquarePtr].SetPiecePointer(-1);
                board[startSquarePtr].PieceMoved();
                pointersMovedToFrom.Add(startSquarePtr);
                //added logic here to prevent ptr misalignment - if a piece is taken and its pointer is less than the current piece, we need to offset current piece ptr by one
                if (moveDataSecondHalf.Contains("X"))
                {
                    if (targetPiecePtr < currentPieceIndex) { currentPieceIndex--; }
                    for (int x = targetPiecePtr; x < pieces.Count(); x++)
                    {
                        board[pieces[x].GetCurrentPosition()].DecrementPiecePointer();
                    }
                }
                board[targetSquarePtr].SetPiecePointer(currentPieceIndex);
                board[targetSquarePtr].PieceMoved();
                pointersMovedToFrom.Add(targetSquarePtr);
                //resets selection data
                currentPieceIndex = -1;
                for (int x = 0; x < currentPossibleMoves.Count(); x++) 
                { 
                    board[currentPossibleMoves[x]].NotUnderThreat();
                    if (pointersMovedToFrom.Contains(currentPossibleMoves[x])) { board[currentPossibleMoves[x]].PieceMoved(); }
                }
                currentPossibleMoves = new List<int>();
                //check if a pawn has reached end rank  
                string promotion = CheckLastRank();

                //aggregate string parts
                move = moveDataFirstHalf + moveDataSecondHalf + promotion;

            }
            else
            {
                SelectPiece(targetSquarePtr, currentPlayer);
            }

            return move;
        }

        public int GetGamestate(int currentPlayer)
        {
            int ret = 0;

            for (int x = 0; x < pieces.Count(); x++)
            {
                if (pieces[x].GetColour() == currentPlayer)
                {
                    List<int> moveList = pieces[x].GeneratePossibleMoves(board, pieces);
                    //filter out null moves
                    List<int> filteredMoves = new List<int>();
                    for (int y = 0; y < moveList.Count(); y++)
                    {
                        if (moveList[y] != -1)
                        {
                            //FILTER OUT SELF CHECK MOVES HERE
                            SimulatedBoard tmp = new SimulatedBoard(board, pieces);
                            bool legal = tmp.LegalMove(moveList[y], x, pieces[x].GetColour());
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
                bool checkMate = CheckThreat(pieces[currentPlayer].GetCurrentPosition(), currentPlayer);
                if (checkMate && currentPlayer == (int)Colours.Black) { ret = (int)Gamestates.WhiteW; }
                else if (checkMate && currentPlayer == (int)Colours.White) { ret = (int)Gamestates.BlackW; }
            }

            return ret;
        }

        private void DisplayMoves(List<int> possibleMoves, bool friendly)
        {
            for (int x = 0; x < possibleMoves.Count(); x++)
            {
                board[possibleMoves[x]].UnderThreat(friendly);
            }
        }

        private string CheckLastRank()
        {
            string ret = "";
            //check promotion linearly
            for(int x = 0; x < 8; x++)
            {
                //white pieces
                int piecePtr = board[x + (7 * 64) + (7 * 8)].GetPiecePointer();
                if(piecePtr != -1 && pieces[piecePtr].GetPieceType() == "P" && pieces[piecePtr].GetColour() == (int)Colours.White)
                {
                    ret = "=";
                }

                //black pieces
                piecePtr = board[x].GetPiecePointer();
                if(piecePtr != -1 && pieces[piecePtr].GetPieceType() == "P" && pieces[piecePtr].GetColour() == (int)Colours.Black)
                {
                    ret = "=";
                }
            }
            return ret;
        }

        public bool PromotePawn(string pieceType, int squarePtr)
        {
            int piecePtr = board[squarePtr].GetPiecePointer();
            bool success = false;
            ///rereference piece in list to a new piece of promoted variant
            switch (pieceType)
            {
                case "Q":
                    pieces[piecePtr] = new Queen(squarePtr, pieces[piecePtr].GetColour());
                    success = true;
                    break;
                case "B":
                    pieces[piecePtr] = new Bishop(squarePtr, pieces[piecePtr].GetColour());
                    success = true;
                    break;
                case "R":
                    pieces[piecePtr] = new Rook(squarePtr, pieces[piecePtr].GetColour());
                    success = true;
                    break;
                case "N":
                    pieces[piecePtr] = new Knight(squarePtr, pieces[piecePtr].GetColour());
                    success = true;
                    break;
            }
            return success;
        }

        private bool CheckThreat(int squarePtr, int currentPlayer)
        {
            bool threat = false;
            ThreatSuperPiece tmp = new ThreatSuperPiece(squarePtr, currentPlayer);
            //see if there are ANY pieces threatening current square
            List<int> threatMoves = tmp.GeneratePossibleMoves(board, pieces);
            if (threatMoves.Count() > 0) { threat = true; }

            return threat;
        }

        public bool CheckCheck(int player)
        {
            bool check = CheckThreat(pieces[player].GetCurrentPosition(), player);
            return check;
        }

        public void ParseMove(string move, int currentPlayer)
        {
            //separate pieces from notation and then enact
            string startCoord = move.Substring(1, 3);
            int startSquare = pieces[0].ConvertStrPosToPtr(startCoord);
            SelectPiece(startSquare, currentPlayer);
            string endCoord = "";
            if (move.Contains('X'))
            {
                endCoord = move.Substring(6, 3);
            }
            else { endCoord = move.Substring(5, 3); }
            //check if promotion, if so enact
            int targetSquare = pieces[0].ConvertStrPosToPtr(endCoord);
            MovePiece(targetSquare, currentPlayer);
            if (move.Contains('=')) 
            {
                string promotedPiece = move.Substring(move.Length - 1, 1);
                PromotePawn(promotedPiece, targetSquare);
            }
        }

        public void UndoMove(string move)
        {
            //grab parts of move, like in ParseMove
            string startCoord = move.Substring(1, 3);
            int startSquare = pieces[0].ConvertStrPosToPtr(startCoord);
            string endCoord = "";
            if (move.Contains('X'))
            {
                endCoord = move.Substring(6, 3);
            }
            else { endCoord = move.Substring(5, 3); }
            int endSquare = pieces[0].ConvertStrPosToPtr(endCoord);
            int piecePtr = board[endSquare].GetPiecePointer();
            //undo promotion if there is one
            if (move.Contains('='))
            {
                pieces[piecePtr] = new Pawn(endSquare, pieces[piecePtr].GetColour());
            }
            //reverse move now
            pieces[piecePtr].MovePiece(startSquare, board, pieces);
            board[endSquare].SetPiecePointer(-1);
            board[startSquare].SetPiecePointer(piecePtr);
            //undo capture if there is one
            if (move.Contains('X'))
            {
                string pieceType = Convert.ToString(move[5]);
                int pieceCol = (pieces[piecePtr].GetColour() + 1) % 2;
                AddPiece(pieceType, endSquare, pieceCol);
            }
            //clear shading
            if (pointersMovedToFrom.Count() > 0)
            {
                board[pointersMovedToFrom[0]].NotUnderThreat();
                board[pointersMovedToFrom[1]].NotUnderThreat();
                pointersMovedToFrom.RemoveAt(0);
                pointersMovedToFrom.RemoveAt(0);
            }
        }

        public Square GetSquare(int ptr)
        {
            return board[ptr];
        }

        public void SetSquareBlue(int ptr) { board[ptr].SetSquareBlue(); }

        public int GetSquareColour(int ptr)
        {
            return board[ptr].GetColour();
        }
    }

}
