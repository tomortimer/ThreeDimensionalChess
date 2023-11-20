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
        public GameInfo(int gameIDInp, string nameInp, string movesInp, int gamestateInp, DateTime lastAccessedInp, int whitePlayerInp, int blackPlayerInp) 
        {
            gameID = gameIDInp;
            name = nameInp;
            moves = new List<string>(movesInp.Split(','));
            //remove tailing comma
            moves.RemoveAt(moves.Count() - 1);
            gamestate = gamestateInp;
            lastAccessed = lastAccessedInp;
            whitePlayerID = whitePlayerInp;
            blackPlayerID = blackPlayerInp;
        }

        public int getGameID() { return gameID; }
        public string getName() { return name; }
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
        public int getGamestateAsInt() { return gamestate; }
        public DateTime getLastAccessed() { return lastAccessed; }
        public int getWhitePlayerID() { return whitePlayerID; }
        public int getBlackPlayerID() { return blackPlayerID; }
    }
}
