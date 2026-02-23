namespace Tao.FixedPoint.DotNetTest
{
    /// <summary>
    /// 取整函数测试：Round, Floor, Ceiling, Truncate, Fract, RoundToInt, FloorToInt, CeilToInt
    /// </summary>
    [TestClass]
    public sealed class MathRoundingTests
    {
        #region Floor

        /// <summary>
        /// Floor 正数小数向下取整
        /// </summary>
        [TestMethod]
        public void Floor_PositiveFraction_RoundsDown()
        {
            TestHelper.AssertApprox(Math.Floor(new FixedPoint(1.5)), 1.0, 0.001);
            TestHelper.AssertApprox(Math.Floor(new FixedPoint(1.9)), 1.0, 0.001);
            TestHelper.AssertApprox(Math.Floor(new FixedPoint(1.1)), 1.0, 0.001);
        }

        /// <summary>
        /// Floor 负数向负无穷取整
        /// </summary>
        [TestMethod]
        public void Floor_NegativeFraction_RoundsToNegInf()
        {
            TestHelper.AssertApprox(Math.Floor(new FixedPoint(-1.5)), -2.0, 0.001);
            TestHelper.AssertApprox(Math.Floor(new FixedPoint(-1.1)), -2.0, 0.001);
        }

        /// <summary>
        /// Floor 精确整数返回自身
        /// </summary>
        [TestMethod]
        public void Floor_ExactInteger_ReturnsSelf()
        {
            Assert.AreEqual(new FixedPoint(3).FixedValue, Math.Floor(new FixedPoint(3)).FixedValue);
            Assert.AreEqual(new FixedPoint(-2).FixedValue, Math.Floor(new FixedPoint(-2)).FixedValue);
        }

        /// <summary>
        /// Floor 零返回零
        /// </summary>
        [TestMethod]
        public void Floor_Zero_ReturnsZero()
        {
            Assert.AreEqual(FixedPoint.Zero, Math.Floor(FixedPoint.Zero));
        }

        #endregion

        #region Ceiling

        /// <summary>
        /// Ceiling 正数小数向上取整
        /// </summary>
        [TestMethod]
        public void Ceiling_PositiveFraction_RoundsUp()
        {
            TestHelper.AssertApprox(Math.Ceiling(new FixedPoint(1.1)), 2.0, 0.001);
            TestHelper.AssertApprox(Math.Ceiling(new FixedPoint(1.5)), 2.0, 0.001);
        }

        /// <summary>
        /// Ceiling 负数向正无穷取整
        /// </summary>
        [TestMethod]
        public void Ceiling_NegativeFraction_RoundsToPosInf()
        {
            TestHelper.AssertApprox(Math.Ceiling(new FixedPoint(-1.5)), -1.0, 0.001);
            TestHelper.AssertApprox(Math.Ceiling(new FixedPoint(-1.1)), -1.0, 0.001);
        }

        /// <summary>
        /// Ceiling 精确整数返回自身
        /// </summary>
        [TestMethod]
        public void Ceiling_ExactInteger_ReturnsSelf()
        {
            Assert.AreEqual(new FixedPoint(3).FixedValue, Math.Ceiling(new FixedPoint(3)).FixedValue);
            Assert.AreEqual(new FixedPoint(-2).FixedValue, Math.Ceiling(new FixedPoint(-2)).FixedValue);
        }

        #endregion

        #region Round

        /// <summary>
        /// Round 四舍五入 (正数)
        /// </summary>
        [TestMethod]
        public void Round_PositiveHalf_RoundsUp()
        {
            TestHelper.AssertApprox(Math.Round(new FixedPoint(1.5)), 2.0, 0.001);
            TestHelper.AssertApprox(Math.Round(new FixedPoint(1.4)), 1.0, 0.001);
            TestHelper.AssertApprox(Math.Round(new FixedPoint(1.6)), 2.0, 0.001);
        }

        /// <summary>
        /// Round 四舍五入 (负数, 远离零)
        /// </summary>
        [TestMethod]
        public void Round_NegativeHalf_RoundsAwayFromZero()
        {
            TestHelper.AssertApprox(Math.Round(new FixedPoint(-1.5)), -2.0, 0.001);
            TestHelper.AssertApprox(Math.Round(new FixedPoint(-1.4)), -1.0, 0.001);
        }

        /// <summary>
        /// Round 精确整数返回自身
        /// </summary>
        [TestMethod]
        public void Round_ExactInteger_ReturnsSelf()
        {
            Assert.AreEqual(new FixedPoint(3).FixedValue, Math.Round(new FixedPoint(3)).FixedValue);
        }

        #endregion

        #region Truncate

        /// <summary>
        /// Truncate 正数向零截断
        /// </summary>
        [TestMethod]
        public void Truncate_Positive_TruncatesTowardsZero()
        {
            TestHelper.AssertApprox(Math.Truncate(new FixedPoint(1.9)), 1.0, 0.001);
            TestHelper.AssertApprox(Math.Truncate(new FixedPoint(1.1)), 1.0, 0.001);
        }

        /// <summary>
        /// Truncate 负数向零截断 (与 Floor 不同)
        /// </summary>
        [TestMethod]
        public void Truncate_Negative_TruncatesTowardsZero()
        {
            TestHelper.AssertApprox(Math.Truncate(new FixedPoint(-1.9)), -1.0, 0.001);
            TestHelper.AssertApprox(Math.Truncate(new FixedPoint(-1.1)), -1.0, 0.001);
        }

        #endregion

        #region Fract

        /// <summary>
        /// Fract 正数返回 [0, 1) 内的小数部分
        /// </summary>
        [TestMethod]
        public void Fract_Positive_ReturnsFraction()
        {
            TestHelper.AssertApprox(Math.Fract(new FixedPoint(1.75)), 0.75, 0.002);
            TestHelper.AssertApprox(Math.Fract(new FixedPoint(3.25)), 0.25, 0.002);
        }

        /// <summary>
        /// Fract 负数也返回 [0, 1) 内的值 (= value - Floor(value))
        /// </summary>
        [TestMethod]
        public void Fract_Negative_ReturnsPositiveFraction()
        {
            FixedPoint result = Math.Fract(new FixedPoint(-1.25));
            TestHelper.AssertApprox(result, 0.75, 0.002);
        }

        /// <summary>
        /// Fract 精确整数返回零
        /// </summary>
        [TestMethod]
        public void Fract_ExactInteger_ReturnsZero()
        {
            Assert.AreEqual(FixedPoint.Zero, Math.Fract(new FixedPoint(3)));
        }

        #endregion

        #region ToInt 系列

        /// <summary>
        /// RoundToInt 返回四舍五入的 int
        /// </summary>
        [TestMethod]
        public void RoundToInt_ReturnsRoundedInt()
        {
            Assert.AreEqual(2, Math.RoundToInt(new FixedPoint(1.5)));
            Assert.AreEqual(1, Math.RoundToInt(new FixedPoint(1.4)));
            Assert.AreEqual(-2, Math.RoundToInt(new FixedPoint(-1.5)));
        }

        /// <summary>
        /// FloorToInt 返回向下取整的 int
        /// </summary>
        [TestMethod]
        public void FloorToInt_ReturnsFlooredInt()
        {
            Assert.AreEqual(1, Math.FloorToInt(new FixedPoint(1.9)));
            Assert.AreEqual(-2, Math.FloorToInt(new FixedPoint(-1.1)));
        }

        /// <summary>
        /// CeilToInt 返回向上取整的 int
        /// </summary>
        [TestMethod]
        public void CeilToInt_ReturnsCeiledInt()
        {
            Assert.AreEqual(2, Math.CeilToInt(new FixedPoint(1.1)));
            Assert.AreEqual(-1, Math.CeilToInt(new FixedPoint(-1.9)));
        }

        #endregion
    }
}