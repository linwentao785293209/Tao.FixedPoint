using System;
using NUnit.Framework;

namespace Tao.FixedPoint.UnityTest
{
    /// <summary>
    /// Quaternion 测试：Identity, Euler, AngleAxis, 乘法, 向量旋转, Slerp, Inverse
    /// </summary>
    [TestFixture]
    public class QuaternionTests
    {
        #region Identity

        [Test]
        public void Identity_Components()
        {
            Quaternion q = Quaternion.Identity;
            Assert.AreEqual(FixedPoint.Zero, q.x);
            Assert.AreEqual(FixedPoint.Zero, q.y);
            Assert.AreEqual(FixedPoint.Zero, q.z);
            Assert.AreEqual(FixedPoint.One, q.w);
        }

        [Test]
        public void Identity_DoesNotRotateVector()
        {
            Vector3 v = new Vector3(new FixedPoint(3), new FixedPoint(4), new FixedPoint(5));
            Vector3 rotated = Quaternion.Identity * v;
            TestHelper.AssertApprox(rotated, 3.0, 4.0, 5.0, 0.01);
        }

        #endregion

        #region Euler

        [Test]
        public void Euler_Zero_ReturnsIdentity()
        {
            Quaternion q = Quaternion.Euler(FixedPoint.Zero, FixedPoint.Zero, FixedPoint.Zero);
            Assert.IsTrue(
                Math.Approximately(q.x, FixedPoint.Zero) &&
                Math.Approximately(q.y, FixedPoint.Zero) &&
                Math.Approximately(q.z, FixedPoint.Zero) &&
                Math.Approximately(q.w, FixedPoint.One));
        }

        [Test]
        public void Euler_Y90_RotatesRightToNegForward()
        {
            Quaternion q = Quaternion.Euler(FixedPoint.Zero, new FixedPoint(90), FixedPoint.Zero);
            Vector3 result = q * Vector3.Right;
            TestHelper.AssertApprox(result, 0.0, 0.0, -1.0, 0.05);
        }

        [Test]
        public void Euler_Vector_MatchesScalar()
        {
            Vector3 angles = new Vector3(new FixedPoint(30), new FixedPoint(45), new FixedPoint(60));
            Quaternion fromVec = Quaternion.Euler(angles);
            Quaternion fromScalar = Quaternion.Euler(new FixedPoint(30), new FixedPoint(45), new FixedPoint(60));
            Assert.AreEqual(fromVec, fromScalar);
        }

        [Test]
        public void EulerAngles_RoundTrip_SimpleAxis()
        {
            Quaternion q = Quaternion.Euler(FixedPoint.Zero, new FixedPoint(90), FixedPoint.Zero);
            Vector3 euler = q.EulerAngles;
            TestHelper.AssertApprox(euler.y, 90.0, 1.5);
        }

        #endregion

        #region AngleAxis

        [Test]
        public void AngleAxis_Y180_FlipsX()
        {
            Quaternion q = Quaternion.AngleAxis(new FixedPoint(180), Vector3.Up);
            Vector3 result = q * Vector3.Right;
            TestHelper.AssertApprox(result, -1.0, 0.0, 0.0, 0.05);
        }

        [Test]
        public void AngleAxis_Zero_IsIdentity()
        {
            Quaternion q = Quaternion.AngleAxis(FixedPoint.Zero, Vector3.Up);
            Vector3 result = q * Vector3.Forward;
            TestHelper.AssertApprox(result, 0.0, 0.0, 1.0, 0.01);
        }

        #endregion

        #region 乘法

        [Test]
        public void Multiply_WithInverse_ReturnsIdentity()
        {
            Quaternion q = Quaternion.Euler(new FixedPoint(30), new FixedPoint(45), new FixedPoint(60));
            Quaternion inv = Quaternion.Inverse(q);
            Quaternion result = q * inv;

            TestHelper.AssertApprox(result.x, 0.0, 0.02);
            TestHelper.AssertApprox(result.y, 0.0, 0.02);
            TestHelper.AssertApprox(result.z, 0.0, 0.02);
            TestHelper.AssertApprox(result.w, 1.0, 0.02);
        }

        [Test]
        public void Multiply_Identity_ReturnsSelf()
        {
            Quaternion q = Quaternion.Euler(new FixedPoint(30), new FixedPoint(45), new FixedPoint(60));
            Quaternion result = Quaternion.Identity * q;
            Assert.AreEqual(q.x, result.x);
            Assert.AreEqual(q.y, result.y);
            Assert.AreEqual(q.z, result.z);
            Assert.AreEqual(q.w, result.w);
        }

        #endregion

        #region Dot / Angle / Inverse

        [Test]
        public void Dot_Same_ReturnsOne()
        {
            Quaternion q = Quaternion.Identity;
            TestHelper.AssertApprox(Quaternion.Dot(q, q), 1.0, 0.01);
        }

        [Test]
        public void Angle_Same_ReturnsZero()
        {
            TestHelper.AssertApprox(Quaternion.Angle(Quaternion.Identity, Quaternion.Identity), 0.0, 0.5);
        }

        [Test]
        public void Angle_90Deg_Returns90()
        {
            Quaternion a = Quaternion.Identity;
            Quaternion b = Quaternion.Euler(FixedPoint.Zero, new FixedPoint(90), FixedPoint.Zero);
            TestHelper.AssertApprox(Quaternion.Angle(a, b), 90.0, 2.0);
        }

        [Test]
        public void Inverse_NegatesImaginary()
        {
            Quaternion q = Quaternion.Euler(new FixedPoint(30), new FixedPoint(45), new FixedPoint(0));
            Quaternion inv = Quaternion.Inverse(q);
            Assert.AreEqual(-q.x.FixedValue, inv.x.FixedValue);
            Assert.AreEqual(-q.y.FixedValue, inv.y.FixedValue);
            Assert.AreEqual(-q.z.FixedValue, inv.z.FixedValue);
            Assert.AreEqual(q.w, inv.w);
        }

        #endregion

        #region Slerp / Lerp

        [Test]
        public void Slerp_T0_ReturnsA()
        {
            Quaternion a = Quaternion.Identity;
            Quaternion b = Quaternion.Euler(FixedPoint.Zero, new FixedPoint(90), FixedPoint.Zero);
            Quaternion result = Quaternion.Slerp(a, b, FixedPoint.Zero);
            Assert.AreEqual(a, result);
        }

        [Test]
        public void Slerp_T1_ReturnsB()
        {
            Quaternion a = Quaternion.Identity;
            Quaternion b = Quaternion.Euler(FixedPoint.Zero, new FixedPoint(90), FixedPoint.Zero);
            Quaternion result = Quaternion.Slerp(a, b, FixedPoint.One);

            Vector3 rotatedByB = b * Vector3.Forward;
            Vector3 rotatedByResult = result * Vector3.Forward;
            TestHelper.AssertApprox(rotatedByResult, rotatedByB.x.RawDouble, rotatedByB.y.RawDouble,
                rotatedByB.z.RawDouble, 0.02);
        }

        [Test]
        public void Lerp_Half_InBetween()
        {
            Quaternion a = Quaternion.Identity;
            Quaternion b = Quaternion.Euler(FixedPoint.Zero, new FixedPoint(90), FixedPoint.Zero);
            Quaternion result = Quaternion.Lerp(a, b, new FixedPoint(0.5));

            Vector3 rotated = result * Vector3.Forward;
            Assert.IsTrue(rotated.x > FixedPoint.Zero, "Should have positive X component");
            Assert.IsTrue(rotated.z > FixedPoint.Zero, "Should have positive Z component");
        }

        #endregion

        #region RotateTowards / FromToRotation / LookRotation

        [Test]
        public void RotateTowards_LargeDelta_ReachesTarget()
        {
            Quaternion a = Quaternion.Identity;
            Quaternion b = Quaternion.Euler(FixedPoint.Zero, new FixedPoint(90), FixedPoint.Zero);
            Quaternion result = Quaternion.RotateTowards(a, b, new FixedPoint(180));

            Vector3 rotatedByB = b * Vector3.Forward;
            Vector3 rotatedByResult = result * Vector3.Forward;
            TestHelper.AssertApprox(rotatedByResult, rotatedByB.x.RawDouble, rotatedByB.y.RawDouble,
                rotatedByB.z.RawDouble, 0.05);
        }

        [Test]
        public void FromToRotation_RotatesFromToTo()
        {
            Quaternion q = Quaternion.FromToRotation(Vector3.Right, Vector3.Forward);
            Vector3 result = q * Vector3.Right;
            TestHelper.AssertApprox(result, 0.0, 0.0, 1.0, 0.05);
        }

        [Test]
        public void LookRotation_ForwardDirection()
        {
            Quaternion q = Quaternion.LookRotation(Vector3.Forward);
            Vector3 result = q * Vector3.Forward;
            TestHelper.AssertApprox(result, 0.0, 0.0, 1.0, 0.05);
        }

        #endregion

        #region Equals / GetHashCode / 索引器

        [Test]
        public void Equals_Consistency()
        {
            Quaternion a = Quaternion.Identity;
            Quaternion b = Quaternion.Identity;
            Assert.IsTrue(a == b);
            Assert.IsTrue(a.Equals(b));
            Assert.IsTrue(a.Equals((object)b));
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void Indexer_GetSet()
        {
            Quaternion q = Quaternion.Identity;
            Assert.AreEqual(FixedPoint.Zero, q[0]);
            Assert.AreEqual(FixedPoint.One, q[3]);
        }

        [Test]
        public void Indexer_OutOfRange_Throws()
        {
            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                Quaternion q = Quaternion.Identity;
                FixedPoint _ = q[4];
            });
        }

        #endregion
    }
}
