namespace Tao.FixedPoint.DotNetTest
{
    /// <summary>
    /// Vector3 测试：构造、运算符、叉积、距离、归一化、投影、反射等
    /// </summary>
    [TestClass]
    public sealed class Vector3Tests
    {
        #region 构造与静态属性

        /// <summary>
        /// 三分量构造函数
        /// </summary>
        [TestMethod]
        public void Constructor_ThreeComponents()
        {
            Vector3 v = new Vector3(new FixedPoint(1), new FixedPoint(2), new FixedPoint(3));
            Assert.AreEqual(new FixedPoint(1).FixedValue, v.x.FixedValue);
            Assert.AreEqual(new FixedPoint(2).FixedValue, v.y.FixedValue);
            Assert.AreEqual(new FixedPoint(3).FixedValue, v.z.FixedValue);
        }

        /// <summary>
        /// 二分量构造函数 (z=0)
        /// </summary>
        [TestMethod]
        public void Constructor_TwoComponents_ZIsZero()
        {
            Vector3 v = new Vector3(new FixedPoint(1), new FixedPoint(2));
            Assert.AreEqual(FixedPoint.Zero, v.z);
        }

        /// <summary>
        /// 静态方向属性
        /// </summary>
        [TestMethod]
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

        /// <summary>
        /// 点积计算
        /// </summary>
        [TestMethod]
        public void Dot_ReturnsCorrectValue()
        {
            Vector3 a = new Vector3(new FixedPoint(1), new FixedPoint(2), new FixedPoint(3));
            Vector3 b = new Vector3(new FixedPoint(4), new FixedPoint(5), new FixedPoint(6));
            TestHelper.AssertApprox(Vector3.Dot(a, b), 32.0, 0.1);
        }

        /// <summary>
        /// 正交向量叉积: Right × Up = Forward
        /// </summary>
        [TestMethod]
        public void Cross_RightUp_ReturnsForward()
        {
            Vector3 result = Vector3.Cross(Vector3.Right, Vector3.Up);
            Assert.AreEqual(FixedPoint.Zero, result.x);
            Assert.AreEqual(FixedPoint.Zero, result.y);
            Assert.AreEqual(FixedPoint.One, result.z);
        }

        /// <summary>
        /// 叉积反交换: a × b = -(b × a)
        /// </summary>
        [TestMethod]
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

        /// <summary>
        /// 平行向量叉积为零
        /// </summary>
        [TestMethod]
        public void Cross_Parallel_ReturnsZero()
        {
            Vector3 a = new Vector3(new FixedPoint(2), new FixedPoint(0), new FixedPoint(0));
            Vector3 b = new Vector3(new FixedPoint(5), new FixedPoint(0), new FixedPoint(0));
            Vector3 result = Vector3.Cross(a, b);
            Assert.AreEqual(Vector3.Zero, result);
        }

        #endregion

        #region 长度 / 距离 / 归一化

        /// <summary>
        /// 3-4-5 扩展到 3D 的长度
        /// </summary>
        [TestMethod]
        public void Magnitude_Known_ReturnsCorrect()
        {
            Vector3 v = new Vector3(new FixedPoint(2), new FixedPoint(3), new FixedPoint(6));
            TestHelper.AssertApprox(v.Magnitude, 7.0, 0.01);
        }

        /// <summary>
        /// 两点距离
        /// </summary>
        [TestMethod]
        public void Distance_ReturnsCorrectValue()
        {
            Vector3 a = Vector3.Zero;
            Vector3 b = new Vector3(new FixedPoint(2), new FixedPoint(3), new FixedPoint(6));
            TestHelper.AssertApprox(Vector3.Distance(a, b), 7.0, 0.01);
        }

        /// <summary>
        /// 归一化后长度为 1
        /// </summary>
        [TestMethod]
        public void Normalize_UnitLength()
        {
            Vector3 v = new Vector3(new FixedPoint(3), new FixedPoint(4), new FixedPoint(0));
            Vector3 normalized = Vector3.Normalize(v);
            TestHelper.AssertApprox(normalized.Magnitude, 1.0, 0.01);
        }

        /// <summary>
        /// 零向量归一化返回零
        /// </summary>
        [TestMethod]
        public void Normalize_ZeroVector_ReturnsZero()
        {
            Assert.AreEqual(Vector3.Zero, Vector3.Normalize(Vector3.Zero));
        }

        /// <summary>
        /// Normalized 属性与静态方法一致
        /// </summary>
        [TestMethod]
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

        /// <summary>
        /// Project 投影到坐标轴
        /// </summary>
        [TestMethod]
        public void Project_OntoAxis_ReturnsComponent()
        {
            Vector3 v = new Vector3(new FixedPoint(3), new FixedPoint(4), new FixedPoint(5));
            Vector3 proj = Vector3.Project(v, Vector3.Right);
            TestHelper.AssertApprox(proj.x, 3.0, 0.01);
            TestHelper.AssertApprox(proj.y, 0.0, 0.01);
            TestHelper.AssertApprox(proj.z, 0.0, 0.01);
        }

        /// <summary>
        /// ProjectOnPlane 投影到 XZ 平面去除 Y 分量
        /// </summary>
        [TestMethod]
        public void ProjectOnPlane_RemovesNormalComponent()
        {
            Vector3 v = new Vector3(new FixedPoint(3), new FixedPoint(4), new FixedPoint(5));
            Vector3 proj = Vector3.ProjectOnPlane(v, Vector3.Up);
            TestHelper.AssertApprox(proj.x, 3.0, 0.01);
            TestHelper.AssertApprox(proj.y, 0.0, 0.01);
            TestHelper.AssertApprox(proj.z, 5.0, 0.01);
        }

        /// <summary>
        /// 反射: 向下入射 XY 平面
        /// </summary>
        [TestMethod]
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

        /// <summary>
        /// Lerp t=0.5 返回中点
        /// </summary>
        [TestMethod]
        public void Lerp_Half_ReturnsMidpoint()
        {
            Vector3 a = Vector3.Zero;
            Vector3 b = new Vector3(new FixedPoint(10), new FixedPoint(10), new FixedPoint(10));
            Vector3 result = Vector3.Lerp(a, b, new FixedPoint(0.5));
            TestHelper.AssertApprox(result, 5.0, 5.0, 5.0, 0.01);
        }

        /// <summary>
        /// MoveTowards 在目标范围内直接到达
        /// </summary>
        [TestMethod]
        public void MoveTowards_CloseEnough_ReachesTarget()
        {
            Vector3 current = Vector3.Zero;
            Vector3 target = new Vector3(new FixedPoint(1), new FixedPoint(0), new FixedPoint(0));
            Vector3 result = Vector3.MoveTowards(current, target, new FixedPoint(5));
            Assert.AreEqual(target, result);
        }

        /// <summary>
        /// ClampMagnitude 限制长度
        /// </summary>
        [TestMethod]
        public void ClampMagnitude_LongVector_Clamped()
        {
            Vector3 v = new Vector3(new FixedPoint(30), new FixedPoint(40), new FixedPoint(0));
            Vector3 clamped = Vector3.ClampMagnitude(v, new FixedPoint(5));
            TestHelper.AssertApprox(clamped.Magnitude, 5.0, 0.02);
        }

        #endregion

        #region Angle / SignedAngle

        /// <summary>
        /// 正交轴之间角度为 90°
        /// </summary>
        [TestMethod]
        public void Angle_Orthogonal_Returns90()
        {
            TestHelper.AssertApprox(Vector3.Angle(Vector3.Right, Vector3.Up), 90.0, 1.0);
        }

        /// <summary>
        /// 同向角度为 0°
        /// </summary>
        [TestMethod]
        public void Angle_SameDirection_ReturnsZero()
        {
            TestHelper.AssertApprox(Vector3.Angle(Vector3.Right, Vector3.Right), 0.0, 0.5);
        }

        /// <summary>
        /// 反向角度为 180°
        /// </summary>
        [TestMethod]
        public void Angle_Opposite_Returns180()
        {
            TestHelper.AssertApprox(Vector3.Angle(Vector3.Right, Vector3.Left), 180.0, 1.0);
        }

        #endregion

        #region 运算符 / 索引器 / Equals

        /// <summary>
        /// 标量左乘与右乘等价
        /// </summary>
        [TestMethod]
        public void Operator_ScalarMul_Commutative()
        {
            Vector3 v = new Vector3(new FixedPoint(1), new FixedPoint(2), new FixedPoint(3));
            FixedPoint s = new FixedPoint(3);
            Assert.AreEqual(v * s, s * v);
        }

        /// <summary>
        /// 索引器访问
        /// </summary>
        [TestMethod]
        public void Indexer_GetSet()
        {
            Vector3 v = new Vector3(new FixedPoint(1), new FixedPoint(2), new FixedPoint(3));
            Assert.AreEqual(new FixedPoint(1).FixedValue, v[0].FixedValue);
            Assert.AreEqual(new FixedPoint(2).FixedValue, v[1].FixedValue);
            Assert.AreEqual(new FixedPoint(3).FixedValue, v[2].FixedValue);
        }

        /// <summary>
        /// Equals 和 GetHashCode 一致性
        /// </summary>
        [TestMethod]
        public void Equals_HashCode_Consistent()
        {
            Vector3 a = new Vector3(new FixedPoint(1), new FixedPoint(2), new FixedPoint(3));
            Vector3 b = new Vector3(new FixedPoint(1), new FixedPoint(2), new FixedPoint(3));
            Assert.IsTrue(a.Equals(b));
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        #endregion

        #region Exclude / AngleBetween

        /// <summary>
        /// Exclude 等同于 ProjectOnPlane
        /// </summary>
        [TestMethod]
        public void Exclude_EqualsProjectOnPlane()
        {
            Vector3 v = new Vector3(new FixedPoint(3), new FixedPoint(4), new FixedPoint(5));
            Vector3 excluded = Vector3.Exclude(Vector3.Up, v);
            Vector3 projected = Vector3.ProjectOnPlane(v, Vector3.Up);
            Assert.AreEqual(projected, excluded);
        }

        /// <summary>
        /// AngleBetween 返回弧度角
        /// </summary>
        [TestMethod]
        public void AngleBetween_Orthogonal_ReturnsPiOver2()
        {
            FixedPoint radians = Vector3.AngleBetween(Vector3.Right, Vector3.Up);
            TestHelper.AssertApprox(radians, System.Math.PI / 2, 0.02);
        }

        #endregion

        #region 补充覆盖

        /// <summary>
        /// Set 方法设置分量
        /// </summary>
        [TestMethod]
        public void Set_UpdatesComponents()
        {
            Vector3 v = Vector3.Zero;
            v.Set(new FixedPoint(3), new FixedPoint(4), new FixedPoint(5));
            Assert.AreEqual(new FixedPoint(3).FixedValue, v.x.FixedValue);
            Assert.AreEqual(new FixedPoint(4).FixedValue, v.y.FixedValue);
            Assert.AreEqual(new FixedPoint(5).FixedValue, v.z.FixedValue);
        }

        /// <summary>
        /// Scale 实例方法分量相乘
        /// </summary>
        [TestMethod]
        public void Scale_Instance_MultipliesComponents()
        {
            Vector3 v = new Vector3(new FixedPoint(2), new FixedPoint(3), new FixedPoint(4));
            v.Scale(new Vector3(new FixedPoint(5), new FixedPoint(6), new FixedPoint(7)));
            Assert.AreEqual(new FixedPoint(10).FixedValue, v.x.FixedValue);
            Assert.AreEqual(new FixedPoint(18).FixedValue, v.y.FixedValue);
            Assert.AreEqual(new FixedPoint(28).FixedValue, v.z.FixedValue);
        }

        /// <summary>
        /// Scale 静态方法分量相乘
        /// </summary>
        [TestMethod]
        public void Scale_Static_MultipliesComponents()
        {
            Vector3 a = new Vector3(new FixedPoint(2), new FixedPoint(3), new FixedPoint(4));
            Vector3 b = new Vector3(new FixedPoint(5), new FixedPoint(6), new FixedPoint(7));
            Vector3 result = Vector3.Scale(a, b);
            Assert.AreEqual(new FixedPoint(10).FixedValue, result.x.FixedValue);
            Assert.AreEqual(new FixedPoint(18).FixedValue, result.y.FixedValue);
            Assert.AreEqual(new FixedPoint(28).FixedValue, result.z.FixedValue);
        }

        /// <summary>
        /// SignedAngle 绕 Y 轴从 Right 到 Forward 为正
        /// </summary>
        [TestMethod]
        public void SignedAngle_RightToForward_AroundUp()
        {
            FixedPoint angle = Vector3.SignedAngle(Vector3.Right, Vector3.Forward, Vector3.Up);
            TestHelper.AssertApprox(angle, -90.0, 2.0);
        }

        /// <summary>
        /// LerpUnclamped t=2 外推
        /// </summary>
        [TestMethod]
        public void LerpUnclamped_T2_Extrapolates()
        {
            Vector3 a = Vector3.Zero;
            Vector3 b = new Vector3(new FixedPoint(10), new FixedPoint(0), new FixedPoint(0));
            Vector3 result = Vector3.LerpUnclamped(a, b, new FixedPoint(2));
            TestHelper.AssertApprox(result.x, 20.0, 0.01);
        }

        /// <summary>
        /// Min 返回分量最小值
        /// </summary>
        [TestMethod]
        public void Min_ReturnsComponentMin()
        {
            Vector3 a = new Vector3(new FixedPoint(1), new FixedPoint(5), new FixedPoint(3));
            Vector3 b = new Vector3(new FixedPoint(3), new FixedPoint(2), new FixedPoint(7));
            Vector3 result = Vector3.Min(a, b);
            Assert.AreEqual(new FixedPoint(1).FixedValue, result.x.FixedValue);
            Assert.AreEqual(new FixedPoint(2).FixedValue, result.y.FixedValue);
            Assert.AreEqual(new FixedPoint(3).FixedValue, result.z.FixedValue);
        }

        /// <summary>
        /// Max 返回分量最大值
        /// </summary>
        [TestMethod]
        public void Max_ReturnsComponentMax()
        {
            Vector3 a = new Vector3(new FixedPoint(1), new FixedPoint(5), new FixedPoint(3));
            Vector3 b = new Vector3(new FixedPoint(3), new FixedPoint(2), new FixedPoint(7));
            Vector3 result = Vector3.Max(a, b);
            Assert.AreEqual(new FixedPoint(3).FixedValue, result.x.FixedValue);
            Assert.AreEqual(new FixedPoint(5).FixedValue, result.y.FixedValue);
            Assert.AreEqual(new FixedPoint(7).FixedValue, result.z.FixedValue);
        }

        /// <summary>
        /// SmoothDamp 朝目标方向移动
        /// </summary>
        [TestMethod]
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

        /// <summary>
        /// ToString 返回非空字符串
        /// </summary>
        [TestMethod]
        public void ToString_ReturnsNonEmpty()
        {
            Vector3 v = new Vector3(new FixedPoint(1), new FixedPoint(2), new FixedPoint(3));
            Assert.IsFalse(string.IsNullOrEmpty(v.ToString()));
        }

        #endregion
    }
}