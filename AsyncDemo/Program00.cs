using System;
using System.Threading;

namespace AsyncDemo
{
    internal class Program00
    {
        private static void Main0(string[] args)
        {
            Thread t = new Thread(WriteY); // 开辟了一个新的线程 Thread
            t.Name = "Y Thread ...";
            t.Start();

            for (int i = 0; i < 1000; i++)
            {
                Console.WriteLine("x");
            }
        }

        private static void WriteY()
        {
            for (int i = 0; i < 1000; i++)
            {
                Console.WriteLine("y");
            }
        }
    }
}