using System;
using NUnit.Framework;

namespace Tao.FixedPoint.UnityTest
{
    /// <summary>
    /// 指数与对数函数测试：Log2, Ln, Log10, Exp, Exp2
    /// </summary>
    [TestFixture]
    public class MathExpLogTests
    {
        #region Log2

        [Test]
        public void Log2_One_ReturnsZero()
        {
            TestHelper.AssertApprox(Math.Log2(FixedPoint.One), 0.0, 0.002);
        }

        [TestCase(2, 1.0)]
        [TestCase(4, 2.0)]
        [TestCase(8, 3.0)]
        [TestCase(16, 4.0)]
        public void Log2_PowerOfTwo_Exact(int input, double expected)
        {
            TestHelper.AssertApprox(Math.Log2(new FixedPoint(input)), expected, 0.002);
        }

        [Test]
        public void Log2_LessThanOne_ReturnsNegative()
        {
            FixedPoint half = new FixedPoint(0.5);
            TestHelper.AssertApprox(Math.Log2(half), -1.0, 0.01);
        }

        [Test]
        public void Log2_Zero_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                Math.Log2(FixedPoint.Zero);
            });
        }

        [Test]
        public void Log2_Negative_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                Math.Log2(new FixedPoint(-1));
            });
        }

        #endregion

        #region Ln

        [Test]
        public void Ln_One_ReturnsZero()
        {
            TestHelper.AssertApprox(Math.Ln(FixedPoint.One), 0.0, 0.01);
        }

        [Test]
        public void Ln_E_ReturnsApproxOne()
        {
            FixedPoint e = new FixedPoint(2.718);
            TestHelper.AssertApprox(Math.Ln(e), 1.0, 0.02);
        }

        [Test]
        public void Ln_Two_ReturnsLn2()
        {
            TestHelper.AssertApprox(Math.Ln(new FixedPoint(2)), 0.693, 0.01);
        }

        #endregion

        #region Log10

        [Test]
        public void Log10_One_ReturnsZero()
        {
            TestHelper.AssertApprox(Math.Log10(FixedPoint.One), 0.0, 0.01);
        }

        [Test]
        public void Log10_Ten_ReturnsOne()
        {
            TestHelper.AssertApprox(Math.Log10(new FixedPoint(10)), 1.0, 0.02);
        }

        [Test]
        public void Log10_Hundred_ReturnsTwo()
        {
            TestHelper.AssertApprox(Math.Log10(new FixedPoint(100)), 2.0, 0.02);
        }

        #endregion

        #region Exp

        [Test]
        public void Exp_Zero_ReturnsOne()
        {
            Assert.AreEqual(FixedPoint.One, Math.Exp(FixedPoint.Zero));
        }

        [Test]
        public void Exp_One_ReturnsE()
        {
            TestHelper.AssertApprox(Math.Exp(FixedPoint.One), System.Math.E, 0.02);
        }

        [Test]
        public void Exp_Negative_ReturnsLessThanOne()
        {
            FixedPoint result = Math.Exp(FixedPoint.NegativeOne);
            Assert.IsTrue(result > FixedPoint.Zero);
            Assert.IsTrue(result < FixedPoint.One);
            TestHelper.AssertApprox(result, 1.0 / System.Math.E, 0.02);
        }

        [Test]
        public void Exp_LargePositive_Saturates()
        {
            FixedPoint result = Math.Exp(new FixedPoint(100));
            Assert.AreEqual(FixedPoint.MaxValue, result);
        }

        [Test]
        public void Exp_LargeNegative_ApproachesZero()
        {
            FixedPoint result = Math.Exp(new FixedPoint(-100));
            Assert.AreEqual(FixedPoint.Zero, result);
        }

        [Test]
        public void Exp_Ln_RoundTrip()
        {
            FixedPoint original = new FixedPoint(3);
            FixedPoint roundTrip = Math.Exp(Math.Ln(original));
            TestHelper.AssertApprox(roundTrip, 3.0, 0.05);
        }

        #endregion

        #region Exp2

        [Test]
        public void Exp2_Zero_ReturnsOne()
        {
            TestHelper.AssertApprox(Math.Exp2(FixedPoint.Zero), 1.0, 0.002);
        }

        [Test]
        public void Exp2_One_ReturnsTwo()
        {
            TestHelper.AssertApprox(Math.Exp2(FixedPoint.One), 2.0, 0.02);
        }

        [Test]
        public void Exp2_Three_ReturnsEight()
        {
            TestHelper.AssertApprox(Math.Exp2(new FixedPoint(3)), 8.0, 0.1);
        }

        #endregion
    }
}
