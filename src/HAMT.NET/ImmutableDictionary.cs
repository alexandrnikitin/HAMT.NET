using System;
using System.Runtime.Intrinsics.X86;

namespace HAMT.NET
{
    public static class ImmutableDictionary
    {
        public static ImmutableDictionary<TKey, TValue> Empty<TKey, TValue>() where TKey : IEquatable<TKey>
        {
            return new EmptyNode<TKey, TValue>();
        }
    }

    public abstract class ImmutableDictionary<TKey, TValue> where TKey : IEquatable<TKey>
    {
        protected const int Shift = 6;
        protected const int Mask = (1 << Shift) - 1;

        public ImmutableDictionary<TKey, TValue> Add(TKey key, TValue value)
        {
            return Add(key, value, (uint) key.GetHashCode(), 0);
        }

        public bool ContainsKey(TKey key)
        {
            return ContainsKey(key, (uint) key.GetHashCode(), 0);
        }

        internal abstract ImmutableDictionary<TKey, TValue> Add(TKey key, TValue value, uint hash, int shift);
        internal abstract bool ContainsKey(TKey key, uint hash, int shift);
    }

    internal sealed class EmptyNode<TKey, TValue> : ImmutableDictionary<TKey, TValue> where TKey : IEquatable<TKey>
    {
        internal override ImmutableDictionary<TKey, TValue> Add(TKey key, TValue value, uint hash, int shift) =>
            new KeyValueNode<TKey, TValue>(key, value, hash);

        internal override bool ContainsKey(TKey key, uint hash, int shift) => false;
    }

    internal sealed class BitMapNode<TKey, TValue> : ImmutableDictionary<TKey, TValue> where TKey : IEquatable<TKey>
    {
        private readonly ulong _bitmap;
        private readonly ImmutableDictionary<TKey, TValue>[] _nodes;

        public BitMapNode(ulong bitmap, ImmutableDictionary<TKey, TValue>[] nodes)
        {
            _bitmap = bitmap;
            _nodes = nodes;
        }

        internal override ImmutableDictionary<TKey, TValue> Add(TKey key, TValue value, uint hash, int shift)
        {
            var bit = 1U << (int) ((hash >> shift) & Mask);
            if ((_bitmap & bit) != 0)
            {
                var newNodes = new ImmutableDictionary<TKey, TValue>[_nodes.Length];
                Array.Copy(_nodes, newNodes, _nodes.Length);
                var index = Popcnt.PopCount((_bitmap >> (int) bit) & Mask);
                newNodes[index] = _nodes[index].Add(key, value, hash, shift + Shift);
                return new BitMapNode<TKey, TValue>(_bitmap, newNodes);
            }
            else
            {
                var index = Popcnt.PopCount((_bitmap >> (int)bit) & Mask);
                var newNodes = new ImmutableDictionary<TKey, TValue>[_nodes.Length + 1];
                Array.Copy(_nodes, newNodes, index);
                Array.Copy(_nodes, index, newNodes, index + 1, _nodes.Length - index);
                newNodes[index] = new KeyValueNode<TKey, TValue>(key, value, hash);
                return new BitMapNode<TKey, TValue>(_bitmap | bit, newNodes);
            }
        }

        internal override bool ContainsKey(TKey key, uint hash, int shift)
        {
            var bit = 1U << (int) ((hash >> shift) & Mask);
            if ((_bitmap & bit) == 0) return false;
            var index = Popcnt.PopCount((_bitmap >> (int)bit) & Mask);
            return _nodes[index].ContainsKey(key, hash, shift + Shift);
        }
    }

    internal sealed class KeyValueNode<TKey, TValue> : ImmutableDictionary<TKey, TValue> where TKey : IEquatable<TKey>
    {
        private readonly TKey _key;
        private readonly TValue _value;
        private readonly uint _hash;

        public KeyValueNode(TKey key, TValue value, uint hash)
        {
            _key = key;
            _value = value;
            _hash = hash;
        }

        internal override ImmutableDictionary<TKey, TValue> Add(TKey key, TValue value, uint hash, int shift)
        {
            if (hash == _hash && key.Equals(_key))
            {
                // TODO duplicates
                return new KeyValueNode<TKey, TValue>(key, value, hash);
            }
            else if (hash == _hash)
            {
                // TODO handle collisions, at what shift level?
                throw new NotImplementedException();
            }
            else
            {
                var bit1 = 1U << (int) ((_hash >> shift) & Mask);
                var bit2 = 1U << (int) ((hash >> shift) & Mask);
                if (bit1 < bit2)
                {
                    return new BitMapNode<TKey, TValue>(bit1 | bit2,
                        new[] {this, new KeyValueNode<TKey, TValue>(key, value, hash)});
                }
                else if (bit1 > bit2)
                {
                    return new BitMapNode<TKey, TValue>(bit1 | bit2,
                        new[] {new KeyValueNode<TKey, TValue>(key, value, hash), this});
                }
                else
                {
                    return new BitMapNode<TKey, TValue>(bit1,
                        new[] {Add(key, value, hash, shift + Shift)});
                }
            }
        }

        internal override bool ContainsKey(TKey key, uint hash, int shift)
        {
            return hash == _hash && key.Equals(_key);
        }
    }
}