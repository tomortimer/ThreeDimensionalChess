using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeDimensionalChess
{
    static class Constants
    {
        //initialise constants
        //board is always cubic, initialise side length as such
        public const int boardDimensions = 8;
        //resulting size of cubic board
        public const int boardSize = boardDimensions * boardDimensions * boardDimensions;
    }

    enum Gamestates
    {
        Stalemate, // 0
        WhiteW, // 1 - White wins by black being checkmated
        BlackW, // 2 - Black wins by white being checkmated
        Ongoing, // 3
        PendingPromo // 4 - Used to wait program for pawn promotion
    }

    enum viewDirections
    {
        Front, //0
        Side, //1
        Top // 2
    }

    class Chess
    {
        public int playerTurn;
        private bool inCheck;
        private bool changeBoardDir;
        private Board board;
        private Stack<string> moveList;
        private int pendingMove;
        private Player whitePlayer;
        private Player blackPlayer;
        //array of pointers that compose the 2D viewport
        private int[] viewport;
        private int viewDir;
        private int viewLayer;
        //extra stuff
        DatabaseHandler db;
        private int ID;
        private int state;

        public Chess(int whiteID, int blackID, string name, bool undoMoves)
        {
            //init variables to defaults - starting turn, starting check and gamestate & create board
            playerTurn = (int)Colours.White;
            board = new Board();
            inCheck = false;
            moveList = new Stack<string>();
            pendingMove = 0;
            db = new DatabaseHandler();
            //init player objects, grab name based on ID from db
            whitePlayer = db.getPlayer(whiteID);
            whitePlayer.setColour((int)Colours.White);
            blackPlayer = db.getPlayer(blackID);
            blackPlayer.setColour((int)Colours.Black);
            /*whitePlayer = new Player(0, "name", 0, 0, 0, 0, 0, DateTime.Today);
            whitePlayer.setColour((int)Colours.White);
            blackPlayer = new Player(0, "name", 0, 0, 0, 0, 0, DateTime.Today);
            blackPlayer.setColour((int)Colours.Black);*/
            state = (int)Gamestates.Ongoing;
            //create game in database
            ID = db.createGame(name, undoMoves, whiteID, blackID);
            //init gamerules
            changeBoardDir = true;
            //init viewport (as front view white)
            viewLayer = 0;
            viewDir = 0;
            viewport = new int[64];
            updateViewport();
        }

        public void click(int squareIndex)
        {
            //can't do anything once game is over - separate method for rewind and view buttons
            if (playerTurn != -1 && state != (int)Gamestates.PendingPromo)
            {
                bool pieceSelected = board.isPieceSelected();
                if (pieceSelected == true)
                {
                    Piece selectedPiece = board.getSelectedPiece();
                    Piece pieceOnSquare = board.getPiece(squareIndex);
                    if (pieceOnSquare == null)
                    {
                        //moves onto square with selected piece if its empty
                        attemptMove(squareIndex);
                    }
                    else if (pieceOnSquare.getColour() == playerTurn)
                    {
                        selectPiece(squareIndex);
                    }
                    else
                    {
                        //getting to this point already eliminates player selecting their own piece due to above condition, failsafes in attemptmove anyway
                        //attempts move if player has pieces of opposite colour selected
                        if (selectedPiece.getColour() != pieceOnSquare.getColour())
                        {
                            attemptMove(squareIndex);
                        }
                        else
                        {
                            //this case should be only triggered when going from selecting one enemy piece to selecting another enemy piece
                            selectPiece(squareIndex);
                        }

                    }

                }
                else
                {
                    selectPiece(squareIndex); //simple outcome if no piece selected
                }
            }
            updateViewport();
        }

        public void viewportClick(int boardIndex)
        {
            click(viewport[boardIndex]);
            //board.setSquareBlue(viewport[boardIndex]);
            updateViewport();
        }

        private void attemptMove(int squareIndex)
        {
            //player turn is checked in board method
            string move = board.movePiece(squareIndex, playerTurn);

            //if the returning notation is not null, then the move has been affected, next player's turn
            if (move != null)
            {
                playerTurn = (playerTurn + 1) % 2;
                //gamestate is evaluated at the start of a player's turn, will be unnoticeable to players but makes the maths easier
                inCheck = board.checkCheck(playerTurn);
                evalGamestate();
                //add check/stalemate/checkmate symbols here
                switch (state)
                {
                    case 0:
                        //add ÷ symbol for stalemate
                        move = move + "÷";
                        break;
                    case 1:
                        // add # symbol for checkmate
                        move = move + "#";
                        break;
                    case 2:
                        // add # symbol for checkmate
                        move = move + "#";
                        break;
                    default:
                        //this is if game is ongoing but still need to append + if in check for movelist readability
                        if (inCheck)
                        {
                            move = move + "+";
                        }
                        break;
                }
                if (move.Contains("=")) 
                { 
                    state = (int)Gamestates.PendingPromo;
                    //reverse playerTurn change
                    playerTurn = (playerTurn - 1) % 2;
                    //holding square int here to ref piece down the line
                    pendingMove = squareIndex;
                    //select piece so that player is more aware of it
                    selectPiece(squareIndex);
                    //push move if promo, just don't forget to pop it later
                }
                moveList.Push(move);
            }

        }

        private void evalGamestate()
        {
            state = board.getGamestate(playerTurn);
            //end game if gamestate isn't ongoing
            if (state != (int)Gamestates.Ongoing) { playerTurn = -1; } //FIXME: BEHAVIOUR WITH -1 PLAYER IS UNDEFINED -> ADD DEFENSIVE PROGRAMMING
        }

        public void promotePawn(string pieceType)
        {
            bool success = board.promotePawn(pieceType, pendingMove);
            if (success)
            {
                pendingMove = -1;
                string move = moveList.Pop();
                move = move + pieceType;
                moveList.Push(move);
                playerTurn = (playerTurn + 1) % 2;
                state = (int)Gamestates.Ongoing;
            }
            updateViewport();
        }

        public void setViewDirection(int mode)
        {
            //if piece is selected, use that piece's relevant co-ord as layer value
            viewDir = mode;
            if (board.isPieceSelected()) 
            {
                int[] pieceVect = board.getSelectedPiece().getCurrentPosAsVect();
                switch (mode)
                {
                    case (int)viewDirections.Front:
                        viewLayer = pieceVect[2];
                        break;
                    case (int)viewDirections.Side:
                        viewLayer = pieceVect[0];
                        break;
                    case (int)viewDirections.Top:
                        viewLayer = pieceVect[1];
                        break;
                }
            }
            else
            {
                //default for side viewlayer is 7 because I just can't understand the game otherwise for some reason
                if(viewDir == (int)viewDirections.Side) { viewLayer = 7; }
                else if(changeBoardDir)
                {
                    //otherwise sets viewlayer to 0/7 depending on player turn - can be toggled as a gamerule
                    if(playerTurn == 0) { viewLayer = 7; }
                    else { viewLayer = 0; }
                }
            }
            updateViewport();
        }

        public void incrementViewLayer() { if (viewLayer < 7) { viewLayer++; updateViewport(); } }
        public void decrementViewLayer() { if(viewLayer > 0) { viewLayer--; updateViewport(); } }

        private void updateViewport()
        {
            int originPointer = 0;
            //grab pointers for relevant slice of board
            switch (viewDir)
            {
                case (int)viewDirections.Front:
                    // first transform origin by viewlayer
                    originPointer += (viewLayer * 64);
                    //then fill out viewport coords from origin pointer
                    for(int y = 0; y < 8; y++)
                    {
                        for(int x = 0; x < 8; x++)
                        {
                            viewport[(8 * y) + x] = originPointer + x + (y * 8);
                        }
                    }
                    break;
                case (int)viewDirections.Side:
                    //transform origin by viewlayer
                    originPointer += viewLayer; // is actually viewLayer * 1
                    for (int y = 0; y < 8; y++)
                    {
                        for(int x = 0; x < 8; x++)
                        {
                            //maps depth coordinates to x coordinates of viewport
                            viewport[(8 * y) + x] = originPointer + (x * 64) + (y * 8);
                        }
                    }
                    break;
                case (int)viewDirections.Top:
                    //transform origin by viewlayer
                    originPointer += (viewLayer * 8);
                    for(int y = 0; y < 8; y++)
                    {
                        for(int x = 0; x < 8; x++)
                        {
                            //maps depth coordinates to y coordinates of viewport
                            viewport[(8 * y) + x] = originPointer + x + (y * 64);
                        }
                    }
                    break;
            }
        }

        public Square getViewportCell(int ptr)
        {
            //takes a one dimensional pointer for the viewport
            //defensive programming
            if(ptr > -1 && ptr < 64)
            {
                return board.getSquare(viewport[ptr]);
            }
            else { throw new ArgumentOutOfRangeException(); }
        }

        public int getViewDirection()
        {
            return viewDir;
        }

        public int getViewLayer()
        {
            //add one for user readability
            return viewLayer + 1;
        }

        private void selectPiece(int squareIndex)
        {
            board.selectPiece(squareIndex, playerTurn);
        }

        //ONLY USE FOR LOADING GAME DATA - formatted by this program or otherwise assuming notation is perfect
        public void parseMove(string m)
        {
            //assumes move is valid
            moveList.Push(m);
            board.parseMove(m);
            playerTurn = (playerTurn + 1) % 2;
            evalGamestate();
        }

        public void undoMove()
        {
            string m = moveList.Pop();
            board.undoMove(m);
            playerTurn = (playerTurn - 1) % 2;
        }

        public void addPiece(string p, int pos, int col)
        {
            board.addPiece(p, pos, col);
        }

        public Player getCurrentPlayer()
        {
            Player ret;
            if(playerTurn == 0) { ret = blackPlayer; }
            else { ret = whitePlayer; }
            return ret;
        }

        public Piece getPieceDirect(int ptr)
        {
            return board.getPieceDirect(ptr);
        }

        public void printPossibleMoves()
        {
            Console.WriteLine(board.getPossibleMoveListRep());
        }

        public void printPieces()
        {
            board.printPieces();
        }

        public void printSelectedPiece()
        {
            board.printSelectedPiece();
        }

        public void printMoveList()
        {
            Console.WriteLine(moveList.representation());
        }

        public bool getInCheck() { return inCheck; }
        public bool checkCheck() { return board.checkCheck(playerTurn); }
        public int getSquareColour(int ptr) { return board.getSquareColour(ptr); }
        public int getGamestate() { return state; }
    }
}
