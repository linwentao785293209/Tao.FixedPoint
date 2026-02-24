namespace Tao.FixedPoint.DotNetTest
{
    /// <summary>
    /// 基础数学函数测试：Abs, Min, Max, Sign, Clamp, Pow, Sqrt, RSqrt, Rcp
    /// </summary>
    [TestClass]
    public sealed class MathBasicTests
    {
        #region Abs

        /// <summary>
        /// Abs 正数返回自身
        /// </summary>
        [TestMethod]
        public void Abs_Positive_ReturnsSelf()
        {
            FixedPoint result = Math.Abs(new FixedPoint(5));
            Assert.AreEqual(new FixedPoint(5).FixedValue, result.FixedValue);
        }

        /// <summary>
        /// Abs 负数返回正值
        /// </summary>
        [TestMethod]
        public void Abs_Negative_ReturnsPositive()
        {
            FixedPoint result = Math.Abs(new FixedPoint(-5));
            Assert.AreEqual(new FixedPoint(5).FixedValue, result.FixedValue);
        }

        /// <summary>
        /// Abs 零返回零
        /// </summary>
        [TestMethod]
        public void Abs_Zero_ReturnsZero()
        {
            Assert.AreEqual(FixedPoint.Zero, Math.Abs(FixedPoint.Zero));
        }

        #endregion

        #region Min / Max

        /// <summary>
        /// Max 返回较大值
        /// </summary>
        [TestMethod]
        public void Max_ReturnsLarger()
        {
            FixedPoint a = new FixedPoint(3);
            FixedPoint b = new FixedPoint(7);
            Assert.AreEqual(b, Math.Max(a, b));
            Assert.AreEqual(b, Math.Max(b, a));
        }

        /// <summary>
        /// Min 返回较小值
        /// </summary>
        [TestMethod]
        public void Min_ReturnsSmaller()
        {
            FixedPoint a = new FixedPoint(3);
            FixedPoint b = new FixedPoint(7);
            Assert.AreEqual(a, Math.Min(a, b));
            Assert.AreEqual(a, Math.Min(b, a));
        }

        /// <summary>
        /// Min/Max 相等时返回相等值
        /// </summary>
        [TestMethod]
        public void MinMax_EqualValues_ReturnsSame()
        {
            FixedPoint v = new FixedPoint(5);
            Assert.AreEqual(v, Math.Min(v, v));
            Assert.AreEqual(v, Math.Max(v, v));
        }

        #endregion

        #region Sign

        /// <summary>
        /// Sign 正数和零返回 1 (与 Unity Mathf.Sign 一致)
        /// </summary>
        [TestMethod]
        public void Sign_PositiveAndZero_ReturnsOne()
        {
            Assert.AreEqual(FixedPoint.One, Math.Sign(new FixedPoint(5)));
            Assert.AreEqual(FixedPoint.One, Math.Sign(FixedPoint.Zero));
        }

        /// <summary>
        /// Sign 负数返回 -1
        /// </summary>
        [TestMethod]
        public void Sign_Negative_ReturnsNegativeOne()
        {
            Assert.AreEqual(FixedPoint.NegativeOne, Math.Sign(new FixedPoint(-3)));
        }

        #endregion

        #region Clamp

        /// <summary>
        /// Clamp 值在范围内时不变
        /// </summary>
        [TestMethod]
        public void Clamp_InRange_ReturnsSelf()
        {
            FixedPoint result = Math.Clamp(new FixedPoint(5), new FixedPoint(0), new FixedPoint(10));
            Assert.AreEqual(new FixedPoint(5).FixedValue, result.FixedValue);
        }

        /// <summary>
        /// Clamp 值低于最小值时返回最小值
        /// </summary>
        [TestMethod]
        public void Clamp_BelowMin_ReturnsMin()
        {
            FixedPoint result = Math.Clamp(new FixedPoint(-5), new FixedPoint(0), new FixedPoint(10));
            Assert.AreEqual(new FixedPoint(0).FixedValue, result.FixedValue);
        }

        /// <summary>
        /// Clamp 值高于最大值时返回最大值
        /// </summary>
        [TestMethod]
        public void Clamp_AboveMax_ReturnsMax()
        {
            FixedPoint result = Math.Clamp(new FixedPoint(15), new FixedPoint(0), new FixedPoint(10));
            Assert.AreEqual(new FixedPoint(10).FixedValue, result.FixedValue);
        }

        /// <summary>
        /// Clamp01 将值限制在 [0, 1]
        /// </summary>
        [TestMethod]
        public void Clamp01_ClampsToUnitRange()
        {
            Assert.AreEqual(FixedPoint.Zero, Math.Clamp01(new FixedPoint(-1)));
            Assert.AreEqual(FixedPoint.One, Math.Clamp01(new FixedPoint(2)));
            TestHelper.AssertApprox(Math.Clamp01(new FixedPoint(0.5)), 0.5, 0.001);
        }

        /// <summary>
        /// int 版 Clamp 行为正确
        /// </summary>
        [TestMethod]
        public void Clamp_Int_ClampsCorrectly()
        {
            Assert.AreEqual(5, Math.Clamp(5, 0, 10));
            Assert.AreEqual(0, Math.Clamp(-3, 0, 10));
            Assert.AreEqual(10, Math.Clamp(15, 0, 10));
        }

        #endregion

        #region Pow (int exponent)

        /// <summary>
        /// 任何数的 0 次幂为 1
        /// </summary>
        [TestMethod]
        public void Pow_Int_ZeroExponent_ReturnsOne()
        {
            Assert.AreEqual(FixedPoint.One, Math.Pow(new FixedPoint(5), 0));
        }

        /// <summary>
        /// 任何数的 1 次幂为自身
        /// </summary>
        [TestMethod]
        public void Pow_Int_OneExponent_ReturnsSelf()
        {
            FixedPoint value = new FixedPoint(5);
            Assert.AreEqual(value, Math.Pow(value, 1));
        }

        /// <summary>
        /// 整数幂计算正确
        /// </summary>
        [TestMethod]
        public void Pow_Int_SquareAndCube()
        {
            TestHelper.AssertApprox(Math.Pow(new FixedPoint(3), 2), 9.0, 0.01);
            TestHelper.AssertApprox(Math.Pow(new FixedPoint(2), 3), 8.0, 0.01);
            TestHelper.AssertApprox(Math.Pow(new FixedPoint(2), 10), 1024.0, 1.0);
        }

        /// <summary>
        /// 负指数: x^(-n) = 1 / x^n
        /// </summary>
        [TestMethod]
        public void Pow_Int_NegativeExponent_ReturnsReciprocal()
        {
            TestHelper.AssertApprox(Math.Pow(new FixedPoint(2), -1), 0.5, 0.002);
            TestHelper.AssertApprox(Math.Pow(new FixedPoint(4), -1), 0.25, 0.002);
        }

        #endregion

        #region Pow (FixedPoint exponent)

        /// <summary>
        /// 定点指数幂: 基数为 1 时返回 1
        /// </summary>
        [TestMethod]
        public void Pow_FP_BaseOne_ReturnsOne()
        {
            Assert.AreEqual(FixedPoint.One, Math.Pow(FixedPoint.One, new FixedPoint(5)));
        }

        /// <summary>
        /// 定点指数幂: 指数为 0 时返回 1
        /// </summary>
        [TestMethod]
        public void Pow_FP_ZeroExponent_ReturnsOne()
        {
            Assert.AreEqual(FixedPoint.One, Math.Pow(new FixedPoint(5), FixedPoint.Zero));
        }

        /// <summary>
        /// 定点指数幂: 底数为零且指数为正时返回零
        /// </summary>
        [TestMethod]
        public void Pow_FP_ZeroBase_PositiveExp_ReturnsZero()
        {
            Assert.AreEqual(FixedPoint.Zero, Math.Pow(FixedPoint.Zero, new FixedPoint(3)));
        }

        /// <summary>
        /// 定点指数幂: 负底数抛出异常
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Pow_FP_NegativeBase_Throws()
        {
            Math.Pow(new FixedPoint(-1), new FixedPoint(0.5));
        }

        /// <summary>
        /// 定点指数幂: 2^0.5 ≈ √2 ≈ 1.414
        /// </summary>
        [TestMethod]
        public void Pow_FP_SqrtTwo()
        {
            FixedPoint result = Math.Pow(new FixedPoint(2), new FixedPoint(0.5));
            TestHelper.AssertApprox(result, System.Math.Sqrt(2), 0.02);
        }

        #endregion

        #region Sqrt

        /// <summary>
        /// Sqrt(0) = 0
        /// </summary>
        [TestMethod]
        public void Sqrt_Zero_ReturnsZero()
        {
            Assert.AreEqual(FixedPoint.Zero, Math.Sqrt(FixedPoint.Zero));
        }

        /// <summary>
        /// Sqrt(1) = 1
        /// </summary>
        [TestMethod]
        public void Sqrt_One_ReturnsOne()
        {
            TestHelper.AssertApprox(Math.Sqrt(FixedPoint.One), 1.0, 0.001);
        }

        /// <summary>
        /// 完全平方数的 Sqrt 精确
        /// </summary>
        [DataTestMethod]
        [DataRow(4, 2.0)]
        [DataRow(9, 3.0)]
        [DataRow(16, 4.0)]
        [DataRow(100, 10.0)]
        public void Sqrt_PerfectSquare_Exact(int input, double expected)
        {
            TestHelper.AssertApprox(Math.Sqrt(new FixedPoint(input)), expected, 0.002);
        }

        /// <summary>
        /// 非完全平方数的 Sqrt 近似正确
        /// </summary>
        [DataTestMethod]
        [DataRow(2, 1.414)]
        [DataRow(3, 1.732)]
        [DataRow(5, 2.236)]
        [DataRow(50, 7.071)]
        public void Sqrt_NonPerfect_Approximate(int input, double expected)
        {
            TestHelper.AssertApprox(Math.Sqrt(new FixedPoint(input)), expected, 0.005);
        }

        /// <summary>
        /// Sqrt 对负数抛出异常
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArithmeticException))]
        public void Sqrt_Negative_Throws()
        {
            Math.Sqrt(new FixedPoint(-1));
        }

        /// <summary>
        /// Sqrt(Epsilon) 不崩溃且返回正值
        /// </summary>
        [TestMethod]
        public void Sqrt_Epsilon_ReturnsPositive()
        {
            FixedPoint result = Math.Sqrt(FixedPoint.Epsilon);
            Assert.IsTrue(result > FixedPoint.Zero);
        }

        /// <summary>
        /// 带指定迭代次数的 Sqrt 重载行为正确
        /// </summary>
        [TestMethod]
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

        /// <summary>
        /// RSqrt(4) = 1/√4 = 0.5
        /// </summary>
        [TestMethod]
        public void RSqrt_Four_ReturnsHalf()
        {
            TestHelper.AssertApprox(Math.RSqrt(new FixedPoint(4)), 0.5, 0.002);
        }

        /// <summary>
        /// Rcp(2) = 1/2 = 0.5
        /// </summary>
        [TestMethod]
        public void Rcp_Two_ReturnsHalf()
        {
            TestHelper.AssertApprox(Math.Rcp(new FixedPoint(2)), 0.5, 0.001);
        }

        /// <summary>
        /// Rcp(0) 除以零返回 MaxValue (饱和处理)
        /// </summary>
        [TestMethod]
        public void Rcp_Zero_ReturnsMaxValue()
        {
            Assert.AreEqual(FixedPoint.MaxValue, Math.Rcp(FixedPoint.Zero));
        }

        #endregion
    }
}