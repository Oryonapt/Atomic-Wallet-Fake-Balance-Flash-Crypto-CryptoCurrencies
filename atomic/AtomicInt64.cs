using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;



namespace SCB.Atomics
{
    using ASC = AtomicSupportClass;


#pragma warning disable CS9107
#pragma warning disable CS0660
#pragma warning disable CS0661
    unsafe sealed class AtomicInt64( long value ) : AtomicNumericsBase<long>( value ), IAtomicBaseOperations<long>
    {
        ///-------Interface-methods-Start------------///

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public long Increment( in long value )
        {
            if ( IsExtended )
            {
                return Interlocked.Add( ref StorageEx->lAtomic, value );
            } else
            {
                return Interlocked.Add( ref Storage->lAtomic, value );
            }
        }


        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public long Decrement( in long value )
        {
            if ( IsExtended )
            {
                return Interlocked.Add( ref StorageEx->lAtomic, -value );
            } else
            {
                return Interlocked.Add( ref Storage->lAtomic, -value );
            }
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public long CompareExchange( in long value, in long comparand )
        {
            if ( IsExtended )
            {
                return Interlocked.CompareExchange( ref StorageEx->lAtomic, value, comparand );
            } else
            {
                return Interlocked.CompareExchange( ref Storage->lAtomic, value, comparand );
            }
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public long Read()
        {
            if ( IsExtended && RefCount > ContentionThreshold )
            {
                using var sL = ScopeLock();
                return Unsafe.Read<long>( &StorageEx->lAtomic );
            } else if ( IsExtended )
            {
                return Interlocked.Read( ref StorageEx->lAtomic );
            } else
            {
                return Interlocked.Read( ref Storage->lAtomic );
            }
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public void Write( in long value )
        {
            if ( IsExtended && RefCount > ContentionThreshold )
            {
                using var sL = ScopeLock();
                Unsafe.Write( &StorageEx->lAtomic, value );
            } else if ( IsExtended )
            {
                _ = Interlocked.Exchange( ref StorageEx->lAtomic, value );
            } else
            {
                _ = Interlocked.Exchange( ref Storage->lAtomic, value );
            }
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public long Supplant( [Optional] in long value, [Optional] in int rsValue, ASC.AtomicOperation aO )
        {
            if ( aO == ASC.AtomicOperation.BitShiftL || aO == ASC.AtomicOperation.BitShiftR ||
                aO == ASC.AtomicOperation.RotateLeft || aO == ASC.AtomicOperation.RotateRight )
            {
                return long.MaxValue;
            }

            long result = aO switch
            {
                ASC.AtomicOperation.Addition => ASC.Add( Read(), value ),
                ASC.AtomicOperation.Subtraction => ASC.Subtract( Read(), value ),
                ASC.AtomicOperation.Multiplication => ASC.Multiply( Read(), value ),
                ASC.AtomicOperation.Division => ASC.Divide( Read(), value ),
                ASC.AtomicOperation.Modulus => ASC.Modulus( Read(), value ),
                ASC.AtomicOperation.BitwiseAnd => ASC.BitwiseAnd( Read(), value ),
                ASC.AtomicOperation.BitwiseOr => ASC.BitwiseOr( Read(), value ),
                ASC.AtomicOperation.BitwiseXor => ASC.BitwiseXor( Read(), value ),
                ASC.AtomicOperation.BitwiseNot => ASC.BitwiseNot( Read() ),
                _ => throw new ArgumentOutOfRangeException( nameof( aO ) ),
            };

            Write( result );
            return result;
        }

        ///------------Interface-methods-End--------------/// 


        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ScopedAtomicRef<AtomicInt64, long> GetScopedReference()
        {
            return new ScopedAtomicRef<AtomicInt64, long>( this );
        }


        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public long Add( in long value )
        {
            return Increment( value );
        }


        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public long Subtract( in long value )
        {
            return Decrement( value );
        }


        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public long Multiply( in long value )
        {
            return Supplant( value, aO: ASC.AtomicOperation.Multiplication );
        }


        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public long Divide( in long value )
        {
            return Supplant( value, aO: ASC.AtomicOperation.Division );
        }


        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public long Modulus( in long value )
        {
            return Supplant( value, aO: ASC.AtomicOperation.Modulus );
        }


        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public long Max( in long value )
        {
            return long.Max( Read(), value );
        }


        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public long Min( in long value )
        {
            return long.Min( Read(), value );
        }


        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public long And( in long value )
        {
            return Supplant( value, aO: ASC.AtomicOperation.BitwiseAnd );
        }


        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public long Or( in long value )
        {
            return Supplant( value, aO: ASC.AtomicOperation.BitwiseOr );
        }


        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public long Xor( in long value )
        {
            return Supplant( value, aO: ASC.AtomicOperation.BitwiseXor );
        }


        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public long Not()
        {
            return Supplant( aO: ASC.AtomicOperation.BitwiseNot );
        }


        // Overload operators

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static implicit operator AtomicInt64( long value )
        {
            return new AtomicInt64( value );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator ==( AtomicInt64 atomic, long value )
        {
            return atomic.Read() == value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator !=( AtomicInt64 atomic, long value )
        {
            return atomic.Read() != value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator ==( AtomicInt64 atomic1, AtomicInt64 atomic2 )
        {
            return atomic1.Read() == atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator !=( AtomicInt64 atomic1, AtomicInt64 atomic2 )
        {
            return atomic1.Read() != atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator ==( long value, AtomicInt64 atomic )
        {
            return value == atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator !=( long value, AtomicInt64 atomic )
        {
            return value != atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator >( AtomicInt64 atomic, long value )
        {
            return atomic.Read() > value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator <( AtomicInt64 atomic, long value )
        {
            return atomic.Read() < value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator >( long value, AtomicInt64 atomic )
        {
            return value > atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator <( long value, AtomicInt64 atomic )
        {
            return value < atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator >( AtomicInt64 atomic1, AtomicInt64 atomic2 )
        {
            return atomic1.Read() > atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator <( AtomicInt64 atomic1, AtomicInt64 atomic2 )
        {
            return atomic1.Read() < atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator >=( AtomicInt64 atomic, long value )
        {
            return atomic.Read() >= value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator <=( AtomicInt64 atomic, long value )
        {
            return atomic.Read() <= value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator >=( long value, AtomicInt64 atomic )
        {
            return value >= atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator <=( long value, AtomicInt64 atomic )
        {
            return value <= atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator >=( AtomicInt64 atomic1, AtomicInt64 atomic2 )
        {
            return atomic1.Read() >= atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator <=( AtomicInt64 atomic1, AtomicInt64 atomic2 )
        {
            return atomic1.Read() <= atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static long operator +( AtomicInt64 atomic, long value )
        {
            return atomic.Read() + value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static long operator +( long value, AtomicInt64 atomic )
        {
            return value + atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static long operator +( AtomicInt64 atomic1, AtomicInt64 atomic2 )
        {
            return atomic1.Read() + atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static long operator -( AtomicInt64 atomic, long value )
        {
            return atomic.Read() - value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static long operator -( long value, AtomicInt64 atomic )
        {
            return value - atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static long operator -( AtomicInt64 atomic1, AtomicInt64 atomic2 )
        {
            return atomic1.Read() - atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static long operator *( AtomicInt64 atomic, long value )
        {
            return atomic.Read() * value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static long operator *( long value, AtomicInt64 atomic )
        {
            return value * atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static long operator *( AtomicInt64 atomic1, AtomicInt64 atomic2 )
        {
            return atomic1.Read() * atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static long operator /( AtomicInt64 atomic, long value )
        {
            return atomic.Read() / value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static long operator /( long value, AtomicInt64 atomic )
        {
            return value / atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static long operator /( AtomicInt64 atomic1, AtomicInt64 atomic2 )
        {
            return atomic1.Read() / atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static long operator %( AtomicInt64 atomic, long value )
        {
            return atomic.Read() % value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static long operator %( long value, AtomicInt64 atomic )
        {
            return value % atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static long operator %( AtomicInt64 atomic1, AtomicInt64 atomic2 )
        {
            return atomic1.Read() % atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static long operator &( AtomicInt64 atomic, long value )
        {
            return atomic.Read() & value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static long operator &( long value, AtomicInt64 atomic )
        {
            return value & atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static long operator &( AtomicInt64 atomic1, AtomicInt64 atomic2 )
        {
            return atomic1.Read() & atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static long operator |( AtomicInt64 atomic, long value )
        {
            return atomic.Read() | value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static long operator |( long value, AtomicInt64 atomic )
        {
            return value | atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static long operator |( AtomicInt64 atomic1, AtomicInt64 atomic2 )
        {
            return atomic1.Read() | atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static long operator ^( AtomicInt64 atomic, long value )
        {
            return atomic.Read() ^ value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static long operator ^( long value, AtomicInt64 atomic )
        {
            return value ^ atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static long operator ^( AtomicInt64 atomic1, AtomicInt64 atomic2 )
        {
            return atomic1.Read() ^ atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static long operator ~( AtomicInt64 atomic )
        {
            return atomic.Not();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static long operator <<( AtomicInt64 atomic, int value )
        {
            return atomic.Read() << value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static long operator >>( AtomicInt64 atomic, int value )
        {
            return atomic.Read() >> value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static AtomicInt64 operator ++( AtomicInt64 atomic )
        {
            atomic.Increment( 1L );
            return atomic;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static AtomicInt64 operator --( AtomicInt64 atomic )
        {
            atomic.Decrement( 1L );
            return atomic;
        }
    }



    sealed unsafe class AtomicUint64( ulong value ) : AtomicNumericsBase<ulong>( value ), IAtomicBaseOperations<ulong>
    {

        ///-------Interface-methods-Start------------///

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ulong Increment( in ulong value )
        {
            if ( IsExtended )
            {
                return Interlocked.Add( ref StorageEx->ulAtomic, value );
            } else
            {
                return Interlocked.Add( ref Storage->ulAtomic, value );
            }
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ulong Decrement( in ulong value )
        {
            return Supplant( value, aO: ASC.AtomicOperation.Subtraction );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]

        public ulong CompareExchange( in ulong value, in ulong comparand )
        {
            if ( IsExtended )
            {
                return Interlocked.CompareExchange( ref StorageEx->ulAtomic, value, comparand );
            } else
            {
                return Interlocked.CompareExchange( ref Storage->ulAtomic, value, comparand );
            }
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ulong Read()
        {
            if ( IsExtended && RefCount > ContentionThreshold )
            {
                using var sL = ScopeLock();
                return Unsafe.Read<ulong>( &StorageEx->ulAtomic );
            } else if ( IsExtended )
            {
                return Interlocked.Read( ref StorageEx->ulAtomic );
            } else
            {
                return Interlocked.Read( ref Storage->ulAtomic );
            }
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public void Write( in ulong value )
        {
            if ( IsExtended && RefCount > ContentionThreshold )
            {
                using var sL = ScopeLock();
                Unsafe.Write( &StorageEx->ulAtomic, value );
            } else if ( IsExtended )
            {
                _ = Interlocked.Exchange( ref StorageEx->ulAtomic, value );
            } else
            {
                _ = Interlocked.Exchange( ref Storage->ulAtomic, value );
            }
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ulong Supplant( [Optional] in ulong value, [Optional] in int rsValue, ASC.AtomicOperation aO )
        {
            ulong result = aO switch
            {
                ASC.AtomicOperation.Addition => ASC.Add( Read(), value ),
                ASC.AtomicOperation.Subtraction => ASC.Subtract( Read(), value ),
                ASC.AtomicOperation.Multiplication => ASC.Multiply( Read(), value ),
                ASC.AtomicOperation.Division => ASC.Divide( Read(), value ),
                ASC.AtomicOperation.Modulus => ASC.Modulus( Read(), value ),
                ASC.AtomicOperation.BitwiseAnd => ASC.BitwiseAnd( Read(), value ),
                ASC.AtomicOperation.BitwiseOr => ASC.BitwiseOr( Read(), value ),
                ASC.AtomicOperation.BitwiseXor => ASC.BitwiseXor( Read(), value ),
                ASC.AtomicOperation.BitwiseNot => ASC.BitwiseNot( Read() ),
                ASC.AtomicOperation.BitShiftL => ASC.LeftShift( Read(), rsValue ),
                ASC.AtomicOperation.BitShiftR => ASC.RightShift( Read(), rsValue ),
                ASC.AtomicOperation.RotateLeft => ASC.RotateLeft( Read(), rsValue ),
                ASC.AtomicOperation.RotateRight => ASC.RotateRight( Read(), rsValue ),
                _ => throw new ArgumentOutOfRangeException( nameof( aO ) ),
            };

            Write( result );
            return result;
        }



        ///------------Interface-methods-End--------------///

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ScopedAtomicRef<AtomicUint64, ulong> GetScopedReference()
        {
            return new ScopedAtomicRef<AtomicUint64, ulong>( this );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ulong Add( ulong value )
        {
            return Increment( value );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ulong Subtract( ulong value )
        {
            return Decrement( value );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ulong Multiply( ulong value )
        {
            return Supplant( value, aO: ASC.AtomicOperation.Multiplication );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ulong Divide( ulong value )
        {
            return Supplant( value, aO: ASC.AtomicOperation.Division );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ulong Modulus( ulong value )
        {
            return Supplant( value, aO: ASC.AtomicOperation.Modulus );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ulong Max( ulong value )
        {
            return ulong.Max( Read(), value );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ulong Min( ulong value )
        {
            return ulong.Min( Read(), value );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ulong And( ulong value )
        {
            return Supplant( value, aO: ASC.AtomicOperation.BitwiseAnd );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ulong Or( ulong value )
        {
            return Supplant( value, aO: ASC.AtomicOperation.BitwiseOr );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ulong Xor( ulong value )
        {
            return Supplant( value, aO: ASC.AtomicOperation.BitwiseXor );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ulong Not()
        {
            return Supplant( aO: ASC.AtomicOperation.BitwiseNot );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ulong LeftShift( int value )
        {
            return Supplant( rsValue: value, aO: ASC.AtomicOperation.BitShiftL );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ulong RightShift( int value )
        {
            return Supplant( rsValue: value, aO: ASC.AtomicOperation.BitShiftR );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ulong RightRotate( int value )
        {
            return Supplant( rsValue: value, aO: ASC.AtomicOperation.RotateRight );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ulong LeftRotate( int value )
        {
            return Supplant( rsValue: value, aO: ASC.AtomicOperation.RotateLeft );
        }


        // Overload operators

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static implicit operator AtomicUint64( ulong value )
        {
            return new AtomicUint64( value );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator ==( AtomicUint64 atomic, ulong value )
        {
            return atomic.Read() == value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator !=( AtomicUint64 atomic, ulong value )
        {
            return atomic.Read() != value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator ==( AtomicUint64 atomic1, AtomicUint64 atomic2 )
        {
            return atomic1.Read() == atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator !=( AtomicUint64 atomic1, AtomicUint64 atomic2 )
        {
            return atomic1.Read() != atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator ==( ulong value, AtomicUint64 atomic )
        {
            return value == atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator !=( ulong value, AtomicUint64 atomic )
        {
            return value != atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator >( AtomicUint64 atomic, ulong value )
        {
            return atomic.Read() > value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator <( AtomicUint64 atomic, ulong value )
        {
            return atomic.Read() < value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator >( ulong value, AtomicUint64 atomic )
        {
            return value > atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator <( ulong value, AtomicUint64 atomic )
        {
            return value < atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator >( AtomicUint64 atomic1, AtomicUint64 atomic2 )
        {
            return atomic1.Read() > atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator <( AtomicUint64 atomic1, AtomicUint64 atomic2 )
        {
            return atomic1.Read() < atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator >=( AtomicUint64 atomic, ulong value )
        {
            return atomic.Read() >= value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator <=( AtomicUint64 atomic, ulong value )
        {
            return atomic.Read() <= value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator >=( ulong value, AtomicUint64 atomic )
        {
            return value >= atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator <=( ulong value, AtomicUint64 atomic )
        {
            return value <= atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator >=( AtomicUint64 atomic1, AtomicUint64 atomic2 )
        {
            return atomic1.Read() >= atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator <=( AtomicUint64 atomic1, AtomicUint64 atomic2 )
        {
            return atomic1.Read() <= atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static ulong operator +( AtomicUint64 atomic, ulong value )
        {
            return atomic.Read() + value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static ulong operator +( ulong value, AtomicUint64 atomic )
        {
            return value + atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static ulong operator +( AtomicUint64 atomic1, AtomicUint64 atomic2 )
        {
            return atomic1.Read() + atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static ulong operator -( AtomicUint64 atomic, ulong value )
        {
            return atomic.Read() - value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static ulong operator -( ulong value, AtomicUint64 atomic )
        {
            return value - atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static ulong operator -( AtomicUint64 atomic1, AtomicUint64 atomic2 )
        {
            return atomic1.Read() - atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static ulong operator *( AtomicUint64 atomic, ulong value )
        {
            return atomic.Read() * value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static ulong operator *( ulong value, AtomicUint64 atomic )
        {
            return value * atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static ulong operator *( AtomicUint64 atomic1, AtomicUint64 atomic2 )
        {
            return atomic1.Read() * atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static ulong operator /( AtomicUint64 atomic, ulong value )
        {
            return atomic.Read() / value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static ulong operator /( ulong value, AtomicUint64 atomic )
        {
            return value / atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static ulong operator /( AtomicUint64 atomic1, AtomicUint64 atomic2 )
        {
            return atomic1.Read() / atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static ulong operator %( AtomicUint64 atomic, ulong value )
        {
            return atomic.Read() % value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static ulong operator %( ulong value, AtomicUint64 atomic )
        {
            return value % atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static ulong operator %( AtomicUint64 atomic1, AtomicUint64 atomic2 )
        {
            return atomic1.Read() % atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static ulong operator &( AtomicUint64 atomic, ulong value )
        {
            return atomic.Read() & value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static ulong operator &( ulong value, AtomicUint64 atomic )
        {
            return value & atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static ulong operator &( AtomicUint64 atomic1, AtomicUint64 atomic2 )
        {
            return atomic1.Read() & atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static ulong operator |( AtomicUint64 atomic, ulong value )
        {
            return atomic.Read() | value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static ulong operator |( ulong value, AtomicUint64 atomic )
        {
            return value | atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static ulong operator |( AtomicUint64 atomic1, AtomicUint64 atomic2 )
        {
            return atomic1.Read() | atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static ulong operator ^( AtomicUint64 atomic, ulong value )
        {
            return atomic.Read() ^ value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static ulong operator ^( ulong value, AtomicUint64 atomic )
        {
            return value ^ atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static ulong operator ^( AtomicUint64 atomic1, AtomicUint64 atomic2 )
        {
            return atomic1.Read() ^ atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static ulong operator ~( AtomicUint64 atomic )
        {
            return atomic.Not();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static ulong operator <<( AtomicUint64 atomic, int value )
        {
            return atomic.Read() << value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static ulong operator >>( AtomicUint64 atomic, int value )
        {
            return atomic.Read() >> value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static AtomicUint64 operator ++( AtomicUint64 atomic )
        {
            atomic.Increment( 1UL );
            return atomic;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static AtomicUint64 operator --( AtomicUint64 atomic )
        {
            atomic.Decrement( 1UL );
            return atomic;
        }
    }

#pragma warning restore CS9107
#pragma warning restore CS0660
#pragma warning restore CS0661
}
