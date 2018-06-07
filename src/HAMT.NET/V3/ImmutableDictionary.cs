using System;
using System.Runtime.Intrinsics.X86;

namespace HAMT.NET.V3
{
    public abstract class ImmutableDictionary<TKey, TValue> where TKey : IEquatable<TKey>
    {
        public static readonly ImmutableDictionary<TKey, TValue> Empty = new EmptyNode<TKey, TValue>();

        protected const int Shift = 3;
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
            new BitMapNode<TKey, TValue, ValueNode1<TKey, TValue>>(0, null, 1U << (int)((hash >> shift) & Mask), new ValueNode1<TKey, TValue>(key, value));

        internal override bool ContainsKey(TKey key, uint hash, int shift) => false;
    }

    internal sealed class BitMapNode<TKey, TValue, TValues> : ImmutableDictionary<TKey, TValue> 
        where TKey : IEquatable<TKey> where TValues: struct, IValueNodes<TKey, TValue>
    {
        private readonly ulong _bitmapNodes;
        private readonly ulong _bitmapValues;
        private readonly ImmutableDictionary<TKey, TValue>[] _nodes;
        private readonly TValues _values;

        public BitMapNode(ulong bitmapNodes, ImmutableDictionary<TKey, TValue>[] nodes, ulong bitmapValues, TValues values)
        {
            _bitmapNodes = bitmapNodes;
            _nodes = nodes;
            _bitmapValues = bitmapValues;
            _values = values;
        }

        internal override ImmutableDictionary<TKey, TValue> Add(TKey key, TValue value, uint hash, int shift)
        {
            var bit = 1U << (int) ((hash >> shift) & Mask);
            if ((_bitmapNodes & bit) != 0)
            {
                var newNodes = new ImmutableDictionary<TKey, TValue>[_nodes.Length];
                Array.Copy(_nodes, newNodes, _nodes.Length);
                var index = Popcnt.PopCount((_bitmapNodes >> (int) bit) & Mask);
                newNodes[index] = _nodes[index].Add(key, value, hash, shift + Shift);
                return new BitMapNode<TKey, TValue, TValues>(_bitmapNodes, newNodes, _bitmapValues, _values);
            }
            else if ((_bitmapValues & bit) != 0)
            {
                // TODO collisions and same value
                var newNodes = new ImmutableDictionary<TKey, TValue>[_nodes.Length + 1];
                var index = Popcnt.PopCount((_bitmapNodes >> (int)bit) & Mask);
                Array.Copy(_nodes, newNodes, index);
                Array.Copy(_nodes, index, newNodes, index + 1, _nodes.Length - index);
                newNodes[index] = AddTwo(key, value, hash, shift + 1, bit);
                // TODO clear lifted values to default
                return new BitMapNode<TKey, TValue, TValues>(_bitmapNodes ^ bit, newNodes, _bitmapValues | bit, _values);
            }
            else
            {
                var index = (uint)Popcnt.PopCount((_bitmapValues >> (int)bit) & Mask);
                return _values.Add(key, value, _bitmapNodes, _nodes, _bitmapValues, index);
            }
        }

        private ImmutableDictionary<TKey, TValue> AddTwo(TKey key, TValue value, uint hash, int shift, uint bit)
        {
            throw new NotImplementedException();
        }

        internal override bool ContainsKey(TKey key, uint hash, int shift)
        {
            var bit = 1U << (int) ((hash >> shift) & Mask);
            if ((_bitmapNodes & bit) != 0)
            {
                var index = Popcnt.PopCount((_bitmapNodes >> (int)bit) & Mask);
                return _nodes[index].ContainsKey(key, hash, shift + Shift);
            }
            else if ((_bitmapValues & bit) != 0)
            {
                var index = Popcnt.PopCount((_bitmapValues >> (int)bit) & Mask);
                return _values.ContainsKey(key, hash, index);
            }
            else
            {
                return false;
            }
        }
    }
}