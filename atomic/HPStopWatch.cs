using System.Diagnostics;

namespace ConsoleApp1
{
    /// <summary>
    /// Custom sleep implementations
    /// </summary>
    internal static class Watch
    {

        /// <summary>
        ///  This is a generalization C# and other languages use much more complex ways to figure the optimal spin count
        /// </summary>
        private readonly static int optimalSpinCount = Environment.ProcessorCount * 10;


        /// <summary>
        /// Provides a precise microsecond-level sleep function that balances CPU usage and precision
        /// using active spinning, yielding, and dynamic contention-based spin adjustments.
        /// </summary>
        /// <param name="microSeconds">The duration to sleep, specified in microseconds.</param>
        /// <remarks>
        /// The method adapts its behavior based on the required sleep duration and system contention:
        /// <list type="bullet">
        ///   <item><description>For very short durations, it uses <see cref="Thread.SpinWait"/> with low contention to achieve minimal delay overhead.</description></item>
        ///   <item><description>For moderate durations, it dynamically adjusts spin count to balance precision and contention.</description></item>
        ///   <item><description>For higher durations (above 1.5 milliseconds), it yields the CPU using <see cref="Thread.Yield"/> to reduce contention and allow other threads to execute.</description></item>
        /// </list>
        /// This approach ensures efficient CPU usage under varying contention levels while maintaining precise sleep timing.
        /// </remarks>
        internal static void MicroSleep( double microSeconds )
        {
            Stopwatch sleepStopWatch = Stopwatch.StartNew();
            long ticks = ( long ) ( microSeconds * Stopwatch.Frequency / 1000000 );
            int spinCount = 0;
            while ( sleepStopWatch.ElapsedTicks < ticks )
            {
                spinCount++;

                if ( ticks > ( ( Stopwatch.Frequency / 1000 ) * 1.5 ) )
                {
                    Thread.Yield();
                } else if ( spinCount > optimalSpinCount )
                {
                    Thread.SpinWait( spinCount << 1 );
                } else if ( microSeconds > 0.01 )
                {
                    Thread.SpinWait( 1 );
                }
            }
        }

        /// <summary>
        /// Forwards sleep to micro sleep with nanoseconds count
        /// </summary>
        /// <param name="nanoSeconds">The number of nanoseconds to sleep.</param>
        internal static void NanoSleep( double nanoSeconds )
        {
            MicroSleep( nanoSeconds / 1000.0 );
        }

        /// <summary>
        /// Forwards sleep to micro sleep with seconds count
        /// </summary>
        /// <param name="seconds">The number of seconds to sleep.</param>
        internal static void SecondsSleep( double seconds )
        {
            MicroSleep( seconds * 1000000.0 );
        }

        /// <summary>
        /// Forwards sleep to micro sleep with milliseconds count
        /// </summary>
        /// <param name="milliseconds">The number of milliseconds to sleep.</param>
        internal static void MilliSleep( double milliseconds )
        {
            MicroSleep( milliseconds * 1000.0 );
        }
    }

}
