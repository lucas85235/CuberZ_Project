using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class ExempleSuiteTest
    {
        private int ReturnFive() { return 5; }

        // A Test behaves as an ordinary method
        [Test]
        public void ExempleSuiteTestSimplePasses()
        {
            // Use the Assert class to test conditions

            Assert.Greater(10, ReturnFive());
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator ExempleSuiteTestWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.

            int numberFive = 5;

            yield return null;

            Assert.AreEqual(numberFive, 5);
        }
    }
}
