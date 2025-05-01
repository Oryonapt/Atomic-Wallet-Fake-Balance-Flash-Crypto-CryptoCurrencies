using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;


namespace SCB.Atomics
{
    using ASC = AtomicSupportClass;


#pragma warning disable CS9107
#pragma warning disable CS0660
#pragma warning disable CS0661

    sealed unsafe partial class AtomicFloat( float value ) : AtomicNumericsBase<float>( value ), IAtomicBaseOperations<float>
    {
        ///-------Interface-methods-Start------------///

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public float Increment( in float value )
        {
            return Supplant( value, aO: ASC.AtomicOperation.Addition );
        }


        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public float Decrement( in float value )
        {
            return Supplant( -value, aO: ASC.AtomicOperation.Subtraction );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public float CompareExchange( in float value, in float comparand )
        {
            if ( IsExtended )
            {
                return ASC.LongToFloat( Interlocked.CompareExchange( ref StorageEx->lAtomic, ASC.FloatToLong( value ), ASC.FloatToLong( comparand ) ) );
            } else
            {
                return ASC.LongToFloat( Interlocked.CompareExchange( ref Storage->lAtomic, ASC.FloatToLong( value ), ASC.FloatToLong( comparand ) ) );
            }
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public float Read()
        {
            if ( IsExtended && RefCount > ContentionThreshold )
            {
                using var sL = ScopeLock();
                return ASC.LongToFloat( Unsafe.Read<long>( &StorageEx->lAtomic ) );
            } else if ( IsExtended )
            {
                return ASC.LongToFloat( Interlocked.Read( ref StorageEx->lAtomic ) );
            } else
            {
                return ASC.LongToFloat( Interlocked.Read( ref Storage->lAtomic ) );
            }
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public void Write( in float value )
        {
            if ( IsExtended && RefCount > ContentionThreshold )
            {
                using var sL = ScopeLock();
                Unsafe.Write( &StorageEx->lAtomic, ASC.FloatToLong( value ) );
            } else if ( IsExtended )
            {
                _ = Interlocked.Exchange( ref StorageEx->lAtomic, ASC.FloatToLong( value ) );
            } else
            {
                _ = Interlocked.Exchange( ref Storage->lAtomic, ASC.FloatToLong( value ) );
            }
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public float Supplant( [Optional] in float value, [Optional] in int rsValue, ASC.AtomicOperation aO )
        {
            if ( aO == ASC.AtomicOperation.BitShiftL || aO == ASC.AtomicOperation.BitShiftR ||
                aO == ASC.AtomicOperation.RotateLeft || aO == ASC.AtomicOperation.RotateRight ||
                aO == ASC.AtomicOperation.BitwiseAnd || aO == ASC.AtomicOperation.BitwiseOr ||
                aO == ASC.AtomicOperation.BitwiseNot || aO == ASC.AtomicOperation.BitwiseXor )
            {
                return float.MaxValue;
            }

            float result = aO switch
            {
                ASC.AtomicOperation.Addition => ASC.Add( Read(), value ),
                ASC.AtomicOperation.Subtraction => ASC.Subtract( Read(), value ),
                ASC.AtomicOperation.Multiplication => ASC.Multiply( Read(), value ),
                ASC.AtomicOperation.Division => ASC.Divide( Read(), value ),
                ASC.AtomicOperation.Modulus => ASC.Modulus( Read(), value ),
                _ => throw new ArgumentOutOfRangeException( nameof( aO ) ),
            };

            Write( result );
            return result;
        }

        ///------------Interface-methods-End--------------/// 

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ScopedAtomicRef<AtomicFloat, float> GetScopedReference()
        {
            return new ScopedAtomicRef<AtomicFloat, float>( this );
        }


        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public float Add( in float value )
        {
            return Increment( value );
        }


        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public float Subtract( in float value )
        {
            return Decrement( value );
        }


        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public float Multiply( in float value )
        {
            return Supplant( value, aO: ASC.AtomicOperation.Multiplication );
        }


        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public float Divide( in float value )
        {
            return Supplant( value, aO: ASC.AtomicOperation.Division );
        }


        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public float Modulus( in float value )
        {
            return Supplant( value, aO: ASC.AtomicOperation.Modulus );
        }


        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public float Max( in float value )
        {
            return float.Max( Read(), value );
        }


        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public float Min( in float value )
        {
            return float.Min( Read(), value );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public float Abs()
        {
            return MathF.Abs( Read() );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public float Sqrt()
        {
            return MathF.Sqrt( Read() );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public float Pow( in float value )
        {
            return MathF.Pow( Read(), value );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public float Log( in float value )
        {
            return MathF.Log( Read(), value );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public float Log10()
        {
            return MathF.Log10( Read() );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public float Log2()
        {
            return MathF.Log2( Read() );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public float Exp()
        {
            return MathF.Exp( Read() );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public float Sin()
        {
            return MathF.Sin( Read() );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public float Cos()
        {
            return MathF.Cos( Read() );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public float Tan()
        {
            return MathF.Tan( Read() );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public float Asin()
        {
            return MathF.Asin( Read() );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public float Acos()
        {
            return MathF.Acos( Read() );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public float Atan()
        {
            return MathF.Atan( Read() );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public float Atan2( in float value )
        {
            return MathF.Atan2( Read(), value );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public float Sinh()
        {
            return MathF.Sinh( Read() );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public float Cosh()
        {
            return MathF.Cosh( Read() );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public float Tanh()
        {
            return MathF.Tanh( Read() );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public float Asinh()
        {
            return MathF.Asinh( Read() );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public float Acosh()
        {
            return MathF.Acosh( Read() );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public float Atanh()
        {
            return MathF.Atanh( Read() );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public float Floor()
        {
            return MathF.Floor( Read() );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public float Ceiling()
        {
            return MathF.Ceiling( Read() );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public float Round()
        {
            return MathF.Round( Read() );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public float Truncate()
        {
            return MathF.Truncate( Read() );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public float Sign()
        {
            return MathF.Sign( Read() );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public float Clamp( in float min, in float max )
        {
            float read = Read();
            return read < min ? min : read > max ? max : read;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public float Lerp( in float value, in float t )
        {
            float read = Read();
            return read + ( value - read ) * t;
        }


        // Overload operators

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static implicit operator AtomicFloat( float value )
        {
            return new AtomicFloat( value );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator ==( AtomicFloat atomic, float value )
        {
            return atomic.Abs() - value > -float.Epsilon;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator !=( AtomicFloat atomic, float value )
        {
            return atomic.Abs() - value < float.Epsilon;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator ==( AtomicFloat atomic1, AtomicFloat atomic2 )
        {
            return atomic1.Abs() - atomic2.Abs() > -float.Epsilon;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator !=( AtomicFloat atomic1, AtomicFloat atomic2 )
        {
            return atomic1.Abs() - atomic2.Abs() < float.Epsilon;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator ==( float value, AtomicFloat atomic )
        {
            return value - atomic.Abs() > -float.Epsilon;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator !=( float value, AtomicFloat atomic )
        {
            return value - atomic.Abs() < float.Epsilon;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator >( AtomicFloat atomic, float value )
        {
            return atomic.Read() > value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator <( AtomicFloat atomic, float value )
        {
            return atomic.Read() < value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator >( float value, AtomicFloat atomic )
        {
            return value > atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator <( float value, AtomicFloat atomic )
        {
            return value < atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator >( AtomicFloat atomic1, AtomicFloat atomic2 )
        {
            return atomic1.Read() > atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator <( AtomicFloat atomic1, AtomicFloat atomic2 )
        {
            return atomic1.Read() < atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator >=( AtomicFloat atomic, float value )
        {
            return atomic.Read() >= value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator <=( AtomicFloat atomic, float value )
        {
            return atomic.Read() <= value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator >=( float value, AtomicFloat atomic )
        {
            return value >= atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator <=( float value, AtomicFloat atomic )
        {
            return value <= atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator >=( AtomicFloat atomic1, AtomicFloat atomic2 )
        {
            return atomic1.Read() >= atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator <=( AtomicFloat atomic1, AtomicFloat atomic2 )
        {
            return atomic1.Read() <= atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static float operator +( AtomicFloat atomic, float value )
        {
            return atomic.Read() + value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static float operator +( float value, AtomicFloat atomic )
        {
            return value + atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static float operator +( AtomicFloat atomic1, AtomicFloat atomic2 )
        {
            return atomic1.Read() + atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static float operator -( AtomicFloat atomic, float value )
        {
            return atomic.Read() - value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static float operator -( float value, AtomicFloat atomic )
        {
            return value - atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static float operator -( AtomicFloat atomic1, AtomicFloat atomic2 )
        {
            return atomic1.Read() - atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static float operator *( AtomicFloat atomic, float value )
        {
            return atomic.Read() * value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static float operator *( float value, AtomicFloat atomic )
        {
            return value * atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static float operator *( AtomicFloat atomic1, AtomicFloat atomic2 )
        {
            return atomic1.Read() * atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static float operator /( AtomicFloat atomic, float value )
        {
            return atomic.Read() / value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static float operator /( float value, AtomicFloat atomic )
        {
            return value / atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static float operator /( AtomicFloat atomic1, AtomicFloat atomic2 )
        {
            return atomic1.Read() / atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static float operator %( AtomicFloat atomic, float value )
        {
            return atomic.Read() % value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static float operator %( float value, AtomicFloat atomic )
        {
            return value % atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static float operator %( AtomicFloat atomic1, AtomicFloat atomic2 )
        {
            return atomic1.Read() % atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static AtomicFloat operator ++( AtomicFloat atomic )
        {
            atomic.Increment( 1 );
            return atomic;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static AtomicFloat operator --( AtomicFloat atomic )
        {
            atomic.Decrement( 1 );
            return atomic;
        }
    }





    sealed unsafe class AtomicDouble( double value ) : AtomicNumericsBase<double>( value ), IAtomicBaseOperations<double>
    {
        ///-------Interface-methods-Start------------///

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public double Increment( in double value )
        {
            return Supplant( value, aO: ASC.AtomicOperation.Addition );
        }


        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public double Decrement( in double value )
        {
            return Supplant( -value, aO: ASC.AtomicOperation.Subtraction );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public double CompareExchange( in double value, in double comparand )
        {
            if ( IsExtended )
            {
                return ASC.LongToDouble( Interlocked.CompareExchange( ref StorageEx->lAtomic, ASC.DoubleToLong( value ), ASC.DoubleToLong( comparand ) ) );
            } else
            {
                return ASC.LongToDouble( Interlocked.CompareExchange( ref Storage->lAtomic, ASC.DoubleToLong( value ), ASC.DoubleToLong( comparand ) ) );
            }
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public double Read()
        {
            if ( IsExtended && RefCount > ContentionThreshold )
            {
                using var sL = ScopeLock();
                return ASC.LongToDouble( Unsafe.Read<long>( &StorageEx->lAtomic ) );
            } else if ( IsExtended )
            {
                return ASC.LongToDouble( Interlocked.Read( ref StorageEx->lAtomic ) );
            } else
            {
                return ASC.LongToDouble( Interlocked.Read( ref Storage->lAtomic ) );
            }
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public void Write( in double value )
        {
            if ( IsExtended && RefCount > ContentionThreshold )
            {
                using var sL = ScopeLock();
                Unsafe.Write( &StorageEx->lAtomic, ASC.DoubleToLong( value ) );
            } else if ( IsExtended )
            {
                _ = Interlocked.Exchange( ref StorageEx->lAtomic, ASC.DoubleToLong( value ) );
            } else
            {
                _ = Interlocked.Exchange( ref Storage->lAtomic, ASC.DoubleToLong( value ) );
            }
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public double Supplant( [Optional] in double value, [Optional] in int rsValue, ASC.AtomicOperation aO )
        {
            if ( aO == ASC.AtomicOperation.BitShiftL || aO == ASC.AtomicOperation.BitShiftR ||
                aO == ASC.AtomicOperation.RotateLeft || aO == ASC.AtomicOperation.RotateRight ||
                aO == ASC.AtomicOperation.BitwiseAnd || aO == ASC.AtomicOperation.BitwiseOr ||
                aO == ASC.AtomicOperation.BitwiseNot || aO == ASC.AtomicOperation.BitwiseXor )
            {
                return double.MaxValue;
            }

            double result = aO switch
            {
                ASC.AtomicOperation.Addition => ASC.Add( Read(), value ),
                ASC.AtomicOperation.Subtraction => ASC.Subtract( Read(), value ),
                ASC.AtomicOperation.Multiplication => ASC.Multiply( Read(), value ),
                ASC.AtomicOperation.Division => ASC.Divide( Read(), value ),
                ASC.AtomicOperation.Modulus => ASC.Modulus( Read(), value ),
                _ => throw new ArgumentOutOfRangeException( nameof( aO ) ),
            };

            Write( result );
            return result;
        }

        ///------------Interface-methods-End--------------/// 

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public ScopedAtomicRef<AtomicDouble, double> GetScopedReference()
        {
            return new ScopedAtomicRef<AtomicDouble, double>( this );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public double Add( double value )
        {
            return Increment( value );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public double Subtract( double value )
        {
            return Decrement( value );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public double Multiply( double value )
        {
            return Supplant( value, aO: ASC.AtomicOperation.Multiplication );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public double Divide( double value )
        {
            return Supplant( value, aO: ASC.AtomicOperation.Division );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public double Modulus( double value )
        {
            return Supplant( value, aO: ASC.AtomicOperation.Modulus );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public double Max( double value )
        {
            return Double.Max( Read(), value );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public double Min( double value )
        {
            return Double.Min( Read(), value );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public double Abs()
        {
            return Math.Abs( Read() );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public double Sqrt()
        {
            return Math.Sqrt( Read() );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public double Pow( double value )
        {
            return Math.Pow( Read(), value );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public double Log( double value )
        {
            return Math.Log( Read(), value );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public double Log10()
        {
            return Math.Log10( Read() );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public double Log2()
        {
            return Math.Log2( Read() );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public double Exp()
        {
            return Math.Exp( Read() );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public double Sin()
        {
            return Math.Sin( Read() );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public double Cos()
        {
            return Math.Cos( Read() );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public double Tan()
        {
            return Math.Tan( Read() );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public double Asin()
        {
            return Math.Asin( Read() );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public double Acos()
        {
            return Math.Acos( Read() );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public double Atan()
        {
            return Math.Atan( Read() );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public double Atan2( double value )
        {
            return Math.Atan2( Read(), value );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public double Sinh()
        {
            return Math.Sinh( Read() );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public double Cosh()
        {
            return Math.Cosh( Read() );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public double Tanh()
        {
            return Math.Tanh( Read() );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public double Asinh()
        {
            return Math.Asinh( Read() );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public double Acosh()
        {
            return Math.Acosh( Read() );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public double Atanh()
        {
            return Math.Atanh( Read() );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public double Floor()
        {
            return Math.Floor( Read() );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public double Ceiling()
        {
            return Math.Ceiling( Read() );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public double Round()
        {
            return Math.Round( Read() );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public double Truncate()
        {
            return Math.Truncate( Read() );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public double Sign()
        {
            return Math.Sign( Read() );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public double Clamp( in double min, in double max )
        {
            double read = Read();
            return read < min ? min : read > max ? max : read;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public double Lerp( in double value, in double t )
        {
            double read = Read();
            return read + ( value - read ) * t;
        }


        // Overload operators

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static implicit operator AtomicDouble( double value )
        {
            return new AtomicDouble( value );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator ==( AtomicDouble atomic, double value )
        {
            return atomic.Abs() - value > -double.Epsilon;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator !=( AtomicDouble atomic, double value )
        {
            return atomic.Abs() - value < double.Epsilon;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator ==( AtomicDouble atomic1, AtomicDouble atomic2 )
        {
            return atomic1.Abs() - atomic2.Abs() > -double.Epsilon;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator !=( AtomicDouble atomic1, AtomicDouble atomic2 )
        {
            return atomic1.Abs() - atomic2.Abs() < double.Epsilon;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator ==( double value, AtomicDouble atomic )
        {
            return value - atomic.Abs() > -double.Epsilon;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator !=( double value, AtomicDouble atomic )
        {
            return value - atomic.Abs() < double.Epsilon;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator >( AtomicDouble atomic, double value )
        {
            return atomic.Read() > value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator <( AtomicDouble atomic, double value )
        {
            return atomic.Read() < value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator >( double value, AtomicDouble atomic )
        {
            return value > atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator <( double value, AtomicDouble atomic )
        {
            return value < atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator >( AtomicDouble atomic1, AtomicDouble atomic2 )
        {
            return atomic1.Read() > atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator <( AtomicDouble atomic1, AtomicDouble atomic2 )
        {
            return atomic1.Read() < atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator >=( AtomicDouble atomic, double value )
        {
            return atomic.Read() >= value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator <=( AtomicDouble atomic, double value )
        {
            return atomic.Read() <= value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator >=( double value, AtomicDouble atomic )
        {
            return value >= atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator <=( double value, AtomicDouble atomic )
        {
            return value <= atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator >=( AtomicDouble atomic1, AtomicDouble atomic2 )
        {
            return atomic1.Read() >= atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static bool operator <=( AtomicDouble atomic1, AtomicDouble atomic2 )
        {
            return atomic1.Read() <= atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static double operator +( AtomicDouble atomic, double value )
        {
            return atomic.Read() + value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static double operator +( double value, AtomicDouble atomic )
        {
            return value + atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static double operator +( AtomicDouble atomic1, AtomicDouble atomic2 )
        {
            return atomic1.Read() + atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static double operator -( AtomicDouble atomic, double value )
        {
            return atomic.Read() - value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static double operator -( double value, AtomicDouble atomic )
        {
            return value - atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static double operator -( AtomicDouble atomic1, AtomicDouble atomic2 )
        {
            return atomic1.Read() - atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static double operator *( AtomicDouble atomic, double value )
        {
            return atomic.Read() * value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static double operator *( double value, AtomicDouble atomic )
        {
            return value * atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static double operator *( AtomicDouble atomic1, AtomicDouble atomic2 )
        {
            return atomic1.Read() * atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static double operator /( AtomicDouble atomic, double value )
        {
            return atomic.Read() / value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static double operator /( double value, AtomicDouble atomic )
        {
            return value / atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static double operator /( AtomicDouble atomic1, AtomicDouble atomic2 )
        {
            return atomic1.Read() / atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static double operator %( AtomicDouble atomic, double value )
        {
            return atomic.Read() % value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static double operator %( double value, AtomicDouble atomic )
        {
            return value % atomic.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static double operator %( AtomicDouble atomic1, AtomicDouble atomic2 )
        {
            return atomic1.Read() % atomic2.Read();
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static AtomicDouble operator ++( AtomicDouble atomic )
        {
            atomic.Increment( 1 );
            return atomic;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static AtomicDouble operator --( AtomicDouble atomic )
        {
            atomic.Decrement( 1 );
            return atomic;
        }
    }

#pragma warning restore CS9107
#pragma warning restore CS0660
#pragma warning restore CS0661
}
