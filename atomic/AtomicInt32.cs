using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SCB.Atomics
{
    using ASC = AtomicSupportClass;


#pragma warning disable CS9107
#pragma warning disable CS0660
#pragma warning disable CS0661

    unsafe sealed class AtomicInt32( int value ) : AtomicNumericsBase<int>( value ), IAtomicBaseOperations<int>
    {
        ///-------Interface-methods-Start------------///

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public int Increment( in int value )
        {
            if ( IsExtended )
            {
                return ASC.LongToInt( Interlocked.Add( ref StorageEx->lAtomic, value ) );
            } else
            {
                return ASC.LongToInt( Interlocked.Add( ref Storage->lAtomic, value ) );
            }
        }


        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public int Decrement( in int value )
        {
            if ( IsExtended )
            {
                return ASC.LongToInt( Interlocked.Add( ref StorageEx->lAtomic, -value ) );
            } else
            {
                return ASC.LongToInt( Interlocked.Add( ref Storage->lAtomic, -value ) );
            }
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public int CompareExchange( in int value, in int comparand )
        {
            if ( IsExtended )
            {
                return ASC.LongToInt( Interlocked.CompareExchange( ref StorageEx->lAtomic, value, comparand ) );
            } else
            {
                return ASC.LongToInt( Interlocked.CompareExchange( ref Storage->lAtomic, value, comparand ) );
            }
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public int Read()
        {
            if ( IsExtended && RefCount > ContentionThreshold )
            {
                using var sL = ScopeLock();
                return ASC.LongToInt( Unsafe.Read<int>( &StorageEx->lAtomic ) );
            } else if ( IsExtended )
            {
                return ASC.LongToInt( Interlocked.Read( ref StorageEx->lAtomic ) );
            } else
            {
                return ASC.LongToInt( Interlocked.Read( ref Storage->lAtomic ) );
            }
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public void Write( in int value )
        {
            if ( IsExtended && RefCount > ContentionThreshold )
            {
                using var sL = ScopeLock();
                Unsafe.Write( &StorageEx->lAtomic, ASC.IntToLong( value ) );
            } else if ( IsExtended )
            {
                _ = Interlocked.Exchange( ref StorageEx->lAtomic, value );
            } else
            {
                _ = Interlocked.Exchange( ref Storage->lAtomic, value );
            }
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public int Supplant( [Optional] in int value, [Optional] in int rsValue, ASC.AtomicOperation aO )
        {
            if ( aO == ASC.AtomicOperation.BitShiftL || aO == ASC.AtomicOperation.BitShiftR ||
                aO == ASC.AtomicOperation.RotateLeft || aO == ASC.AtomicOperation.RotateRight )
            {
                return int.MaxValue;
            }

            int result = aO switch
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
        public ScopedAtomicRef<AtomicInt32, int> GetScopedReference()
        {
            return new ScopedAtomicRef<AtomicInt32, int>( this );
        }


        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public int Add( in int value )
        {
            return Increment( value );
        }


        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public int Subtract( in int value )
        {
            return Decrement( value );
        }


        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public int Multiply( in int value )
        {
            return Supplant( value, aO: ASC.AtomicOperation.Multiplication );
        }


        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public int Divide( in int value )
        {
            return Supplant( value, aO: ASC.AtomicOperation.Division );
        }


        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public int Modulus( in int value )
        {
            return Supplant( value, aO: ASC.AtomicOperation.Modulus );
        }


        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public int Max( in int value )
        {
            return int.Max( Read(), value );
        }


        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public int Min( in int value )
        {
            return int.Min( Read(), value );
        }


        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public int And( in int value )
        {
            return Supplant( value, aO: ASC.AtomicOperation.BitwiseAnd );
        }


        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public int Or( in int value )
        {
            return Supplant( value, aO: ASC.AtomicOperation.BitwiseOr );
        }


        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public int Xor( in int value )
        {
            return Supplant( value, aO: ASC.AtomicOperation.BitwiseXor );
        }


        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public int Not()
        {
            return Supplant( aO: ASC.AtomicOperation.BitwiseNot );
        }


        // Overload operators

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static implicit operator AtomicInt32( int value )
        {
            return new AtomicInt32( value );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator ==( AtomicInt32 atomic, int value )
        {
            return atomic.Read() == value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator !=( AtomicInt32 atomic, int value )
        {
            return atomic.Read() != value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator ==( AtomicInt32 atomic1, AtomicInt32 atomic2 )
        {
            return atomic1.Read() == atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator !=( AtomicInt32 atomic1, AtomicInt32 atomic2 )
        {
            return atomic1.Read() != atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator ==( int value, AtomicInt32 atomic )
        {
            return value == atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator !=( int value, AtomicInt32 atomic )
        {
            return value != atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator >( AtomicInt32 atomic, int value )
        {
            return atomic.Read() > value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator <( AtomicInt32 atomic, int value )
        {
            return atomic.Read() < value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator >( int value, AtomicInt32 atomic )
        {
            return value > atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator <( int value, AtomicInt32 atomic )
        {
            return value < atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator >( AtomicInt32 atomic1, AtomicInt32 atomic2 )
        {
            return atomic1.Read() > atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator <( AtomicInt32 atomic1, AtomicInt32 atomic2 )
        {
            return atomic1.Read() < atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator >=( AtomicInt32 atomic, int value )
        {
            return atomic.Read() >= value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator <=( AtomicInt32 atomic, int value )
        {
            return atomic.Read() <= value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator >=( int value, AtomicInt32 atomic )
        {
            return value >= atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator <=( int value, AtomicInt32 atomic )
        {
            return value <= atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator >=( AtomicInt32 atomic1, AtomicInt32 atomic2 )
        {
            return atomic1.Read() >= atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator <=( AtomicInt32 atomic1, AtomicInt32 atomic2 )
        {
            return atomic1.Read() <= atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static int operator +( AtomicInt32 atomic, int value )
        {
            return atomic.Read() + value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static int operator +( int value, AtomicInt32 atomic )
        {
            return value + atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static int operator +( AtomicInt32 atomic1, AtomicInt32 atomic2 )
        {
            return atomic1.Read() + atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static int operator -( AtomicInt32 atomic, int value )
        {
            return atomic.Read() - value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static int operator -( int value, AtomicInt32 atomic )
        {
            return value - atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static int operator -( AtomicInt32 atomic1, AtomicInt32 atomic2 )
        {
            return atomic1.Read() - atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static int operator *( AtomicInt32 atomic, int value )
        {
            return atomic.Read() * value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static int operator *( int value, AtomicInt32 atomic )
        {
            return value * atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static int operator *( AtomicInt32 atomic1, AtomicInt32 atomic2 )
        {
            return atomic1.Read() * atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static int operator /( AtomicInt32 atomic, int value )
        {
            return atomic.Read() / value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static int operator /( int value, AtomicInt32 atomic )
        {
            return value / atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static int operator /( AtomicInt32 atomic1, AtomicInt32 atomic2 )
        {
            return atomic1.Read() / atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static int operator %( AtomicInt32 atomic, int value )
        {
            return atomic.Read() % value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static int operator %( int value, AtomicInt32 atomic )
        {
            return value % atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static int operator %( AtomicInt32 atomic1, AtomicInt32 atomic2 )
        {
            return atomic1.Read() % atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static int operator &( AtomicInt32 atomic, int value )
        {
            return atomic.Read() & value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static int operator &( int value, AtomicInt32 atomic )
        {
            return value & atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static int operator &( AtomicInt32 atomic1, AtomicInt32 atomic2 )
        {
            return atomic1.Read() & atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static int operator |( AtomicInt32 atomic, int value )
        {
            return atomic.Read() | value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static int operator |( int value, AtomicInt32 atomic )
        {
            return value | atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static int operator |( AtomicInt32 atomic1, AtomicInt32 atomic2 )
        {
            return atomic1.Read() | atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static int operator ^( AtomicInt32 atomic, int value )
        {
            return atomic.Read() ^ value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static int operator ^( int value, AtomicInt32 atomic )
        {
            return value ^ atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static int operator ^( AtomicInt32 atomic1, AtomicInt32 atomic2 )
        {
            return atomic1.Read() ^ atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static int operator ~( AtomicInt32 atomic )
        {
            return atomic.Not();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static int operator <<( AtomicInt32 atomic, int value )
        {
            return atomic.Read() << value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static int operator >>( AtomicInt32 atomic, int value )
        {
            return atomic.Read() >> value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static AtomicInt32 operator ++( AtomicInt32 atomic )
        {
            atomic.Increment( 1 );
            return atomic;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static AtomicInt32 operator --( AtomicInt32 atomic )
        {
            atomic.Decrement( 1 );
            return atomic;
        }
    }





    sealed unsafe class AtomicUint32( uint value ) : AtomicNumericsBase<uint>( value ), IAtomicBaseOperations<uint>
    {
        ///-------Interface-methods-Start------------///

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public uint Increment( in uint value )
        {
            if ( IsExtended )
            {
                return ASC.UlongToUint( Interlocked.Add( ref StorageEx->ulAtomic, value ) );
            } else
            {
                return ASC.UlongToUint( Interlocked.Add( ref Storage->ulAtomic, value ) );
            }
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public uint Decrement( in uint value )
        {
            return Supplant( value, aO: ASC.AtomicOperation.Subtraction );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]

        public uint CompareExchange( in uint value, in uint comparand )
        {
            if ( IsExtended )
            {
                return ASC.UlongToUint( Interlocked.CompareExchange( ref StorageEx->ulAtomic, value, comparand ) );
            } else
            {
                return ASC.UlongToUint( Interlocked.CompareExchange( ref Storage->ulAtomic, value, comparand ) );
            }
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public uint Read()
        {
            if ( IsExtended && RefCount > ContentionThreshold )
            {
                using var sL = ScopeLock();
                return ASC.UlongToUint( Unsafe.Read<ulong>( &StorageEx->ulAtomic ) );
            } else if ( IsExtended )
            {
                return ASC.UlongToUint( Interlocked.Read( ref StorageEx->ulAtomic ) );
            } else
            {
                return ASC.UlongToUint( Interlocked.Read( ref Storage->ulAtomic ) );
            }
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public void Write( in uint value )
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
        public uint Supplant( [Optional] in uint value, [Optional] in int rsValue, ASC.AtomicOperation aO )
        {
            uint result = aO switch
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
        public ScopedAtomicRef<AtomicUint32, uint> GetScopedReference()
        {
            return new ScopedAtomicRef<AtomicUint32, uint>( this );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public uint Add( uint value )
        {
            return Increment( value );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public uint Subtract( uint value )
        {
            return Decrement( value );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public uint Multiply( uint value )
        {
            return Supplant( value, aO: ASC.AtomicOperation.Multiplication );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public uint Divide( uint value )
        {
            return Supplant( value, aO: ASC.AtomicOperation.Division );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public uint Modulus( uint value )
        {
            return Supplant( value, aO: ASC.AtomicOperation.Modulus );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public uint Max( uint value )
        {
            return uint.Max( Read(), value );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public uint Min( uint value )
        {
            return uint.Min( Read(), value );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public uint And( uint value )
        {
            return Supplant( value, aO: ASC.AtomicOperation.BitwiseAnd );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public uint Or( uint value )
        {
            return Supplant( value, aO: ASC.AtomicOperation.BitwiseOr );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public uint Xor( uint value )
        {
            return Supplant( value, aO: ASC.AtomicOperation.BitwiseXor );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public uint Not()
        {
            return Supplant( aO: ASC.AtomicOperation.BitwiseNot );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public uint LeftShift( int value )
        {
            return Supplant( rsValue: value, aO: ASC.AtomicOperation.BitShiftL );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public uint RightShift( int value )
        {
            return Supplant( rsValue: value, aO: ASC.AtomicOperation.BitShiftR );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public uint RightRotate( int value )
        {
            return Supplant( rsValue: value, aO: ASC.AtomicOperation.RotateRight );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public uint LeftRotate( int value )
        {
            return Supplant( rsValue: value, aO: ASC.AtomicOperation.RotateLeft );
        }


        // Overload operators

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static implicit operator AtomicUint32( uint value )
        {
            return new AtomicUint32( value );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator ==( AtomicUint32 atomic, uint value )
        {
            return atomic.Read() == value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator !=( AtomicUint32 atomic, uint value )
        {
            return atomic.Read() != value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator ==( AtomicUint32 atomic1, AtomicUint32 atomic2 )
        {
            return atomic1.Read() == atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator !=( AtomicUint32 atomic1, AtomicUint32 atomic2 )
        {
            return atomic1.Read() != atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator ==( uint value, AtomicUint32 atomic )
        {
            return value == atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator !=( uint value, AtomicUint32 atomic )
        {
            return value != atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator >( AtomicUint32 atomic, uint value )
        {
            return atomic.Read() > value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator <( AtomicUint32 atomic, uint value )
        {
            return atomic.Read() < value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator >( uint value, AtomicUint32 atomic )
        {
            return value > atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator <( uint value, AtomicUint32 atomic )
        {
            return value < atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator >( AtomicUint32 atomic1, AtomicUint32 atomic2 )
        {
            return atomic1.Read() > atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator <( AtomicUint32 atomic1, AtomicUint32 atomic2 )
        {
            return atomic1.Read() < atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator >=( AtomicUint32 atomic, uint value )
        {
            return atomic.Read() >= value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator <=( AtomicUint32 atomic, uint value )
        {
            return atomic.Read() <= value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator >=( uint value, AtomicUint32 atomic )
        {
            return value >= atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator <=( uint value, AtomicUint32 atomic )
        {
            return value <= atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator >=( AtomicUint32 atomic1, AtomicUint32 atomic2 )
        {
            return atomic1.Read() >= atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator <=( AtomicUint32 atomic1, AtomicUint32 atomic2 )
        {
            return atomic1.Read() <= atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static uint operator +( AtomicUint32 atomic, uint value )
        {
            return atomic.Read() + value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static uint operator +( uint value, AtomicUint32 atomic )
        {
            return value + atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static uint operator +( AtomicUint32 atomic1, AtomicUint32 atomic2 )
        {
            return atomic1.Read() + atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static uint operator -( AtomicUint32 atomic, uint value )
        {
            return atomic.Read() - value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static uint operator -( uint value, AtomicUint32 atomic )
        {
            return value - atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static uint operator -( AtomicUint32 atomic1, AtomicUint32 atomic2 )
        {
            return atomic1.Read() - atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static uint operator *( AtomicUint32 atomic, uint value )
        {
            return atomic.Read() * value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static uint operator *( uint value, AtomicUint32 atomic )
        {
            return value * atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static uint operator *( AtomicUint32 atomic1, AtomicUint32 atomic2 )
        {
            return atomic1.Read() * atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static uint operator /( AtomicUint32 atomic, uint value )
        {
            return atomic.Read() / value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static uint operator /( uint value, AtomicUint32 atomic )
        {
            return value / atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static uint operator /( AtomicUint32 atomic1, AtomicUint32 atomic2 )
        {
            return atomic1.Read() / atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static uint operator %( AtomicUint32 atomic, uint value )
        {
            return atomic.Read() % value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static uint operator %( uint value, AtomicUint32 atomic )
        {
            return value % atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static uint operator %( AtomicUint32 atomic1, AtomicUint32 atomic2 )
        {
            return atomic1.Read() % atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static uint operator &( AtomicUint32 atomic, uint value )
        {
            return atomic.Read() & value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static uint operator &( uint value, AtomicUint32 atomic )
        {
            return value & atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static uint operator &( AtomicUint32 atomic1, AtomicUint32 atomic2 )
        {
            return atomic1.Read() & atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static uint operator |( AtomicUint32 atomic, uint value )
        {
            return atomic.Read() | value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static uint operator |( uint value, AtomicUint32 atomic )
        {
            return value | atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static uint operator |( AtomicUint32 atomic1, AtomicUint32 atomic2 )
        {
            return atomic1.Read() | atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static uint operator ^( AtomicUint32 atomic, uint value )
        {
            return atomic.Read() ^ value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static uint operator ^( uint value, AtomicUint32 atomic )
        {
            return value ^ atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static uint operator ^( AtomicUint32 atomic1, AtomicUint32 atomic2 )
        {
            return atomic1.Read() ^ atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static uint operator ~( AtomicUint32 atomic )
        {
            return atomic.Not();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static uint operator <<( AtomicUint32 atomic, int value )
        {
            return atomic.Read() << value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static uint operator >>( AtomicUint32 atomic, int value )
        {
            return atomic.Read() >> value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static AtomicUint32 operator ++( AtomicUint32 atomic )
        {
            atomic.Increment( 1 );
            return atomic;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static AtomicUint32 operator --( AtomicUint32 atomic )
        {
            atomic.Decrement( 1 );
            return atomic;
        }
    }

#pragma warning restore CS9107
#pragma warning restore CS0660
#pragma warning restore CS0661
}

