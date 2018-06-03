using System;
using BenchmarkDotNet.Attributes;

namespace HAMT.NET.Benchmarks
{
    [MemoryDiagnoser]
    public class IntsContainsCompareBenchmark
    {
        private const int N = 1002;
        private readonly int[] _numbers;
        private readonly Random _random = new Random(42);
        private readonly ImmutableDictionary<int, int> _sut;
        private readonly V2.ImmutableDictionary<int, int> _sutV2;

        public IntsContainsCompareBenchmark()
        {
            _sut = ImmutableDictionary<int, int>.Empty;
            _sutV2 = V2.ImmutableDictionary<int, int>.Empty;

            _numbers = new int[N];
            for (var i = 0; i < _numbers.Length; i++)
            {
                var next = _random.Next();
                _numbers[i] = next;
                _sut = _sut.Add(next, next);
                _sutV2 = _sutV2.Add(next, next);
            }
        }

        [Benchmark(OperationsPerInvoke = N)]
        public bool Contains()
        {
            var consume = false;
            foreach (var n in _numbers)
            {
                consume = _sut.ContainsKey(n);
            }

            return consume;
        }

        [Benchmark(OperationsPerInvoke = N)]
        public bool ContainsV2()
        {
            var consume = false;
            foreach (var n in _numbers)
            {
                consume = _sutV2.ContainsKey(n);
            }

            return consume;
        }
    }
}