using System;
using System.Threading;

namespace AsyncDemo
{
    /// <summary>
    ///
    /// </summary>
    internal class Program15
    {
        //static void Main()
        //{
        //    try
        //    {
        //        new Thread(Go).Start();
        //    }
        //    catch (Exception ex)
        //    {
        //        // We'll never get here!
        //        Console.WriteLine("Exception!");
        //    }
        //}

        //static void Go() { throw null; } // Throws a NullReferenceException

        // 补救办法就是把异常处理放在 Go 方法里 (remedy-exception)

        public static void Main15()
        {
            new Thread(Go).Start();
        }

        private static void Go()
        {
            try
            {
                throw null;
            }
            catch (Exception ex)
            {
                // We'll never get here!
                Console.WriteLine("Exception!");
            }
        } // Throws a NullReferenceException
    }
}