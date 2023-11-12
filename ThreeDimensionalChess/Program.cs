﻿using Raylib_cs;
using System.Numerics;

namespace ThreeDimensionalChess
{
    class Program
    {
        static class UIConstants
        {
            /* ----- graphical notes -----
            origin points to start drawing for board - 550x550
            top left: (225, 24)(due to integer div) bottom left: (225, 500)
            top right: (769, 24)(769 due to 550/8 = 68.75) bottom right: (769, 500)
            is close enough to desired values
            -------------------------------
            viewport controls area:
            (225, 24) to (769, 500)
             */
            
            public const int boardXOrigin = 225;
            public const int boardYOrigin = 500;
            public const int squareSide = 550 / 8;


            public const int windowWidth = 1000;
            public const int windowHeight = 680;
        }

        static void Main()
        {
            Raylib.InitWindow(1000, 680, "Three-Dimensional Chess");
            //make it so esc doesn't close window
            Raylib.SetExitKey(0);
            Chess game = new Chess(8232, 121);
            game.addPiece("N", 13, 0); // knight for testing

            while (!Raylib.WindowShouldClose())
            {
                // ------ update and input here ------
                //track mouse clicks
                if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
                {
                    //take mouse x y and find which element was clicked using coordinate geometry
                    int[] mousePos = { Raylib.GetMouseX(), Raylib.GetMouseY() };

                    //this is if mouse is in board borders
                    if (225 <= mousePos[0] && mousePos[0] <= 769 && 24 <= mousePos[1] && mousePos[1] <= (500 + UIConstants.squareSide))
                    {
                        //calculate square index
                        int x = (mousePos[0] - 225) / UIConstants.squareSide;
                        int y = 7 - ((mousePos[1] - 25) / UIConstants.squareSide);
                        game.viewportClick(x + (y * 8));
                    }
                }
                //step through board
                if (Raylib.IsKeyPressed(KeyboardKey.KEY_UP)) { game.incrementViewLayer(); }
                if (Raylib.IsKeyPressed(KeyboardKey.KEY_DOWN)) { game.decrementViewLayer(); }
                // ------ draw here ------
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.WHITE);

                updateBoard(game);
                updateViewPortControls(game);


                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();
        }

        static void updateBoard(Chess game)
        {
            int offset = UIConstants.squareSide;
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    //grab the relevant cell from the viewport array in the game
                    Square cell = game.getViewportCell((y * 8) + x);
                    //calculate the draw areas
                    int xPos = UIConstants.boardXOrigin + (x * offset);
                    int yPos = UIConstants.boardYOrigin - (y * offset);
                    //draw filled if black square, draw outline if white square
                    switch (cell.getColour())
                    {
                        case (int)Colours.Black:
                            Raylib.DrawRectangle(xPos, yPos, offset, offset, Color.BLACK);
                            break;
                        case (int)Colours.White:
                            Raylib.DrawRectangleLines(xPos, yPos, offset, offset, Color.BLACK);
                            break;
                        case (int)Colours.BlackBlue:
                            Raylib.DrawRectangle(xPos, yPos, offset, offset, Color.DARKBLUE);
                            break;
                        case (int)Colours.WhiteBlue:
                            Raylib.DrawRectangle(xPos, yPos, offset, offset, Color.BLUE);
                            break;
                        case (int)Colours.BlackRed:
                            Raylib.DrawRectangle(xPos, yPos, offset, offset, Color.MAROON);
                            break;
                        case (int)Colours.WhiteRed:
                            Raylib.DrawRectangle(xPos, yPos, offset, offset, Color.RED);
                            break;
                        case (int)Colours.BlackYellow:
                            Raylib.DrawRectangle(xPos, yPos, offset, offset, Color.BLACK);
                            Raylib.DrawRectangle(xPos, yPos, offset, offset, Raylib.Fade(Color.YELLOW, 0.8f));
                            break;
                        case (int)Colours.WhiteYellow:
                            Raylib.DrawRectangle(xPos, yPos, offset, offset, Color.YELLOW);
                            break;
                    }
                    if (cell.getPiecePointer() != -1)
                    {
                        Piece p = game.getPieceDirect(cell.getPiecePointer());
                        Raylib.DrawText(p.getPieceType(), xPos + (offset / 2), yPos + (offset / 2), 30, Color.LIME);
                    }
                }
            }
        }

        static void updateViewPortControls(Chess game)
        {
            //draw control triangles
            Raylib.DrawTriangle(new Vector2(100, 10), new Vector2(10, 100), new Vector2(190, 100), Color.RED);
            //draw text to show 3d coords
            string coordText = "X: x, Y: x, Z: x";
            switch (game.getViewDirection())
            {
                case (int)viewDirections.Front:
                    coordText = coordText.Substring(0, 15) + game.getViewLayer();
                    break;
                case (int)viewDirections.Side:
                    coordText = coordText.Substring(0, 3) + game.getViewLayer() + coordText.Substring(4);
                    break;
                case (int)viewDirections.Top:
                    coordText = coordText.Substring(0, 8) + game.getViewLayer() + coordText.Substring(9);
                    break;
            }
            Raylib.DrawText(coordText, 420, 575, 25, Color.BLACK);
        }

    }
}