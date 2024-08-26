using System;
using System.Threading;

class ReaderWriterProblem
{
    // Contador de lectores activos
    private static int readCount = 0;

    // Lock para controlar el acceso al contador de lectores
    private static object readCountLock = new object();

    // Lock para controlar el acceso al recurso compartido
    private static object resourceLock = new object();

    static void Main(string[] args)
    {
        Random random = new Random();
        // Creación de un array de hilos (threads)
        Thread[] threads = new Thread[10];

        // Inicialización y arranque de los hilos
        for (int i = 0; i < threads.Length; i++)
        {
            // Alterna entre crear un lector y un escritor
            if (i % 2 == 0)
            {
                // Crea un hilo lector
                threads[i] = new Thread(() => Reader(random.Next(1000, 5000)));
            }
            else
            {
                // Crea un hilo escritor
                threads[i] = new Thread(() => Writer(random.Next(1000, 5000)));
            }
            // Inicia el hilo
            threads[i].Start();
        }

        // Espera a que todos los hilos terminen su ejecución
        foreach (Thread thread in threads)
        {
            thread.Join();
        }

        Console.WriteLine("Todos los hilos han terminado.");
    }

    // Método que representa la operación de lectura
    static void Reader(int delay)
    {
        while (true)
        {
            // Bloquea el acceso al contador de lectores
            Monitor.Enter(readCountLock);
            readCount++;
            // Si este es el primer lector, bloquea el acceso al recurso
            if (readCount == 1)
            {
                Monitor.Enter(resourceLock);
            }
            // Libera el lock del contador de lectores
            Monitor.Exit(readCountLock);

            // Simula la lectura
            Console.WriteLine($"Lector {Thread.CurrentThread.ManagedThreadId} está leyendo.");
            Thread.Sleep(delay); // Simula el tiempo de lectura

            // Bloquea nuevamente el acceso al contador de lectores
            Monitor.Enter(readCountLock);
            readCount--;
            // Si este es el último lector, libera el lock del recurso
            if (readCount == 0)
            {
                Monitor.Exit(resourceLock);
            }
            // Libera el lock del contador de lectores
            Monitor.Exit(readCountLock);

            // Simula el final de la lectura
            Console.WriteLine($"Lector {Thread.CurrentThread.ManagedThreadId} ha terminado de leer.");
            Thread.Sleep(delay); // Espera antes de intentar leer de nuevo
        }
    }

    // Método que representa la operación de escritura
    static void Writer(int delay)
    {
        while (true)
        {
            // Bloquea el acceso al recurso para escribir
            Monitor.Enter(resourceLock);
            // Simula la escritura
            Console.WriteLine($"Escritor {Thread.CurrentThread.ManagedThreadId} está escribiendo.");
            Thread.Sleep(delay); // Simula el tiempo de escritura
            Console.WriteLine($"Escritor {Thread.CurrentThread.ManagedThreadId} ha terminado de escribir.");
            // Libera el lock del recurso
            Monitor.Exit(resourceLock);

            // Espera antes de intentar escribir de nuevo
            Thread.Sleep(delay);
        }
    }
}


/*
Explicación de las Funcionalidades:

Bloqueo y Contadores:

readCount: Es un contador que lleva la cuenta de cuántos lectores están leyendo actualmente.
readCountLock: Un objeto de bloqueo para sincronizar el acceso al contador readCount.
resourceLock: Un objeto de bloqueo para sincronizar el acceso al recurso compartido (lo que los lectores leen y los escritores escriben).

Lógica de Lectura (Reader):
Un lector incrementa readCount cuando comienza a leer. Si es el primer lector, también bloquea resourceLock para evitar que los
escritores accedan al recurso mientras hay lectores activos.
Después de leer, el lector decrece readCount. Si es el último lector, libera resourceLock permitiendo que los escritores accedan al recurso.

Lógica de Escritura (Writer):
Un escritor simplemente bloquea resourceLock antes de escribir, asegurando que ningún lector o escritor pueda acceder al recurso mientras escribe.
Después de escribir, libera resourceLock.

Creación y Ejecución de Hilos:
En Main, se crean hilos alternando entre lectores y escritores. Se utilizan para simular operaciones concurrentes sobre un recurso compartido.

Simulación de Concurrencia:
El Thread.Sleep(delay) dentro de los métodos Reader y Writer simula el tiempo que toma realizar la lectura o escritura. Esto ayuda
a visualizar cómo los lectores y escritores compiten por el acceso al recurso.
*/

