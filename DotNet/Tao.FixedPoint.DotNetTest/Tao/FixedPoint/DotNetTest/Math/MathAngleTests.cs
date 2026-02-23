namespace Tao.FixedPoint.DotNetTest
{
    /// <summary>
    /// 角度与周期函数测试：DegreesToRadians, RadiansToDegrees, DeltaAngle, LerpAngle, MoveTowardsAngle, Repeat, PingPong
    /// </summary>
    [TestClass]
    public sealed class MathAngleTests
    {
        #region DegreesToRadians / RadiansToDegrees

        /// <summary>
        /// 180° → π
        /// </summary>
        [TestMethod]
        public void DegreesToRadians_180_ReturnsPi()
        {
            TestHelper.AssertApprox(Math.DegreesToRadians(new FixedPoint(180)), System.Math.PI, 0.01);
        }

        /// <summary>
        /// 360° → 2π
        /// </summary>
        [TestMethod]
        public void DegreesToRadians_360_ReturnsTwoPi()
        {
            TestHelper.AssertApprox(Math.DegreesToRadians(new FixedPoint(360)), 2 * System.Math.PI, 0.02);
        }

        /// <summary>
        /// 90° → π/2
        /// </summary>
        [TestMethod]
        public void DegreesToRadians_90_ReturnsPiOver2()
        {
            TestHelper.AssertApprox(Math.DegreesToRadians(new FixedPoint(90)), System.Math.PI / 2, 0.01);
        }

        /// <summary>
        /// π → 180°
        /// </summary>
        [TestMethod]
        public void RadiansToDegrees_Pi_Returns180()
        {
            TestHelper.AssertApprox(Math.RadiansToDegrees(FixedPoint.Pi), 180.0, 0.5);
        }

        /// <summary>
        /// 度数和弧度互为反函数
        /// </summary>
        [TestMethod]
        public void DegreesRadians_RoundTrip()
        {
            FixedPoint degrees = new FixedPoint(45);
            FixedPoint radians = Math.DegreesToRadians(degrees);
            FixedPoint roundTrip = Math.RadiansToDegrees(radians);
            TestHelper.AssertApprox(roundTrip, 45.0, 0.5);
        }

        /// <summary>
        /// Deg2Rad 常量近似 π/180
        /// </summary>
        [TestMethod]
        public void Deg2Rad_Constant_ApproxCorrect()
        {
            TestHelper.AssertApprox(Math.Deg2Rad, System.Math.PI / 180, 0.002);
        }

        /// <summary>
        /// Rad2Deg 常量近似 180/π
        /// </summary>
        [TestMethod]
        public void Rad2Deg_Constant_ApproxCorrect()
        {
            TestHelper.AssertApprox(Math.Rad2Deg, 180 / System.Math.PI, 0.1);
        }

        #endregion

        #region DeltaAngle

        /// <summary>
        /// DeltaAngle 同方向不回绕
        /// </summary>
        [TestMethod]
        public void DeltaAngle_SmallDifference_ReturnsDiff()
        {
            TestHelper.AssertApprox(Math.DeltaAngle(new FixedPoint(10), new FixedPoint(30)), 20.0, 0.5);
        }

        /// <summary>
        /// DeltaAngle 跨越 360° 边界取最短路径
        /// </summary>
        [TestMethod]
        public void DeltaAngle_Wrapping_ReturnsShortest()
        {
            TestHelper.AssertApprox(Math.DeltaAngle(new FixedPoint(350), new FixedPoint(10)), 20.0, 0.5);
        }

        /// <summary>
        /// DeltaAngle 反方向
        /// </summary>
        [TestMethod]
        public void DeltaAngle_Reverse_NegativeDelta()
        {
            TestHelper.AssertApprox(Math.DeltaAngle(new FixedPoint(30), new FixedPoint(10)), -20.0, 0.5);
        }

        /// <summary>
        /// DeltaAngle 相同角度返回零
        /// </summary>
        [TestMethod]
        public void DeltaAngle_Same_ReturnsZero()
        {
            TestHelper.AssertApprox(Math.DeltaAngle(new FixedPoint(90), new FixedPoint(90)), 0.0, 0.001);
        }

        #endregion

        #region LerpAngle

        /// <summary>
        /// LerpAngle t=0 返回 a
        /// </summary>
        [TestMethod]
        public void LerpAngle_T0_ReturnsA()
        {
            FixedPoint result = Math.LerpAngle(new FixedPoint(10), new FixedPoint(30), FixedPoint.Zero);
            TestHelper.AssertApprox(result, 10.0, 0.5);
        }

        /// <summary>
        /// LerpAngle t=1 返回 b
        /// </summary>
        [TestMethod]
        public void LerpAngle_T1_ReturnsB()
        {
            FixedPoint result = Math.LerpAngle(new FixedPoint(10), new FixedPoint(30), FixedPoint.One);
            TestHelper.AssertApprox(result, 30.0, 0.5);
        }

        /// <summary>
        /// LerpAngle 跨越 360° 边界正确插值 (350→10 中点 = 360，等价于 0°)
        /// </summary>
        [TestMethod]
        public void LerpAngle_Wrapping_Interpolates()
        {
            FixedPoint result = Math.LerpAngle(new FixedPoint(350), new FixedPoint(10), new FixedPoint(0.5));
            TestHelper.AssertApprox(result, 360.0, 1.0);
        }

        #endregion

        #region MoveTowardsAngle

        /// <summary>
        /// MoveTowardsAngle 在 delta 范围内直接到达
        /// </summary>
        [TestMethod]
        public void MoveTowardsAngle_WithinDelta_Reaches()
        {
            FixedPoint result = Math.MoveTowardsAngle(new FixedPoint(10), new FixedPoint(15), new FixedPoint(10));
            TestHelper.AssertApprox(result, 15.0, 0.5);
        }

        /// <summary>
        /// MoveTowardsAngle 超过 delta 时只移动 delta
        /// </summary>
        [TestMethod]
        public void MoveTowardsAngle_BeyondDelta_MovesByDelta()
        {
            FixedPoint result = Math.MoveTowardsAngle(new FixedPoint(0), new FixedPoint(90), new FixedPoint(30));
            TestHelper.AssertApprox(result, 30.0, 0.5);
        }

        #endregion

        #region Repeat

        /// <summary>
        /// Repeat 在范围内返回自身
        /// </summary>
        [TestMethod]
        public void Repeat_InRange_ReturnsSelf()
        {
            TestHelper.AssertApprox(Math.Repeat(new FixedPoint(3), new FixedPoint(10)), 3.0, 0.01);
        }

        /// <summary>
        /// Repeat 超出范围时回绕
        /// </summary>
        [TestMethod]
        public void Repeat_AboveRange_Wraps()
        {
            TestHelper.AssertApprox(Math.Repeat(new FixedPoint(13), new FixedPoint(10)), 3.0, 0.01);
        }

        /// <summary>
        /// Repeat 负值时回绕到正范围
        /// </summary>
        [TestMethod]
        public void Repeat_Negative_WrapsToPositive()
        {
            FixedPoint result = Math.Repeat(new FixedPoint(-3), new FixedPoint(10));
            TestHelper.AssertApprox(result, 7.0, 0.01);
        }

        #endregion

        #region PingPong

        /// <summary>
        /// PingPong 在第一段正向递增
        /// </summary>
        [TestMethod]
        public void PingPong_FirstHalf_Increases()
        {
            TestHelper.AssertApprox(Math.PingPong(new FixedPoint(3), new FixedPoint(5)), 3.0, 0.01);
        }

        /// <summary>
        /// PingPong 在第二段反向递减
        /// </summary>
        [TestMethod]
        public void PingPong_SecondHalf_Decreases()
        {
            TestHelper.AssertApprox(Math.PingPong(new FixedPoint(7), new FixedPoint(5)), 3.0, 0.01);
        }

        /// <summary>
        /// PingPong 边界: t=length 返回 length
        /// </summary>
        [TestMethod]
        public void PingPong_AtLength_ReturnsLength()
        {
            TestHelper.AssertApprox(Math.PingPong(new FixedPoint(5), new FixedPoint(5)), 5.0, 0.01);
        }

        /// <summary>
        /// PingPong 零返回零
        /// </summary>
        [TestMethod]
        public void PingPong_Zero_ReturnsZero()
        {
            TestHelper.AssertApprox(Math.PingPong(FixedPoint.Zero, new FixedPoint(5)), 0.0, 0.01);
        }

        #endregion
    }
}