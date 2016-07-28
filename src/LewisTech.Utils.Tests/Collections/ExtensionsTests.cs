using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LewisTech.Utils.Collections;
using NUnit.Framework;

namespace LewisTech.Utils.Tests.Collections
{
    [TestFixture]
    class ExtensionsTests
    {

        private int[] TestIntArray;
        private TestObj[] TestObjArray;

        [SetUp]
        public void Setup()
        {
            var rnd = new Random();

            TestIntArray = Enumerable.Range(0, 10000).Select(m => rnd.Next(0, int.MaxValue)).Distinct().OrderBy(m => m).ToArray();
            TestObjArray = Enumerable.Range(0, 10000).Select(m => rnd.Next(0, int.MaxValue)).Distinct().OrderBy(m => m).Select(m => new TestObj { Int = m }).ToArray();
        }


        [Test]
        public void BinarySearchInt_FindsMatch()
        {

            for (var x = 0; x < TestIntArray.Length; x++)
            {

                var testMatch = TestIntArray[x];

                for (var y = 0; y < 10; y++)
                {
                    var match = Array.BinarySearch(TestIntArray, testMatch);
                    Assert.AreEqual(x, match);
                }
            }

        }

        [Test]
        public void BinarySearchInt_UnsortedDoesntFindMatch()
        {
            var testArray = TestIntArray.Randomize().ToList();

            var testMatch = testArray[1342];

            var ex = Assert.Throws<Exception>(
                () =>
                    {
                        var match = testArray.BinarySearch(m => m, testMatch);
                    });

            Assert.AreEqual("Array not sorted", ex.Message);
        }

        [Test]
        public void BinarySearchObj_FindsMatch()
        {

            for (int x = 0; x < TestObjArray.Length; x++)
            {

                var testMatch = TestObjArray[x];

                for (var y = 0; y < 10; y++)
                {
                    var match = TestObjArray.BinarySearch(m => m.Int, testMatch.Int);
                    Assert.AreEqual(testMatch.Int, match.Int);

                }
            }

        }

    }

    internal class TestObj
    {
        public int Int { get; set; }
    }

    //public class CustomComparer<T> : IComparer<T>
    //    {
    //        public int Compare(T x, T y)
    //        {

    //        }
    //    }
}
