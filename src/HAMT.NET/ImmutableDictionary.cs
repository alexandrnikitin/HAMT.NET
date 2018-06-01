using System;
using System.Runtime.Intrinsics.X86;

namespace HAMT.NET
{
    internal abstract class ImmutableDictionary<TKey, TValue> where TKey : IEquatable<TKey>
    {
        public ImmutableDictionary<TKey, TValue> Add(TKey key, TValue value)
        {
            throw new NotImplementedException();
        }

        public bool ContainsKey(TKey key)
        {
            return ContainsKey(key, (uint)key.GetHashCode(), 0);
        }

        internal abstract bool ContainsKey(TKey key, uint hash, int shift);
    }

    internal sealed class EmptyNode<TKey, TValue> : ImmutableDictionary<TKey, TValue> where TKey : IEquatable<TKey>
    {
        internal override bool ContainsKey(TKey key, uint hash, int shift)
        {
            return false;
        }
    }

    internal sealed class BitMapNode<TKey, TValue> : ImmutableDictionary<TKey, TValue> where TKey : IEquatable<TKey>
    {
        private readonly ulong _bitmap;
        private readonly ImmutableDictionary<TKey, TValue>[] _nodes;

        internal override bool ContainsKey(TKey key, uint hash, int shift)
        {
            const int mask = (1 << 6) - 1;
            var bit = 1U << (int)((hash >> shift) & mask);
            if ((_bitmap & bit) == 0) return false;
            var index = Popcnt.PopCount(_bitmap >> (int)bit);
            return _nodes[index].ContainsKey(key, hash, shift + 6);
        }
    }

    internal sealed class KeyValueNode<TKey, TValue> : ImmutableDictionary<TKey, TValue> where TKey : IEquatable<TKey>
    {
        private readonly TKey _key;
        private readonly uint _hash;
        private readonly TValue _value;

        public KeyValueNode(TKey key, uint hash, TValue value)
        {
            _key = key;
            _hash = hash;
            _value = value;
        }

        internal override bool ContainsKey(TKey key, uint hash, int shift)
        {
            return hash==_hash && key.Equals(_key);
        }
    }
}