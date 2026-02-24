using System;
using NUnit.Framework;

namespace Tao.FixedPoint.UnityTest
{
    /// <summary>
    /// Vector2 测试：构造、运算符、点积、距离、归一化、插值、反射等
    /// </summary>
    [TestFixture]
    public class Vector2Tests
    {
        #region 构造与静态属性

        [Test]
        public void Constructor_FixedPoint_SetsComponents()
        {
            Vector2 v = new Vector2(new FixedPoint(3), new FixedPoint(4));
            Assert.AreEqual(new FixedPoint(3).FixedValue, v.x.FixedValue);
            Assert.AreEqual(new FixedPoint(4).FixedValue, v.y.FixedValue);
        }

        [Test]
        public void StaticProperties_CorrectValues()
        {
            Assert.AreEqual(FixedPoint.Zero, Vector2.Zero.x);
            Assert.AreEqual(FixedPoint.Zero, Vector2.Zero.y);
            Assert.AreEqual(FixedPoint.One, Vector2.One.x);
            Assert.AreEqual(FixedPoint.One, Vector2.One.y);
            Assert.AreEqual(FixedPoint.One, Vector2.Up.y);
            Assert.AreEqual(FixedPoint.NegativeOne, Vector2.Down.y);
            Assert.AreEqual(FixedPoint.NegativeOne, Vector2.Left.x);
            Assert.AreEqual(FixedPoint.One, Vector2.Right.x);
        }

        #endregion

        #region 运算符

        [Test]
        public void Operator_Add_SumsComponents()
        {
            Vector2 a = new Vector2(new FixedPoint(1), new FixedPoint(2));
            Vector2 b = new Vector2(new FixedPoint(3), new FixedPoint(4));
            Vector2 result = a + b;
            Assert.AreEqual(new FixedPoint(4).FixedValue, result.x.FixedValue);
            Assert.AreEqual(new FixedPoint(6).FixedValue, result.y.FixedValue);
        }

        [Test]
        public void Operator_ScalarMul_ScalesComponents()
        {
            Vector2 v = new Vector2(new FixedPoint(2), new FixedPoint(3));
            Vector2 result = v * new FixedPoint(2);
            Assert.AreEqual(new FixedPoint(4).FixedValue, result.x.FixedValue);
            Assert.AreEqual(new FixedPoint(6).FixedValue, result.y.FixedValue);
        }

        [Test]
        public void Operator_Negate_NegatesComponents()
        {
            Vector2 v = new Vector2(new FixedPoint(3), new FixedPoint(-4));
            Vector2 neg = -v;
            Assert.AreEqual(new FixedPoint(-3).FixedValue, neg.x.FixedValue);
            Assert.AreEqual(new FixedPoint(4).FixedValue, neg.y.FixedValue);
        }

        [Test]
        public void Operator_Equality_ComparesComponents()
        {
            Vector2 a = new Vector2(new FixedPoint(1), new FixedPoint(2));
            Vector2 b = new Vector2(new FixedPoint(1), new FixedPoint(2));
            Vector2 c = new Vector2(new FixedPoint(3), new FixedPoint(4));
            Assert.IsTrue(a == b);
            Assert.IsTrue(a != c);
        }

        #endregion

        #region 点积 / 距离 / 长度

        [Test]
        public void Dot_ReturnsCorrectValue()
        {
            Vector2 a = new Vector2(new FixedPoint(3), new FixedPoint(4));
            Vector2 b = new Vector2(new FixedPoint(2), new FixedPoint(1));
            TestHelper.AssertApprox(Vector2.Dot(a, b), 10.0, 0.01);
        }

        [Test]
        public void Dot_Orthogonal_ReturnsZero()
        {
            TestHelper.AssertApprox(Vector2.Dot(Vector2.Right, Vector2.Up), 0.0, 0.001);
        }

        [Test]
        public void Magnitude_345_ReturnsFive()
        {
            Vector2 v = new Vector2(new FixedPoint(3), new FixedPoint(4));
            TestHelper.AssertApprox(v.Magnitude, 5.0, 0.01);
        }

        [Test]
        public void SqrMagnitude_345_Returns25()
        {
            Vector2 v = new Vector2(new FixedPoint(3), new FixedPoint(4));
            TestHelper.AssertApprox(v.SqrMagnitude, 25.0, 0.01);
        }

        [Test]
        public void Distance_ReturnsCorrectValue()
        {
            Vector2 a = new Vector2(new FixedPoint(0), new FixedPoint(0));
            Vector2 b = new Vector2(new FixedPoint(3), new FixedPoint(4));
            TestHelper.AssertApprox(Vector2.Distance(a, b), 5.0, 0.01);
        }

        #endregion

        #region Normalize

        [Test]
        public void Normalize_UnitLength()
        {
            Vector2 v = new Vector2(new FixedPoint(3), new FixedPoint(4));
            Vector2 normalized = v.Normalized;
            TestHelper.AssertApprox(normalized.Magnitude, 1.0, 0.01);
        }

        [Test]
        public void Normalize_ZeroVector_ReturnsZero()
        {
            Vector2 v = Vector2.Zero;
            Assert.AreEqual(Vector2.Zero, v.Normalized);
        }

        #endregion

        #region Lerp / MoveTowards

        [Test]
        public void Lerp_Half_ReturnsMidpoint()
        {
            Vector2 a = new Vector2(new FixedPoint(0), new FixedPoint(0));
            Vector2 b = new Vector2(new FixedPoint(10), new FixedPoint(10));
            Vector2 result = Vector2.Lerp(a, b, new FixedPoint(0.5));
            TestHelper.AssertApprox(result.x, 5.0, 0.01);
            TestHelper.AssertApprox(result.y, 5.0, 0.01);
        }

        [Test]
        public void MoveTowards_MovesByDelta()
        {
            Vector2 current = Vector2.Zero;
            Vector2 target = new Vector2(new FixedPoint(10), new FixedPoint(0));
            Vector2 result = Vector2.MoveTowards(current, target, new FixedPoint(3));
            TestHelper.AssertApprox(result.x, 3.0, 0.01);
            TestHelper.AssertApprox(result.y, 0.0, 0.01);
        }

        #endregion

        #region Reflect / Perpendicular

        [Test]
        public void Reflect_HorizontalNormal_ReflectsY()
        {
            Vector2 inDir = new Vector2(FixedPoint.One, FixedPoint.NegativeOne);
            Vector2 normal = Vector2.Up;
            Vector2 result = Vector2.Reflect(inDir, normal);
            TestHelper.AssertApprox(result.x, 1.0, 0.01);
            TestHelper.AssertApprox(result.y, 1.0, 0.01);
        }

        [Test]
        public void Perpendicular_RotatesBy90()
        {
            Vector2 v = Vector2.Right;
            Vector2 perp = Vector2.Perpendicular(v);
            Assert.AreEqual(FixedPoint.Zero, perp.x);
            Assert.AreEqual(FixedPoint.One, perp.y);
        }

        #endregion

        #region 索引器 / Set / Scale / ClampMagnitude

        [Test]
        public void Indexer_GetSet()
        {
            Vector2 v = new Vector2(new FixedPoint(1), new FixedPoint(2));
            Assert.AreEqual(new FixedPoint(1).FixedValue, v[0].FixedValue);
            Assert.AreEqual(new FixedPoint(2).FixedValue, v[1].FixedValue);
            v[0] = new FixedPoint(5);
            Assert.AreEqual(new FixedPoint(5).FixedValue, v[0].FixedValue);
        }

        [Test]
        public void Indexer_OutOfRange_Throws()
        {
            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                Vector2 v = Vector2.Zero;
                FixedPoint _ = v[2];
            });
        }

        [Test]
        public void Set_UpdatesComponents()
        {
            Vector2 v = Vector2.Zero;
            v.Set(new FixedPoint(3), new FixedPoint(4));
            Assert.AreEqual(new FixedPoint(3).FixedValue, v.x.FixedValue);
            Assert.AreEqual(new FixedPoint(4).FixedValue, v.y.FixedValue);
        }

        [Test]
        public void ClampMagnitude_LongVector_Clamped()
        {
            Vector2 v = new Vector2(new FixedPoint(30), new FixedPoint(40));
            Vector2 clamped = Vector2.ClampMagnitude(v, new FixedPoint(5));
            TestHelper.AssertApprox(clamped.Magnitude, 5.0, 0.02);
        }

        [Test]
        public void ClampMagnitude_ShortVector_Unchanged()
        {
            Vector2 v = new Vector2(new FixedPoint(1), new FixedPoint(0));
            Vector2 clamped = Vector2.ClampMagnitude(v, new FixedPoint(5));
            Assert.AreEqual(v, clamped);
        }

        #endregion

        #region 类型转换

        [Test]
        public void ImplicitConversion_ToVector3()
        {
            Vector2 v2 = new Vector2(new FixedPoint(3), new FixedPoint(4));
            Vector3 v3 = v2;
            Assert.AreEqual(v2.x, v3.x);
            Assert.AreEqual(v2.y, v3.y);
            Assert.AreEqual(FixedPoint.Zero, v3.z);
        }

        #endregion

        #region Equals / GetHashCode / ToString

        [Test]
        public void Equals_HashCode_Consistent()
        {
            Vector2 a = new Vector2(new FixedPoint(1), new FixedPoint(2));
            Vector2 b = new Vector2(new FixedPoint(1), new FixedPoint(2));
            Assert.IsTrue(a.Equals(b));
            Assert.IsTrue(a.Equals((object)b));
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void ToString_ReturnsNonEmpty()
        {
            Vector2 v = new Vector2(new FixedPoint(1), new FixedPoint(2));
            Assert.IsFalse(string.IsNullOrEmpty(v.ToString()));
        }

        #endregion

        #region 补充覆盖

        [Test]
        public void Scale_Instance_MultipliesComponents()
        {
            Vector2 v = new Vector2(new FixedPoint(2), new FixedPoint(3));
            v.Scale(new Vector2(new FixedPoint(4), new FixedPoint(5)));
            Assert.AreEqual(new FixedPoint(8).FixedValue, v.x.FixedValue);
            Assert.AreEqual(new FixedPoint(15).FixedValue, v.y.FixedValue);
        }

        [Test]
        public void Scale_Static_MultipliesComponents()
        {
            Vector2 a = new Vector2(new FixedPoint(2), new FixedPoint(3));
            Vector2 b = new Vector2(new FixedPoint(4), new FixedPoint(5));
            Vector2 result = Vector2.Scale(a, b);
            Assert.AreEqual(new FixedPoint(8).FixedValue, result.x.FixedValue);
            Assert.AreEqual(new FixedPoint(15).FixedValue, result.y.FixedValue);
        }

        [Test]
        public void Angle_Orthogonal_Returns90()
        {
            TestHelper.AssertApprox(Vector2.Angle(Vector2.Right, Vector2.Up), 90.0, 1.0);
        }

        [Test]
        public void SignedAngle_CCW_Positive()
        {
            FixedPoint angle = Vector2.SignedAngle(Vector2.Right, Vector2.Up);
            Assert.IsTrue(angle > FixedPoint.Zero);
            TestHelper.AssertApprox(angle, 90.0, 1.0);
        }

        [Test]
        public void LerpUnclamped_T2_Extrapolates()
        {
            Vector2 a = Vector2.Zero;
            Vector2 b = new Vector2(new FixedPoint(10), new FixedPoint(0));
            Vector2 result = Vector2.LerpUnclamped(a, b, new FixedPoint(2));
            TestHelper.AssertApprox(result.x, 20.0, 0.01);
        }

        [Test]
        public void Min_ReturnsComponentMin()
        {
            Vector2 a = new Vector2(new FixedPoint(1), new FixedPoint(5));
            Vector2 b = new Vector2(new FixedPoint(3), new FixedPoint(2));
            Vector2 result = Vector2.Min(a, b);
            Assert.AreEqual(new FixedPoint(1).FixedValue, result.x.FixedValue);
            Assert.AreEqual(new FixedPoint(2).FixedValue, result.y.FixedValue);
        }

        [Test]
        public void Max_ReturnsComponentMax()
        {
            Vector2 a = new Vector2(new FixedPoint(1), new FixedPoint(5));
            Vector2 b = new Vector2(new FixedPoint(3), new FixedPoint(2));
            Vector2 result = Vector2.Max(a, b);
            Assert.AreEqual(new FixedPoint(3).FixedValue, result.x.FixedValue);
            Assert.AreEqual(new FixedPoint(5).FixedValue, result.y.FixedValue);
        }

        [Test]
        public void Rotate_90Degrees()
        {
            Vector2 result = Vector2.Rotate(Vector2.Right, new FixedPoint(90));
            TestHelper.AssertApprox(result.x, 0.0, 0.02);
            TestHelper.AssertApprox(result.y, 1.0, 0.02);
        }

        [Test]
        public void SmoothDamp_MovesTowardsTarget()
        {
            Vector2 current = Vector2.Zero;
            Vector2 target = new Vector2(new FixedPoint(10), new FixedPoint(0));
            Vector2 velocity = Vector2.Zero;
            FixedPoint smoothTime = new FixedPoint(1);
            FixedPoint deltaTime = new FixedPoint(0.02);

            Vector2 result = Vector2.SmoothDamp(current, target, ref velocity, smoothTime, deltaTime);
            Assert.IsTrue(result.x > FixedPoint.Zero, "Should move towards target");
        }

        #endregion
    }
}
