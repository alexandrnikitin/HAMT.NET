using System;
using BenchmarkDotNet.Running;

namespace HAMT.NET.Benchmarks
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<IntsContainsBenchmark>();
        }
    }
}
