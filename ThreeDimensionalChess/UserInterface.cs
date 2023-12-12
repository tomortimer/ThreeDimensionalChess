using Raylib_cs;
using System;
using System.Numerics;
using System.Reflection.PortableExecutable;

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
            ConfirmForfeitStalemate, // 9
            GameUI3D // 10
        }

        static void Main()
        {
            Raylib.InitWindow(UIConstants.windowWidth, UIConstants.windowHeight, "Three-Dimensional Chess");
            //make it so esc doesn't close window
            Raylib.SetExitKey(0);
            Chess game = null;
            int mode = (int)UIModes.MainMenu;
            //set this as necessary
            int lastMode = 0;
            DatabaseHandler database = new DatabaseHandler();
            Raylib.InitAudioDevice();

            //load sounds
            Sound emptySquare = Raylib.LoadSound("resources/emptySquare.mp3");
            Sound captureSquare = Raylib.LoadSound("resources/captureSquare.mp3");

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
            // -- Players List Values --
            int playerListIndex = 0;
            int selectedPlayerID = -1;
            string sortMode = "ID"; // 0 is ascending by default, 1 is reverse
            int sortOrder = 0;
            List<Player> playersList = new List<Player>();
            Rectangle firstListButton = new Rectangle(805, 10, UIConstants.windowWidth - 805 - 5, 75);
            Rectangle secondListButton = new Rectangle(805, 95, UIConstants.windowWidth - 805 - 5, 75);
            Rectangle backButton = new Rectangle(UIConstants.windowWidth - 85, UIConstants.windowHeight - 85, 75, 75);
            // -- Create Player Values --
            string entryStr = "";
            Rectangle finaliseNewPlayerButton = new Rectangle(350, 300, 300, 100);
            Rectangle entryBox = new Rectangle(250, 190, 500, 100);
            // -- New / Load Game Choice Menu --
            Rectangle newGame = new Rectangle(400, 225, 200, 75);
            Rectangle loadGame = new Rectangle(400, 310, 200, 75);
            // -- Confirm Game Choice
            // -- New Game Menu --
            Rectangle whitePlayerDropDown = new Rectangle(10, 150, 300, 50);
            Rectangle addWhitePlayer = new Rectangle(320, 150, 50, 50);
            int whitePlayerID = 0;
            Rectangle blackPlayerDropDown = new Rectangle(690, 150, 300, 50);
            Rectangle addBlackPlayer = new Rectangle(630, 150, 50, 50);
            int blackPlayerID = 0;
            Rectangle undoMovesTickbox = new Rectangle(10, 410, 75, 75);
            Rectangle gameNameEntryBox = new Rectangle(250, 410, 500, 100);
            Rectangle startGameButton = new Rectangle(350, 520, 300, 100);
            bool gameCanStart = false;
            bool undoMovesChoice = true;
            int whitePlayerListIndex = -1; // when -1, drop down is closed
            int blackPlayerListIndex = -1;
            bool newBlackFlag = false;
            bool newWhiteFlag = false;
            // -- Load Game Values --
            List<GameInfo> games = new List<GameInfo>();
            int gameListIndex = 0;
            GameInfo selectedGame = null;
            // -- Game UI 2D Rectangles --
            Rectangle frontButton = new Rectangle(10, 10, 200, 75);
            Rectangle topButton = new Rectangle(10, 95, 200, 75);
            Rectangle sideButton = new Rectangle(10, 180, 200, 75);
            Rectangle queenPromoRec = new Rectangle(10, 265, 100, 100);
            Rectangle rookPromoRec = new Rectangle(110, 265, 100, 100);
            Rectangle bishopPromoRec = new Rectangle(10, 365, 100, 100);
            Rectangle knightPromoRec = new Rectangle(110, 365, 100, 100);
            Rectangle moveListRec = new Rectangle(780, 10, 210, 505);
            Rectangle undoMoveButton = new Rectangle(780, 525, 210, 50);
            // -- Pause Menu Elements --
            Rectangle resumeButton = new Rectangle(350, 150, 300, 75);
            Rectangle exitMenuButton = new Rectangle(350, 225, 300, 75);
            Rectangle exitDesktopButton = new Rectangle(350, 300, 300, 75);
            Rectangle whiteForfeitButton = new Rectangle(40, 500, 300, 75);
            Rectangle mutualAgreementStalemateButton = new Rectangle(350, 500, 300, 75);
            Rectangle blackForfeitButton = new Rectangle(660, 500, 300, 75);
            // -- Forfeit/Draw Menu --
            int proposedOutcome = -1;
            // -- Game Ui 3D --
            Rectangle viewmodeToggleButton = new Rectangle(10, 595, 75, 75);
            Camera3D camera = new Camera3D();
            camera.Position = new Vector3(10f, 10f, 10f);
            camera.Target = new Vector3(0f, 0f, 0f);
            camera.Up = new Vector3(0f, 1f, 0f);
            camera.FovY = 45f;
            camera.Projection = CameraProjection.CAMERA_PERSPECTIVE;
            Vector3 CubePos = new Vector3(0f, 0f, 0f);
            bool viewmodelWiresChoice = false;
            Rectangle wiremodeToggleButton = new Rectangle(10, 510, 75, 75);



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
                                    else { sortOrder = 0; }
                                }
                                else
                                {
                                    row--;
                                    if(row < playersList.Count()) { selectedPlayerID = playersList[row].GetID(); }
                                }
                            }

                            bool createPlayerPressed = Raylib.CheckCollisionPointRec(mousePos, secondListButton);
                            bool deletePlayerPressed = Raylib.CheckCollisionPointRec(mousePos, firstListButton);
                            bool backButtonPressed = Raylib.CheckCollisionPointRec(mousePos, backButton);

                            if (createPlayerPressed) { mode = (int)UIModes.CreatePlayer; lastMode = (int)UIModes.PlayersList; }
                            if(deletePlayerPressed && selectedPlayerID != -1) { database.DeletePlayer(selectedPlayerID); selectedPlayerID = -1; }
                            if (backButtonPressed) { mode = (int)UIModes.MainMenu; playerListIndex = 0; }
                        }
                        //use arrow keys to move up or down table
                        if (Raylib.IsKeyPressed(KeyboardKey.KEY_DOWN))
                        {
                            if(playerListIndex < playersList.Count() - 1) { playerListIndex++; }
                        }
                        if (Raylib.IsKeyPressed(KeyboardKey.KEY_UP))
                        {
                            if(playerListIndex > 0) { playerListIndex--; }
                        }
                        break;
                    case (int)UIModes.CreatePlayer:
                        if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
                        {
                            Vector2 mousePos = Raylib.GetMousePosition();

                            bool finaliseButtonPressed = Raylib.CheckCollisionPointRec(mousePos, finaliseNewPlayerButton);
                            bool backButtonPressed = Raylib.CheckCollisionPointRec(mousePos, backButton);
                            if (finaliseButtonPressed)
                            {
                                //shouldn't reach this without having moved from another mode so move back to that
                                if (entryStr != "") 
                                { 
                                    int ID = database.AddPlayer(entryStr); 
                                    mode = lastMode; 
                                    lastMode = 0;
                                    entryStr = "";
                                    if (newWhiteFlag) { whitePlayerID = ID; newWhiteFlag = false; }
                                    else if (newBlackFlag) { blackPlayerID = ID; newBlackFlag = false; }
                                }
                            }
                            if (backButtonPressed) { mode = lastMode; lastMode = 0; newBlackFlag = false; newWhiteFlag = false; }
                        }

                        //raylib uses an input queue for keys(if they are occuring on the same frame), parse this here
                        int keyPressed = Raylib.GetCharPressed();
                        while(keyPressed > 0)
                        {
                            //only accept character keys, limit names to 20 char (only 10 is displayed in table anyway)
                            if(keyPressed > 31 && keyPressed < 126 && entryStr.Length < 20)
                            {
                                entryStr += (char)keyPressed;
                            }
                            keyPressed = Raylib.GetCharPressed();
                        }
                        if (Raylib.IsKeyPressed(KeyboardKey.KEY_BACKSPACE))
                        {
                            if(entryStr.Length <= 1) { entryStr = ""; }
                            else
                            {
                                entryStr = entryStr.Substring(0, entryStr.Length - 1);
                            }
                        }
                        //had to put this here as it wouldn't work as a composite eval with the button bool above for some reason
                        if (Raylib.IsKeyPressed(KeyboardKey.KEY_ENTER))
                        {
                            if (entryStr != "")
                            {
                                int ID = database.AddPlayer(entryStr);
                                mode = lastMode;
                                lastMode = 0;
                                entryStr = "";
                                if (newWhiteFlag) { whitePlayerID = ID; newWhiteFlag = false; }
                                else if (newBlackFlag) { blackPlayerID = ID; newBlackFlag = false; }
                            }
                        }
                        break;
                    case (int)UIModes.NewLoadChoice:
                        if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
                        {
                            Vector2 mousePos = Raylib.GetMousePosition();

                            bool newGameSelected = Raylib.CheckCollisionPointRec(mousePos, newGame);
                            bool loadGameSelected = Raylib.CheckCollisionPointRec(mousePos, loadGame);
                            bool backButtonPressed = Raylib.CheckCollisionPointRec(mousePos, backButton);

                            if (newGameSelected) { mode = (int)UIModes.NewGameMenu; }
                            if (loadGameSelected) { mode = (int)UIModes.GamesList; }
                            if (backButtonPressed) { mode = (int)UIModes.MainMenu; }
                        }
                        break;
                    case (int)UIModes.NewGameMenu:
                        if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
                        {
                            Vector2 mousePos = Raylib.GetMousePosition();

                            bool whiteDropDownSelected = Raylib.CheckCollisionPointRec(mousePos, whitePlayerDropDown);
                            //check which name has been selected
                            if (mousePos.X >= 10 && mousePos.X <= 310 && mousePos.Y >= 200 && mousePos.Y <= 400 && whitePlayerListIndex != -1)
                            {
                                int row = ((int)mousePos.Y - 200) / 50;
                                row += whitePlayerListIndex;
                                if (row < playersList.Count() && row != -1)
                                {
                                    whitePlayerID = playersList[row].GetID();
                                }
                            }
                            //toggle drop down menu
                            if (whitePlayerListIndex >= 0) { whitePlayerListIndex = -1; }
                            else if (whiteDropDownSelected) { whitePlayerListIndex = 0; }

                            //repeat for black
                            bool blackDropDownSelected = Raylib.CheckCollisionPointRec(mousePos, blackPlayerDropDown);
                            if(mousePos.X >= 690 && mousePos.X <= 990 && mousePos.Y >= 200 && mousePos.Y <= 400 && blackPlayerListIndex != -1)
                            {
                                int row = ((int)mousePos.Y - 200) / 50;
                                row += blackPlayerListIndex;
                                if (row < playersList.Count() && row != -1)
                                {
                                    blackPlayerID = playersList[row].GetID();
                                }
                            }
                            //toggle drop down
                            if(blackPlayerListIndex >= 0) { blackPlayerListIndex = -1; }
                            else if (blackDropDownSelected) { blackPlayerListIndex = 0; }

                            //process other buttons
                            bool backButtonPressed = Raylib.CheckCollisionPointRec(mousePos, backButton);
                            bool addBlackPlayerPressed = Raylib.CheckCollisionPointRec(mousePos, addBlackPlayer);
                            bool addWhitePlayerPressed = Raylib.CheckCollisionPointRec(mousePos, addWhitePlayer);
                            bool undoMovesTickboxPressed = Raylib.CheckCollisionPointRec(mousePos, undoMovesTickbox);
                            bool startButtonPressed = Raylib.CheckCollisionPointRec(mousePos, startGameButton);
                            if (backButtonPressed)
                            {
                                whitePlayerID = 0;
                                blackPlayerID = 0;
                                whitePlayerListIndex = -1;
                                blackPlayerListIndex = -1;
                                mode = (int)UIModes.NewLoadChoice;
                            }
                            if (addBlackPlayerPressed)
                            {
                                //if adding a new player, raise flag to automatically assign id
                                lastMode = (int)UIModes.NewGameMenu;
                                newBlackFlag = true;
                                mode = (int)UIModes.CreatePlayer;
                            }
                            if (addWhitePlayerPressed)
                            {
                                lastMode = (int)UIModes.NewGameMenu;
                                newWhiteFlag = true;
                                mode = (int)UIModes.CreatePlayer;
                            }
                            if (undoMovesTickboxPressed) { undoMovesChoice = !undoMovesChoice; }
                            if (startButtonPressed)
                            {
                                if(gameCanStart)
                                {
                                    string gameName = entryStr;
                                    if (entryStr == "") { entryStr = database.GetPlayer(whitePlayerID).GetName() + database.GetPlayer(blackPlayerID).GetName(); }
                                    game = new Chess(whitePlayerID, blackPlayerID, gameName, undoMovesChoice);
                                    mode = (int)UIModes.GameUI2D;
                                }
                            }
                        }
                        //use arrow keys to move up or down selection - limit at ends of lists
                        if (Raylib.IsKeyPressed(KeyboardKey.KEY_DOWN))
                        {
                            if (whitePlayerListIndex < playersList.Count() - 1 && whitePlayerListIndex > -1) { whitePlayerListIndex++; }
                            else if(blackPlayerListIndex < playersList.Count() - 1 && blackPlayerListIndex > -1) { blackPlayerListIndex++; } 
                        }
                        if (Raylib.IsKeyPressed(KeyboardKey.KEY_UP))
                        {
                            if (whitePlayerListIndex > 0) { whitePlayerListIndex--; }
                            else if(blackPlayerListIndex > 0) { blackPlayerListIndex--; }
                        }

                        //raylib uses an input queue for keys(if they are occuring on the same frame), parse this here
                        keyPressed = Raylib.GetCharPressed();
                        while (keyPressed > 0)
                        {
                            //only accept character keys, limit names to 20 char (only 10 is displayed in table anyway)
                            if (keyPressed > 31 && keyPressed < 126 && entryStr.Length < 20)
                            {
                                entryStr += (char)keyPressed;
                            }
                            keyPressed = Raylib.GetCharPressed();
                        }
                        if (Raylib.IsKeyPressed(KeyboardKey.KEY_BACKSPACE))
                        {
                            if (entryStr.Length <= 1) { entryStr = ""; }
                            else
                            {
                                entryStr = entryStr.Substring(0, entryStr.Length - 1);
                            }
                        }
                        break;
                    case (int)UIModes.GamesList:
                        if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
                        {
                            Vector2 mousePos = Raylib.GetMousePosition();

                            //take clicks on the table
                            if (10 <= mousePos.X && mousePos.X <= 800 && 10 <= mousePos.Y && mousePos.Y <= (10 + (13 * 50)))
                            {
                                int row = ((int)mousePos.Y - 10) / 50;
                                int column = -1;
                                if (10 <= mousePos.X && mousePos.X < 200) { column = 0; }
                                else if (200 <= mousePos.X && mousePos.X < 300) { column = 1; }
                                else if (300 <= mousePos.X && mousePos.X < 450) { column = 2; }
                                else if (450 <= mousePos.X && mousePos.X < 620) { column = 3; }
                                else { column = 4; }

                                //affect table now

                                if (row == 0)
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
                                            sortMode = "gamestate";
                                            break;
                                        case 3:
                                            sortMode = "white";
                                            break;
                                        case 4:
                                            sortMode = "black";
                                            break;
                                    }
                                    if (prevMode == sortMode) { sortOrder = (sortOrder + 1) % 2; }
                                    else { sortOrder = 0; }
                                }
                                else
                                {
                                    row--;
                                    if (row < games.Count()) { selectedGame = games[row]; }
                                }
                            }

                            //using same locations as from 
                            bool loadGameButtonPressed = Raylib.CheckCollisionPointRec(mousePos, firstListButton);
                            bool deleteGamePressed = Raylib.CheckCollisionPointRec(mousePos, secondListButton);
                            bool backButtonPressed = Raylib.CheckCollisionPointRec(mousePos, backButton);
                            if (loadGameButtonPressed) { mode = (int)UIModes.ConfirmGame; }
                            else if (deleteGamePressed && selectedGame != null) { database.DeleteGame(selectedGame.GetGameID()); selectedGame = null; }
                            else if (backButtonPressed) { mode = (int)UIModes.NewLoadChoice; selectedGame = null; }
                        }
                        //use arrow keys to move up or down table
                        if (Raylib.IsKeyPressed(KeyboardKey.KEY_DOWN))
                        {
                            if (gameListIndex < games.Count() - 1) { gameListIndex++; }
                        }
                        if (Raylib.IsKeyPressed(KeyboardKey.KEY_UP))
                        {
                            if (gameListIndex > 0) { gameListIndex--; }
                        }
                        break;
                    case (int)UIModes.ConfirmGame:
                        if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
                        {
                            Vector2 mousePos = Raylib.GetMousePosition();

                            bool confirmGamePressed = Raylib.CheckCollisionPointRec(mousePos, startGameButton);
                            bool backButtonPressed = Raylib.CheckCollisionPointRec(mousePos, backButton);

                            if (confirmGamePressed)
                            {
                                game = new Chess(selectedGame);
                                selectedGame = null;
                                mode = (int)UIModes.GameUI2D;
                            }else if (backButtonPressed)
                            {
                                selectedGame = null;
                                mode = (int)UIModes.GamesList;
                            }
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
                                string lastMove = game.GetLastMove();
                                game.ViewportClick(x + (y * 8));
                                string newLastMove = game.GetLastMove();
                                //play sounds
                                if (lastMove != newLastMove && newLastMove.Contains('-'))
                                {
                                    Raylib.PlaySound(emptySquare);
                                }else if (lastMove != newLastMove && newLastMove.Contains('X'))
                                {
                                    Raylib.PlaySound(captureSquare);
                                }
                            }

                            //step through board using triangle buttons
                            bool upButtonPressed = Raylib.CheckCollisionPointTriangle(mousePos, new Vector2(360, 570), new Vector2(330, 600), new Vector2(390, 600));
                            bool downButtonPressed = Raylib.CheckCollisionPointTriangle(mousePos, new Vector2(610, 570), new Vector2(640, 600), new Vector2(670, 570));
                            //could list all buttons and give them numerical values then switch statement here?
                            if (upButtonPressed) { game.IncrementViewLayer(); }
                            else if (downButtonPressed) { game.DecrementViewLayer(); }

                            //check collision with viewDirection buttons
                            bool frontButtonPressed = Raylib.CheckCollisionPointRec(mousePos, frontButton);
                            bool topButtonPressed = Raylib.CheckCollisionPointRec(mousePos, topButton);
                            bool sideButtonPressed = Raylib.CheckCollisionPointRec(mousePos, sideButton);
                            bool pauseButtonPressed = Raylib.CheckCollisionPointRec(mousePos, backButton);
                            bool undoMovesPressed = Raylib.CheckCollisionPointRec(mousePos, undoMoveButton);
                            bool viewmodeTogglePressed = Raylib.CheckCollisionPointRec(mousePos, viewmodeToggleButton);
                            if (frontButtonPressed)
                            {
                                game.SetViewDirection((int)ViewDirections.Front);
                            }
                            else if (topButtonPressed)
                            {
                                game.SetViewDirection((int)ViewDirections.Top);
                            }
                            else if (sideButtonPressed)
                            {
                                game.SetViewDirection((int)ViewDirections.Side);
                            }else if (pauseButtonPressed)
                            {
                                mode = (int)UIModes.PauseMenu;
                            }else if (undoMovesPressed)
                            {
                                game.UndoMove();
                            }else if (viewmodeTogglePressed)
                            {
                                mode = (int)UIModes.GameUI3D;
                            }

                            if (game.GetGamestate() == (int)Gamestates.PendingPromo)
                            {
                                //check for pawn promotion selection ehre
                                bool queenSelected = Raylib.CheckCollisionPointRec(mousePos, queenPromoRec);
                                bool rookSelected = Raylib.CheckCollisionPointRec(mousePos, rookPromoRec);
                                bool bishopSelected = Raylib.CheckCollisionPointRec(mousePos, bishopPromoRec);
                                bool knightSelected = Raylib.CheckCollisionPointRec(mousePos, knightPromoRec);
                                if (queenSelected) { game.PromotePawn("Q"); }
                                else if (rookSelected) { game.PromotePawn("R"); }
                                else if (bishopSelected) { game.PromotePawn("B"); }
                                else if (knightSelected) { game.PromotePawn("N"); }
                            }
                        }

                        //step through board using keys
                        if (Raylib.IsKeyPressed(KeyboardKey.KEY_UP)) { game.IncrementViewLayer(); }
                        if (Raylib.IsKeyPressed(KeyboardKey.KEY_DOWN)) { game.DecrementViewLayer(); }
                        break;
                    case (int)UIModes.PauseMenu:
                        if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
                        {
                            Vector2 mousePos = Raylib.GetMousePosition();

                            bool resumeButtonPressed = Raylib.CheckCollisionPointRec(mousePos, resumeButton);
                            bool exitToMenuPressed = Raylib.CheckCollisionPointRec(mousePos, exitMenuButton);
                            bool exitToDesktopPressed = Raylib.CheckCollisionPointRec(mousePos, exitDesktopButton);
                            bool whiteForfeitPressed = Raylib.CheckCollisionPointRec(mousePos, whiteForfeitButton);
                            bool blackForfeitPressed = Raylib.CheckCollisionPointRec(mousePos, blackForfeitButton);
                            bool agreeToDrawPressed = Raylib.CheckCollisionPointRec(mousePos, mutualAgreementStalemateButton);
                            if (resumeButtonPressed) { mode = (int)UIModes.GameUI2D; }
                            else if (exitToDesktopPressed) { exitButtonClicked = true; }
                            else if (exitToMenuPressed)
                            {
                                //clean up variables then exit to menu
                                game = null;
                                selectedGame = null;
                                whitePlayerID = 0;
                                blackPlayerID = 0;
                                mode = (int)UIModes.MainMenu;
                            }
                            else if (whiteForfeitPressed && game.GetGamestate() == (int)Gamestates.Ongoing)
                            {
                                mode = (int)UIModes.ConfirmForfeitStalemate;
                                proposedOutcome = (int)Gamestates.BlackW;
                            }
                            else if (blackForfeitPressed && game.GetGamestate() == (int)Gamestates.Ongoing)
                            {
                                mode = (int)UIModes.ConfirmForfeitStalemate;
                                proposedOutcome = (int)Gamestates.WhiteW;
                            }else if (agreeToDrawPressed && game.GetGamestate() == (int)Gamestates.Ongoing)
                            {
                                mode = (int)UIModes.ConfirmForfeitStalemate;
                                proposedOutcome = (int)Gamestates.Stalemate;
                            }
                        }
                        break;
                    case (int)UIModes.ConfirmForfeitStalemate:
                        if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
                        {
                            Vector2 mousePos = Raylib.GetMousePosition();

                            bool confirmPressed = Raylib.CheckCollisionPointRec(mousePos, startGameButton);
                            bool backPressed = Raylib.CheckCollisionPointRec(mousePos, backButton);
                            if (confirmPressed)
                            {
                                game.ManualEndGame(proposedOutcome);
                                mode = (int)UIModes.GameUI2D;
                                proposedOutcome = -1;
                            }else if (backPressed) { mode = (int)UIModes.PauseMenu; proposedOutcome = -1; }
                        }
                        break;
                    case (int)UIModes.GameUI3D:
                        if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
                        {
                            Vector2 mousePos = Raylib.GetMousePosition();
                            bool viewmodeTogglePressed = Raylib.CheckCollisionPointRec(mousePos, viewmodeToggleButton);
                            bool wiremodeTogglepressed = Raylib.CheckCollisionPointRec(mousePos, wiremodeToggleButton);
                            if (viewmodeTogglePressed)
                            {
                                mode = (int)UIModes.GameUI2D;
                            }else if (wiremodeTogglepressed)
                            {
                                viewmodelWiresChoice = !viewmodelWiresChoice;
                            }
                        }
                        break;
                }

                // ------ draw here ------
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.WHITE);
                //switch to draw elements based on Ui mode
                switch (mode)
                {
                    case (int)UIModes.MainMenu:
                        UpdateMainMenu(titlePos, exitButton, playButton, playerButton);
                        break;
                    case (int)UIModes.PlayersList:
                        playersList = UpdatePlayersTable(playerListIndex, database, sortMode, sortOrder, selectedPlayerID);
                        UpdatePlayersListButtons(firstListButton, secondListButton, backButton, "Delete");
                        break;
                    case (int)UIModes.CreatePlayer:
                        UpdateCreatePlayer(entryStr, entryBox, finaliseNewPlayerButton, backButton);
                        break;
                    case (int)UIModes.NewLoadChoice:
                        UpdateNewLoadButtons(newGame, loadGame, backButton);
                        break;
                    case (int)UIModes.NewGameMenu:
                        //if both players have chosen non identical profiles the game start button becomes available
                        if (whitePlayerID != blackPlayerID && whitePlayerID != 0 && blackPlayerID != 0 && entryStr.Length > 0)
                        {
                            gameCanStart = true;
                        }
                        else { gameCanStart = false; }
                        UpdateNewGameButtons(undoMovesTickbox, gameNameEntryBox, startGameButton, backButton, entryStr, gameCanStart, undoMovesChoice);
                        playersList = UpdatePlayerNamesNewGame(whitePlayerDropDown, blackPlayerDropDown, whitePlayerID, blackPlayerID, addWhitePlayer, addBlackPlayer, whitePlayerListIndex, blackPlayerListIndex, database);
                        break;
                    case (int)UIModes.GamesList:
                        games = UpdateGamesTable(gameListIndex, sortMode, sortOrder, database, selectedGame);
                        //uses same positions as in player list so can use same rectangles
                        UpdateLoadGameButtons(firstListButton, secondListButton, backButton);
                        break;
                    case (int)UIModes.ConfirmGame:
                        UpdateConfirmGameMenu(startGameButton, backButton, selectedGame);
                        break;
                    case (int)UIModes.GameUI2D:
                        int state = game.GetGamestate();

                        UpdateBoard(game, textures);
                        UpdateViewportControls(game, frontButton, topButton, sideButton);
                        if (state == (int)Gamestates.PendingPromo) { UpdatePromoWindow(game, queenPromoRec, rookPromoRec, bishopPromoRec, knightPromoRec, textures); }
                        UpdateGameUI(backButton, moveListRec, game, undoMoveButton, game.GetIsUndoAllowed(), viewmodeToggleButton);
                        break;
                    case (int)UIModes.PauseMenu:
                        UpdatePauseMenu(resumeButton, exitMenuButton, exitDesktopButton, whiteForfeitButton, blackForfeitButton, mutualAgreementStalemateButton, game.GetGamestate());
                        break;
                    case (int)UIModes.ConfirmForfeitStalemate:
                        UpdateConfirmForfeitStalemate(startGameButton, backButton, proposedOutcome);
                        break;
                    case (int)UIModes.GameUI3D:
                        Raylib.UpdateCamera(ref camera, CameraMode.CAMERA_ORBITAL);
                        Raylib.BeginMode3D(camera);
                        Update3DRepresentation(CubePos, viewmodelWiresChoice, game, camera, textures);
                        Raylib.EndMode3D();
                        Update3DRepresentationControls(viewmodeToggleButton, wiremodeToggleButton, viewmodelWiresChoice);
                        break;
                }

                Raylib.EndDrawing();
            }

            //unload textures
            for(int i = 0; i < textures.Count(); i++)
            {
                Raylib.UnloadTexture(textures[i]);
            }
            //unload sfx
            Raylib.UnloadSound(emptySquare);
            Raylib.UnloadSound(captureSquare);


            Raylib.CloseWindow();
        }

        static void UpdateMainMenu(Vector2 textPos, Rectangle exit, Rectangle play,  Rectangle player)
        {
            Raylib.DrawText("Three Dimensional Chess", (int)textPos.X, (int)textPos.Y, 50, Color.BLACK);
            Raylib.DrawRectangleLinesEx(exit, 1, Color.BLACK);
            Raylib.DrawText("Exit", (int)exit.X + 45, (int)exit.Y + 30, 40, Color.BLACK);
            Raylib.DrawRectangleLinesEx(play, 1, Color.BLACK);
            Raylib.DrawText("Play", (int)play.X + 45, (int)play.Y + 30, 40, Color.BLACK);
            Raylib.DrawRectangleLinesEx(player, 1, Color.BLACK);
            Raylib.DrawText("Players", (int)player.X + 5, (int)player.Y + 30, 40, Color.BLACK);
        }

        static List<Player> UpdatePlayersTable(int startIndex, DatabaseHandler db, string sortMode, int sortOrder, int selectedPlayerID) 
        {
            //Get a list of all players
            List<Player> players = db.GetPlayers();
            Sorter sorter = new Sorter();
            switch (sortMode)
            {
                case "name":
                    players = sorter.MergeSortString(players);
                    break;
                case "date":
                    players = sorter.MergeSortDate(players);
                    break;
                case "winRatio":
                    players = sorter.MergeSortWins(players);
                    break;
                case "whiteWR":
                    players = sorter.MergeSortWhiteWR(players);
                    break;
                case "blackWR":
                    players = sorter.MergeSortBlackWR(players);
                    break;
            }
            if(sortOrder == 1)
            {
                players = sorter.Reverse(players);
            }
            //setup rectangles to be transformed
            Rectangle baseRec = new Rectangle(10, 10, 790, 50);

            for (int y = 0; y < (660 / 50); y++)
            {
                Raylib.DrawRectangleLinesEx(new Rectangle(baseRec.X, baseRec.Y + (y * 50), baseRec.Width, baseRec.Height), 1, Color.BLACK);
                //fill in with values from table
                if (y > 0 && startIndex + y - 1 < players.Count()) {
                    
                    Player tmp = players[startIndex + y - 1];
                    if(tmp.GetID() == selectedPlayerID)
                    {
                        Raylib.DrawRectangle((int)baseRec.X, (int)baseRec.Y + (y*50), (int)baseRec.Width, (int)baseRec.Height, Color.YELLOW);
                    }
                    //using position values from columns drawn above
                    string playerName = tmp.GetName();
                    //shorten name if it would write over column
                    if(playerName.Length > 10){ playerName = playerName.Substring(0, 10);}
                    Raylib.DrawText(playerName, 15, 23 + (y * 50), 30, Color.BLACK);
                    DateOnly joinDate = tmp.GetJoinDate();
                    Raylib.DrawText(joinDate.ToString().Substring(0, joinDate.ToString().Length - 5), 205, 23 + (y * 50), 30, Color.BLACK);
                    string winRatio = tmp.GetTotalWins() + "/" + tmp.GetTotalLosses() + "/" + tmp.GetDraws();
                    Raylib.DrawText(winRatio, 305, 23 + (y * 50), 30, Color.BLACK);
                    string whiteWR = String.Format("{0:0.00}",  tmp.GetWhiteWinrate()) + "%";
                    string blackWR = String.Format("{0:0.00}", tmp.GetBlackWinrate()) + "%";
                    Raylib.DrawText(whiteWR, 505, 23 + (y * 50), 30, Color.BLACK);
                    Raylib.DrawText(blackWR, 655, 23 + (y*50), 30, Color.BLACK);
                }
            }

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
            Raylib.DrawRectangleLines(10, 10, 790, 50 + (12 * 50), Color.BLACK);

            return players;
        }

        static void UpdatePlayersListButtons(Rectangle func, Rectangle create, Rectangle back, string funcText)
        {
            Raylib.DrawRectangleLinesEx(func, 1, Color.BLACK);
            Raylib.DrawText(funcText, (int)func.X + 25, (int)func.Y + 25, 30, Color.BLACK);
            Raylib.DrawRectangleLinesEx(create, 1, Color.BLACK);
            Raylib.DrawText("Add Player", (int)create.X + 10, (int)create.Y + 25, 30, Color.BLACK);
            Raylib.DrawRectangleLinesEx(back, 1, Color.BLACK);
            //draw a little back icon triangle
            Raylib.DrawTriangle(new Vector2(back.X + 70, back.Y + 5), new Vector2(back.X + 5, back.Y + (75 / 2)), new Vector2(back.X + 70, back.Y + 70), Color.BLACK);
        }
        
        static List<GameInfo> UpdateGamesTable(int startIndex, string sortMode, int sortOrder, DatabaseHandler db, GameInfo selectedGame)
        {
            List<GameInfo> games = db.GetGames();
            Sorter sorter = new Sorter();
            switch (sortMode)
            {
                case "name":
                    games = sorter.MergeSortName(games);
                    break;
                case "date":
                    games = sorter.MergeSortDate(games);
                    break;
                case "gamestate":
                    games = sorter.MergeSortState(games);
                    break;
                case "white":
                    games = sorter.MergeSortWhitePlayerName(games);
                    break;
                case "black":
                    games = sorter.MergeSortBlackPlayerName(games);
                    break;
            }
            if(sortOrder == 1) { games = sorter.Reverse(games); }
            //setup rectangles to be transformed
            Rectangle baseRec = new Rectangle(10, 10, 790, 50);
           

            for (int y = 0; y < (660 / 50); y++)
            {
                Raylib.DrawRectangleLinesEx(new Rectangle(baseRec.X, baseRec.Y + (y * 50), baseRec.Width, baseRec.Height), 1, Color.BLACK);
                //fill in with values from table
                if (y > 0 && startIndex + y - 1 < games.Count())
                {
                    GameInfo tmp = games[startIndex + y - 1];
                    //if game is equal to one selected tint it
                    if (selectedGame != null && tmp.GetGameID() == selectedGame.GetGameID())
                    {
                        Raylib.DrawRectangle((int)baseRec.X, (int)baseRec.Y + (y * 50), (int)baseRec.Width, (int)baseRec.Height, Color.YELLOW);
                    }
                    //using position values from columns drawn above
                    string gameName = tmp.GetName();
                    //shorten name if it would write over column
                    if (gameName.Length > 10) { gameName = gameName.Substring(0, 10); }
                    Raylib.DrawText(gameName, 15, 23 + (y * 50), 30, Color.BLACK);
                    DateOnly lastAccessed = DateOnly.FromDateTime(tmp.GetLastAccessed());
                    Raylib.DrawText(lastAccessed.ToString().Substring(0, lastAccessed.ToString().Length - 5), 205, 23 + (y * 50), 30, Color.BLACK);
                    string state = tmp.GetGamestate();
                    Raylib.DrawText(state, 305, 23 + (y * 50), 30, Color.BLACK);
                    string white = db.GetPlayer(tmp.GetWhitePlayerID()).GetName();
                    if (white.Length > 10) { white = white.Substring(0, 10); }
                    string black = db.GetPlayer(tmp.GetBlackPlayerID()).GetName();
                    if (black.Length > 10) { black = black.Substring(0, 10); }
                    Raylib.DrawText(white, 455, 23 + (y * 50), 30, Color.BLACK);
                    Raylib.DrawText(black, 625, 23 + (y * 50), 30, Color.BLACK);
                }
            }
            //put here so yellow rectangle is behind
            //draw columns in here
            Raylib.DrawLine(200, 10, 200, 660, Color.BLACK);
            Raylib.DrawText("Name", 15, 23, 30, Color.BLACK);
            Raylib.DrawLine(300, 10, 300, 660, Color.BLACK);
            Raylib.DrawText("Date", 205, 23, 30, Color.BLACK);
            Raylib.DrawLine(450, 10, 450, 660, Color.BLACK);
            Raylib.DrawText("State", 305, 23, 30, Color.BLACK);
            Raylib.DrawLine(620, 10, 620, 660, Color.BLACK);
            Raylib.DrawText("White", 455, 23, 30, Color.BLACK);
            Raylib.DrawText("Black", 625, 23, 30, Color.BLACK);
            // draw outline
            Raylib.DrawRectangleLines(10, 10, 790, 50 + (12 * 50), Color.BLACK);

            return games;
        }

        static void UpdateLoadGameButtons(Rectangle load, Rectangle delete, Rectangle back)
        {
            Raylib.DrawRectangleLinesEx(load, 1, Color.BLACK);
            Raylib.DrawText("Load Game", (int)load.X + 5, (int)load.Y + 25, 30, Color.BLACK);
            Raylib.DrawRectangleLinesEx(delete, 1, Color.BLACK);
            Raylib.DrawText("Delete Game", (int)delete.X + 5, (int)delete.Y + 25, 30, Color.BLACK);
            Raylib.DrawRectangleLinesEx(back, 1, Color.BLACK);
            //draw a little back icon triangle
            Raylib.DrawTriangle(new Vector2(back.X + 70, back.Y + 5), new Vector2(back.X + 5, back.Y + (75 / 2)), new Vector2(back.X + 70, back.Y + 70), Color.BLACK);
        }

        static void UpdateConfirmGameMenu(Rectangle confirm, Rectangle back, GameInfo info)
        {
            Raylib.DrawRectangleLinesEx(confirm, 1, Color.BLACK);
            Raylib.DrawText("Confirm", (int)confirm.X + 70, (int)confirm.Y + 27, 40, Color.BLACK);
            string lastMove = info.GetMoves()[info.GetMoves().Count() - 1];
            Raylib.DrawText("Last Move: " + lastMove, (int)confirm.X - 40, (int)confirm.Y - 100, 40, Color.BLACK);
            Raylib.DrawRectangleLinesEx(back, 1, Color.BLACK);
            //draw a little back icon triangle
            Raylib.DrawTriangle(new Vector2(back.X + 70, back.Y + 5), new Vector2(back.X + 5, back.Y + (75 / 2)), new Vector2(back.X + 70, back.Y + 70), Color.BLACK);
        }
        
        static void UpdateCreatePlayer(string inp, Rectangle entry, Rectangle doneButton, Rectangle back)
        {
            Raylib.DrawText("Enter player name:", (int)entry.X, (int)entry.Y - 31, 30, Color.BLACK);
            Raylib.DrawRectangleLinesEx(entry, 1, Color.BLACK);
            //show input text as user types it
            Raylib.DrawText(inp, (int)entry.X + 5, (int)entry.Y + 40, 40, Color.BLACK);
            Raylib.DrawRectangleLinesEx(doneButton, 1, Color.BLACK);
            Raylib.DrawText("Confirm", (int)doneButton.X + 70, (int)doneButton.Y + 27, 40, Color.BLACK);
            Raylib.DrawRectangleLinesEx(back, 1, Color.BLACK);
            Raylib.DrawTriangle(new Vector2(back.X + 70, back.Y + 5), new Vector2(back.X + 5, back.Y + (75 / 2)), new Vector2(back.X + 70, back.Y + 70), Color.BLACK);
        }

        static void UpdateNewLoadButtons(Rectangle create, Rectangle load, Rectangle back)
        {
            Raylib.DrawRectangleLinesEx(create, 1, Color.BLACK);
            Raylib.DrawText("New Game", (int)create.X + 25, (int)create.Y + 23, 30, Color.BLACK);
            Raylib.DrawRectangleLinesEx(load, 1, Color.BLACK);
            Raylib.DrawText("Load Game", (int)load.X + 25, (int)load.Y + 23, 30, Color.BLACK);
            Raylib.DrawRectangleLinesEx(back, 1, Color.BLACK);
            Raylib.DrawTriangle(new Vector2(back.X + 70, back.Y + 5), new Vector2(back.X + 5, back.Y + (75 / 2)), new Vector2(back.X + 70, back.Y + 70), Color.BLACK);
        }

        static void UpdateNewGameButtons(Rectangle undoMoves, Rectangle entry, Rectangle start, Rectangle back, string inp, bool canStart, bool undoMovesChoice)
        {
            Raylib.DrawRectangleLinesEx(undoMoves, 1, Color.BLACK);
            Raylib.DrawText("Undo Moves?", (int)undoMoves.X, (int)undoMoves.Y + (int)undoMoves.Height + 2, 30, Color.BLACK);
            if(undoMovesChoice)
            {
                //show that the box has been selected
                Raylib.DrawLine((int)undoMoves.X, (int)undoMoves.Y, (int)undoMoves.X + 75, (int)undoMoves.Y + 75, Color.BLACK);
                Raylib.DrawLine((int)undoMoves.X, (int)undoMoves.Y + 75, (int)undoMoves.X + 75, (int)undoMoves.Y, Color.BLACK);
            }
            Raylib.DrawRectangleLinesEx(entry, 1, Color.BLACK);
            Raylib.DrawText("Enter game name:", (int)entry.X+ 120, (int)entry.Y - 31, 30, Color.BLACK);
            //show input text as user types it
            Raylib.DrawText(inp, (int)entry.X + 5, (int)entry.Y + 40, 40, Color.BLACK);
            Color startGameCol = Color.BLACK;
            if (!canStart)
            {
                startGameCol = Color.GRAY;
            }
            Raylib.DrawRectangleLinesEx(start, 1, startGameCol);
            Raylib.DrawText("Start Game", (int)start.X + 40, (int)start.Y + 27, 40, startGameCol);
            Raylib.DrawRectangleLinesEx(back, 1, Color.BLACK);
            Raylib.DrawTriangle(new Vector2(back.X + 70, back.Y + 5), new Vector2(back.X + 5, back.Y + (75 / 2)), new Vector2(back.X + 70, back.Y + 70), Color.BLACK);
        }

        static List<Player> UpdatePlayerNamesNewGame(Rectangle whiteBase, Rectangle blackBase, int whitePlayer, int blackPlayer, Rectangle addWhite, Rectangle addBlack, int whiteListIndex, int blackListIndex, DatabaseHandler db) 
        {
            List<Player> players = db.GetPlayers();
            Raylib.DrawRectangleLinesEx(whiteBase, 1, Color.BLACK);
            Raylib.DrawText("Select white player:", (int)whiteBase.X + 2, (int)whiteBase.Y - 31, 30, Color.BLACK);
            //if a player has been selected write their name in
            if(whitePlayer > 0)
            {
                Player p = db.GetPlayer(whitePlayer);
                Raylib.DrawText(p.GetName(), 15, 10 + (int)whiteBase.Y, 30, Color.BLACK);
            }
            int limit = 4;
            if(players.Count() < limit) { limit = players.Count(); }
            //drop down menu here
            if(whiteListIndex != -1)
            {
                for(int y = 0; y < limit; y++)
                {
                    if(y + whiteListIndex < players.Count()) 
                    {
                        string playerName = players[y + whiteListIndex].GetName();
                        if(playerName.Length > 15) { playerName = playerName.Substring(0, 15); }
                        Raylib.DrawRectangleLinesEx(new Rectangle(whiteBase.X, whiteBase.Y + 50 + (y*50), whiteBase.Width, whiteBase.Height), 1, Color.BLACK);
                        Raylib.DrawText(playerName, 15, (int)whiteBase.Y + 55 + (y * 50), 30, Color.BLACK);
                    }
                }
            }

            //now repeat for black
            Raylib.DrawRectangleLinesEx(blackBase, 1, Color.BLACK);
            Raylib.DrawText("Select black player:", (int)blackBase.X - 6, (int)blackBase.Y - 31, 30, Color.BLACK);
            if(blackPlayer > 0)
            {
                Player b = db.GetPlayer(blackPlayer);
                Raylib.DrawText(b.GetName(), (int)blackBase.X + 5, (int)blackBase.Y + 10, 30, Color.BLACK);
            }
            if(players.Count() < limit) { limit = players.Count(); }
            if(blackListIndex != -1)
            {
                for(int y = 0; y < limit; y++)
                {
                    if(y + blackListIndex < players.Count())
                    {
                        string playerName = players[y + blackListIndex].GetName();
                        if(playerName.Length > 15) { playerName = playerName.Substring(0, 15); }
                        Raylib.DrawRectangleLinesEx(new Rectangle(blackBase.X, blackBase.Y + 50 + (y * 50), blackBase.Width, blackBase.Height), 1, Color.BLACK);
                        Raylib.DrawText(playerName, (int)blackBase.X + 5, (int)blackBase.Y + 55 + (y * 50), 30, Color.BLACK);
                    }
                }
            }

            //draw add player buttons
            Raylib.DrawRectangleLinesEx(addWhite, 1, Color.BLACK);
            Raylib.DrawLine((int)addWhite.X + ((int)addWhite.Width / 2), (int)addWhite.Y + 10, (int)addWhite.X + ((int)addWhite.Width / 2), (int)addWhite.Y + (int)addWhite.Height - 10, Color.BLACK);
            Raylib.DrawLine((int)addWhite.X + 10, (int)addWhite.Y + ((int)addWhite.Height / 2), (int)addWhite.X + (int)addWhite.Width - 10, (int)addWhite.Y + ((int)addWhite.Height / 2), Color.BLACK);
            Raylib.DrawRectangleLinesEx(addBlack, 1, Color.BLACK);
            Raylib.DrawLine((int)addBlack.X + ((int)addBlack.Width / 2), (int)addBlack.Y + 10, (int)addBlack.X + ((int)addBlack.Width / 2), (int)addBlack.Y + (int)addBlack.Height - 10, Color.BLACK);
            Raylib.DrawLine((int)addBlack.X + 10, (int)addBlack.Y + ((int)addBlack.Height / 2), (int)addBlack.X + (int)addBlack.Width - 10, (int)addBlack.Y + ((int)addBlack.Height / 2), Color.BLACK);
            return players;
        }

        static void UpdateBoard(Chess game, List<Texture2D> textures)
        {
            int offset = UIConstants.squareSide;
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    //grab the relevant cell from the viewport array in the game
                    Square cell = game.GetViewportCell((y * 8) + x);
                    //calculate the draw areas
                    int xPos = UIConstants.boardXOrigin + (x * offset);
                    int yPos = UIConstants.boardYOrigin - (y * offset);
                    //draw filled if black square, draw outline if white square
                    switch (cell.GetColour())
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
                    if (cell.GetPiecePointer() != -1)
                    {
                        Piece p = game.GetPieceDirect(cell.GetPiecePointer());

                        string type = p.GetPieceType();
                        int col = p.GetColour();
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

        static void UpdateViewportControls(Chess game, Rectangle frontButton, Rectangle topButton, Rectangle sideButton)
        {
            //draw control triangles
            //up triangle
            Color upCol = Color.BLACK;
            if(game.GetViewLayer() == 8) { upCol = Color.GRAY; }
            Raylib.DrawTriangle(new Vector2(360, 570), new Vector2(330, 600), new Vector2(390, 600), upCol);
            //down triangle
            Color downCol = Color.BLACK;
            if(game.GetViewLayer() == 1) { downCol = Color.GRAY; }
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
            switch (game.GetViewDirection())
            {
                case (int)ViewDirections.Front:
                    coordText = coordText.Substring(0, 15) + game.GetViewLayer();
                    break;
                case (int)ViewDirections.Side:
                    coordText = coordText.Substring(0, 3) + game.GetViewLayer() + coordText.Substring(4);
                    break;
                case (int)ViewDirections.Top:
                    coordText = coordText.Substring(0, 8) + game.GetViewLayer() + coordText.Substring(9);
                    break;
            }
            Raylib.DrawText(coordText, 395, 575, 30, Color.BLACK);
        }

        static void UpdatePromoWindow(Chess game, Rectangle queenRec, Rectangle rookRec, Rectangle bishopRec, Rectangle knightRec, List<Texture2D> textures)
        {
            //draw cells for promotion - borders make collisions clear            
            Rectangle sourceRec = new Rectangle(0, 0, 60, 60); // for loading texture from source

            Raylib.DrawRectangleLinesEx(queenRec, 1, Color.BLACK);
            Raylib.DrawRectangleLinesEx(rookRec, 1, Color.BLACK);
            Raylib.DrawRectangleLinesEx(bishopRec, 1, Color.BLACK);
            Raylib.DrawRectangleLinesEx(knightRec, 1, Color.BLACK);

            //draw relevant player's pieces
            if (game.GetCurrentPlayer().GetColour() == 0)
            {
                Raylib.DrawTexturePro(textures[(int)PieceTextureIndices.BlackQueen], sourceRec, queenRec, new Vector2(0, 0), 0, Color.WHITE);
                Raylib.DrawTexturePro(textures[(int)PieceTextureIndices.BlackRook], sourceRec, rookRec, new Vector2(0, 0), 0, Color.WHITE);
                Raylib.DrawTexturePro(textures[(int)PieceTextureIndices.BlackBishop], sourceRec, bishopRec, new Vector2(0, 0), 0, Color.WHITE);
                Raylib.DrawTexturePro(textures[(int)PieceTextureIndices.BlackKnight], sourceRec, knightRec, new Vector2(0, 0), 0, Color.WHITE);
            }// this else case should only arise if player is white but doesn't hurt to check
            else if(game.GetCurrentPlayer().GetColour() == 1)
            {
                Raylib.DrawTexturePro(textures[(int)PieceTextureIndices.WhiteQueen], sourceRec, queenRec, new Vector2(0, 0), 0, Color.WHITE);
                Raylib.DrawTexturePro(textures[(int)PieceTextureIndices.WhiteRook], sourceRec, rookRec, new Vector2(0, 0), 0, Color.WHITE);
                Raylib.DrawTexturePro(textures[(int)PieceTextureIndices.WhiteBishop], sourceRec, bishopRec, new Vector2(0, 0), 0, Color.WHITE);
                Raylib.DrawTexturePro(textures[(int)PieceTextureIndices.WhiteKnight], sourceRec, knightRec, new Vector2(0, 0), 0, Color.WHITE);
            }

            Raylib.DrawText("Promotion", 40, 467, 30, Color.BLACK);
        }

        static void UpdateGameUI(Rectangle pause, Rectangle moveList, Chess game, Rectangle undo, bool undoEnabled, Rectangle viewmode)
        {
            //draw pause button
            Raylib.DrawRectangleLinesEx(pause, 1, Color.BLACK);
            Raylib.DrawRectangle((int)pause.X + 18, (int)pause.Y + 10, 15, (int)pause.Height - 20, Color.BLACK);
            Raylib.DrawRectangle((int)pause.X + (int)pause.Width - 32, (int)pause.Y + 10, 15, (int)pause.Height - 20, Color.BLACK);
            //draw toggle button
            Raylib.DrawRectangleLinesEx(viewmode, 1, Color.BLACK);
            Raylib.DrawText("3D", (int)viewmode.X + 10, (int)viewmode.Y + 15, 50, Color.BLACK);

            //draw moveList
            Raylib.DrawRectangleLinesEx(moveList, 1, Color.BLACK);
            Stack<string> moves = game.GetMoveList();
            int numMoves = moves.Count();
            int ctr = 0;
            while (!moves.IsEmpty() && ctr < 17)
            {
                string moveStr = (numMoves - ctr) + ". " + moves.Pop();
                switch((numMoves - ctr)%2)
                {
                    case 1:
                        Raylib.DrawRectangleLines(960, (int)moveList.Y + (ctr * 30)+2, 20, 20, Color.BLACK);
                        break;
                    case 0:
                        Raylib.DrawRectangle(960, (int)moveList.Y + (ctr * 30)+3, 20, 20, Color.BLACK);
                        break;
                }
                Raylib.DrawText(moveStr, (int)moveList.X + 4, (int)moveList.Y + (ctr * 30)+3, 20, Color.BLACK);
                ctr++;
            }

            //draw undo moves button
            if (undoEnabled)
            {
                Raylib.DrawRectangleLinesEx(undo, 1, Color.BLACK);
                Raylib.DrawText("Undo Move", (int)undo.X + 20, (int)undo.Y + 12, 30, Color.BLACK);
            }

            //draw text when there's a winner
            switch (game.GetGamestate())
            {
                case (int)Gamestates.WhiteW:
                    string playerName = game.GetWhitePlayerName();
                    Raylib.DrawText(playerName + " Wins!", 350, 630, 30, Color.BLACK);
                    break;
                case (int)Gamestates.BlackW:
                    playerName = game.GetBlackPlayerName();
                    Raylib.DrawText(playerName + " Wins!", 350, 630, 30, Color.BLACK);
                    break;
                case (int)Gamestates.Stalemate:
                    Raylib.DrawText("Stalemate", 425, 630, 30, Color.BLACK);
                    break;
                default:
                    playerName = game.GetCurrentPlayer().GetName();
                    Raylib.DrawText(playerName + "'s Turn", 350, 615, 30, Color.BLACK);
                    if (game.GetInCheck())
                    {
                        Raylib.DrawText(playerName+" in check", 350, 645, 30, Color.BLACK);
                    }
                    break;
            }
        }

        static void UpdatePauseMenu(Rectangle resume, Rectangle exitMenu, Rectangle exitDesktop, Rectangle whiteForf, Rectangle blackForf, Rectangle stalemate, int state)
        {
            Raylib.DrawRectangleLinesEx(resume, 1, Color.BLACK);
            Raylib.DrawText("Resume", (int)resume.X + 95, (int)resume.Y + 25, 30, Color.BLACK);
            Raylib.DrawRectangleLinesEx(exitMenu, 1, Color.BLACK);
            Raylib.DrawText("Exit To Menu",(int)exitMenu.X + 50, (int)exitMenu.Y + 25, 30, Color.BLACK);
            Raylib.DrawRectangleLinesEx(exitDesktop, 1, Color.BLACK);
            Raylib.DrawText("Exit To Desktop", (int)exitDesktop.X + 30, (int)exitDesktop.Y + 25, 30, Color.BLACK);
            if(state == (int)Gamestates.Ongoing)
            {
                Raylib.DrawRectangleLinesEx(whiteForf, 1, Color.BLACK);
                Raylib.DrawText("White Forfeit", (int)whiteForf.X + 50, (int)whiteForf.Y + 25, 30, Color.BLACK);
                Raylib.DrawRectangleLinesEx(blackForf, 1, Color.BLACK);
                Raylib.DrawText("Black Forfeit", (int)blackForf.X + 50, (int)blackForf.Y + 25, 30, Color.BLACK);
                Raylib.DrawRectangleLinesEx(stalemate, 1, Color.BLACK);
                Raylib.DrawText("Agree To Draw", (int)stalemate.X + 30, (int)stalemate.Y + 25, 30, Color.BLACK);
            }
        }

        static void UpdateConfirmForfeitStalemate(Rectangle confirm, Rectangle back, int proposedOutcome)
        {
            Raylib.DrawRectangleLinesEx(confirm, 1, Color.BLACK);
            Raylib.DrawText("Confirm", (int)confirm.X + 70, (int)confirm.Y + 27, 40, Color.BLACK);
            Raylib.DrawRectangleLinesEx(back, 1, Color.BLACK);
            Raylib.DrawTriangle(new Vector2(back.X + 70, back.Y + 5), new Vector2(back.X + 5, back.Y + (75 / 2)), new Vector2(back.X + 70, back.Y + 70), Color.BLACK);
            string context = "";
            switch (proposedOutcome)
            {
                case (int)Gamestates.Stalemate:
                    context = "Agree To Draw?";
                    break;
                case (int)Gamestates.WhiteW:
                    context = "Black Forfeit?";
                    break;
                case (int)Gamestates.BlackW:
                    context = "White Forfeit?";
                    break;
            }
            Raylib.DrawText(context, (int)confirm.X + 5, (int)confirm.Y - 32, 30, Color.BLACK);
        }

        static void Update3DRepresentation(Vector3 cubePos, bool wiresSelected, Chess game, Camera3D camera, List<Texture2D> textures)
        {
            Vector3 baseCubePos = new Vector3(-3.5f, -3.5f, -3.5f);
            Rectangle sourceRec = new Rectangle(0, 0, 60, 60); // for loading texture from source
            Raylib.DrawCubeWires(cubePos, 8, 8, 8, Color.BLACK);
            for(int z = 0; z < 8; z++)
            {
                for (int x = 0; x < 8; x++)
                {
                    for (int y = 0; y < 8; y++)
                    {
                        Vector3 tmpVect = new Vector3(baseCubePos.X + x, baseCubePos.Y + y, baseCubePos.Z + z);
                        Square cell = game.GetCell(x + (y * 8) + (z * 64));
                        if (wiresSelected) { Raylib.DrawCubeWires(tmpVect, 1, 1, 1, Color.BLACK); }
                        if(cell.GetPiecePointer() != -1)
                        {
                            Piece p = game.GetPieceDirect(cell.GetPiecePointer());

                            string type = p.GetPieceType();
                            int col = p.GetColour();
                            Texture2D pieceText = new Texture2D();
                            if (col == (int)Colours.White)
                            {
                                //select each piece
                                switch (type)
                                {
                                    case "P":
                                        pieceText = textures[0];
                                        break;
                                    case "R":
                                        pieceText = textures[1];
                                        break;
                                    case "N":
                                        pieceText = textures[2];
                                        break;
                                    case "B":
                                        pieceText = textures[3];
                                        break;
                                    case "Q":
                                        pieceText = textures[4];
                                        break;
                                    case "K":
                                        pieceText = textures[5];
                                        break;
                                }
                            }
                            else
                            {
                                switch (type)
                                {
                                    case "P":
                                        pieceText = textures[6];
                                        break;
                                    case "R":
                                        pieceText = textures[7];
                                        break;
                                    case "N":
                                        pieceText = textures[8];
                                        break;
                                    case "B":
                                        pieceText = textures[9];
                                        break;
                                    case "Q":
                                        pieceText = textures[10];
                                        break;
                                    case "K":
                                        pieceText = textures[11];
                                        break;
                                }
                            }
                            //draw piece onto 3d board here
                            Raylib.DrawBillboardRec(camera, pieceText, sourceRec, tmpVect, new Vector2(1f, 1f), Color.WHITE);
                        }
                        //now draw colour of cell
                        Color cellCol = Color.BLANK;
                        switch (cell.GetColour())
                        {
                            case (int)Colours.WhiteBlue: cellCol = Color.BLUE; break;
                            case (int)Colours.BlackBlue: cellCol = Color.DARKBLUE; break;
                            case (int)Colours.WhiteRed: cellCol = Color.RED; break;
                            case (int)Colours.BlackRed: cellCol = Color.MAROON; break;
                            case (int)Colours.WhiteYellow: cellCol = Color.YELLOW; break; // NOTE: need to fade this
                            case (int)Colours.BlackYellow: cellCol = Color.YELLOW; break;
                        }
                        if (!cellCol.Equals(Color.BLANK)) 
                        { 
                            Raylib.DrawCube(tmpVect, 1f, 1f, 1f, Raylib.Fade(cellCol, 0.9f));
                            if (!wiresSelected) { Raylib.DrawCubeWires(tmpVect, 1, 1, 1, Color.BLACK); }
                        }
                    }
                }
            }
        }
        static void Update3DRepresentationControls(Rectangle viewmodeToggle, Rectangle wireToggle, bool wireChoice)
        {
            //draw reverse toggle button
            Raylib.DrawRectangleLinesEx(viewmodeToggle, 1, Color.BLACK);
            Raylib.DrawText("2D", (int)viewmodeToggle.X + 10, (int)viewmodeToggle.Y + 15, 50, Color.BLACK);
            //draw wire toggle button
            Raylib.DrawRectangleLinesEx(wireToggle, 1, Color.BLACK);
            Raylib.DrawText("Enable Wires", (int)wireToggle.X, (int)wireToggle.Y - 31, 30, Color.BLACK);
            if (wireChoice)
            {
                Raylib.DrawLine((int)wireToggle.X, (int)wireToggle.Y, (int)wireToggle.X + (int)wireToggle.Width, (int)wireToggle.Y + (int)wireToggle.Height, Color.BLACK);
                Raylib.DrawLine((int)wireToggle.X, (int)wireToggle.Y + (int)wireToggle.Height, (int)wireToggle.X + (int)wireToggle.Width, (int)wireToggle.Y, Color.BLACK);
            }
        }
    }
}