using System.Diagnostics;
using SCB.Atomics;

namespace ConsoleApp1
{
    public static class AtomicsTester
    {

        // Unsafe variable
        private static long unsafeCounter = 0;

        public static void RunTest( int threadCount, int operationsPerThread )
        {
            Console.WriteLine( $"Starting test with {threadCount} threads, {operationsPerThread} operations each" );

            // Reset counters
            AtomicInt32 atomicInt32 = new AtomicInt32( 0 );
            AtomicInt64 atomicInt64 = new AtomicInt64( 0L );
            unsafeCounter = 0;

            // Create and start threads for atomic operations
            var threads = new List<Thread>();
            for ( int i = 0; i < threadCount; i++ )
            {
                var t = new Thread( () =>
                {
                    for ( int j = 0; j < operationsPerThread; j++ )
                    {
                        atomicInt32.Increment( 1 );
                        atomicInt64.Increment( 1L );
                    }
                } );
                threads.Add( t );
                t.Start();
            }

            // Create and start threads for non-atomic operations
            var unsafeThreads = new List<Thread>();
            for ( int i = 0; i < threadCount; i++ )
            {
                var t = new Thread( () =>
                {
                    for ( int j = 0; j < operationsPerThread; j++ )
                    {
                        Interlocked.Increment( ref unsafeCounter ); // Using Interlocked for comparison
                    }
                } );
                unsafeThreads.Add( t );
                t.Start();
            }

            // Wait for all threads to complete
            foreach ( var t in threads )
                t.Join();
            foreach ( var t in unsafeThreads )
                t.Join();

            // Verify results
            long expectedValue = threadCount * operationsPerThread;
            int int32Value = atomicInt32.Read();
            long int64Value = atomicInt64.Read();


            Console.WriteLine( $"Atomic counter: {int32Value}, Expected: {expectedValue}" );
            Console.WriteLine( $"Atomic counter: {int64Value}, Expected: {expectedValue}" );
            Console.WriteLine( $"Control counter: {unsafeCounter}, Expected: {expectedValue}" );

            // Test result
            bool success = int32Value == expectedValue && int64Value == expectedValue && unsafeCounter == expectedValue;

            Console.WriteLine( $"Test result: {( success ? "PASSED" : "FAILED" )}" );

            atomicInt32.Dispose();
            atomicInt64.Dispose();
        }

        // 2.A High Contention Testing
        public static void HighContentionTest()
        {
            AtomicInt64 sharedCounter = new AtomicInt64( 0L );
            int threadCount = Environment.ProcessorCount * 4; // Multiply by 4 to ensure oversubscription
            int operationsPerThread = 100000;

            Console.WriteLine( $"High contention test: {threadCount} threads all updating the same variable" );

            var threads = new Thread[ threadCount ];
            var stopwatch = Stopwatch.StartNew();

            for ( int i = 0; i < threadCount; i++ )
            {
                threads[ i ] = new Thread( () =>
                {
                    for ( int j = 0; j < operationsPerThread; j++ )
                    {
                        sharedCounter.Increment( 1L );
                    }
                } );
                threads[ i ].Start();
            }

            foreach ( var thread in threads )
            {
                thread.Join();
            }

            stopwatch.Stop();

            long result = sharedCounter.Read();
            long expected = threadCount * operationsPerThread;
            Console.WriteLine( $"Final value: {result}, Expected: {expected}" );
            Console.WriteLine( $"Time taken: {stopwatch.ElapsedMilliseconds}ms" );
            Console.WriteLine( $"Test result: {( result == expected ? "PASSED" : "FAILED" )}" );

            sharedCounter.Dispose();
        }

        // 2.B Compare-And-Exchange Testing
        public static void CompareAndExchangeTest()
        {
            AtomicInt64 value = new AtomicInt64( 100L );
            int successCount = 0;
            int failCount = 0;
            int threadCount = 8;

            var threads = new Thread[ threadCount ];

            for ( int i = 0; i < threadCount; i++ )
            {
                int threadId = i;
                threads[ i ] = new Thread( () =>
                {
                    for ( int j = 0; j < 1000; j++ )
                    {
                        long current = value.Read();
                        long newValue = current + threadId + 1;

                        long previousValue = value.CompareExchange( newValue, current );
                        bool success = previousValue == current;

                        if ( success )
                            Interlocked.Increment( ref successCount );
                        else
                            Interlocked.Increment( ref failCount );

                        // Small delay to increase chance of race conditions
                        if ( j % 10 == 0 )
                            Thread.Sleep( 1 );
                    }
                } );
            }

            foreach ( var thread in threads )
            {
                thread.Start();
            }

            foreach ( var thread in threads )
            {
                thread.Join();
            }

            Console.WriteLine( $"CAS Operations - Successful: {successCount}, Failed: {failCount}" );
            Console.WriteLine( $"Final value: {value.Read()}" );

            value.Dispose();
        }

        // 2.C Testing Complex Operations
        public static void ComplexOperationsTest()
        {
            AtomicInt64 value = new AtomicInt64( 1L );
            int threadCount = 4;

            var threads = new Thread[ threadCount ];

            for ( int i = 0; i < threadCount; i++ )
            {
                threads[ i ] = new Thread( () =>
                {
                    for ( int j = 0; j < 10000; j++ )
                    {
                        // Mix of operations to test
                        switch ( j % 4 )
                        {
                            case 0:
                            value.Add( 2L );
                            break;
                            case 1:
                            // Multiply by 2
                            long currentVal = value.Read();
                            while ( true )
                            {
                                long newVal = currentVal * 2L;
                                if ( value.CompareExchange( newVal, currentVal ) == currentVal )
                                    break;
                                currentVal = value.Read();
                            }
                            break;
                            case 2:
                            value.Add( -1L ); // Subtract 1
                            break;
                            case 3:
                            // BitwiseOr with 4
                            currentVal = value.Read();
                            while ( true )
                            {
                                long newVal = currentVal | 4L;
                                if ( value.CompareExchange( newVal, currentVal ) == currentVal )
                                    break;
                                currentVal = value.Read();
                            }
                            break;
                        }
                    }
                } );
            }

            foreach ( var thread in threads )
            {
                thread.Start();
            }

            foreach ( var thread in threads )
            {
                thread.Join();
            }

            Console.WriteLine( $"After complex operations, final value: {value.Read()}" );

            value.Dispose();
        }

        // 3.A Delay Injection Test
        public static void DelayInjectionTest()
        {
            Console.WriteLine( "\n=======================================" );
            Console.WriteLine( "3.A Delay Injection Test" );
            Console.WriteLine( "=======================================" );

            AtomicInt64 atomic = new AtomicInt64( 0L );

            // Use volatile to prevent some compiler optimizations
            // but this won't make it thread-safe
            long volatileNonAtomic = 0;

            // More threads = more contention
            int threadCount = Environment.ProcessorCount * 2; // Use multiple threads based on CPU count
            int operationsPerThread = 1000; // More operations
            int expectedTotal = threadCount * operationsPerThread;

            Console.WriteLine( $"Running with {threadCount} threads, {operationsPerThread} operations each" );

            // Create a barrier to ensure all threads start at roughly the same time
            // This increases the chance of race conditions
            using var startBarrier = new CountdownEvent( threadCount * 2 );

            var threads = new Thread[ threadCount ];
            var nonAtomicThreads = new Thread[ threadCount ];

            // Test with artificial delays and busy work to force race conditions
            for ( int i = 0; i < threadCount; i++ )
            {
                threads[ i ] = new Thread( () =>
                {
                    // Signal ready and wait for all threads
                    startBarrier.Signal();
                    startBarrier.Wait();

                    for ( int j = 0; j < operationsPerThread; j++ )
                    {

                        // Do some busy work to increase chance of contention
                        if ( j % 10 == 0 )
                        {
                            // Sporadic delays of varying lengths to increase race condition chances
                            Thread.Yield();
                            if ( j % 50 == 0 )
                                Thread.Sleep( 1 );
                        }

                        // This should be atomic and safe
                        atomic.Increment( 1L );
                    }
                } );

                nonAtomicThreads[ i ] = new Thread( () =>
                {
                    // Signal ready and wait for all threads
                    startBarrier.Signal();
                    startBarrier.Wait();

                    for ( int j = 0; j < operationsPerThread; j++ )
                    {
                        // Read-modify-write pattern with non-atomic operation
                        long current = volatileNonAtomic;

                        // Do some busy work to increase chance of contention
                        if ( j % 10 == 0 )
                        {
                            // Sporadic delays of varying lengths to increase race condition chances
                            Thread.Yield();
                            if ( j % 50 == 0 )
                                Thread.Sleep( 1 );
                        }

                        // This will have race conditions
                        volatileNonAtomic = current + 1L;
                    }
                } );
            }

            Console.WriteLine( "Starting threads..." );

            // Start all threads
            foreach ( var t in threads )
                t.Start();
            foreach ( var t in nonAtomicThreads )
                t.Start();

            // Wait for all threads to complete
            foreach ( var t in threads )
                t.Join();
            foreach ( var t in nonAtomicThreads )
                t.Join();

            // Check results
            long atomicResult = atomic.Read();
            long nonAtomicResult = Volatile.Read( ref volatileNonAtomic );

            Console.WriteLine( $"Atomic result: {atomicResult}, Expected: {expectedTotal}" );
            Console.WriteLine( $"Non-atomic result: {nonAtomicResult}, Expected: {expectedTotal}" );

            bool atomicPass = atomicResult == expectedTotal;
            bool nonAtomicPass = nonAtomicResult == expectedTotal;

            Console.WriteLine( $"Atomic Test Result: {( atomicPass ? "PASSED" : "FAILED" )}" );
            Console.WriteLine( $"Non-atomic Test Result: {( nonAtomicPass ? "PASSED" : "FAILED" )}" );

            atomic.Dispose();
        }

        // 3.B CPU Bound Test
        public static void CpuBoundTest()
        {
            AtomicInt64 atomic = new AtomicInt64( 0L );

            // Start many threads that will compete for CPU time
            int threadCount = Environment.ProcessorCount * 8;
            var threads = new Thread[ threadCount ];

            for ( int i = 0; i < threadCount; i++ )
            {
                threads[ i ] = new Thread( () =>
                {
                    // Perform lots of operations without yielding
                    for ( int j = 0; j < 100000; j++ )
                    {
                        atomic.Increment( 1L );

                        // Make the thread CPU bound with computation
                        if ( j % 100 == 0 )
                        {
                            double x = 0;
                            for ( int k = 0; k < 1000; k++ )
                            {
                                x = Math.Sqrt( x + k );
                            }
                        }
                    }
                } );

                threads[ i ].Priority = ThreadPriority.Highest; // Set high priority to force context switching
            }

            foreach ( var t in threads )
                t.Start();
            foreach ( var t in threads )
                t.Join();

            long expected = threadCount * 100000;
            Console.WriteLine( $"Final value: {atomic.Read()}, Expected: {expected}" );
            Console.WriteLine( $"Test result: {( atomic.Read() == expected ? "PASSED" : "FAILED" )}" );

            atomic.Dispose();
        }


        // 5. Stress Testing with Random Operations
        public static void StressTest( TimeSpan duration )
        {
            AtomicInt64 value = new AtomicInt64( 0L );
            long expectedSum = 0;
            int operationCount = 0;
            bool running = true;

            Console.WriteLine( $"Starting stress test for {duration.TotalSeconds} seconds" );

            // Start multiple threads that perform random operations
            int threadCount = Environment.ProcessorCount * 2;
            var threads = new Thread[ threadCount ];

            var stopwatch = Stopwatch.StartNew();
            var random = new Random();


            for ( int i = 0; i < threadCount; i++ )
            {
                threads[ i ] = new Thread( () =>
                {
                    var localRandom = new Random( random.Next() );
                    long localSum = 0;
                    int localCount = 0;

                    while ( running )
                    {
                        // Choose a random operation
                        int op = localRandom.Next( 4 );
                        long delta = ( ( long ) localRandom.Next( 10 ) - 5 );// Random value between -5 and 4

                        switch ( op )
                        {
                            case 0:
                            value.Add( delta );
                            localSum += delta;
                            break;
                            case 1:
                            value.Add( -delta );
                            localSum += -delta;
                            break;
                            case 2:
                            value.Subtract( threadCount );
                            localSum -= threadCount;
                            break;
                            case 3:
                            long currentVal = value.Read();
                            if ( value.CompareExchange( currentVal + delta, currentVal ) == currentVal )
                            {
                                localSum += delta;
                            }
                            break;
                        }

                        localCount++;
                    }

                    Interlocked.Add( ref expectedSum, localSum );
                    Interlocked.Add( ref operationCount, localCount );
                } );

                threads[ i ].Start();
            }

            // Run for the specified duration
            Thread.Sleep( duration );
            running = false;

            foreach ( var t in threads )
                t.Join();
            stopwatch.Stop();

            long atomicResult = value.Read();

            Console.WriteLine( $"Stress test complete: {operationCount} operations in {stopwatch.ElapsedMilliseconds}ms" );
            Console.WriteLine( $"Final value: {atomicResult}, Expected sum of operations: {expectedSum}" );
            Console.WriteLine( $"Result: {( atomicResult == expectedSum ? "PASSED" : "FAILED" )}" );

            value.Dispose();
        }

        // Method to run all tests
        public static void RunAllTests()
        {
            Console.WriteLine( "=======================================" );
            Console.WriteLine( "1. Basic Thread Safety Test" );
            Console.WriteLine( "=======================================" );
            RunTest( 16, 10000 );

            Console.WriteLine( "\n=======================================" );
            Console.WriteLine( "2.A High Contention Test" );
            Console.WriteLine( "=======================================" );
            HighContentionTest();

            Console.WriteLine( "\n=======================================" );
            Console.WriteLine( "2.B Compare-And-Exchange Test" );
            Console.WriteLine( "=======================================" );
            CompareAndExchangeTest();

            Console.WriteLine( "\n=======================================" );
            Console.WriteLine( "2.C Complex Operations Test" );
            Console.WriteLine( "=======================================" );
            ComplexOperationsTest();

            Console.WriteLine( "\n=======================================" );
            Console.WriteLine( "3.A Delay Injection Test" );
            Console.WriteLine( "=======================================" );
            DelayInjectionTest();

            Console.WriteLine( "\n=======================================" );
            Console.WriteLine( "3.B CPU Bound Test" );
            Console.WriteLine( "=======================================" );
            CpuBoundTest();

            Console.WriteLine( "\n=======================================" );
            Console.WriteLine( "5. Stress Test (10 seconds)" );
            Console.WriteLine( "=======================================" );
            StressTest( TimeSpan.FromSeconds( 10 ) );

            Console.WriteLine( "\n=======================================" );
            Console.WriteLine( "All tests completed" );
            Console.WriteLine( "=======================================" );
        }
    }




    public class WatchTester
    {
        // Test the accuracy of MicroSleep by comparing expected vs actual delays
        public static void TestMicroSleepAccuracy()
        {
            Console.WriteLine( "Testing MicroSleep accuracy at various durations..." );

            // Test different microsecond values
            double[] testDurations = { 1, 10, 50, 100, 500, 1000, 5000, 10000 };
            int trials = 5; // Multiple trials for each duration to account for system fluctuations

            Console.WriteLine( "\n{0,-15} {1,-15} {2,-15} {3,-15}", "Target (µs)", "Avg Actual (µs)", "Error (%)", "Consistency (%)" );
            Console.WriteLine( new string( '-', 60 ) );

            foreach ( double duration in testDurations )
            {
                double totalMicroseconds = 0;
                double[] measurements = new double[ trials ];

                for ( int i = 0; i < trials; i++ )
                {
                    Stopwatch sw = Stopwatch.StartNew();
                    Watch.MicroSleep( duration );
                    sw.Stop();

                    // Convert ticks to microseconds
                    double actualMicroseconds = sw.ElapsedTicks * 1000000.0 / Stopwatch.Frequency;
                    measurements[ i ] = actualMicroseconds;
                    totalMicroseconds += actualMicroseconds;
                }

                // Calculate statistics
                double avgMicroseconds = totalMicroseconds / trials;
                double errorPercent = Math.Abs( ( avgMicroseconds - duration ) / duration * 100 );

                // Calculate consistency (standard deviation as percentage of mean)
                double sumSquaredDiffs = 0;
                foreach ( double measurement in measurements )
                {
                    sumSquaredDiffs += Math.Pow( measurement - avgMicroseconds, 2 );
                }
                double stdDev = Math.Sqrt( sumSquaredDiffs / trials );
                double consistencyPercent = ( stdDev / avgMicroseconds ) * 100;

                Console.WriteLine( "{0,-15:F2} {1,-15:F2} {2,-15:F2} {3,-15:F2}",
                    duration, avgMicroseconds, errorPercent, consistencyPercent );
            }
        }

        // Test CPU usage patterns during different types of sleep
        public static void TestCpuUsagePatterns()
        {
            Console.WriteLine( "\nTesting CPU usage patterns during high precision delays..." );

            TimeSpan testDuration = TimeSpan.FromSeconds( 2 );

            // Measure operations per second for different delay types
            Console.WriteLine( "\nMeasuring operations per second with different delay types:" );
            Console.WriteLine( "{0,-15} {1,-15}", "Delay Type", "Ops per Second" );
            Console.WriteLine( new string( '-', 30 ) );

            // Test 1: No delay (baseline)
            {
                int count = 0;
                Stopwatch sw = Stopwatch.StartNew();
                while ( sw.Elapsed < testDuration )
                {
                    count++;
                }
                double opsPerSecond = count / testDuration.TotalSeconds;
                Console.WriteLine( "{0,-15} {1,-15:N0}", "No delay", opsPerSecond );
            }

            // Test 2: Thread.Sleep(0)
            {
                int count = 0;
                Stopwatch sw = Stopwatch.StartNew();
                while ( sw.Elapsed < testDuration )
                {
                    Thread.Sleep( 0 );
                    count++;
                }
                double opsPerSecond = count / testDuration.TotalSeconds;
                Console.WriteLine( "{0,-15} {1,-15:N0}", "Thread.Sleep(0)", opsPerSecond );
            }

            // Test 3: Thread.Sleep(1)
            {
                int count = 0;
                Stopwatch sw = Stopwatch.StartNew();
                while ( sw.Elapsed < testDuration )
                {
                    Thread.Sleep( 1 );
                    count++;
                }
                double opsPerSecond = count / testDuration.TotalSeconds;
                Console.WriteLine( "{0,-15} {1,-15:N0}", "Thread.Sleep(1)", opsPerSecond );
            }

            // Test 4: Watch.MicroSleep(1)
            {
                int count = 0;
                Stopwatch sw = Stopwatch.StartNew();
                while ( sw.Elapsed < testDuration )
                {
                    Watch.MicroSleep( 1 );
                    count++;
                }
                double opsPerSecond = count / testDuration.TotalSeconds;
                Console.WriteLine( "{0,-15} {1,-15:N0}", "MicroSleep(1)", opsPerSecond );
            }

            // Test 5: Watch.MicroSleep(50)
            {
                int count = 0;
                Stopwatch sw = Stopwatch.StartNew();
                while ( sw.Elapsed < testDuration )
                {
                    Watch.MicroSleep( 50 );
                    count++;
                }
                double opsPerSecond = count / testDuration.TotalSeconds;
                Console.WriteLine( "{0,-15} {1,-15:N0}", "MicroSleep(50)", opsPerSecond );
            }

            // Test 6: Watch.MicroSleep(1000)
            {
                int count = 0;
                Stopwatch sw = Stopwatch.StartNew();
                while ( sw.Elapsed < testDuration )
                {
                    Watch.MicroSleep( 1000 );
                    count++;
                }
                double opsPerSecond = count / testDuration.TotalSeconds;
                Console.WriteLine( "{0,-15} {1,-15:N0}", "MicroSleep(1000)", opsPerSecond );
            }
        }

        // Test the relative precision between different sleep methods
        public static void TestSleepMethodPrecision()
        {
            Console.WriteLine( "\nComparing precision between different sleep methods..." );

            // Target sleep time in milliseconds
            double targetMs = 1.0;
            int trials = 100;

            // Test Thread.Sleep
            double threadSleepTotalError = 0;
            for ( int i = 0; i < trials; i++ )
            {
                Stopwatch sw = Stopwatch.StartNew();
                Thread.Sleep( 1 ); // 1ms is minimum for Thread.Sleep
                sw.Stop();
                double actualMs = sw.ElapsedMilliseconds;
                threadSleepTotalError += Math.Abs( actualMs - targetMs );
            }
            double threadSleepAvgError = threadSleepTotalError / trials;

            // Test Watch.MilliSleep
            double watchMilliSleepTotalError = 0;
            for ( int i = 0; i < trials; i++ )
            {
                Stopwatch sw = Stopwatch.StartNew();
                Watch.MilliSleep( targetMs );
                sw.Stop();
                double actualMs = sw.ElapsedMilliseconds + ( sw.ElapsedTicks % Stopwatch.Frequency ) * 1000.0 / Stopwatch.Frequency;
                watchMilliSleepTotalError += Math.Abs( actualMs - targetMs );
            }
            double watchMilliSleepAvgError = watchMilliSleepTotalError / trials;

            // Test Watch.MicroSleep
            double watchMicroSleepTotalError = 0;
            for ( int i = 0; i < trials; i++ )
            {
                Stopwatch sw = Stopwatch.StartNew();
                Watch.MicroSleep( targetMs * 1000 );
                sw.Stop();
                double actualMs = sw.ElapsedTicks * 1000.0 / Stopwatch.Frequency;
                watchMicroSleepTotalError += Math.Abs( actualMs - targetMs );
            }
            double watchMicroSleepAvgError = watchMicroSleepTotalError / trials;

            Console.WriteLine( "\nAverage error from target of {0}ms over {1} trials:", targetMs, trials );
            Console.WriteLine( "{0,-20} {1,-15:F6}ms", "Thread.Sleep", threadSleepAvgError );
            Console.WriteLine( "{0,-20} {1,-15:F6}ms", "Watch.MilliSleep", watchMilliSleepAvgError );
            Console.WriteLine( "{0,-20} {1,-15:F6}ms", "Watch.MicroSleep", watchMicroSleepAvgError );

            Console.WriteLine( "\nRelative precision improvements:" );
            if ( threadSleepAvgError > 0 )
            {
                Console.WriteLine( "MilliSleep is {0:F2}x more precise than Thread.Sleep",
                                  threadSleepAvgError / watchMilliSleepAvgError );
                Console.WriteLine( "MicroSleep is {0:F2}x more precise than Thread.Sleep",
                                  threadSleepAvgError / watchMicroSleepAvgError );
            }
        }

        // Test different variant methods (NanoSleep, MicroSleep, MilliSleep, SecondsSleep)
        public static void TestSleepVariants()
        {
            Console.WriteLine( "\nTesting different sleep variant methods..." );

            // Test equivalent values across different methods
            double[][] testCases = new double[][]
            {
            new double[] { 1000000, 1000, 1, 0.001 },    // 1ms
            new double[] { 10000000, 10000, 10, 0.01 },  // 10ms
            new double[] { 100000000, 100000, 100, 0.1 } // 100ms
            };

            Console.WriteLine( "\n{0,-15} {1,-15} {2,-15} {3,-15} {4,-15}",
                "Target (ms)", "NanoSleep", "MicroSleep", "MilliSleep", "SecondsSleep" );
            Console.WriteLine( new string( '-', 75 ) );

            foreach ( double[] testCase in testCases )
            {
                double targetMs = testCase[ 2 ]; // Use millisecond value as the target

                // Test NanoSleep
                Stopwatch sw = Stopwatch.StartNew();
                Watch.NanoSleep( testCase[ 0 ] );
                sw.Stop();
                double nanoSleepMs = sw.ElapsedTicks * 1000.0 / Stopwatch.Frequency;

                // Test MicroSleep
                sw.Restart();
                Watch.MicroSleep( testCase[ 1 ] );
                sw.Stop();
                double microSleepMs = sw.ElapsedTicks * 1000.0 / Stopwatch.Frequency;

                // Test MilliSleep
                sw.Restart();
                Watch.MilliSleep( testCase[ 2 ] );
                sw.Stop();
                double milliSleepMs = sw.ElapsedTicks * 1000.0 / Stopwatch.Frequency;

                // Test SecondsSleep
                sw.Restart();
                Watch.SecondsSleep( testCase[ 3 ] );
                sw.Stop();
                double secondsSleepMs = sw.ElapsedTicks * 1000.0 / Stopwatch.Frequency;

                Console.WriteLine( "{0,-15:F2} {1,-15:F2} {2,-15:F2} {3,-15:F2} {4,-15:F2}",
                    targetMs, nanoSleepMs, microSleepMs, milliSleepMs, secondsSleepMs );
            }
        }

        // Test high-frequency timing (for applications like game loops or high-frequency trading)
        public static void TestHighFrequencyTiming()
        {
            Console.WriteLine( "\nTesting high-frequency timing capability..." );

            // Target frequencies to test (in Hz)
            int[] frequencies = { 1000, 2000, 5000, 10000 };
            int samplesToCollect = 1000;

            foreach ( int targetFrequency in frequencies )
            {
                double targetMicroseconds = 1000000.0 / targetFrequency;
                Console.WriteLine( $"\nTesting frequency: {targetFrequency}Hz (period: {targetMicroseconds:F2} µs)" );

                var intervals = new double[ samplesToCollect ];

                Stopwatch sw = Stopwatch.StartNew();
                long lastTicks = sw.ElapsedTicks;

                for ( int i = 0; i < samplesToCollect; i++ )
                {
                    Watch.MicroSleep( targetMicroseconds );

                    long currentTicks = sw.ElapsedTicks;
                    double intervalMicroseconds = ( currentTicks - lastTicks ) * 1000000.0 / Stopwatch.Frequency;
                    intervals[ i ] = intervalMicroseconds;
                    lastTicks = currentTicks;
                }

                // Calculate statistics
                double sum = intervals.Sum();
                double average = sum / intervals.Length;
                double variance = intervals.Select( x => Math.Pow( x - average, 2 ) ).Sum() / intervals.Length;
                double stdDev = Math.Sqrt( variance );
                double min = intervals.Min();
                double max = intervals.Max();
                double jitterPercent = ( stdDev / average ) * 100;

                Console.WriteLine( $"Average interval: {average:F2} µs (target: {targetMicroseconds:F2} µs)" );
                Console.WriteLine( $"Standard deviation: {stdDev:F2} µs (jitter: {jitterPercent:F2}%)" );
                Console.WriteLine( $"Min/Max: {min:F2}/{max:F2} µs, Range: {max - min:F2} µs" );
            }
        }

        // Run all Watch tests
        public static void RunAllTests()
        {
            Console.WriteLine( "=======================================" );
            Console.WriteLine( "High Precision Thread Delay Tests" );
            Console.WriteLine( "=======================================" );

            TestMicroSleepAccuracy();
            TestCpuUsagePatterns();
            TestSleepMethodPrecision();
            TestSleepVariants();
            TestHighFrequencyTiming();

            Console.WriteLine( "\n=======================================" );
            Console.WriteLine( "All Watch tests completed" );
            Console.WriteLine( "=======================================" );
        }
    }


    static class Program
    {
        internal static void Main( string[] args )
        {
            // Run all atomic tests
            AtomicsTester.RunAllTests();

            // Run all Watch tests
            WatchTester.RunAllTests();

            Console.WriteLine( "PRESS ANY KEY..." );
            System.Console.ReadKey();
        }
    }
}





