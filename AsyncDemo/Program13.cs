using System;
using System.Threading;

namespace AsyncDemo
{
    /// <summary>
    /// 甚至可以把整个逻辑都放在lambda里面
    /// </summary>
    internal class Program13
    {
        private static void Main13()
        {
            new Thread(() =>
            {
                Console.WriteLine("I'm runnning on another thread!");
                Console.WriteLine("This is so easy!");
            }).Start();
        }
    }
}