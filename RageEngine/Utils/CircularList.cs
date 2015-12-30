// <copyright file="CircularList.cs">
// Copyright (c) 2011, 2011 All Right Reserved
//
// This source is subject to the Microsoft Permissive License.
// Please see the License.txt file for more information.
// All other rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY 
// KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// </copyright>
// <author>Jonathon Penberthy</author>
// <stylecop version="4.5">true</stylecop>
// <fxcop version="10">true</fxcop>
// <unitTests>none</unitTest>
namespace RageEngine
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Collections;

    /// <summary>
    /// Circular List provides a fixed size list that will overright its oldest items when full.
    /// </summary>
    /// <typeparam name="T">Collection Typing</typeparam>
    public class CircularList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IList, ICollection, IEnumerable, IDisposable
    {
        /// <summary>
        /// internal array of item that this collection wrapps
        /// </summary>
        private T[] items;

        /// <summary>
        /// The current number of items in the list
        /// </summary>
        private int count;

        /// <summary>
        /// THe maximum number of ites allowed in this list
        /// </summary>
        private int capacity;

        /// <summary>
        /// the location in the intenal array "items" of the oldest item in the list, equivialant to external index 0
        /// </summary>
        private int head;

        /// <summary>
        /// the location in the internal array "items" of the newest item in the list, equivilant to external index count -1
        /// </summary>
        private int tail;

        /// <summary>
        /// read write lock used to protect the integrity of the internal state of this object
        /// </summary>
        private ReaderWriterLockSlim locker = new ReaderWriterLockSlim();

        /// <summary>
        /// Initializes a new instance of the CircularList class
        /// </summary>
        /// <param name="cap">the maximum number of items allowed in this list</param>
        public CircularList(int cap)
        {
            this.capacity = cap;
            this.items = new T[cap];
            this.count = 0;
            this.head = 0;
        }

        /// <summary>
        /// Gets the number of items in the list
        /// </summary>
        public int Count
        {
            get
            {
                return this.count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this list is read only
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the collection is fixed size, which it  is.
        /// </summary>
        public bool IsFixedSize
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether the list is syncronised
        /// </summary>
        public bool IsSynchronized
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the object used to sycronise access to this collection, however all public methods and properties on this class are sycronised already
        /// </summary>
        public object SyncRoot
        {
            get { return this.locker; }
        }

        /// <summary>
        /// Gets the read write lock used to protect the integrity of the internal state of this object, exposed externaly for the use of CircularEnumerator
        /// </summary>
        internal ReaderWriterLockSlim Locker
        {
            get { return this.locker; }
        }

        /// <summary>
        /// returns thr item referenced by the index passed
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">index > count or index = capacity</exception>
        /// <param name="index">the index of the item to return</param>
        /// <returns>the object at index</returns>
        object IList.this[int index]
        {
            get
            {
                return this[index];
            }

            set
            {
                if (value is T)
                {
                    this[index] = (T)value;
                }
            }
        }

        /// <summary>
        /// Returns the item stored at the "index" in this collection
        /// </summary>
        /// <param name="index">Index of item</param>
        /// <returns>Item at index</returns>
        /// <exception cref="ArgumentOutOfRangeException">index >= to Count</exception>
        public T this[int index]
        {
            get
            {
                if (index >= this.count || index < 0)
                {
                    throw new ArgumentOutOfRangeException("index");
                }

                T item = this.items[this.LocalIndex(index)];
                return item;
                
                
            }

            set
            {
                throw new InvalidOperationException("the content of a circular cache may not be assigned to in this, use Add() instead");
            }
        }

        /// <summary>
        /// Returns the index of the item in the list or -1 if not in list
        /// </summary>
        /// <param name="item">item to be located</param>
        /// <returns>index of item</returns>
        public int IndexOf(T item)
        {
            int index = -1;
            this.locker.EnterReadLock();          
            Parallel.For(0, this.items.Length, i =>
                {
                    if (item.Equals(this.items[i]))
                    {
                        index = this.ExternalIndex(i);
                    }
                });

            this.locker.ExitReadLock();

            // we need to check the it hasn'e found a match beyond the items currently in use
            // which counld happen if this is a list of ints that is not full to capacity
            // and the index of 0 is asked for
            if (index >= this.count)
            {
                index = -1;
            }

            return index;
        }

        /// <summary>
        /// Inserts an item into the list
        /// </summary>
        /// <param name="index">the index at which the item will be inserted</param>
        /// <param name="item">the item to insert</param>
        /// <exception cref="ArgumentOutOfRangeException">if index > count or index >= capacity</exception>
        public void Insert(int index, T item)
        {
            throw new InvalidOperationException("the content of a circular cache may not be assigned to in this, use Add() instead");

            /*this.locker.EnterWriteLock();
            if (index > this.count || index >= this.capacity)
            {
                this.locker.ExitWriteLock();
                throw new ArgumentOutOfRangeException("index");
            }

            this.items[this.LocalIndex(index)] = item;
            this.locker.ExitWriteLock();*/
        }

        /// <summary>
        /// Removes the item at location referenced by index
        /// </summary>
        /// <param name="index">the index of the item to be removed</param>
        /// <remarks>This action is slow in comparrison to Add() and should be avoided where possible</remarks>
        public void RemoveAt(int index)
        {
            if (index >= this.count || index < 0)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            this.locker.EnterWriteLock();
            int myIndex = this.LocalIndex(index);
            for (int i = index; i < this.count - 1; i++)
            {
                this.items[this.LocalIndex(i)] = this[i + 1];
            }

            if (--this.head < 0)
            {
                this.head += this.capacity;
            }

            this.count--;
            this.locker.ExitWriteLock();
        }

        /// <summary>
        /// Adds item to this collection, if Count = Capacity then item replaces 
        /// the item at index 0 and the index used to reference all the the items in the list is then -= 1
        /// </summary>
        /// <param name="item">item to be added</param>
        public void Add(T item)
        {
            this.locker.EnterWriteLock();           
            this.items[this.head] = item;
            if (++this.head >= this.capacity)
            {
                this.head -= this.capacity;
            }

            if (this.count < this.capacity)
            {
                this.count++;
            }
            this.locker.ExitWriteLock();
        }

        /// <summary>
        /// Emties the collection
        /// </summary>
        public void Clear()
        {
            this.locker.EnterWriteLock();
            this.items = new T[this.capacity];
            this.head = 0;
            this.count = 0;
            this.locker.ExitWriteLock();
        }

        /// <summary>
        /// Searches for item in collection
        /// </summary>
        /// <param name="item">item to find</param>
        /// <returns>true if the item is in the collection</returns>
        public bool Contains(T item)
        {
            bool doesContain = false;
            this.locker.EnterReadLock();
            Parallel.ForEach(this.items, (itemsItem,loop) =>
                {
                    doesContain = itemsItem.Equals(item);
                    if(doesContain) loop.Stop();
                });
            this.locker.ExitReadLock();

            return doesContain;
        }

        /// <summary>
        /// Copies the content of this collection to array passed in
        /// </summary>
        /// <param name="array">the array to copy to</param>
        /// <param name="arrayIndex">the index to start at</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            if(this.count > (array.Length - arrayIndex))
            {
                throw new ArgumentOutOfRangeException("array","the size of array - array index is less than the length of this collectio");
            }

            this.locker.EnterReadLock();
            int startAt = this.LocalIndex(0);
            int endAt = this.LocalIndex(this.count - 1);
            if (startAt < endAt)
            {
                System.Array.Copy(this.items, startAt, array, arrayIndex, this.count);
            }
            else
            {
                int toEnd = this.count - startAt;
                System.Array.Copy(this.items, startAt, array, arrayIndex, toEnd);
                System.Array.Copy(this.items, 0, array, arrayIndex+(toEnd), endAt + 1);
            }

            this.locker.ExitReadLock();
        }

        /// <summary>
        /// Removes all items in the list that = item 
        /// </summary>
        /// <remarks>This action is slow in comparrison to the Add() method, avoid using if possible</remarks>
        /// <param name="item">the item to remove</param>
        /// <returns>true if item was removed</returns>
        public bool Remove(T item)
        {
            bool removed = false;
            int index = this.IndexOf(item);
            this.locker.EnterUpgradeableReadLock();
            while (index >= 0)
            {
                this.RemoveAt(index);
                index = this.IndexOf(item);
            }

            this.locker.ExitUpgradeableReadLock();
            return removed;
        }

        /// <summary>
        /// Gets the Enumerator for this collection
        /// </summary>
        /// <remarks>Readlocks this collection whilst in use</remarks>
        /// <returns>The Enumerator</returns>
        public IEnumerator<T> GetEnumerator()
        {
            this.locker.EnterReadLock();
            for (int i = 0; i < this.count; i++)
            {

                yield return this[i];
            }

            this.locker.ExitReadLock();
        }

        /// <summary>
        /// Gets the Enumerator for this collection
        /// </summary>
        /// <remarks>Readlocks this collection whilst in use</remarks>
        /// <returns>The Enumerator</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            this.locker.EnterReadLock();
            int n = this.head;
            for (int i = 0; i < this.count; i++)
            {

                yield return this.items[n];
                n = i == this.capacity ? 0 : n + 1;
            }
            this.locker.ExitReadLock();
        }

        /// <summary>
        /// Adds a new object to the collection
        /// </summary>
        /// <param name="value">the object to add to the collection</param>
        /// <returns>the index of the item added, -1 if insert failed</returns>
        public int Add(object value)
        {
            int index = -1;
            if (value is T)
            {
                this.Add((T)value);
                index = this.count - 1;
            }

            return index;
        }

        /// <summary>
        /// Searches for the object in the list
        /// </summary>
        /// <param name="value">the object to look for</param>
        /// <returns>true if the object is in the list</returns>
        public bool Contains(object value)
        {
            bool doesContain = false;
            if (value is T)
            {
                doesContain = this.Contains((T)value);
            }

            return doesContain;
        }

        /// <summary>
        /// returns the index of the object in the list, returns -1 if the object is not in the list
        /// </summary>
        /// <param name="value">the object to look for</param>
        /// <returns>the index of the object in the list</returns>
        public int IndexOf(object value)
        {
            int index = -1;
            if (value is T)
            {
                index = this.IndexOf((T)value);
            }

            return index;
        }

        /// <summary>
        /// Inserts the object at the specified location in the list
        /// </summary>
        /// <exception cref="ArgumentException">object must be of type T</exception>
        /// <exception cref="ArgumentOutOfRangeException">if the index is > count or = capacity</exception>
        /// <param name="index">the index where the object is ibserted</param>
        /// <param name="value">the object to insert</param>
        public void Insert(int index, object value)
        {
            throw new InvalidOperationException("the content of a circular cache may not be assigned to in this, use Add() instead");

            /*if (value is T)
            {
                this.Insert(index, (T)value);
            }
            else
            {
                throw new ArgumentException("Must be of type" + typeof(T).FullName, "value");
            }*/
        }

        /// <summary>
        /// Remove the object from the collection
        /// </summary>
        /// <param name="value">the object to remove</param>
        public void Remove(object value)
        {
            if (value is T)
            {
                this.Remove((T)value);
            }
        }

        /// <summary>
        /// Used to prevent writes to collection during parallel actions without prevent other write actions.
        /// </summary>
        /// <param name="setReadLockTo">true sets read locak to on false sets it to of for the current thread</param>
        public void SetReadLocked(bool setReadLockTo)
        {
            if (this.locker.IsReadLockHeld != setReadLockTo)
            {
                if (setReadLockTo)
                {
                    this.locker.EnterReadLock();
                }
                else
                {
                    this.locker.EnterReadLock();
                }
            }
        }

        /// <summary>
        /// Copies the content of this list to the array passed
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">index >= count</exception>
        /// <param name="array">the array to copy to</param>
        /// <param name="index">the index of the item to start copieng to</param>
        public void CopyTo(Array array, int index)
        {
            if (index >= this.count)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            this.locker.EnterReadLock();
            Parallel.For(index, this.count, i =>
            {
                if (array.Length > i)
                {
                    array.SetValue(this[i], i);
                }
            });

            this.locker.ExitReadLock();
        }

        /// <summary>
        /// Disposes of all disposable members in this instance
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        /// <summary>
        /// Disposes of all the items in the list
        /// </summary>
        /// <param name="disposeNative">has no effect</param>
        protected virtual void Dispose(bool disposeNative)
        {
            this.head = -1;
            this.locker.Dispose();
            foreach (T item in this.items)
            {
                IDisposable dispItem = item as IDisposable;
                if (dispItem != null)
                {
                    dispItem.Dispose();
                }
            }
        }

        /// <summary>
        /// This method converts the external index to the local index
        /// </summary>
        /// <param name="externalIndex">the index used to reference an item outside this class</param>
        /// <returns>the index in the local T[] items</returns>
        private int LocalIndex(int externalIndex)
        {
            int localIndex = externalIndex + (this.head - this.count);
            if (localIndex < 0)
            {
                localIndex += this.capacity;
            }

            return localIndex;
        }

        /// <summary>
        /// this method coverts the internal index to the one used by code in
        /// </summary>
        /// <param name="internalIndex">an idex that refers to a location in the local T[] items</param>
        /// <returns>the external reference</returns>
        private int ExternalIndex(int internalIndex)
        {
            int extreanIndex = internalIndex - (this.head - this.count);
            if (extreanIndex >= this.capacity)
            {
                extreanIndex -= this.capacity;
            }

            return extreanIndex;
        }
    }
}
