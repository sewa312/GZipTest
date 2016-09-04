using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace HashSign
{
    class BlockQueue<T>
    {
        private Queue<T> queue;
        private readonly int size;
        private bool closed;

        public BlockQueue(int maxSize)
        {
            size = maxSize;
            closed = false;
            queue = new Queue<T>();
        }

        public void Enqueue(T item)
        {
            lock (queue)
            {
                while (true)
                {
                    if (closed)
                    {
                        throw new InvalidOperationException("Queue is closed");
                    }
                    else if (queue.Count < size)
                    {
                        queue.Enqueue(item);
                        Monitor.Pulse(queue);
                        break;
                    }
                    Monitor.Wait(queue);
                }
            }
        }

        public bool Dequeue(out T item)
        {
            lock (queue)
            {
                while (true)
                {
                    if (queue.Count > 0)
                    {
                        item = queue.Dequeue();
                        Monitor.Pulse(queue);
                        return true; 
                    }
                    else if (closed)
                    {
                        item = default(T);
                        return false;
                    }
                    Monitor.Wait(queue);
                }
            }
        }

        public void Close()
        {
            lock (queue)
            {
                closed = true;
                Monitor.PulseAll(queue);
            }
        }
    }
}
