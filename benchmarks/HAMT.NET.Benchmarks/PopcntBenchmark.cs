using System;
using System.Runtime.Intrinsics.X86;
using BenchmarkDotNet.Attributes;

namespace HAMT.NET.Benchmarks
{
    public class PopcntBenchmark
    {
        private const int N = 1002;
        private readonly ulong[] numbers;
        private readonly Random random = new Random(42);

        public ulong NextUInt64()
        {
            var buffer = new byte[sizeof(long)];
            random.NextBytes(buffer);
            return BitConverter.ToUInt64(buffer, 0);
        }

        public PopcntBenchmark()
        {
            numbers = new ulong[N];
            for (int i = 0; i < N; i++)
                numbers[i] = NextUInt64();
        }

        [Benchmark]
        public int PopCount1()
        {
            int counter = 0;
            for (int i = 0; i < N; i++)
                counter += BitCountHelper.PopCount1(numbers[i]);
            return counter;
        }

        [Benchmark]
        public int PopCountIntrinsic()
        {
            long longResult = 0;
            var data = numbers;
            for (int i = 0; i < N; i++)
            {
                longResult += Popcnt.PopCount(data[i]);
            }

            return (int) longResult;
        }

        internal static class BitCountHelper
        {
            const ulong m1 = 0x5555555555555555;
            const ulong m2 = 0x3333333333333333;
            const ulong m4 = 0x0f0f0f0f0f0f0f0f;
            const ulong m8 = 0x00ff00ff00ff00ff;
            const ulong m16 = 0x0000ffff0000ffff;
            const ulong m32 = 0x00000000ffffffff;
            const ulong hff = 0xffffffffffffffff;
            const ulong h01 = 0x0101010101010101;

            //This is a naive implementation, shown for comparison,
            //and to help in understanding the better functions.
            //It uses 24 arithmetic operations (shift, add, and).
            public static int PopCount1(ulong x)
            {
                x = (x & m1) + ((x >> 1) & m1); //put count of each  2 bits into those  2 bits 
                x = (x & m2) + ((x >> 2) & m2); //put count of each  4 bits into those  4 bits 
                x = (x & m4) + ((x >> 4) & m4); //put count of each  8 bits into those  8 bits 
                x = (x & m8) + ((x >> 8) & m8); //put count of each 16 bits into those 16 bits 
                x = (x & m16) + ((x >> 16) & m16); //put count of each 32 bits into those 32 bits 
                x = (x & m32) + ((x >> 32) & m32); //put count of each 64 bits into those 64 bits 
                return (int) x;
            }
        }
    }
}