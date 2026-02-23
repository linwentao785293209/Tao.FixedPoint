namespace Tao.FixedPoint.DotNetTest
{
    /// <summary>
    /// 插值与比较函数测试：Lerp, LerpUnclamped, InverseLerp, MoveTowards, SmoothStep, SmoothDamp, Approximately
    /// </summary>
    [TestClass]
    public sealed class MathInterpolationTests
    {
        #region Lerp

        /// <summary>
        /// Lerp t=0 返回起始值
        /// </summary>
        [TestMethod]
        public void Lerp_T0_ReturnsA()
        {
            FixedPoint result = Math.Lerp(new FixedPoint(0), new FixedPoint(10), FixedPoint.Zero);
            Assert.AreEqual(new FixedPoint(0).FixedValue, result.FixedValue);
        }

        /// <summary>
        /// Lerp t=1 返回终止值
        /// </summary>
        [TestMethod]
        public void Lerp_T1_ReturnsB()
        {
            FixedPoint result = Math.Lerp(new FixedPoint(0), new FixedPoint(10), FixedPoint.One);
            Assert.AreEqual(new FixedPoint(10).FixedValue, result.FixedValue);
        }

        /// <summary>
        /// Lerp t=0.5 返回中点
        /// </summary>
        [TestMethod]
        public void Lerp_THalf_ReturnsMidpoint()
        {
            FixedPoint result = Math.Lerp(new FixedPoint(0), new FixedPoint(10), new FixedPoint(0.5));
            TestHelper.AssertApprox(result, 5.0, 0.01);
        }

        /// <summary>
        /// Lerp t < 0 被钳位到 0
        /// </summary>
        [TestMethod]
        public void Lerp_TNegative_ClampsToA()
        {
            FixedPoint result = Math.Lerp(new FixedPoint(0), new FixedPoint(10), new FixedPoint(-1));
            Assert.AreEqual(new FixedPoint(0).FixedValue, result.FixedValue);
        }

        /// <summary>
        /// Lerp t > 1 被钳位到 1
        /// </summary>
        [TestMethod]
        public void Lerp_TOver1_ClampsToB()
        {
            FixedPoint result = Math.Lerp(new FixedPoint(0), new FixedPoint(10), new FixedPoint(2));
            Assert.AreEqual(new FixedPoint(10).FixedValue, result.FixedValue);
        }

        #endregion

        #region LerpUnclamped

        /// <summary>
        /// LerpUnclamped t=2 允许超出范围
        /// </summary>
        [TestMethod]
        public void LerpUnclamped_T2_Extrapolates()
        {
            FixedPoint result = Math.LerpUnclamped(new FixedPoint(0), new FixedPoint(10), new FixedPoint(2));
            TestHelper.AssertApprox(result, 20.0, 0.01);
        }

        /// <summary>
        /// LerpUnclamped t=-0.5 允许反方向外推
        /// </summary>
        [TestMethod]
        public void LerpUnclamped_TNegative_Extrapolates()
        {
            FixedPoint result = Math.LerpUnclamped(new FixedPoint(0), new FixedPoint(10), new FixedPoint(-0.5));
            TestHelper.AssertApprox(result, -5.0, 0.01);
        }

        #endregion

        #region InverseLerp

        /// <summary>
        /// InverseLerp 中间值返回 0.5
        /// </summary>
        [TestMethod]
        public void InverseLerp_Midpoint_ReturnsHalf()
        {
            FixedPoint result = Math.InverseLerp(new FixedPoint(0), new FixedPoint(10), new FixedPoint(5));
            TestHelper.AssertApprox(result, 0.5, 0.01);
        }

        /// <summary>
        /// InverseLerp 端点值
        /// </summary>
        [TestMethod]
        public void InverseLerp_Endpoints()
        {
            FixedPoint t0 = Math.InverseLerp(new FixedPoint(0), new FixedPoint(10), new FixedPoint(0));
            FixedPoint t1 = Math.InverseLerp(new FixedPoint(0), new FixedPoint(10), new FixedPoint(10));
            TestHelper.AssertApprox(t0, 0.0, 0.001);
            TestHelper.AssertApprox(t1, 1.0, 0.001);
        }

        /// <summary>
        /// InverseLerp a == b 时返回零
        /// </summary>
        [TestMethod]
        public void InverseLerp_EqualRange_ReturnsZero()
        {
            FixedPoint result = Math.InverseLerp(new FixedPoint(5), new FixedPoint(5), new FixedPoint(5));
            Assert.AreEqual(FixedPoint.Zero, result);
        }

        /// <summary>
        /// InverseLerp 超出范围被钳位到 [0, 1]
        /// </summary>
        [TestMethod]
        public void InverseLerp_OutOfRange_Clamped()
        {
            FixedPoint below = Math.InverseLerp(new FixedPoint(0), new FixedPoint(10), new FixedPoint(-5));
            FixedPoint above = Math.InverseLerp(new FixedPoint(0), new FixedPoint(10), new FixedPoint(15));
            Assert.AreEqual(FixedPoint.Zero, below);
            Assert.AreEqual(FixedPoint.One, above);
        }

        #endregion

        #region MoveTowards

        /// <summary>
        /// MoveTowards 距离小于 delta 时直接到达目标
        /// </summary>
        [TestMethod]
        public void MoveTowards_WithinDelta_ReachesTarget()
        {
            FixedPoint result = Math.MoveTowards(new FixedPoint(0), new FixedPoint(3), new FixedPoint(5));
            Assert.AreEqual(new FixedPoint(3).FixedValue, result.FixedValue);
        }

        /// <summary>
        /// MoveTowards 距离大于 delta 时移动 delta 步长
        /// </summary>
        [TestMethod]
        public void MoveTowards_BeyondDelta_MovesByDelta()
        {
            FixedPoint result = Math.MoveTowards(new FixedPoint(0), new FixedPoint(10), new FixedPoint(3));
            TestHelper.AssertApprox(result, 3.0, 0.01);
        }

        /// <summary>
        /// MoveTowards 反方向
        /// </summary>
        [TestMethod]
        public void MoveTowards_Negative_MovesByDelta()
        {
            FixedPoint result = Math.MoveTowards(new FixedPoint(10), new FixedPoint(0), new FixedPoint(3));
            TestHelper.AssertApprox(result, 7.0, 0.01);
        }

        #endregion

        #region SmoothStep

        /// <summary>
        /// SmoothStep t=0 返回 from
        /// </summary>
        [TestMethod]
        public void SmoothStep_T0_ReturnsFrom()
        {
            FixedPoint result = Math.SmoothStep(new FixedPoint(0), new FixedPoint(10), FixedPoint.Zero);
            Assert.AreEqual(new FixedPoint(0).FixedValue, result.FixedValue);
        }

        /// <summary>
        /// SmoothStep t=1 返回 to
        /// </summary>
        [TestMethod]
        public void SmoothStep_T1_ReturnsTo()
        {
            FixedPoint result = Math.SmoothStep(new FixedPoint(0), new FixedPoint(10), FixedPoint.One);
            Assert.AreEqual(new FixedPoint(10).FixedValue, result.FixedValue);
        }

        /// <summary>
        /// SmoothStep t=0.5 在中间附近
        /// </summary>
        [TestMethod]
        public void SmoothStep_THalf_NearMidpoint()
        {
            FixedPoint result = Math.SmoothStep(new FixedPoint(0), new FixedPoint(10), new FixedPoint(0.5));
            TestHelper.AssertApprox(result, 5.0, 0.1);
        }

        #endregion

        #region SmoothDamp

        /// <summary>
        /// SmoothDamp 朝目标方向移动
        /// </summary>
        [TestMethod]
        public void SmoothDamp_MovesTowardsTarget()
        {
            FixedPoint current = new FixedPoint(0);
            FixedPoint target = new FixedPoint(10);
            FixedPoint velocity = FixedPoint.Zero;
            FixedPoint smoothTime = new FixedPoint(1);
            FixedPoint deltaTime = new FixedPoint(0.02);
            FixedPoint maxSpeed = new FixedPoint(100);

            FixedPoint result = Math.SmoothDamp(current, target, ref velocity, smoothTime, deltaTime, maxSpeed);
            Assert.IsTrue(result > current, "SmoothDamp should move towards target");
            Assert.IsTrue(result < target, "SmoothDamp should not overshoot on first step");
        }

        #endregion

        #region Approximately

        /// <summary>
        /// Approximately 相同值返回 true
        /// </summary>
        [TestMethod]
        public void Approximately_SameValue_ReturnsTrue()
        {
            Assert.IsTrue(Math.Approximately(new FixedPoint(5), new FixedPoint(5)));
        }

        /// <summary>
        /// Approximately 差 1 个 Epsilon 返回 true
        /// </summary>
        [TestMethod]
        public void Approximately_DiffByEpsilon_ReturnsTrue()
        {
            FixedPoint a = new FixedPoint(5);
            FixedPoint b = new FixedPoint(a.FixedValue + 1L);
            Assert.IsTrue(Math.Approximately(a, b));
        }

        /// <summary>
        /// Approximately 差值超过 Epsilon 返回 false
        /// </summary>
        [TestMethod]
        public void Approximately_LargeDiff_ReturnsFalse()
        {
            Assert.IsFalse(Math.Approximately(new FixedPoint(5), new FixedPoint(6)));
        }

        #endregion
    }
}