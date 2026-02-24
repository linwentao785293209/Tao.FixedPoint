using NUnit.Framework;

namespace Tao.FixedPoint.UnityTest
{
    /// <summary>
    /// 取整函数测试：Round, Floor, Ceiling, Truncate, Fract, RoundToInt, FloorToInt, CeilToInt
    /// </summary>
    [TestFixture]
    public class MathRoundingTests
    {
        #region Floor

        [Test]
        public void Floor_PositiveFraction_RoundsDown()
        {
            TestHelper.AssertApprox(Math.Floor(new FixedPoint(1.5)), 1.0, 0.001);
            TestHelper.AssertApprox(Math.Floor(new FixedPoint(1.9)), 1.0, 0.001);
            TestHelper.AssertApprox(Math.Floor(new FixedPoint(1.1)), 1.0, 0.001);
        }

        [Test]
        public void Floor_NegativeFraction_RoundsToNegInf()
        {
            TestHelper.AssertApprox(Math.Floor(new FixedPoint(-1.5)), -2.0, 0.001);
            TestHelper.AssertApprox(Math.Floor(new FixedPoint(-1.1)), -2.0, 0.001);
        }

        [Test]
        public void Floor_ExactInteger_ReturnsSelf()
        {
            Assert.AreEqual(new FixedPoint(3).FixedValue, Math.Floor(new FixedPoint(3)).FixedValue);
            Assert.AreEqual(new FixedPoint(-2).FixedValue, Math.Floor(new FixedPoint(-2)).FixedValue);
        }

        [Test]
        public void Floor_Zero_ReturnsZero()
        {
            Assert.AreEqual(FixedPoint.Zero, Math.Floor(FixedPoint.Zero));
        }

        #endregion

        #region Ceiling

        [Test]
        public void Ceiling_PositiveFraction_RoundsUp()
        {
            TestHelper.AssertApprox(Math.Ceiling(new FixedPoint(1.1)), 2.0, 0.001);
            TestHelper.AssertApprox(Math.Ceiling(new FixedPoint(1.5)), 2.0, 0.001);
        }

        [Test]
        public void Ceiling_NegativeFraction_RoundsToPosInf()
        {
            TestHelper.AssertApprox(Math.Ceiling(new FixedPoint(-1.5)), -1.0, 0.001);
            TestHelper.AssertApprox(Math.Ceiling(new FixedPoint(-1.1)), -1.0, 0.001);
        }

        [Test]
        public void Ceiling_ExactInteger_ReturnsSelf()
        {
            Assert.AreEqual(new FixedPoint(3).FixedValue, Math.Ceiling(new FixedPoint(3)).FixedValue);
            Assert.AreEqual(new FixedPoint(-2).FixedValue, Math.Ceiling(new FixedPoint(-2)).FixedValue);
        }

        #endregion

        #region Round

        [Test]
        public void Round_PositiveHalf_RoundsUp()
        {
            TestHelper.AssertApprox(Math.Round(new FixedPoint(1.5)), 2.0, 0.001);
            TestHelper.AssertApprox(Math.Round(new FixedPoint(1.4)), 1.0, 0.001);
            TestHelper.AssertApprox(Math.Round(new FixedPoint(1.6)), 2.0, 0.001);
        }

        [Test]
        public void Round_NegativeHalf_RoundsAwayFromZero()
        {
            TestHelper.AssertApprox(Math.Round(new FixedPoint(-1.5)), -2.0, 0.001);
            TestHelper.AssertApprox(Math.Round(new FixedPoint(-1.4)), -1.0, 0.001);
        }

        [Test]
        public void Round_ExactInteger_ReturnsSelf()
        {
            Assert.AreEqual(new FixedPoint(3).FixedValue, Math.Round(new FixedPoint(3)).FixedValue);
        }

        #endregion

        #region Truncate

        [Test]
        public void Truncate_Positive_TruncatesTowardsZero()
        {
            TestHelper.AssertApprox(Math.Truncate(new FixedPoint(1.9)), 1.0, 0.001);
            TestHelper.AssertApprox(Math.Truncate(new FixedPoint(1.1)), 1.0, 0.001);
        }

        [Test]
        public void Truncate_Negative_TruncatesTowardsZero()
        {
            TestHelper.AssertApprox(Math.Truncate(new FixedPoint(-1.9)), -1.0, 0.001);
            TestHelper.AssertApprox(Math.Truncate(new FixedPoint(-1.1)), -1.0, 0.001);
        }

        #endregion

        #region Fract

        [Test]
        public void Fract_Positive_ReturnsFraction()
        {
            TestHelper.AssertApprox(Math.Fract(new FixedPoint(1.75)), 0.75, 0.002);
            TestHelper.AssertApprox(Math.Fract(new FixedPoint(3.25)), 0.25, 0.002);
        }

        [Test]
        public void Fract_Negative_ReturnsPositiveFraction()
        {
            FixedPoint result = Math.Fract(new FixedPoint(-1.25));
            TestHelper.AssertApprox(result, 0.75, 0.002);
        }

        [Test]
        public void Fract_ExactInteger_ReturnsZero()
        {
            Assert.AreEqual(FixedPoint.Zero, Math.Fract(new FixedPoint(3)));
        }

        #endregion

        #region ToInt 系列

        [Test]
        public void RoundToInt_ReturnsRoundedInt()
        {
            Assert.AreEqual(2, Math.RoundToInt(new FixedPoint(1.5)));
            Assert.AreEqual(1, Math.RoundToInt(new FixedPoint(1.4)));
            Assert.AreEqual(-2, Math.RoundToInt(new FixedPoint(-1.5)));
        }

        [Test]
        public void FloorToInt_ReturnsFlooredInt()
        {
            Assert.AreEqual(1, Math.FloorToInt(new FixedPoint(1.9)));
            Assert.AreEqual(-2, Math.FloorToInt(new FixedPoint(-1.1)));
        }

        [Test]
        public void CeilToInt_ReturnsCeiledInt()
        {
            Assert.AreEqual(2, Math.CeilToInt(new FixedPoint(1.1)));
            Assert.AreEqual(-1, Math.CeilToInt(new FixedPoint(-1.9)));
        }

        #endregion
    }
}
