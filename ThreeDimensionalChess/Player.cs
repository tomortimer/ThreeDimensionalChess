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

        public int GetWinrate()
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

        public DateOnly GetJoinDate()
        {
            return DateOnly.FromDateTime(joinDate);
        }

        public int GetWhiteWinrate()
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

        public int GetBlackWinrate()
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

        public int GetTotalGames() { return whiteLosses + whiteWins + blackWins + blackLosses + whiteDraws + blackDraws; }
    }
}
