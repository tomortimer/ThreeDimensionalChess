using Raylib_cs;
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
            game.addPiece("P", 14, 0);

            while (!Raylib.WindowShouldClose())
            {
                // ------ update and input here ------
                //track mouse clicks
                if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
                {
                    //take mouse x y and find which element was clicked using coordinate geometry
                    Vector2 mousePos = Raylib.GetMousePosition();

                    //this is if mouse is in board borders
                    if (225 <= mousePos.X && mousePos.X <= 769 && 24 <= mousePos.Y && mousePos.Y <= (500 + UIConstants.squareSide))
                    {
                        //calculate square index
                        int x = (Convert.ToInt32(mousePos.X) - 225) / UIConstants.squareSide;
                        int y = 7 - ((Convert.ToInt32(mousePos.Y) - 25) / UIConstants.squareSide);
                        game.viewportClick(x + (y * 8));
                    }

                    //step through board using triangle buttons
                    bool upButtonPressed = Raylib.CheckCollisionPointTriangle(mousePos, new Vector2(360, 570), new Vector2(330, 600), new Vector2(390, 600));
                    bool downButtonPressed = Raylib.CheckCollisionPointTriangle(mousePos, new Vector2(610, 570), new Vector2(640, 600), new Vector2(670, 570));
                    //could list all buttons and give them numerical values then switch statement here?
                    if (upButtonPressed) { game.incrementViewLayer(); }
                    if (downButtonPressed) { game.decrementViewLayer(); }

                    //check collision with viewDirection buttons
                    if(10 <= mousePos.X && mousePos.X <= 210 && 10 <= mousePos.Y && mousePos.Y <= 85)
                    {
                        game.setViewDirection((int)viewDirections.Front);
                    }else if (10 <= mousePos.X && mousePos.X <= 210 && 95 <= mousePos.Y && mousePos.Y <= 170)
                    {
                        game.setViewDirection((int)viewDirections.Top);
                    }else if (10 <= mousePos.X && mousePos.X <= 210 && 180 <= mousePos.Y && mousePos.Y <= 255)
                    {
                        game.setViewDirection((int)viewDirections.Side);
                    }
                }

                //step through board using keys
                if (Raylib.IsKeyPressed(KeyboardKey.KEY_UP)) { game.incrementViewLayer(); }
                if (Raylib.IsKeyPressed(KeyboardKey.KEY_DOWN)) { game.decrementViewLayer(); }

                

                // ------ draw here ------
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.WHITE);
                int state = game.getGamestate();

                updateBoard(game);
                updateViewPortControls(game);
                if(state == (int)Gamestates.PendingPromo) { updatePromoWindow(game); }


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

                        string type = p.getPieceType();
                        Image piece;
                        switch (type)
                        {
                            case "K":
                                piece = Raylib.LoadImage()
                        }
                    }
                }
            }
        }

        static void updateViewPortControls(Chess game)
        {
            //draw control triangles
            //up triangle
            Color upCol = Color.BLACK;
            if(game.getViewLayer() == 8) { upCol = Color.GRAY; }
            Raylib.DrawTriangle(new Vector2(360, 570), new Vector2(330, 600), new Vector2(390, 600), upCol);
            //down triangle
            Color downCol = Color.BLACK;
            if(game.getViewLayer() == 1) { downCol = Color.GRAY; }
            Raylib.DrawTriangle(new Vector2(610, 570), new Vector2(640, 600), new Vector2(670, 570), downCol);

            //draw view buttons
            //front view button
            Raylib.DrawRectangleLines(10, 10, 200, 75, Color.BLACK);
            Raylib.DrawText("Front", 68, 35, 30, Color.BLACK);
            //top view button
            Raylib.DrawRectangleLines(10, 95, 200, 75, Color.BLACK);
            Raylib.DrawText("Top", 68, 120, 30, Color.BLACK);
            //side view button
            Raylib.DrawRectangleLines(10, 180, 200, 75, Color.BLACK);
            Raylib.DrawText("Side", 68, 205, 30, Color.BLACK);

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
            Raylib.DrawText(coordText, 395, 575, 30, Color.BLACK);
        }

        static void updatePromoWindow(Chess game)
        {

        }

    }
}