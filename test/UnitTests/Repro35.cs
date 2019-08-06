using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class Repro35
    {
        [Test]
        public void Fail() => Assert.Fail();
    }
}
