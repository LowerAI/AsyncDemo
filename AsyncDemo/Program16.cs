using System;
using System.Threading;

namespace AsyncDemo
{
    /// <summary>
    /// 后台线程
    /// </summary>
    internal class Program16
    {
        private static void Main16(string[] args)
        {
            Thread worker = new Thread(() => Console.ReadLine());
            if (args.Length > 0)
            {
                worker.IsBackground = true;
            }
            worker.Start();
        }
    }
}