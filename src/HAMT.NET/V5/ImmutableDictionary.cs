using System;
using System.Runtime.Intrinsics.X86;

namespace HAMT.NET.V5
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
            new BitMapNode<TKey, TValue, ValueNode1<TKey, TValue>, BranchNodes0<TKey, TValue>>(
                0, new BranchNodes0<TKey, TValue>(), 1U << (int)((hash >> shift) & Mask), new ValueNode1<TKey, TValue>(key, value));

        internal override bool ContainsKey(TKey key, uint hash, int shift) => false;
    }

//    [StructLayout(LayoutKind.Sequential)]
    internal sealed class BitMapNode<TKey, TValue, TValueNodes, TBranchNodes> : ImmutableDictionary<TKey, TValue> 
        where TKey : IEquatable<TKey> 
        where TValueNodes: struct, IValueNodes<TKey, TValue>
        where TBranchNodes : struct, IBranchNodes<TKey, TValue>
    {
        private readonly ulong _bitmapNodes;
        private readonly ulong _bitmapValues;
        private readonly TBranchNodes _nodes;
        private readonly TValueNodes _values;

        public BitMapNode(ulong bitmapNodes, TBranchNodes nodes, ulong bitmapValues, TValueNodes values)
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
                var index = Popcnt.PopCount(_bitmapNodes & (bit - 1));
                return _nodes.DuplicateWith(key, value, hash, index, shift, _bitmapNodes, _bitmapValues, _values);
            }
            else if ((_bitmapValues & bit) != 0)
            {
                // TODO collisions and same value
                var index = Popcnt.PopCount(_bitmapNodes & (bit - 1));
                var indexValues = Popcnt.PopCount(_bitmapValues & (bit - 1));
                return _nodes.Add(key, value, hash, (uint) index, (uint) indexValues, shift, _bitmapNodes | bit, _bitmapValues ^ bit, _values);
            }
            else
            {
                var index = (uint)Popcnt.PopCount(_bitmapValues & (bit - 1));
                return _values.Add(key, value, _bitmapNodes, _nodes, _bitmapValues | bit, index);
            }
        }

        internal static ImmutableDictionary<TKey, TValue> From(TKey key, TValue value, uint hash, int shift, TKey key2, TValue value2)
        {
            if (hash == (uint)key2.GetHashCode() && key.Equals(key2))
            {
                // TODO lift hash and key equlity (update) to above layer
                var newValues = new ValueNode1<TKey, TValue>(key, value);
                return new BitMapNode<TKey, TValue, ValueNode1<TKey, TValue>, BranchNodes0<TKey, TValue>>(
                    0, new BranchNodes0<TKey, TValue>(), 1U << (int)((hash >> shift) & Mask), newValues);
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
                    return new BitMapNode<TKey, TValue, ValueNode2<TKey, TValue>, BranchNodes0<TKey, TValue>>(
                        0, new BranchNodes0<TKey, TValue>(), bit1 | bit2, newValues);
                }
                else if (bit1 > bit2)
                {
                    var newValues = new ValueNode2<TKey, TValue>(key, value, key2, value2);
                    return new BitMapNode<TKey, TValue, ValueNode2<TKey, TValue>, BranchNodes0<TKey,TValue>>(
                        0, new BranchNodes0<TKey, TValue>(), bit1 | bit2, newValues);
                }
                else
                {
                    var newNodes = BitMapNode<TKey, TValue, TValueNodes, TBranchNodes>.From(key, value, hash, shift + Shift, key2, value2);
                    return new BitMapNode<TKey, TValue, ValueNode0<TKey, TValue>, BranchNodes1<TKey, TValue>>(bit1, new BranchNodes1<TKey, TValue>(newNodes), 0, new ValueNode0<TKey,TValue>());
                }
            }
        }

        internal override bool ContainsKey(TKey key, uint hash, int shift)
        {
            var bit = 1U << (int) ((hash >> shift) & Mask);
            if ((_bitmapNodes & bit) != 0)
            {
                var index = Popcnt.PopCount(_bitmapNodes & (bit - 1));
                return _nodes.ContainsKey(key, hash, (int) index, shift + Shift);
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