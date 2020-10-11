using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncDemo
{
    /// <summary>
    /// 例子6133
    /// </summary>
    class Program43
    {
        static async Task Main43(string[] args)
        {
            
        }

        void Go()
        {
            PrintAnswerToLife();
            Console.WriteLine("Done");
        }

        void PrintAnswerToLife()
        {
            int answer = GetAnswerToLife();
            Console.WriteLine(answer);
        }

        int GetAnswerToLife()
        {
            Task.Delay(5000);
            int answer = 21 * 2;
            return answer;
        }
    }
}