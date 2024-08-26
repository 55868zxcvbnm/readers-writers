using System;
using System.Threading;

class ReaderWriterProblem
{
    private static int readCount = 0;
    private static object readCountLock = new object();
    private static object resourceLock = new object();

    static void Main(string[] args)
    {
        Random random = new Random();
        Thread[] threads = new Thread[10];

        for (int i = 0; i < threads.Length; i++)
        {
            if (i % 2 == 0)
            {
                threads[i] = new Thread(() => Reader(random.Next(1000, 5000)));
            }
            else
            {
                threads[i] = new Thread(() => Writer(random.Next(1000, 5000)));
            }
            threads[i].Start();
        }

        foreach (Thread thread in threads)
        {
            thread.Join();
        }

        Console.WriteLine("Todos los hilos han terminado.");
    }

    static void Reader(int delay)
    {
        while (true)
        {
            Monitor.Enter(readCountLock);
            readCount++;
            if (readCount == 1)
            {
                Monitor.Enter(resourceLock);
            }
            Monitor.Exit(readCountLock);

            Console.WriteLine($"Lector {Thread.CurrentThread.ManagedThreadId} está leyendo.");
            Thread.Sleep(delay);

            Monitor.Enter(readCountLock);
            readCount--;
            if (readCount == 0)
            {
                Monitor.Exit(resourceLock);
            }
            Monitor.Exit(readCountLock);

            Console.WriteLine($"Lector {Thread.CurrentThread.ManagedThreadId} ha terminado de leer.");
            Thread.Sleep(delay);
        }
    }

    static void Writer(int delay)
    {
        while (true)
        {
            Monitor.Enter(resourceLock);
            Console.WriteLine($"Escritor {Thread.CurrentThread.ManagedThreadId} está escribiendo.");
            Thread.Sleep(delay);
            Console.WriteLine($"Escritor {Thread.CurrentThread.ManagedThreadId} ha terminado de escribir.");
            Monitor.Exit(resourceLock);

            Thread.Sleep(delay);
        }
    }
}