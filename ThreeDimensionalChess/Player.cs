using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeDimensionalChess
{
    class Player
    {
        private int ID;
        private string name;
        private int whiteLosses;
        private int blackLosses;
        private int whiteDraws;
        private int blackDraws;
        private int whiteWins;
        private int blackWins;
        private DateTime joinDate;
        private int colour;
        DatabaseHandler db;

        public Player(int IDInp, string nameInp, int whiteLossesInp, int blackLossesInp, int whiteDrawsInp, int blackDrawsInp, int whiteWinsInp, int blackWinsInp, DateTime joinDateInp)
        {
            ID = IDInp;
            name = nameInp;
            whiteLosses = whiteLossesInp;
            blackLosses = blackLossesInp;
            whiteDraws = whiteDrawsInp;
            blackDraws = blackDrawsInp;
            whiteWins = whiteWinsInp;
            blackWins = blackWinsInp;
            joinDate = joinDateInp;
            DatabaseHandler db = new DatabaseHandler();
        }

        // write methods
        public void AddWhiteWin() { whiteWins++;}
        public void AddBlackWin() { blackWins++;}
        public void AddWhiteLoss() { whiteLosses++;}
        public void AddBlackLoss() { blackLosses++; }
        public void AddWhiteDraw() { whiteDraws++; }
        public void AddBlackDraw() {  blackDraws++; }

        // get methods
        public int GetColour() { return colour; }
        public void SetColour(int col) { colour = col; }
        public string GetName() { return name; }
        public int GetWhiteLosses() { return whiteLosses; }
        public int GetBlackLosses() { return blackLosses; }
        public int GetDraws() { return whiteDraws+blackDraws; }
        public int GetWhiteDraws() { return whiteDraws; }
        public int GetBlackDraws() { return blackDraws; }
        public int GetWhiteWins() { return whiteWins; }
        public int GetBlackWins() { return blackWins; }
        public int GetTotalWins() {  return blackWins + whiteWins; }
        public int GetTotalLosses() { return blackLosses + whiteLosses; }
        public int GetID() { return ID; }

        public double GetWinrate()
        {
            double WR;
            WR = (double)(whiteWins + blackWins) / (double)(whiteLosses + blackLosses + whiteWins + blackWins + whiteDraws + blackDraws);
            WR *= 100;
            if (double.IsNaN(WR))
            {
                WR = 0;
                if(whiteWins + blackWins > 0) { WR = 100; }
            }
            return WR;
        }

        public DateOnly GetJoinDate()
        {
            return DateOnly.FromDateTime(joinDate);
        }

        public double GetWhiteWinrate()
        {
            double WR;
            WR = (double)whiteWins / (double)(whiteLosses + whiteDraws + whiteWins);
            WR *= 100;
            //eliminate divide by 0 problems
            if (double.IsNaN(WR)) 
            { 
                WR = 0; 
                if(whiteWins > 0) { WR = 100; }
            }
            return WR;
        }

        public double GetBlackWinrate()
        {
            double WR;
            WR = (double)blackWins / (double)(blackLosses + blackDraws + blackWins);
            WR *= 100;
            //eliminate divide by 0 problems
            if (double.IsNaN(WR)) 
            { 
                WR = 0; 
                if(blackWins > 0) { WR = 100; }
            }
            return WR;
        }

        public int GetTotalGames() { return whiteLosses + whiteWins + blackWins + blackLosses + whiteDraws + blackDraws; }
    }
}
