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

        public List<Player> MergeSortString(List<Player> list)
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
            left = MergeSortString(left);
            right = MergeSortString(right);
            List<Player> sorted = MergeString(left, right);
            return sorted;
        }

        private List<Player> MergeString(List<Player> left, List<Player> right)
        {
            List<Player> ret = new List<Player>();
            // combine and sort elements
            while(left.Count()  > 0 || right.Count() > 0) 
            {
                //check both lists have elemnts to compare
                if(left.Count() > 0 && right.Count() > 0)
                {
                    //compare and reorder elemnts
                    if (left[0].GetName()[0] <= right[0].GetName()[0])
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

        public List<Player> MergeSortDate(List<Player> list)
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
            left = MergeSortDate(left);
            right = MergeSortDate(right);
            List<Player> ret = MergeDate(left, right);
            return ret;
        }

        private List<Player> MergeDate(List<Player> left, List<Player> right)
        {
            List<Player> ret = new List<Player>();
            // combine and sort elements
            while (left.Count() > 0 || right.Count() > 0)
            {
                //check both lists have elemnts to compare
                if (left.Count() > 0 && right.Count() > 0)
                {
                    //compare and reorder elemnts
                    if (left[0].GetJoinDate() <= right[0].GetJoinDate())
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

        public List<Player> MergeSortWins(List<Player> list)
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
            left = MergeSortWins(left);
            right = MergeSortWins(right);
            List<Player> ret = MergeWins(left, right);
            return ret;
        }

        private List<Player> MergeWins(List<Player> left, List<Player> right)
        {
            List<Player> ret = new List<Player>();
            // combine and sort elements
            while (left.Count() > 0 || right.Count() > 0)
            {
                //check both lists have elemnts to compare
                if (left.Count() > 0 && right.Count() > 0)
                {
                    //compare and reorder elemnts
                    if (left[0].GetWinrate() <= right[0].GetWinrate())
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

        public List<Player> MergeSortBlackWR(List<Player> list)
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
            left = MergeSortBlackWR(left);
            right = MergeSortBlackWR(right);
            List<Player> ret = MergeBlackWR(left, right);
            return ret;
        }

        private List<Player> MergeBlackWR(List<Player> left, List<Player> right)
        {
            List<Player> ret = new List<Player>();
            // combine and sort elements
            while (left.Count() > 0 || right.Count() > 0)
            {
                //check both lists have elemnts to compare
                if (left.Count() > 0 && right.Count() > 0)
                {
                    //compare and reorder elemnts
                    if (left[0].GetBlackWinrate() <= right[0].GetBlackWinrate())
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

        public List<Player> MergeSortWhiteWR(List<Player> list)
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
            left = MergeSortWhiteWR(left);
            right = MergeSortWhiteWR(right);
            List<Player> ret = MergeWhiteWR(left, right);
            return ret;
        }

        private List<Player> MergeWhiteWR(List<Player> left, List<Player> right)
        {
            List<Player> ret = new List<Player>();
            // combine and sort elements
            while (left.Count() > 0 || right.Count() > 0)
            {
                //check both lists have elemnts to compare
                if (left.Count() > 0 && right.Count() > 0)
                {
                    //compare and reorder elemnts
                    if (left[0].GetWhiteWinrate() <= right[0].GetWhiteWinrate())
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
        public List<GameInfo> MergeSortName(List<GameInfo> list)
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
            left = MergeSortName(left);
            right = MergeSortName(right);
            List<GameInfo> sorted = MergeName(left, right);
            return sorted;
        }

        private List<GameInfo> MergeName(List<GameInfo> left, List<GameInfo> right)
        {
            List<GameInfo> ret = new List<GameInfo>();
            // combine and sort elements
            while (left.Count() > 0 || right.Count() > 0)
            {
                //check both lists have elemnts to compare
                if (left.Count() > 0 && right.Count() > 0)
                {
                    //compare and reorder elemnts
                    if (left[0].GetName()[0] <= right[0].GetName()[0])
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

        public List<GameInfo> MergeSortDate(List<GameInfo> list)
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
            left = MergeSortDate(left);
            right = MergeSortDate(right);
            List<GameInfo> ret = MergeDate(left, right);
            return ret;
        }

        private List<GameInfo> MergeDate(List<GameInfo> left, List<GameInfo> right)
        {
            List<GameInfo> ret = new List<GameInfo>();
            // combine and sort elements
            while (left.Count() > 0 || right.Count() > 0)
            {
                //check both lists have elemnts to compare
                if (left.Count() > 0 && right.Count() > 0)
                {
                    //compare and reorder elemnts
                    if (left[0].GetLastAccessed() <= right[0].GetLastAccessed())
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

        public List<GameInfo> MergeSortState(List<GameInfo> list)
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
            left = MergeSortState(left);
            right = MergeSortState(right);
            List<GameInfo> sorted = MergeState(left, right);
            return sorted;
        }

        private List<GameInfo> MergeState(List<GameInfo> left, List<GameInfo> right)
        {
            List<GameInfo> ret = new List<GameInfo>();
            // combine and sort elements
            while (left.Count() > 0 || right.Count() > 0)
            {
                //check both lists have elemnts to compare
                if (left.Count() > 0 && right.Count() > 0)
                {
                    //compare and reorder elemnts
                    if (left[0].GetGamestateForComparison() <= right[0].GetGamestateForComparison())
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

        public List<GameInfo> MergeSortWhitePlayerName(List<GameInfo> list)
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
            left = MergeSortWhitePlayerName(left);
            right = MergeSortWhitePlayerName(right);
            List<GameInfo> sorted = MergeWhitePlayerName(left, right);
            return sorted;
        }

        private List<GameInfo> MergeWhitePlayerName(List<GameInfo> left, List<GameInfo> right)
        {
            List<GameInfo> ret = new List<GameInfo>();
            // combine and sort elements
            while (left.Count() > 0 || right.Count() > 0)
            {
                //check both lists have elemnts to compare
                if (left.Count() > 0 && right.Count() > 0)
                {
                    //compare and reorder elemnts
                    if (left[0].GetWhitePlayerName()[0] <= right[0].GetWhitePlayerName()[0])
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

        public List<GameInfo> MergeSortBlackPlayerName(List<GameInfo> list)
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
            left = MergeSortBlackPlayerName(left);
            right = MergeSortBlackPlayerName(right);
            List<GameInfo> sorted = MergeBlackPlayerName(left, right);
            return sorted;
        }

        private List<GameInfo> MergeBlackPlayerName(List<GameInfo> left, List<GameInfo> right)
        {
            List<GameInfo> ret = new List<GameInfo>();
            // combine and sort elements
            while (left.Count() > 0 || right.Count() > 0)
            {
                //check both lists have elemnts to compare
                if (left.Count() > 0 && right.Count() > 0)
                {
                    //compare and reorder elemnts
                    if (left[0].GetBlackPlayerName()[0] <= right[0].GetBlackPlayerName()[0])
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
