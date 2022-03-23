using System;
using System.Threading;

namespace ReaderWriterLock;

public class MyRwLock : IRwLock
{
    private readonly object readerLock = new();
    private readonly object writeLock = new();
    private volatile int readers;

    public void ReadLocked(Action action)
    {
        lock (writeLock)
        {
            Interlocked.Increment(ref readers);
        }

        try
        {
            action();
        }
        finally
        {
            lock (readerLock)
            {
                Interlocked.Decrement(ref readers);
                Monitor.PulseAll(readerLock);
            }
        }
    }

    public void WriteLocked(Action action)
    {
        lock (writeLock)
        {
            lock (readerLock)
            {
                while (readers > 0)
                    Monitor.Wait(readerLock);

                action();
            }
        }
    }
}