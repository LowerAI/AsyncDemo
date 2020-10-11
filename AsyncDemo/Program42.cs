using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncDemo
{
    /// <summary>
    /// 例子6132
    /// </summary>
    class Program42
    {
        static async Task Main42(string[] args)
        {
            
        }

        async Task PrintAnswerToLife()
        {
            int answer = await GetAnswerToLife();
            Console.WriteLine(answer);
        }

        async Task<int> GetAnswerToLife()
        {
            await Task.Delay(5000);
            int answer = 21 * 2;
            return answer;
        }
    }
}