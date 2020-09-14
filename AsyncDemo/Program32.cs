using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncDemo
{
    /// <summary>
    /// 
    /// </summary>
    class Program32
    {
        private static void Main32(string[] args)
        {
            DisplayPriimeCounts();
            Console.ReadLine();
        }

        static void DisplayPriimeCounts()
        {
            DisplayPriimeCountsFrom(0);
        }

        static void DisplayPriimeCountsFrom(int i)
        {
            var awaiter = GetPrimesCountAsync(i * 1000000 + 2, 1000000).GetAwaiter();
            awaiter.OnCompleted(() =>
            {
                Console.WriteLine(awaiter.GetResult() + " primes between... ");
                if (++i < 10)
                {
                    DisplayPriimeCountsFrom(i);
                }
                else Console.WriteLine("Done!");
            });
        }

        static Task<int> GetPrimesCountAsync(int start, int count)
        {
            return Task.Run(() => ParallelEnumerable.Range(start, count).Count(n => Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i => n % i > 0)));
        }
    }
}