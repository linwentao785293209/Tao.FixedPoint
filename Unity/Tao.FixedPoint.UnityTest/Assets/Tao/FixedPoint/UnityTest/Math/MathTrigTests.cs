using NUnit.Framework;

namespace Tao.FixedPoint.UnityTest
{
    /// <summary>
    /// 三角函数测试：Sin, Cos, Tan, SinCos, Acos, Asin, Atan, Atan2
    /// </summary>
    [TestFixture]
    public class MathTrigTests
    {
        private const double TRIG_TOLERANCE = 0.005;

        #region Sin

        [Test]
        public void Sin_Zero_ReturnsZero()
        {
            TestHelper.AssertApprox(Math.Sin(FixedPoint.Zero), 0.0, TRIG_TOLERANCE);
        }

        [Test]
        public void Sin_PiOver2_ReturnsOne()
        {
            TestHelper.AssertApprox(Math.Sin(FixedPoint.PiOver2), 1.0, TRIG_TOLERANCE);
        }

        [Test]
        public void Sin_Pi_ReturnsZero()
        {
            TestHelper.AssertApprox(Math.Sin(FixedPoint.Pi), 0.0, TRIG_TOLERANCE);
        }

        [Test]
        public void Sin_TwoPi_ReturnsZero()
        {
            TestHelper.AssertApprox(Math.Sin(FixedPoint.TwoPi), 0.0, TRIG_TOLERANCE);
        }

        [Test]
        public void Sin_NegPiOver2_ReturnsNegOne()
        {
            TestHelper.AssertApprox(Math.Sin(-FixedPoint.PiOver2), -1.0, TRIG_TOLERANCE);
        }

        [Test]
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

        [Test]
        public void Cos_Zero_ReturnsOne()
        {
            TestHelper.AssertApprox(Math.Cos(FixedPoint.Zero), 1.0, TRIG_TOLERANCE);
        }

        [Test]
        public void Cos_PiOver2_ReturnsZero()
        {
            TestHelper.AssertApprox(Math.Cos(FixedPoint.PiOver2), 0.0, TRIG_TOLERANCE);
        }

        [Test]
        public void Cos_Pi_ReturnsNegOne()
        {
            TestHelper.AssertApprox(Math.Cos(FixedPoint.Pi), -1.0, TRIG_TOLERANCE);
        }

        [Test]
        public void Cos_TwoPi_ReturnsOne()
        {
            TestHelper.AssertApprox(Math.Cos(FixedPoint.TwoPi), 1.0, TRIG_TOLERANCE);
        }

        [Test]
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

        [Test]
        public void SinCos_MatchesSeparateCalls()
        {
            FixedPoint angle = new FixedPoint(1);
            Math.SinCos(angle, out FixedPoint sin, out FixedPoint cos);
            FixedPoint sinSeparate = Math.Sin(angle);
            FixedPoint cosSeparate = Math.Cos(angle);

            Assert.AreEqual(sinSeparate.FixedValue, sin.FixedValue);
            Assert.AreEqual(cosSeparate.FixedValue, cos.FixedValue);
        }

        [Test]
        public void SinCos_Zero_Sin0Cos1()
        {
            Math.SinCos(FixedPoint.Zero, out FixedPoint sin, out FixedPoint cos);
            TestHelper.AssertApprox(sin, 0.0, TRIG_TOLERANCE);
            TestHelper.AssertApprox(cos, 1.0, TRIG_TOLERANCE);
        }

        #endregion

        #region Tan

        [Test]
        public void Tan_Zero_ReturnsZero()
        {
            TestHelper.AssertApprox(Math.Tan(FixedPoint.Zero), 0.0, TRIG_TOLERANCE);
        }

        [Test]
        public void Tan_PiOver4_ReturnsOne()
        {
            FixedPoint piOver4 = FixedPoint.PiOver2 >> 1;
            TestHelper.AssertApprox(Math.Tan(piOver4), 1.0, 0.02);
        }

        [Test]
        public void Tan_AtPiOver2_ReturnsLargeValue()
        {
            FixedPoint result = Math.Tan(FixedPoint.PiOver2);
            Assert.IsTrue(Math.Abs(result) > new FixedPoint(100));
        }

        #endregion

        #region Acos

        [Test]
        public void Acos_One_ReturnsZero()
        {
            TestHelper.AssertApprox(Math.Acos(FixedPoint.One), 0.0, 0.01);
        }

        [Test]
        public void Acos_Zero_ReturnsPiOver2()
        {
            TestHelper.AssertApprox(Math.Acos(FixedPoint.Zero), System.Math.PI / 2, 0.01);
        }

        [Test]
        public void Acos_NegOne_ReturnsPi()
        {
            TestHelper.AssertApprox(Math.Acos(FixedPoint.NegativeOne), System.Math.PI, 0.01);
        }

        [Test]
        public void Acos_Half_ReturnsPiOver3()
        {
            TestHelper.AssertApprox(Math.Acos(new FixedPoint(0.5)), System.Math.PI / 3, 0.02);
        }

        [Test]
        public void Acos_WithDenominator_MatchesSingleArg()
        {
            FixedPoint result = Math.Acos(new FixedPoint(0.5), FixedPoint.MULTIPLE);
            FixedPoint expected = Math.Acos(new FixedPoint(0.5));
            long diff = System.Math.Abs(result.FixedValue - expected.FixedValue);
            Assert.IsTrue(diff <= 2, $"Acos two-arg vs single-arg diff={diff}");
        }

        #endregion

        #region Asin

        [Test]
        public void Asin_Zero_ReturnsZero()
        {
            TestHelper.AssertApprox(Math.Asin(FixedPoint.Zero), 0.0, 0.01);
        }

        [Test]
        public void Asin_One_ReturnsPiOver2()
        {
            TestHelper.AssertApprox(Math.Asin(FixedPoint.One), System.Math.PI / 2, 0.01);
        }

        [Test]
        public void Asin_NegOne_ReturnsNegPiOver2()
        {
            TestHelper.AssertApprox(Math.Asin(FixedPoint.NegativeOne), -System.Math.PI / 2, 0.01);
        }

        #endregion

        #region Atan / Atan2

        [Test]
        public void Atan_Zero_ReturnsZero()
        {
            TestHelper.AssertApprox(Math.Atan(FixedPoint.Zero), 0.0, 0.01);
        }

        [Test]
        public void Atan_One_ReturnsPiOver4()
        {
            TestHelper.AssertApprox(Math.Atan(FixedPoint.One), System.Math.PI / 4, 0.02);
        }

        [Test]
        public void Atan2_ZeroZero_ReturnsZero()
        {
            Assert.AreEqual(FixedPoint.Zero, Math.Atan2(FixedPoint.Zero, FixedPoint.Zero));
        }

        [Test]
        public void Atan2_PositiveY_ReturnsPiOver2()
        {
            TestHelper.AssertApprox(Math.Atan2(FixedPoint.One, FixedPoint.Zero), System.Math.PI / 2, 0.02);
        }

        [Test]
        public void Atan2_PositiveX_ReturnsZero()
        {
            TestHelper.AssertApprox(Math.Atan2(FixedPoint.Zero, FixedPoint.One), 0.0, 0.01);
        }

        [Test]
        public void Atan2_FirstQuadrant_45Degrees()
        {
            TestHelper.AssertApprox(Math.Atan2(FixedPoint.One, FixedPoint.One), System.Math.PI / 4, 0.02);
        }

        [Test]
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

        [Test]
        public void Atan2_Float_MatchesFixedPoint()
        {
            FixedPoint resultFP = Math.Atan2(new FixedPoint(3), new FixedPoint(4));
            FixedPoint resultFloat = Math.Atan2(3.0f, 4.0f);
            long diff = System.Math.Abs(resultFP.FixedValue - resultFloat.FixedValue);
            Assert.IsTrue(diff <= 5, $"float vs FixedPoint Atan2 diff={diff}");
        }

        #endregion

        #region 反三角函数一致性

        [Test]
        public void Acos_Cos_RoundTrip()
        {
            FixedPoint angle = FixedPoint.One;
            FixedPoint roundTrip = Math.Acos(Math.Cos(angle));
            TestHelper.AssertApprox(roundTrip, 1.0, 0.02);
        }

        [Test]
        public void Sin_Asin_RoundTrip()
        {
            FixedPoint half = new FixedPoint(0.5);
            FixedPoint roundTrip = Math.Sin(Math.Asin(half));
            TestHelper.AssertApprox(roundTrip, 0.5, 0.02);
        }

        #endregion

        #region 特殊角度精确值 (通过 DegreesToRadians 路径，确定性回归测试)

        [Test]
        public void SinCos_0Deg_Exact()
        {
            FixedPoint rad = Math.DegreesToRadians(new FixedPoint(0));
            Assert.AreEqual(0L, Math.Sin(rad).FixedValue, "Sin(0°)");
            Assert.AreEqual(1024L, Math.Cos(rad).FixedValue, "Cos(0°)");
        }

        [Test]
        public void SinCos_30Deg_Exact()
        {
            FixedPoint rad = Math.DegreesToRadians(new FixedPoint(30));
            Assert.AreEqual(512L, Math.Sin(rad).FixedValue, "Sin(30°)");
            Assert.AreEqual(887L, Math.Cos(rad).FixedValue, "Cos(30°)");
        }

        [Test]
        public void SinCos_45Deg_Exact()
        {
            FixedPoint rad = Math.DegreesToRadians(new FixedPoint(45));
            Assert.AreEqual(724L, Math.Sin(rad).FixedValue, "Sin(45°)");
            Assert.AreEqual(724L, Math.Cos(rad).FixedValue, "Cos(45°)");
        }

        [Test]
        public void SinCos_60Deg_Exact()
        {
            FixedPoint rad = Math.DegreesToRadians(new FixedPoint(60));
            Assert.AreEqual(887L, Math.Sin(rad).FixedValue, "Sin(60°)");
            Assert.AreEqual(512L, Math.Cos(rad).FixedValue, "Cos(60°)");
        }

        [Test]
        public void SinCos_90Deg_Exact()
        {
            FixedPoint rad = Math.DegreesToRadians(new FixedPoint(90));
            Assert.AreEqual(1024L, Math.Sin(rad).FixedValue, "Sin(90°)");
            Assert.AreEqual(0L, Math.Cos(rad).FixedValue, "Cos(90°)");
        }

        [Test]
        public void SinCos_120Deg_Exact()
        {
            FixedPoint rad = Math.DegreesToRadians(new FixedPoint(120));
            Assert.AreEqual(887L, Math.Sin(rad).FixedValue, "Sin(120°)");
            Assert.AreEqual(-512L, Math.Cos(rad).FixedValue, "Cos(120°)");
        }

        [Test]
        public void SinCos_150Deg_Exact()
        {
            FixedPoint rad = Math.DegreesToRadians(new FixedPoint(150));
            Assert.AreEqual(512L, Math.Sin(rad).FixedValue, "Sin(150°)");
            Assert.AreEqual(-887L, Math.Cos(rad).FixedValue, "Cos(150°)");
        }

        [Test]
        public void SinCos_180Deg_Exact()
        {
            FixedPoint rad = Math.DegreesToRadians(new FixedPoint(180));
            Assert.AreEqual(0L, Math.Sin(rad).FixedValue, "Sin(180°)");
            Assert.AreEqual(-1024L, Math.Cos(rad).FixedValue, "Cos(180°)");
        }

        [Test]
        public void SinCos_270Deg_Exact()
        {
            FixedPoint rad = Math.DegreesToRadians(new FixedPoint(270));
            Assert.AreEqual(-1024L, Math.Sin(rad).FixedValue, "Sin(270°)");
            Assert.AreEqual(0L, Math.Cos(rad).FixedValue, "Cos(270°)");
        }

        [Test]
        public void SinCos_360Deg_Exact()
        {
            FixedPoint rad = Math.DegreesToRadians(new FixedPoint(360));
            Assert.AreEqual(0L, Math.Sin(rad).FixedValue, "Sin(360°)");
            Assert.AreEqual(1024L, Math.Cos(rad).FixedValue, "Cos(360°)");
        }

        [Test]
        public void Tan_45Deg_ReturnsOne()
        {
            FixedPoint rad = Math.DegreesToRadians(new FixedPoint(45));
            Assert.AreEqual(1024L, Math.Tan(rad).FixedValue, "Tan(45°)");
        }

        [Test]
        public void Tan_90Deg_Saturates()
        {
            FixedPoint rad = Math.DegreesToRadians(new FixedPoint(90));
            Assert.AreEqual(FixedPoint.MaxValue.FixedValue, Math.Tan(rad).FixedValue, "Tan(90°)");
        }

        #endregion
    }
}
