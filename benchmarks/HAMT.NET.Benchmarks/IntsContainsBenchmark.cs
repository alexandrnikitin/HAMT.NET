using System;
using BenchmarkDotNet.Attributes;
using ImTools;
using ImmutableDictionaryBCL = System.Collections.Immutable.ImmutableDictionary<int, int>;

namespace HAMT.NET.Benchmarks
{
    [MemoryDiagnoser]
    public class IntsContainsBenchmark
    {
        private const int N = 1002;
        private readonly int[] _numbers;
        private readonly Random _random = new Random(42);
        private readonly ImmutableDictionary<int, int> _sut;
        private readonly ImmutableDictionaryBCL _sutBCL;
        private readonly ImHashMap<int, int> _sutIm;

        public IntsContainsBenchmark()
        {
            _sut = ImmutableDictionary.Empty<int, int>();
            _sutBCL = ImmutableDictionaryBCL.Empty;
            _sutIm = ImHashMap<int, int>.Empty;

            _numbers = new int[N];
            for (var i = 0; i < _numbers.Length; i++)
            {
                var next = _random.Next();
                _numbers[i] = next;
                _sut = _sut.Add(next, next);
                _sutBCL = _sutBCL.Add(next, next);
                _sutIm = _sutIm.AddOrUpdate(next, next);
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
        public bool ContainsBCL()
        {
            var consume = false;
            foreach (var n in _numbers)
            {
                consume = _sutBCL.ContainsKey(n);
            }

            return consume;
        }

        [Benchmark(OperationsPerInvoke = N)]
        public bool ContainsImTools()
        {
            var consume = false;
            foreach (var n in _numbers)
            {
                consume = _sutIm.TryFind(n, out _);
            }

            return consume;
        }
    }
}