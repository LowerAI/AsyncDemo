﻿using System;
using System.Threading;

namespace AsyncDemo
{
    internal class Program1
    {
        private static void Main1(string[] args)
        {
            Thread t = new Thread(Go);
            t.Start();
            t.Join();
            Console.WriteLine("Thread t has ended!");
        }

        private static void Go()
        {
            for (int i = 0; i < 1000; i++)
            {
                Console.Write("y");
            }
        }
    }
}