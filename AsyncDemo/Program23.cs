using System;
using System.Threading.Tasks;

namespace AsyncDemo
{
    /// <summary>
    /// 例子exception
    /// </summary>
    internal class Program23
    {
        private static void Main23(string[] args)
        {
            Task task = Task.Run(() => { throw null; });
            try
            {
                task.Wait();
            }
            catch (AggregateException aex)
            {// VS2019更改了调试状态时异常的抛出策略：先直接在异常发生点抛出，如果继续才会跳转到catch块中
                if (aex.InnerException is NullReferenceException)
                {
                    Console.WriteLine("Null");
                }
                else
                {
                    throw;
                }
                //aex.Handle(eachException =>
                //{
                //    Console.WriteLine(eachException.Message);
                //    return true;
                //});
            }
        }
    }
}