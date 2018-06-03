using System;
using System.Diagnostics.Contracts;

namespace HAMT.NET.V3
{
    public struct ValueNodes<TKey, TValue> : IValueNodes<TKey, TValue> where TKey : IEquatable<TKey>
    {
        public ValueNodes(TKey key, TValue value)
        {
            _key = key;
            _value = value;
        }

        private readonly TKey _key;
        private readonly TValue _value;

        public bool ContainsKey(TKey key, uint hash, long index)
        {
            return hash == (uint) _key.GetHashCode() && key.Equals(_key);
        }

        public ImmutableDictionary<TKey, TValue> Add(TKey key, TValue value, ulong bitmapNodes,
            ImmutableDictionary<TKey, TValue>[] nodes, ulong bitmapValues, uint index)
        {
            if (index == 0)
            {
                var values = new ValueNode2<TKey, TValue>(key, value, _key, _value);
                return new BitMapNode<TKey, TValue, ValueNode2<TKey, TValue>>(bitmapNodes, nodes, bitmapValues | index,
                    values);
            }
            else
            {
                var values = new ValueNode2<TKey, TValue>(_key, _value, key, value);
                return new BitMapNode<TKey, TValue, ValueNode2<TKey, TValue>>(bitmapNodes, nodes, bitmapValues | index,
                    values);
            }
        }
    }

    public struct ValueNode2<TKey, TValue> : IValueNodes<TKey, TValue> where TKey : IEquatable<TKey>
    {
        private readonly TKey _key1;
        private readonly TValue _value1;
        private readonly TKey _key2;
        private readonly TValue _value2;

        public ValueNode2(TKey key1, TValue value1, TKey key2, TValue value2)
        {
            _key1 = key1;
            _value1 = value1;
            _key2 = key2;
            _value2 = value2;
        }


        public bool ContainsKey(TKey key, uint hash, long index)
        {
            if (index == 0)
            {
                return hash == (uint) _key1.GetHashCode() && key.Equals(_key1);
            }

            return hash == (uint) _key2.GetHashCode() && key.Equals(_key2);
        }

        public ImmutableDictionary<TKey, TValue> Add(TKey key, TValue value, ulong bitmapNodes,
            ImmutableDictionary<TKey, TValue>[] nodes, ulong bitmapValues, uint index)
        {
            throw new NotImplementedException();
        }
    }

    public interface IValueNodes<TKey, TValue> where TKey : IEquatable<TKey>
    {
        [Pure]
        bool ContainsKey(TKey key, uint hash, long index);

        [Pure]
        ImmutableDictionary<TKey, TValue> Add(TKey key, TValue value, ulong bitmapNodes,
            ImmutableDictionary<TKey, TValue>[] nodes, ulong bitmapValues, uint index);
    }
}