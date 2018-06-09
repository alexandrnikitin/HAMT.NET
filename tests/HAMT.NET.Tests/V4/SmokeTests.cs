using System;
using Xunit;
using ImmutableDictionaryV4 = HAMT.NET.V4.ImmutableDictionary<int,int>;

namespace HAMT.NET.Tests.V4
{
    public class SmokeTests
    {
        [Fact]
        public void Test1()
        {
            var sut = ImmutableDictionaryV4.Empty;
            sut = sut.Add(1, 1);
            Assert.True(sut.ContainsKey(1));
        }
        [Fact]
        public void Test2()
        {
            var sut = ImmutableDictionaryV4.Empty;
            sut = sut.Add(1, 1);
            sut = sut.Add(1, 2);
            Assert.True(sut.ContainsKey(1));
        }

        [Fact]
        public void Test20()
        {
            var sut = ImmutableDictionaryV4.Empty;
            sut = sut.Add(0, 0);
            sut = sut.Add(1, 1);
            sut = sut.Add(2, 2);
            sut = sut.Add(3, 3);
            sut = sut.Add(4, 4);
            Assert.True(sut.ContainsKey(0));
            Assert.True(sut.ContainsKey(1));
            Assert.True(sut.ContainsKey(2));
            Assert.True(sut.ContainsKey(3));
            Assert.True(sut.ContainsKey(4));
        }

        [Fact]
        public void Test3()
        {
            var sut = ImmutableDictionaryV4.Empty;
            var N = 1000000;
            for (var i = 0; i < N; i++)
            {
                sut = sut.Add(i, i);
            }

            for (var i = 0; i < N; i++)
            {
                Assert.True(sut.ContainsKey(i), $"{i}");
            }
        }

        [Fact]
        public void Test4()
        {
            const int N = 1002;
            var random = new Random(42);
            var sut = ImmutableDictionaryV4.Empty;

            var numbers = new int[N];
            for (var i = 0; i < numbers.Length; i++)
            {
                var next = random.Next();
                numbers[i] = next;
                sut = sut.Add(next, next);
            }

            foreach (var n in numbers)
            {
                Assert.True(sut.ContainsKey(n));
            }
        }
    }
}