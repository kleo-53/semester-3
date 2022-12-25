namespace deadlocks;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class Philosophers
{
    private Mutex mutex = new();

    public static Object[] forks { get; set; }

    public static void Beginning(int n)
    {
        forks = new Object[n];
        for (int i = 0; i < n; i++)
        {
            forks[i] = new Object();
        }

        var threads = new Thread[n];
        for (int i = 0; i < n; ++i)
        {
            var localI = i;
            threads[i] = new Thread(() =>
            {
                for (int j = localI; j < n; j += threads.Length)
                {
                    Philosopher(localI);
                }
            });
        }
        foreach (var thread in threads)
        {
            thread.Start();
        }
        Console.ReadLine();
        flag = false;
        foreach (var thread in threads)
        {
            thread.Join();
        }
    }

    private static volatile bool flag = true;

    private static void Philosopher(int number)
    {
        var random = new Random();
        var nextFork = number == forks.Length - 1 ? 0 : (number + 1);
        if (number < nextFork)
        {
            var tmp = number;
            number = nextFork;
            nextFork = tmp;
        }
        while (flag)
        {
            Console.WriteLine($"Philosopher number {number} is thinking");
            Thread.Sleep(random.Next(1, 100));
            Console.WriteLine($"Philosopher number {number} tries to take fork number {number}");
            lock (forks[number])
            {
                Console.WriteLine($"Philosopher number {number} tries to take fork number {nextFork}");
                lock (forks[nextFork])
                {
                    Console.WriteLine($"Philosopher number {number} started eating");
                    Thread.Sleep(random.Next(1, 150));
                    Console.WriteLine($"Philosopher number {number} finished eating");
                    Monitor.Pulse(forks[nextFork]);
                }
                Monitor.Pulse(forks[number]);
            }
        }
    }
}
