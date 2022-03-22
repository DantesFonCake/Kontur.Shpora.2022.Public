using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;

namespace FindTimeQuant
{
    class Program
    {
        static void Main(string[] args)
        {
            var processorNum = args.Length > 0 ? int.Parse(args[0]) - 1 : 1;
            Process.GetCurrentProcess().ProcessorAffinity = (IntPtr)(1 << processorNum);

            var first = new Thread(Spinner);
            var second = new Thread(Timer);
            first.IsBackground = true;
            second.IsBackground = true;
            first.Start();
            second.Start();

            Thread.Sleep(3000);
        }

        static void Spinner()
        {
            var i = 0;
            while (true)
            {
                i = (i + 1) % 1000;
            }
        }

        static void Timer()
        {
            var w = new Stopwatch();
            long prev = 0;
            while (true)
            {
                w.Start();
                w.Stop();
                var elapsed = w.ElapsedMilliseconds;
                w.Reset();
                if (elapsed > prev/2)
                {
                    Console.WriteLine($"Possible quant: {elapsed}");
                    prev=elapsed;
                }
            }
        }

        private static int count = 0;
    }
}