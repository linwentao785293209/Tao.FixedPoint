using NUnit.Framework;

namespace Tao.FixedPoint.UnityTest
{
    /// <summary>
    /// 插值与比较函数测试：Lerp, LerpUnclamped, InverseLerp, MoveTowards, SmoothStep, SmoothDamp, Approximately
    /// </summary>
    [TestFixture]
    public class MathInterpolationTests
    {
        #region Lerp

        [Test]
        public void Lerp_T0_ReturnsA()
        {
            FixedPoint result = Math.Lerp(new FixedPoint(0), new FixedPoint(10), FixedPoint.Zero);
            Assert.AreEqual(new FixedPoint(0).FixedValue, result.FixedValue);
        }

        [Test]
        public void Lerp_T1_ReturnsB()
        {
            FixedPoint result = Math.Lerp(new FixedPoint(0), new FixedPoint(10), FixedPoint.One);
            Assert.AreEqual(new FixedPoint(10).FixedValue, result.FixedValue);
        }

        [Test]
        public void Lerp_THalf_ReturnsMidpoint()
        {
            FixedPoint result = Math.Lerp(new FixedPoint(0), new FixedPoint(10), new FixedPoint(0.5));
            TestHelper.AssertApprox(result, 5.0, 0.01);
        }

        [Test]
        public void Lerp_TNegative_ClampsToA()
        {
            FixedPoint result = Math.Lerp(new FixedPoint(0), new FixedPoint(10), new FixedPoint(-1));
            Assert.AreEqual(new FixedPoint(0).FixedValue, result.FixedValue);
        }

        [Test]
        public void Lerp_TOver1_ClampsToB()
        {
            FixedPoint result = Math.Lerp(new FixedPoint(0), new FixedPoint(10), new FixedPoint(2));
            Assert.AreEqual(new FixedPoint(10).FixedValue, result.FixedValue);
        }

        #endregion

        #region LerpUnclamped

        [Test]
        public void LerpUnclamped_T2_Extrapolates()
        {
            FixedPoint result = Math.LerpUnclamped(new FixedPoint(0), new FixedPoint(10), new FixedPoint(2));
            TestHelper.AssertApprox(result, 20.0, 0.01);
        }

        [Test]
        public void LerpUnclamped_TNegative_Extrapolates()
        {
            FixedPoint result = Math.LerpUnclamped(new FixedPoint(0), new FixedPoint(10), new FixedPoint(-0.5));
            TestHelper.AssertApprox(result, -5.0, 0.01);
        }

        #endregion

        #region InverseLerp

        [Test]
        public void InverseLerp_Midpoint_ReturnsHalf()
        {
            FixedPoint result = Math.InverseLerp(new FixedPoint(0), new FixedPoint(10), new FixedPoint(5));
            TestHelper.AssertApprox(result, 0.5, 0.01);
        }

        [Test]
        public void InverseLerp_Endpoints()
        {
            FixedPoint t0 = Math.InverseLerp(new FixedPoint(0), new FixedPoint(10), new FixedPoint(0));
            FixedPoint t1 = Math.InverseLerp(new FixedPoint(0), new FixedPoint(10), new FixedPoint(10));
            TestHelper.AssertApprox(t0, 0.0, 0.001);
            TestHelper.AssertApprox(t1, 1.0, 0.001);
        }

        [Test]
        public void InverseLerp_EqualRange_ReturnsZero()
        {
            FixedPoint result = Math.InverseLerp(new FixedPoint(5), new FixedPoint(5), new FixedPoint(5));
            Assert.AreEqual(FixedPoint.Zero, result);
        }

        [Test]
        public void InverseLerp_OutOfRange_Clamped()
        {
            FixedPoint below = Math.InverseLerp(new FixedPoint(0), new FixedPoint(10), new FixedPoint(-5));
            FixedPoint above = Math.InverseLerp(new FixedPoint(0), new FixedPoint(10), new FixedPoint(15));
            Assert.AreEqual(FixedPoint.Zero, below);
            Assert.AreEqual(FixedPoint.One, above);
        }

        #endregion

        #region MoveTowards

        [Test]
        public void MoveTowards_WithinDelta_ReachesTarget()
        {
            FixedPoint result = Math.MoveTowards(new FixedPoint(0), new FixedPoint(3), new FixedPoint(5));
            Assert.AreEqual(new FixedPoint(3).FixedValue, result.FixedValue);
        }

        [Test]
        public void MoveTowards_BeyondDelta_MovesByDelta()
        {
            FixedPoint result = Math.MoveTowards(new FixedPoint(0), new FixedPoint(10), new FixedPoint(3));
            TestHelper.AssertApprox(result, 3.0, 0.01);
        }

        [Test]
        public void MoveTowards_Negative_MovesByDelta()
        {
            FixedPoint result = Math.MoveTowards(new FixedPoint(10), new FixedPoint(0), new FixedPoint(3));
            TestHelper.AssertApprox(result, 7.0, 0.01);
        }

        #endregion

        #region SmoothStep

        [Test]
        public void SmoothStep_T0_ReturnsFrom()
        {
            FixedPoint result = Math.SmoothStep(new FixedPoint(0), new FixedPoint(10), FixedPoint.Zero);
            Assert.AreEqual(new FixedPoint(0).FixedValue, result.FixedValue);
        }

        [Test]
        public void SmoothStep_T1_ReturnsTo()
        {
            FixedPoint result = Math.SmoothStep(new FixedPoint(0), new FixedPoint(10), FixedPoint.One);
            Assert.AreEqual(new FixedPoint(10).FixedValue, result.FixedValue);
        }

        [Test]
        public void SmoothStep_THalf_NearMidpoint()
        {
            FixedPoint result = Math.SmoothStep(new FixedPoint(0), new FixedPoint(10), new FixedPoint(0.5));
            TestHelper.AssertApprox(result, 5.0, 0.1);
        }

        #endregion

        #region SmoothDamp

        [Test]
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

        [Test]
        public void Approximately_SameValue_ReturnsTrue()
        {
            Assert.IsTrue(Math.Approximately(new FixedPoint(5), new FixedPoint(5)));
        }

        [Test]
        public void Approximately_DiffByEpsilon_ReturnsTrue()
        {
            FixedPoint a = new FixedPoint(5);
            FixedPoint b = new FixedPoint(a.FixedValue + 1L);
            Assert.IsTrue(Math.Approximately(a, b));
        }

        [Test]
        public void Approximately_LargeDiff_ReturnsFalse()
        {
            Assert.IsFalse(Math.Approximately(new FixedPoint(5), new FixedPoint(6)));
        }

        #endregion
    }
}
