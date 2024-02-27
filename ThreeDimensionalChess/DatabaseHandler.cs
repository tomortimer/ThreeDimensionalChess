using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace ThreeDimensionalChess
{
    

    class DatabaseHandler
    {
        //constructor
        public DatabaseHandler()
        {
            //create tables, first init only
            //CreateTables();
        }

        private void CreateTables()
        {
            //create db connection
            SQLiteConnection dbConnection = new SQLiteConnection("Data Source=database.db");
            dbConnection.Open();
            SQLiteCommand comm = dbConnection.CreateCommand();
            //enter command string
            comm.CommandText =
                @"
CREATE TABLE player (
    playerID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL,
    whiteLosses INTEGER NOT NULL,
    blackLosses INTEGER NOT NULL,
    whiteDraws INTEGER NOT NULL,
    blackDraws INTEGER NOT NULL,
    whiteWins INTEGER NOT NULL,
    blackWins INTEGER NOT NULL,
    date DATE NOT NULL
);

CREATE TABLE game (
    gameID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL,
    moveList TEXT NOT NULL,
    gamestate INTEGER NOT NULL,
    lastAccessed DATE NOT NULL,
    undoMoves BOOLEAN NOT NULL
);

CREATE TABLE gamesPlayers (
    gameID INTEGER NOT NULL,
    whitePlayerID INTEGER NOT NULL,
    blackPlayerID INTEGER NOT NULL
);

INSERT INTO player (name, whiteLosses, blackLosses, whiteDraws, blackDraws, whiteWins, blackWins, date) 
VALUES ('testPlayer1', 0, 0, 0, 0, 0, 0, $date);

INSERT INTO player (name, whiteLosses, blackLosses, whiteDraws, blackDraws, whiteWins, blackWins, date) 
VALUES ('testPlayer2', 0, 0, 0, 0, 0, 0, $date);";
            comm.Parameters.AddWithValue("$date", DateTime.Today);
            comm.ExecuteNonQuery();
            dbConnection.Close();
        }

        public int AddPlayer(string name)
        {
            SQLiteConnection dbConnection = new SQLiteConnection("Data Source=database.db");
            dbConnection.Open();
            SQLiteCommand comm = dbConnection.CreateCommand();

            //enter command text
            comm.CommandText = 
                @"
INSERT INTO player (name, whiteLosses, blackLosses, whiteDraws, blackDraws, whiteWins, blackWins, date) 
VALUES ($name, 0, 0, 0, 0, 0, 0, $date);";
            comm.Parameters.AddWithValue("$name", name);
            comm.Parameters.AddWithValue("$date", DateTime.Today);

            comm.ExecuteNonQuery();
            
            //once player is created need to return its ID to higher level
            comm.CommandText = "SELECT * FROM player ORDER BY playerID DESC LIMIT 1;";
            SQLiteDataReader reader = comm.ExecuteReader();
            reader.Read();
            int ret = reader.GetInt32(0);
            reader.Close();
            dbConnection.Close();

            return ret;
        }

        public List<Player> GetPlayers()
        {
            //set up query
            List<Player> ret = new List<Player>();
            SQLiteConnection dbConnection = new SQLiteConnection("Data Source=database.db");
            dbConnection.Open();
            SQLiteCommand comm = dbConnection.CreateCommand();

            comm.CommandText = "SELECT * FROM player";
            SQLiteDataReader reader = comm.ExecuteReader();

            //read the db into the list
            while (reader.Read())
            {
                int ID = reader.GetInt32(0);
                string name = reader.GetString(1);
                int whiteLosses = reader.GetInt32(2);
                int blackLosses = reader.GetInt32(3);
                int whiteDraws = reader.GetInt32(4);
                int blackDraws = reader.GetInt32(5);
                int whiteWins = reader.GetInt32(6);
                int blackWins = reader.GetInt32(7);
                DateTime joinDate = reader.GetDateTime(8);
                Player tmp = new Player(ID, name, whiteLosses, blackLosses, whiteDraws, blackDraws, whiteWins, blackWins, joinDate);
                ret.Add(tmp);
            }
            reader.Close();
            dbConnection.Close();

            return ret;
        }

        public Player GetPlayer(int inp)
        {
            SQLiteConnection dbConnection = new SQLiteConnection("Data Source=database.db");
            dbConnection.Open();
            SQLiteCommand comm = dbConnection.CreateCommand();

            comm.CommandText = "SELECT * FROM player WHERE playerID=$input;";
            comm.Parameters.AddWithValue("$input", inp);
            SQLiteDataReader reader = comm.ExecuteReader();

            reader.Read();
            int ID = reader.GetInt32(0);
            string name = reader.GetString(1);
            int whiteLosses = reader.GetInt32(2);
            int blackLosses = reader.GetInt32(3);
            int whiteDraws = reader.GetInt32(4);
            int blackDraws = reader.GetInt32(5);
            int whiteWins = reader.GetInt32(6);
            int blackWins = reader.GetInt32(7);
            DateTime joinDate = reader.GetDateTime(8);
            Player ret = new Player(ID, name, whiteLosses, blackLosses, whiteDraws, blackDraws, whiteWins, blackWins, joinDate);
            reader.Close();
            dbConnection.Close();

            return ret;
        }

        public void UpdatePlayer(Player p)
        {
            SQLiteConnection dbConnection = new SQLiteConnection("Data Source=database.db");
            dbConnection.Open();
            SQLiteCommand comm = dbConnection.CreateCommand();

            comm.CommandText = @"
UPDATE player
SET whiteLosses=$WHL, blackLosses=$BLL, whiteDraws=$WHD, blackDraws=$BLD, whiteWins=$WHW, blackWins=$BLW
WHERE playerID=$ID;";

            //load parameters from player obj
            comm.Parameters.AddWithValue("$WHL", p.GetWhiteLosses());
            comm.Parameters.AddWithValue("$BLL", p.GetBlackLosses());
            comm.Parameters.AddWithValue("$WHD", p.GetWhiteDraws());
            comm.Parameters.AddWithValue("$BLD", p.GetBlackDraws());
            comm.Parameters.AddWithValue("$WHW", p.GetWhiteWins());
            comm.Parameters.AddWithValue("$BLW", p.GetBlackWins());
            comm.Parameters.AddWithValue("$ID", p.GetID());

            comm.ExecuteNonQuery();
            dbConnection.Close();
        }

        //returns false if unsuccessful, true if successful
        public bool DeletePlayer(int inp)
        {
            SQLiteConnection dbConnection = new SQLiteConnection("Data Source=database.db");
            dbConnection.Open();
            SQLiteCommand comm = dbConnection.CreateCommand();

            //delete player from db, make sure to delete any games referencing them too
            comm.CommandText = @"
DELETE FROM player WHERE playerID=$input;

SELECT gameID FROM gamesPlayers WHERE whitePlayerID=$input OR blackPlayerID=$input;
";
            comm.Parameters.AddWithValue("$input", inp);

            bool ret = true; 
            try
            {
                SQLiteDataReader reader = comm.ExecuteReader();
                int gameID = reader.GetInt32(0);
                reader.Close();

                comm.CommandText = @"
DELETE FROM game WHERE gameID=$input;
DELETE FROM gamesPlayers WHERE gameID=$input;
";
                comm.Parameters.AddWithValue("$input", gameID);
                comm.ExecuteNonQuery();
            }
            catch(Exception e)
            {
                //probably expecting an SQLLogic or SQLArgument exception here
                Console.WriteLine("Experienced Error: "+e.Message);
                ret = false;
            }
            dbConnection.Close();

            return ret;
        }

        public int CreateGame(string name, bool undo, int whiteID, int blackID)
        {
            SQLiteConnection dbConnection = new SQLiteConnection("Data Source=database.db");
            dbConnection.Open();
            SQLiteCommand comm = dbConnection.CreateCommand();

            comm.CommandText = @"
INSERT INTO game (name, moveList, gamestate, lastAccessed, undoMoves) 
VALUES ($name, $empty, $state, $date, $undo);";

            //insert params
            comm.Parameters.AddWithValue("$name", name);
            comm.Parameters.AddWithValue("$empty", "");
            comm.Parameters.AddWithValue("$state", (int)Gamestates.Ongoing);
            comm.Parameters.AddWithValue("$date", DateTime.Today);
            comm.Parameters.AddWithValue("$undo", undo);

            comm.ExecuteNonQuery();

            //once game is created need to return its ID to higher level
            comm.CommandText = "SELECT * FROM game ORDER BY gameID DESC LIMIT 1;";
            SQLiteDataReader reader = comm.ExecuteReader();
            reader.Read();
            int ret = reader.GetInt32(0);
            reader.Close();

            //update join table with game info
            comm.CommandText = @"
INSERT INTO gamesPlayers (gameID, whitePlayerID, blackPlayerID)
VALUES ($game, $white, $black);";
            comm.Parameters.AddWithValue("$game", ret);
            comm.Parameters.AddWithValue("$white", whiteID);
            comm.Parameters.AddWithValue("$black", blackID);
            comm.ExecuteNonQuery();
            
            dbConnection.Close();
            return ret;
        }

        public void UpdateGame(string moves, int state, int ID)
        {
            SQLiteConnection dbConnection = new SQLiteConnection("Data Source=database.db");
            dbConnection.Open();
            SQLiteCommand comm = dbConnection.CreateCommand();

            comm.CommandText = @"
UPDATE game
SET moveList=$moves, gamestate=$state, lastAccessed=$date
WHERE gameID=$ID";

            //load params into query
            comm.Parameters.AddWithValue("$moves", moves);
            comm.Parameters.AddWithValue("$state", state);
            comm.Parameters.AddWithValue("$date", DateTime.Today);
            comm.Parameters.AddWithValue("$ID", ID);

            comm.ExecuteNonQuery();
            dbConnection.Close();
        }

        public List<GameInfo> GetGames()
        {
            List<GameInfo> ret = new List<GameInfo>();
            SQLiteConnection dbConnection = new SQLiteConnection("Data Source=database.db");
            dbConnection.Open();
            SQLiteCommand comm = dbConnection.CreateCommand();

            comm.CommandText = "SELECT game.gameID, game.name, game.moveList, game.gamestate, game.lastAccessed, game.undoMoves, gamesPlayers.whitePlayerID, gamesPlayers.blackPlayerID FROM game, gamesPlayers WHERE game.gameID=gamesPlayers.gameID;";
            SQLiteDataReader reader = comm.ExecuteReader();

            //iterate through all records
            while (reader.Read())
            {
                int ID = reader.GetInt32(0);
                string name = reader.GetString(1);
                string moveListRepr = reader.GetString(2);
                int gamestate = reader.GetInt32(3);
                DateTime lastAccessed = reader.GetDateTime(4);
                bool undoMoves = reader.GetBoolean(5);
                int whiteID = reader.GetInt32(6);
                int blackID = reader.GetInt32(7);
                
                GameInfo tmp = new GameInfo(ID, name, moveListRepr, gamestate, lastAccessed, whiteID, blackID, undoMoves);
                ret.Add(tmp);
            }
            reader.Close();
            dbConnection.Close();
            
            return ret;
        }

        public GameInfo GetGame(int inp)
        {
            SQLiteConnection dbConnection = new SQLiteConnection("Data Source=database.db");
            dbConnection.Open();
            SQLiteCommand comm = dbConnection.CreateCommand();

            comm.CommandText = "SELECT game.gameID, game.name, game.moveList, game.gamestate, game.lastAccessed, game.undoMoves, gamesPlayers.whitePlayerID, gamesPlayers.blackPlayerID FROM game, gamesPlayers WHERE game.gameID=$input AND game.gameID=gamesPlayers.gameID;";
            comm.Parameters.AddWithValue("$input", inp);
            SQLiteDataReader reader = comm.ExecuteReader();

            //pull values
            reader.Read();
            int ID = reader.GetInt32(0);
            string name = reader.GetString(1);
            string moveListRepr = reader.GetString(2);
            int gamestate = reader.GetInt32(3);
            DateTime lastAccessed = reader.GetDateTime(4);
            bool undoMoves = reader.GetBoolean(5);
            int whiteID = reader.GetInt32(6);
            int blackID = reader.GetInt32(7);
            
            GameInfo tmp = new GameInfo(ID, name, moveListRepr, gamestate, lastAccessed, whiteID, blackID, undoMoves);
            reader.Close();
            dbConnection.Close();

            return tmp;
        }

        public bool DeleteGame(int inp)
        {
            SQLiteConnection dbConnection = new SQLiteConnection("Data Source=database.db");
            dbConnection.Open();
            SQLiteCommand comm = dbConnection.CreateCommand();

            comm.CommandText = @"
DELETE FROM game WHERE gameID=$input;
DELETE FROM gamesPlayers WHERE gameID=$input;
";
            comm.Parameters.AddWithValue("$input", inp);

            bool ret = true;
            try
            {
                comm.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                //probably expecting an SQLLogic or SQLArgument exception here
                Console.WriteLine("Experienced Error: " + e.Message);
                ret = false;
            }
            dbConnection.Close();

            return ret;
        }
    }
}
