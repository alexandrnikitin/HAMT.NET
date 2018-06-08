using BenchmarkDotNet.Attributes;
using HAMT.NET.V3;

namespace HAMT.NET.Benchmarks
{
    public class ValueNode2AddBenchmarks
    {
        private const int N = 1002;
        private readonly ValueNode2<int, int> _sut = new ValueNode2<int, int>(1, 2, 3, 4);

        [Benchmark(OperationsPerInvoke = N * 2)]
        public V3.ImmutableDictionary<int, int> Add()
        {
            var sut = _sut;
            V3.ImmutableDictionary<int, int> ret = null;
            for (int i = 0; i < N; i++)
            {
                ret = sut.Add(5, 6, 0, new V3.ImmutableDictionary<int, int>[0], 0, 1);
            }

            return ret;
        }

        [Benchmark(OperationsPerInvoke = N * 2, Baseline = true)]
        public V3.ImmutableDictionary<int, int> AddBaseline()
        {
            var sut = _sut;
            V3.ImmutableDictionary<int, int> ret = null;
            for (int i = 0; i < N; i++)
            {
                ret = sut.AddBaseline(5, 6, 0, new V3.ImmutableDictionary<int, int>[0], 0, 1);
            }

            return ret;
        }

    }
}