using System;
using System.Collections.Concurrent;
using System.Threading;

namespace ThreadPool;

internal class MyThreadPool : IThreadPool
{
    private readonly ConcurrentQueue<Action> actions = new();
    private readonly Thread[] threads;
    private readonly object waiter = new();
    private volatile bool disposed;

    public MyThreadPool(int concurrency)
    {
        threads = new Thread[concurrency];
        for (var i = 0; i < concurrency; i++)
        {
            threads[i] = new Thread(ThreadRoutine);
            threads[i].Start();
        }
    }

    public void Dispose()
    {
        lock (waiter)
        {
            disposed = true;
            Monitor.PulseAll(waiter);
        }

        foreach (var thread in threads)
            thread.Join();
    }

    public void EnqueueAction(Action action)
    {
        lock (waiter)
        {
            actions.Enqueue(action);
            Monitor.Pulse(waiter);
        }
    }

    private void ThreadRoutine()
    {
        while (!disposed)
        {
            if (actions.TryDequeue(out var action))
            {
                try
                {
                    action();
                }
                catch
                {
                }

                continue;
            }

            lock (waiter)
            {
                Monitor.Wait(waiter);
            }
        }
    }
}