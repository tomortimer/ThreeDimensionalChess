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
        int whiteLosses;
        int blackLosses;
        int draws;
        int whiteWins;
        int blackWins;
        DateTime joinDate;
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
    }
}
