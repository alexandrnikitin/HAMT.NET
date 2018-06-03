using System;
using BenchmarkDotNet.Attributes;
using ImTools;
using ImmutableDictionaryBCL = System.Collections.Immutable.ImmutableDictionary<int, int>;

namespace HAMT.NET.Benchmarks
{
    [MemoryDiagnoser]
    public class IntsAddBenchmark
    {
        private const int N = 1002;
        private readonly int[] _numbers;
        private readonly Random _random = new Random(42);
        private readonly ImmutableDictionary<int, int> _sut;
        private readonly ImmutableDictionaryBCL _sutBCL;
        private readonly ImHashMap<int, int> _sutIm;

        public IntsAddBenchmark()
        {
            _numbers = new int[N];
            for (int i = 0; i < _numbers.Length; i++)
            {
                _numbers[i] = _random.Next();
            }

            _sut = ImmutableDictionary<int, int>.Empty;
            _sutBCL = ImmutableDictionaryBCL.Empty;
            _sutIm = ImHashMap<int, int>.Empty;
        }

        [Benchmark(OperationsPerInvoke = N)]
        public ImmutableDictionary<int, int> Add()
        {
            var sut = _sut;
            foreach (var n in _numbers)
            {
                sut = sut.Add(n, n);
            }

            return sut;
        }

        [Benchmark(OperationsPerInvoke = N)]
        public ImmutableDictionaryBCL AddBCL()
        {
            var sut = _sutBCL;
            foreach (var n in _numbers)
            {
                sut = sut.Add(n, n);
            }

            return sut;
        }

        [Benchmark(OperationsPerInvoke = N)]
        public ImHashMap<int, int> AddImTools()
        {
            var sut = _sutIm;
            foreach (var n in _numbers)
            {
                sut = sut.AddOrUpdate(n, n);
            }

            return sut;
        }
    }
}