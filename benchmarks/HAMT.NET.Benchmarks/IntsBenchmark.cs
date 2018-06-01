using System;
using BenchmarkDotNet.Attributes;

namespace HAMT.NET.Benchmarks
{
    public class IntsBenchmark
    {
        private const int N = 1002;
        private readonly int[] numbers;
        private readonly Random _random = new Random(42);
        private readonly ImmutableDictionary<int, int> _sut;

        public IntsBenchmark()
        {
            numbers = new int[N];
            for (int i = 0; i < N; i++)
                numbers[i] = _random.Next();

            _sut = ImmutableDictionary.Empty<int, int>();
        }

        [Benchmark]
        public ImmutableDictionary<int, int> Add()
        {
            var sut = _sut;
            for (int i = 0; i < N; i++)
                sut = sut.Add(i, i);

            return sut;
        }
    }
}