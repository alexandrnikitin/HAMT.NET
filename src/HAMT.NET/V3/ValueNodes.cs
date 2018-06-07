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
            if (index == 0)
            {
                var values = new ValueNode3<TKey, TValue>(key, value, _key1, _value1, _key2, _value2);
                return new BitMapNode<TKey, TValue, ValueNode3<TKey, TValue>>(bitmapNodes, nodes, bitmapValues | index, values);
            }

            if (index == 1)
            {
                var values = new ValueNode3<TKey, TValue>(_key1, _value1, key, value, _key2, _value2);
                return new BitMapNode<TKey, TValue, ValueNode3<TKey, TValue>>(bitmapNodes, nodes, bitmapValues | index, values);
            }

            {
                var values = new ValueNode3<TKey, TValue>(_key1, _value1, _key2, _value2, key, value);
                return new BitMapNode<TKey, TValue, ValueNode3<TKey, TValue>>(bitmapNodes, nodes, bitmapValues | index, values);
            }
        }
    }

    public struct ValueNode3<TKey, TValue> : IValueNodes<TKey, TValue> where TKey : IEquatable<TKey>
    {
        private readonly TKey _key1;
        private readonly TValue _value1;
        private readonly TKey _key2;
        private readonly TValue _value2;
        private readonly TKey _key3;
        private readonly TValue _value3;

        public ValueNode3(TKey key1, TValue value1, TKey key2, TValue value2, TKey key3, TValue value3)
        {
            _key1 = key1;
            _value1 = value1;
            _key2 = key2;
            _value2 = value2;
            _key3 = key3;
            _value3 = value3;
        }


        public bool ContainsKey(TKey key, uint hash, long index)
        {
            if (index == 0)
            {
                return hash == (uint) _key1.GetHashCode() && key.Equals(_key1);
            }

            if (index == 1)
            {
                return hash == (uint) _key2.GetHashCode() && key.Equals(_key2);
            }

            return hash == (uint) _key3.GetHashCode() && key.Equals(_key3);
        }

        public ImmutableDictionary<TKey, TValue> Add(TKey key, TValue value, ulong bitmapNodes,
            ImmutableDictionary<TKey, TValue>[] nodes, ulong bitmapValues, uint index)
        {
            if (index == 0)
            {
                var values = new ValueNode4<TKey, TValue>(key, value, _key1, _value1, _key2, _value2, _key3, _value3);
                return new BitMapNode<TKey, TValue, ValueNode4<TKey, TValue>>(bitmapNodes, nodes, bitmapValues | index, values);
            }

            if (index == 1)
            {
                var values = new ValueNode4<TKey, TValue>(_key1, _value1, key, value, _key2, _value2, _key3, _value3);
                return new BitMapNode<TKey, TValue, ValueNode4<TKey, TValue>>(bitmapNodes, nodes, bitmapValues | index, values);
            }

            if (index == 2)
            {
                var values = new ValueNode4<TKey, TValue>(_key1, _value1, _key2, _value2, key, value, _key3, _value3);
                return new BitMapNode<TKey, TValue, ValueNode4<TKey, TValue>>(bitmapNodes, nodes, bitmapValues | index, values);
            }

            {
                var values = new ValueNode4<TKey, TValue>(_key1, _value1, _key2, _value2, _key3, _value3, key, value);
                return new BitMapNode<TKey, TValue, ValueNode4<TKey, TValue>>(bitmapNodes, nodes, bitmapValues | index, values);
            }
        }
    }

    public struct ValueNode4<TKey, TValue> : IValueNodes<TKey, TValue> where TKey : IEquatable<TKey>
    {
        private readonly TKey _key1;
        private readonly TValue _value1;
        private readonly TKey _key2;
        private readonly TValue _value2;
        private readonly TKey _key3;
        private readonly TValue _value3;
        private readonly TKey _key4;
        private readonly TValue _value4;

        public ValueNode4(TKey key1, TValue value1, TKey key2, TValue value2, TKey key3, TValue value3, TKey key4, TValue value4)
        {
            _key1 = key1;
            _value1 = value1;
            _key2 = key2;
            _value2 = value2;
            _key3 = key3;
            _value3 = value3;
            _key4 = key4;
            _value4 = value4;
        }


        public bool ContainsKey(TKey key, uint hash, long index)
        {
            if (index == 0)
            {
                return hash == (uint) _key1.GetHashCode() && key.Equals(_key1);
            }

            if (index == 1)
            {
                return hash == (uint) _key2.GetHashCode() && key.Equals(_key2);
            }

            if (index == 2)
            {
                return hash == (uint) _key3.GetHashCode() && key.Equals(_key3);
            }

            return hash == (uint) _key4.GetHashCode() && key.Equals(_key4);
        }

        public ImmutableDictionary<TKey, TValue> Add(TKey key, TValue value, ulong bitmapNodes,
            ImmutableDictionary<TKey, TValue>[] nodes, ulong bitmapValues, uint index)
        {
            if (index == 0)
            {
                var values = new ValueNode5<TKey, TValue>(key, value, _key1, _value1, _key2, _value2, _key3, _value3, _key4, _value4);
                return new BitMapNode<TKey, TValue, ValueNode5<TKey, TValue>>(bitmapNodes, nodes, bitmapValues | index, values);
            }

            if (index == 1)
            {
                var values = new ValueNode5<TKey, TValue>(_key1, _value1, key, value, _key2, _value2, _key3, _value3, _key4, _value4);
                return new BitMapNode<TKey, TValue, ValueNode5<TKey, TValue>>(bitmapNodes, nodes, bitmapValues | index, values);
            }

            if (index == 2)
            {
                var values = new ValueNode5<TKey, TValue>(_key1, _value1, _key2, _value2, key, value, _key3, _value3, _key4, _value4);
                return new BitMapNode<TKey, TValue, ValueNode5<TKey, TValue>>(bitmapNodes, nodes, bitmapValues | index, values);
            }

            if (index == 3)
            {
                var values = new ValueNode5<TKey, TValue>(_key1, _value1, _key2, _value2, _key3, _value3, key, value, _key4, _value4);
                return new BitMapNode<TKey, TValue, ValueNode5<TKey, TValue>>(bitmapNodes, nodes, bitmapValues | index, values);
            }

            {
                var values = new ValueNode5<TKey, TValue>(_key1, _value1, _key2, _value2, _key3, _value3, _key4, _value4, key, value);
                return new BitMapNode<TKey, TValue, ValueNode5<TKey, TValue>>(bitmapNodes, nodes, bitmapValues | index, values);
            }
        }
    }

    public struct ValueNode5<TKey, TValue> : IValueNodes<TKey, TValue> where TKey : IEquatable<TKey>
    {
        private readonly TKey _key1;
        private readonly TValue _value1;
        private readonly TKey _key2;
        private readonly TValue _value2;
        private readonly TKey _key3;
        private readonly TValue _value3;
        private readonly TKey _key4;
        private readonly TValue _value4;
        private readonly TKey _key5;
        private readonly TValue _value5;

        public ValueNode5(TKey key1, TValue value1, TKey key2, TValue value2, TKey key3, TValue value3, TKey key4, TValue value4, TKey key5,
            TValue value5)
        {
            _key1 = key1;
            _value1 = value1;
            _key2 = key2;
            _value2 = value2;
            _key3 = key3;
            _value3 = value3;
            _key4 = key4;
            _value4 = value4;
            _key5 = key5;
            _value5 = value5;
        }


        public bool ContainsKey(TKey key, uint hash, long index)
        {
            if (index == 0)
            {
                return hash == (uint) _key1.GetHashCode() && key.Equals(_key1);
            }

            if (index == 1)
            {
                return hash == (uint) _key2.GetHashCode() && key.Equals(_key2);
            }

            if (index == 2)
            {
                return hash == (uint) _key3.GetHashCode() && key.Equals(_key3);
            }

            if (index == 3)
            {
                return hash == (uint) _key4.GetHashCode() && key.Equals(_key4);
            }

            return hash == (uint) _key5.GetHashCode() && key.Equals(_key5);
        }

        public ImmutableDictionary<TKey, TValue> Add(TKey key, TValue value, ulong bitmapNodes,
            ImmutableDictionary<TKey, TValue>[] nodes, ulong bitmapValues, uint index)
        {
            if (index == 0)
            {
                var values = new ValueNode6<TKey, TValue>(key, value, _key1, _value1, _key2, _value2, _key3, _value3, _key4, _value4, _key5, _value5);
                return new BitMapNode<TKey, TValue, ValueNode6<TKey, TValue>>(bitmapNodes, nodes, bitmapValues | index, values);
            }

            if (index == 1)
            {
                var values = new ValueNode6<TKey, TValue>(_key1, _value1, key, value, _key2, _value2, _key3, _value3, _key4, _value4, _key5, _value5);
                return new BitMapNode<TKey, TValue, ValueNode6<TKey, TValue>>(bitmapNodes, nodes, bitmapValues | index, values);
            }

            if (index == 2)
            {
                var values = new ValueNode6<TKey, TValue>(_key1, _value1, _key2, _value2, key, value, _key3, _value3, _key4, _value4, _key5, _value5);
                return new BitMapNode<TKey, TValue, ValueNode6<TKey, TValue>>(bitmapNodes, nodes, bitmapValues | index, values);
            }

            if (index == 3)
            {
                var values = new ValueNode6<TKey, TValue>(_key1, _value1, _key2, _value2, _key3, _value3, key, value, _key4, _value4, _key5, _value5);
                return new BitMapNode<TKey, TValue, ValueNode6<TKey, TValue>>(bitmapNodes, nodes, bitmapValues | index, values);
            }

            if (index == 4)
            {
                var values = new ValueNode6<TKey, TValue>(_key1, _value1, _key2, _value2, _key3, _value3, _key4, _value4, key, value, _key5, _value5);
                return new BitMapNode<TKey, TValue, ValueNode6<TKey, TValue>>(bitmapNodes, nodes, bitmapValues | index, values);
            }

            {
                var values = new ValueNode6<TKey, TValue>(_key1, _value1, _key2, _value2, _key3, _value3, _key4, _value4, _key5, _value5, key, value);
                return new BitMapNode<TKey, TValue, ValueNode6<TKey, TValue>>(bitmapNodes, nodes, bitmapValues | index, values);
            }
        }
    }

    public struct ValueNode6<TKey, TValue> : IValueNodes<TKey, TValue> where TKey : IEquatable<TKey>
    {
        private readonly TKey _key1;
        private readonly TValue _value1;
        private readonly TKey _key2;
        private readonly TValue _value2;
        private readonly TKey _key3;
        private readonly TValue _value3;
        private readonly TKey _key4;
        private readonly TValue _value4;
        private readonly TKey _key5;
        private readonly TValue _value5;
        private readonly TKey _key6;
        private readonly TValue _value6;

        public ValueNode6(TKey key1, TValue value1, TKey key2, TValue value2, TKey key3, TValue value3, TKey key4, TValue value4, TKey key5,
            TValue value5, TKey key6, TValue value6)
        {
            _key1 = key1;
            _value1 = value1;
            _key2 = key2;
            _value2 = value2;
            _key3 = key3;
            _value3 = value3;
            _key4 = key4;
            _value4 = value4;
            _key5 = key5;
            _value5 = value5;
            _key6 = key6;
            _value6 = value6;
        }


        public bool ContainsKey(TKey key, uint hash, long index)
        {
            if (index == 0)
            {
                return hash == (uint) _key1.GetHashCode() && key.Equals(_key1);
            }

            if (index == 1)
            {
                return hash == (uint) _key2.GetHashCode() && key.Equals(_key2);
            }

            if (index == 2)
            {
                return hash == (uint) _key3.GetHashCode() && key.Equals(_key3);
            }

            if (index == 3)
            {
                return hash == (uint) _key4.GetHashCode() && key.Equals(_key4);
            }

            if (index == 4)
            {
                return hash == (uint) _key5.GetHashCode() && key.Equals(_key5);
            }

            return hash == (uint) _key6.GetHashCode() && key.Equals(_key6);
        }

        public ImmutableDictionary<TKey, TValue> Add(TKey key, TValue value, ulong bitmapNodes,
            ImmutableDictionary<TKey, TValue>[] nodes, ulong bitmapValues, uint index)
        {
            if (index == 0)
            {
                var values = new ValueNode7<TKey, TValue>(key, value, _key1, _value1, _key2, _value2, _key3, _value3, _key4, _value4, _key5, _value5, _key6, _value6);
                return new BitMapNode<TKey, TValue, ValueNode7<TKey, TValue>>(bitmapNodes, nodes, bitmapValues | index, values);
            }
            if (index == 1)
            {
                var values = new ValueNode7<TKey, TValue>(_key1, _value1, key, value, _key2, _value2, _key3, _value3, _key4, _value4, _key5, _value5, _key6, _value6);
                return new BitMapNode<TKey, TValue, ValueNode7<TKey, TValue>>(bitmapNodes, nodes, bitmapValues | index, values);
            }
            if (index == 2)
            {
                var values = new ValueNode7<TKey, TValue>(_key1, _value1, _key2, _value2, key, value, _key3, _value3, _key4, _value4, _key5, _value5, _key6, _value6);
                return new BitMapNode<TKey, TValue, ValueNode7<TKey, TValue>>(bitmapNodes, nodes, bitmapValues | index, values);
            }
            if (index == 3)
            {
                var values = new ValueNode7<TKey, TValue>(_key1, _value1, _key2, _value2, _key3, _value3, key, value, _key4, _value4, _key5, _value5, _key6, _value6);
                return new BitMapNode<TKey, TValue, ValueNode7<TKey, TValue>>(bitmapNodes, nodes, bitmapValues | index, values);
            }
            if (index == 4)
            {
                var values = new ValueNode7<TKey, TValue>(_key1, _value1, _key2, _value2, _key3, _value3, _key4, _value4, key, value, _key5, _value5, _key6, _value6);
                return new BitMapNode<TKey, TValue, ValueNode7<TKey, TValue>>(bitmapNodes, nodes, bitmapValues | index, values);
            }
            if (index == 5)
            {
                var values = new ValueNode7<TKey, TValue>(_key1, _value1, _key2, _value2, _key3, _value3, _key4, _value4, _key5, _value5, key, value, _key6, _value6);
                return new BitMapNode<TKey, TValue, ValueNode7<TKey, TValue>>(bitmapNodes, nodes, bitmapValues | index, values);
            }
            {
                var values = new ValueNode7<TKey, TValue>(_key1, _value1, _key2, _value2, _key3, _value3, _key4, _value4, _key5, _value5, _key6, _value6, key, value);
                return new BitMapNode<TKey, TValue, ValueNode7<TKey, TValue>>(bitmapNodes, nodes, bitmapValues | index, values);
            }
        }
    }

    public struct ValueNode7<TKey, TValue> : IValueNodes<TKey, TValue> where TKey : IEquatable<TKey>
    {
        private readonly TKey _key1;
        private readonly TValue _value1;
        private readonly TKey _key2;
        private readonly TValue _value2;
        private readonly TKey _key3;
        private readonly TValue _value3;
        private readonly TKey _key4;
        private readonly TValue _value4;
        private readonly TKey _key5;
        private readonly TValue _value5;
        private readonly TKey _key6;
        private readonly TValue _value6;
        private readonly TKey _key7;
        private readonly TValue _value7;

        public ValueNode7(TKey key1, TValue value1, TKey key2, TValue value2, TKey key3, TValue value3, TKey key4, TValue value4, TKey key5,
            TValue value5, TKey key6, TValue value6, TKey key7, TValue value7)
        {
            _key1 = key1;
            _value1 = value1;
            _key2 = key2;
            _value2 = value2;
            _key3 = key3;
            _value3 = value3;
            _key4 = key4;
            _value4 = value4;
            _key5 = key5;
            _value5 = value5;
            _key6 = key6;
            _value6 = value6;
            _key7 = key7;
            _value7 = value7;
        }


        public bool ContainsKey(TKey key, uint hash, long index)
        {
            if (index == 0)
            {
                return hash == (uint) _key1.GetHashCode() && key.Equals(_key1);
            }

            if (index == 1)
            {
                return hash == (uint) _key2.GetHashCode() && key.Equals(_key2);
            }

            if (index == 2)
            {
                return hash == (uint) _key3.GetHashCode() && key.Equals(_key3);
            }

            if (index == 3)
            {
                return hash == (uint) _key4.GetHashCode() && key.Equals(_key4);
            }

            if (index == 4)
            {
                return hash == (uint) _key5.GetHashCode() && key.Equals(_key5);
            }

            if (index == 5)
            {
                return hash == (uint) _key6.GetHashCode() && key.Equals(_key6);
            }

            return hash == (uint) _key7.GetHashCode() && key.Equals(_key7);
        }

        public ImmutableDictionary<TKey, TValue> Add(TKey key, TValue value, ulong bitmapNodes,
            ImmutableDictionary<TKey, TValue>[] nodes, ulong bitmapValues, uint index)
        {
            if (index == 0)
            {
                var values = new ValueNode8<TKey, TValue>(key, value, _key1, _value1, _key2, _value2, _key3, _value3, _key4, _value4, _key5, _value5, _key6, _value6, _key7, _value7);
                return new BitMapNode<TKey, TValue, ValueNode8<TKey, TValue>>(bitmapNodes, nodes, bitmapValues | index, values);
            }
            if (index == 1)
            {
                var values = new ValueNode8<TKey, TValue>(_key1, _value1, key, value, _key2, _value2, _key3, _value3, _key4, _value4, _key5, _value5, _key6, _value6, _key7, _value7);
                return new BitMapNode<TKey, TValue, ValueNode8<TKey, TValue>>(bitmapNodes, nodes, bitmapValues | index, values);
            }
            if (index == 2)
            {
                var values = new ValueNode8<TKey, TValue>(_key1, _value1, _key2, _value2, key, value, _key3, _value3, _key4, _value4, _key5, _value5, _key6, _value6, _key7, _value7);
                return new BitMapNode<TKey, TValue, ValueNode8<TKey, TValue>>(bitmapNodes, nodes, bitmapValues | index, values);
            }
            if (index == 3)
            {
                var values = new ValueNode8<TKey, TValue>(_key1, _value1, _key2, _value2, _key3, _value3, key, value, _key4, _value4, _key5, _value5, _key6, _value6, _key7, _value7);
                return new BitMapNode<TKey, TValue, ValueNode8<TKey, TValue>>(bitmapNodes, nodes, bitmapValues | index, values);
            }
            if (index == 4)
            {
                var values = new ValueNode8<TKey, TValue>(_key1, _value1, _key2, _value2, _key3, _value3, _key4, _value4, key, value, _key5, _value5, _key6, _value6, _key7, _value7);
                return new BitMapNode<TKey, TValue, ValueNode8<TKey, TValue>>(bitmapNodes, nodes, bitmapValues | index, values);
            }
            if (index == 5)
            {
                var values = new ValueNode8<TKey, TValue>(_key1, _value1, _key2, _value2, _key3, _value3, _key4, _value4, _key5, _value5, key, value, _key6, _value6, _key7, _value7);
                return new BitMapNode<TKey, TValue, ValueNode8<TKey, TValue>>(bitmapNodes, nodes, bitmapValues | index, values);
            }
            if (index == 6)
            {
                var values = new ValueNode8<TKey, TValue>(_key1, _value1, _key2, _value2, _key3, _value3, _key4, _value4, _key5, _value5, _key6, _value6, key, value, _key7, _value7);
                return new BitMapNode<TKey, TValue, ValueNode8<TKey, TValue>>(bitmapNodes, nodes, bitmapValues | index, values);
            }
            {
                var values = new ValueNode8<TKey, TValue>(_key1, _value1, _key2, _value2, _key3, _value3, _key4, _value4, _key5, _value5, _key6, _value6, _key7, _value7, key, value);
                return new BitMapNode<TKey, TValue, ValueNode8<TKey, TValue>>(bitmapNodes, nodes, bitmapValues | index, values);
            }
        }
    }

    public struct ValueNode8<TKey, TValue> : IValueNodes<TKey, TValue> where TKey : IEquatable<TKey>
    {
        private readonly TKey _key1;
        private readonly TValue _value1;
        private readonly TKey _key2;
        private readonly TValue _value2;
        private readonly TKey _key3;
        private readonly TValue _value3;
        private readonly TKey _key4;
        private readonly TValue _value4;
        private readonly TKey _key5;
        private readonly TValue _value5;
        private readonly TKey _key6;
        private readonly TValue _value6;
        private readonly TKey _key7;
        private readonly TValue _value7;
        private readonly TKey _key8;
        private readonly TValue _value8;

        public ValueNode8(TKey key1, TValue value1, TKey key2, TValue value2, TKey key3, TValue value3, TKey key4, TValue value4, TKey key5,
            TValue value5, TKey key6, TValue value6, TKey key7, TValue value7, TKey key8, TValue value8)
        {
            _key1 = key1;
            _value1 = value1;
            _key2 = key2;
            _value2 = value2;
            _key3 = key3;
            _value3 = value3;
            _key4 = key4;
            _value4 = value4;
            _key5 = key5;
            _value5 = value5;
            _key6 = key6;
            _value6 = value6;
            _key7 = key7;
            _value7 = value7;
            _key8 = key8;
            _value8 = value8;
        }


        public bool ContainsKey(TKey key, uint hash, long index)
        {
            if (index == 0)
            {
                return hash == (uint) _key1.GetHashCode() && key.Equals(_key1);
            }

            if (index == 1)
            {
                return hash == (uint) _key2.GetHashCode() && key.Equals(_key2);
            }

            if (index == 2)
            {
                return hash == (uint) _key3.GetHashCode() && key.Equals(_key3);
            }

            if (index == 3)
            {
                return hash == (uint) _key4.GetHashCode() && key.Equals(_key4);
            }

            if (index == 4)
            {
                return hash == (uint) _key5.GetHashCode() && key.Equals(_key5);
            }

            if (index == 5)
            {
                return hash == (uint) _key6.GetHashCode() && key.Equals(_key6);
            }

            if (index == 6)
            {
                return hash == (uint) _key7.GetHashCode() && key.Equals(_key7);
            }

            return hash == (uint) _key8.GetHashCode() && key.Equals(_key8);
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