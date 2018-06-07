using System;
using System.Collections.Generic;
using BenchmarkDotNet.Attributes;

namespace HAMT.NET.Benchmarks
{
    [MemoryDiagnoser]
    public class IntsContainsSingleCompareBenchmark
    {
        private readonly Dictionary<int, int> _sutBaseline;
        private readonly V2.ImmutableDictionary<int, int> _sutControl;
        private readonly V3.ImmutableDictionary<int, int> _sutExperiment;

        public IntsContainsSingleCompareBenchmark()
        {
            _sutBaseline = new Dictionary<int, int>();
            _sutControl = V2.ImmutableDictionary<int, int>.Empty;
            _sutExperiment = V3.ImmutableDictionary<int, int>.Empty;
            _sutBaseline.Add(1, 1);
            _sutControl = _sutControl.Add(1, 1);
            _sutExperiment = _sutExperiment.Add(1, 1);
        }

        [Benchmark(OperationsPerInvoke = 1000, Baseline = true)]
        public bool ContainsBaseline()
        {
            var consume = false;
            for (var i = 0; i < 1000; i++)
            {
                consume = _sutBaseline.ContainsKey(1);
            }

            return consume;
        }

        [Benchmark(OperationsPerInvoke = 1000)]
        public bool ContainsControl()
        {
            var consume = false;
            for (var i = 0; i < 1000; i++)
            {
                consume = _sutControl.ContainsKey(1);
            }

            return consume;
        }

        [Benchmark(OperationsPerInvoke = 1000)]
        public bool ContainsExperiment()
        {
            var consume = false;
            for (var i = 0; i < 1000; i++)
            {
                consume = _sutExperiment.ContainsKey(1);
            }

            return consume;
        }
    }
}