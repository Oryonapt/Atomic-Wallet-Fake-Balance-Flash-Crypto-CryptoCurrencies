using System.Runtime.CompilerServices;

namespace SCB.Atomics
{

    /* 
    * Both of these classes could be expanded on but i made them in 
    * A quick fashion because i needed a nullable concurrent internal
    * Storage thread safe signal
    * 
    * The biggest improvments would be adding a ton of overloads! 
    */

    /// <summary>
    /// Thread safe signal for multiple threads to use for blocking
    /// Can be used to transfer data as well like return values
    /// This is meant for non nullable data types
    /// </summary>
    /// <typeparam name="T">Generics</typeparam>
    /// <param name="inputValue">The initial value to set internally</param>
    public partial class NonNullableAtomicThreadSignal<T>( T inputValue ) : IDisposable
    {
        private bool disposed = false;
#if NET9_0_OR_GREATER
        private readonly Lock internalLock = new();
#else
        private readonly object internalLock = new();
#endif
        private T value = inputValue;
        private readonly ManualResetEventSlim threadSignal = new();

        ~NonNullableAtomicThreadSignal()
        {
            Dispose( false );
        }


        /// <summary>
        /// Blocks calling thread for timeout interval
        /// </summary>
        /// <param name="millisecondTimeout">Timeout you want to wait for signal state</param>
        /// <returns>Returns true if it gets set to signaled state during timeout, else false</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public bool WaitForSignaledState( int millisecondTimeout ) => threadSignal.Wait( millisecondTimeout );


        /// <summary>
        /// Blocks calling thread till returns signaled
        /// </summary>
        /// <returns>Returns true if it gets set to signaled state during timeout, else false</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public void WaitForSignaledState() => threadSignal.Wait();



        /// <summary>
        /// Gets the current thread signal
        /// Thread safe
        /// </summary>
        /// <returns>Returns true if its in a signaled state, else false</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public bool IsSignaled() => threadSignal.IsSet;



        /// <summary>
        /// Gets the current thread signal
        /// Thread safe
        /// </summary>
        /// <returns>Returns true if its in a non signaled state, else false</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public bool IsNonSignaled() => !threadSignal.IsSet;


        /// <summary>
        /// Sets to signaled state
        /// This will unblock other Threads
        /// <function cref="ManualResetEventSlim.Set"></function>
        /// </summary>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public void SetSignaled()
        {
            threadSignal.Set();
        }

        /// <summary>
        /// Sets to signaled state
        /// This will unblock other Threads
        /// Set the internal value simultaneously
        /// <function cref="ManualResetEventSlim.Set"></function>
        /// <param name="newValue">New value to be set internally</param>
        /// </summary>
        /// <returns>Returns old internal value</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public T SetSignaled( T newValue )
        {
            threadSignal.Set();
            var result = SetValue( newValue );
            return result;
        }


        /// <summary>
        /// Sets to non signaled state
        /// This will Block other Threads
        /// <function cref="ManualResetEventSlim.Reset"></function>
        /// </summary>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public void SetNonSignaled()
        {
            threadSignal.Reset();
        }


        /// <summary>
        /// Sets to non signaled state
        /// This will Block other Threads
        /// Set the internal value simultaneously
        /// <function cref="ManualResetEventSlim.Reset"></function>
        /// <param name="newValue">New value to be set internally</param> 
        /// </summary>
        /// <returns>Returns old internal value</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public T SetNonSignaled( T newValue )
        {
            threadSignal.Reset();
            var result = SetValue( newValue );
            return result;
        }


        /// <summary>
        /// Set the internal value
        /// </summary>
        /// <param name="newValue">New value to be set internally</param>
        /// <returns>Resturns old internal value</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public T SetValue( T newValue )
        {
            T oldValue;
            using var sL = this.ScopeLock();
            oldValue = value;
            value = newValue;
            return oldValue;
        }


        /// <summary>
        /// Gets the internal value
        /// </summary>
        /// <returns>Resturns current internal value</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public T GetValue()
        {
            T oldValue;
            using var sL = this.ScopeLock();
            oldValue = value;
            return oldValue;
        }

#if NET9_0_OR_GREATER
        /// <summary>
        /// Enter a scope lock for the atomic operations.
        /// This will wait until the lock is available.
        /// Dispose the scope lock to release the lock.
        /// </summary>
        /// <returns><see cref="System.Threading.Lock.Scope"/></returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public System.Threading.Lock.Scope ScopeLock()
        {
            return internalLock.EnterScope();
        }
#else

        /// <summary>
        /// Enter a scope lock for the atomic operations.
        /// This will wait until the lock is available.
        /// Dispose the scope lock to release the lock.
        /// </summary>
        /// <returns><see cref="System.Threading.Lock.Scope"/></returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public AtomicScopeLock ScopeLock()
        {
            return new AtomicScopeLock( internalLock );
        }

#endif

        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }

        protected virtual void Dispose( bool disposing )
        {
            if ( !disposed &&
                disposing )
            {
                threadSignal.Dispose();
            }
            disposed = true;
        }
    }



    /// <summary>
    /// Thread safe signal for multiple threads to use for blocking
    /// Can be used to transfer data as well like return values
    /// This is meant for any data types that are
    /// Nullable and or unmanaged
    /// Accepts IDisposable generic types as well
    /// </summary>
    /// <typeparam name="T">Generics</typeparam>
    /// <param name="inputValue">The initial value to set internally</param>
    public partial class NullableAtomicThreadSignal<T>( T? inputValue ) : IDisposable
    {
        private bool disposed = false;
#if NET9_0_OR_GREATER
        private readonly Lock internalLock = new();
#else
        private readonly object internalLock = new();
#endif
        private T? value = inputValue;
        private readonly ManualResetEventSlim threadSignal = new();

        ~NullableAtomicThreadSignal()
        {
            Dispose( false );
        }

        /// <summary>
        /// Blocks calling thread for timeout interval
        /// </summary>
        /// <param name="millisecondTimeout">Timeout you want to wait for signal state</param>
        /// <returns>Returns true if it gets set to signaled state during timeout, else false</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public bool WaitForSignaledState( int millisecondTimeout ) => threadSignal.Wait( millisecondTimeout );

        /// <summary>
        /// Blocks calling thread till returns signaled
        /// </summary>
        /// <returns>Returns true if it gets set to signaled state during timeout, else false</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public void WaitForSignaledState() => threadSignal.Wait();


        /// <summary>
        /// Gets the current thread signal
        /// Thread safe
        /// </summary>
        /// <returns>Returns true if its in a signaled state, else false</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public bool IsSignaled() => threadSignal.IsSet;



        /// <summary>
        /// Gets the current thread signal
        /// Thread safe
        /// </summary>
        /// <returns>Returns true if its in a non signaled state, else false</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public bool IsNonSignaled() => !threadSignal.IsSet;


        /// <summary>
        /// Sets to signaled state
        /// This will unblock other Threads
        /// <function cref="ManualResetEventSlim.Set"></function>
        /// </summary>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public void SetSignaled()
        {
            threadSignal.Set();
        }

        /// <summary>
        /// This will unblock other Threads
        /// Sets to signaled state
        /// Set the internal value
        /// <function cref="ManualResetEventSlim.Set"></function>
        /// <param name="newValue">New value to be set internally</param>
        /// </summary>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public void SetSignaled( T newValue )
        {
            threadSignal.Set();
            SetValue( newValue );
        }


        /// <summary>
        /// Sets to non signaled state
        /// This will Block other Threads
        /// <function cref="ManualResetEventSlim.Reset"></function>
        /// </summary>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public void SetNonSignaled()
        {
            threadSignal.Reset();
        }


        /// <summary>
        /// This will unblock other Threads
        /// Sets to non signaled state
        /// This will Block other Threads
        /// <function cref="ManualResetEventSlim.Reset"></function>
        /// <param name="newValue">New value to be set internally</param> 
        /// </summary>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public void SetNonSignaled( T newValue )
        {
            threadSignal.Reset();
            SetValue( newValue );
        }


        /// <summary>
        /// Calling dispose on interal storage if we can
        /// </summary>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        private void DisposeInternalStorage()
        {
            if ( value is IDisposable disposal )
            {
                disposal?.Dispose();
            }
        }


        /// <summary>
        /// Set the internal value
        /// Disposes old object before assigning new object
        /// </summary>
        /// <param name="newValue">New value to be set internally</param>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public void SetValue( T newValue )
        {
            using var sL = this.ScopeLock();
            DisposeInternalStorage();
            value = newValue;

        }


        /// <summary>
        /// Gets the internal value
        /// </summary>
        /// <returns>Resturns current internal value</returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public T? GetValue()
        {
            T? oldValue;
            using var sL = this.ScopeLock();
            oldValue = value;
            return oldValue;
        }



#if NET9_0_OR_GREATER
        /// <summary>
        /// Enter a scope lock for the atomic operations.
        /// This will wait until the lock is available.
        /// Dispose the scope lock to release the lock.
        /// </summary>
        /// <returns><see cref="System.Threading.Lock.Scope"/></returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public System.Threading.Lock.Scope ScopeLock()
        {
            return internalLock.EnterScope();
        }
#else

        /// <summary>
        /// Enter a scope lock for the atomic operations.
        /// This will wait until the lock is available.
        /// Dispose the scope lock to release the lock.
        /// </summary>
        /// <returns><see cref="System.Threading.Lock.Scope"/></returns>
        [MethodImpl( MethodImplOptions.AggressiveInlining )]
        public AtomicScopeLock ScopeLock()
        {
            return new AtomicScopeLock( internalLock );
        }

#endif

        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }

        protected virtual void Dispose( bool disposing )
        {
            if ( !disposed &&
                disposing )
            {
                threadSignal.Dispose();
                DisposeInternalStorage();
            }
            disposed = true;
        }
    }
}
