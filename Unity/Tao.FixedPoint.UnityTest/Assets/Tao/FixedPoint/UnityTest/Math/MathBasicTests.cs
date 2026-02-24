using System;
using NUnit.Framework;

namespace Tao.FixedPoint.UnityTest
{
    /// <summary>
    /// 基础数学函数测试：Abs, Min, Max, Sign, Clamp, Pow, Sqrt, RSqrt, Rcp
    /// </summary>
    [TestFixture]
    public class MathBasicTests
    {
        #region Abs

        [Test]
        public void Abs_Positive_ReturnsSelf()
        {
            FixedPoint result = Math.Abs(new FixedPoint(5));
            Assert.AreEqual(new FixedPoint(5).FixedValue, result.FixedValue);
        }

        [Test]
        public void Abs_Negative_ReturnsPositive()
        {
            FixedPoint result = Math.Abs(new FixedPoint(-5));
            Assert.AreEqual(new FixedPoint(5).FixedValue, result.FixedValue);
        }

        [Test]
        public void Abs_Zero_ReturnsZero()
        {
            Assert.AreEqual(FixedPoint.Zero, Math.Abs(FixedPoint.Zero));
        }

        #endregion

        #region Min / Max

        [Test]
        public void Max_ReturnsLarger()
        {
            FixedPoint a = new FixedPoint(3);
            FixedPoint b = new FixedPoint(7);
            Assert.AreEqual(b, Math.Max(a, b));
            Assert.AreEqual(b, Math.Max(b, a));
        }

        [Test]
        public void Min_ReturnsSmaller()
        {
            FixedPoint a = new FixedPoint(3);
            FixedPoint b = new FixedPoint(7);
            Assert.AreEqual(a, Math.Min(a, b));
            Assert.AreEqual(a, Math.Min(b, a));
        }

        [Test]
        public void MinMax_EqualValues_ReturnsSame()
        {
            FixedPoint v = new FixedPoint(5);
            Assert.AreEqual(v, Math.Min(v, v));
            Assert.AreEqual(v, Math.Max(v, v));
        }

        #endregion

        #region Sign

        [Test]
        public void Sign_PositiveAndZero_ReturnsOne()
        {
            Assert.AreEqual(FixedPoint.One, Math.Sign(new FixedPoint(5)));
            Assert.AreEqual(FixedPoint.One, Math.Sign(FixedPoint.Zero));
        }

        [Test]
        public void Sign_Negative_ReturnsNegativeOne()
        {
            Assert.AreEqual(FixedPoint.NegativeOne, Math.Sign(new FixedPoint(-3)));
        }

        #endregion

        #region Clamp

        [Test]
        public void Clamp_InRange_ReturnsSelf()
        {
            FixedPoint result = Math.Clamp(new FixedPoint(5), new FixedPoint(0), new FixedPoint(10));
            Assert.AreEqual(new FixedPoint(5).FixedValue, result.FixedValue);
        }

        [Test]
        public void Clamp_BelowMin_ReturnsMin()
        {
            FixedPoint result = Math.Clamp(new FixedPoint(-5), new FixedPoint(0), new FixedPoint(10));
            Assert.AreEqual(new FixedPoint(0).FixedValue, result.FixedValue);
        }

        [Test]
        public void Clamp_AboveMax_ReturnsMax()
        {
            FixedPoint result = Math.Clamp(new FixedPoint(15), new FixedPoint(0), new FixedPoint(10));
            Assert.AreEqual(new FixedPoint(10).FixedValue, result.FixedValue);
        }

        [Test]
        public void Clamp01_ClampsToUnitRange()
        {
            Assert.AreEqual(FixedPoint.Zero, Math.Clamp01(new FixedPoint(-1)));
            Assert.AreEqual(FixedPoint.One, Math.Clamp01(new FixedPoint(2)));
            TestHelper.AssertApprox(Math.Clamp01(new FixedPoint(0.5)), 0.5, 0.001);
        }

        [Test]
        public void Clamp_Int_ClampsCorrectly()
        {
            Assert.AreEqual(5, Math.Clamp(5, 0, 10));
            Assert.AreEqual(0, Math.Clamp(-3, 0, 10));
            Assert.AreEqual(10, Math.Clamp(15, 0, 10));
        }

        #endregion

        #region Pow (int exponent)

        [Test]
        public void Pow_Int_ZeroExponent_ReturnsOne()
        {
            Assert.AreEqual(FixedPoint.One, Math.Pow(new FixedPoint(5), 0));
        }

        [Test]
        public void Pow_Int_OneExponent_ReturnsSelf()
        {
            FixedPoint value = new FixedPoint(5);
            Assert.AreEqual(value, Math.Pow(value, 1));
        }

        [Test]
        public void Pow_Int_SquareAndCube()
        {
            TestHelper.AssertApprox(Math.Pow(new FixedPoint(3), 2), 9.0, 0.01);
            TestHelper.AssertApprox(Math.Pow(new FixedPoint(2), 3), 8.0, 0.01);
            TestHelper.AssertApprox(Math.Pow(new FixedPoint(2), 10), 1024.0, 1.0);
        }

        [Test]
        public void Pow_Int_NegativeExponent_ReturnsReciprocal()
        {
            TestHelper.AssertApprox(Math.Pow(new FixedPoint(2), -1), 0.5, 0.002);
            TestHelper.AssertApprox(Math.Pow(new FixedPoint(4), -1), 0.25, 0.002);
        }

        #endregion

        #region Pow (FixedPoint exponent)

        [Test]
        public void Pow_FP_BaseOne_ReturnsOne()
        {
            Assert.AreEqual(FixedPoint.One, Math.Pow(FixedPoint.One, new FixedPoint(5)));
        }

        [Test]
        public void Pow_FP_ZeroExponent_ReturnsOne()
        {
            Assert.AreEqual(FixedPoint.One, Math.Pow(new FixedPoint(5), FixedPoint.Zero));
        }

        [Test]
        public void Pow_FP_ZeroBase_PositiveExp_ReturnsZero()
        {
            Assert.AreEqual(FixedPoint.Zero, Math.Pow(FixedPoint.Zero, new FixedPoint(3)));
        }

        [Test]
        public void Pow_FP_NegativeBase_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                Math.Pow(new FixedPoint(-1), new FixedPoint(0.5));
            });
        }

        [Test]
        public void Pow_FP_SqrtTwo()
        {
            FixedPoint result = Math.Pow(new FixedPoint(2), new FixedPoint(0.5));
            TestHelper.AssertApprox(result, System.Math.Sqrt(2), 0.02);
        }

        #endregion

        #region Sqrt

        [Test]
        public void Sqrt_Zero_ReturnsZero()
        {
            Assert.AreEqual(FixedPoint.Zero, Math.Sqrt(FixedPoint.Zero));
        }

        [Test]
        public void Sqrt_One_ReturnsOne()
        {
            TestHelper.AssertApprox(Math.Sqrt(FixedPoint.One), 1.0, 0.001);
        }

        [TestCase(4, 2.0)]
        [TestCase(9, 3.0)]
        [TestCase(16, 4.0)]
        [TestCase(100, 10.0)]
        public void Sqrt_PerfectSquare_Exact(int input, double expected)
        {
            TestHelper.AssertApprox(Math.Sqrt(new FixedPoint(input)), expected, 0.002);
        }

        [TestCase(2, 1.414)]
        [TestCase(3, 1.732)]
        [TestCase(5, 2.236)]
        [TestCase(50, 7.071)]
        public void Sqrt_NonPerfect_Approximate(int input, double expected)
        {
            TestHelper.AssertApprox(Math.Sqrt(new FixedPoint(input)), expected, 0.005);
        }

        [Test]
        public void Sqrt_Negative_Throws()
        {
            Assert.Throws<ArithmeticException>(() =>
            {
                Math.Sqrt(new FixedPoint(-1));
            });
        }

        [Test]
        public void Sqrt_Epsilon_ReturnsPositive()
        {
            FixedPoint result = Math.Sqrt(FixedPoint.Epsilon);
            Assert.IsTrue(result > FixedPoint.Zero);
        }

        [Test]
        public void Sqrt_WithIterations_MatchesDefault()
        {
            FixedPoint value = new FixedPoint(50);
            FixedPoint result8 = Math.Sqrt(value);
            FixedPoint result12 = Math.Sqrt(value, 12);
            long diff = System.Math.Abs(result8.FixedValue - result12.FixedValue);
            Assert.IsTrue(diff <= 1, $"8 iterations vs 12 iterations diff = {diff}");
        }

        #endregion

        #region RSqrt / Rcp

        [Test]
        public void RSqrt_Four_ReturnsHalf()
        {
            TestHelper.AssertApprox(Math.RSqrt(new FixedPoint(4)), 0.5, 0.002);
        }

        [Test]
        public void Rcp_Two_ReturnsHalf()
        {
            TestHelper.AssertApprox(Math.Rcp(new FixedPoint(2)), 0.5, 0.001);
        }

        [Test]
        public void Rcp_Zero_ReturnsMaxValue()
        {
            Assert.AreEqual(FixedPoint.MaxValue, Math.Rcp(FixedPoint.Zero));
        }

        #endregion
    }
}
