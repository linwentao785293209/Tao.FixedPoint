using System;
using System.Globalization;

namespace Tao.FixedPoint
{
    /// <summary>
    /// 定点数四元数，用于 3D 旋转表示，避免万向锁
    /// 所有运算基于整数，保证跨平台确定性
    /// </summary>
    public struct Quaternion : IEquatable<Quaternion>
    {
        #region 字段和属性

        /// <summary>
        /// X 分量 (虚部 i)
        /// </summary>
        public FixedPoint x;

        /// <summary>
        /// Y 分量 (虚部 j)
        /// </summary>
        public FixedPoint y;

        /// <summary>
        /// Z 分量 (虚部 k)
        /// </summary>
        public FixedPoint z;

        /// <summary>
        /// W 分量 (实部)
        /// </summary>
        public FixedPoint w;

        /// <summary>
        /// 单位四元数缓存 (0, 0, 0, 1)
        /// </summary>
        private static readonly Quaternion _identity = new Quaternion(
            FixedPoint.Zero, FixedPoint.Zero, FixedPoint.Zero, FixedPoint.One);

        /// <summary>
        /// Slerp 近线性插值阈值 (dot > 0.999 时退化为 Lerp)
        /// </summary>
        private static readonly FixedPoint _slerpThreshold = new FixedPoint(1023L);

        /// <summary>
        /// 单位四元数，表示无旋转
        /// </summary>
        public static Quaternion Identity => _identity;

        /// <summary>
        /// 单位化后的四元数 (只读)
        /// </summary>
        public Quaternion Normalized
        {
            get
            {
                Quaternion result = new Quaternion(x, y, z, w);
                result.Normalize();
                return result;
            }
        }

        /// <summary>
        /// 欧拉角表示 (度，ZXY 旋转顺序，与 Unity 一致)
        /// </summary>
        public Vector3 EulerAngles
        {
            get => ToEulerAngles();
            set
            {
                Quaternion q = Euler(value.x, value.y, value.z);
                x = q.x;
                y = q.y;
                z = q.z;
                w = q.w;
            }
        }

        /// <summary>
        /// 通过索引访问分量 (0=x, 1=y, 2=z, 3=w)
        /// </summary>
        /// <param name="index">分量索引</param>
        public FixedPoint this[int index]
        {
            get => index switch
            {
                0 => x,
                1 => y,
                2 => z,
                3 => w,
                _ => throw new IndexOutOfRangeException("Invalid Quaternion index!"),
            };
            set
            {
                switch (index)
                {
                    case 0: x = value; break;
                    case 1: y = value; break;
                    case 2: z = value; break;
                    case 3: w = value; break;
                    default: throw new IndexOutOfRangeException("Invalid Quaternion index!");
                }
            }
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 从四个定点数构造
        /// </summary>
        /// <param name="x">X 分量 (虚部 i)</param>
        /// <param name="y">Y 分量 (虚部 j)</param>
        /// <param name="z">Z 分量 (虚部 k)</param>
        /// <param name="w">W 分量 (实部)</param>
        public Quaternion(FixedPoint x, FixedPoint y, FixedPoint z, FixedPoint w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        #endregion

        #region 静态构造方法

        /// <summary>
        /// 从欧拉角创建四元数 (度，ZXY 旋转顺序，与 Unity 一致)
        /// </summary>
        /// <param name="euler">欧拉角向量 (x=pitch, y=yaw, z=roll)</param>
        public static Quaternion Euler(Vector3 euler)
        {
            return Euler(euler.x, euler.y, euler.z);
        }

        /// <summary>
        /// 从欧拉角创建四元数 (度，ZXY 旋转顺序，与 Unity 一致)
        /// R = Rz · Rx · Ry
        /// </summary>
        /// <param name="xDeg">X 轴旋转角度 (pitch)</param>
        /// <param name="yDeg">Y 轴旋转角度 (yaw)</param>
        /// <param name="zDeg">Z 轴旋转角度 (roll)</param>
        public static Quaternion Euler(FixedPoint xDeg, FixedPoint yDeg, FixedPoint zDeg)
        {
            // 将角度转为半角弧度 (四元数公式需要半角)
            FixedPoint halfX = Math.DegreesToRadians(xDeg) >> 1;
            FixedPoint halfY = Math.DegreesToRadians(yDeg) >> 1;
            FixedPoint halfZ = Math.DegreesToRadians(zDeg) >> 1;

            FixedPoint sinHalfX = Math.Sin(halfX);
            FixedPoint cosHalfX = Math.Cos(halfX);
            FixedPoint sinHalfY = Math.Sin(halfY);
            FixedPoint cosHalfY = Math.Cos(halfY);
            FixedPoint sinHalfZ = Math.Sin(halfZ);
            FixedPoint cosHalfZ = Math.Cos(halfZ);

            // Hamilton 乘积: Qz · Qx · Qy
            return new Quaternion(
                cosHalfZ * sinHalfX * cosHalfY - sinHalfZ * cosHalfX * sinHalfY,
                cosHalfZ * cosHalfX * sinHalfY + sinHalfZ * sinHalfX * cosHalfY,
                cosHalfZ * sinHalfX * sinHalfY + sinHalfZ * cosHalfX * cosHalfY,
                cosHalfZ * cosHalfX * cosHalfY - sinHalfZ * sinHalfX * sinHalfY);
        }

        /// <summary>
        /// 从旋转轴和角度创建四元数
        /// </summary>
        /// <param name="angle">旋转角度 (度)</param>
        /// <param name="axis">旋转轴 (不需要预先归一化)</param>
        public static Quaternion AngleAxis(FixedPoint angle, Vector3 axis)
        {
            axis = Vector3.Normalize(axis);
            FixedPoint halfRad = Math.DegreesToRadians(angle) >> 1;
            FixedPoint sinHalf = Math.Sin(halfRad);
            return new Quaternion(axis.x * sinHalf, axis.y * sinHalf, axis.z * sinHalf, Math.Cos(halfRad));
        }

        /// <summary>
        /// 创建从 fromDirection 旋转到 toDirection 的四元数
        /// </summary>
        /// <param name="fromDirection">起始方向</param>
        /// <param name="toDirection">目标方向</param>
        public static Quaternion FromToRotation(Vector3 fromDirection, Vector3 toDirection)
        {
            Vector3 from = Vector3.Normalize(fromDirection);
            Vector3 to = Vector3.Normalize(toDirection);
            FixedPoint dot = Vector3.Dot(from, to);

            if (dot >= FixedPoint.One)
            {
                return _identity;
            }

            // 反向情况：找一个正交轴做 180° 旋转
            if (dot <= FixedPoint.NegativeOne)
            {
                Vector3 ortho = Vector3.Cross(Vector3.Right, from);
                if (ortho.SqrMagnitude < FixedPoint.Epsilon)
                {
                    ortho = Vector3.Cross(Vector3.Up, from);
                }

                ortho = Vector3.Normalize(ortho);
                return new Quaternion(ortho.x, ortho.y, ortho.z, FixedPoint.Zero);
            }

            // 一般情况: q = (cross, 1+dot)，然后归一化
            Vector3 cross = Vector3.Cross(from, to);
            Quaternion q = new Quaternion(cross.x, cross.y, cross.z, FixedPoint.One + dot);
            return q.Normalized;
        }

        /// <summary>
        /// 创建面向 forward 方向的旋转 (以 Vector3.Up 为默认上方向)
        /// </summary>
        /// <param name="forward">前方方向</param>
        public static Quaternion LookRotation(Vector3 forward)
        {
            return LookRotation(forward, Vector3.Up);
        }

        /// <summary>
        /// 创建面向 forward 方向的旋转
        /// 通过构建正交基 (right, up, forward) 再转换为四元数
        /// </summary>
        /// <param name="forward">前方方向</param>
        /// <param name="upwards">参考上方向</param>
        public static Quaternion LookRotation(Vector3 forward, Vector3 upwards)
        {
            forward = Vector3.Normalize(forward);
            Vector3 right = Vector3.Cross(upwards, forward);

            // upwards 与 forward 平行时，选择替代轴 (与 FromToRotation 模式一致)
            if (right.SqrMagnitude < FixedPoint.Epsilon)
            {
                right = Vector3.Cross(Vector3.Right, forward);
                if (right.SqrMagnitude < FixedPoint.Epsilon)
                {
                    right = Vector3.Cross(Vector3.Up, forward);
                }
            }

            right = Vector3.Normalize(right);
            Vector3 up = Vector3.Cross(forward, right);

            // 正交基转四元数 (Shepperd 方法)
            // m00=right.x  m01=up.x  m02=forward.x
            // m10=right.y  m11=up.y  m12=forward.y
            // m20=right.z  m21=up.z  m22=forward.z
            FixedPoint trace = right.x + up.y + forward.z;

            Quaternion q;
            if (trace > FixedPoint.Zero)
            {
                // 最大分量为 w
                FixedPoint sqrtTerm = Math.Sqrt(trace + FixedPoint.One);
                FixedPoint divisor = sqrtTerm + sqrtTerm;
                q = new Quaternion(
                    (up.z - forward.y) / divisor,
                    (forward.x - right.z) / divisor,
                    (right.y - up.x) / divisor,
                    sqrtTerm >> 1);
            }
            else if (right.x >= up.y && right.x >= forward.z)
            {
                // 最大分量为 x
                FixedPoint sqrtTerm = Math.Sqrt(FixedPoint.One + right.x - up.y - forward.z);
                FixedPoint divisor = sqrtTerm + sqrtTerm;
                q = new Quaternion(
                    sqrtTerm >> 1,
                    (right.y + up.x) / divisor,
                    (forward.x + right.z) / divisor,
                    (up.z - forward.y) / divisor);
            }
            else if (up.y > forward.z)
            {
                // 最大分量为 y
                FixedPoint sqrtTerm = Math.Sqrt(FixedPoint.One + up.y - right.x - forward.z);
                FixedPoint divisor = sqrtTerm + sqrtTerm;
                q = new Quaternion(
                    (right.y + up.x) / divisor,
                    sqrtTerm >> 1,
                    (up.z + forward.y) / divisor,
                    (forward.x - right.z) / divisor);
            }
            else
            {
                // 最大分量为 z
                FixedPoint sqrtTerm = Math.Sqrt(FixedPoint.One + forward.z - right.x - up.y);
                FixedPoint divisor = sqrtTerm + sqrtTerm;
                q = new Quaternion(
                    (forward.x + right.z) / divisor,
                    (up.z + forward.y) / divisor,
                    sqrtTerm >> 1,
                    (right.y - up.x) / divisor);
            }

            return q.Normalized;
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 设置分量值
        /// </summary>
        /// <param name="newX">新的 X 分量</param>
        /// <param name="newY">新的 Y 分量</param>
        /// <param name="newZ">新的 Z 分量</param>
        /// <param name="newW">新的 W 分量</param>
        public void Set(FixedPoint newX, FixedPoint newY, FixedPoint newZ, FixedPoint newW)
        {
            x = newX;
            y = newY;
            z = newZ;
            w = newW;
        }

        /// <summary>
        /// 将自身归一化为单位四元数
        /// </summary>
        public void Normalize()
        {
            FixedPoint mag = Math.Sqrt(x * x + y * y + z * z + w * w);
            if (mag > FixedPoint.Zero)
            {
                x /= mag;
                y /= mag;
                z /= mag;
                w /= mag;
            }
            else
            {
                this = _identity;
            }
        }

        /// <summary>
        /// 四元数点积
        /// </summary>
        /// <param name="a">左四元数</param>
        /// <param name="b">右四元数</param>
        public static FixedPoint Dot(Quaternion a, Quaternion b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
        }

        /// <summary>
        /// 两个旋转之间的夹角 (度)
        /// </summary>
        /// <param name="a">起始旋转</param>
        /// <param name="b">目标旋转</param>
        public static FixedPoint Angle(Quaternion a, Quaternion b)
        {
            FixedPoint dot = Math.Clamp(Dot(a, b), FixedPoint.NegativeOne, FixedPoint.One);
            FixedPoint absDot = Math.Abs(dot);
            // θ = 2·acos(|dot|)
            return Math.Acos(absDot) * FixedPoint.Two * Math.Rad2Deg;
        }

        /// <summary>
        /// 四元数的逆 (对单位四元数即为共轭)
        /// </summary>
        /// <param name="rotation">输入四元数 (必须为单位四元数)</param>
        public static Quaternion Inverse(Quaternion rotation)
        {
            return new Quaternion(-rotation.x, -rotation.y, -rotation.z, rotation.w);
        }

        /// <summary>
        /// 球面线性插值 (t 限制在 [0,1]，自动取最短路径)
        /// </summary>
        /// <param name="a">起始旋转</param>
        /// <param name="b">目标旋转</param>
        /// <param name="t">插值系数</param>
        public static Quaternion Slerp(Quaternion a, Quaternion b, FixedPoint t)
        {
            t = Math.Clamp01(t);
            return SlerpUnclamped(a, b, t);
        }

        /// <summary>
        /// 球面线性插值 (t 不限制范围，自动取最短路径)
        /// </summary>
        /// <param name="a">起始旋转</param>
        /// <param name="b">目标旋转</param>
        /// <param name="t">插值系数</param>
        public static Quaternion SlerpUnclamped(Quaternion a, Quaternion b, FixedPoint t)
        {
            FixedPoint dot = Dot(a, b);

            // 取最短路径: 若点积为负，翻转 b 使插值走短弧
            if (dot < FixedPoint.Zero)
            {
                b = new Quaternion(-b.x, -b.y, -b.z, -b.w);
                dot = -dot;
            }

            // 接近平行时退化为归一化线性插值，避免除零
            if (dot > _slerpThreshold)
            {
                return NLerp(a, b, t);
            }

            dot = Math.Clamp(dot, FixedPoint.NegativeOne, FixedPoint.One);
            FixedPoint theta = Math.Acos(dot);
            FixedPoint sinTheta = Math.Sin(theta);

            // 球面插值权重: 按弧度比例分配
            FixedPoint weightA = Math.Sin((FixedPoint.One - t) * theta) / sinTheta;
            FixedPoint weightB = Math.Sin(t * theta) / sinTheta;

            return new Quaternion(
                a.x * weightA + b.x * weightB,
                a.y * weightA + b.y * weightB,
                a.z * weightA + b.z * weightB,
                a.w * weightA + b.w * weightB);
        }

        /// <summary>
        /// 线性插值后归一化 (t 限制在 [0,1])
        /// </summary>
        /// <param name="a">起始旋转</param>
        /// <param name="b">目标旋转</param>
        /// <param name="t">插值系数</param>
        public static Quaternion Lerp(Quaternion a, Quaternion b, FixedPoint t)
        {
            t = Math.Clamp01(t);
            return NLerp(a, b, t);
        }

        /// <summary>
        /// 线性插值后归一化 (t 不限制范围)
        /// </summary>
        /// <param name="a">起始旋转</param>
        /// <param name="b">目标旋转</param>
        /// <param name="t">插值系数</param>
        public static Quaternion LerpUnclamped(Quaternion a, Quaternion b, FixedPoint t)
        {
            return NLerp(a, b, t);
        }

        /// <summary>
        /// 从当前旋转向目标旋转移动，每步最多旋转 maxDegreesDelta 度
        /// </summary>
        /// <param name="from">当前旋转</param>
        /// <param name="to">目标旋转</param>
        /// <param name="maxDegreesDelta">每步最大旋转角度</param>
        public static Quaternion RotateTowards(Quaternion from, Quaternion to, FixedPoint maxDegreesDelta)
        {
            FixedPoint angle = Angle(from, to);
            if (angle == FixedPoint.Zero)
            {
                return to;
            }

            FixedPoint t = Math.Min(FixedPoint.One, maxDegreesDelta / angle);
            return SlerpUnclamped(from, to, t);
        }

        /// <summary>
        /// 返回默认格式字符串
        /// </summary>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                "({0:F2}, {1:F2}, {2:F2}, {3:F2})",
                x.RawDouble, y.RawDouble, z.RawDouble, w.RawDouble);
        }

        /// <summary>
        /// 返回哈希码
        /// </summary>
        public override int GetHashCode()
        {
            return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2) ^ (w.GetHashCode() >> 1);
        }

        /// <summary>
        /// 判断与任意对象是否相等
        /// </summary>
        /// <param name="other">要比较的对象</param>
        public override bool Equals(object? other)
        {
            return other is Quaternion q && Equals(q);
        }

        /// <summary>
        /// 判断与另一个四元数是否相等
        /// </summary>
        /// <param name="other">要比较的四元数</param>
        public bool Equals(Quaternion other)
        {
            return x == other.x && y == other.y && z == other.z && w == other.w;
        }

        #endregion

        #region 运算符

        /// <summary>
        /// 四元数乘法 (Hamilton 乘积，表示旋转组合)
        /// </summary>
        /// <param name="lhs">左四元数</param>
        /// <param name="rhs">右四元数</param>
        public static Quaternion operator *(Quaternion lhs, Quaternion rhs)
        {
            return new Quaternion(
                lhs.w * rhs.x + lhs.x * rhs.w + lhs.y * rhs.z - lhs.z * rhs.y,
                lhs.w * rhs.y - lhs.x * rhs.z + lhs.y * rhs.w + lhs.z * rhs.x,
                lhs.w * rhs.z + lhs.x * rhs.y - lhs.y * rhs.x + lhs.z * rhs.w,
                lhs.w * rhs.w - lhs.x * rhs.x - lhs.y * rhs.y - lhs.z * rhs.z);
        }

        /// <summary>
        /// 旋转三维向量: v' = q · v · q⁻¹
        /// 使用优化公式: v' = v + 2w(u×v) + 2(u×(u×v))，其中 u = (x,y,z)
        /// </summary>
        /// <param name="rotation">旋转四元数</param>
        /// <param name="point">要旋转的向量</param>
        public static Vector3 operator *(Quaternion rotation, Vector3 point)
        {
            // t = 2(u × v)
            FixedPoint tx = FixedPoint.Two * (rotation.y * point.z - rotation.z * point.y);
            FixedPoint ty = FixedPoint.Two * (rotation.z * point.x - rotation.x * point.z);
            FixedPoint tz = FixedPoint.Two * (rotation.x * point.y - rotation.y * point.x);

            // u × t
            FixedPoint utx = rotation.y * tz - rotation.z * ty;
            FixedPoint uty = rotation.z * tx - rotation.x * tz;
            FixedPoint utz = rotation.x * ty - rotation.y * tx;

            return new Vector3(
                point.x + rotation.w * tx + utx,
                point.y + rotation.w * ty + uty,
                point.z + rotation.w * tz + utz);
        }

        /// <summary>
        /// 精确相等比较 (与 Equals / GetHashCode 语义一致)
        /// </summary>
        /// <param name="lhs">左操作数</param>
        /// <param name="rhs">右操作数</param>
        public static bool operator ==(Quaternion lhs, Quaternion rhs)
        {
            return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z && lhs.w == rhs.w;
        }

        /// <summary>
        /// 不等比较
        /// </summary>
        /// <param name="lhs">左操作数</param>
        /// <param name="rhs">右操作数</param>
        public static bool operator !=(Quaternion lhs, Quaternion rhs) => !(lhs == rhs);

        #endregion

        #region 内部辅助

        /// <summary>
        /// 归一化线性插值 (NLerp)
        /// </summary>
        /// <param name="a">起始旋转</param>
        /// <param name="b">目标旋转</param>
        /// <param name="t">插值系数</param>
        private static Quaternion NLerp(Quaternion a, Quaternion b, FixedPoint t)
        {
            FixedPoint oneMinusT = FixedPoint.One - t;
            Quaternion result = new Quaternion(
                oneMinusT * a.x + t * b.x,
                oneMinusT * a.y + t * b.y,
                oneMinusT * a.z + t * b.z,
                oneMinusT * a.w + t * b.w);
            result.Normalize();
            return result;
        }

        /// <summary>
        /// 将四元数转换为欧拉角 (度，ZXY 旋转顺序)
        /// 通过旋转矩阵元素反算: sinX = 2(yz+xw), Y = atan2(...), Z = atan2(...)
        /// </summary>
        private Vector3 ToEulerAngles()
        {
            // 旋转矩阵元素 m21 = 2(yz + xw) 对应 sin(eulerX)
            FixedPoint sinX = FixedPoint.Two * (y * z + x * w);

            FixedPoint eulerX;
            FixedPoint eulerY;
            FixedPoint eulerZ;

            // 万向锁检测: |sinX| ≥ 1 时, cos(X) ≈ 0, atan2 不可靠
            if (sinX >= FixedPoint.One)
            {
                eulerX = FixedPoint.PiOver2;
                eulerY = FixedPoint.Zero;
                eulerZ = Math.Atan2(
                    FixedPoint.Two * (x * y + w * z),
                    FixedPoint.One - FixedPoint.Two * (y * y + z * z));
            }
            else if (sinX <= FixedPoint.NegativeOne)
            {
                eulerX = -FixedPoint.PiOver2;
                eulerY = FixedPoint.Zero;
                eulerZ = Math.Atan2(
                    FixedPoint.Two * (x * y + w * z),
                    FixedPoint.One - FixedPoint.Two * (y * y + z * z));
            }
            else
            {
                eulerX = Math.Asin(sinX);
                // m20 = 2(xz - yw), m22 = 1 - 2(x²+y²)
                eulerY = Math.Atan2(
                    -(FixedPoint.Two * (x * z - y * w)),
                    FixedPoint.One - FixedPoint.Two * (x * x + y * y));
                // m01 = 2(xy - zw), m11 = 1 - 2(x²+z²)
                eulerZ = Math.Atan2(
                    -(FixedPoint.Two * (x * y - z * w)),
                    FixedPoint.One - FixedPoint.Two * (x * x + z * z));
            }

            return new Vector3(
                Math.RadiansToDegrees(eulerX),
                Math.RadiansToDegrees(eulerY),
                Math.RadiansToDegrees(eulerZ));
        }

        #endregion
    }
}
