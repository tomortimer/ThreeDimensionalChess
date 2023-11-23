using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreeDimensionalChess
{
    class Sorter
    {
        public Sorter() { }

        public List<Player> mergeSortString(List<Player> list)
        {
            //exit recursion
            if (list.Count() <= 1) { return list; }

            //split list into two
            int midpoint = list.Count() / 2;
            List<Player> left = new List<Player>();
            List<Player> right = new List<Player>();
            for(int x = 0; x < midpoint; x++)
            {
                left.Add(list[x]);
            }
            for(int x =  midpoint + 1; x < list.Count(); x++)
            {
                right.Add(list[x]);
            }

            mergeSortString(left);
            mergeSortString(right);
            List<Player> ret = mergeString(left, right);
            return ret;
        }

        private List<Player> mergeString(List<Player> left, List<Player> right)
        {
            List<Player> ret = new List<Player>();
            // combine and sort elements
            while(left.Count()  > 0 || right.Count() > 0) 
            {
                //check both lists have elemnts to compare
                if(left.Count() > 0 && right.Count() > 0)
                {
                    if (left[0] <= right[0])
                    {
                        ret.Add(left[0])
                    }
                }
            }
        }
    }
}
