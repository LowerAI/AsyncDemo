using System;
using System.Threading;

namespace AsyncDemo
{
    class Program5
    {
        static void Main5()
        {
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine("Sleep for 2 seconds.");
                Thread.Sleep(2000);
            }

            Console.WriteLine("Main thread exits.");
        }
    }
}
