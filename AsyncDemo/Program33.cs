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
    class Program33
    {
        private static void Main33(string[] args)
        {
            DisplayPriimeCountsAsync();
            Console.ReadLine();
        }

        public static Task DisplayPriimeCountsAsync()
        {
            var machine = new PrimesStateMachine();
            machine.DisplayPrimeCountsFrom(0);
            return machine.Task;
        }

        public static void DisplayPriimeCountsFrom(int i)
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

        public static Task<int> GetPrimesCountAsync(int start, int count)
        {
            return Task.Run(() => ParallelEnumerable.Range(start, count).Count(n => Enumerable.Range(2, (int)Math.Sqrt(n) - 1).All(i => n % i > 0)));
        }
    }

    class PrimesStateMachine
    {
        TaskCompletionSource<object> _tcs = new TaskCompletionSource<object>();

        public Task Task { get { return _tcs.Task; } }

        public void DisplayPrimeCountsFrom(int i)
        {
            var awaiter = Program33.GetPrimesCountAsync(i * 1000000 + 2, 1000000).GetAwaiter();
            awaiter.OnCompleted(() =>
            {
                Console.WriteLine(awaiter.GetResult());
                if (++i < 10)
                {
                    DisplayPrimeCountsFrom(i);
                }
                else { Console.WriteLine("Done"); _tcs.SetResult(null); }
            });
        }
    }
}