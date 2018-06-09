using HAMT.NET.V4;
using Xunit;

namespace HAMT.NET.Tests.V4
{
    public class BranchNodesTests
    {
        [Fact]
        public void Test1()
        {
            var sut = new BranchNodes0<int, int>();
            Assert.True(sut.Add(1, 1, (uint) 1.GetHashCode(), 0, 0, 0, 10, 0, new ValueNode1<int, int>(0,0)).ContainsKey(1));
            var sut2 = new BranchNodes0<int, int>();
            var sut3 = sut2.DuplicateWith(2, 2, (uint) 2.GetHashCode(), 0, 0, 1, 0, new ValueNode0<int, int>());
            Assert.True(sut3.ContainsKey(2));

        }
    }
}