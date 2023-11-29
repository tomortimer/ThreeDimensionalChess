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

        //equivalent sorts copied for GameInfo lists here
        public List<GameInfo> mergeSortName(List<GameInfo> list)
        {
            //exit recursion
            if (list.Count() <= 1) { return list; }

            //split list into two
            int midpoint = list.Count() / 2;
            List<GameInfo> left = new List<GameInfo>();
            List<GameInfo> right = new List<GameInfo>();
            for (int x = 0; x < midpoint; x++)
            {
                left.Add(list[x]);
            }
            for (int x = midpoint; x < list.Count(); x++)
            {
                right.Add(list[x]);
            }
            //recurse with each half
            left = mergeSortName(left);
            right = mergeSortName(right);
            List<GameInfo> sorted = mergeName(left, right);
            return sorted;
        }

        private List<GameInfo> mergeName(List<GameInfo> left, List<GameInfo> right)
        {
            List<GameInfo> ret = new List<GameInfo>();
            // combine and sort elements
            while (left.Count() > 0 || right.Count() > 0)
            {
                //check both lists have elemnts to compare
                if (left.Count() > 0 && right.Count() > 0)
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

        public List<GameInfo> mergeSortDate(List<GameInfo> list)
        {
            //exit recursion
            if (list.Count() <= 1) { return list; }

            //split list into two
            int midpoint = list.Count() / 2;
            List<GameInfo> left = new List<GameInfo>();
            List<GameInfo> right = new List<GameInfo>();
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
            List<GameInfo> ret = mergeDate(left, right);
            return ret;
        }

        private List<GameInfo> mergeDate(List<GameInfo> left, List<GameInfo> right)
        {
            List<GameInfo> ret = new List<GameInfo>();
            // combine and sort elements
            while (left.Count() > 0 || right.Count() > 0)
            {
                //check both lists have elemnts to compare
                if (left.Count() > 0 && right.Count() > 0)
                {
                    //compare and reorder elemnts
                    if (left[0].getLastAccessed() <= right[0].getLastAccessed())
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

        public List<GameInfo> mergeSortState(List<GameInfo> list)
        {
            //exit recursion
            if (list.Count() <= 1) { return list; }

            //split list into two
            int midpoint = list.Count() / 2;
            List<GameInfo> left = new List<GameInfo>();
            List<GameInfo> right = new List<GameInfo>();
            for (int x = 0; x < midpoint; x++)
            {
                left.Add(list[x]);
            }
            for (int x = midpoint; x < list.Count(); x++)
            {
                right.Add(list[x]);
            }
            //recurse with each half
            left = mergeSortState(left);
            right = mergeSortState(right);
            List<GameInfo> sorted = mergeState(left, right);
            return sorted;
        }

        private List<GameInfo> mergeState(List<GameInfo> left, List<GameInfo> right)
        {
            List<GameInfo> ret = new List<GameInfo>();
            // combine and sort elements
            while (left.Count() > 0 || right.Count() > 0)
            {
                //check both lists have elemnts to compare
                if (left.Count() > 0 && right.Count() > 0)
                {
                    //compare and reorder elemnts
                    if (left[0].getGamestateForComparison() <= right[0].getGamestateForComparison())
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

        public List<GameInfo> mergeSortWhitePlayerName(List<GameInfo> list)
        {
            //exit recursion
            if (list.Count() <= 1) { return list; }

            //split list into two
            int midpoint = list.Count() / 2;
            List<GameInfo> left = new List<GameInfo>();
            List<GameInfo> right = new List<GameInfo>();
            for (int x = 0; x < midpoint; x++)
            {
                left.Add(list[x]);
            }
            for (int x = midpoint; x < list.Count(); x++)
            {
                right.Add(list[x]);
            }
            //recurse with each half
            left = mergeSortWhitePlayerName(left);
            right = mergeSortWhitePlayerName(right);
            List<GameInfo> sorted = mergeWhitePlayerName(left, right);
            return sorted;
        }

        private List<GameInfo> mergeWhitePlayerName(List<GameInfo> left, List<GameInfo> right)
        {
            List<GameInfo> ret = new List<GameInfo>();
            // combine and sort elements
            while (left.Count() > 0 || right.Count() > 0)
            {
                //check both lists have elemnts to compare
                if (left.Count() > 0 && right.Count() > 0)
                {
                    //compare and reorder elemnts
                    if (left[0].getWhitePlayerName()[0] <= right[0].getWhitePlayerName()[0])
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

        public List<GameInfo> mergeSortBlackPlayerName(List<GameInfo> list)
        {
            //exit recursion
            if (list.Count() <= 1) { return list; }

            //split list into two
            int midpoint = list.Count() / 2;
            List<GameInfo> left = new List<GameInfo>();
            List<GameInfo> right = new List<GameInfo>();
            for (int x = 0; x < midpoint; x++)
            {
                left.Add(list[x]);
            }
            for (int x = midpoint; x < list.Count(); x++)
            {
                right.Add(list[x]);
            }
            //recurse with each half
            left = mergeSortBlackPlayerName(left);
            right = mergeSortBlackPlayerName(right);
            List<GameInfo> sorted = mergeBlackPlayerName(left, right);
            return sorted;
        }

        private List<GameInfo> mergeBlackPlayerName(List<GameInfo> left, List<GameInfo> right)
        {
            List<GameInfo> ret = new List<GameInfo>();
            // combine and sort elements
            while (left.Count() > 0 || right.Count() > 0)
            {
                //check both lists have elemnts to compare
                if (left.Count() > 0 && right.Count() > 0)
                {
                    //compare and reorder elemnts
                    if (left[0].getBlackPlayerName()[0] <= right[0].getBlackPlayerName()[0])
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
