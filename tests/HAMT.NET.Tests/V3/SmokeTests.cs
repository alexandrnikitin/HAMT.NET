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
            Assert.True(sut.ContainsKey(1));
            Assert.True(sut.ContainsKey(2));
            Assert.True(sut.ContainsKey(3));
            Assert.True(sut.ContainsKey(4));
        }

        [Fact]
        public void Test3()
        {
            var sut = ImmutableDictionaryV3.Empty;
            var N = 1000000;
            for (var i = 0; i < N; i++)
            {
                sut = sut.Add(i,i);
            }

            for (var i = 0; i < N; i++)
            {
                Assert.True(sut.ContainsKey(i));
            }
        }
    }
}