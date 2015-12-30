using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RageEngine.Graphics
{
    public class BatchCollection<T>
    {
        public int Count = 0;
        public int Capacity = 10;
        private int currentStack = 0;
        private int currentStackItemsCount = 0;
        private List<T[]> list;
        private T[] currentList;

        public BatchCollection(int capacity) {
            this.Capacity = capacity;
            list = new List<T[]>();
            currentStack=0;
            list.Add(new T[Capacity]);
            currentList = list[0];
        }

        public void Add(T item) {
            if (currentStackItemsCount==Capacity) {
                currentStack++;
                if (currentStack==list.Count) {
                    currentList = new T[Capacity];
                    list.Add(currentList);
                } else {// Clear previous stored array
                    currentList = list[currentStack];
                    Array.Clear(currentList, 0, Capacity);
                }
                currentStackItemsCount=0;
            }

            currentList[currentStackItemsCount++] = item;
            Count++;
        }

        private int currentStackGet = -1;

        public int CurrentStackCount() {
            return currentStackGet;
        }

        public bool Seek() {
            return ((++currentStackGet)!=(currentStack+1));
        }

        public T[] Get() {
            return list[currentStackGet];
        }

        public int GetItemsCount() {
            return (currentStackGet==currentStack)?currentStackItemsCount:Capacity;
        }

        public void Clear() {
            currentStackGet=-1;
            currentStack=0;
            currentStackItemsCount=0;
            Count=0;
            currentList = list[0];
        }
    }
}
