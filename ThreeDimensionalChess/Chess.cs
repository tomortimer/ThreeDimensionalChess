﻿using System;
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
        Ongoing // 3
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
        private Board board;
        private Stack<string> moveList;
        private int whitePlayerID;
        //array of pointers that compose the 2D viewport
        private int[] viewport;
        private int viewDir;
        private int viewLayer;
        private int blackPlayerID;
        private int state;

        public Chess(int whiteID, int blackID)
        {
            //init variables to defaults - starting turn, starting check and gamestate & create board
            playerTurn = (int)Colours.White;
            board = new Board();
            inCheck = false;
            moveList = new Stack<string>();
            //init player objects, grab name based on ID from db
            whitePlayerID = whiteID;
            blackPlayerID = blackID;
            state = (int)Gamestates.Ongoing;
            //init viewport (as front view white)
            viewLayer = 0;
            viewDir = 0;
            viewport = new int[64];
            updateViewport();
        }

        public void click(int squareIndex)
        {
            //can't do anything once game is over - separate method for rewind and view buttons
            if (playerTurn != -1)
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
                moveList.Push(move);
            }

        }

        private void evalGamestate()
        {
            state = board.getGamestate(playerTurn);
            //end game if gamestate isn't ongoing
            if (state != (int)Gamestates.Ongoing) { playerTurn = -1; } //FIXME: BEHAVIOUR WITH -1 PLAYER IS UNDEFINED -> ADD DEFENSIVE PROGRAMMING
        }

        public void changeView(int mode)
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