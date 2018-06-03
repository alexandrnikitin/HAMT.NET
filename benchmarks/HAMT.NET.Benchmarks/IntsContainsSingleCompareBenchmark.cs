using System;
using BenchmarkDotNet.Attributes;

namespace HAMT.NET.Benchmarks
{
    [MemoryDiagnoser]
    public class IntsContainsSingleCompareBenchmark
    {
        private readonly V3.ImmutableDictionary<int, int> _sutExperiment;
        private readonly V2.ImmutableDictionary<int, int> _sutControl;

        public IntsContainsSingleCompareBenchmark()
        {
            _sutControl = V2.ImmutableDictionary<int, int>.Empty;
            _sutExperiment = V3.ImmutableDictionary<int, int>.Empty;
            _sutControl= _sutControl.Add(1, 1);
            _sutExperiment = _sutExperiment.Add(1, 1);
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
    }
}