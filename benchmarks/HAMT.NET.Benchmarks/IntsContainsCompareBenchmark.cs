using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;

namespace HAMT.NET.Benchmarks
{
    [MemoryDiagnoser]
    public class IntsContainsCompareBenchmark
    {
        private const int N = 1002;
        private readonly int[] _numbers;
        private readonly Random _random = new Random(42);
        private readonly Dictionary<int, int> _sutBaseline;
        private readonly V2.ImmutableDictionary<int, int> _sutControl;
        private readonly V3.ImmutableDictionary<int, int> _sutExperiment;

        public IntsContainsCompareBenchmark()
        {
            _sutBaseline = new Dictionary<int, int>();
            _sutControl = V2.ImmutableDictionary<int, int>.Empty;
            _sutExperiment = V3.ImmutableDictionary<int, int>.Empty;

            _numbers = new int[N];
            for (var i = 0; i < _numbers.Length; i++)
            {
                var next = _random.Next();
                _numbers[i] = next;
                _sutBaseline.Add(next, next);
                _sutControl = _sutControl.Add(next, next);
                _sutExperiment = _sutExperiment.Add(next, next);
            }
        }

        [Benchmark(OperationsPerInvoke = N, Baseline = true)]
        public bool ContainsBaseline()
        {
            var consume = false;
            foreach (var n in _numbers)
            {
                consume = _sutBaseline.ContainsKey(n);
            }

            return consume;
        }

        [Benchmark(OperationsPerInvoke = N)]
        public bool ContainsControl()
        {
            var consume = false;
            foreach (var n in _numbers)
            {
                consume = _sutControl.ContainsKey(n);
            }

            return consume;
        }
        [Benchmark(OperationsPerInvoke = N)]
        public bool ContainsExperiment()
        {
            var consume = false;
            foreach (var n in _numbers)
            {
                consume = _sutExperiment.ContainsKey(n);
            }

            return consume;
        }
    }
}