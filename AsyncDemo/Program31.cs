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
    class Program31
    {
        private static void Main31(string[] args)
        {
            //DisplayPriimeCounts();
            Task.Run(() => DisplayPriimeCounts());
        }

        static void DisplayPriimeCounts()
        {
            for (int i = 0; i < 10; i++)
            {
                var awaiter = GetPrimesCountAsync(i * 1000000 + 2, 1000000).GetAwaiter();
                awaiter.OnCompleted(() => Console.WriteLine(awaiter.GetResult() + " primes between... "));
            }
            //for (int i = 0; i < 10; i++)
            //{
            //    Console.WriteLine(GetPrimesCount(i * 1000000 + 2, 1000000) + " primes between " + (i * 1000000) + " and " + ((i + 1) * 1000000 - 1));
            //}
            Console.WriteLine("Done!");
        }

        static Task<int> GetPrimesCountAsync(int start, int count)
        {
            return Task.Run(() => ParallelEnumerable.Range(start, count).Count(n => Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i => n % i > 0)));
        }
    }
}