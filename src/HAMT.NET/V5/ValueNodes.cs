using System;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace HAMT.NET.V5
{
    public interface IValueNodes<TKey, TValue> where TKey : IEquatable<TKey>
    {
        [Pure]
        bool ContainsKey(TKey key, uint hash, long index);

        [Pure]
        TKey GetKey(long index);

        [Pure]
        TValue GetValue(long index);

        [Pure]
        ImmutableDictionary<TKey, TValue> Add<TBranchNodes>(TKey key, TValue value, ulong bitmapNodes, TBranchNodes nodes, ulong bitmapValues, uint index)
            where TBranchNodes : struct, IBranchNodes<TKey, TValue>;

        [Pure]
        ImmutableDictionary<TKey, TValue> Shrink<TBranchNodes>(ulong bitmapNodes, TBranchNodes nodes, ulong bitmapValues, uint index)
            where TBranchNodes : struct, IBranchNodes<TKey, TValue>;
    }

    public static class ValueNodes<TKey, TValue> where TKey : IEquatable<TKey>
    {
        public static unsafe bool ContainsKey<TValues>(TValues this0, TKey key, uint hash, long index) where TValues: struct 
        {
            var ptr = (byte*)Unsafe.AsPointer(ref this0) + index * (Unsafe.SizeOf<TKey>() + Unsafe.SizeOf<TValue>());
            return hash == (uint)Unsafe.Read<TKey>(ptr).GetHashCode() && key.Equals(Unsafe.Read<TKey>(ptr));
        }

        public static unsafe TKey GetKey<TValues>(TValues this0, long index) where TValues : struct
        {
            var ptr = (byte*)Unsafe.AsPointer(ref this0) + index * (Unsafe.SizeOf<TKey>() + Unsafe.SizeOf<TValue>());
            return Unsafe.Read<TKey>(ptr);
        }

        public static unsafe TValue GetValue<TValues>(TValues this0, long index) where TValues : struct
        {
            var ptr = (byte*)Unsafe.AsPointer(ref this0) + index * (Unsafe.SizeOf<TKey>() + Unsafe.SizeOf<TValue>()) + Unsafe.SizeOf<TKey>();
            return Unsafe.Read<TValue>(ptr);
        }

        public static unsafe ImmutableDictionary<TKey, TValue> Expand<TFrom, TTo, TBranchNodes>(
            TFrom @from, TKey key, TValue value, ulong bitmapNodes, TBranchNodes nodes, ulong bitmapValues, uint index)
            where TFrom : struct, IValueNodes<TKey, TValue> where TTo : struct, IValueNodes<TKey, TValue> 
            where TBranchNodes : struct, IBranchNodes<TKey, TValue>
        {
            var to = default(TTo);
            var ptrFrom = Unsafe.AsPointer(ref @from);
            var ptrTo = Unsafe.AsPointer(ref to);
            Unsafe.CopyBlock(ptrTo, ptrFrom, (uint) (index * (Unsafe.SizeOf<TKey>() + Unsafe.SizeOf<TValue>())));
            Unsafe.Write((byte*)ptrTo + (uint)(index * (Unsafe.SizeOf<TKey>() + Unsafe.SizeOf<TValue>())), key);
            Unsafe.Write((byte*)ptrTo + (uint)(index * (Unsafe.SizeOf<TKey>() + Unsafe.SizeOf<TValue>())) + Unsafe.SizeOf<TKey>(), value);
            Unsafe.CopyBlock(
                (byte*)ptrTo + (uint)((index + 1) * (Unsafe.SizeOf<TKey>() + Unsafe.SizeOf<TValue>())),
                (byte*)ptrFrom + (uint)((index) * (Unsafe.SizeOf<TKey>() + Unsafe.SizeOf<TValue>())),
                (uint) (Unsafe.SizeOf<TFrom>()- index * (Unsafe.SizeOf<TKey>() + Unsafe.SizeOf<TValue>())));

            return new BitMapNode<TKey, TValue, TTo, TBranchNodes>(bitmapNodes, nodes, bitmapValues, to);
        }

        public static unsafe ImmutableDictionary<TKey, TValue> Shrink<TFrom, TTo, TBranchNodes>(
            TFrom @from, ulong bitmapNodes, TBranchNodes nodes, ulong bitmapValues, uint index)
            where TFrom : struct, IValueNodes<TKey, TValue> 
            where TTo : struct, IValueNodes<TKey, TValue> 
            where TBranchNodes : struct, IBranchNodes<TKey, TValue>
        {
            var to = default(TTo);
            var ptrFrom = Unsafe.AsPointer(ref @from);
            var ptrTo = Unsafe.AsPointer(ref to);
            Unsafe.CopyBlock(ptrTo, ptrFrom, (uint) (index * (Unsafe.SizeOf<TKey>() + Unsafe.SizeOf<TValue>())));
            Unsafe.CopyBlock(
                (byte*)ptrTo + (uint)(index * (Unsafe.SizeOf<TKey>() + Unsafe.SizeOf<TValue>())),
                (byte*)ptrFrom + (uint)((index + 1) * (Unsafe.SizeOf<TKey>() + Unsafe.SizeOf<TValue>())),
                (uint) (Unsafe.SizeOf<TFrom>() - (index + 1) * (Unsafe.SizeOf<TKey>() + Unsafe.SizeOf<TValue>())));

            return new BitMapNode<TKey, TValue, TTo, TBranchNodes>(bitmapNodes, nodes, bitmapValues, to);
        }

    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ValueNode0<TKey, TValue> : IValueNodes<TKey, TValue> where TKey : IEquatable<TKey>
    {
        public bool ContainsKey(TKey key, uint hash, long index) => false;
        public TKey GetKey(long index) => default(TKey);
        public TValue GetValue(long index) => default(TValue);
        public ImmutableDictionary<TKey, TValue> Add<TBranchNodes>(TKey key, TValue value, ulong bitmapNodes, TBranchNodes nodes, ulong bitmapValues, uint index) 
            where TBranchNodes : struct, IBranchNodes<TKey, TValue>
        {
            return ValueNodes<TKey, TValue>.Expand<ValueNode0<TKey, TValue>, ValueNode1<TKey, TValue>, TBranchNodes>(this, key, value, bitmapNodes,
                nodes, bitmapValues, index);
        }

        public ImmutableDictionary<TKey, TValue> Shrink<TBranchNodes>(ulong bitmapNodes, TBranchNodes nodes, ulong bitmapValues, uint index) 
            where TBranchNodes : struct, IBranchNodes<TKey, TValue>
        {
            throw new NotImplementedException();
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ValueNode1<TKey, TValue> : IValueNodes<TKey, TValue> where TKey : IEquatable<TKey>
    {
        public ValueNode1(TKey key, TValue value)
        {
            _key = key;
            _value = value;
        }

        private readonly TKey _key;
        private readonly TValue _value;

        public bool ContainsKey(TKey key, uint hash, long index) => ValueNodes<TKey, TValue>.ContainsKey(this, key, hash, index);
        public TKey GetKey(long index) => ValueNodes<TKey, TValue>.GetKey(this, index);
        public TValue GetValue(long index) => ValueNodes<TKey, TValue>.GetValue(this, index);
        public ImmutableDictionary<TKey, TValue> Add<TBranchNodes>(TKey key, TValue value, ulong bitmapNodes, TBranchNodes nodes, ulong bitmapValues, uint index)
            where TBranchNodes : struct, IBranchNodes<TKey, TValue>
        {
            return ValueNodes<TKey, TValue>.Expand<ValueNode1<TKey, TValue>, ValueNode2<TKey, TValue>, TBranchNodes>(this, key, value, bitmapNodes, nodes,
                bitmapValues, index);
        }

        public ImmutableDictionary<TKey, TValue> Shrink<TBranchNodes>(ulong bitmapNodes, TBranchNodes nodes, ulong bitmapValues, uint index)
            where TBranchNodes : struct, IBranchNodes<TKey, TValue>
        {
            return ValueNodes<TKey, TValue>.Shrink<ValueNode1<TKey, TValue>, ValueNode0<TKey, TValue>, TBranchNodes>(this, bitmapNodes, nodes, bitmapValues, index);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
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

        public bool ContainsKey(TKey key, uint hash, long index) => ValueNodes<TKey, TValue>.ContainsKey(this, key, hash, index);
        public TKey GetKey(long index) => ValueNodes<TKey, TValue>.GetKey(this, index);
        public TValue GetValue(long index) => ValueNodes<TKey, TValue>.GetValue(this, index);

        public unsafe bool ContainsKeyBaseline(TKey key, uint hash, long index)
        {
            if (index == 0)
            {
                return hash == (uint)_key1.GetHashCode() && key.Equals(_key1);
            }

            return hash == (uint)_key2.GetHashCode() && key.Equals(_key2);
        }
        public ImmutableDictionary<TKey, TValue> Add<TBranchNodes>(TKey key, TValue value, ulong bitmapNodes, TBranchNodes nodes, ulong bitmapValues, uint index)
            where TBranchNodes : struct, IBranchNodes<TKey, TValue>
        {
            return ValueNodes<TKey, TValue>.Expand<ValueNode2<TKey, TValue>, ValueNode3<TKey, TValue>, TBranchNodes>(
                this, key, value, bitmapNodes, nodes,bitmapValues, index);
        }

        public ImmutableDictionary<TKey, TValue> Shrink<TBranchNodes>(ulong bitmapNodes, TBranchNodes nodes, ulong bitmapValues, uint index)
            where TBranchNodes : struct, IBranchNodes<TKey, TValue>
        {
            return ValueNodes<TKey, TValue>.Shrink<ValueNode2<TKey, TValue>, ValueNode1<TKey, TValue>, TBranchNodes>(this, bitmapNodes, nodes, bitmapValues, index);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
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

        public bool ContainsKey(TKey key, uint hash, long index) => ValueNodes<TKey, TValue>.ContainsKey(this, key, hash, index);
        public TKey GetKey(long index) => ValueNodes<TKey, TValue>.GetKey(this, index);
        public TValue GetValue(long index) => ValueNodes<TKey, TValue>.GetValue(this, index);
        public ImmutableDictionary<TKey, TValue> Add<TBranchNodes>(TKey key, TValue value, ulong bitmapNodes, TBranchNodes nodes, ulong bitmapValues, uint index)
            where TBranchNodes : struct, IBranchNodes<TKey, TValue>
        {
            return ValueNodes<TKey, TValue>.Expand<ValueNode3<TKey, TValue>, ValueNode4<TKey, TValue>, TBranchNodes>(this, key, value, bitmapNodes, nodes,
                bitmapValues, index);
        }

        public ImmutableDictionary<TKey, TValue> Shrink<TBranchNodes>(ulong bitmapNodes, TBranchNodes nodes, ulong bitmapValues, uint index)
            where TBranchNodes : struct, IBranchNodes<TKey, TValue>
        {
            return ValueNodes<TKey, TValue>.Shrink<ValueNode3<TKey, TValue>, ValueNode2<TKey, TValue>, TBranchNodes>(this, bitmapNodes, nodes, bitmapValues, index);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
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

        public bool ContainsKey(TKey key, uint hash, long index) => ValueNodes<TKey, TValue>.ContainsKey(this, key, hash, index);
        public TKey GetKey(long index) => ValueNodes<TKey, TValue>.GetKey(this, index);
        public TValue GetValue(long index) => ValueNodes<TKey, TValue>.GetValue(this, index);
        public ImmutableDictionary<TKey, TValue> Add<TBranchNodes>(TKey key, TValue value, ulong bitmapNodes, TBranchNodes nodes, ulong bitmapValues, uint index)
            where TBranchNodes : struct, IBranchNodes<TKey, TValue>
        {
            return ValueNodes<TKey, TValue>.Expand<ValueNode4<TKey, TValue>, ValueNode5<TKey, TValue>, TBranchNodes>(this, key, value, bitmapNodes, nodes,
                bitmapValues, index);
        }

        public ImmutableDictionary<TKey, TValue> Shrink<TBranchNodes>(ulong bitmapNodes, TBranchNodes nodes, ulong bitmapValues, uint index)
            where TBranchNodes : struct, IBranchNodes<TKey, TValue>
        {
            return ValueNodes<TKey, TValue>.Shrink<ValueNode4<TKey, TValue>, ValueNode3<TKey, TValue>, TBranchNodes>(this, bitmapNodes, nodes, bitmapValues, index);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
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

        public bool ContainsKey(TKey key, uint hash, long index) => ValueNodes<TKey, TValue>.ContainsKey(this, key, hash, index);
        public TKey GetKey(long index) => ValueNodes<TKey, TValue>.GetKey(this, index);
        public TValue GetValue(long index) => ValueNodes<TKey, TValue>.GetValue(this, index);
        public ImmutableDictionary<TKey, TValue> Add<TBranchNodes>(TKey key, TValue value, ulong bitmapNodes, TBranchNodes nodes, ulong bitmapValues, uint index)
            where TBranchNodes : struct, IBranchNodes<TKey, TValue>
        {
            return ValueNodes<TKey, TValue>.Expand<ValueNode5<TKey, TValue>, ValueNode6<TKey, TValue>, TBranchNodes>(this, key, value, bitmapNodes, nodes,
                bitmapValues, index);
        }

        public ImmutableDictionary<TKey, TValue> Shrink<TBranchNodes>(ulong bitmapNodes, TBranchNodes nodes, ulong bitmapValues, uint index)
            where TBranchNodes : struct, IBranchNodes<TKey, TValue>
        {
            return ValueNodes<TKey, TValue>.Shrink<ValueNode5<TKey, TValue>, ValueNode4<TKey, TValue>, TBranchNodes>(this, bitmapNodes, nodes, bitmapValues, index);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
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

        public bool ContainsKey(TKey key, uint hash, long index) => ValueNodes<TKey, TValue>.ContainsKey(this, key, hash, index);
        public TKey GetKey(long index) => ValueNodes<TKey, TValue>.GetKey(this, index);
        public TValue GetValue(long index) => ValueNodes<TKey, TValue>.GetValue(this, index);
        public ImmutableDictionary<TKey, TValue> Add<TBranchNodes>(TKey key, TValue value, ulong bitmapNodes, TBranchNodes nodes, ulong bitmapValues, uint index)
            where TBranchNodes : struct, IBranchNodes<TKey, TValue>
        {
            return ValueNodes<TKey, TValue>.Expand<ValueNode6<TKey, TValue>, ValueNode7<TKey, TValue>, TBranchNodes>(this, key, value, bitmapNodes, nodes, bitmapValues, index);
        }

        public ImmutableDictionary<TKey, TValue> Shrink<TBranchNodes>(ulong bitmapNodes, TBranchNodes nodes, ulong bitmapValues, uint index)
            where TBranchNodes : struct, IBranchNodes<TKey, TValue>
        {
            return ValueNodes<TKey, TValue>.Shrink<ValueNode6<TKey, TValue>, ValueNode5<TKey, TValue>, TBranchNodes>(this, bitmapNodes, nodes, bitmapValues, index);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
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

        public bool ContainsKey(TKey key, uint hash, long index) => ValueNodes<TKey, TValue>.ContainsKey(this, key, hash, index);
        public TKey GetKey(long index) => ValueNodes<TKey, TValue>.GetKey(this, index);
        public TValue GetValue(long index) => ValueNodes<TKey, TValue>.GetValue(this, index);
        public ImmutableDictionary<TKey, TValue> Add<TBranchNodes>(TKey key, TValue value, ulong bitmapNodes, TBranchNodes nodes, ulong bitmapValues, uint index)
            where TBranchNodes : struct, IBranchNodes<TKey, TValue>
        {
            return ValueNodes<TKey, TValue>.Expand<ValueNode7<TKey, TValue>, ValueNode8<TKey, TValue>, TBranchNodes>(this, key, value, bitmapNodes, nodes, bitmapValues, index);
        }

        public ImmutableDictionary<TKey, TValue> Shrink<TBranchNodes>(ulong bitmapNodes, TBranchNodes nodes, ulong bitmapValues, uint index)
            where TBranchNodes : struct, IBranchNodes<TKey, TValue>
        {
            return ValueNodes<TKey, TValue>.Shrink<ValueNode7<TKey, TValue>, ValueNode6<TKey, TValue>, TBranchNodes>(this, bitmapNodes, nodes, bitmapValues, index);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
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

        public bool ContainsKey(TKey key, uint hash, long index) => ValueNodes<TKey, TValue>.ContainsKey(this, key, hash, index);
        public TKey GetKey(long index) => ValueNodes<TKey, TValue>.GetKey(this, index);
        public TValue GetValue(long index) => ValueNodes<TKey, TValue>.GetValue(this, index);

        public ImmutableDictionary<TKey, TValue> Add<TBranchNodes>(TKey key, TValue value, ulong bitmapNodes, TBranchNodes nodes, ulong bitmapValues, uint index)
            where TBranchNodes : struct, IBranchNodes<TKey, TValue>
        {
            throw new NotImplementedException();
        }

        public ImmutableDictionary<TKey, TValue> Shrink<TBranchNodes>(ulong bitmapNodes, TBranchNodes nodes, ulong bitmapValues, uint index)
            where TBranchNodes : struct, IBranchNodes<TKey, TValue>
        {
            return ValueNodes<TKey, TValue>.Shrink<ValueNode8<TKey, TValue>, ValueNode7<TKey, TValue>, TBranchNodes>(this, bitmapNodes, nodes, bitmapValues, index);
        }
    }
}