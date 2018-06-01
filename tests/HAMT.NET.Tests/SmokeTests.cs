using Xunit;

namespace HAMT.NET.Tests
{
    public class SmokeTests
    {
        [Fact]
        public void Test1()
        {
            var sut = ImmutableDictionary.Empty<int, int>();
            sut = sut.Add(1, 1);
            Assert.True(sut.ContainsKey(1));
        }
        [Fact]
        public void Test2()
        {
            var sut = ImmutableDictionary.Empty<int, int>();
            sut = sut.Add(1, 1);
            sut = sut.Add(1, 2);
            Assert.True(sut.ContainsKey(1));
        }

        [Fact]
        public void Test3()
        {
            var sut = ImmutableDictionary.Empty<int, int>();
            for (var i = 0; i < 1000; i++)
            {
                sut = sut.Add(i,i);
            }
            
        }
    }
}