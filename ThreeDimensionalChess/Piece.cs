using System;
using System.Linq;

namespace ThreeDimensionalChess
{
    //neat little enum to help with generating moves
    enum Directions
    {
        Right, // 0- Positive movement on x
        Left, // 1 - Negative Movement on X
        Up, // 2 - Positive movement on Y
        Down, // 3 - Negative movement on y
        Forwards, // 4 - Positive movement on z
        Backwards, // 5 - Negative movement on z
    }

    abstract class Piece
    {
        //store movement as a three dimensional vector [x, y, z] - can be implemented in constructor of a piece, some don't need this
        public int[] movementVect = new int[3];
        //stores current position on board
        public int currentPosition;
        public int colour;

        //constructor for piece
        public Piece(int startPos, int col)
        {
            currentPosition = startPos;
            colour = col;
        }

        public abstract string getPieceType();

        //method to calculate all possible moves by a piece - returns a list, must be implemented on a per piece basis
        public abstract List<int> generatePossibleMoves(List<Square> board, List<Piece> pieces);

        public virtual string movePiece(int endPosition, List<Square> board, List<Piece> pieces)
        {
            //setup string to return LA3DN data
            string data = "";
            //dereference captured piece here
            int targetPiecePtr = board[endPosition].getPiecePointer();
            if (targetPiecePtr != -1)
            {
                data += "X" + pieces[targetPiecePtr].getPieceType();
                pieces.RemoveAt(targetPiecePtr);
            }
            else { data += "-"; }
            data += convertPosToStr(endPosition);
            currentPosition = endPosition;
            return data;
        }

        public int getCurrentPosition()
        {
            return currentPosition;
        }

        //self explanatory names, useful for eachother
        public int[] convertPtrToVect(int inp)
        {
            //uses modular arithmetic around base 8 to convert a position in the list of squares to a 3D coordinate/vector of the piece's position
            int[] vect = { 0, 0, 0 };
            int remainder = inp % 64;
            vect[2] = inp / 64;
            vect[1] = remainder / 8;
            vect[0] = remainder % 8;

            return vect;
        }

        public int convertVectToPtr(int[] arr)
        {
            // if this method doesn't receive a three dimensional vector in array form, it will throw an argument exception - thankfully this should never happen
            if (arr.Length != 3) { throw new ArgumentException(); }
            int[] currentPosVect = convertPtrToVect(currentPosition);
            int movePtr = 0;

            // uses base 8 system to convert three dimensional vectors into pointers for the list of board squares
            movePtr += arr[0];
            movePtr += arr[1] * 8;
            movePtr += arr[2] * 64;

            return movePtr;
        }

        public int getColour()
        {
            return colour;
        }

        //returns a LA3DN compatible coordinate representation
        public string convertPosToStr(int pos)
        {
            int[] vect = convertPtrToVect(pos);
            string ret = "";
            //parse x coord
            switch (vect[0])
            {
                case 0:
                    ret += "a";
                    break;
                case 1:
                    ret += "b";
                    break;
                case 2:
                    ret += "c";
                    break;
                case 3:
                    ret += "d";
                    break;
                case 4:
                    ret += "e";
                    break;
                case 5:
                    ret += "f";
                    break;
                case 6:
                    ret += "g";
                    break;
                case 7:
                    ret += "h";
                    break;
            }
            //parse y coord
            vect[1]++;
            ret += vect[1];
            //parze z coord
            switch (vect[2])
            {
                case 0:
                    ret += "s";
                    break;
                case 1:
                    ret += "t";
                    break;
                case 2:
                    ret += "u";
                    break;
                case 3:
                    ret += "v";
                    break;
                case 4:
                    ret += "w";
                    break;
                case 5:
                    ret += "x";
                    break;
                case 6:
                    ret += "y";
                    break;
                case 7:
                    ret += "z";
                    break;
            }
            return ret;
        }

        public int convertStrPosToPtr(string pos)
        {
            int[] vect = new int[3];
            switch (pos[0])
            {
                case 'a':
                    vect[0] = 0;
                    break;
                case 'b':
                    vect[0] = 1;
                    break;
                case 'c':
                    vect[0] = 2;
                    break;
                case 'd':
                    vect[0] = 3;
                    break;
                case 'e':
                    vect[0] = 4;
                    break;
                case 'f':
                    vect[0] = 5;
                    break;
                case 'g':
                    vect[0] = 6;
                    break;
                case 'h':
                    vect[0] = 7;
                    break;
            }
            //because pos[1] is a char, need to offset by -48 to get the actual number it represents and then -1 to get a 0-7 digit
            vect[1] = Convert.ToInt32(pos[1]) - 49;
            switch (pos[2])
            {
                case 's':
                    vect[0] = 0;
                    break;
                case 't':
                    vect[0] = 1;
                    break;
                case 'u':
                    vect[0] = 2;
                    break;
                case 'v':
                    vect[0] = 3;
                    break;
                case 'w':
                    vect[0] = 4;
                    break;
                case 'x':
                    vect[0] = 5;
                    break;
                case 'y':
                    vect[0] = 6;
                    break;
                case 'z':
                    vect[0] = 7;
                    break;
            }
            return convertVectToPtr(vect);
        }

        public string getCurrentPosAsStr()
        {
            return convertPosToStr(currentPosition);
        }

        public int[] getCurrentPosAsVect()
        {
            return convertPtrToVect(currentPosition);
        }
    }
}
