using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncDemo
{
    /// <summary>
    /// 例子614
    /// </summary>
    class Program44
    {
        static async Task Main44(string[] args)
        {
            await Go(); // main thread
        }

        static async Task Go()
        {
            var task = PrintAnswerToLife();
            await task;
            Console.WriteLine("Done");
        }

        static async Task PrintAnswerToLife()
        {
            var task = GetAnswerToLife();
            int answer = await task;
            Console.WriteLine(answer);
        }

        static async Task<int> GetAnswerToLife()
        {
            var task = Task.Delay(5000);
            await task;
            int answer = 21 * 2;
            return answer;
        }
    }
}