using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashSign
{
    class ObjectPool<T>
    {
        readonly Func<T> factory;
        readonly Stack<T> items = new Stack<T>();

        public ObjectPool(Func<T> factory)
        {
            this.factory = factory;
        }

        public T Get()
        {
            lock (items)
            {
                if (items.Count > 0)
                {
                    return items.Pop();
                }
                else
                {
                    return factory();
                }
            }
            
        }

        public void Release(T item)
        {
            lock (items)
            {
                items.Push(item);
            }
        }
    }
}
