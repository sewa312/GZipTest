using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashSign
{
    class HashBlock
    {
        public long ID { get; }
        public byte[] Hash { get; }
        public int ThreadID { get; }

        public HashBlock(long id, byte[] hashcode, int threadID)
        {
            ID = id;
            Hash = hashcode;
            ThreadID = threadID;
        }
    }
}
