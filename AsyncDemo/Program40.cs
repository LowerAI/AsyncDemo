using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncDemo
{
    /// <summary>
    /// 例子6121
    /// </summary>
    class Program40
    {
        static async Task Main40(string[] args)
        {
            
        }

        //async Task Go()
        //{
        //    await PrintAnswerToLife();
        //    Console.WriteLine("Done");
        //}

        Task PrintAnswerToLife()
        {
            var tcs = new TaskCompletionSource<object>();
            var awaiter = Task.Delay(5000).GetAwaiter();
            awaiter.OnCompleted(() =>
            {
                try
                {
                    awaiter.GetResult();
                    int answer = 21 * 2;
                    Console.WriteLine(answer);
                    tcs.SetResult(null);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });
            return tcs.Task;
        }
    }
}