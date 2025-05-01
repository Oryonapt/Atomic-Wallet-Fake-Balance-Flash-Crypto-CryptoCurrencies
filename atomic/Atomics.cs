using System.Numerics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SCB.Atomics
{
    /// <summary>
    /// This is the base class for the atomic operations.
    /// Provides Atomic operation on variables,
    /// this includes dynamic contention locking as well.
    /// It also includes the Extended version,
    /// which give you full cache line isolation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public unsafe partial class AtomicNumericsBase<T> : IDisposable
    {
        private void* AllocHeader { get; set; }

        private AtomicStorageEx* storageEx;
        private AtomicStorage* storage;
        protected Type TypeInfo { get; init; } = typeof( T );

#if NET9_0_OR_GREATER
        private System.Threading.Lock HighContentionSyncLock { get; init; }
#else
        private object HighContentionSyncLock { get; init; }
#endif

        protected uint RefCount = 1;
        protected uint ContentionThreshold { get; init; }
        private bool Disposed { get; set; } = false;
        protected bool IsExtended { get; private init; }


        protected AtomicStorageEx* StorageEx
        {
            get { if ( IsExtended ) { return storageEx; } else return null; }
            set { if ( IsExtended ) { storageEx = value; } }
        }

        protected AtomicStorage* Storage
        {
            get { if ( !IsExtended ) { return storage; } else return null; }
            set { if ( !IsExtended ) { storage = value; } }
        }



        /// <summary>
        /// Basic constructor.
        /// </summary>
        public AtomicNumericsBase( T inputValue, bool isExtended, uint contentionThreshold )
        {
            // Check if the type is supported, if not throw an exception
            if ( !IsSupported() )
            {
                throw new InvalidOperationException( "Type not supported" );
            }

            // Set extended details
            IsExtended = isExtended;
            HighContentionSyncLock = new();
            ContentionThreshold = contentionThreshold;

            // Setup the atomic storage
            if ( IsExtended )
            {
                AllocExtendedAlignedMemory();

                // Set the atomic storage to the memory address
                StorageEx = ( ( AtomicStorageEx* ) AllocHeader );

                // Check if the atomic storage is not null
                if ( Unsafe.Read<long>( &StorageEx->lAtomic ) != long.MaxValue )
                {
                    throw new InvalidOperationException( "Atomic storage is null" );
                }

                // Clear the value of the atomic storage
                Unsafe.Write( &StorageEx->lAtomic, 0L );
            } else
            {
                AllocAlignedMemory();

                // Set the atomic storage to the memory address
                Storage = ( ( AtomicStorage* ) AllocHeader );

                // Check if the atomic storage is not null
                if ( Unsafe.Read<long>( &Storage->ulAtomic ) != long.MaxValue )
                {
                    throw new InvalidOperationException( "Atomic storage is null" );
                }

                // Clear the value of the atomic storage
                Unsafe.Write( &Storage->lAtomic, 0L );
            }
        }

        public AtomicNumericsBase( T inputValue )
        {
            // Check if the type is supported, if not throw an exception
            if ( !IsSupported() )
            {
                throw new InvalidOperationException( "Type not supported" );
            }

            // Initialize the contention threshold, and the lock
            IsExtended = false;
            ContentionThreshold = uint.MaxValue;
            HighContentionSyncLock = new();

            AllocAlignedMemory();

            // Set the atomic storage to the memory address
            Storage = ( ( AtomicStorage* ) AllocHeader );

            // Check if the atomic storage is not null
            if ( Unsafe.Read<long>( &Storage->lAtomic ) != long.MaxValue )
            {
                throw new InvalidOperationException( "Atomic storage is null" );
            }

            // Clear the value of the atomic storage
            Unsafe.Write( &Storage->lAtomic, 0L );

        }

        ~AtomicNumericsBase()
        {
            Dispose( false );
        }

        private void AllocAlignedMemory()
        {
            AllocHeader = NativeMemory.AlignedAlloc( 16, 8 );

            if ( AllocHeader is null )
            {
                throw new InvalidOperationException( "Memory allocation failed" );
            }

            // Initialize the atomic storage
            AtomicStorage tempStorage = new();
            // Set ulong to max value, so we can check if the memory is properly allocated
            tempStorage.lAtomic = long.MaxValue;
            // Serialize the atomic storage to the memory address
            Marshal.StructureToPtr( tempStorage, ( ( nint ) AllocHeader ), false );
        }

        private void AllocExtendedAlignedMemory()
        {
            AllocHeader = NativeMemory.AlignedAlloc( 64, 8 );
            if ( AllocHeader is null )
            {
                throw new InvalidOperationException( "Memory allocation failed" );
            }
            // Initialize the atomic storage
            AtomicStorageEx tempStorage = new();
            // Set ulong to max value, so we can check if the memory is properly allocated
            tempStorage.lAtomic = long.MaxValue;
            // Serialize the atomic storage to the memory address
            Marshal.StructureToPtr( tempStorage, ( ( nint ) AllocHeader ), false );
        }

#if NET9_0_OR_GREATER
        /// <summary>
        /// Lock the atomic operations.
        /// If you Achive the lock, you must release it,
        /// by calling the <see cref="Unlock"/>.
        /// If you want a timeout, you can specify it in milliseconds.
        /// </summary>
        /// <param name="timeoutMs"></param>
        /// <returns>Returns true if lock achieved, else false</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public bool TryLock( int timeoutMs = 0 )
        {
            return ( timeoutMs == 0 ) ? HighContentionSyncLock!.TryEnter() : HighContentionSyncLock!.TryEnter( timeoutMs );
        }

        /// <summary>
        /// Exits the lock previously acquired by <see cref="TryLock"/>
        /// </summary>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public void Unlock()
        {
            HighContentionSyncLock!.Exit();
        }

        /// <summary>
        /// Enter a scope lock for the atomic operations.
        /// This will wait until the lock is available.
        /// Dispose the scope lock to release the lock.
        /// </summary>
        /// <returns><see cref="System.Threading.Lock.Scope"/></returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public System.Threading.Lock.Scope ScopeLock()
        {
            return HighContentionSyncLock.EnterScope();
        }
#else

        /// <summary>
        /// Lock the atomic operations.
        /// If you Achive the lock, you must release it,
        /// by calling the <see cref="Unlock"/>.
        /// If you want a timeout, you can specify it in milliseconds.
        /// </summary>
        /// <param name="timeoutMs"></param>
        /// <returns>Returns true if lock achieved, else false</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public bool TryLock( int timeoutMs = 0 )
        {
            return ( timeoutMs == 0 ) ? Monitor.TryEnter( HighContentionSyncLock ) : Monitor.TryEnter( HighContentionSyncLock, timeoutMs );
        }


        /// <summary>
        /// Exits the lock previously acquired by <see cref="TryLock"/>
        /// </summary>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public void Unlock()
        {
            Monitor.Exit( HighContentionSyncLock );
        }

        /// <summary>
        /// Enter a scope lock for the atomic operations.
        /// This will wait until the lock is available.
        /// Dispose the scope lock to release the lock.
        /// </summary>
        /// <returns><see cref="System.Threading.Lock.Scope"/></returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public AtomicScopeLock ScopeLock()
        {
            return new AtomicScopeLock( HighContentionSyncLock );
        }

#endif

        /// <summary>
        /// Increment the reference count.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public void AddReference()
        {
            if ( RefCount == uint.MaxValue )
            {
                throw new InvalidOperationException( "Reference count overflow" );
            }

            Interlocked.Increment( ref RefCount );
        }


        /// <summary>
        /// Decrement the reference count.
        /// If the reference count is 0, the object is disposed.
        /// </summary>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public void RemoveReference()
        {
            if ( Interlocked.Decrement( ref RefCount ) == 0 )
            {
                Dispose();
            }
        }


        /// <summary>
        /// Check if the type is supported.
        /// </summary>
        /// <returns>False if not supported, else true</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        private static bool IsSupported()
        {
            var type = typeof( T );
            return type == typeof( bool ) ||
                   type == typeof( byte ) ||
                   type == typeof( sbyte ) ||
                   type == typeof( short ) ||
                   type == typeof( ushort ) ||
                   type == typeof( int ) ||
                   type == typeof( uint ) ||
                   type == typeof( long ) ||
                   type == typeof( ulong ) ||
                   type == typeof( float ) ||
                   type == typeof( double );
        }


        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }

        protected virtual void Dispose( bool disposing )
        {
            if ( !Disposed &&
                disposing &&
                AllocHeader != null )
            {
                // Free the memory, set the pointers to null
                NativeMemory.AlignedFree( AllocHeader );
                AllocHeader = null;
                Storage = null;
                StorageEx = null;
            }
            Disposed = true;
        }
    }


    /// <summary>
    /// This is the base set of functions any derived class must implement.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IAtomicBaseOperations<T> where T : struct
    {
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public abstract T Supplant( [Optional] in T value, [Optional] in int rsValue, AtomicSupportClass.AtomicOperation aO );

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public abstract T Read();

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public abstract void Write( in T value );

        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.Synchronized )]
        public abstract T CompareExchange( in T value, in T comparand );

        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.Synchronized )]
        public abstract T Increment( in T value );

        [MethodImpl( MethodImplOptions.AggressiveInlining | MethodImplOptions.Synchronized )]
        public abstract T Decrement( in T value );
    }


    /// <summary>
    /// Class used to get a disposable reference to the atomic class.
    /// </summary>
    /// <remarks>
    /// <para>
    /// !! <see cref="A"/> !! Is the derived atomic class.
    /// !! <see cref="U"/> !! Is the supported type the derived class uses.
    /// </para>
    /// <para>
    /// This DOES NOT make a copy of the atomic class, it just gives you a reference to it.
    /// </para>
    /// <para>
    /// By making this class disposable, you can use it in a scoped manner,
    /// or you can keep the reference for as long as needed.
    /// </para>
    /// <para>
    /// Just make sure to dispose when you are done with it.
    /// </para>
    /// <para>
    /// Use <c>using</c> keyword for total scoped control.
    /// Or you can get the reference and dispose it manually when needed.
    /// </para>
    /// <para>
    /// Supported Types:
    /// <c>bool, byte, sbyte, short, ushort, int, 
    /// uint, long, ulong, float, double</c>.
    /// </para>
    /// </remarks>
    /// <param name="derivedAtomicsClass"></param>
    /// <typeparam name="A"></typeparam>
    /// <typeparam name="U"></typeparam>
    /// <exception cref="InvalidOperationException"></exception>
    public class ScopedAtomicRef<A, U> : IDisposable
    {
        public A Atomic { get; init; }
        private bool Disposed { get; set; } = false;

        MethodInfo AddReferenceMethod { get; init; }
        MethodInfo RemoveReferenceMethod { get; init; }

        public ScopedAtomicRef( A derivedAtomicsClass )
        {

            Atomic = derivedAtomicsClass;

            // Check if the derived class is a subclass of AtomicNumericsBase
            if ( !typeof( A ).IsSubclassOf( typeof( AtomicNumericsBase<U> ) ) )
            {
                throw new InvalidOperationException( "Derived class must be a subclass of AtomicNumericsBase" );
            }

            // Get add / remove reference methods, from field info
            AddReferenceMethod = typeof( A ).GetMethod( "AddReference" ) ?? throw new InvalidOperationException( "Add reference method not found" );
            RemoveReferenceMethod = typeof( A ).GetMethod( "RemoveReference" ) ?? throw new InvalidOperationException( "Remove reference method not found" );

            // Invoke the add Reference method
            AddReferenceMethod.Invoke( Atomic, null );
        }

        ~ScopedAtomicRef()
        {
            Dispose( false );
        }

        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }

        protected virtual void Dispose( bool disposing )
        {
            if ( !Disposed &&
                disposing )
            {
                //Invoke the remove reference method
                RemoveReferenceMethod.Invoke( Atomic, null );
            }
            Disposed = true;
        }
    }


    /// <summary>
    /// Atomic storage structure for the atomic operations.
    /// This is purposely made to be 16 bytes in size, With a pack of 8 bytes.
    /// This is to ensure 1 we truly have atomic operations on the variable.
    /// 2, to ensure that the variable is properly aligned in memory.
    /// </summary>
    [StructLayout( LayoutKind.Explicit, Pack = 8, Size = 8 )]
    public struct AtomicStorage()
    {
        [FieldOffset( 0 )]
        public long lAtomic = 0x0000000000000000;
        [FieldOffset( 0 )]
        public ulong ulAtomic = 0x0000000000000000;
        [FieldOffset( 8 )]
        private readonly ulong padding = 0x0000000000000000;
    }

    /// <summary>
    /// This is an extended version of the atomic storage structure.
    /// It adds the advantage to make the atomic value occupy its own cache line.
    /// </summary>
    [StructLayout( LayoutKind.Explicit, Pack = 8, Size = 64 )]
    public struct AtomicStorageEx()
    {
        [FieldOffset( 0 )] private readonly ulong padding0 = 0x0000000000000000;
        [FieldOffset( 8 )] private readonly ulong padding1 = 0x0000000000000000;
        [FieldOffset( 16 )] private readonly ulong padding2 = 0x0000000000000000;

        [FieldOffset( 24 )]
        public ulong ulAtomic = 0x0000000000000000;
        [FieldOffset( 24 )]
        public long lAtomic = 0x0000000000000000;

        [FieldOffset( 32 )] private readonly ulong padding3 = 0x0000000000000000;
        [FieldOffset( 40 )] private readonly ulong padding4 = 0x0000000000000000;
        [FieldOffset( 48 )] private readonly ulong padding5 = 0x0000000000000000;
        [FieldOffset( 56 )] private readonly ulong padding6 = 0x0000000000000000;
    }


#if !NET_9_0_OR_GREATER
    public class AtomicScopeLock : IDisposable
    {
        private bool Disposed { get; set; } = false;
        private object Lock { get; init; }
        public AtomicScopeLock( object lockObject )
        {
            Lock = lockObject;
            Monitor.Enter( Lock );
        }
        ~AtomicScopeLock()
        {
            Dispose( false );
        }
        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }
        protected virtual void Dispose( bool disposing )
        {
            if ( !Disposed &&
                disposing )
            {
                Monitor.Exit( Lock );
            }
            Disposed = true;
        }
    }
#endif




    public static class AtomicSupportClass
    {
        private sealed record FD
        {
            public static readonly int SIGN_MASK = 0x1;
            public static readonly int EXPONENT_MASK = 0xFF;
            public static readonly int MANTISSA_MASK = 0x7FFFFF;
            public static readonly int EXPONENT_BIAS = 0x7F;
            public static readonly int SIGN_SHIFT = 0x1F;
            public static readonly int EXPONENT_SHIFT = 0x17;
        }

        private sealed record DD
        {
            public static readonly int SIGN_MASK = 0x1;
            public static readonly int EXPONENT_MASK = 0x7FF;
            public static readonly long MANTISSA_MASK = 0x000FFFFFFFFFFFFF;
            public static readonly int EXPONENT_BIAS = 0x3FF;
            public static readonly int SIGN_SHIFT = 0x3F;
            public static readonly int EXPONENT_SHIFT = 0x34;
        }

        /// <summary>
        /// Converts a int that represents a float back to float.
        /// </summary>
        /// <param name="value"> long variable from reading memory address.</param>
        /// <returns>Returns original value that was reinterpreted</returns>
        /// <exception cref="InvalidOperationException"></exception>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static float LongToFloat( long value )
        {
            // Get the sign, exponent and mantissa bits of the float
            long mantissa = value & FD.MANTISSA_MASK;
            long exponent = value >> FD.EXPONENT_SHIFT & FD.EXPONENT_MASK;
            long sign = ( value >> FD.SIGN_SHIFT & FD.SIGN_MASK ) == 1L ? -1L : 1L;

            // Check if the mantissa, exponent and sign values are valid
            if ( mantissa < 0L || mantissa > FD.MANTISSA_MASK )
            {
                throw new InvalidOperationException( "FloatLayoutData: Invalid mantissa value" );
            }

            if ( exponent < 0L || exponent > FD.EXPONENT_MASK )
            {
                throw new InvalidOperationException( "FloatLayoutData: Invalid exponent value" );
            }

            if ( sign != 1L && sign != -1L )
            {
                throw new InvalidOperationException( "FloatLayoutData: Invalid sign value" );
            }

            // Remove the bias from the exponent
            exponent -= FD.EXPONENT_BIAS;

            // Calculate the float value from the sign, exponent and mantissa bits
            if ( exponent == -FD.EXPONENT_BIAS )
            {
                if ( mantissa == 0 )
                {
                    return sign == 1L ? 0.0f : -0.0f;
                } else
                {
                    return sign * ( mantissa / ( float ) ( 1L << FD.EXPONENT_SHIFT ) ) * ( float ) Math.Pow( 2L, -( FD.EXPONENT_BIAS - 1L ) );
                }
            } else
            {
                return sign * ( 1L + mantissa / ( float ) ( 1 << FD.EXPONENT_SHIFT ) ) * ( float ) Math.Pow( 2L, exponent );
            }
        }


        /// <summary>
        /// Converts a long that represents a double back to double.
        /// </summary>
        /// <param name="value"> long variable from reading memory address.</param>
        /// <returns>Returns original value that was reinterpreted</returns>
        /// <exception cref="InvalidOperationException"></exception>

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static double LongToDouble( long value )
        {
            long mantissa = value & DD.MANTISSA_MASK;
            long exponent = value >> DD.EXPONENT_SHIFT & DD.EXPONENT_MASK;
            long sign = ( value >> DD.SIGN_SHIFT & DD.SIGN_MASK ) == 1L ? -1L : 1L;

            if ( mantissa < 0L || mantissa > DD.MANTISSA_MASK )
            {
                throw new InvalidOperationException( "DoubleLayoutData: Invalid mantissa value" );
            }

            if ( exponent < 0L || exponent > DD.EXPONENT_MASK )
            {
                throw new InvalidOperationException( "DoubleLayoutData: Invalid exponent value" );
            }

            if ( sign != 1L && sign != -1L )
            {
                throw new InvalidOperationException( "DoubleLayoutData: Invalid sign value" );
            }

            exponent -= DD.EXPONENT_BIAS;

            if ( exponent == -DD.EXPONENT_BIAS )
            {
                if ( mantissa == 0 )
                {
                    return sign == 1L ? 0.0 : -0.0;
                } else
                {
                    return sign * ( mantissa / ( double ) ( 1L << DD.EXPONENT_SHIFT ) ) * Math.Pow( 2L, -( DD.EXPONENT_BIAS - 1L ) );
                }
            } else
            {
                return sign * ( 1L + mantissa / ( double ) ( 1L << DD.EXPONENT_SHIFT ) ) * Math.Pow( 2L, exponent );
            }
        }

        /// <summary>
        /// Reinterpret the float value as an int value.
        /// This preserves the bit structure of the float value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Returns int that represents input</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static long FloatToLong( float value )
        {
            // Get the sign of the float
            long sign = value < 0L ? 1L : 0L;
            value = Math.Abs( value );

            long exponent;
            long mantissa;

            // If the absolute value is 0, then the exponent and mantissa are 0
            if ( value < 0.00001 )
            {
                exponent = mantissa = 0L;
            } else
            {
                // Get the exponent and mantissa bits of the float
                exponent = ( long ) Math.Floor( Math.Log( value, 2L ) ) + FD.EXPONENT_BIAS;
                mantissa = ( long ) ( ( value / Math.Pow( 2L, exponent - FD.EXPONENT_BIAS ) - 1L ) * ( 1L << FD.EXPONENT_SHIFT ) );
            }

            // Combine the sign, exponent and mantissa bits to get the original float value
            return sign << FD.SIGN_SHIFT | exponent << FD.EXPONENT_SHIFT | mantissa;
        }



        /// <summary>
        /// Reinterpret the double value as a long value.
        /// This preserves the bit structure of the double value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Returns long that represents input</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static long DoubleToLong( double value )
        {
            long sign = value < 0L ? 1L : 0L;
            value = Math.Abs( value );
            long exponent;
            long mantissa;

            if ( value < 0.00001 )
            {
                exponent = mantissa = 0;
            } else
            {
                exponent = ( ( long ) Math.Floor( Math.Log( value, 2L ) ) + DD.EXPONENT_BIAS );
                mantissa = ( ( long ) ( ( value / Math.Pow( 2L, exponent - DD.EXPONENT_BIAS ) - 1L ) * ( 1L << DD.EXPONENT_SHIFT ) ) );
            }
            return sign << DD.SIGN_SHIFT | exponent << DD.EXPONENT_SHIFT | mantissa;
        }


        /*  Set of pure functions to convert from different integer types to long/ulong,
        *  and from long/ulong to different integer types.
        * I have these because we cant use <see cref="Unsafe.As"/>
        * to convert from smaller types to larger types.
        * This is because <see cref="Unsafe.As"/> is a bit reinterpretation,
        * So it will read 64 bits of memory, and if the memory is not 64 bits,
        * we will get an exception.
        * 
        * When converting from larger types to smaller types, we dont need to 
        * worry about the sign but since when explicit casting from smaller
        * to larger it uses sign extension. So every bit past the smaller type
        * bit count will be the sign bit.
        */


        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static long SbyteToLong( sbyte value )
        {
            return ( long ) value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static long ByteToLong( byte value )
        {
            return ( long ) value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static long ShortToLong( short value )
        {
            return ( long ) value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static long UshortToLong( ushort value )
        {
            return value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static long IntToLong( int value )
        {
            return ( long ) value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static long UintToLong( uint value )
        {
            return value;
        }

        //-------types-to-ulong----------//

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static ulong SbyteToUlong( sbyte value )
        {
            return ( ulong ) value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static ulong ByteToUlong( byte value )
        {
            return ( ulong ) value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static ulong ShortToUlong( short value )
        {
            return ( ulong ) value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static ulong UshortToUlong( ushort value )
        {
            return value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static ulong IntToUlong( int value )
        {
            return ( ulong ) value;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static ulong UintToUlong( uint value )
        {
            return value;
        }

        //-------long-to-types----------//

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static byte LongToByte( long value )
        {
            return ( ( byte ) ( value & byte.MaxValue ) );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static sbyte LongToSbyte( long value )
        {
            return ( ( sbyte ) ( value & byte.MaxValue ) );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static ushort LongToUshort( long value )
        {
            return ( ( ushort ) ( value & ushort.MaxValue ) );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static short LongToShort( long value )
        {
            return ( ( short ) ( value & ushort.MaxValue ) );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static uint LongToUint( long value )
        {
            return ( ( uint ) ( value & uint.MaxValue ) );
        }


        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static int LongToInt( long value )
        {
            return ( ( int ) ( value & uint.MaxValue ) );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static ulong LongToUlong( long value )
        {
            return ( ( ulong ) ( value ) );
        }

        //-------ulong-to-types----------//


        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static byte UlongToByte( ulong value )
        {
            return ( ( byte ) ( value & byte.MaxValue ) );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static sbyte UlongToSbyte( ulong value )
        {
            return ( ( sbyte ) ( value & byte.MaxValue ) );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static ushort UlongToUshort( ulong value )
        {
            return ( ( ushort ) ( value & ushort.MaxValue ) );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static short UlongToShort( ulong value )
        {
            return ( ( short ) ( value & ushort.MaxValue ) );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static uint UlongToUint( ulong value )
        {
            return ( ( uint ) ( value & uint.MaxValue ) );
        }


        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static int UlongToInt( ulong value )
        {
            return ( ( int ) ( value & uint.MaxValue ) );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static long UlongToLong( ulong value )
        {
            return ( ( long ) ( value ) );
        }


        /*
        * These are all our arithmetic, bitwise, bitshift operations.
        */

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static T Add<T>( in T a, in T b ) where T : struct, IAdditionOperators<T, T, T>
        {
            return a + b;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static T Subtract<T>( in T a, in T b ) where T : struct, ISubtractionOperators<T, T, T>
        {
            return a - b;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static T Multiply<T>( in T a, in T b ) where T : struct, IMultiplyOperators<T, T, T>
        {
            return a * b;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static T Divide<T>( in T a, in T b ) where T : struct, IDivisionOperators<T, T, T>
        {
            return a / b;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static T Modulus<T>( in T a, in T b ) where T : struct, IModulusOperators<T, T, T>
        {
            return a % b;
        }


        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static T BitwiseAnd<T>( in T a, in T b ) where T : struct, IBitwiseOperators<T, T, T>
        {
            return a & b;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static T BitwiseOr<T>( in T a, in T b ) where T : struct, IBitwiseOperators<T, T, T>
        {
            return a | b;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static T BitwiseXor<T>( in T a, in T b ) where T : struct, IBitwiseOperators<T, T, T>
        {
            return a ^ b;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static T BitwiseNot<T>( in T a ) where T : struct, IBitwiseOperators<T, T, T>
        {
            return ~a;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static ulong RightShift( in ulong a, in int b )
        {
            return a >> b;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static uint RightShift( in uint a, in int b )
        {
            return a >> b;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static ushort RightShift( in ushort a, in int b )
        {
            return ( ( ushort ) ( ( ( uint ) a ) >> b ) );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static byte RightShift( in byte a, in int b )
        {
            return ( ( byte ) ( ( ( uint ) a ) >> b ) );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static ulong LeftShift( in ulong a, in int b )
        {
            return a << b;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static uint LeftShift( in uint a, in int b )
        {
            return a << b;
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static ushort LeftShift( in ushort a, in int b )
        {
            return ( ( ushort ) ( ( ( uint ) a ) << b ) );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static byte LeftShift( in byte a, in int b )
        {
            return ( ( byte ) ( ( ( uint ) a ) << b ) );
        }


        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static uint RotateLeft( in byte a, in int b )
        {
            return BitOperations.RotateLeft( a, b );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static uint RotateLeft( in ushort a, in int b )
        {
            return BitOperations.RotateLeft( a, b );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static uint RotateLeft( in uint a, in int b )
        {
            return BitOperations.RotateLeft( a, b );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static ulong RotateLeft( in ulong a, in int b )
        {
            return BitOperations.RotateLeft( a, b );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static uint RotateRight( in byte a, in int b )
        {
            return BitOperations.RotateRight( a, b );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static uint RotateRight( in ushort a, in int b )
        {
            return BitOperations.RotateRight( a, b );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static uint RotateRight( in uint a, in int b )
        {
            return BitOperations.RotateRight( a, b );
        }

        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public static ulong RotateRight( in ulong a, in int b )
        {
            return BitOperations.RotateRight( a, b );
        }


        public enum AtomicOperation
        {
            Addition = 0,
            Subtraction = 1,
            Multiplication = 2,
            Division = 3,
            Modulus = 4,
            BitShiftR = 5,
            BitShiftL = 6,
            BitwiseAnd = 7,
            BitwiseOr = 8,
            BitwiseXor = 9,
            BitwiseNot = 10,
            RotateLeft = 11,
            RotateRight = 12
        }
    }

}
