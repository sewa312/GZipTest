using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace HashSign
{
    class Hasher
    {
        readonly SHA256 sha256;
        readonly BlockQueue<Block> queue;
        readonly ObjectPool<byte[]> bufferPool;

        public Hasher(BlockQueue<Block> queue, ObjectPool<byte[]> bufferPool)
        {
            sha256 = SHA256Managed.Create();
            this.queue = queue;
            this.bufferPool = bufferPool;
        }

        public void Start(FileHash output)
        {
            Block dataToHash;

            while (queue.Dequeue(out dataToHash))
            {
                SingleBlockHash(dataToHash, output);
            }
        }

        private void SingleBlockHash(Block toHash, FileHash output)
        {
            byte[] hash = sha256.ComputeHash(toHash.Data, 0, toHash.Size);
            bufferPool.Release(toHash.Data);

            output.Add(toHash.ID, hash, Thread.CurrentThread.ManagedThreadId);
        }
    }
}
