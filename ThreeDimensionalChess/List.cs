﻿using System;
using System.Collections;

namespace ThreeDimensionalChess
{
    class List<T>
    {
        //head node pointing to nothing by default
        private ListNode<T> head = null;

        public List() { }
        public List(T inp)
        {
            Add(inp);
        }
        //constructor that takes an array as input
        public List(T[] arr)
        {
            foreach (T o in arr) { Add(o); }
        }

        //adds to end of list
        public void Add(T inp)
        {
            ListNode<T> tmp = head;
            //checks if head is null, if it is it starts the list there with a new node
            if (head != null)
            {
                //moves through to end of list
                while (tmp.next != null)
                {
                    tmp = tmp.next;
                }
                //sets next reference to a new node
                tmp.next = new ListNode<T>(inp);
            }
            else { head = new ListNode<T>(inp); }

        }

        //ALLOWS THE LIST TO BE ACCESSED BY INDEXERS?!?!?!?
        public T this[int i]
        {
            //uses private methods
            get { return RetrieveAt(i, head); }
            set { SetAt(i, head, value); }
        }

        //private method for retrieving data, adjusted to use while loops like SetAt()
        private T RetrieveAt(int i, ListNode<T> node)
        {
            if (i < 0) { throw new IndexOutOfRangeException(); }
            ListNode<T> tmp = node;
            while (i > 0)
            {
                //if tmp points to null then that means index has run out of bounds
                if (tmp == null) { throw new IndexOutOfRangeException(); }
                tmp = tmp.next;
                i--;
            }
            return tmp.GetData();
        }


        //private set method - didn't work when recursive so need to fix this to work with while I think - it may have work recursively but I just didn't put the method in the indexer... oh well, while is probably safer
        private void SetAt(int i, ListNode<T> node, T inp)
        {
            if (i < 0) { throw new IndexOutOfRangeException(); }
            ListNode<T> tmp = node;
            //runs while i >= 0, fine to do this here since using ints
            while (i > 0)
            {
                //if tmp is empty then that means index has run out of bounds
                if (tmp == null) { throw new IndexOutOfRangeException(); }
                tmp = tmp.next;
                i--;
            }
            tmp.SetData(inp);
        }

        public T RemoveAt(int i)
        {
            ListNode<T> tmp = head;
            T ret = default;
            if (i == 0)
            {
                if (tmp == null) { throw new ArgumentOutOfRangeException(); }
                ret = head.GetData();
                head = tmp.next;
            }
            else
            {
                i--;
                //RETRIEVES THE LIST ITEM BEFORE THE ONE TO REMOVE
                while (i > 0)
                {
                    //if tmp is empty that means index is out of bounds
                    if (tmp == null) { throw new ArgumentOutOfRangeException(); }
                    tmp = tmp.next;
                    i--;
                    if (tmp.next == null) { throw new ArgumentOutOfRangeException(); }
                }
                //dereferences next node and sets reference to next node over
                ret = tmp.next.GetData();
                tmp.next = tmp.next.next;
            }

            return ret;
        }

        //wanted to have these two options for count but functionality's different, so:
        public int Count()
        {
            return Length(head);    
        }

        public int Count(T inp)
        {
            return Search(head, inp);
        }

        public bool Contains(T inp)
        {
            //checks if it contains a thingy
            bool ret = false;
            if (this.Count(inp) > 0)
            {
                ret = true;
            }
            return ret;
        }

        //recursive, looks a bit messy but it's built on the Length method so it's alright really
        private int Search(ListNode<T> node, T inp)
        {
            //if node == null list is empty or somehow got out of bounds or is missing a node, most normally an empty list or the end of one so returns 0
            if (node != null)
            {
                if (node.next != null)
                {
                    //only increments return, via recursion, when data == input
                    if (node.GetData().Equals(inp))
                    {
                        return 1 + Search(node.next, inp);
                    }
                    else { return Search(node.next, inp); }
                }
                else
                {
                    //if we are here, next node is null, therefore recursion unspools
                    if (node.GetData().Equals(inp))
                    {
                        return 1;
                    }
                    else { return 0; }
                }
            }
            else { return 0; }
        }

        //this could also be done with while but seems like clean recursion so I'll leave it in :)
        private int Length(ListNode<T> node)
        {
            //if node == null, the list is empty
            if (node != null)
            {
                return 1 + Length(node.next);
            }
            else { return 0; }
        }

        public void AddFront(T inp)
        {
            //creates new node of input value
            ListNode<T> a = new ListNode<T>(inp);
            //points new node towards current head so it isn't lost when head now points at our new node
            a.next = head;
            head = a;
        }

        //use this for comparisons
        public string ConvertToString()
        {
            string tmpstr = "";
            for (int x = 0; x < Count(); x++)
            {
                tmpstr = tmpstr + Convert.ToString(RetrieveAt(x, head));
                //if not the last record add a comma, can be used to split list later !!!
                if(x !=  Count() - 1) { tmpstr = tmpstr + ","; }
            }
            return tmpstr;
        }

        public void Insert(int i, T inp)
        {
            if (i < 0 || i > this.Count()) { throw new ArgumentOutOfRangeException(); }

            //needs to handle insert at 0 differently BECAUSE SOMEONE WOULD DEFINITELY DO THAT
            if (i == 0)
            {
                this.AddFront(inp);
            }
            else
            {
                //needs to get node before where to insert
                i--;
                ListNode<T> tmp = head;
                while (i > 0)
                {
                    tmp = tmp.next;
                    i--;
                }
                //swaps things around without accidentally derefencing
                ListNode<T> n = new ListNode<T>(inp);
                n.next = tmp.next;
                tmp.next = n;
            }
        }

    }
}
