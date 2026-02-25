namespace Tao.FixedPoint.DotNetTest
{
    /// <summary>
    /// 三角函数测试：Sin, Cos, Tan, SinCos, Acos, Asin, Atan, Atan2
    /// </summary>
    [TestClass]
    public sealed class MathTrigTests
    {
        /// <summary>
        /// 三角函数容差 (查表 + 线性插值, Q10 精度约 ±0.005)
        /// </summary>
        private const double TRIG_TOLERANCE = 0.005;

        #region Sin

        /// <summary>
        /// Sin(0) = 0
        /// </summary>
        [TestMethod]
        public void Sin_Zero_ReturnsZero()
        {
            TestHelper.AssertApprox(Math.Sin(FixedPoint.Zero), 0.0, TRIG_TOLERANCE);
        }

        /// <summary>
        /// Sin(π/2) = 1
        /// </summary>
        [TestMethod]
        public void Sin_PiOver2_ReturnsOne()
        {
            TestHelper.AssertApprox(Math.Sin(FixedPoint.PiOver2), 1.0, TRIG_TOLERANCE);
        }

        /// <summary>
        /// Sin(π) ≈ 0
        /// </summary>
        [TestMethod]
        public void Sin_Pi_ReturnsZero()
        {
            TestHelper.AssertApprox(Math.Sin(FixedPoint.Pi), 0.0, TRIG_TOLERANCE);
        }

        /// <summary>
        /// Sin(2π) ≈ 0 (周期性)
        /// </summary>
        [TestMethod]
        public void Sin_TwoPi_ReturnsZero()
        {
            TestHelper.AssertApprox(Math.Sin(FixedPoint.TwoPi), 0.0, TRIG_TOLERANCE);
        }

        /// <summary>
        /// Sin(-π/2) = -1
        /// </summary>
        [TestMethod]
        public void Sin_NegPiOver2_ReturnsNegOne()
        {
            TestHelper.AssertApprox(Math.Sin(-FixedPoint.PiOver2), -1.0, TRIG_TOLERANCE);
        }

        /// <summary>
        /// Sin 负角度: Sin(-x) = -Sin(x)
        /// </summary>
        [TestMethod]
        public void Sin_Negative_IsOdd()
        {
            FixedPoint angle = new FixedPoint(1);
            FixedPoint sinPos = Math.Sin(angle);
            FixedPoint sinNeg = Math.Sin(-angle);
            long diff = System.Math.Abs(sinPos.FixedValue + sinNeg.FixedValue);
            Assert.IsTrue(diff <= 2, $"Sin(-x) should ≈ -Sin(x), diff={diff}");
        }

        #endregion

        #region Cos

        /// <summary>
        /// Cos(0) = 1
        /// </summary>
        [TestMethod]
        public void Cos_Zero_ReturnsOne()
        {
            TestHelper.AssertApprox(Math.Cos(FixedPoint.Zero), 1.0, TRIG_TOLERANCE);
        }

        /// <summary>
        /// Cos(π/2) ≈ 0
        /// </summary>
        [TestMethod]
        public void Cos_PiOver2_ReturnsZero()
        {
            TestHelper.AssertApprox(Math.Cos(FixedPoint.PiOver2), 0.0, TRIG_TOLERANCE);
        }

        /// <summary>
        /// Cos(π) = -1
        /// </summary>
        [TestMethod]
        public void Cos_Pi_ReturnsNegOne()
        {
            TestHelper.AssertApprox(Math.Cos(FixedPoint.Pi), -1.0, TRIG_TOLERANCE);
        }

        /// <summary>
        /// Cos(2π) = 1
        /// </summary>
        [TestMethod]
        public void Cos_TwoPi_ReturnsOne()
        {
            TestHelper.AssertApprox(Math.Cos(FixedPoint.TwoPi), 1.0, TRIG_TOLERANCE);
        }

        /// <summary>
        /// sin²(x) + cos²(x) = 1 (勾股恒等式)
        /// </summary>
        [TestMethod]
        public void SinCos_PythagoreanIdentity()
        {
            FixedPoint angle = new FixedPoint(1);
            FixedPoint sin = Math.Sin(angle);
            FixedPoint cos = Math.Cos(angle);
            FixedPoint sum = sin * sin + cos * cos;
            TestHelper.AssertApprox(sum, 1.0, 0.01);
        }

        #endregion

        #region SinCos

        /// <summary>
        /// SinCos 与分别调用 Sin/Cos 结果一致
        /// </summary>
        [TestMethod]
        public void SinCos_MatchesSeparateCalls()
        {
            FixedPoint angle = new FixedPoint(1);
            Math.SinCos(angle, out FixedPoint sin, out FixedPoint cos);
            FixedPoint sinSeparate = Math.Sin(angle);
            FixedPoint cosSeparate = Math.Cos(angle);

            Assert.AreEqual(sinSeparate.FixedValue, sin.FixedValue);
            Assert.AreEqual(cosSeparate.FixedValue, cos.FixedValue);
        }

        /// <summary>
        /// SinCos 在零点: Sin=0, Cos=1
        /// </summary>
        [TestMethod]
        public void SinCos_Zero_Sin0Cos1()
        {
            Math.SinCos(FixedPoint.Zero, out FixedPoint sin, out FixedPoint cos);
            TestHelper.AssertApprox(sin, 0.0, TRIG_TOLERANCE);
            TestHelper.AssertApprox(cos, 1.0, TRIG_TOLERANCE);
        }

        #endregion

        #region Tan

        /// <summary>
        /// Tan(0) = 0
        /// </summary>
        [TestMethod]
        public void Tan_Zero_ReturnsZero()
        {
            TestHelper.AssertApprox(Math.Tan(FixedPoint.Zero), 0.0, TRIG_TOLERANCE);
        }

        /// <summary>
        /// Tan(π/4) ≈ 1
        /// </summary>
        [TestMethod]
        public void Tan_PiOver4_ReturnsOne()
        {
            FixedPoint piOver4 = FixedPoint.PiOver2 >> 1;
            TestHelper.AssertApprox(Math.Tan(piOver4), 1.0, 0.02);
        }

        /// <summary>
        /// Tan(π/2) Cos 为零或极小时返回饱和值
        /// </summary>
        [TestMethod]
        public void Tan_AtPiOver2_ReturnsLargeValue()
        {
            FixedPoint result = Math.Tan(FixedPoint.PiOver2);
            Assert.IsTrue(Math.Abs(result) > new FixedPoint(100));
        }

        #endregion

        #region Acos

        /// <summary>
        /// Acos(1) = 0
        /// </summary>
        [TestMethod]
        public void Acos_One_ReturnsZero()
        {
            TestHelper.AssertApprox(Math.Acos(FixedPoint.One), 0.0, 0.01);
        }

        /// <summary>
        /// Acos(0) = π/2
        /// </summary>
        [TestMethod]
        public void Acos_Zero_ReturnsPiOver2()
        {
            TestHelper.AssertApprox(Math.Acos(FixedPoint.Zero), System.Math.PI / 2, 0.01);
        }

        /// <summary>
        /// Acos(-1) = π
        /// </summary>
        [TestMethod]
        public void Acos_NegOne_ReturnsPi()
        {
            TestHelper.AssertApprox(Math.Acos(FixedPoint.NegativeOne), System.Math.PI, 0.01);
        }

        /// <summary>
        /// Acos(0.5) ≈ π/3 ≈ 1.047
        /// </summary>
        [TestMethod]
        public void Acos_Half_ReturnsPiOver3()
        {
            TestHelper.AssertApprox(Math.Acos(new FixedPoint(0.5)), System.Math.PI / 3, 0.02);
        }

        /// <summary>
        /// Acos 双参数重载：Acos(value, denominator) ≈ Acos(value / denominator)
        /// </summary>
        [TestMethod]
        public void Acos_WithDenominator_MatchesSingleArg()
        {
            FixedPoint result = Math.Acos(new FixedPoint(0.5), FixedPoint.MULTIPLE);
            FixedPoint expected = Math.Acos(new FixedPoint(0.5));
            long diff = System.Math.Abs(result.FixedValue - expected.FixedValue);
            Assert.IsTrue(diff <= 2, $"Acos two-arg vs single-arg diff={diff}");
        }

        #endregion

        #region Asin

        /// <summary>
        /// Asin(0) = 0
        /// </summary>
        [TestMethod]
        public void Asin_Zero_ReturnsZero()
        {
            TestHelper.AssertApprox(Math.Asin(FixedPoint.Zero), 0.0, 0.01);
        }

        /// <summary>
        /// Asin(1) = π/2
        /// </summary>
        [TestMethod]
        public void Asin_One_ReturnsPiOver2()
        {
            TestHelper.AssertApprox(Math.Asin(FixedPoint.One), System.Math.PI / 2, 0.01);
        }

        /// <summary>
        /// Asin(-1) = -π/2
        /// </summary>
        [TestMethod]
        public void Asin_NegOne_ReturnsNegPiOver2()
        {
            TestHelper.AssertApprox(Math.Asin(FixedPoint.NegativeOne), -System.Math.PI / 2, 0.01);
        }

        #endregion

        #region Atan / Atan2

        /// <summary>
        /// Atan(0) = 0
        /// </summary>
        [TestMethod]
        public void Atan_Zero_ReturnsZero()
        {
            TestHelper.AssertApprox(Math.Atan(FixedPoint.Zero), 0.0, 0.01);
        }

        /// <summary>
        /// Atan(1) ≈ π/4
        /// </summary>
        [TestMethod]
        public void Atan_One_ReturnsPiOver4()
        {
            TestHelper.AssertApprox(Math.Atan(FixedPoint.One), System.Math.PI / 4, 0.02);
        }

        /// <summary>
        /// Atan2(0, 0) = 0
        /// </summary>
        [TestMethod]
        public void Atan2_ZeroZero_ReturnsZero()
        {
            Assert.AreEqual(FixedPoint.Zero, Math.Atan2(FixedPoint.Zero, FixedPoint.Zero));
        }

        /// <summary>
        /// Atan2(1, 0) ≈ π/2 (正 Y 轴)
        /// </summary>
        [TestMethod]
        public void Atan2_PositiveY_ReturnsPiOver2()
        {
            TestHelper.AssertApprox(Math.Atan2(FixedPoint.One, FixedPoint.Zero), System.Math.PI / 2, 0.02);
        }

        /// <summary>
        /// Atan2(0, 1) ≈ 0 (正 X 轴)
        /// </summary>
        [TestMethod]
        public void Atan2_PositiveX_ReturnsZero()
        {
            TestHelper.AssertApprox(Math.Atan2(FixedPoint.Zero, FixedPoint.One), 0.0, 0.01);
        }

        /// <summary>
        /// Atan2(1, 1) ≈ π/4 (第一象限 45°)
        /// </summary>
        [TestMethod]
        public void Atan2_FirstQuadrant_45Degrees()
        {
            TestHelper.AssertApprox(Math.Atan2(FixedPoint.One, FixedPoint.One), System.Math.PI / 4, 0.02);
        }

        /// <summary>
        /// Atan2 四象限符号正确
        /// </summary>
        [TestMethod]
        public void Atan2_AllQuadrants_CorrectSign()
        {
            FixedPoint one = FixedPoint.One;
            FixedPoint negOne = FixedPoint.NegativeOne;

            FixedPoint q1 = Math.Atan2(one, one);
            FixedPoint q2 = Math.Atan2(one, negOne);
            FixedPoint q3 = Math.Atan2(negOne, negOne);
            FixedPoint q4 = Math.Atan2(negOne, one);

            Assert.IsTrue(q1 > FixedPoint.Zero);
            Assert.IsTrue(q2 > FixedPoint.Zero);
            Assert.IsTrue(q3 < FixedPoint.Zero);
            Assert.IsTrue(q4 < FixedPoint.Zero);
        }

        /// <summary>
        /// float 版 Atan2 与定点版结果近似
        /// </summary>
        [TestMethod]
        public void Atan2_Float_MatchesFixedPoint()
        {
            FixedPoint resultFP = Math.Atan2(new FixedPoint(3), new FixedPoint(4));
            FixedPoint resultFloat = Math.Atan2(3.0f, 4.0f);
            long diff = System.Math.Abs(resultFP.FixedValue - resultFloat.FixedValue);
            Assert.IsTrue(diff <= 5, $"float vs FixedPoint Atan2 diff={diff}");
        }

        #endregion

        #region 反三角函数一致性

        /// <summary>
        /// Acos(Cos(x)) ≈ x (正向验证，x ∈ [0, π])
        /// </summary>
        [TestMethod]
        public void Acos_Cos_RoundTrip()
        {
            FixedPoint angle = FixedPoint.One;
            FixedPoint roundTrip = Math.Acos(Math.Cos(angle));
            TestHelper.AssertApprox(roundTrip, 1.0, 0.02);
        }

        /// <summary>
        /// Sin(Asin(x)) ≈ x (正向验证，x ∈ [-1, 1])
        /// </summary>
        [TestMethod]
        public void Sin_Asin_RoundTrip()
        {
            FixedPoint half = new FixedPoint(0.5);
            FixedPoint roundTrip = Math.Sin(Math.Asin(half));
            TestHelper.AssertApprox(roundTrip, 0.5, 0.02);
        }

        #endregion

        #region 特殊角度精确值 (通过 DegreesToRadians 路径，确定性回归测试)

        /// <summary>
        /// Sin(0°) = 0, Cos(0°) = 1 → 精确值
        /// </summary>
        [TestMethod]
        public void SinCos_0Deg_Exact()
        {
            FixedPoint rad = Math.DegreesToRadians(new FixedPoint(0));
            Assert.AreEqual(0L, Math.Sin(rad).FixedValue, "Sin(0°)");
            Assert.AreEqual(1024L, Math.Cos(rad).FixedValue, "Cos(0°)");
        }

        /// <summary>
        /// Sin(30°) = 0.5 (FixedValue=512), Cos(30°) ≈ √3/2 (FixedValue=887)
        /// </summary>
        [TestMethod]
        public void SinCos_30Deg_Exact()
        {
            FixedPoint rad = Math.DegreesToRadians(new FixedPoint(30));
            Assert.AreEqual(512L, Math.Sin(rad).FixedValue, "Sin(30°)");
            Assert.AreEqual(887L, Math.Cos(rad).FixedValue, "Cos(30°)");
        }

        /// <summary>
        /// Sin(45°) ≈ Cos(45°) ≈ √2/2 (FixedValue=724)
        /// </summary>
        [TestMethod]
        public void SinCos_45Deg_Exact()
        {
            FixedPoint rad = Math.DegreesToRadians(new FixedPoint(45));
            Assert.AreEqual(724L, Math.Sin(rad).FixedValue, "Sin(45°)");
            Assert.AreEqual(724L, Math.Cos(rad).FixedValue, "Cos(45°)");
        }

        /// <summary>
        /// Sin(60°) ≈ √3/2 (FixedValue=887), Cos(60°) = 0.5 (FixedValue=512)
        /// </summary>
        [TestMethod]
        public void SinCos_60Deg_Exact()
        {
            FixedPoint rad = Math.DegreesToRadians(new FixedPoint(60));
            Assert.AreEqual(887L, Math.Sin(rad).FixedValue, "Sin(60°)");
            Assert.AreEqual(512L, Math.Cos(rad).FixedValue, "Cos(60°)");
        }

        /// <summary>
        /// Sin(90°) = 1 (FixedValue=1024), Cos(90°) = 0 → 精确值
        /// </summary>
        [TestMethod]
        public void SinCos_90Deg_Exact()
        {
            FixedPoint rad = Math.DegreesToRadians(new FixedPoint(90));
            Assert.AreEqual(1024L, Math.Sin(rad).FixedValue, "Sin(90°)");
            Assert.AreEqual(0L, Math.Cos(rad).FixedValue, "Cos(90°)");
        }

        /// <summary>
        /// Sin(120°) ≈ √3/2 (FixedValue=887), Cos(120°) = -0.5 (FixedValue=-512)
        /// </summary>
        [TestMethod]
        public void SinCos_120Deg_Exact()
        {
            FixedPoint rad = Math.DegreesToRadians(new FixedPoint(120));
            Assert.AreEqual(887L, Math.Sin(rad).FixedValue, "Sin(120°)");
            Assert.AreEqual(-512L, Math.Cos(rad).FixedValue, "Cos(120°)");
        }

        /// <summary>
        /// Sin(150°) = 0.5 (FixedValue=512), Cos(150°) ≈ -√3/2 (FixedValue=-887)
        /// </summary>
        [TestMethod]
        public void SinCos_150Deg_Exact()
        {
            FixedPoint rad = Math.DegreesToRadians(new FixedPoint(150));
            Assert.AreEqual(512L, Math.Sin(rad).FixedValue, "Sin(150°)");
            Assert.AreEqual(-887L, Math.Cos(rad).FixedValue, "Cos(150°)");
        }

        /// <summary>
        /// Sin(180°) = 0, Cos(180°) = -1 → 精确值
        /// </summary>
        [TestMethod]
        public void SinCos_180Deg_Exact()
        {
            FixedPoint rad = Math.DegreesToRadians(new FixedPoint(180));
            Assert.AreEqual(0L, Math.Sin(rad).FixedValue, "Sin(180°)");
            Assert.AreEqual(-1024L, Math.Cos(rad).FixedValue, "Cos(180°)");
        }

        /// <summary>
        /// Sin(270°) = -1 (FixedValue=-1024), Cos(270°) = 0 → 精确值
        /// </summary>
        [TestMethod]
        public void SinCos_270Deg_Exact()
        {
            FixedPoint rad = Math.DegreesToRadians(new FixedPoint(270));
            Assert.AreEqual(-1024L, Math.Sin(rad).FixedValue, "Sin(270°)");
            Assert.AreEqual(0L, Math.Cos(rad).FixedValue, "Cos(270°)");
        }

        /// <summary>
        /// Sin(360°) = 0, Cos(360°) = 1 → 精确值 (周期性)
        /// </summary>
        [TestMethod]
        public void SinCos_360Deg_Exact()
        {
            FixedPoint rad = Math.DegreesToRadians(new FixedPoint(360));
            Assert.AreEqual(0L, Math.Sin(rad).FixedValue, "Sin(360°)");
            Assert.AreEqual(1024L, Math.Cos(rad).FixedValue, "Cos(360°)");
        }

        /// <summary>
        /// Tan(45°) = 1 → FixedValue=1024 精确值
        /// </summary>
        [TestMethod]
        public void Tan_45Deg_ReturnsOne()
        {
            FixedPoint rad = Math.DegreesToRadians(new FixedPoint(45));
            Assert.AreEqual(1024L, Math.Tan(rad).FixedValue, "Tan(45°)");
        }

        /// <summary>
        /// Tan(90°): Cos(90°) = 0，除法饱和到 MaxValue
        /// </summary>
        [TestMethod]
        public void Tan_90Deg_Saturates()
        {
            FixedPoint rad = Math.DegreesToRadians(new FixedPoint(90));
            Assert.AreEqual(FixedPoint.MaxValue.FixedValue, Math.Tan(rad).FixedValue, "Tan(90°)");
        }

        #endregion
    }
}