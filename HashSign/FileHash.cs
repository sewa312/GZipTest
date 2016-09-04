using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashSign
{
    class FileHash
    {
        public List<HashBlock> signature;

        public FileHash()
        {
            signature = new List<HashBlock>();
        }

        public void Add(long id, byte[] hashcode, int threadID)
        {
            lock (signature)
            {
                signature.Add(new HashBlock(id, hashcode, threadID));
            }
        }

        public void Show()
        {
            lock (signature)
            {
                foreach (var block in signature.OrderBy(b => b.ID))
                {
                    Console.Write("Thread: {0}, block: {1}, hash: ", block.ThreadID, block.ID);
                    for (int i = 0; i < block.Hash.Length; i++)
                    {
                        Console.Write("{0:X2}", block.Hash[i]);
                    }
                    Console.WriteLine();
                }
            }
        }
    }
}
