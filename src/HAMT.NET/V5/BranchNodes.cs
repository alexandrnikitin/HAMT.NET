using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace HAMT.NET.V5
{
    public interface IBranchNodes<TKey, TValue> where TKey : IEquatable<TKey>
    {
        [Pure]
        bool ContainsKey(TKey key, uint hash, int index, int shift);

        [Pure]
        ImmutableDictionary<TKey, TValue> Add<TValueNodes>(
            TKey key, TValue value, uint hash, uint index, uint indexValues, int shift, ulong bitmapNodes, ulong bitmapValues, TValueNodes values)
            where TValueNodes : struct, IValueNodes<TKey, TValue>;

        [Pure]
        ImmutableDictionary<TKey, TValue> DuplicateWith<TValueNodes>(
            TKey key, TValue value, uint hash, long index, int shift, ulong bitmapNodes, ulong bitmapValues, TValueNodes values)
            where TValueNodes : struct, IValueNodes<TKey, TValue>;
    }

    public static class BranchNodes<TKey, TValue> where TKey : IEquatable<TKey>
    {
        public static unsafe bool ContainsKey<TBranchNodes>(TBranchNodes this0, TKey key, uint hash, long index, int shift) 
            where TBranchNodes : struct
        {
            var ptr = (byte*)Unsafe.AsPointer(ref this0) + index * IntPtr.Size;
            var node = Unsafe.AsRef<ImmutableDictionary<TKey, TValue>>(ptr);
            return node.ContainsKey(key, hash, shift);
        }

        public static unsafe ImmutableDictionary<TKey, TValue> Add<TFrom, TTo, TValueNodes>(
            TFrom @from, TKey key, TValue value, uint hash, uint index, uint indexValues, int shift, ulong bitmapNodes,
            ulong bitmapValues, TValueNodes values)
            where TFrom : struct, IBranchNodes<TKey, TValue>
            where TTo : struct, IBranchNodes<TKey, TValue>
            where TValueNodes : struct, IValueNodes<TKey, TValue>
        {
            var to = default(TTo);
            var ptrFrom = Unsafe.AsPointer(ref @from);
            var ptrTo = Unsafe.AsPointer(ref to);

            Unsafe.CopyBlock(ptrTo, ptrFrom, (uint)(index * IntPtr.Size));
            var key2 = values.GetKey(indexValues);
            var value2 = values.GetValue(indexValues);
            var newNode = BitMapNode<TKey, TValue, TValueNodes, TFrom>.From(key, value, hash, shift + ImmutableDictionary.Shift, key2, value2);
            Unsafe.Write((byte*)ptrTo + (uint)(index * IntPtr.Size), newNode);
            Unsafe.CopyBlock(
                (byte*)ptrTo + (uint)((index + 1) * IntPtr.Size),
                (byte*)ptrFrom + (uint)((index) * IntPtr.Size),
                (uint)(Unsafe.SizeOf<TFrom>() - index * IntPtr.Size));

            for (int i = 0; i < Unsafe.SizeOf<TTo>(); i += IntPtr.Size)
            {
                Debug.Assert(Unsafe.AsRef<ImmutableDictionary<TKey, TValue>>((byte*)ptrTo + i) != null);
            }

            return values.Shrink(bitmapNodes, to, bitmapValues, (uint)indexValues);

        }

        public static unsafe ImmutableDictionary<TKey, TValue> Clone<TBranchNodes, TValueNodes>(
            TBranchNodes @from, TKey key, TValue value, uint hash, long index, int shift, ulong bitmapNodes, ulong bitmapValues, TValueNodes values)
            where TBranchNodes : struct, IBranchNodes<TKey, TValue>
            where TValueNodes : struct, IValueNodes<TKey, TValue>
        {
            var to = default(TBranchNodes);
            var ptrFrom = Unsafe.AsPointer(ref @from);
            var ptrTo = Unsafe.AsPointer(ref to);
            Unsafe.CopyBlock(ptrTo, ptrFrom, (uint) Unsafe.SizeOf<TBranchNodes>());
            for (int i = 0; i < Unsafe.SizeOf<TBranchNodes>(); i += IntPtr.Size)
            {
                Debug.Assert(Unsafe.AsRef<ImmutableDictionary<TKey, TValue>>((byte*)ptrTo + i) != null);
            }
            var newNode = Unsafe.AsRef<ImmutableDictionary<TKey, TValue>>((byte*)ptrTo + index * IntPtr.Size).Add(key, value, hash, shift + ImmutableDictionary.Shift);
            Unsafe.Write((byte*)ptrTo + index * IntPtr.Size, newNode);

            for (int i = 0; i < Unsafe.SizeOf<TBranchNodes>(); i += IntPtr.Size)
            {
                Debug.Assert(Unsafe.AsRef<ImmutableDictionary<TKey, TValue>>((byte*)ptrTo + i) != null);
            }

            return new BitMapNode<TKey, TValue, TValueNodes, TBranchNodes>(bitmapNodes, to, bitmapValues, values);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BranchNodes0<TKey, TValue> : IBranchNodes<TKey, TValue> where TKey : IEquatable<TKey>
    {
        public bool ContainsKey(TKey key, uint hash, int index, int shift)
        {
            throw new NotImplementedException();
        }

        public ImmutableDictionary<TKey, TValue> Add<TValueNodes>(
            TKey key, TValue value, uint hash, uint index, uint indexValues, int shift, ulong bitmapNodes, 
            ulong bitmapValues, TValueNodes values) 
            where TValueNodes : struct, IValueNodes<TKey, TValue>
        {
            return BranchNodes<TKey, TValue>.Add<BranchNodes0<TKey, TValue>, BranchNodes1<TKey, TValue>, TValueNodes>(
                this, key, value, hash, index, indexValues, shift, bitmapNodes, bitmapValues, values);
        }

        public ImmutableDictionary<TKey, TValue> DuplicateWith<TValueNodes>(
            TKey key, TValue value, uint hash, long index, int shift, ulong bitmapNodes, ulong bitmapValues, TValueNodes values)
            where TValueNodes : struct, IValueNodes<TKey, TValue>
        {
            throw new NotImplementedException();
//            return BranchNodes<TKey, TValue>.Clone(this, key, value, hash, index, shift, bitmapNodes, bitmapValues, values);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BranchNodes1<TKey, TValue> : IBranchNodes<TKey, TValue> where TKey : IEquatable<TKey>
    {
        public BranchNodes1(ImmutableDictionary<TKey, TValue> node1)
        {
            _node1 = node1;
        }

        private readonly ImmutableDictionary<TKey, TValue> _node1;

        public bool ContainsKey(TKey key, uint hash, int index, int shift) => 
            BranchNodes<TKey, TValue>.ContainsKey(this, key, hash, index, shift);

        public ImmutableDictionary<TKey, TValue> Add<TValueNodes>(
            TKey key, TValue value, uint hash, uint index, uint indexValues, int shift, ulong bitmapNodes,
            ulong bitmapValues, TValueNodes values)
            where TValueNodes : struct, IValueNodes<TKey, TValue>
        {
            Debug.Assert(_node1 != null);
            return BranchNodes<TKey, TValue>.Add<BranchNodes1<TKey, TValue>, BranchNodes2<TKey, TValue>, TValueNodes>(
                this, key, value, hash, index, indexValues, shift, bitmapNodes, bitmapValues, values);
        }

        public ImmutableDictionary<TKey, TValue> DuplicateWith<TValueNodes>(
            TKey key, TValue value, uint hash, long index, int shift, ulong bitmapNodes, ulong bitmapValues, TValueNodes values)
            where TValueNodes : struct, IValueNodes<TKey, TValue>
        {
            return BranchNodes<TKey, TValue>.Clone(this, key, value, hash, index, shift, bitmapNodes, bitmapValues, values);
        }

    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BranchNodes2<TKey, TValue> : IBranchNodes<TKey, TValue> where TKey : IEquatable<TKey>
    {
        private readonly ImmutableDictionary<TKey, TValue> _node1;
        private readonly ImmutableDictionary<TKey, TValue> _node2;

        public bool ContainsKey(TKey key, uint hash, int index, int shift) => 
            BranchNodes<TKey, TValue>.ContainsKey(this, key, hash, index, shift);

        public ImmutableDictionary<TKey, TValue> Add<TValueNodes>(
            TKey key, TValue value, uint hash, uint index, uint indexValues, int shift, ulong bitmapNodes,
            ulong bitmapValues, TValueNodes values)
            where TValueNodes : struct, IValueNodes<TKey, TValue>
        {
            Debug.Assert(_node1 != null);
            Debug.Assert(_node2 != null);
            return BranchNodes<TKey, TValue>.Add<BranchNodes2<TKey, TValue>, BranchNodes3<TKey, TValue>, TValueNodes>(
                this, key, value, hash, index, indexValues, shift, bitmapNodes, bitmapValues, values);
        }

        public ImmutableDictionary<TKey, TValue> DuplicateWith<TValueNodes>(
            TKey key, TValue value, uint hash, long index, int shift, ulong bitmapNodes, ulong bitmapValues, TValueNodes values)
            where TValueNodes : struct, IValueNodes<TKey, TValue>
        {
            Debug.Assert(_node1 != null);
            Debug.Assert(_node2 != null);

            return BranchNodes<TKey, TValue>.Clone(this, key, value, hash, index, shift, bitmapNodes, bitmapValues, values);
        }

    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BranchNodes3<TKey, TValue> : IBranchNodes<TKey, TValue> where TKey : IEquatable<TKey>
    {
        private readonly ImmutableDictionary<TKey, TValue> _node1;
        private readonly ImmutableDictionary<TKey, TValue> _node2;
        private readonly ImmutableDictionary<TKey, TValue> _node3;

        public bool ContainsKey(TKey key, uint hash, int index, int shift) => 
            BranchNodes<TKey, TValue>.ContainsKey(this, key, hash, index, shift);

        public ImmutableDictionary<TKey, TValue> Add<TValueNodes>(
            TKey key, TValue value, uint hash, uint index, uint indexValues, int shift, ulong bitmapNodes,
            ulong bitmapValues, TValueNodes values)
            where TValueNodes : struct, IValueNodes<TKey, TValue>
        {
            Debug.Assert(_node1 != null);
            Debug.Assert(_node2 != null);
            Debug.Assert(_node3 != null);
            return BranchNodes<TKey, TValue>.Add<BranchNodes3<TKey, TValue>, BranchNodes4<TKey, TValue>, TValueNodes>(
                this, key, value, hash, index, indexValues, shift, bitmapNodes, bitmapValues, values);
        }

        public ImmutableDictionary<TKey, TValue> DuplicateWith<TValueNodes>(
            TKey key, TValue value, uint hash, long index, int shift, ulong bitmapNodes, ulong bitmapValues, TValueNodes values)
            where TValueNodes : struct, IValueNodes<TKey, TValue>
        {
            Debug.Assert(_node1 != null);
            Debug.Assert(_node2 != null);
            Debug.Assert(_node3 != null);

            return BranchNodes<TKey, TValue>.Clone(this, key, value, hash, index, shift, bitmapNodes, bitmapValues, values);
        }

    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BranchNodes4<TKey, TValue> : IBranchNodes<TKey, TValue> where TKey : IEquatable<TKey>
    {
        private readonly ImmutableDictionary<TKey, TValue> _node1;
        private readonly ImmutableDictionary<TKey, TValue> _node2;
        private readonly ImmutableDictionary<TKey, TValue> _node3;
        private readonly ImmutableDictionary<TKey, TValue> _node4;

        public bool ContainsKey(TKey key, uint hash, int index, int shift) => 
            BranchNodes<TKey, TValue>.ContainsKey(this, key, hash, index, shift);

        public ImmutableDictionary<TKey, TValue> Add<TValueNodes>(
            TKey key, TValue value, uint hash, uint index, uint indexValues, int shift, ulong bitmapNodes,
            ulong bitmapValues, TValueNodes values)
            where TValueNodes : struct, IValueNodes<TKey, TValue>
        {
            Debug.Assert(_node1 != null);
            Debug.Assert(_node2 != null);
            Debug.Assert(_node3 != null);
            Debug.Assert(_node4 != null);

            return BranchNodes<TKey, TValue>.Add<BranchNodes4<TKey, TValue>, BranchNodes5<TKey, TValue>, TValueNodes>(
                this, key, value, hash, index, indexValues, shift, bitmapNodes, bitmapValues, values);
        }

        public ImmutableDictionary<TKey, TValue> DuplicateWith<TValueNodes>(
            TKey key, TValue value, uint hash, long index, int shift, ulong bitmapNodes, ulong bitmapValues, TValueNodes values)
            where TValueNodes : struct, IValueNodes<TKey, TValue>
        {
            Debug.Assert(_node1 != null);
            Debug.Assert(_node2 != null);
            Debug.Assert(_node3 != null);
            Debug.Assert(_node4 != null);

            return BranchNodes<TKey, TValue>.Clone(this, key, value, hash, index, shift, bitmapNodes, bitmapValues, values);
        }

    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BranchNodes5<TKey, TValue> : IBranchNodes<TKey, TValue> where TKey : IEquatable<TKey>
    {
        private readonly ImmutableDictionary<TKey, TValue> _node1;
        private readonly ImmutableDictionary<TKey, TValue> _node2;
        private readonly ImmutableDictionary<TKey, TValue> _node3;
        private readonly ImmutableDictionary<TKey, TValue> _node4;
        private readonly ImmutableDictionary<TKey, TValue> _node5;

        public bool ContainsKey(TKey key, uint hash, int index, int shift) => 
            BranchNodes<TKey, TValue>.ContainsKey(this, key, hash, index, shift);

        public ImmutableDictionary<TKey, TValue> Add<TValueNodes>(
            TKey key, TValue value, uint hash, uint index, uint indexValues, int shift, ulong bitmapNodes,
            ulong bitmapValues, TValueNodes values)
            where TValueNodes : struct, IValueNodes<TKey, TValue>
        {
            Debug.Assert(_node1 != null);
            Debug.Assert(_node2 != null);
            Debug.Assert(_node3 != null);
            Debug.Assert(_node4 != null);
            Debug.Assert(_node5 != null);

            return BranchNodes<TKey, TValue>.Add<BranchNodes5<TKey, TValue>, BranchNodes6<TKey, TValue>, TValueNodes>(
                this, key, value, hash, index, indexValues, shift, bitmapNodes, bitmapValues, values);
        }

        public ImmutableDictionary<TKey, TValue> DuplicateWith<TValueNodes>(
            TKey key, TValue value, uint hash, long index, int shift, ulong bitmapNodes, ulong bitmapValues, TValueNodes values)
            where TValueNodes : struct, IValueNodes<TKey, TValue>
        {
            Debug.Assert(_node1 != null);
            Debug.Assert(_node2 != null);
            Debug.Assert(_node3 != null);
            Debug.Assert(_node4 != null);
            Debug.Assert(_node5 != null);

            return BranchNodes<TKey, TValue>.Clone(this, key, value, hash, index, shift, bitmapNodes, bitmapValues, values);
        }

    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BranchNodes6<TKey, TValue> : IBranchNodes<TKey, TValue> where TKey : IEquatable<TKey>
    {
        private readonly ImmutableDictionary<TKey, TValue> _node1;
        private readonly ImmutableDictionary<TKey, TValue> _node2;
        private readonly ImmutableDictionary<TKey, TValue> _node3;
        private readonly ImmutableDictionary<TKey, TValue> _node4;
        private readonly ImmutableDictionary<TKey, TValue> _node5;
        private readonly ImmutableDictionary<TKey, TValue> _node6;

        public bool ContainsKey(TKey key, uint hash, int index, int shift) => 
            BranchNodes<TKey, TValue>.ContainsKey(this, key, hash, index, shift);

        public ImmutableDictionary<TKey, TValue> Add<TValueNodes>(
            TKey key, TValue value, uint hash, uint index, uint indexValues, int shift, ulong bitmapNodes,
            ulong bitmapValues, TValueNodes values)
            where TValueNodes : struct, IValueNodes<TKey, TValue>
        {
            Debug.Assert(_node1 != null);
            Debug.Assert(_node2 != null);
            Debug.Assert(_node3 != null);
            Debug.Assert(_node4 != null);
            Debug.Assert(_node5 != null);
            Debug.Assert(_node6 != null);

            return BranchNodes<TKey, TValue>.Add<BranchNodes6<TKey, TValue>, BranchNodes7<TKey, TValue>, TValueNodes>(
                this, key, value, hash, index, indexValues, shift, bitmapNodes, bitmapValues, values);
        }

        public ImmutableDictionary<TKey, TValue> DuplicateWith<TValueNodes>(
            TKey key, TValue value, uint hash, long index, int shift, ulong bitmapNodes, ulong bitmapValues, TValueNodes values)
            where TValueNodes : struct, IValueNodes<TKey, TValue>
        {
            Debug.Assert(_node1 != null);
            Debug.Assert(_node2 != null);
            Debug.Assert(_node3 != null);
            Debug.Assert(_node4 != null);
            Debug.Assert(_node5 != null);
            Debug.Assert(_node6 != null);

            return BranchNodes<TKey, TValue>.Clone(this, key, value, hash, index, shift, bitmapNodes, bitmapValues, values);
        }

    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BranchNodes7<TKey, TValue> : IBranchNodes<TKey, TValue> where TKey : IEquatable<TKey>
    {
        private readonly ImmutableDictionary<TKey, TValue> _node1;
        private readonly ImmutableDictionary<TKey, TValue> _node2;
        private readonly ImmutableDictionary<TKey, TValue> _node3;
        private readonly ImmutableDictionary<TKey, TValue> _node4;
        private readonly ImmutableDictionary<TKey, TValue> _node5;
        private readonly ImmutableDictionary<TKey, TValue> _node6;
        private readonly ImmutableDictionary<TKey, TValue> _node7;

        public bool ContainsKey(TKey key, uint hash, int index, int shift) => 
            BranchNodes<TKey, TValue>.ContainsKey(this, key, hash, index, shift);

        public ImmutableDictionary<TKey, TValue> Add<TValueNodes>(
            TKey key, TValue value, uint hash, uint index, uint indexValues, int shift, ulong bitmapNodes,
            ulong bitmapValues, TValueNodes values)
            where TValueNodes : struct, IValueNodes<TKey, TValue>
        {
            Debug.Assert(_node1 != null);
            Debug.Assert(_node2 != null);
            Debug.Assert(_node3 != null);
            Debug.Assert(_node4 != null);
            Debug.Assert(_node5 != null);
            Debug.Assert(_node6 != null);
            Debug.Assert(_node7 != null);

            return BranchNodes<TKey, TValue>.Add<BranchNodes7<TKey, TValue>, BranchNodes8<TKey, TValue>, TValueNodes>(
                this, key, value, hash, index, indexValues, shift, bitmapNodes, bitmapValues, values);
        }

        public ImmutableDictionary<TKey, TValue> DuplicateWith<TValueNodes>(
            TKey key, TValue value, uint hash, long index, int shift, ulong bitmapNodes, ulong bitmapValues, TValueNodes values)
            where TValueNodes : struct, IValueNodes<TKey, TValue>
        {
            Debug.Assert(_node1 != null);
            Debug.Assert(_node2 != null);
            Debug.Assert(_node3 != null);
            Debug.Assert(_node4 != null);
            Debug.Assert(_node5 != null);
            Debug.Assert(_node6 != null);
            Debug.Assert(_node7 != null);

            return BranchNodes<TKey, TValue>.Clone(this, key, value, hash, index, shift, bitmapNodes, bitmapValues, values);
        }

    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct BranchNodes8<TKey, TValue> : IBranchNodes<TKey, TValue> where TKey : IEquatable<TKey>
    {
        private readonly ImmutableDictionary<TKey, TValue> _node1;
        private readonly ImmutableDictionary<TKey, TValue> _node2;
        private readonly ImmutableDictionary<TKey, TValue> _node3;
        private readonly ImmutableDictionary<TKey, TValue> _node4;
        private readonly ImmutableDictionary<TKey, TValue> _node5;
        private readonly ImmutableDictionary<TKey, TValue> _node6;
        private readonly ImmutableDictionary<TKey, TValue> _node7;
        private readonly ImmutableDictionary<TKey, TValue> _node8;

        public bool ContainsKey(TKey key, uint hash, int index, int shift) => 
            BranchNodes<TKey, TValue>.ContainsKey(this, key, hash, index, shift);

        public ImmutableDictionary<TKey, TValue> Add<TValueNodes>(
            TKey key, TValue value, uint hash, uint index, uint indexValues, int shift, ulong bitmapNodes,
            ulong bitmapValues, TValueNodes values)
            where TValueNodes : struct, IValueNodes<TKey, TValue>
        {
            throw new NotImplementedException();
        }

        public ImmutableDictionary<TKey, TValue> DuplicateWith<TValueNodes>(
            TKey key, TValue value, uint hash, long index, int shift, ulong bitmapNodes, ulong bitmapValues, TValueNodes values)
            where TValueNodes : struct, IValueNodes<TKey, TValue>
        {
            return BranchNodes<TKey, TValue>.Clone(this, key, value, hash, index, shift, bitmapNodes, bitmapValues, values);
        }

    }
}