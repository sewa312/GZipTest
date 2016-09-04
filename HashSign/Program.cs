using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Security.Cryptography;
using System.Diagnostics;

namespace HashSign
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: HashSign.exe <file path> <block size>");
                Environment.Exit(1);
            }

            string path = args[0];
            long blockSize;
            if (!long.TryParse(args[1], out blockSize))
            {
                Console.Error.WriteLine("Invalid block size");
                Environment.Exit(2);
            }

            Stopwatch watch = Stopwatch.StartNew();

            int processors = Environment.ProcessorCount;

            BlockQueue<Block> queue = new BlockQueue<Block>(processors * 8);
            ObjectPool<byte[]> bufferPool = new ObjectPool<byte[]>(() => new byte[blockSize]);

            BlockReader file = new BlockReader(path, bufferPool);
            FileHash fileHash = new FileHash();

            Thread[] hashThreads = new Thread[processors];
            for (int i = 0; i < hashThreads.Length; i++)
            {
                var hasher = new Hasher(queue, bufferPool);
                Thread thread = new Thread(() => { hasher.Start(fileHash); });
                thread.Start();
                hashThreads[i] = thread;
            }

            Thread cutThread = new Thread(() => { file.CutAndSendToQueue(queue); });
            cutThread.Start();

            foreach (Thread hashThread in hashThreads)
            {
                hashThread.Join();
            }

            watch.Stop();
            Console.WriteLine("elapsed time {0} ms", watch.ElapsedMilliseconds);

            if (file.Error == null)
            {
                fileHash.Show();
            }
            else
            {
                Console.Error.WriteLine("The following error happened: {0}", file.Error);
                Environment.Exit(3);
            }
            
            Console.ReadLine();
        }
    }
}
