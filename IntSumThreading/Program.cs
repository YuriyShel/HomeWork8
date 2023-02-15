using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IntSumThreading
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Введите количество генерируемых int'ов:");
            int[] generated = new int[] { };
            if (int.TryParse(Console.ReadLine(), out int count))
            {
                generated = Generator(count);
            }
            Stopwatch stopBasic = new Stopwatch();
            Stopwatch stopParallel = new Stopwatch();
            Stopwatch stopThread = new Stopwatch();
            stopBasic.Start();
            var outBasic = BasicIntSumm(generated);
            stopBasic.Stop();
            Console.WriteLine($"Результат: {outBasic}");
            Console.WriteLine($"При обычном цикле потребовалось {stopBasic.ElapsedMilliseconds}мс");

            stopParallel.Start();
            var outParalel = ParalelIntSumm(generated);
            stopParallel.Stop();
            Console.WriteLine($"Результат: {outParalel}");
            Console.WriteLine($"При параллельном вычислении потребовалось {stopParallel.ElapsedMilliseconds}мс");

            stopThread.Start();
            var outThread = ThreadIntSumm(generated);
            stopThread.Stop();
            Console.WriteLine($"Результат: {outThread}");
            Console.WriteLine($"При 4 потоках потребовалось {stopThread.ElapsedMilliseconds}мс");
            Console.ReadLine();
        }

        /// <summary>
        /// Генерирует указанное количество случайных чисел типа Int
        /// </summary>
        /// <param name="count">количество</param>
        /// <returns></returns>
        static int[] Generator(int count)
        {
            int[] result = new int[count];
            for (var i=0; i < count; i++)
            {
                result[i] = new Random().Next(int.MinValue, int.MaxValue);
            }
            return result;
        }

        /// <summary>
        /// Последовательное однопоточное суммирование
        /// </summary>
        /// <param name="matrix">массив Int</param>
        /// <returns></returns>
        static long BasicIntSumm(int[] matrix)
        {
            return matrix.Sum(p => (long)p);
        }

        /// <summary>
        /// Суммирование через PLINQ
        /// </summary>
        /// <param name="matrix">массив Int</param>
        /// <returns></returns>
        static long ParalelIntSumm(int[] matrix)
        {
            return matrix.AsParallel().WithExecutionMode(ParallelExecutionMode.ForceParallelism).Sum(p => (long)p);
        }

        /// <summary>
        /// Суммирование через создание четыреёх потоков
        /// </summary>
        /// <param name="matrix">массив Int</param>
        /// <returns></returns>
        static long ThreadIntSumm(int[] matrix)
        {
            long result = 0;
            var lenght = matrix.Length;
            int step = lenght / 4;
            int ost = lenght % 4;

            Stopwatch stop = new Stopwatch();
            List<Thread> listThread = new List<Thread>();                           
            for (var i = 0; i < 4; i++)
            {
                var z = i; long temp = 0;
                listThread.Add(new Thread(() =>
                {
                    var ostatok = 0;
                    if (z == 3) { ostatok = ost ; }  
                    for (var j = (z * step); j < (((z + 1) * step) + ost); j++)
                    {
                      temp = temp + matrix[j];
                    }
                    result = result + temp;
                }));
                listThread[i].Start();
            }
            for (var i = 0; i < 4; i++)
            {
                listThread[i].Join();
            }            
            return result;
        }
    }
}
