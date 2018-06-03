using System;
using System.Runtime.Intrinsics.X86;

namespace HAMT.NET.V3
{
    public abstract class ImmutableDictionary<TKey, TValue> where TKey : IEquatable<TKey>
    {
        public static readonly ImmutableDictionary<TKey, TValue> Empty = new EmptyNode<TKey, TValue>();

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
            new BitMapNode<TKey, TValue, ValueNodes<TKey, TValue>>(0, null, 1U << (int)((hash >> shift) & Mask), new ValueNodes<TKey, TValue>(key, value));

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
                // TODO
                throw new NotImplementedException();
            }
            else
            {
                var index = (uint)Popcnt.PopCount((_bitmapValues >> (int)bit) & Mask);
                return _values.Add(key, value, _bitmapNodes, _nodes, _bitmapValues, index);
            }
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