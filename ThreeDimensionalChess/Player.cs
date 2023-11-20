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
        public int getID() { return ID; }

        public int getWinrate()
        {
            return (whiteWins+blackWins) / (whiteLosses+blackLosses+whiteWins+blackWins+draws);
        }

        public int getWhiteWinrate()
        {
            return whiteWins / whiteLosses;
        }

        public int getBlackWinrate()
        {
            return blackWins / blackLosses;
        }

        public int getTotalGames() { return whiteLosses + whiteWins + blackWins + blackLosses + draws; }
    }
}
