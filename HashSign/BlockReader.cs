using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HashSign
{
    class BlockReader
    {
        enum ReadResult
        {
            NonLastBlock,
            LastBlock,
            Error,
        }

        readonly string path;
        readonly ObjectPool<byte[]> bufferPool;
        readonly object locking = new object();
        Exception error;

        public Exception Error
        {
            get
            {
                lock (locking)
                {
                    return error;
                }
            }
            private set
            {
                lock (locking)
                {
                    error = value;
                }
            }
        }
        
        public BlockReader(string path, ObjectPool<byte[]> bufferPool)
        {
            this.path = path;
            this.bufferPool = bufferPool;
        }

        public void CutAndSendToQueue(BlockQueue<Block> output)
        {
            FileStream stream;
            try
            {
                stream = new FileStream(path, FileMode.Open);
            }
            catch (Exception ex)
            {
                Error = ex;
                output.Close();
                return;
            }

            long i = 0;
            while (true)
            {
                byte[] blockData = bufferPool.Get();
                int total;

                var result = ReadBlock(stream, blockData, out total);
                if (result == ReadResult.Error)
                    break;

                if (total > 0)
                {
                    output.Enqueue(new Block(i, blockData, total));
                    i++;
                }

                if (result == ReadResult.LastBlock)
                    break;
            }

            try
            {
                stream.Close();
            }
            catch (Exception ex)
            {
                Error = ex;
            }

            output.Close();
        }

        private ReadResult ReadBlock(FileStream stream, byte[] buffer, out int total)
        {
            total = 0;
            int read;
            do
            {
                try
                {
                    read = stream.Read(buffer, total, buffer.Length - total);
                }
                catch (Exception ex)
                {
                    Error = ex;
                    return ReadResult.Error;
                }
                total += read;
            } while (read != 0 && total < buffer.Length);

            return read == 0 ? ReadResult.LastBlock : ReadResult.NonLastBlock;
        }
    }
}
