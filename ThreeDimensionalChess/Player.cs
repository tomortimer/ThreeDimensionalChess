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

        }

        public int getColour() { return colour; }
        public void setColour(int col) { colour = col; }
        public string getName() { return name; }
        public int getWhiteLosses() { return whiteLosses; }
        public int getBlackLosses() { return blackLosses; }
        public int getDraws() { return whiteDraws+blackDraws; }
        public int getWhiteDraws() { return whiteDraws; }
        public int getBlackDraws() { return blackDraws; }
        public int getWhiteWins() { return whiteWins; }
        public int getBlackWins() { return blackWins; }
        public int getTotalWins() {  return blackWins + whiteWins; }
        public int getTotalLosses() { return blackLosses + whiteLosses; }
        public int getID() { return ID; }

        public int getWinrate()
        {
            int WR;
            try 
            { 
                WR = (whiteWins + blackWins) / (whiteLosses + blackLosses + whiteWins + blackWins + whiteDraws + blackDraws); 
            } catch(DivideByZeroException)
            {
                WR = 0;
            }
            return WR;
        }

        public DateOnly getJoinDate()
        {
            return DateOnly.FromDateTime(joinDate);
        }

        public int getWhiteWinrate()
        {
            int WR;
            try
            {
                WR = whiteWins / (whiteLosses + whiteDraws);
            }catch(DivideByZeroException)
            {
                if (whiteWins > 0) { WR = 100; }
                else { WR = 0; }
            }
            return WR;
        }

        public int getBlackWinrate()
        {
            int WR;
            try
            {
                WR = blackWins / (blackLosses + blackDraws);
            }catch(DivideByZeroException)
            {
                if (blackWins > 0) { WR = 100; }
                else { WR = 0; }
            }
            return WR;
        }

        public int getTotalGames() { return whiteLosses + whiteWins + blackWins + blackLosses + whiteDraws + blackDraws; }
    }
}
