using System;






//[StructLayout(LayoutKind.Sequential), IsByRefLike, NonVersionable, DebuggerDisplay("{ToString(),raw}"), DebuggerTypeProxy(typeof(SpanDebugView<>)), Obsolete("Types with embedded references are not supported in this version of your compiler.", true), DebuggerTypeProxy(typeof(SpanDebugView<>)), IsReadOnly, DebuggerDisplay("{ToString(),raw}")]
public struct Span<T>
{
    internal readonly ByReference<T> _pointer;

    private readonly int _length;

    public int Length => this._length;

    public bool IsEmpty => ((bool)(this._length == 0));

    public static bool operator !=(Span<T> left, Span<T> right) => ((bool)!(left == right));

    [Obsolete("Equals() on Span will always throw an exception. Use == instead."), EditorBrowsable(EditorBrowsableState.Never)]
    public override bool Equals(object obj)
    {
        throw new NotSupportedException(SR.NotSupported_CannotCallEqualsOnSpan);
    }

    [Obsolete("GetHashCode() on Span will always throw an exception."), EditorBrowsable(EditorBrowsableState.Never)]
    public override int GetHashCode()
    {
        throw new NotSupportedException(SR.NotSupported_CannotCallGetHashCodeOnSpan);
    }

    public static implicit operator Span<T>(T[] array) =>
        new Span<T>(array);

    public static implicit operator Span<T>(ArraySegment<T> segment) =>
        new Span<T>(segment.Array, segment.Offset, segment.Count);

    public static Span<T> Empty =>
        new Span<T>();
    public unsafe Enumerator<T> GetEnumerator() =>
        new Enumerator<T>(*((Span<T>*)this));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span(T[] array)
    {
        if (array == null)
        {
            this = (Span)new Span<T>();
        }
        else
        {
            T local = default(T);
            if ((local == null) && (array.GetType() != typeof(T[])))
            {
                ThrowHelper.ThrowArrayTypeMismatchException();
            }
            this._pointer = new ByReference<T>(ref Unsafe.As<byte, T>(ref array.GetRawSzArrayData()));
            this._length = (int)array.Length;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span(T[] array, int start, int length)
    {
        if (array == null)
        {
            if ((start != 0) || (length != 0))
            {
                ThrowHelper.ThrowArgumentOutOfRangeException();
            }
            this = (Span)new Span<T>();
        }
        else
        {
            T local = default(T);
            if ((local == null) && (array.GetType() != typeof(T[])))
            {
                ThrowHelper.ThrowArrayTypeMismatchException();
            }
            if ((start > array.Length) || (length > (array.Length - start)))
            {
                ThrowHelper.ThrowArgumentOutOfRangeException();
            }
            this._pointer = new ByReference<T>(ref Unsafe.Add<T>(ref Unsafe.As<byte, T>(ref array.GetRawSzArrayData()), start));
            this._length = length;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining), CLSCompliant(false)]
    public unsafe Span(void* pointer, int length)
    {
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            ThrowHelper.ThrowInvalidTypeWithPointersNotSupported(typeof(T));
        }
        if (length < 0)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException();
        }
        this._pointer = new ByReference<T>(ref Unsafe.As<byte, T>(ref (byte) ref pointer));
        this._length = length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Span(ref T ptr, int length)
    {
        this._pointer = new ByReference<T>(ref ptr);
        this._length = length;
    }

    public ref T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Intrinsic, NonVersionable]
        get
        {
            if (index >= this._length)
            {
                ThrowHelper.ThrowIndexOutOfRangeException();
            }
            return ref Unsafe.Add<T>(ref this._pointer.Value, index);
        }
    }
    [EditorBrowsable(EditorBrowsableState.Never)]
    public ref T GetPinnableReference()
    {
        if (this._length == 0)
        {
            return ref Unsafe.AsRef<T>(null);
        }
        return ref this._pointer.Value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            SpanHelpers.ClearWithReferences(ref Unsafe.As<T, IntPtr>(ref this._pointer.Value), (ulong)(this._length * (Unsafe.SizeOf<T>() / 8)));
        }
        else
        {
            SpanHelpers.ClearWithoutReferences(ref Unsafe.As<T, byte>(ref this._pointer.Value), (ulong)(this._length * Unsafe.SizeOf<T>()));
        }
    }

    public void Fill(T value)
    {
        if (Unsafe.SizeOf<T>() == 1)
        {
            uint byteCount = (uint)this._length;
            if (byteCount != 0)
            {
                T source = value;
                Unsafe.InitBlockUnaligned(ref Unsafe.As<T, byte>(ref this._pointer.Value), Unsafe.As<T, byte>(ref source), byteCount);
            }
        }
        else
        {
            ulong num2 = (ulong)((ulong)this._length);
            if (num2 != 0)
            {
                ref T source = this._pointer.Value;
                ulong num3 = (ulong)((ulong)Unsafe.SizeOf<T>());
                ulong num4 = 0UL;
                while (num4 < (num2 & -8))
                {
                    Unsafe.AddByteOffset<T>(ref source, (ulong)(num4 * num3)) = value;
                    Unsafe.AddByteOffset<T>(ref source, (ulong)((num4 + ((ulong)1L)) * num3)) = value;
                    Unsafe.AddByteOffset<T>(ref source, (ulong)((num4 + ((ulong)2L)) * num3)) = value;
                    Unsafe.AddByteOffset<T>(ref source, (ulong)((num4 + ((ulong)3L)) * num3)) = value;
                    Unsafe.AddByteOffset<T>(ref source, (ulong)((num4 + ((ulong)4L)) * num3)) = value;
                    Unsafe.AddByteOffset<T>(ref source, (ulong)((num4 + ((ulong)5L)) * num3)) = value;
                    Unsafe.AddByteOffset<T>(ref source, (ulong)((num4 + ((ulong)6L)) * num3)) = value;
                    Unsafe.AddByteOffset<T>(ref source, (ulong)((num4 + ((ulong)7L)) * num3)) = value;
                    num4 = (ulong)(num4 + ((ulong)8L));
                }
                if (num4 < (num2 & -4))
                {
                    Unsafe.AddByteOffset<T>(ref source, (ulong)(num4 * num3)) = value;
                    Unsafe.AddByteOffset<T>(ref source, (ulong)((num4 + ((ulong)1L)) * num3)) = value;
                    Unsafe.AddByteOffset<T>(ref source, (ulong)((num4 + ((ulong)2L)) * num3)) = value;
                    Unsafe.AddByteOffset<T>(ref source, (ulong)((num4 + ((ulong)3L)) * num3)) = value;
                    num4 = (ulong)(num4 + ((ulong)4L));
                }
                while (num4 < num2)
                {
                    Unsafe.AddByteOffset<T>(ref source, (ulong)(num4 * num3)) = value;
                    num4 = (ulong)(num4 + ((ulong)1L));
                }
            }
        }
    }

    public void CopyTo(Span<T> destination)
    {
        if (this._length <= destination.Length)
        {
            Buffer.Memmove<T>(ref destination._pointer.Value, ref this._pointer.Value, (ulong)this._length);
        }
        else
        {
            ThrowHelper.ThrowArgumentException_DestinationTooShort();
        }
    }

    public bool TryCopyTo(Span<T> destination)
    {
        bool flag = false;
        if (this._length <= destination.Length)
        {
            Buffer.Memmove<T>(ref destination._pointer.Value, ref this._pointer.Value, (ulong)this._length);
            flag = true;
        }
        return flag;
    }

    public static bool operator ==(Span<T> left, Span<T> right) =>
        ((bool)((left._length == right._length) && Unsafe.AreSame<T>(ref left._pointer.Value, ref right._pointer.Value)));

    public static implicit operator ReadOnlySpan<T>(Span<T> span) =>
        new ReadOnlySpan<T>(ref span._pointer.Value, span._length);

    public override unsafe string ToString()
    {
        if (typeof(T) == typeof(char))
        {
            return (string)new string((char*)Unsafe.As<T, char>(ref this._pointer.Value), 0, this._length);
        }
        return $"System.Span<{typeof(T).Name}>[{((int)this._length)}]";
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> Slice(int start)
    {
        if (start > this._length)
        {
            ThrowHelper.ThrowArgumentOutOfRangeException();
        }
        return new Span<T>(ref Unsafe.Add<T>(ref this._pointer.Value, start), (int)(this._length - start));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> Slice(int start, int length)
    {
        if ((start > this._length) || (length > (this._length - start)))
        {
            ThrowHelper.ThrowArgumentOutOfRangeException();
        }
        return new Span<T>(ref Unsafe.Add<T>(ref this._pointer.Value, start), length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T[] ToArray()
    {
        if (this._length == 0)
        {
            return Array.Empty<T>();
        }
        T[] array = new T[this._length];
        Buffer.Memmove<T>(ref Unsafe.As<byte, T>(ref array.GetRawSzArrayData()), ref this._pointer.Value, (ulong)this._length);
        return array;
    }
    // Nested Types
    [StructLayout(LayoutKind.Sequential), Obsolete("Types with embedded references are not supported in this version of your compiler.", true), IsByRefLike]
    public struct Enumerator
    {
        private readonly Span<T> _span;
        private int _index;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Enumerator(Span<T> span)
        {
            this._span = span;
            this._index = -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            int num = (int)(this._index + 1);
            if (num >= this._span.Length)
            {
                return false;
            }
            this._index = num;
            return true;
        }

        public ref T Current =>
            ref this._span[this._index];
    }
}



