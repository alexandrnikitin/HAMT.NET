using System;
using System.Runtime.Intrinsics.X86;

namespace HAMT.NET.V4
{
    public static class ImmutableDictionary
    {
        public const int Shift = 3;
        public const int Mask = (1 << Shift) - 1;
    }

    public abstract class ImmutableDictionary<TKey, TValue> where TKey : IEquatable<TKey>
    {
        public static readonly ImmutableDictionary<TKey, TValue> Empty = new EmptyNode<TKey, TValue>();

        public const int Shift = 3;
        public const int Mask = (1 << Shift) - 1;

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
            new BitMapNode<TKey, TValue, ValueNode1<TKey, TValue>>(0, new ImmutableDictionary<TKey, TValue>[0], 1U << (int)((hash >> shift) & Mask), new ValueNode1<TKey, TValue>(key, value));

        internal override bool ContainsKey(TKey key, uint hash, int shift) => false;
    }

//    [StructLayout(LayoutKind.Sequential)]
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
                var index = Popcnt.PopCount(_bitmapNodes & (bit - 1));
                newNodes[index] = _nodes[index].Add(key, value, hash, shift + Shift);
                return new BitMapNode<TKey, TValue, TValues>(_bitmapNodes, newNodes, _bitmapValues, _values);
            }
            else if ((_bitmapValues & bit) != 0)
            {
                // TODO collisions and same value
                var newNodes = new ImmutableDictionary<TKey, TValue>[_nodes.Length + 1];
                var index = Popcnt.PopCount(_bitmapNodes & (bit - 1));
                Array.Copy(_nodes, newNodes, index);
                Array.Copy(_nodes, index, newNodes, index + 1, _nodes.Length - index);

                var indexValues = Popcnt.PopCount(_bitmapValues & (bit - 1));
                var key2 = _values.GetKey(indexValues);
                var value2 = _values.GetValue(indexValues);
                newNodes[index] = BitMapNode<TKey, TValue, TValues>.From(key, value, hash, shift + Shift, key2, value2);
                return _values.Shrink(_bitmapNodes | bit, newNodes, _bitmapValues ^ bit, (uint) indexValues);
            }
            else
            {
                var index = (uint)Popcnt.PopCount(_bitmapValues & (bit - 1));
                return _values.Add(key, value, _bitmapNodes, _nodes, _bitmapValues | bit, index);
            }
        }

        private static ImmutableDictionary<TKey, TValue> From(TKey key, TValue value, uint hash, int shift, TKey key2, TValue value2)
        {
            if (hash == (uint)key2.GetHashCode() && key.Equals(key2))
            {
                // TODO lift hash and key equlity (update) to above layer
                var newValues = new ValueNode1<TKey, TValue>(key, value);
                return new BitMapNode<TKey, TValue, ValueNode1<TKey, TValue>>(0, new ImmutableDictionary<TKey, TValue>[0], 1U << (int)((hash >> shift) & Mask), newValues);
            }
            else if (hash == (uint)key2.GetHashCode())
            {
                // TODO handle collisions, at what shift level?
                throw new NotImplementedException();
            }
            else
            {
                var bit1 = 1U << (int)(((uint)key2.GetHashCode() >> shift) & Mask);
                var bit2 = 1U << (int)((hash >> shift) & Mask);
                if (bit1 < bit2)
                {
                    var newValues = new ValueNode2<TKey, TValue>(key2, value2, key, value);
                    return new BitMapNode<TKey, TValue, ValueNode2<TKey, TValue>>(0, new ImmutableDictionary<TKey, TValue>[0], bit1 | bit2, newValues);
                }
                else if (bit1 > bit2)
                {
                    var newValues = new ValueNode2<TKey, TValue>(key, value, key2, value2);
                    return new BitMapNode<TKey, TValue, ValueNode2<TKey, TValue>>(0, new ImmutableDictionary<TKey, TValue>[0], bit1 | bit2, newValues);
                }
                else
                {
                    var newNodes = BitMapNode<TKey, TValue, TValues>.From(key, value, hash, shift + Shift, key2, value2);
                    return new BitMapNode<TKey, TValue, ValueNode0<TKey, TValue>>(bit1, new []{ newNodes }, 0, new ValueNode0<TKey,TValue>());
                }
            }
        }

        internal override bool ContainsKey(TKey key, uint hash, int shift)
        {
            var bit = 1U << (int) ((hash >> shift) & Mask);
            if ((_bitmapNodes & bit) != 0)
            {
                var index = Popcnt.PopCount(_bitmapNodes & (bit - 1));
                return _nodes[index].ContainsKey(key, hash, shift + Shift);
            }
            else if ((_bitmapValues & bit) != 0)
            {
                var index = Popcnt.PopCount(_bitmapValues & (bit - 1));
                return _values.ContainsKey(key, hash, index);
            }
            else
            {
                return false;
            }
        }
    }
}