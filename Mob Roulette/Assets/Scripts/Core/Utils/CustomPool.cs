using System;
using System.Collections.Generic;
using MobRoulette.Core.Interfaces;
using UnityEngine;

namespace MobRoulette.Core.Utils
{
    public class CustomPool<T> where T : IPooled
    {
        private readonly Stack<T> inactiveItems;

        private readonly int maxSize;
        private readonly Action<T> getHandler;
        private readonly Action<T> destroyHandler;
        private readonly Action<T> cleanHandler;
        private readonly Func<T> createHandler;
        private readonly List<T> allItems;

        private readonly int prefabId;

        private T CreateNew()
        {
            if (allItems.Count >= maxSize)
            {
                Debug.LogError("Pool is full");
            }
            
            var item = createHandler();
            allItems.Add(item);
            return item;
        }
        
        public CustomPool(Func<T> createHandler, Action<T> getHandler, Action<T> cleanHandler, Action<T> destroyHandler, int initialSize = 10, int maxSize = 1000)
        {
            this.cleanHandler = cleanHandler;
            this.createHandler = createHandler;
            this.getHandler = getHandler;
            this.destroyHandler = destroyHandler;
            this.maxSize = maxSize;
            inactiveItems = new Stack<T>(maxSize);
            allItems = new List<T>(maxSize);
            for (int i = 0; i < initialSize; i++)
            {
                inactiveItems.Push(CreateNew());
            }
        }

        public T Get()
        {
            var item = inactiveItems.Count == 0  ? CreateNew() : inactiveItems.Pop();
            getHandler(item);
            
            return item;
        }

        public void Release(T instance)
        {
            cleanHandler(instance);
            inactiveItems.Push(instance);
        }

        public void ReleaseAll()
        {
            foreach (T item in allItems)
            {
                if (item.IsInUse)
                {
                    Release(item);
                }
            }
        }

        public void DisposeAll()
        {
            foreach (T allItem in allItems)
            {
                destroyHandler(allItem);
            }
            inactiveItems.Clear();
            allItems.Clear();
        }

    }
}