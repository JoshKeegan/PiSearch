using System;

namespace UnitTests.Legacy
{
    /// <summary>
    /// For test fixtures that can consume large amounts of memory, force GC to run after them.
    /// This frees up as much memory as possible for the next test, and also keeps GC runtime variability out of tests
    /// that didn't create the objects being collected.
    /// </summary>
    public class ForceGcBetweenTests : IDisposable
    {
        public void Dispose()
        {
            GC.Collect();
        }
    }
}
