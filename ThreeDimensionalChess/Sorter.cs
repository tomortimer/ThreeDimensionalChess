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
            for(int x =  midpoint; x < list.Count(); x++)
            {
                right.Add(list[x]);
            }
            //recurse with each half
            left = mergeSortString(left);
            right = mergeSortString(right);
            List<Player> sorted = mergeString(left, right);
            return sorted;
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

        public List<Player> mergeSortDate(List<Player> list)
        {
            //exit recursion
            if (list.Count() <= 1) { return list; }

            //split list into two
            int midpoint = list.Count() / 2;
            List<Player> left = new List<Player>();
            List<Player> right = new List<Player>();
            for (int x = 0; x < midpoint; x++)
            {
                left.Add(list[x]);
            }
            for (int x = midpoint; x < list.Count(); x++)
            {
                right.Add(list[x]);
            }
            //recurse with each half
            left = mergeSortDate(left);
            right = mergeSortDate(right);
            List<Player> ret = mergeDate(left, right);
            return ret;
        }

        private List<Player> mergeDate(List<Player> left, List<Player> right)
        {
            List<Player> ret = new List<Player>();
            // combine and sort elements
            while (left.Count() > 0 || right.Count() > 0)
            {
                //check both lists have elemnts to compare
                if (left.Count() > 0 && right.Count() > 0)
                {
                    //compare and reorder elemnts
                    if (left[0].getJoinDate() <= right[0].getJoinDate())
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
                else if (left.Count() > 0)
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

        public List<Player> mergeSortWins(List<Player> list)
        {
            //exit recursion
            if (list.Count() <= 1) { return list; }

            //split list into two
            int midpoint = list.Count() / 2;
            List<Player> left = new List<Player>();
            List<Player> right = new List<Player>();
            for (int x = 0; x < midpoint; x++)
            {
                left.Add(list[x]);
            }
            for (int x = midpoint; x < list.Count(); x++)
            {
                right.Add(list[x]);
            }
            //recurse with each half
            left = mergeSortWins(left);
            right = mergeSortWins(right);
            List<Player> ret = mergeWins(left, right);
            return ret;
        }

        private List<Player> mergeWins(List<Player> left, List<Player> right)
        {
            List<Player> ret = new List<Player>();
            // combine and sort elements
            while (left.Count() > 0 || right.Count() > 0)
            {
                //check both lists have elemnts to compare
                if (left.Count() > 0 && right.Count() > 0)
                {
                    //compare and reorder elemnts
                    if (left[0].getWinrate() <= right[0].getWinrate())
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
                else if (left.Count() > 0)
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

        public List<Player> mergeSortBlackWR(List<Player> list)
        {
            //exit recursion
            if (list.Count() <= 1) { return list; }

            //split list into two
            int midpoint = list.Count() / 2;
            List<Player> left = new List<Player>();
            List<Player> right = new List<Player>();
            for (int x = 0; x < midpoint; x++)
            {
                left.Add(list[x]);
            }
            for (int x = midpoint; x < list.Count(); x++)
            {
                right.Add(list[x]);
            }
            //recurse with each half
            left = mergeSortBlackWR(left);
            right = mergeSortBlackWR(right);
            List<Player> ret = mergeBlackWR(left, right);
            return ret;
        }

        private List<Player> mergeBlackWR(List<Player> left, List<Player> right)
        {
            List<Player> ret = new List<Player>();
            // combine and sort elements
            while (left.Count() > 0 || right.Count() > 0)
            {
                //check both lists have elemnts to compare
                if (left.Count() > 0 && right.Count() > 0)
                {
                    //compare and reorder elemnts
                    if (left[0].getBlackWinrate() <= right[0].getBlackWinrate())
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
                else if (left.Count() > 0)
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

        public List<Player> mergeSortWhiteWR(List<Player> list)
        {
            //exit recursion
            if (list.Count() <= 1) { return list; }

            //split list into two
            int midpoint = list.Count() / 2;
            List<Player> left = new List<Player>();
            List<Player> right = new List<Player>();
            for (int x = 0; x < midpoint; x++)
            {
                left.Add(list[x]);
            }
            for (int x = midpoint; x < list.Count(); x++)
            {
                right.Add(list[x]);
            }
            //recurse with each half
            left = mergeSortWhiteWR(left);
            right = mergeSortWhiteWR(right);
            List<Player> ret = mergeWhiteWR(left, right);
            return ret;
        }

        private List<Player> mergeWhiteWR(List<Player> left, List<Player> right)
        {
            List<Player> ret = new List<Player>();
            // combine and sort elements
            while (left.Count() > 0 || right.Count() > 0)
            {
                //check both lists have elemnts to compare
                if (left.Count() > 0 && right.Count() > 0)
                {
                    //compare and reorder elemnts
                    if (left[0].getWhiteWinrate() <= right[0].getWhiteWinrate())
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
                else if (left.Count() > 0)
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

        public List<GameInfo> Reverse(List<GameInfo> list)
        {
            //reverse a list simply, using a stack
            Stack<GameInfo> stck = new Stack<GameInfo>();
            for (int x = 0; x < list.Count(); x++)
            {
                stck.Push(list[x]);
            }
            List<GameInfo> ret = new List<GameInfo>();
            for (int x = 0; x < list.Count(); x++)
            {
                ret.Add(stck.Pop());
            }
            return ret;
        }
    }
}
