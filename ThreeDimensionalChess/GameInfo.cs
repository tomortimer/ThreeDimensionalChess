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
            //if last move is an unfinished promotion, remove it, set state accordingly
            if (moves[moves.Count() - 1].Contains("=") && moves[moves.Count() - 1].Split('=')[1].Length == 0) { moves.RemoveAt(moves.Count() - 1); gamestate = (int)Gamestates.Ongoing; }
            lastAccessed = lastAccessedInp;
            whitePlayerID = whitePlayerInp;
            blackPlayerID = blackPlayerInp;
            undoMoves = undoMovesInp;
        }

        public int GetGameID() { return gameID; }
        public string GetName() { return name; }
        public string GetWhitePlayerName()
        {
            DatabaseHandler db = new DatabaseHandler();
            string ret = db.GetPlayer(whitePlayerID).GetName();
            return ret;
        }
        public string GetBlackPlayerName()
        {
            DatabaseHandler db = new DatabaseHandler();
            string ret = db.GetPlayer(blackPlayerID).GetName();
            return ret;
        }
        public bool GetUndoMoves() { return undoMoves; }
        public List<string> GetMoves() {  return moves; }
        public string GetGamestate() 
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
        public int GetGamestateForComparison() 
        { 
            // return a -1 if game is ongoing in anyway
            if(gamestate == (int)Gamestates.Ongoing || gamestate == (int)Gamestates.PendingPromo) return -1;
            return gamestate; 
        }
        public string GetLastMove()
        {
            string tmp = moves[moves.Count()-1];
            if (tmp == "") { tmp = "No moves have been made"; }
            return tmp;
        }
        public int GetGamestateAsInt() { return gamestate; }
        public DateTime GetLastAccessed() { return lastAccessed; }
        public int GetWhitePlayerID() { return whitePlayerID; }
        public int GetBlackPlayerID() { return blackPlayerID; }
    }
}
