using Xunit;
using ImmutableDictionaryV3 = HAMT.NET.V3.ImmutableDictionary<int,int>;

namespace HAMT.NET.Tests.V3
{
    public class SmokeTests
    {
        [Fact]
        public void Test1()
        {
            var sut = ImmutableDictionaryV3.Empty;
            sut = sut.Add(1, 1);
            Assert.True(sut.ContainsKey(1));
        }
        [Fact]
        public void Test2()
        {
            var sut = ImmutableDictionaryV3.Empty;
            sut = sut.Add(1, 1);
            sut = sut.Add(1, 2);
            Assert.True(sut.ContainsKey(1));
        }
        [Fact]
        public void Test20()
        {
            var sut = ImmutableDictionaryV3.Empty;
            sut = sut.Add(0, 0);
            sut = sut.Add(1, 1);
            sut = sut.Add(2, 2);
            sut = sut.Add(3, 3);
            sut = sut.Add(4, 4);
            Assert.True(sut.ContainsKey(0));
        }

        [Fact]
        public void Test3()
        {
            var sut = ImmutableDictionaryV3.Empty;
            for (var i = 0; i < 10; i++)
            {
                sut = sut.Add(i,i);
            }

            for (var i = 0; i < 10; i++)
            {
                Assert.True(sut.ContainsKey(i), $"No {i}");
            }
        }

        [Fact]
        public void Test4()
        {
         const int Shift = 3;
        const int Mask = (1 << Shift) - 1;
            var hash = 0;
            var bit = 1U << (int)((hash >> Shift) & Mask);
        }
    }
}