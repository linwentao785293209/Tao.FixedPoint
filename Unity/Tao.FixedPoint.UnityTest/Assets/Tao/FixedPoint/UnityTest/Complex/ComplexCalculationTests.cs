using NUnit.Framework;

namespace Tao.FixedPoint.UnityTest
{
    /// <summary>
    /// 复杂计算测试：多步向量链、物理模拟、精度累积、完整变换管线、确定性验证
    /// </summary>
    [TestFixture]
    public sealed class ComplexCalculationTests
    {
        #region 多步向量链

        /// <summary>
        /// normalize → dot → angle 链式计算
        /// </summary>
        [Test]
        public void VectorChain_NormalizeDotAngle()
        {
            Vector3 a = new Vector3(new FixedPoint(3), new FixedPoint(4), new FixedPoint(0));
            Vector3 b = new Vector3(new FixedPoint(0), new FixedPoint(5), new FixedPoint(12));

            Vector3 aN = a.Normalized;
            Vector3 bN = b.Normalized;
            FixedPoint dot = Vector3.Dot(aN, bN);
            FixedPoint angle = Math.Acos(Math.Clamp(dot, FixedPoint.NegativeOne, FixedPoint.One));
            FixedPoint angleDeg = Math.RadiansToDegrees(angle);

            double expectedAngle = System.Math.Acos(
                (0 * 0 + 4 * 5 + 0 * 12) /
                (System.Math.Sqrt(9 + 16) * System.Math.Sqrt(25 + 144))
            ) * (180.0 / System.Math.PI);

            TestHelper.AssertApprox(angleDeg, expectedAngle, 1.5);
        }

        /// <summary>
        /// project → reflect → magnitude 链式计算
        /// </summary>
        [Test]
        public void VectorChain_ProjectReflectMagnitude()
        {
            Vector3 velocity = new Vector3(new FixedPoint(5), new FixedPoint(-3), new FixedPoint(2));
            Vector3 wallNormal = Vector3.Up;

            Vector3 projected = Vector3.ProjectOnPlane(velocity, wallNormal);
            Vector3 reflected = Vector3.Reflect(velocity, wallNormal);

            TestHelper.AssertApprox(projected.y, 0.0, 0.01);
            TestHelper.AssertApprox(reflected.x, 5.0, 0.01);
            TestHelper.AssertApprox(reflected.y, 3.0, 0.01);
            TestHelper.AssertApprox(reflected.z, 2.0, 0.01);
            TestHelper.AssertApprox(reflected.Magnitude, velocity.Magnitude.RawDouble, 0.02);
        }

        /// <summary>
        /// lerp → normalize → dot 链式计算
        /// </summary>
        [Test]
        public void VectorChain_LerpNormalizeDot()
        {
            Vector3 a = Vector3.Right;
            Vector3 b = Vector3.Forward;
            Vector3 mid = Vector3.Lerp(a, b, new FixedPoint(0.5));
            Vector3 midN = mid.Normalized;

            FixedPoint dotA = Vector3.Dot(midN, a.Normalized);
            FixedPoint dotB = Vector3.Dot(midN, b.Normalized);
            long diff = System.Math.Abs(dotA.FixedValue - dotB.FixedValue);
            Assert.IsTrue(diff <= 2, $"Dot with a and b should be equal, diff={diff}");
        }

        /// <summary>
        /// cross → normalize → dot 验证正交性
        /// </summary>
        [Test]
        public void VectorChain_CrossNormalizeDot_Orthogonal()
        {
            Vector3 a = new Vector3(new FixedPoint(1), new FixedPoint(2), new FixedPoint(3));
            Vector3 b = new Vector3(new FixedPoint(4), new FixedPoint(-1), new FixedPoint(2));

            Vector3 cross = Vector3.Cross(a, b);
            Vector3 crossN = cross.Normalized;

            TestHelper.AssertApprox(Vector3.Dot(crossN, a.Normalized), 0.0, 0.02);
            TestHelper.AssertApprox(Vector3.Dot(crossN, b.Normalized), 0.0, 0.02);
        }

        #endregion

        #region 物理模拟

        /// <summary>
        /// 匀速直线运动：position += velocity * dt，100 帧后位置正确
        /// FixedPoint(0.02) 存储为 20/1024 ≈ 0.01953，实际距离 = 100 * 0.01953 * 10 ≈ 19.53
        /// </summary>
        [Test]
        public void Physics_UniformMotion_100Steps()
        {
            Vector3 position = Vector3.Zero;
            Vector3 velocity = new Vector3(new FixedPoint(10), new FixedPoint(0), new FixedPoint(0));
            FixedPoint dt = new FixedPoint(0.02);

            for (int i = 0; i < 100; i++)
            {
                position = position + velocity * dt;
            }

            double expectedX = 100.0 * TestHelper.ToDouble(dt) * 10.0;
            TestHelper.AssertApprox(position, expectedX, 0.0, 0.0, 0.1);
        }

        /// <summary>
        /// 自由落体：velocity += gravity * dt, position += velocity * dt
        /// 欧拉法 + 定点数量化 dt 共同引入累积误差，用宽松公差验证合理性
        /// </summary>
        [Test]
        public void Physics_FreeFall_200Steps()
        {
            Vector3 position = new Vector3(new FixedPoint(0), new FixedPoint(100), new FixedPoint(0));
            Vector3 velocity = Vector3.Zero;
            Vector3 gravity = new Vector3(new FixedPoint(0), new FixedPoint(-10), new FixedPoint(0));
            FixedPoint dt = new FixedPoint(0.02);

            for (int i = 0; i < 200; i++)
            {
                velocity = velocity + gravity * dt;
                position = position + velocity * dt;
            }

            double dtActual = TestHelper.ToDouble(dt);
            double tTotal = 200 * dtActual;
            double yAnalytic = 100.0 - 0.5 * 10.0 * tTotal * tTotal;
            TestHelper.AssertApprox(position.y, yAnalytic, 5.0);
            TestHelper.AssertApprox(position.x, 0.0, 0.01);
        }

        /// <summary>
        /// 抛体运动：含水平速度的自由落体
        /// </summary>
        [Test]
        public void Physics_Projectile_300Steps()
        {
            Vector3 position = Vector3.Zero;
            Vector3 velocity = new Vector3(new FixedPoint(20), new FixedPoint(30), new FixedPoint(0));
            Vector3 gravity = new Vector3(new FixedPoint(0), new FixedPoint(-10), new FixedPoint(0));
            FixedPoint dt = new FixedPoint(0.02);

            for (int i = 0; i < 300; i++)
            {
                velocity = velocity + gravity * dt;
                position = position + velocity * dt;
            }

            double dtActual = TestHelper.ToDouble(dt);
            double tTotal = 300 * dtActual;
            double expectedX = 20.0 * tTotal;
            TestHelper.AssertApprox(position.x, expectedX, 5.0);
            TestHelper.AssertApprox(position.y, 0.0, 10.0);
        }

        /// <summary>
        /// 圆周运动：验证 cos²θ + sin²θ ≈ 1（半径守恒）
        /// </summary>
        [Test]
        public void Physics_CircularMotion_RadiusPreserved()
        {
            FixedPoint radius = new FixedPoint(10);
            FixedPoint maxDeviation = FixedPoint.Zero;

            for (int i = 0; i < 360; i++)
            {
                FixedPoint angle = new FixedPoint(i);
                FixedPoint rad = Math.DegreesToRadians(angle);
                FixedPoint x = radius * Math.Cos(rad);
                FixedPoint z = radius * Math.Sin(rad);

                FixedPoint dist = Math.Sqrt(x * x + z * z);
                FixedPoint deviation = Math.Abs(dist - radius);
                if (deviation > maxDeviation)
                    maxDeviation = deviation;
            }

            TestHelper.AssertApprox(maxDeviation, 0.0, 0.5);
        }

        #endregion

        #region 精度累积

        /// <summary>
        /// 反复归一化 1000 次，长度仍为 1
        /// </summary>
        [Test]
        public void Precision_RepeatedNormalize_1000()
        {
            Vector3 v = new Vector3(new FixedPoint(3), new FixedPoint(4), new FixedPoint(5));
            for (int i = 0; i < 1000; i++)
            {
                v = v.Normalized;
            }
            TestHelper.AssertApprox(v.Magnitude, 1.0, 0.01);
        }

        /// <summary>
        /// 反复加减 10000 次，结果应回到原值
        /// </summary>
        [Test]
        public void Precision_AddSubRoundTrip_10000()
        {
            FixedPoint original = new FixedPoint(100);
            FixedPoint delta = new FixedPoint(0.1);
            FixedPoint value = original;

            for (int i = 0; i < 10000; i++)
            {
                value = value + delta;
            }
            for (int i = 0; i < 10000; i++)
            {
                value = value - delta;
            }

            TestHelper.AssertApprox(value, 100.0, 0.01);
        }

        /// <summary>
        /// 反复乘除 1000 次，结果不漂移
        /// </summary>
        [Test]
        public void Precision_MulDivRoundTrip_1000()
        {
            FixedPoint value = new FixedPoint(7);
            FixedPoint factor = new FixedPoint(1.5);

            for (int i = 0; i < 1000; i++)
            {
                value = value * factor;
                value = value / factor;
            }

            TestHelper.AssertApprox(value, 7.0, 0.1);
        }

        /// <summary>
        /// sin/cos 累积旋转 100 步后精度验证
        /// </summary>
        [Test]
        public void Precision_SinCosAccumulation_100()
        {
            FixedPoint x = FixedPoint.One;
            FixedPoint y = FixedPoint.Zero;
            FixedPoint angle = Math.DegreesToRadians(new FixedPoint(1));

            FixedPoint sin = Math.Sin(angle);
            FixedPoint cos = Math.Cos(angle);

            for (int i = 0; i < 100; i++)
            {
                FixedPoint newX = x * cos - y * sin;
                FixedPoint newY = x * sin + y * cos;
                x = newX;
                y = newY;
            }

            FixedPoint length = Math.Sqrt(x * x + y * y);
            TestHelper.AssertApprox(length, 1.0, 0.05);
        }

        /// <summary>
        /// Lerp 多步逼近不超越目标
        /// </summary>
        [Test]
        public void Precision_LerpConvergence_500()
        {
            Vector3 current = Vector3.Zero;
            Vector3 target = new Vector3(new FixedPoint(100), new FixedPoint(50), new FixedPoint(25));
            FixedPoint t = new FixedPoint(0.1);

            for (int i = 0; i < 500; i++)
            {
                current = Vector3.Lerp(current, target, t);
            }

            TestHelper.AssertApprox(current, 100.0, 50.0, 25.0, 0.1);
        }

        #endregion

        #region 完整变换管线

        /// <summary>
        /// TRS 变换 → 逆变换 → 还原原始坐标
        /// </summary>
        [Test]
        public void Transform_TRS_InverseRoundTrip()
        {
            Vector3 pos = new Vector3(new FixedPoint(10), new FixedPoint(20), new FixedPoint(30));
            Quaternion rot = Quaternion.Euler(new FixedPoint(30), new FixedPoint(45), new FixedPoint(60));
            Vector3 scale = new Vector3(new FixedPoint(2), new FixedPoint(3), new FixedPoint(1));

            Matrix4x4 trs = Matrix4x4.TRS(pos, rot, scale);
            Matrix4x4 inv = trs.Inverse;

            Vector3 original = new Vector3(new FixedPoint(5), new FixedPoint(7), new FixedPoint(3));
            Vector3 transformed = trs.MultiplyPoint(original);
            Vector3 restored = inv.MultiplyPoint(transformed);

            TestHelper.AssertApprox(restored, 5.0, 7.0, 3.0, 0.1);
        }

        /// <summary>
        /// Quaternion 旋转 Vector3 后用逆四元数还原
        /// </summary>
        [Test]
        public void Transform_QuaternionRotateRestore()
        {
            Vector3 original = new Vector3(new FixedPoint(3), new FixedPoint(4), new FixedPoint(5));
            Quaternion q = Quaternion.Euler(new FixedPoint(45), new FixedPoint(90), new FixedPoint(30));

            Vector3 rotated = q * original;
            Vector3 restored = Quaternion.Inverse(q) * rotated;

            TestHelper.AssertApprox(restored, 3.0, 4.0, 5.0, 0.1);
        }

        /// <summary>
        /// 多级父子变换：Parent * Child * point，然后 Child⁻¹ * Parent⁻¹ 还原
        /// </summary>
        [Test]
        public void Transform_ParentChild_Hierarchy()
        {
            Matrix4x4 parent = Matrix4x4.TRS(
                new Vector3(new FixedPoint(10), new FixedPoint(0), new FixedPoint(0)),
                Quaternion.Euler(FixedPoint.Zero, new FixedPoint(90), FixedPoint.Zero),
                Vector3.One);

            Matrix4x4 child = Matrix4x4.TRS(
                new Vector3(new FixedPoint(0), new FixedPoint(5), new FixedPoint(0)),
                Quaternion.Identity,
                new Vector3(new FixedPoint(2), new FixedPoint(2), new FixedPoint(2)));

            Vector3 localPoint = new Vector3(new FixedPoint(1), new FixedPoint(0), new FixedPoint(0));

            Matrix4x4 worldMatrix = parent * child;
            Vector3 worldPoint = worldMatrix.MultiplyPoint(localPoint);

            Matrix4x4 invWorld = worldMatrix.Inverse;
            Vector3 backToLocal = invWorld.MultiplyPoint(worldPoint);

            TestHelper.AssertApprox(backToLocal, 1.0, 0.0, 0.0, 0.15);
        }

        /// <summary>
        /// LookRotation → 旋转 Forward → 应与目标方向一致
        /// </summary>
        [Test]
        public void Transform_LookRotation_MatchesDirection()
        {
            Vector3 direction = new Vector3(new FixedPoint(3), new FixedPoint(0), new FixedPoint(4));
            Vector3 dirN = direction.Normalized;

            Quaternion look = Quaternion.LookRotation(direction);
            Vector3 result = look * Vector3.Forward;
            Vector3 resultN = result.Normalized;

            TestHelper.AssertApprox(resultN.x, dirN.x.RawDouble, 0.05);
            TestHelper.AssertApprox(resultN.y, dirN.y.RawDouble, 0.05);
            TestHelper.AssertApprox(resultN.z, dirN.z.RawDouble, 0.05);
        }

        /// <summary>
        /// 连续 Euler → 四元数 → 矩阵旋转 → 向量变换管线
        /// </summary>
        [Test]
        public void Transform_EulerToMatrixToVector_Pipeline()
        {
            Quaternion q = Quaternion.Euler(FixedPoint.Zero, new FixedPoint(90), FixedPoint.Zero);
            Matrix4x4 m = Matrix4x4.Rotate(q);

            Vector3 right = Vector3.Right;
            Vector3 byQuat = q * right;
            Vector3 byMatrix = m.MultiplyVector(right);

            TestHelper.AssertApprox(byQuat.x, byMatrix.x.RawDouble, 0.02);
            TestHelper.AssertApprox(byQuat.y, byMatrix.y.RawDouble, 0.02);
            TestHelper.AssertApprox(byQuat.z, byMatrix.z.RawDouble, 0.02);
        }

        #endregion

        #region 确定性验证

        /// <summary>
        /// 同一复杂计算执行两次，结果逐位相同（核心确定性保证）
        /// </summary>
        [Test]
        public void Determinism_SameComputation_BitExact()
        {
            long result1 = RunComplexSimulation(42);
            long result2 = RunComplexSimulation(42);
            Assert.AreEqual(result1, result2, "Same computation must produce bit-exact results");
        }

        /// <summary>
        /// 不同种子产生不同轨迹
        /// </summary>
        [Test]
        public void Determinism_DifferentSeed_DifferentResult()
        {
            long result1 = RunComplexSimulation(42);
            long result2 = RunComplexSimulation(99);
            Assert.AreNotEqual(result1, result2, "Different seeds should produce different trajectories");
        }

        /// <summary>
        /// 100 次独立执行，结果全部一致
        /// </summary>
        [Test]
        public void Determinism_100Runs_AllIdentical()
        {
            long expected = RunComplexSimulation(12345);
            for (int i = 0; i < 100; i++)
            {
                long actual = RunComplexSimulation(12345);
                Assert.AreEqual(expected, actual, $"Run {i} diverged");
            }
        }

        /// <summary>
        /// 综合模拟：随机初始条件 + 物理 + 旋转 + 归一化，返回最终位置的 FixedValue 用于比较
        /// </summary>
        private static long RunComplexSimulation(int seed)
        {
            RandomSeed rng = new RandomSeed(seed);

            Vector3 position = new Vector3(
                new FixedPoint(rng.Range(-100, 100)),
                new FixedPoint(rng.Range(0, 50)),
                new FixedPoint(rng.Range(-100, 100)));

            Vector3 velocity = new Vector3(
                new FixedPoint(rng.Range(-20, 20)),
                new FixedPoint(rng.Range(-5, 5)),
                new FixedPoint(rng.Range(-20, 20)));

            Vector3 gravity = new Vector3(FixedPoint.Zero, new FixedPoint(-10), FixedPoint.Zero);
            FixedPoint dt = new FixedPoint(0.02);

            Quaternion rotation = Quaternion.Euler(
                new FixedPoint(rng.Range(0, 360)),
                new FixedPoint(rng.Range(0, 360)),
                new FixedPoint(rng.Range(0, 360)));

            for (int i = 0; i < 500; i++)
            {
                velocity = velocity + gravity * dt;
                position = position + velocity * dt;

                if (i % 50 == 0)
                {
                    Quaternion step = Quaternion.Euler(
                        FixedPoint.Zero,
                        new FixedPoint(rng.Range(-45, 45)),
                        FixedPoint.Zero);
                    rotation = rotation * step;
                    velocity = rotation * velocity.Normalized * velocity.Magnitude;
                }

                if (position.y < FixedPoint.Zero)
                {
                    position.y = -position.y;
                    velocity.y = -velocity.y * new FixedPoint(0.8);
                }
            }

            return position.x.FixedValue ^ position.y.FixedValue ^ position.z.FixedValue;
        }

        #endregion
    }
}
