using Raylib_cs;
using System.Numerics;

namespace ThreeDimensionalChess
{
    class UserInterface
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

        enum PieceTextureIndices
        {
            //another neat little enum to deal with loading textures from the texture list
            WhitePawn, // 0
            WhiteRook, // 1
            WhiteKnight, // 2
            WhiteBishop, // 3
            WhiteQueen, // 4
            WhiteKing, // 5
            BlackPawn, // 6
            BlackRook, // 7
            BlackKnight, // 8
            BlackBishop, // 9
            BlackQueen, // 10
            BlackKing // 11
        }

        enum UIModes
        {
            MainMenu, // 0
            PlayersList, // 1
            NewLoadChoice, // 2
            NewGameMenu, // 3
            GamesList, // 4
            ConfirmGame, // 5
            CreatePlayer, // 6
            GameUI2D, // 7
            PauseMenu, // 8
        }

        static void Main()
        {
            Raylib.InitWindow(UIConstants.windowWidth, UIConstants.windowHeight, "Three-Dimensional Chess");
            //make it so esc doesn't close window
            Raylib.SetExitKey(0);
            Chess game = null;
            int mode = (int)UIModes.MainMenu;
            DatabaseHandler database = new DatabaseHandler();

            //load textures
            List<Texture2D> textures = new List<Texture2D>();
            Texture2D WhitePawn = Raylib.LoadTexture("resources/WhP.png");
            textures.Add(WhitePawn); // 0
            Texture2D WhiteRook = Raylib.LoadTexture("resources/WhR.png");
            textures.Add(WhiteRook); // 1
            Texture2D WhiteKnight = Raylib.LoadTexture("resources/WhN.png");
            textures.Add(WhiteKnight); // 2
            Texture2D WhiteBishop = Raylib.LoadTexture("resources/WhB.png");
            textures.Add(WhiteBishop); // 3
            Texture2D WhiteQueen = Raylib.LoadTexture("resources/WhQ.png");
            textures.Add(WhiteQueen); // 4
            Texture2D WhiteKing = Raylib.LoadTexture("resources/WhK.png");
            textures.Add(WhiteKing); // 5
            Texture2D BlackPawn = Raylib.LoadTexture("resources/BlP.png");
            textures.Add(BlackPawn); // 6
            Texture2D BlackRook = Raylib.LoadTexture("resources/BlR.png");
            textures.Add(BlackRook); // 7
            Texture2D BlackKnight = Raylib.LoadTexture("resources/BlN.png");
            textures.Add(BlackKnight); // 8
            Texture2D BlackBishop = Raylib.LoadTexture("resources/BlB.png");
            textures.Add(BlackBishop); // 9
            Texture2D BlackQueen = Raylib.LoadTexture("resources/BlQ.png");
            textures.Add(BlackQueen); //10
            Texture2D BlackKing = Raylib.LoadTexture("resources/BlK.png");
            textures.Add(BlackKing); //11

            //some button placements, store Rec structures here to make life easier
            // rectangle struct - {xOrigin, yOrigin, width, height}
            // can't place in constants because they are objects
            // -- Main Menu Rectangles -- 
            Vector2 titlePos = new Vector2(200, 200);
            Rectangle exitButton = new Rectangle(250, 400, 500 / 3, 100);
            bool exitButtonClicked = false;
            Rectangle playButton = new Rectangle(250 + (500/3), 400, 500 / 3, 100);
            Rectangle playerButton = new Rectangle(250 + 2*(500/3), 400, 500 / 3, 100);
            // -- Players List Values
            int playerListIndex = 0;
            int selectedPlayerID = -1;
            string sortMode = "ID"; // 0 is ascending by default, 1 is reverse
            int sortOrder = 0;
            List<Player> playersList = new List<Player>();
            Rectangle deletePlayerButton = new Rectangle(805, 10, UIConstants.windowWidth - 805 - 5, 75);
            Rectangle newPlayerButton = new Rectangle(805, 95, UIConstants.windowWidth - 805 - 5, 75);
            Rectangle backFromPlayers = new Rectangle(UIConstants.windowWidth - 85, UIConstants.windowHeight - 85, 75, 75);
            // -- Game UI 2D Rectangles --
            Rectangle frontButton = new Rectangle(10, 10, 200, 75);
            Rectangle topButton = new Rectangle(10, 95, 200, 75);
            Rectangle sideButton = new Rectangle(10, 180, 200, 75);

            Rectangle queenPromoRec = new Rectangle(10, 265, 100, 100);
            Rectangle rookPromoRec = new Rectangle(110, 265, 100, 100);
            Rectangle bishopPromoRec = new Rectangle(10, 365, 100, 100);
            Rectangle knightPromoRec = new Rectangle(110, 365, 100, 100);


            while (!Raylib.WindowShouldClose() && !exitButtonClicked)
            {
                // ------ update and input here ------
                // switch for current part of UI
                switch (mode)
                {
                    case (int)UIModes.MainMenu:
                        if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
                        {
                            Vector2 mousePos = Raylib.GetMousePosition();
                            exitButtonClicked = Raylib.CheckCollisionPointRec(mousePos, exitButton);
                            bool playButtonClicked = Raylib.CheckCollisionPointRec(mousePos, playButton);
                            bool playersButtonClicked = Raylib.CheckCollisionPointRec(mousePos, playerButton);
                            if (playButtonClicked) { mode = (int)UIModes.NewLoadChoice; }
                            if (playersButtonClicked) { mode = (int)UIModes.PlayersList; }
                        }
                        break;
                    case (int)UIModes.PlayersList:
                        if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
                        {
                            Vector2 mousePos = Raylib.GetMousePosition();

                            //take clicks on the table
                            if(10 <= mousePos.X && mousePos.X <= 800 && 10 <= mousePos.Y && mousePos.Y <= (10 + (13*50)))
                            {
                                int row = ((int)mousePos.Y - 10) / 50;
                                int column = -1;
                                if (10 <= mousePos.X && mousePos.X < 200) { column = 0; }
                                else if (200 <= mousePos.X && mousePos.X < 300) { column = 1; }
                                else if (300 <= mousePos.X && mousePos.X < 500) { column = 2; }
                                else if(500 <= mousePos.X && mousePos.X < 650) { column = 3; }
                                else { column = 4; }

                                //affect table now
                                
                                if(row == 0)
                                {
                                    string prevMode = sortMode;
                                    switch (column)
                                    {
                                        case 0:
                                            sortMode = "name";
                                            break;
                                        case 1:
                                            sortMode = "date";
                                            break;
                                        case 2:
                                            sortMode = "winRatio";
                                            break;
                                        case 3:
                                            sortMode = "whiteWR";
                                            break;
                                        case 4:
                                            sortMode = "blackWR";
                                            break;
                                    }
                                    if (prevMode == sortMode) { sortOrder = (sortOrder + 1) % 2; }
                                }
                                else
                                {
                                    row--;
                                    if(row < playersList.Count()) { selectedPlayerID = playersList[row].getID(); }
                                }
                            }

                            bool createPlayerPressed = Raylib.CheckCollisionPointRec(mousePos, newPlayerButton);
                            bool deletePlayerPressed = Raylib.CheckCollisionPointRec(mousePos, deletePlayerButton);
                            bool backFromPlayersPressed = Raylib.CheckCollisionPointRec(mousePos, backFromPlayers);

                            if (createPlayerPressed) { database.addPlayer("TEST"); }
                            if(deletePlayerPressed && selectedPlayerID != -1) { database.deletePlayer(selectedPlayerID); selectedPlayerID = -1; }
                            if (backFromPlayersPressed) { mode = (int)UIModes.MainMenu; }
                        }
                        break;
                    case (int)UIModes.GameUI2D:
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
                            bool frontButtonPressed = Raylib.CheckCollisionPointRec(mousePos, frontButton);
                            bool topButtonPressed = Raylib.CheckCollisionPointRec(mousePos, topButton);
                            bool sideButtonPressed = Raylib.CheckCollisionPointRec(mousePos, sideButton);
                            if (frontButtonPressed)
                            {
                                game.setViewDirection((int)viewDirections.Front);
                            }
                            else if (topButtonPressed)
                            {
                                game.setViewDirection((int)viewDirections.Top);
                            }
                            else if (sideButtonPressed)
                            {
                                game.setViewDirection((int)viewDirections.Side);
                            }

                            if (game.getGamestate() == (int)Gamestates.PendingPromo)
                            {
                                //check for pawn promotion selection ehre
                                bool queenSelected = Raylib.CheckCollisionPointRec(mousePos, queenPromoRec);
                                bool rookSelected = Raylib.CheckCollisionPointRec(mousePos, rookPromoRec);
                                bool bishopSelected = Raylib.CheckCollisionPointRec(mousePos, bishopPromoRec);
                                bool knightSelected = Raylib.CheckCollisionPointRec(mousePos, knightPromoRec);
                                if (queenSelected) { game.promotePawn("Q"); }
                                else if (rookSelected) { game.promotePawn("R"); }
                                else if (bishopSelected) { game.promotePawn("B"); }
                                else if (knightSelected) { game.promotePawn("N"); }
                            }
                        }

                        //step through board using keys
                        if (Raylib.IsKeyPressed(KeyboardKey.KEY_UP)) { game.incrementViewLayer(); }
                        if (Raylib.IsKeyPressed(KeyboardKey.KEY_DOWN)) { game.decrementViewLayer(); }
                        break;
                }

                // ------ draw here ------
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.WHITE);
                //switch to draw elements based on Ui mode
                switch (mode)
                {
                    case (int)UIModes.MainMenu:
                        updateMainMenu(titlePos, exitButton, playButton, playerButton);
                        break;
                    case (int)UIModes.PlayersList:
                        playersList = updatePlayersTable(playerListIndex, database, sortMode, sortOrder);
                        updatePlayersListButtons(deletePlayerButton, newPlayerButton, backFromPlayers);
                        break;
                    case (int)UIModes.GameUI2D:
                        int state = game.getGamestate();

                        updateBoard(game, textures);
                        updateViewPortControls(game, frontButton, topButton, sideButton);
                        if (state == (int)Gamestates.PendingPromo) { updatePromoWindow(game, queenPromoRec, rookPromoRec, bishopPromoRec, knightPromoRec, textures); }
                        break;
                }

                Raylib.EndDrawing();
            }

            //unload textures
            for(int i = 0; i < textures.Count(); i++)
            {
                Raylib.UnloadTexture(textures[i]);
            }


            Raylib.CloseWindow();
        }

        static void updateMainMenu(Vector2 textPos, Rectangle exit, Rectangle play,  Rectangle player)
        {
            Raylib.DrawText("Three Dimensional Chess", (int)textPos.X, (int)textPos.Y, 50, Color.BLACK);
            Raylib.DrawRectangleLinesEx(exit, 1, Color.BLACK);
            Raylib.DrawText("Exit", (int)exit.X + 45, (int)exit.Y + 30, 40, Color.BLACK);
            Raylib.DrawRectangleLinesEx(play, 1, Color.BLACK);
            Raylib.DrawText("Play", (int)play.X + 45, (int)play.Y + 30, 40, Color.BLACK);
            Raylib.DrawRectangleLinesEx(player, 1, Color.BLACK);
            Raylib.DrawText("Players", (int)player.X + 5, (int)player.Y + 30, 40, Color.BLACK);
        }

        static List<Player> updatePlayersTable(int startIndex, DatabaseHandler db, string sortMode, int sortOrder) 
        {
            //get a list of all players
            List<Player> players = db.getPlayers();
            switch (sortMode)
            {
                case "name":
                    
                    break;
            }
            //setup rectangles to be transformed
            Rectangle baseRec = new Rectangle(10, 10, 790, 50);
            //draw columns in here
            Raylib.DrawLine(200, 10, 200, 660, Color.BLACK);
            Raylib.DrawText("Name", 15, 23, 30, Color.BLACK);
            Raylib.DrawLine(300, 10, 300, 660, Color.BLACK);
            Raylib.DrawText("Date", 205, 23, 30, Color.BLACK);
            Raylib.DrawLine(500, 10, 500, 660, Color.BLACK);
            Raylib.DrawText("W/L/D", 305, 23, 30, Color.BLACK);
            Raylib.DrawLine(650, 10, 650, 660, Color.BLACK);
            Raylib.DrawText("White WR", 505, 23, 30, Color.BLACK);
            Raylib.DrawText("Black WR", 655, 23, 30, Color.BLACK);
            // draw outline
            Raylib.DrawRectangleLines(10, 10, 790, 50 + (12*50), Color.BLACK);

            for (int y = 0; y < (660 / 50); y++)
            {
                Raylib.DrawRectangleLinesEx(new Rectangle(baseRec.X, baseRec.Y + (y * 50), baseRec.Width, baseRec.Height), 1, Color.BLACK);
                //fill in with values from table
                if (y > 0 && startIndex + y - 1 < players.Count()) {
                    
                    Player tmp = players[startIndex + y - 1];
                    //using position values from columns drawn above
                    string playerName = tmp.getName();
                    //shorten name if it would write over column
                    if(playerName.Length > 11){ playerName = playerName.Substring(0, 11);}
                    Raylib.DrawText(playerName, 15, 23 + (y * 50), 30, Color.BLACK);
                    DateOnly joinDate = tmp.getJoinDate();
                    Raylib.DrawText(joinDate.ToString().Substring(0, joinDate.ToString().Length - 5), 205, 23 + (y * 50), 30, Color.BLACK);
                    string winRatio = tmp.getTotalWins() + "/" + tmp.getTotalLosses() + "/" + tmp.getDraws();
                    Raylib.DrawText(winRatio, 305, 23 + (y * 50), 30, Color.BLACK);
                    string whiteWR = tmp.getWhiteWinrate() + "%";
                    string blackWR = tmp.getBlackWinrate() + "%";
                    Raylib.DrawText(whiteWR, 505, 23 + (y * 50), 30, Color.BLACK);
                    Raylib.DrawText(blackWR, 655, 23 + (y*50), 30, Color.BLACK);
                }
            }
            return players;
        }

        static void updatePlayersListButtons(Rectangle delete, Rectangle create, Rectangle back)
        {
            Raylib.DrawRectangleLinesEx(delete, 1, Color.BLACK);
            Raylib.DrawText("Delete", (int)delete.X + 25, (int)delete.Y + 25, 30, Color.BLACK);
            Raylib.DrawRectangleLinesEx(create, 1, Color.BLACK);
            Raylib.DrawText("Add Player", (int)create.X + 10, (int)create.Y + 25, 30, Color.BLACK);
            Raylib.DrawRectangleLinesEx(back, 1, Color.BLACK);
        }

        static void updateBoard(Chess game, List<Texture2D> textures)
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

                        string type = p.getPieceType();
                        int col = p.getColour();
                        if (col == (int)Colours.White)
                        {
                            //draw each texture, referencing list using above indices, + 4 to each pos to centre 60x60 texture
                            switch (type)
                            {
                                case "P":
                                    Raylib.DrawTexture(textures[0], xPos + 4, yPos + 4, Color.WHITE);
                                    break;
                                case "R":
                                    Raylib.DrawTexture(textures[1], xPos + 4, yPos + 4, Color.WHITE);
                                    break;
                                case "N":
                                    Raylib.DrawTexture(textures[2], xPos + 4, yPos + 4, Color.WHITE);
                                    break;
                                case "B":
                                    Raylib.DrawTexture(textures[3], xPos + 4, yPos + 4, Color.WHITE);
                                    break;
                                case "Q":
                                    Raylib.DrawTexture(textures[4], xPos + 4, yPos + 4, Color.WHITE);
                                    break;
                                case "K":
                                    Raylib.DrawTexture(textures[5], xPos + 4, yPos + 4, Color.WHITE);
                                    break;
                            }
                        }
                        else
                        {
                            switch (type) {
                                case "P":
                                    Raylib.DrawTexture(textures[6], xPos + 4, yPos + 4, Color.WHITE);
                                    break;
                                case "R":
                                    Raylib.DrawTexture(textures[7], xPos + 4, yPos + 4, Color.WHITE);
                                    break;
                                case "N":
                                    Raylib.DrawTexture(textures[8], xPos + 4, yPos + 4, Color.WHITE);
                                    break;
                                case "B":
                                    Raylib.DrawTexture(textures[9], xPos + 4, yPos + 4, Color.WHITE);
                                    break;
                                case "Q":
                                    Raylib.DrawTexture(textures[10], xPos + 4, yPos + 4, Color.WHITE);
                                    break;
                                case "K":
                                    Raylib.DrawTexture(textures[11], xPos + 4, yPos + 4, Color.WHITE);
                                    break;
                            } 
                        }
                        
                    }
                }
            }
        }

        static void updateViewPortControls(Chess game, Rectangle frontButton, Rectangle topButton, Rectangle sideButton)
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
            Raylib.DrawRectangleLinesEx(frontButton, 1, Color.BLACK);
            Raylib.DrawText("Front", 68, 35, 30, Color.BLACK);
            //top view button
            Raylib.DrawRectangleLinesEx(topButton, 1, Color.BLACK);
            Raylib.DrawText("Top", 68, 120, 30, Color.BLACK);
            //side view button
            Raylib.DrawRectangleLinesEx(sideButton, 1, Color.BLACK);
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

        static void updatePromoWindow(Chess game, Rectangle queenRec, Rectangle rookRec, Rectangle bishopRec, Rectangle knightRec, List<Texture2D> textures)
        {
            //draw cells for promotion - borders make collisions clear            
            Rectangle sourceRec = new Rectangle(0, 0, 60, 60); // for loading texture from source

            Raylib.DrawRectangleLinesEx(queenRec, 1, Color.BLACK);
            Raylib.DrawRectangleLinesEx(rookRec, 1, Color.BLACK);
            Raylib.DrawRectangleLinesEx(bishopRec, 1, Color.BLACK);
            Raylib.DrawRectangleLinesEx(knightRec, 1, Color.BLACK);

            //draw relevant player's pieces
            if (game.getCurrentPlayer().getColour() == 0)
            {
                Raylib.DrawTexturePro(textures[(int)PieceTextureIndices.BlackQueen], sourceRec, queenRec, new Vector2(0, 0), 0, Color.WHITE);
                Raylib.DrawTexturePro(textures[(int)PieceTextureIndices.BlackRook], sourceRec, rookRec, new Vector2(0, 0), 0, Color.WHITE);
                Raylib.DrawTexturePro(textures[(int)PieceTextureIndices.BlackBishop], sourceRec, bishopRec, new Vector2(0, 0), 0, Color.WHITE);
                Raylib.DrawTexturePro(textures[(int)PieceTextureIndices.BlackKnight], sourceRec, knightRec, new Vector2(0, 0), 0, Color.WHITE);
            }// this else case should only arise if player is white but doesn't hurt to check
            else if(game.getCurrentPlayer().getColour() == 1)
            {
                Raylib.DrawTexturePro(textures[(int)PieceTextureIndices.WhiteQueen], sourceRec, queenRec, new Vector2(0, 0), 0, Color.WHITE);
                Raylib.DrawTexturePro(textures[(int)PieceTextureIndices.WhiteRook], sourceRec, rookRec, new Vector2(0, 0), 0, Color.WHITE);
                Raylib.DrawTexturePro(textures[(int)PieceTextureIndices.WhiteBishop], sourceRec, bishopRec, new Vector2(0, 0), 0, Color.WHITE);
                Raylib.DrawTexturePro(textures[(int)PieceTextureIndices.WhiteKnight], sourceRec, knightRec, new Vector2(0, 0), 0, Color.WHITE);
            }

            Raylib.DrawText("Promotion", 40, 467, 30, Color.BLACK);
        }

    }
}