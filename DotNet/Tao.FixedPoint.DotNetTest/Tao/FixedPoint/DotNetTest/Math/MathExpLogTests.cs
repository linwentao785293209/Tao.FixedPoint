namespace Tao.FixedPoint.DotNetTest
{
    /// <summary>
    /// 指数与对数函数测试：Log2, Ln, Log10, Exp, Exp2
    /// </summary>
    [TestClass]
    public sealed class MathExpLogTests
    {
        #region Log2

        /// <summary>
        /// Log2(1) = 0
        /// </summary>
        [TestMethod]
        public void Log2_One_ReturnsZero()
        {
            TestHelper.AssertApprox(Math.Log2(FixedPoint.One), 0.0, 0.002);
        }

        /// <summary>
        /// Log2 对 2 的整数幂返回精确结果
        /// </summary>
        [DataTestMethod]
        [DataRow(2, 1.0)]
        [DataRow(4, 2.0)]
        [DataRow(8, 3.0)]
        [DataRow(16, 4.0)]
        public void Log2_PowerOfTwo_Exact(int input, double expected)
        {
            TestHelper.AssertApprox(Math.Log2(new FixedPoint(input)), expected, 0.002);
        }

        /// <summary>
        /// Log2 对小于 1 的值返回负数
        /// </summary>
        [TestMethod]
        public void Log2_LessThanOne_ReturnsNegative()
        {
            FixedPoint half = new FixedPoint(0.5);
            TestHelper.AssertApprox(Math.Log2(half), -1.0, 0.01);
        }

        /// <summary>
        /// Log2 对非正值抛出异常
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Log2_Zero_Throws()
        {
            Math.Log2(FixedPoint.Zero);
        }

        /// <summary>
        /// Log2 对负数抛出异常
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Log2_Negative_Throws()
        {
            Math.Log2(new FixedPoint(-1));
        }

        #endregion

        #region Ln

        /// <summary>
        /// Ln(1) = 0
        /// </summary>
        [TestMethod]
        public void Ln_One_ReturnsZero()
        {
            TestHelper.AssertApprox(Math.Ln(FixedPoint.One), 0.0, 0.01);
        }

        /// <summary>
        /// Ln(e) ≈ 1 (e ≈ 2.718)
        /// </summary>
        [TestMethod]
        public void Ln_E_ReturnsApproxOne()
        {
            FixedPoint e = new FixedPoint(2.718);
            TestHelper.AssertApprox(Math.Ln(e), 1.0, 0.02);
        }

        /// <summary>
        /// Ln(2) ≈ 0.693
        /// </summary>
        [TestMethod]
        public void Ln_Two_ReturnsLn2()
        {
            TestHelper.AssertApprox(Math.Ln(new FixedPoint(2)), 0.693, 0.01);
        }

        #endregion

        #region Log10

        /// <summary>
        /// Log10(1) = 0
        /// </summary>
        [TestMethod]
        public void Log10_One_ReturnsZero()
        {
            TestHelper.AssertApprox(Math.Log10(FixedPoint.One), 0.0, 0.01);
        }

        /// <summary>
        /// Log10(10) ≈ 1
        /// </summary>
        [TestMethod]
        public void Log10_Ten_ReturnsOne()
        {
            TestHelper.AssertApprox(Math.Log10(new FixedPoint(10)), 1.0, 0.02);
        }

        /// <summary>
        /// Log10(100) ≈ 2
        /// </summary>
        [TestMethod]
        public void Log10_Hundred_ReturnsTwo()
        {
            TestHelper.AssertApprox(Math.Log10(new FixedPoint(100)), 2.0, 0.02);
        }

        #endregion

        #region Exp

        /// <summary>
        /// Exp(0) = 1
        /// </summary>
        [TestMethod]
        public void Exp_Zero_ReturnsOne()
        {
            Assert.AreEqual(FixedPoint.One, Math.Exp(FixedPoint.Zero));
        }

        /// <summary>
        /// Exp(1) ≈ e ≈ 2.718
        /// </summary>
        [TestMethod]
        public void Exp_One_ReturnsE()
        {
            TestHelper.AssertApprox(Math.Exp(FixedPoint.One), System.Math.E, 0.02);
        }

        /// <summary>
        /// Exp 负值返回 (0, 1) 区间的值
        /// </summary>
        [TestMethod]
        public void Exp_Negative_ReturnsLessThanOne()
        {
            FixedPoint result = Math.Exp(FixedPoint.NegativeOne);
            Assert.IsTrue(result > FixedPoint.Zero);
            Assert.IsTrue(result < FixedPoint.One);
            TestHelper.AssertApprox(result, 1.0 / System.Math.E, 0.02);
        }

        /// <summary>
        /// Exp 极大值饱和到 MaxValue
        /// </summary>
        [TestMethod]
        public void Exp_LargePositive_Saturates()
        {
            FixedPoint result = Math.Exp(new FixedPoint(100));
            Assert.AreEqual(FixedPoint.MaxValue, result);
        }

        /// <summary>
        /// Exp 极小负值趋近零
        /// </summary>
        [TestMethod]
        public void Exp_LargeNegative_ApproachesZero()
        {
            FixedPoint result = Math.Exp(new FixedPoint(-100));
            Assert.AreEqual(FixedPoint.Zero, result);
        }

        /// <summary>
        /// Exp 和 Ln 互为反函数: Exp(Ln(x)) ≈ x
        /// </summary>
        [TestMethod]
        public void Exp_Ln_RoundTrip()
        {
            FixedPoint original = new FixedPoint(3);
            FixedPoint roundTrip = Math.Exp(Math.Ln(original));
            TestHelper.AssertApprox(roundTrip, 3.0, 0.05);
        }

        #endregion

        #region Exp2

        /// <summary>
        /// Exp2(0) = 1
        /// </summary>
        [TestMethod]
        public void Exp2_Zero_ReturnsOne()
        {
            TestHelper.AssertApprox(Math.Exp2(FixedPoint.Zero), 1.0, 0.002);
        }

        /// <summary>
        /// Exp2(1) = 2
        /// </summary>
        [TestMethod]
        public void Exp2_One_ReturnsTwo()
        {
            TestHelper.AssertApprox(Math.Exp2(FixedPoint.One), 2.0, 0.02);
        }

        /// <summary>
        /// Exp2(3) ≈ 8
        /// </summary>
        [TestMethod]
        public void Exp2_Three_ReturnsEight()
        {
            TestHelper.AssertApprox(Math.Exp2(new FixedPoint(3)), 8.0, 0.1);
        }

        #endregion
    }
}