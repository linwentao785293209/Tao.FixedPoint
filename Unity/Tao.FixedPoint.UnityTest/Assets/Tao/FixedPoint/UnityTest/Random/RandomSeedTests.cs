using NUnit.Framework;

namespace Tao.FixedPoint.UnityTest
{
    /// <summary>
    /// RandomSeed 测试：确定性、范围约束、种子差异性
    /// </summary>
    [TestFixture]
    public class RandomSeedTests
    {
        #region 确定性

        [Test]
        public void SameSeed_ProducesSameSequence()
        {
            RandomSeed rng1 = new RandomSeed(42);
            RandomSeed rng2 = new RandomSeed(42);

            for (int i = 0; i < 100; i++)
            {
                Assert.AreEqual(rng1.Range(0, 1000), rng2.Range(0, 1000),
                    $"Sequence diverged at iteration {i}");
            }
        }

        [Test]
        public void DifferentSeed_ProducesDifferentSequence()
        {
            RandomSeed rng1 = new RandomSeed(42);
            RandomSeed rng2 = new RandomSeed(99);

            bool anyDifferent = false;
            for (int i = 0; i < 100; i++)
            {
                if (rng1.Range(0, 1000) != rng2.Range(0, 1000))
                {
                    anyDifferent = true;
                    break;
                }
            }

            Assert.IsTrue(anyDifferent, "Different seeds should produce different sequences");
        }

        #endregion

        #region Range (int)

        [Test]
        public void Range_Int_WithinBounds()
        {
            RandomSeed rng = new RandomSeed(123);
            for (int i = 0; i < 1000; i++)
            {
                int value = rng.Range(5, 15);
                Assert.IsTrue(value >= 5 && value < 15,
                    $"Value {value} out of [5, 15) at iteration {i}");
            }
        }

        [Test]
        public void Range_Int_MinEqualsMax_ReturnsMin()
        {
            RandomSeed rng = new RandomSeed(42);
            Assert.AreEqual(5, rng.Range(5, 5));
            Assert.AreEqual(10, rng.Range(10, 3));
        }

        [Test]
        public void Range_Int_NegativeRange()
        {
            RandomSeed rng = new RandomSeed(42);
            for (int i = 0; i < 100; i++)
            {
                int value = rng.Range(-10, 10);
                Assert.IsTrue(value >= -10 && value < 10);
            }
        }

        #endregion

        #region Range (FixedPoint)

        [Test]
        public void Range_FixedPoint_ReturnsInt()
        {
            RandomSeed rng = new RandomSeed(42);
            FixedPoint min = new FixedPoint(0);
            FixedPoint max = new FixedPoint(10);
            for (int i = 0; i < 100; i++)
            {
                int value = rng.Range(min, max);
                Assert.IsTrue(value >= 0 && value < 10,
                    $"Value {value} out of range at iteration {i}");
            }
        }

        #endregion

        #region 种子

        [Test]
        public void SeedId_ReturnsConstructorValue()
        {
            RandomSeed rng = new RandomSeed(12345);
            Assert.AreEqual(12345, rng.SeedId);
        }

        [Test]
        public void ZeroSeed_DoesNotCrash()
        {
            RandomSeed rng = new RandomSeed(0);
            int value = rng.Range(0, 100);
            Assert.IsTrue(value >= 0 && value < 100);
        }

        #endregion
    }
}
