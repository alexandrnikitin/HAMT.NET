using HAMT.NET.V3;
using Xunit;

namespace HAMT.NET.Tests.V3
{
    public class ValueNodeTests
    {
        [Fact]
        public void Test1()
        {
            var sut = new ValueNode1<int, int>(1, 2);
            Assert.True(sut.ContainsKey(1, (uint)1.GetHashCode(), 0));
            var sut2 = new ValueNode2<int, int>(1, 2, 3, 4);
            Assert.True(sut2.ContainsKey(1, (uint)1.GetHashCode(), 0));
            Assert.True(sut2.ContainsKey(3, (uint)3.GetHashCode(), 1));
        }

        [Fact]
        public void TestAdd()
        {
            var sut = new ValueNode1<int, int>(1, 2);
            var actual = sut.Add(3, 4, 0, new NET.V3.ImmutableDictionary<int, int>[0], 0, 1);
        }
    }
}