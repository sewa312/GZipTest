using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HashSign
{
    class Block
    {
        public long ID { get; }
        public byte[] Data { get; }
        public int Size { get; }

        public Block(long id, byte[] inputdData, int size)
        {
            ID = id;
            Data = inputdData;
            this.Size = size;
        }
    }
}
