using BenchmarkDotNet.Attributes;
using HAMT.NET.V3;

namespace HAMT.NET.Benchmarks
{
    public class ValueNode2Benchmarks
    {
        private const int N = 1002;
        private readonly ValueNode2<int, int> _sut = new ValueNode2<int, int>(1, 2, 3, 4);

        [Benchmark(OperationsPerInvoke = N * 2)]
        public bool ContainsKey()
        {
            var sut = _sut;
            var ret = false;
            for (int i = 0; i < N; i++)
            {
                ret = sut.ContainsKey(1, (uint) 1.GetHashCode(), 0);
                ret = sut.ContainsKey(3, (uint) 3.GetHashCode(), 1);
            }

            return ret;
        }
        [Benchmark(OperationsPerInvoke = N * 2, Baseline = true)]
        public bool ContainsKeyBaseline()
        {
            var sut = _sut;
            var ret = false;
            for (int i = 0; i < N; i++)
            {
                ret = sut.ContainsKeyBaseline(1, (uint) 1.GetHashCode(), 0);
                ret = sut.ContainsKeyBaseline(3, (uint) 3.GetHashCode(), 1);
            }

            return ret;
        }

    }
}