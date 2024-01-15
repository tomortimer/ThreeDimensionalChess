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

    enum ViewDirections
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
        private bool undoMovesAllowed;
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
            whitePlayer = db.GetPlayer(whiteID);
            whitePlayer.SetColour((int)Colours.White);
            blackPlayer = db.GetPlayer(blackID);
            blackPlayer.SetColour((int)Colours.Black);
            /*whitePlayer = new Player(0, "name", 0, 0, 0, 0, 0, DateTime.Today);
            whitePlayer.SetColour((int)Colours.White);
            blackPlayer = new Player(0, "name", 0, 0, 0, 0, 0, DateTime.Today);
            blackPlayer.SetColour((int)Colours.Black);*/
            state = (int)Gamestates.Ongoing;
            //create game in database
            ID = db.CreateGame(name, undoMoves, whiteID, blackID);
            //init games rules
            undoMovesAllowed = undoMoves;
            changeBoardDir = true;
            //init viewport (as front view white)
            viewLayer = 0;
            viewDir = 0;
            viewport = new int[64];
            UpdateViewport();
        }

        //constructor for loading games from GameInfo
        public Chess(GameInfo info)
        {
            playerTurn = (int)Colours.White;
            board = new Board();
            inCheck = false;
            moveList = new Stack<string>();
            pendingMove = 0;
            db = new DatabaseHandler();
            //load values from info obj
            ID = info.GetGameID();
            undoMovesAllowed = info.GetUndoMoves();
            state = info.GetGamestateAsInt();
            //init players
            whitePlayer = db.GetPlayer(info.GetWhitePlayerID());
            whitePlayer.SetColour((int)Colours.White);
            blackPlayer = db.GetPlayer(info.GetBlackPlayerID());
            blackPlayer.SetColour((int)Colours.Black);
            //init viewport (as front view white)
            changeBoardDir = true;
            viewLayer = 0;
            viewDir = 0;
            viewport = new int[64];
            UpdateViewport();
            //enact saved moves
            List<string> moves = info.GetMoves();
            while (moves.Count() > 0 && moves[0] != "")
            {
                //using list like a queue
                string tmp = moves.RemoveAt(0);
                if (!(tmp == "Mutual Agreement÷" || tmp == "White#" || tmp == "Black#")) { ParseMove(tmp); }
                else 
                { 
                    undoMovesAllowed = true;
                    ID = db.CreateGame(info.GetName() + "-Copy", true, info.GetWhitePlayerID(), info.GetBlackPlayerID());
                }
            }
            //if game is finished enabled undo moves
            if (!moveList.IsEmpty() && (moveList.Peek().Contains('#') || moveList.Peek().Contains('÷')))
            {
                ID = db.CreateGame(info.GetName() + "-Copy", true, info.GetWhitePlayerID(), info.GetBlackPlayerID());
                undoMovesAllowed = true;
                UndoMove();
            }
            if (moveList.Peek() != null && moveList.Peek().Contains('+')) { inCheck = true; }
        }

        public void Click(int squareIndex)
        {
            //can't do anything once game is over - separate method for rewind and view buttons
            if (playerTurn != -1 && state != (int)Gamestates.PendingPromo)
            {
                bool pieceSelected = board.IsPieceSelected();
                if (pieceSelected == true)
                {
                    Piece selectedPiece = board.GetSelectedPiece();
                    Piece pieceOnSquare = board.GetPiece(squareIndex);
                    if (pieceOnSquare == null)
                    {
                        //moves onto square with selected piece if its empty
                        AttemptMove(squareIndex);
                    }
                    else if (pieceOnSquare.GetColour() == playerTurn)
                    {
                        SelectPiece(squareIndex);
                    }
                    else
                    {
                        //Getting to this point already eliminates player selecting their own piece due to above condition, failsafes in attemptmove anyway
                        //attempts move if player has pieces of opposite colour selected
                        if (selectedPiece.GetColour() != pieceOnSquare.GetColour())
                        {
                            AttemptMove(squareIndex);
                        }
                        else
                        {
                            //this case should be only triggered when going from selecting one enemy piece to selecting another enemy piece
                            SelectPiece(squareIndex);
                        }

                    }

                }
                else
                {
                    SelectPiece(squareIndex); //simple outcome if no piece selected
                }
            }
            UpdateViewport();
        }

        public void ViewportClick(int boardIndex)
        {
            Click(viewport[boardIndex]);
            //board.SetSquareBlue(viewport[boardIndex]);
            UpdateViewport();
        }

        private void AttemptMove(int squareIndex)
        {
            //player turn is checked in board method
            string move = board.MovePiece(squareIndex, playerTurn);

            //if the returning notation is not null, then the move has been effected, next player's turn
            if (move != null)
            {
                playerTurn = (playerTurn + 1) % 2;
                //gamestate is evaluated at the start of a player's turn, will be unnoticeable to players but makes the maths easier
                inCheck = board.CheckCheck(playerTurn);
                EvalGamestate();
                //add check/stalemate/checkmate symbols here
                switch (state)
                {
                    //lock undo once game is complete, if game is reopened from menu it opens a undoable copy
                    case (int)Gamestates.Stalemate:
                        //add ÷ symbol for stalemate
                        move = move + "÷";
                        undoMovesAllowed = false;
                        whitePlayer.AddWhiteDraw();
                        blackPlayer.AddBlackDraw();
                        break;
                    case (int)Gamestates.WhiteW:
                        // add # symbol for checkmate
                        move = move + "#";
                        undoMovesAllowed = false;
                        whitePlayer.AddWhiteWin();
                        blackPlayer.AddBlackLoss();
                        break;
                    case (int)Gamestates.BlackW:
                        // add # symbol for checkmate
                        move = move + "#";
                        undoMovesAllowed = false;
                        blackPlayer.AddBlackWin();
                        whitePlayer.AddWhiteLoss();
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
                    SelectPiece(squareIndex);
                    //push move if promo, just don't forGet to pop it later
                }
                moveList.Push(move);
                db.UpdateGame(moveList.ConvertToString(), state, ID);
                db.UpdatePlayer(whitePlayer);
                db.UpdatePlayer(blackPlayer);
            }

        }

        private void EvalGamestate()
        {
            state = board.GetGamestate(playerTurn);
            //end game if gamestate isn't ongoing
            if (state != (int)Gamestates.Ongoing) { playerTurn = -1; }
        }

        public void PromotePawn(string pieceType)
        {
            bool success = board.PromotePawn(pieceType, pendingMove);
            if (success)
            {
                pendingMove = -1;
                string move = moveList.Pop();
                move = move + pieceType;
                moveList.Push(move);
                playerTurn = (playerTurn + 1) % 2;
                state = (int)Gamestates.Ongoing;
                db.UpdateGame(moveList.ConvertToString(), state, ID);
            }
            UpdateViewport();
        }

        public void SetViewDirection(int mode)
        {
            //if piece is selected, use that piece's relevant co-ord as layer value
            viewDir = mode;
            if (board.IsPieceSelected()) 
            {
                int[] pieceVect = board.GetSelectedPiece().GetCurrentPosAsVect();
                switch (mode)
                {
                    case (int)ViewDirections.Front:
                        viewLayer = pieceVect[2];
                        break;
                    case (int)ViewDirections.Side:
                        viewLayer = pieceVect[0];
                        break;
                    case (int)ViewDirections.Top:
                        viewLayer = pieceVect[1];
                        break;
                }
            }
            else
            {
                //default for side viewlayer is 7 because I just can't understand the game otherwise for some reason
                if(viewDir == (int)ViewDirections.Side) { viewLayer = 7; }
                else if(changeBoardDir)
                {
                    //otherwise sets viewlayer to 0/7 depending on player turn - can be toggled as a gamerule
                    if(playerTurn == 0) { viewLayer = 7; }
                    else { viewLayer = 0; }
                }
            }
            UpdateViewport();
        }

        public void IncrementViewLayer() { if (viewLayer < 7) { viewLayer++; UpdateViewport(); } }
        public void DecrementViewLayer() { if(viewLayer > 0) { viewLayer--; UpdateViewport(); } }

        private void UpdateViewport()
        {
            int originPointer = 0;
            //grab pointers for relevant slice of board
            switch (viewDir)
            {
                case (int)ViewDirections.Front:
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
                case (int)ViewDirections.Side:
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
                case (int)ViewDirections.Top:
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

        public Square GetViewportCell(int ptr)
        {
            //takes a one dimensional pointer for the viewport
            //defensive programming
            if(ptr > -1 && ptr < 64)
            {
                return board.GetSquare(viewport[ptr]);
            }
            else { throw new ArgumentOutOfRangeException(); }
        }

        public int GetViewDirection()
        {
            return viewDir;
        }

        public int GetViewLayer()
        {
            //add one for user readability
            return viewLayer + 1;
        }

        private void SelectPiece(int squareIndex)
        {
            board.SelectPiece(squareIndex, playerTurn);
        }

        //ONLY USE FOR LOADING GAME DATA - formatted by this program or otherwise assuming notation is perfect
        private void ParseMove(string m)
        {
            //assumes move is valid
            moveList.Push(m);
            board.ParseMove(m, playerTurn);
            playerTurn = (playerTurn + 1) % 2;
            EvalGamestate();
        }

        public void UndoMove()
        {
            if (!moveList.IsEmpty() && undoMovesAllowed)
            {
                string m = moveList.Pop();
                board.UndoMove(m);
                playerTurn = (playerTurn + 1) % 2;
                switch (state)
                {
                    case (int)Gamestates.WhiteW:
                        playerTurn = (int)Colours.White;
                        break;
                    case (int)Gamestates.BlackW:
                        playerTurn = (int)Colours.Black;
                        break;
                    case (int)Gamestates.Stalemate:
                        playerTurn = (moveList.Count() + 1) % 2;
                        break;
                }
                EvalGamestate();
                db.UpdateGame(moveList.ConvertToString(), state, ID);
            }
        }

        //use this for handling accepted draws/forfeits
        public void ManualEndGame(int outcome)
        {
            switch (outcome)
            {
                //lock undo once game is complete, if game is reopened from menu it opens a undoable copy
                case (int)Gamestates.Stalemate:
                    //mutual agreement is the correct term for a agreed upon draw
                    moveList.Push("Mutual Agreement÷");
                    state = (int)Gamestates.Stalemate;
                    undoMovesAllowed = false;
                    whitePlayer.AddWhiteDraw();
                    blackPlayer.AddBlackDraw();
                    playerTurn = -1;
                    break;
                case (int)Gamestates.WhiteW:
                    // black forfeit
                    moveList.Push("Black#");
                    state = (int)Gamestates.WhiteW;
                    undoMovesAllowed = false;
                    whitePlayer.AddWhiteWin();
                    blackPlayer.AddBlackLoss();
                    playerTurn = -1;
                    break;
                case (int)Gamestates.BlackW:
                    // white forfeit
                    moveList.Push("White#");
                    state = (int)Gamestates.BlackW;
                    undoMovesAllowed = false;
                    blackPlayer.AddBlackWin();
                    whitePlayer.AddWhiteLoss();
                    playerTurn = -1;
                    break;
            }
            db.UpdateGame(moveList.ConvertToString(), state, ID);
            db.UpdatePlayer(whitePlayer);
            db.UpdatePlayer(blackPlayer);
        }

        public Player GetCurrentPlayer()
        {
            Player ret;
            if(playerTurn == 0) { ret = blackPlayer; }
            else { ret = whitePlayer; }
            return ret;
        }

        public Piece GetPieceDirect(int ptr)
        {
            return board.GetPieceDirect(ptr);
        }

        public bool GetInCheck() { return inCheck; }
        public int GetGamestate() { return state; }
        public bool GetIsUndoAllowed() { return undoMovesAllowed; }

        public Stack<string> GetMoveList() { return moveList.Clone(); }

        public string GetWhitePlayerName() { return whitePlayer.GetName(); }
        public string GetBlackPlayerName() { return blackPlayer.GetName(); }
        public string GetLastMove() { return moveList.Peek(); }
        public Square GetCell(int index) { return board.GetSquare(index); }
    }
}
