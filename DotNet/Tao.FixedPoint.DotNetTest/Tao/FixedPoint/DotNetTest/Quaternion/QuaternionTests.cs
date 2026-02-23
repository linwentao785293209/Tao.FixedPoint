namespace Tao.FixedPoint.DotNetTest
{
    /// <summary>
    /// Quaternion 测试：Identity, Euler, AngleAxis, 乘法, 向量旋转, Slerp, Inverse
    /// </summary>
    [TestClass]
    public sealed class QuaternionTests
    {
        #region Identity

        /// <summary>
        /// Identity 四元数分量为 (0, 0, 0, 1)
        /// </summary>
        [TestMethod]
        public void Identity_Components()
        {
            Quaternion q = Quaternion.Identity;
            Assert.AreEqual(FixedPoint.Zero, q.x);
            Assert.AreEqual(FixedPoint.Zero, q.y);
            Assert.AreEqual(FixedPoint.Zero, q.z);
            Assert.AreEqual(FixedPoint.One, q.w);
        }

        /// <summary>
        /// Identity 不改变向量
        /// </summary>
        [TestMethod]
        public void Identity_DoesNotRotateVector()
        {
            Vector3 v = new Vector3(new FixedPoint(3), new FixedPoint(4), new FixedPoint(5));
            Vector3 rotated = Quaternion.Identity * v;
            TestHelper.AssertApprox(rotated, 3.0, 4.0, 5.0, 0.01);
        }

        #endregion

        #region Euler

        /// <summary>
        /// 零欧拉角产生 Identity
        /// </summary>
        [TestMethod]
        public void Euler_Zero_ReturnsIdentity()
        {
            Quaternion q = Quaternion.Euler(FixedPoint.Zero, FixedPoint.Zero, FixedPoint.Zero);
            Assert.IsTrue(
                Math.Approximately(q.x, FixedPoint.Zero) &&
                Math.Approximately(q.y, FixedPoint.Zero) &&
                Math.Approximately(q.z, FixedPoint.Zero) &&
                Math.Approximately(q.w, FixedPoint.One));
        }

        /// <summary>
        /// 绕 Y 轴旋转 90° 后 Right 变为 -Forward (Hamilton 四元数约定)
        /// </summary>
        [TestMethod]
        public void Euler_Y90_RotatesRightToNegForward()
        {
            Quaternion q = Quaternion.Euler(FixedPoint.Zero, new FixedPoint(90), FixedPoint.Zero);
            Vector3 result = q * Vector3.Right;
            TestHelper.AssertApprox(result, 0.0, 0.0, -1.0, 0.05);
        }

        /// <summary>
        /// Euler(Vector3) 与 Euler(x, y, z) 一致
        /// </summary>
        [TestMethod]
        public void Euler_Vector_MatchesScalar()
        {
            Vector3 angles = new Vector3(new FixedPoint(30), new FixedPoint(45), new FixedPoint(60));
            Quaternion fromVec = Quaternion.Euler(angles);
            Quaternion fromScalar = Quaternion.Euler(new FixedPoint(30), new FixedPoint(45), new FixedPoint(60));
            Assert.AreEqual(fromVec, fromScalar);
        }

        /// <summary>
        /// EulerAngles 属性往返一致 (简单轴向旋转)
        /// </summary>
        [TestMethod]
        public void EulerAngles_RoundTrip_SimpleAxis()
        {
            Quaternion q = Quaternion.Euler(FixedPoint.Zero, new FixedPoint(90), FixedPoint.Zero);
            Vector3 euler = q.EulerAngles;
            TestHelper.AssertApprox(euler.y, 90.0, 1.5);
        }

        #endregion

        #region AngleAxis

        /// <summary>
        /// AngleAxis 绕 Y 轴 180° 翻转 X
        /// </summary>
        [TestMethod]
        public void AngleAxis_Y180_FlipsX()
        {
            Quaternion q = Quaternion.AngleAxis(new FixedPoint(180), Vector3.Up);
            Vector3 result = q * Vector3.Right;
            TestHelper.AssertApprox(result, -1.0, 0.0, 0.0, 0.05);
        }

        /// <summary>
        /// AngleAxis 角度 0 等同于 Identity
        /// </summary>
        [TestMethod]
        public void AngleAxis_Zero_IsIdentity()
        {
            Quaternion q = Quaternion.AngleAxis(FixedPoint.Zero, Vector3.Up);
            Vector3 result = q * Vector3.Forward;
            TestHelper.AssertApprox(result, 0.0, 0.0, 1.0, 0.01);
        }

        #endregion

        #region 乘法

        /// <summary>
        /// 四元数乘以自身的逆 = Identity
        /// </summary>
        [TestMethod]
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

        /// <summary>
        /// Identity 乘以任何四元数 = 该四元数
        /// </summary>
        [TestMethod]
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

        /// <summary>
        /// 同一四元数的 Dot = 1
        /// </summary>
        [TestMethod]
        public void Dot_Same_ReturnsOne()
        {
            Quaternion q = Quaternion.Identity;
            TestHelper.AssertApprox(Quaternion.Dot(q, q), 1.0, 0.01);
        }

        /// <summary>
        /// Identity 和 Identity 夹角为 0
        /// </summary>
        [TestMethod]
        public void Angle_Same_ReturnsZero()
        {
            TestHelper.AssertApprox(Quaternion.Angle(Quaternion.Identity, Quaternion.Identity), 0.0, 0.5);
        }

        /// <summary>
        /// 90° 旋转的夹角 = 90°
        /// </summary>
        [TestMethod]
        public void Angle_90Deg_Returns90()
        {
            Quaternion a = Quaternion.Identity;
            Quaternion b = Quaternion.Euler(FixedPoint.Zero, new FixedPoint(90), FixedPoint.Zero);
            TestHelper.AssertApprox(Quaternion.Angle(a, b), 90.0, 2.0);
        }

        /// <summary>
        /// Inverse 翻转虚部
        /// </summary>
        [TestMethod]
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

        /// <summary>
        /// Slerp t=0 返回起始旋转
        /// </summary>
        [TestMethod]
        public void Slerp_T0_ReturnsA()
        {
            Quaternion a = Quaternion.Identity;
            Quaternion b = Quaternion.Euler(FixedPoint.Zero, new FixedPoint(90), FixedPoint.Zero);
            Quaternion result = Quaternion.Slerp(a, b, FixedPoint.Zero);
            Assert.AreEqual(a, result);
        }

        /// <summary>
        /// Slerp t=1 返回目标旋转
        /// </summary>
        [TestMethod]
        public void Slerp_T1_ReturnsB()
        {
            Quaternion a = Quaternion.Identity;
            Quaternion b = Quaternion.Euler(FixedPoint.Zero, new FixedPoint(90), FixedPoint.Zero);
            Quaternion result = Quaternion.Slerp(a, b, FixedPoint.One);

            // 比较旋转效果而非精确分量值
            Vector3 rotatedByB = b * Vector3.Forward;
            Vector3 rotatedByResult = result * Vector3.Forward;
            TestHelper.AssertApprox(rotatedByResult, rotatedByB.x.RawDouble, rotatedByB.y.RawDouble,
                rotatedByB.z.RawDouble, 0.02);
        }

        /// <summary>
        /// Lerp t=0.5 在两个旋转之间
        /// </summary>
        [TestMethod]
        public void Lerp_Half_InBetween()
        {
            Quaternion a = Quaternion.Identity;
            Quaternion b = Quaternion.Euler(FixedPoint.Zero, new FixedPoint(90), FixedPoint.Zero);
            Quaternion result = Quaternion.Lerp(a, b, new FixedPoint(0.5));

            // 半程旋转应使 Forward 方向在 Right-Forward 之间
            Vector3 rotated = result * Vector3.Forward;
            Assert.IsTrue(rotated.x > FixedPoint.Zero, "Should have positive X component");
            Assert.IsTrue(rotated.z > FixedPoint.Zero, "Should have positive Z component");
        }

        #endregion

        #region RotateTowards / FromToRotation / LookRotation

        /// <summary>
        /// RotateTowards delta 足够大时到达目标
        /// </summary>
        [TestMethod]
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

        /// <summary>
        /// FromToRotation 将 from 旋转到 to
        /// </summary>
        [TestMethod]
        public void FromToRotation_RotatesFromToTo()
        {
            Quaternion q = Quaternion.FromToRotation(Vector3.Right, Vector3.Forward);
            Vector3 result = q * Vector3.Right;
            TestHelper.AssertApprox(result, 0.0, 0.0, 1.0, 0.05);
        }

        /// <summary>
        /// LookRotation Forward 朝向指定方向
        /// </summary>
        [TestMethod]
        public void LookRotation_ForwardDirection()
        {
            Quaternion q = Quaternion.LookRotation(Vector3.Forward);
            Vector3 result = q * Vector3.Forward;
            TestHelper.AssertApprox(result, 0.0, 0.0, 1.0, 0.05);
        }

        #endregion

        #region Equals / GetHashCode / 索引器

        /// <summary>
        /// Equals 和 == 一致
        /// </summary>
        [TestMethod]
        public void Equals_Consistency()
        {
            Quaternion a = Quaternion.Identity;
            Quaternion b = Quaternion.Identity;
            Assert.IsTrue(a == b);
            Assert.IsTrue(a.Equals(b));
            Assert.IsTrue(a.Equals((object)b));
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        /// <summary>
        /// 索引器读写
        /// </summary>
        [TestMethod]
        public void Indexer_GetSet()
        {
            Quaternion q = Quaternion.Identity;
            Assert.AreEqual(FixedPoint.Zero, q[0]);
            Assert.AreEqual(FixedPoint.One, q[3]);
        }

        /// <summary>
        /// 索引器越界抛出异常
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(IndexOutOfRangeException))]
        public void Indexer_OutOfRange_Throws()
        {
            Quaternion q = Quaternion.Identity;
            FixedPoint _ = q[4];
        }

        #endregion
    }
}