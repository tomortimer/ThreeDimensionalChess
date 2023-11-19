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
    draws INTEGER NOT NULL,
    whiteWins INTEGER NOT NULL,
    blackWins INTEGER NOT NULL,
    date DATE NOT NULL
);

CREATE TABLE game (
    gameID INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL,
    moveList TEXT NOT NULL,
    gamestate INTEGER NOT NULL,
    lastAccessed DATE NOT NULL
);

INSERT INTO player (name, whiteLosses, blackLosses, draws, whiteWins, blackWins, date) 
VALUES ('testPlayer1', 0, 0, 0, 0, 0, $date);

INSERT INTO player (name, whiteLosses, blackLosses, draws, whiteWins, blackWins, date) 
VALUES ('testPlayer2', 0, 0, 0, 0, 0, $date);";
            comm.Parameters.AddWithValue("$date", DateTime.Today);
            comm.ExecuteNonQuery();
            dbConnection.Close();
        }

        public void addPlayer(string name)
        {
            SQLiteConnection dbConnection = new SQLiteConnection("Data Source=database.db");
            dbConnection.Open();
            SQLiteCommand comm = dbConnection.CreateCommand();

            //enter command text
            comm.CommandText = 
                @"
INSERT INTO player (name, whiteLosses, blackLosses, draws, whiteWins, blackWins, date) 
VALUES ($name, 0, 0, 0, 0, 0, $date);";
            comm.Parameters.AddWithValue("$name", name);
            comm.Parameters.AddWithValue("$date", DateTime.Today);

            comm.ExecuteNonQuery();
            dbConnection.Close();
        }

        public List<Player> getPlayers()
        {
            //set up query
            List<Player> ret = new List<Player>();
            SQLiteConnection dbConnection = new SQLiteConnection("Date Source=database.db");
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
                int draws = reader.GetInt32(4);
                int whiteWins = reader.GetInt32(5);
                int blackWins = reader.GetInt32(6);
                DateTime joinDate = reader.GetDateTime(7);
                Player tmp = new Player(ID, name, whiteLosses, blackLosses, draws, whiteWins, blackWins, joinDate);
                ret.Add(tmp);
            }
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
            int draws = reader.GetInt32(4);
            int whiteWins = reader.GetInt32(5);
            int blackWins = reader.GetInt32(6);
            DateTime joinDate = reader.GetDateTime(7);
            Player ret = new Player(ID, name, whiteLosses, blackLosses, draws, whiteWins, blackWins, joinDate);

            return ret;
        }

        public int createGame(string name)
        {
            SQLiteConnection dbConnection = new SQLiteConnection("Date Source=database.db");
            dbConnection.Open();
            SQLiteCommand comm = dbConnection.CreateCommand();

            comm.CommandText = @"
INSERT INTO GAME (name, moveList, gamestate, lastAccessed) 
VALUES ($name, $empty, $state, $date)";

            //insert params
            comm.Parameters.AddWithValue("$name", name);
            comm.Parameters.AddWithValue("$empty", "");
            comm.Parameters.AddWithValue("$state", (int)Gamestates.Ongoing);
            comm.Parameters.AddWithValue("$date", DateTime.Today);

            comm.ExecuteNonQuery();

            //once game is created need to return its ID to higher level
            comm.CommandText = "SELECT * FROM game ORDER BY gameID DESC LIMIT 1;";
            SQLiteDataReader reader = comm.ExecuteReader();
            int ret = reader.GetInt32(0);

            dbConnection.Close();
            return ret;
        }
    }
}
