using System;
using NUnit.Framework;

namespace Tao.FixedPoint.UnityTest
{
    /// <summary>
    /// Matrix4x4 测试：Identity, TRS, 乘法, 转置, 行列式, 逆矩阵, 向量变换
    /// </summary>
    [TestFixture]
    public class Matrix4x4Tests
    {
        #region Identity / Zero

        [Test]
        public void Identity_DiagonalOnes()
        {
            Matrix4x4 id = Matrix4x4.Identity;
            Assert.AreEqual(FixedPoint.One, id.m00);
            Assert.AreEqual(FixedPoint.One, id.m11);
            Assert.AreEqual(FixedPoint.One, id.m22);
            Assert.AreEqual(FixedPoint.One, id.m33);
            Assert.AreEqual(FixedPoint.Zero, id.m01);
            Assert.AreEqual(FixedPoint.Zero, id.m10);
        }

        [Test]
        public void Zero_AllZeros()
        {
            Matrix4x4 zero = Matrix4x4.Zero;
            for (int i = 0; i < 16; i++)
            {
                Assert.AreEqual(FixedPoint.Zero, zero[i]);
            }
        }

        #endregion

        #region 乘法

        [Test]
        public void Multiply_Identity_ReturnsSelf()
        {
            Matrix4x4 m = Matrix4x4.Translate(new Vector3(new FixedPoint(5), new FixedPoint(10), new FixedPoint(15)));
            Matrix4x4 result = Matrix4x4.Identity * m;
            Assert.AreEqual(m, result);
        }

        [Test]
        public void Multiply_ByIdentity_ReturnsSelf()
        {
            Matrix4x4 m = Matrix4x4.Scale(new Vector3(new FixedPoint(2), new FixedPoint(3), new FixedPoint(4)));
            Matrix4x4 result = m * Matrix4x4.Identity;
            Assert.AreEqual(m, result);
        }

        #endregion

        #region Translate / Scale / Rotate

        [Test]
        public void Translate_TranslatesPoint()
        {
            Matrix4x4 m = Matrix4x4.Translate(new Vector3(new FixedPoint(5), new FixedPoint(0), new FixedPoint(0)));
            Vector3 result = m.MultiplyPoint(Vector3.Zero);
            TestHelper.AssertApprox(result, 5.0, 0.0, 0.0, 0.01);
        }

        [Test]
        public void Scale_ScalesPoint()
        {
            Matrix4x4 m = Matrix4x4.Scale(new Vector3(new FixedPoint(2), new FixedPoint(3), new FixedPoint(4)));
            Vector3 point = new Vector3(new FixedPoint(1), new FixedPoint(1), new FixedPoint(1));
            Vector3 result = m.MultiplyPoint(point);
            TestHelper.AssertApprox(result, 2.0, 3.0, 4.0, 0.01);
        }

        [Test]
        public void Rotate_RotatesVector()
        {
            Matrix4x4 m = Matrix4x4.Rotate(Quaternion.Euler(FixedPoint.Zero, new FixedPoint(90), FixedPoint.Zero));
            Vector3 result = m.MultiplyVector(Vector3.Right);
            TestHelper.AssertApprox(result, 0.0, 0.0, -1.0, 0.05);
        }

        #endregion

        #region TRS

        [Test]
        public void TRS_TranslateOnly()
        {
            Vector3 pos = new Vector3(new FixedPoint(5), new FixedPoint(10), new FixedPoint(15));
            Matrix4x4 trs = Matrix4x4.TRS(pos, Quaternion.Identity, Vector3.One);
            Vector3 result = trs.MultiplyPoint(Vector3.Zero);
            TestHelper.AssertApprox(result, 5.0, 10.0, 15.0, 0.01);
        }

        [Test]
        public void TRS_WithScale()
        {
            Vector3 scale = new Vector3(new FixedPoint(2), new FixedPoint(2), new FixedPoint(2));
            Matrix4x4 trs = Matrix4x4.TRS(Vector3.Zero, Quaternion.Identity, scale);
            Vector3 point = new Vector3(new FixedPoint(1), new FixedPoint(1), new FixedPoint(1));
            Vector3 result = trs.MultiplyPoint(point);
            TestHelper.AssertApprox(result, 2.0, 2.0, 2.0, 0.01);
        }

        #endregion

        #region Transpose / Determinant / Inverse

        [Test]
        public void Transpose_Identity_ReturnsSelf()
        {
            Assert.AreEqual(Matrix4x4.Identity, Matrix4x4.Identity.Transpose);
        }

        [Test]
        public void Transpose_Twice_ReturnsSelf()
        {
            Matrix4x4 m = Matrix4x4.Translate(new Vector3(new FixedPoint(1), new FixedPoint(2), new FixedPoint(3)));
            Assert.AreEqual(m, m.Transpose.Transpose);
        }

        [Test]
        public void Determinant_Identity_ReturnsOne()
        {
            TestHelper.AssertApprox(Matrix4x4.Identity.Determinant, 1.0, 0.001);
        }

        [Test]
        public void Determinant_Scale_ReturnsProduct()
        {
            Matrix4x4 m = Matrix4x4.Scale(new Vector3(new FixedPoint(2), new FixedPoint(3), new FixedPoint(4)));
            TestHelper.AssertApprox(m.Determinant, 24.0, 0.1);
        }

        [Test]
        public void Inverse_TimesOriginal_IsIdentity()
        {
            Matrix4x4 m = Matrix4x4.TRS(
                new Vector3(new FixedPoint(3), new FixedPoint(4), new FixedPoint(5)),
                Quaternion.Euler(FixedPoint.Zero, new FixedPoint(45), FixedPoint.Zero),
                new Vector3(new FixedPoint(2), new FixedPoint(2), new FixedPoint(2)));
            Matrix4x4 result = m * m.Inverse;

            TestHelper.AssertApprox(result.m00, 1.0, 0.05);
            TestHelper.AssertApprox(result.m11, 1.0, 0.05);
            TestHelper.AssertApprox(result.m22, 1.0, 0.05);
            TestHelper.AssertApprox(result.m33, 1.0, 0.05);
            TestHelper.AssertApprox(result.m01, 0.0, 0.05);
            TestHelper.AssertApprox(result.m10, 0.0, 0.05);
        }

        [Test]
        public void Inverse_SingularMatrix_Throws()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                Matrix4x4 _ = Matrix4x4.Zero.Inverse;
            });
        }

        #endregion

        #region MultiplyPoint / MultiplyPoint3x4 / MultiplyVector

        [Test]
        public void MultiplyPoint3x4_IgnoresFourthRow()
        {
            Matrix4x4 m = Matrix4x4.Translate(new Vector3(new FixedPoint(10), new FixedPoint(20), new FixedPoint(30)));
            Vector3 result = m.MultiplyPoint3x4(Vector3.Zero);
            TestHelper.AssertApprox(result, 10.0, 20.0, 30.0, 0.01);
        }

        [Test]
        public void MultiplyVector_IgnoresTranslation()
        {
            Matrix4x4 m = Matrix4x4.Translate(new Vector3(new FixedPoint(10), new FixedPoint(20), new FixedPoint(30)));
            Vector3 result = m.MultiplyVector(Vector3.Right);
            TestHelper.AssertApprox(result, 1.0, 0.0, 0.0, 0.01);
        }

        [Test]
        public void Operator_MatVec4_Mul()
        {
            Matrix4x4 id = Matrix4x4.Identity;
            Vector4 v = new Vector4(new FixedPoint(1), new FixedPoint(2), new FixedPoint(3), new FixedPoint(4));
            Vector4 result = id * v;
            Assert.AreEqual(v, result);
        }

        #endregion

        #region 索引器 / GetRow / GetColumn / Equals

        [Test]
        public void Indexer_RowCol_MatchesLinear()
        {
            Matrix4x4 m = Matrix4x4.Identity;
            Assert.AreEqual(m[0, 0], m[0]);
            Assert.AreEqual(m[1, 1], m[5]);
            Assert.AreEqual(m[3, 3], m[15]);
        }

        [Test]
        public void GetColumn_GetRow()
        {
            Matrix4x4 m = Matrix4x4.Identity;
            Vector4 col0 = m.GetColumn(0);
            Assert.AreEqual(FixedPoint.One, col0.x);
            Assert.AreEqual(FixedPoint.Zero, col0.y);

            Vector4 row0 = m.GetRow(0);
            Assert.AreEqual(FixedPoint.One, row0.x);
            Assert.AreEqual(FixedPoint.Zero, row0.y);
        }

        [Test]
        public void Equals_HashCode_Consistent()
        {
            Matrix4x4 a = Matrix4x4.Identity;
            Matrix4x4 b = Matrix4x4.Identity;
            Assert.IsTrue(a.Equals(b));
            Assert.IsTrue(a == b);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void ToString_ReturnsNonEmpty()
        {
            Assert.IsFalse(string.IsNullOrEmpty(Matrix4x4.Identity.ToString()));
        }

        #endregion
    }
}
