using NUnit.Framework;

namespace Tao.FixedPoint.UnityTest
{
    /// <summary>
    /// Vector3 测试：构造、运算符、叉积、距离、归一化、投影、反射等
    /// </summary>
    [TestFixture]
    public class Vector3Tests
    {
        #region 构造与静态属性

        [Test]
        public void Constructor_ThreeComponents()
        {
            Vector3 v = new Vector3(new FixedPoint(1), new FixedPoint(2), new FixedPoint(3));
            Assert.AreEqual(new FixedPoint(1).FixedValue, v.x.FixedValue);
            Assert.AreEqual(new FixedPoint(2).FixedValue, v.y.FixedValue);
            Assert.AreEqual(new FixedPoint(3).FixedValue, v.z.FixedValue);
        }

        [Test]
        public void Constructor_TwoComponents_ZIsZero()
        {
            Vector3 v = new Vector3(new FixedPoint(1), new FixedPoint(2));
            Assert.AreEqual(FixedPoint.Zero, v.z);
        }

        [Test]
        public void StaticProperties_CorrectDirections()
        {
            Assert.AreEqual(FixedPoint.One, Vector3.Forward.z);
            Assert.AreEqual(FixedPoint.NegativeOne, Vector3.Back.z);
            Assert.AreEqual(FixedPoint.One, Vector3.Up.y);
            Assert.AreEqual(FixedPoint.NegativeOne, Vector3.Down.y);
            Assert.AreEqual(FixedPoint.NegativeOne, Vector3.Left.x);
            Assert.AreEqual(FixedPoint.One, Vector3.Right.x);
        }

        #endregion

        #region 点积 / 叉积

        [Test]
        public void Dot_ReturnsCorrectValue()
        {
            Vector3 a = new Vector3(new FixedPoint(1), new FixedPoint(2), new FixedPoint(3));
            Vector3 b = new Vector3(new FixedPoint(4), new FixedPoint(5), new FixedPoint(6));
            TestHelper.AssertApprox(Vector3.Dot(a, b), 32.0, 0.1);
        }

        [Test]
        public void Cross_RightUp_ReturnsForward()
        {
            Vector3 result = Vector3.Cross(Vector3.Right, Vector3.Up);
            Assert.AreEqual(FixedPoint.Zero, result.x);
            Assert.AreEqual(FixedPoint.Zero, result.y);
            Assert.AreEqual(FixedPoint.One, result.z);
        }

        [Test]
        public void Cross_AntiCommutative()
        {
            Vector3 a = new Vector3(new FixedPoint(1), new FixedPoint(2), new FixedPoint(3));
            Vector3 b = new Vector3(new FixedPoint(4), new FixedPoint(5), new FixedPoint(6));
            Vector3 ab = Vector3.Cross(a, b);
            Vector3 ba = Vector3.Cross(b, a);
            Assert.AreEqual(-ab.x.FixedValue, ba.x.FixedValue);
            Assert.AreEqual(-ab.y.FixedValue, ba.y.FixedValue);
            Assert.AreEqual(-ab.z.FixedValue, ba.z.FixedValue);
        }

        [Test]
        public void Cross_Parallel_ReturnsZero()
        {
            Vector3 a = new Vector3(new FixedPoint(2), new FixedPoint(0), new FixedPoint(0));
            Vector3 b = new Vector3(new FixedPoint(5), new FixedPoint(0), new FixedPoint(0));
            Vector3 result = Vector3.Cross(a, b);
            Assert.AreEqual(Vector3.Zero, result);
        }

        #endregion

        #region 长度 / 距离 / 归一化

        [Test]
        public void Magnitude_Known_ReturnsCorrect()
        {
            Vector3 v = new Vector3(new FixedPoint(2), new FixedPoint(3), new FixedPoint(6));
            TestHelper.AssertApprox(v.Magnitude, 7.0, 0.01);
        }

        [Test]
        public void Distance_ReturnsCorrectValue()
        {
            Vector3 a = Vector3.Zero;
            Vector3 b = new Vector3(new FixedPoint(2), new FixedPoint(3), new FixedPoint(6));
            TestHelper.AssertApprox(Vector3.Distance(a, b), 7.0, 0.01);
        }

        [Test]
        public void Normalize_UnitLength()
        {
            Vector3 v = new Vector3(new FixedPoint(3), new FixedPoint(4), new FixedPoint(0));
            Vector3 normalized = Vector3.Normalize(v);
            TestHelper.AssertApprox(normalized.Magnitude, 1.0, 0.01);
        }

        [Test]
        public void Normalize_ZeroVector_ReturnsZero()
        {
            Assert.AreEqual(Vector3.Zero, Vector3.Normalize(Vector3.Zero));
        }

        [Test]
        public void Normalized_Property_MatchesStatic()
        {
            Vector3 v = new Vector3(new FixedPoint(3), new FixedPoint(4), new FixedPoint(5));
            Vector3 fromProp = v.Normalized;
            Vector3 fromStatic = Vector3.Normalize(v);
            Assert.AreEqual(fromStatic.x, fromProp.x);
            Assert.AreEqual(fromStatic.y, fromProp.y);
            Assert.AreEqual(fromStatic.z, fromProp.z);
        }

        #endregion

        #region 投影 / 反射

        [Test]
        public void Project_OntoAxis_ReturnsComponent()
        {
            Vector3 v = new Vector3(new FixedPoint(3), new FixedPoint(4), new FixedPoint(5));
            Vector3 proj = Vector3.Project(v, Vector3.Right);
            TestHelper.AssertApprox(proj.x, 3.0, 0.01);
            TestHelper.AssertApprox(proj.y, 0.0, 0.01);
            TestHelper.AssertApprox(proj.z, 0.0, 0.01);
        }

        [Test]
        public void ProjectOnPlane_RemovesNormalComponent()
        {
            Vector3 v = new Vector3(new FixedPoint(3), new FixedPoint(4), new FixedPoint(5));
            Vector3 proj = Vector3.ProjectOnPlane(v, Vector3.Up);
            TestHelper.AssertApprox(proj.x, 3.0, 0.01);
            TestHelper.AssertApprox(proj.y, 0.0, 0.01);
            TestHelper.AssertApprox(proj.z, 5.0, 0.01);
        }

        [Test]
        public void Reflect_DownOnUp_ReflectsUp()
        {
            Vector3 inDir = new Vector3(FixedPoint.One, FixedPoint.NegativeOne, FixedPoint.Zero);
            Vector3 normal = Vector3.Up;
            Vector3 result = Vector3.Reflect(inDir, normal);
            TestHelper.AssertApprox(result.x, 1.0, 0.01);
            TestHelper.AssertApprox(result.y, 1.0, 0.01);
        }

        #endregion

        #region Lerp / MoveTowards / ClampMagnitude

        [Test]
        public void Lerp_Half_ReturnsMidpoint()
        {
            Vector3 a = Vector3.Zero;
            Vector3 b = new Vector3(new FixedPoint(10), new FixedPoint(10), new FixedPoint(10));
            Vector3 result = Vector3.Lerp(a, b, new FixedPoint(0.5));
            TestHelper.AssertApprox(result, 5.0, 5.0, 5.0, 0.01);
        }

        [Test]
        public void MoveTowards_CloseEnough_ReachesTarget()
        {
            Vector3 current = Vector3.Zero;
            Vector3 target = new Vector3(new FixedPoint(1), new FixedPoint(0), new FixedPoint(0));
            Vector3 result = Vector3.MoveTowards(current, target, new FixedPoint(5));
            Assert.AreEqual(target, result);
        }

        [Test]
        public void ClampMagnitude_LongVector_Clamped()
        {
            Vector3 v = new Vector3(new FixedPoint(30), new FixedPoint(40), new FixedPoint(0));
            Vector3 clamped = Vector3.ClampMagnitude(v, new FixedPoint(5));
            TestHelper.AssertApprox(clamped.Magnitude, 5.0, 0.02);
        }

        #endregion

        #region Angle / SignedAngle

        [Test]
        public void Angle_Orthogonal_Returns90()
        {
            TestHelper.AssertApprox(Vector3.Angle(Vector3.Right, Vector3.Up), 90.0, 1.0);
        }

        [Test]
        public void Angle_SameDirection_ReturnsZero()
        {
            TestHelper.AssertApprox(Vector3.Angle(Vector3.Right, Vector3.Right), 0.0, 0.5);
        }

        [Test]
        public void Angle_Opposite_Returns180()
        {
            TestHelper.AssertApprox(Vector3.Angle(Vector3.Right, Vector3.Left), 180.0, 1.0);
        }

        #endregion

        #region 运算符 / 索引器 / Equals

        [Test]
        public void Operator_ScalarMul_Commutative()
        {
            Vector3 v = new Vector3(new FixedPoint(1), new FixedPoint(2), new FixedPoint(3));
            FixedPoint s = new FixedPoint(3);
            Assert.AreEqual(v * s, s * v);
        }

        [Test]
        public void Indexer_GetSet()
        {
            Vector3 v = new Vector3(new FixedPoint(1), new FixedPoint(2), new FixedPoint(3));
            Assert.AreEqual(new FixedPoint(1).FixedValue, v[0].FixedValue);
            Assert.AreEqual(new FixedPoint(2).FixedValue, v[1].FixedValue);
            Assert.AreEqual(new FixedPoint(3).FixedValue, v[2].FixedValue);
        }

        [Test]
        public void Equals_HashCode_Consistent()
        {
            Vector3 a = new Vector3(new FixedPoint(1), new FixedPoint(2), new FixedPoint(3));
            Vector3 b = new Vector3(new FixedPoint(1), new FixedPoint(2), new FixedPoint(3));
            Assert.IsTrue(a.Equals(b));
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        #endregion

        #region Exclude / AngleBetween

        [Test]
        public void Exclude_EqualsProjectOnPlane()
        {
            Vector3 v = new Vector3(new FixedPoint(3), new FixedPoint(4), new FixedPoint(5));
            Vector3 excluded = Vector3.Exclude(Vector3.Up, v);
            Vector3 projected = Vector3.ProjectOnPlane(v, Vector3.Up);
            Assert.AreEqual(projected, excluded);
        }

        [Test]
        public void AngleBetween_Orthogonal_ReturnsPiOver2()
        {
            FixedPoint radians = Vector3.AngleBetween(Vector3.Right, Vector3.Up);
            TestHelper.AssertApprox(radians, System.Math.PI / 2, 0.02);
        }

        #endregion

        #region 补充覆盖

        [Test]
        public void Set_UpdatesComponents()
        {
            Vector3 v = Vector3.Zero;
            v.Set(new FixedPoint(3), new FixedPoint(4), new FixedPoint(5));
            Assert.AreEqual(new FixedPoint(3).FixedValue, v.x.FixedValue);
            Assert.AreEqual(new FixedPoint(4).FixedValue, v.y.FixedValue);
            Assert.AreEqual(new FixedPoint(5).FixedValue, v.z.FixedValue);
        }

        [Test]
        public void Scale_Instance_MultipliesComponents()
        {
            Vector3 v = new Vector3(new FixedPoint(2), new FixedPoint(3), new FixedPoint(4));
            v.Scale(new Vector3(new FixedPoint(5), new FixedPoint(6), new FixedPoint(7)));
            Assert.AreEqual(new FixedPoint(10).FixedValue, v.x.FixedValue);
            Assert.AreEqual(new FixedPoint(18).FixedValue, v.y.FixedValue);
            Assert.AreEqual(new FixedPoint(28).FixedValue, v.z.FixedValue);
        }

        [Test]
        public void Scale_Static_MultipliesComponents()
        {
            Vector3 a = new Vector3(new FixedPoint(2), new FixedPoint(3), new FixedPoint(4));
            Vector3 b = new Vector3(new FixedPoint(5), new FixedPoint(6), new FixedPoint(7));
            Vector3 result = Vector3.Scale(a, b);
            Assert.AreEqual(new FixedPoint(10).FixedValue, result.x.FixedValue);
            Assert.AreEqual(new FixedPoint(18).FixedValue, result.y.FixedValue);
            Assert.AreEqual(new FixedPoint(28).FixedValue, result.z.FixedValue);
        }

        [Test]
        public void SignedAngle_RightToForward_AroundUp()
        {
            FixedPoint angle = Vector3.SignedAngle(Vector3.Right, Vector3.Forward, Vector3.Up);
            TestHelper.AssertApprox(angle, -90.0, 0.01);
        }

        [Test]
        public void LerpUnclamped_T2_Extrapolates()
        {
            Vector3 a = Vector3.Zero;
            Vector3 b = new Vector3(new FixedPoint(10), new FixedPoint(0), new FixedPoint(0));
            Vector3 result = Vector3.LerpUnclamped(a, b, new FixedPoint(2));
            TestHelper.AssertApprox(result.x, 20.0, 0.01);
        }

        [Test]
        public void Min_ReturnsComponentMin()
        {
            Vector3 a = new Vector3(new FixedPoint(1), new FixedPoint(5), new FixedPoint(3));
            Vector3 b = new Vector3(new FixedPoint(3), new FixedPoint(2), new FixedPoint(7));
            Vector3 result = Vector3.Min(a, b);
            Assert.AreEqual(new FixedPoint(1).FixedValue, result.x.FixedValue);
            Assert.AreEqual(new FixedPoint(2).FixedValue, result.y.FixedValue);
            Assert.AreEqual(new FixedPoint(3).FixedValue, result.z.FixedValue);
        }

        [Test]
        public void Max_ReturnsComponentMax()
        {
            Vector3 a = new Vector3(new FixedPoint(1), new FixedPoint(5), new FixedPoint(3));
            Vector3 b = new Vector3(new FixedPoint(3), new FixedPoint(2), new FixedPoint(7));
            Vector3 result = Vector3.Max(a, b);
            Assert.AreEqual(new FixedPoint(3).FixedValue, result.x.FixedValue);
            Assert.AreEqual(new FixedPoint(5).FixedValue, result.y.FixedValue);
            Assert.AreEqual(new FixedPoint(7).FixedValue, result.z.FixedValue);
        }

        [Test]
        public void SmoothDamp_MovesTowardsTarget()
        {
            Vector3 current = Vector3.Zero;
            Vector3 target = new Vector3(new FixedPoint(10), new FixedPoint(0), new FixedPoint(0));
            Vector3 velocity = Vector3.Zero;
            FixedPoint smoothTime = new FixedPoint(1);
            FixedPoint deltaTime = new FixedPoint(0.02);

            Vector3 result = Vector3.SmoothDamp(current, target, ref velocity, smoothTime, deltaTime);
            Assert.IsTrue(result.x > FixedPoint.Zero, "Should move towards target");
        }

        [Test]
        public void ToString_ReturnsNonEmpty()
        {
            Vector3 v = new Vector3(new FixedPoint(1), new FixedPoint(2), new FixedPoint(3));
            Assert.IsFalse(string.IsNullOrEmpty(v.ToString()));
        }

        #endregion
    }
}
