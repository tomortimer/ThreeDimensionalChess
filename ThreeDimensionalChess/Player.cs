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
        private int colour;

        public Player(int IDInp, string nameInp, int col)
        {
            ID = IDInp;
            name = nameInp;
            colour = col;
        }

        public int getColour() { return colour; }
        public string getName() { return name; }
    }
}
