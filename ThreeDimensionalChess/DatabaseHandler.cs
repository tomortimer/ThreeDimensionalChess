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
        public DatabaseHandler()
        {
            //create tables, first init only
            //createTables();
        }

        private void createTables()
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
    whitePlayerID INTEGER NOT NULL,
    blackPlayerID INTEGER NOT NULL,
    undoMoves BOOLEAN NOT NULL
);

INSERT INTO player (name, whiteLosses, blackLosses, whiteDraws, blackDraws, whiteWins, blackWins, date) 
VALUES ('testPlayer1', 0, 0, 0, 0, 0, 0, $date);

INSERT INTO player (name, whiteLosses, blackLosses, whiteDraws, blackDraws, whiteWins, blackWins, date) 
VALUES ('testPlayer2', 0, 0, 0, 0, 0, 0, $date);";
            comm.Parameters.AddWithValue("$date", DateTime.Today);
            comm.ExecuteNonQuery();
            dbConnection.Close();
        }

        public int addPlayer(string name)
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

        public List<Player> getPlayers()
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

        public Player getPlayer(int inp)
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

        public void updatePlayer(Player p)
        {
            SQLiteConnection dbConnection = new SQLiteConnection("Data Source=database.db");
            dbConnection.Open();
            SQLiteCommand comm = dbConnection.CreateCommand();

            comm.CommandText = @"
UPDATE player
SET whiteLosses=$WHL, blackLosses=$BLL, whiteDraws=$WHD, blackDraws=$BLD, whiteWins=$WHW, blackWins=$BLW
WHERE playerID=$ID;";

            //load parameters from player obj
            comm.Parameters.AddWithValue("$WHL", p.getWhiteLosses());
            comm.Parameters.AddWithValue("$BLL", p.getBlackLosses());
            comm.Parameters.AddWithValue("$WHD", p.getWhiteDraws());
            comm.Parameters.AddWithValue("$BLD", p.getBlackDraws());
            comm.Parameters.AddWithValue("$WHW", p.getWhiteWins());
            comm.Parameters.AddWithValue("$BLW", p.getBlackWins());
            comm.Parameters.AddWithValue("$ID", p.getID());

            comm.ExecuteNonQuery();
            dbConnection.Close();
        }

        //returns false if unsuccessful, true if successful
        public bool deletePlayer(int inp)
        {
            SQLiteConnection dbConnection = new SQLiteConnection("Data Source=database.db");
            dbConnection.Open();
            SQLiteCommand comm = dbConnection.CreateCommand();

            //delete player from db, make sure to delete any games referencing them too
            comm.CommandText = @"
DELETE FROM player WHERE playerID=$input;

DELETE FROM game WHERE whitePlayerID=$input OR blackPlayerID=$input;
";
            comm.Parameters.AddWithValue("$input", inp);

            bool ret = true; 
            try
            {
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

        public int createGame(string name, bool undo, int whiteID, int blackID)
        {
            SQLiteConnection dbConnection = new SQLiteConnection("Data Source=database.db");
            dbConnection.Open();
            SQLiteCommand comm = dbConnection.CreateCommand();

            comm.CommandText = @"
INSERT INTO GAME (name, moveList, gamestate, lastAccessed, whitePlayerID, blackPlayerID, undoMoves) 
VALUES ($name, $empty, $state, $date, $white, $black, $undo);";

            //insert params
            comm.Parameters.AddWithValue("$name", name);
            comm.Parameters.AddWithValue("$empty", "");
            comm.Parameters.AddWithValue("$state", (int)Gamestates.Ongoing);
            comm.Parameters.AddWithValue("$date", DateTime.Today);
            comm.Parameters.AddWithValue("$white", whiteID);
            comm.Parameters.AddWithValue("$black", blackID);
            comm.Parameters.AddWithValue("$undo", undo);

            comm.ExecuteNonQuery();

            //once game is created need to return its ID to higher level
            comm.CommandText = "SELECT * FROM game ORDER BY gameID DESC LIMIT 1;";
            SQLiteDataReader reader = comm.ExecuteReader();
            reader.Read();
            int ret = reader.GetInt32(0);
            reader.Close();
            dbConnection.Close();
            return ret;
        }

        public void writeGame(List<string> moves, int state, int ID)
        {
            SQLiteConnection dbConnection = new SQLiteConnection("Data Source=database.db");
            dbConnection.Open();
            SQLiteCommand comm = dbConnection.CreateCommand();

            comm.CommandText = @"
UPDATE game
SET moveList=$moves, gamestate=$state, lastAccessed=$date
WHERE gameID=$ID";

            //load params into query
            comm.Parameters.AddWithValue("$moves", moves.ConvertToString());
            comm.Parameters.AddWithValue("$state", state);
            comm.Parameters.AddWithValue("$date", DateTime.Today);
            comm.Parameters.AddWithValue("$ID", ID);

            comm.ExecuteNonQuery();
            dbConnection.Close();
        }

        public List<GameInfo> getGames()
        {
            List<GameInfo> ret = new List<GameInfo>();
            SQLiteConnection dbConnection = new SQLiteConnection("Data Source=database.db");
            dbConnection.Open();
            SQLiteCommand comm = dbConnection.CreateCommand();

            comm.CommandText = "SELECT * FROM game";
            SQLiteDataReader reader = comm.ExecuteReader();

            //iterate through all records
            while (reader.Read())
            {
                int ID = reader.GetInt32(0);
                string name = reader.GetString(1);
                string moveListRepr = reader.GetString(2);
                int gamestate = reader.GetInt32(3);
                DateTime lastAccessed = reader.GetDateTime(4);
                int whiteID = reader.GetInt32(5);
                int blackID = reader.GetInt32(6);
                bool undoMoves = reader.GetBoolean(7);
                GameInfo tmp = new GameInfo(ID, name, moveListRepr, gamestate, lastAccessed, whiteID, blackID, undoMoves);
                ret.Add(tmp);
            }
            reader.Close();
            dbConnection.Close();
            
            return ret;
        }

        public GameInfo getGame(int inp)
        {
            SQLiteConnection dbConnection = new SQLiteConnection("Data Source=database.db");
            dbConnection.Open();
            SQLiteCommand comm = dbConnection.CreateCommand();

            comm.CommandText = "SELECT * FROM game WHERE gameID=$input;";
            comm.Parameters.AddWithValue("$input", inp);
            SQLiteDataReader reader = comm.ExecuteReader();

            //pull values
            reader.Read();
            int ID = reader.GetInt32(0);
            string name = reader.GetString(1);
            string moveListRepr = reader.GetString(2);
            int gamestate = reader.GetInt32(3);
            DateTime lastAccessed = reader.GetDateTime(4);
            int whiteID = reader.GetInt32(5);
            int blackID = reader.GetInt32(6);
            bool undoMoves = reader.GetBoolean(7);
            GameInfo tmp = new GameInfo(ID, name, moveListRepr, gamestate, lastAccessed, whiteID, blackID, undoMoves);
            reader.Close();
            dbConnection.Close();

            return tmp;
        }

        public bool deleteGame(int inp)
        {
            SQLiteConnection dbConnection = new SQLiteConnection("Data Source=database.db");
            dbConnection.Open();
            SQLiteCommand comm = dbConnection.CreateCommand();

            comm.CommandText = "DELETE FROM game WHERE gameID=$input;";
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
