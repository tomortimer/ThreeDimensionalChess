using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeDimensionalChess
{
    class GameInfo
    {
        private int gameID;
        private string name;
        private List<string> moves;
        private int gamestate;
        private DateTime lastAccessed;
        private int whitePlayerID;
        private int blackPlayerID;
        private bool undoMoves;
        public GameInfo(int gameIDInp, string nameInp, string movesInp, int gamestateInp, DateTime lastAccessedInp, int whitePlayerInp, int blackPlayerInp, bool undoMovesInp)
        {
            gameID = gameIDInp;
            name = nameInp;
            moves = new List<string>(movesInp.Split(','));
            gamestate = gamestateInp;
            lastAccessed = lastAccessedInp;
            whitePlayerID = whitePlayerInp;
            blackPlayerID = blackPlayerInp;
            undoMoves = undoMovesInp;
        }

        public int getGameID() { return gameID; }
        public string getName() { return name; }
        public string getWhitePlayerName()
        {
            DatabaseHandler db = new DatabaseHandler();
            string ret = db.getPlayer(whitePlayerID).getName();
            return ret;
        }
        public string getBlackPlayerName()
        {
            DatabaseHandler db = new DatabaseHandler();
            string ret = db.getPlayer(blackPlayerID).getName();
            return ret;
        }
        public bool getUndoMoves() { return undoMoves; }
        public List<string> getMoves() {  return moves; }
        public string getGamestate() 
        {
            //return a string, better for UI rep
            string ret = "Ongoing";
            switch (gamestate)
            {
                case (int)Gamestates.WhiteW:
                    ret = "White Won";
                    break;
                case (int)Gamestates.BlackW:
                    ret = "Black Won";
                    break;
                case (int)Gamestates.Stalemate:
                    ret = "Stalemate";
                    break;
            }
            return ret;
        }
        public int getGamestateForComparison() 
        { 
            // return a -1 if game is ongoing in anyway
            if(gamestate == (int)Gamestates.Ongoing || gamestate == (int)Gamestates.PendingPromo) return -1;
            return gamestate; 
        }
        public int getGamestateAsInt() { return gamestate; }
        public DateTime getLastAccessed() { return lastAccessed; }
        public int getWhitePlayerID() { return whitePlayerID; }
        public int getBlackPlayerID() { return blackPlayerID; }
    }
}
