using System;
using NUnit.Framework;

namespace Tao.FixedPoint.UnityTest
{
    /// <summary>
    /// Vector4 测试：构造、运算符、点积、距离、归一化、类型转换
    /// </summary>
    [TestFixture]
    public class Vector4Tests
    {
        #region 构造与静态属性

        [Test]
        public void Constructor_FourComponents()
        {
            Vector4 v = new Vector4(new FixedPoint(1), new FixedPoint(2), new FixedPoint(3), new FixedPoint(4));
            Assert.AreEqual(new FixedPoint(1).FixedValue, v.x.FixedValue);
            Assert.AreEqual(new FixedPoint(2).FixedValue, v.y.FixedValue);
            Assert.AreEqual(new FixedPoint(3).FixedValue, v.z.FixedValue);
            Assert.AreEqual(new FixedPoint(4).FixedValue, v.w.FixedValue);
        }

        [Test]
        public void Constructor_ThreeComponents_WIsZero()
        {
            Vector4 v = new Vector4(new FixedPoint(1), new FixedPoint(2), new FixedPoint(3));
            Assert.AreEqual(FixedPoint.Zero, v.w);
        }

        [Test]
        public void Constructor_TwoComponents_ZWAreZero()
        {
            Vector4 v = new Vector4(new FixedPoint(1), new FixedPoint(2));
            Assert.AreEqual(FixedPoint.Zero, v.z);
            Assert.AreEqual(FixedPoint.Zero, v.w);
        }

        [Test]
        public void StaticProperties_CorrectValues()
        {
            Assert.AreEqual(FixedPoint.Zero, Vector4.Zero.x);
            Assert.AreEqual(FixedPoint.One, Vector4.One.x);
            Assert.AreEqual(FixedPoint.One, Vector4.One.w);
        }

        #endregion

        #region 运算符

        [Test]
        public void Operator_AddSub()
        {
            Vector4 a = new Vector4(new FixedPoint(1), new FixedPoint(2), new FixedPoint(3), new FixedPoint(4));
            Vector4 b = new Vector4(new FixedPoint(4), new FixedPoint(3), new FixedPoint(2), new FixedPoint(1));
            Vector4 sum = a + b;
            Vector4 diff = a - b;

            Assert.AreEqual(new FixedPoint(5).FixedValue, sum.x.FixedValue);
            Assert.AreEqual(new FixedPoint(-3).FixedValue, diff.x.FixedValue);
        }

        [Test]
        public void Operator_Equality()
        {
            Vector4 a = new Vector4(new FixedPoint(1), new FixedPoint(2), new FixedPoint(3), new FixedPoint(4));
            Vector4 b = new Vector4(new FixedPoint(1), new FixedPoint(2), new FixedPoint(3), new FixedPoint(4));
            Assert.IsTrue(a == b);
            Assert.IsFalse(a != b);
        }

        #endregion

        #region Dot / Distance / Normalize

        [Test]
        public void Dot_ReturnsCorrectValue()
        {
            Vector4 a = new Vector4(new FixedPoint(1), new FixedPoint(0), new FixedPoint(0), new FixedPoint(0));
            Vector4 b = new Vector4(new FixedPoint(5), new FixedPoint(3), new FixedPoint(2), new FixedPoint(1));
            TestHelper.AssertApprox(Vector4.Dot(a, b), 5.0, 0.01);
        }

        [Test]
        public void Normalize_UnitLength()
        {
            Vector4 v = new Vector4(new FixedPoint(3), new FixedPoint(4), new FixedPoint(0), new FixedPoint(0));
            Vector4 n = v.Normalized;
            TestHelper.AssertApprox(n.Magnitude, 1.0, 0.01);
        }

        [Test]
        public void Distance_ReturnsCorrectValue()
        {
            Vector4 a = Vector4.Zero;
            Vector4 b = new Vector4(new FixedPoint(3), new FixedPoint(4), new FixedPoint(0), new FixedPoint(0));
            TestHelper.AssertApprox(Vector4.Distance(a, b), 5.0, 0.01);
        }

        #endregion

        #region 类型转换

        [Test]
        public void ImplicitConversion_FromVector3()
        {
            Vector3 v3 = new Vector3(new FixedPoint(1), new FixedPoint(2), new FixedPoint(3));
            Vector4 v4 = v3;
            Assert.AreEqual(v3.x, v4.x);
            Assert.AreEqual(v3.y, v4.y);
            Assert.AreEqual(v3.z, v4.z);
            Assert.AreEqual(FixedPoint.Zero, v4.w);
        }

        [Test]
        public void ImplicitConversion_ToVector3()
        {
            Vector4 v4 = new Vector4(new FixedPoint(1), new FixedPoint(2), new FixedPoint(3), new FixedPoint(4));
            Vector3 v3 = v4;
            Assert.AreEqual(v4.x, v3.x);
            Assert.AreEqual(v4.y, v3.y);
            Assert.AreEqual(v4.z, v3.z);
        }

        [Test]
        public void ImplicitConversion_ToVector2()
        {
            Vector4 v4 = new Vector4(new FixedPoint(1), new FixedPoint(2), new FixedPoint(3), new FixedPoint(4));
            Vector2 v2 = v4;
            Assert.AreEqual(v4.x, v2.x);
            Assert.AreEqual(v4.y, v2.y);
        }

        [Test]
        public void ImplicitConversion_FromVector2()
        {
            Vector2 v2 = new Vector2(new FixedPoint(1), new FixedPoint(2));
            Vector4 v4 = v2;
            Assert.AreEqual(v2.x, v4.x);
            Assert.AreEqual(v2.y, v4.y);
            Assert.AreEqual(FixedPoint.Zero, v4.z);
            Assert.AreEqual(FixedPoint.Zero, v4.w);
        }

        #endregion

        #region Lerp / ClampMagnitude

        [Test]
        public void Lerp_Half_ReturnsMidpoint()
        {
            Vector4 a = Vector4.Zero;
            Vector4 b = new Vector4(new FixedPoint(10), new FixedPoint(10), new FixedPoint(10), new FixedPoint(10));
            Vector4 result = Vector4.Lerp(a, b, new FixedPoint(0.5));
            TestHelper.AssertApprox(result.x, 5.0, 0.01);
            TestHelper.AssertApprox(result.w, 5.0, 0.01);
        }

        [Test]
        public void ClampMagnitude_LongVector_Clamped()
        {
            Vector4 v = new Vector4(new FixedPoint(30), new FixedPoint(40), new FixedPoint(0), new FixedPoint(0));
            Vector4 clamped = Vector4.ClampMagnitude(v, new FixedPoint(5));
            TestHelper.AssertApprox(clamped.Magnitude, 5.0, 0.02);
        }

        #endregion

        #region 索引器 / Equals

        [Test]
        public void Indexer_GetSet()
        {
            Vector4 v = new Vector4(new FixedPoint(1), new FixedPoint(2), new FixedPoint(3), new FixedPoint(4));
            Assert.AreEqual(new FixedPoint(1).FixedValue, v[0].FixedValue);
            Assert.AreEqual(new FixedPoint(4).FixedValue, v[3].FixedValue);
        }

        [Test]
        public void Indexer_OutOfRange_Throws()
        {
            Assert.Throws<IndexOutOfRangeException>(() =>
            {
                FixedPoint _ = Vector4.Zero[4];
            });
        }

        [Test]
        public void Equals_HashCode_Consistent()
        {
            Vector4 a = Vector4.One;
            Vector4 b = Vector4.One;
            Assert.IsTrue(a.Equals(b));
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        #endregion
    }
}
