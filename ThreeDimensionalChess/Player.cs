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
        private int draws;
        private int whiteWins;
        private int blackWins;
        private DateTime joinDate;
        private int colour;

        public Player(int IDInp, string nameInp, int whiteLossesInp, int blackLossesInp, int drawsInp, int whiteWinsInp, int blackWinsInp, DateTime joinDateInp)
        {
            ID = IDInp;
            name = nameInp;
            whiteLosses = whiteLossesInp;
            blackLosses = blackLossesInp;
            draws = drawsInp;
            whiteWins = whiteWinsInp;
            blackWins = blackWinsInp;
            joinDate = joinDateInp;

        }

        public int getColour() { return colour; }
        public void setColour(int col) { colour = col; }
        public string getName() { return name; }
        public int getWhiteLosses() { return whiteLosses; }
        public int getBlackLosses() { return blackLosses; }
        public int getDraws() { return draws; }
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
                WR = (whiteWins + blackWins) / (whiteLosses + blackLosses + whiteWins + blackWins + draws); 
            } catch(DivideByZeroException e)
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
                WR = whiteWins / whiteLosses;
            }catch(DivideByZeroException e)
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
                WR = blackWins / blackLosses;
            }catch(DivideByZeroException e)
            {
                if (blackWins > 0) { WR = 100; }
                else { WR = 0; }
            }
            return WR;
        }

        public int getTotalGames() { return whiteLosses + whiteWins + blackWins + blackLosses + draws; }
    }
}
