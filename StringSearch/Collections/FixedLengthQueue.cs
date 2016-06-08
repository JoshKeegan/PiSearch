/*
 * PiSearch
 * FixedLengthQueue - a generic Queue that has a fixed number of elements
 *  Can use the additional contraint of a fixed length to improve performance over that of a regular Queue
 * By Josh Keegan 01/12/2014
 * Last Edit 08/06/2016
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringSearch.Collections
{
    public class FixedLengthQueue<T>
    {
        //Private Vars
        private readonly T[] array;
        private int head;

        //Public vars
        public int Count
        {
            get
            {
                return array.Length;
            }
        }

        public int Length
        {
            get
            {
                return Count;
            }
        }

        public T this[int i]
        {
            get
            {
                //Validation
                if(i < 0 || i >= array.Length)
                {
                    throw new IndexOutOfRangeException("index must be >=0 and < Length");
                }

                int arrayIdx = (i + head) % array.Length;

                return array[arrayIdx];
            }
        }

        //Constructors
        public FixedLengthQueue(int length)
        { 
            if(length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), "must be >= 0");
            }

            array = new T[length];
            head = 0;
        }

        public FixedLengthQueue(T[] initialValues, int head = 0)
        {
            //Validation
            if (head < 0 || head >= initialValues.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(head), "must be >= 0 and < initialValues.Length");
            }

            array = initialValues;
            this.head = head;
        }

        //Public methods
        public T DequeueEnqueue(T t)
        {
            T toRet = array[head];
            Enqueue(t);
            return toRet;
        }

        public void Enqueue(T t)
        {
            array[head] = t;
            head++;

            //Roll the head around if necessary
            head %= array.Length;
        }
    }
}
