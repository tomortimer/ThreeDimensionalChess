﻿using System;
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
            //recurse with each half
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
                    //compare and reorder elemnts
                    if (left[0].getName()[0] <= right[0].getName()[0])
                    {
                        ret.Add(left[0]);
                        left.RemoveAt(0);
                    }
                    else
                    {
                        ret.Add(right[0]);
                        right.RemoveAt(0);
                    }
                }//otherwise add remaining contents of list
                else if(left.Count() > 0)
                {
                    ret.Add(left[0]);
                    left.RemoveAt(0);
                }
                else 
                { 
                    ret.Add(right[0]); 
                    right.RemoveAt(0);
                }
            }
            return ret;
        }

        public List<Player> Reverse(List<Player> list)
        {
            //reverse a list simply, using a stack
            Stack<Player> stck = new Stack<Player>();
            for(int x = 0; x < list.Count(); x++)
            {
                stck.Push(list[x]);
            }
            List<Player> ret = new List<Player>();
            for (int x = 0; x < list.Count(); x++)
            {
                ret.Add(stck.Pop());
            }
            return ret;
        }
    }
}