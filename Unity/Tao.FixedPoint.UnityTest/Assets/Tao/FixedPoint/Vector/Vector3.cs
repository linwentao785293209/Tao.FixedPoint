using System;
using System.Globalization;

namespace Tao.FixedPoint
{
    /// <summary>
    /// 三维定点数向量，保证跨平台确定性
    /// </summary>
    public struct Vector3 : IEquatable<Vector3>, IFormattable
    {
        #region 字段和属性

        /// <summary>
        /// X 分量
        /// </summary>
        public FixedPoint x;

        /// <summary>
        /// Y 分量
        /// </summary>
        public FixedPoint y;

        /// <summary>
        /// Z 分量
        /// </summary>
        public FixedPoint z;

        /// <summary>
        /// 零向量缓存
        /// </summary>
        private static readonly Vector3 _zeroVector = new Vector3(FixedPoint.Zero, FixedPoint.Zero, FixedPoint.Zero);

        /// <summary>
        /// 单位向量缓存
        /// </summary>
        private static readonly Vector3 _oneVector = new Vector3(FixedPoint.One, FixedPoint.One, FixedPoint.One);

        /// <summary>
        /// 上方向缓存
        /// </summary>
        private static readonly Vector3 _upVector = new Vector3(FixedPoint.Zero, FixedPoint.One, FixedPoint.Zero);

        /// <summary>
        /// 下方向缓存
        /// </summary>
        private static readonly Vector3 _downVector = new Vector3(FixedPoint.Zero, FixedPoint.NegativeOne, FixedPoint.Zero);

        /// <summary>
        /// 左方向缓存
        /// </summary>
        private static readonly Vector3 _leftVector = new Vector3(FixedPoint.NegativeOne, FixedPoint.Zero, FixedPoint.Zero);

        /// <summary>
        /// 右方向缓存
        /// </summary>
        private static readonly Vector3 _rightVector = new Vector3(FixedPoint.One, FixedPoint.Zero, FixedPoint.Zero);

        /// <summary>
        /// 前方向缓存
        /// </summary>
        private static readonly Vector3 _forwardVector = new Vector3(FixedPoint.Zero, FixedPoint.Zero, FixedPoint.One);

        /// <summary>
        /// 后方向缓存
        /// </summary>
        private static readonly Vector3 _backVector = new Vector3(FixedPoint.Zero, FixedPoint.Zero, FixedPoint.NegativeOne);

        /// <summary>
        /// 通过索引访问分量 (0=x, 1=y, 2=z)
        /// </summary>
        /// <param name="index">分量索引</param>
        public FixedPoint this[int index]
        {
            get => index switch
            {
                0 => x,
                1 => y,
                2 => z,
                _ => throw new IndexOutOfRangeException("Invalid Vector3 index!"),
            };
            set
            {
                switch (index)
                {
                    case 0: x = value; break;
                    case 1: y = value; break;
                    case 2: z = value; break;
                    default: throw new IndexOutOfRangeException("Invalid Vector3 index!");
                }
            }
        }

        /// <summary>
        /// 单位化向量 (只读)
        /// </summary>
        public Vector3 Normalized => Normalize(this);

        /// <summary>
        /// 向量长度 (只读)
        /// </summary>
        public FixedPoint Magnitude => Math.Sqrt(x * x + y * y + z * z);

        /// <summary>
        /// 向量长度的平方 (只读，避免开方)
        /// </summary>
        public FixedPoint SqrMagnitude => x * x + y * y + z * z;

        /// <summary>
        /// 零向量 (0, 0, 0)
        /// </summary>
        public static Vector3 Zero => _zeroVector;

        /// <summary>
        /// 单位向量 (1, 1, 1)
        /// </summary>
        public static Vector3 One => _oneVector;

        /// <summary>
        /// 前方向 (0, 0, 1)
        /// </summary>
        public static Vector3 Forward => _forwardVector;

        /// <summary>
        /// 后方向 (0, 0, -1)
        /// </summary>
        public static Vector3 Back => _backVector;

        /// <summary>
        /// 上方向 (0, 1, 0)
        /// </summary>
        public static Vector3 Up => _upVector;

        /// <summary>
        /// 下方向 (0, -1, 0)
        /// </summary>
        public static Vector3 Down => _downVector;

        /// <summary>
        /// 左方向 (-1, 0, 0)
        /// </summary>
        public static Vector3 Left => _leftVector;

        /// <summary>
        /// 右方向 (1, 0, 0)
        /// </summary>
        public static Vector3 Right => _rightVector;

        #endregion

        #region 构造函数

        /// <summary>
        /// 从三个定点数构造
        /// </summary>
        /// <param name="x">X 分量</param>
        /// <param name="y">Y 分量</param>
        /// <param name="z">Z 分量</param>
        public Vector3(FixedPoint x, FixedPoint y, FixedPoint z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        /// <summary>
        /// 从三个浮点数构造 (仅用于初始化常量，运行时逻辑请使用 FixedPoint 构造)
        /// </summary>
        /// <param name="x">X 分量</param>
        /// <param name="y">Y 分量</param>
        /// <param name="z">Z 分量</param>
        public Vector3(float x, float y, float z)
        {
            this.x = new FixedPoint((long)System.Math.Round(x * FixedPoint.MULTIPLE));
            this.y = new FixedPoint((long)System.Math.Round(y * FixedPoint.MULTIPLE));
            this.z = new FixedPoint((long)System.Math.Round(z * FixedPoint.MULTIPLE));
        }

        /// <summary>
        /// 从两个定点数构造 (z = 0)
        /// </summary>
        /// <param name="x">X 分量</param>
        /// <param name="y">Y 分量</param>
        public Vector3(FixedPoint x, FixedPoint y)
        {
            this.x = x;
            this.y = y;
            z = FixedPoint.Zero;
        }

        /// <summary>
        /// 从两个浮点数构造 (z = 0，仅用于初始化常量，运行时逻辑请使用 FixedPoint 构造)
        /// </summary>
        /// <param name="x">X 分量</param>
        /// <param name="y">Y 分量</param>
        public Vector3(float x, float y)
        {
            this.x = new FixedPoint((long)System.Math.Round(x * FixedPoint.MULTIPLE));
            this.y = new FixedPoint((long)System.Math.Round(y * FixedPoint.MULTIPLE));
            z = FixedPoint.Zero;
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 设置分量值
        /// </summary>
        /// <param name="newX">新的 X 分量</param>
        /// <param name="newY">新的 Y 分量</param>
        /// <param name="newZ">新的 Z 分量</param>
        public void Set(FixedPoint newX, FixedPoint newY, FixedPoint newZ)
        {
            x = newX;
            y = newY;
            z = newZ;
        }

        /// <summary>
        /// 将自身单位化
        /// </summary>
        public void Normalize()
        {
            FixedPoint mag = Magnitude;
            if (mag > FixedPoint.Zero)
            {
                this /= mag;
            }
            else
            {
                this = _zeroVector;
            }
        }

        /// <summary>
        /// 分量相乘 (实例)
        /// </summary>
        /// <param name="scale">缩放向量</param>
        public void Scale(Vector3 scale)
        {
            x *= scale.x;
            y *= scale.y;
            z *= scale.z;
        }

        /// <summary>
        /// 点积
        /// </summary>
        /// <param name="lhs">左向量</param>
        /// <param name="rhs">右向量</param>
        public static FixedPoint Dot(Vector3 lhs, Vector3 rhs)
        {
            return lhs.x * rhs.x + lhs.y * rhs.y + lhs.z * rhs.z;
        }

        /// <summary>
        /// 叉积
        /// </summary>
        /// <param name="lhs">左向量</param>
        /// <param name="rhs">右向量</param>
        public static Vector3 Cross(Vector3 lhs, Vector3 rhs)
        {
            return new Vector3(
                lhs.y * rhs.z - lhs.z * rhs.y,
                lhs.z * rhs.x - lhs.x * rhs.z,
                lhs.x * rhs.y - lhs.y * rhs.x);
        }

        /// <summary>
        /// 两点之间的距离
        /// </summary>
        /// <param name="a">起点</param>
        /// <param name="b">终点</param>
        public static FixedPoint Distance(Vector3 a, Vector3 b)
        {
            FixedPoint dx = a.x - b.x;
            FixedPoint dy = a.y - b.y;
            FixedPoint dz = a.z - b.z;
            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        /// <summary>
        /// 两个向量之间的无符号角度 (度)
        /// </summary>
        /// <param name="from">起始向量</param>
        /// <param name="to">目标向量</param>
        public static FixedPoint Angle(Vector3 from, Vector3 to)
        {
            FixedPoint denominator = Math.Sqrt(from.SqrMagnitude * to.SqrMagnitude);
            if (denominator <= FixedPoint.Zero)
            {
                return FixedPoint.Zero;
            }

            FixedPoint dot = Math.Clamp(Dot(from, to) / denominator, FixedPoint.NegativeOne, FixedPoint.One);
            // 正交时 dot==0，直接返回精确 90°，避免 Acos+Rad2Deg 的精度损失
            if (dot == FixedPoint.Zero)
            {
                return new FixedPoint(90);
            }
            return Math.Acos(dot) * Math.Rad2Deg;
        }

        /// <summary>
        /// 两个向量之间的有符号角度 (度)
        /// </summary>
        /// <param name="from">起始向量</param>
        /// <param name="to">目标向量</param>
        /// <param name="axis">法线轴向量</param>
        public static FixedPoint SignedAngle(Vector3 from, Vector3 to, Vector3 axis)
        {
            FixedPoint angle = Angle(from, to);
            FixedPoint crossX = from.y * to.z - from.z * to.y;
            FixedPoint crossY = from.z * to.x - from.x * to.z;
            FixedPoint crossZ = from.x * to.y - from.y * to.x;
            FixedPoint sign = Math.Sign(axis.x * crossX + axis.y * crossY + axis.z * crossZ);
            return angle * sign;
        }

        /// <summary>
        /// 两个向量之间的弧度角
        /// </summary>
        /// <param name="from">起始向量</param>
        /// <param name="to">目标向量</param>
        public static FixedPoint AngleBetween(Vector3 from, Vector3 to)
        {
            return Math.Acos(Math.Clamp(Dot(from.Normalized, to.Normalized),
                FixedPoint.NegativeOne, FixedPoint.One));
        }

        /// <summary>
        /// 线性插值 (t 限制在 [0,1])
        /// </summary>
        /// <param name="a">起始向量</param>
        /// <param name="b">目标向量</param>
        /// <param name="t">插值系数</param>
        public static Vector3 Lerp(Vector3 a, Vector3 b, FixedPoint t)
        {
            t = Math.Clamp01(t);
            return new Vector3(
                a.x + (b.x - a.x) * t,
                a.y + (b.y - a.y) * t,
                a.z + (b.z - a.z) * t);
        }

        /// <summary>
        /// 线性插值 (t 不限制范围)
        /// </summary>
        /// <param name="a">起始向量</param>
        /// <param name="b">目标向量</param>
        /// <param name="t">插值系数</param>
        public static Vector3 LerpUnclamped(Vector3 a, Vector3 b, FixedPoint t)
        {
            return new Vector3(
                a.x + (b.x - a.x) * t,
                a.y + (b.y - a.y) * t,
                a.z + (b.z - a.z) * t);
        }

        /// <summary>
        /// 向目标移动，每次最多移动 maxDistanceDelta
        /// </summary>
        /// <param name="current">当前向量</param>
        /// <param name="target">目标向量</param>
        /// <param name="maxDistanceDelta">最大移动距离</param>
        public static Vector3 MoveTowards(Vector3 current, Vector3 target, FixedPoint maxDistanceDelta)
        {
            FixedPoint dx = target.x - current.x;
            FixedPoint dy = target.y - current.y;
            FixedPoint dz = target.z - current.z;
            FixedPoint sqrDist = dx * dx + dy * dy + dz * dz;
            if (sqrDist == FixedPoint.Zero
                || (maxDistanceDelta >= FixedPoint.Zero && sqrDist <= maxDistanceDelta * maxDistanceDelta))
            {
                return target;
            }

            FixedPoint dist = Math.Sqrt(sqrDist);
            return new Vector3(
                current.x + dx / dist * maxDistanceDelta,
                current.y + dy / dist * maxDistanceDelta,
                current.z + dz / dist * maxDistanceDelta);
        }

        /// <summary>
        /// 单位化 (静态)
        /// </summary>
        /// <param name="value">输入向量</param>
        public static Vector3 Normalize(Vector3 value)
        {
            FixedPoint mag = value.Magnitude;
            if (mag > FixedPoint.Zero)
            {
                return value / mag;
            }
            return _zeroVector;
        }

        /// <summary>
        /// 分量相乘 (静态)
        /// </summary>
        /// <param name="a">第一个向量</param>
        /// <param name="b">第二个向量</param>
        public static Vector3 Scale(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
        }

        /// <summary>
        /// 反射向量
        /// </summary>
        /// <param name="inDirection">入射方向</param>
        /// <param name="inNormal">法线方向</param>
        public static Vector3 Reflect(Vector3 inDirection, Vector3 inNormal)
        {
            FixedPoint factor = -FixedPoint.Two * Dot(inNormal, inDirection);
            return new Vector3(
                factor * inNormal.x + inDirection.x,
                factor * inNormal.y + inDirection.y,
                factor * inNormal.z + inDirection.z);
        }

        /// <summary>
        /// 向量投影到另一个向量上
        /// </summary>
        /// <param name="vector">输入向量</param>
        /// <param name="onNormal">目标法线</param>
        public static Vector3 Project(Vector3 vector, Vector3 onNormal)
        {
            FixedPoint sqrMag = Dot(onNormal, onNormal);
            if (sqrMag <= FixedPoint.Zero)
            {
                return _zeroVector;
            }

            FixedPoint dot = Dot(vector, onNormal);
            return new Vector3(
                onNormal.x * dot / sqrMag,
                onNormal.y * dot / sqrMag,
                onNormal.z * dot / sqrMag);
        }

        /// <summary>
        /// 向量投影到平面上
        /// </summary>
        /// <param name="vector">输入向量</param>
        /// <param name="planeNormal">平面法线</param>
        public static Vector3 ProjectOnPlane(Vector3 vector, Vector3 planeNormal)
        {
            FixedPoint sqrMag = Dot(planeNormal, planeNormal);
            if (sqrMag <= FixedPoint.Zero)
            {
                return vector;
            }

            FixedPoint dot = Dot(vector, planeNormal);
            return new Vector3(
                vector.x - planeNormal.x * dot / sqrMag,
                vector.y - planeNormal.y * dot / sqrMag,
                vector.z - planeNormal.z * dot / sqrMag);
        }

        /// <summary>
        /// 限制向量长度
        /// </summary>
        /// <param name="vector">输入向量</param>
        /// <param name="maxLength">最大长度</param>
        public static Vector3 ClampMagnitude(Vector3 vector, FixedPoint maxLength)
        {
            FixedPoint sqrMag = vector.SqrMagnitude;
            if (sqrMag > maxLength * maxLength)
            {
                FixedPoint mag = Math.Sqrt(sqrMag);
                return new Vector3(
                    vector.x / mag * maxLength,
                    vector.y / mag * maxLength,
                    vector.z / mag * maxLength);
            }
            return vector;
        }

        /// <summary>
        /// 返回分量最小值组合的向量
        /// </summary>
        /// <param name="lhs">左向量</param>
        /// <param name="rhs">右向量</param>
        public static Vector3 Min(Vector3 lhs, Vector3 rhs)
        {
            return new Vector3(Math.Min(lhs.x, rhs.x), Math.Min(lhs.y, rhs.y), Math.Min(lhs.z, rhs.z));
        }

        /// <summary>
        /// 返回分量最大值组合的向量
        /// </summary>
        /// <param name="lhs">左向量</param>
        /// <param name="rhs">右向量</param>
        public static Vector3 Max(Vector3 lhs, Vector3 rhs)
        {
            return new Vector3(Math.Max(lhs.x, rhs.x), Math.Max(lhs.y, rhs.y), Math.Max(lhs.z, rhs.z));
        }

        /// <summary>
        /// 排除某方向的分量 (等同于 ProjectOnPlane)
        /// </summary>
        /// <param name="excludeThis">要排除的方向</param>
        /// <param name="fromThat">输入向量</param>
        public static Vector3 Exclude(Vector3 excludeThis, Vector3 fromThat)
        {
            return ProjectOnPlane(fromThat, excludeThis);
        }

        /// <summary>
        /// 平滑阻尼 (无速度限制)
        /// </summary>
        /// <param name="current">当前位置</param>
        /// <param name="target">目标位置</param>
        /// <param name="currentVelocity">当前速度 (由函数修改)</param>
        /// <param name="smoothTime">到达目标的近似时间</param>
        /// <param name="deltaTime">帧同步时间增量 (不要使用 Time.deltaTime)</param>
        public static Vector3 SmoothDamp(Vector3 current, Vector3 target,
            ref Vector3 currentVelocity, FixedPoint smoothTime, FixedPoint deltaTime)
        {
            return SmoothDamp(current, target, ref currentVelocity, smoothTime, deltaTime,
                new FixedPoint(999999));
        }

        /// <summary>
        /// 平滑阻尼
        /// </summary>
        /// <param name="current">当前位置</param>
        /// <param name="target">目标位置</param>
        /// <param name="currentVelocity">当前速度 (由函数修改)</param>
        /// <param name="smoothTime">到达目标的近似时间</param>
        /// <param name="deltaTime">帧同步时间增量 (不要使用 Time.deltaTime)</param>
        /// <param name="maxSpeed">最大速度限制</param>
        public static Vector3 SmoothDamp(Vector3 current, Vector3 target,
            ref Vector3 currentVelocity, FixedPoint smoothTime, FixedPoint deltaTime,
            FixedPoint maxSpeed)
        {
            // 弹簧阻尼系统: omega = 2/smoothTime 为角频率
            smoothTime = Math.Max(FixedPoint.Epsilon, smoothTime);
            FixedPoint omega = FixedPoint.Two / smoothTime;
            FixedPoint dampingInput = omega * deltaTime;
            FixedPoint decayFactor = FixedPoint.One /
                             (FixedPoint.One + dampingInput + Math.SmoothDampC2 * dampingInput * dampingInput
                              + Math.SmoothDampC3 * dampingInput * dampingInput * dampingInput);
            FixedPoint dx = current.x - target.x;
            FixedPoint dy = current.y - target.y;
            FixedPoint dz = current.z - target.z;
            Vector3 original = target;
            FixedPoint maxChange = maxSpeed * smoothTime;
            FixedPoint maxChangeSqr = maxChange * maxChange;
            FixedPoint sqrDist = dx * dx + dy * dy + dz * dz;
            if (sqrDist > maxChangeSqr)
            {
                FixedPoint dist = Math.Sqrt(sqrDist);
                dx = dx / dist * maxChange;
                dy = dy / dist * maxChange;
                dz = dz / dist * maxChange;
            }

            target.x = current.x - dx;
            target.y = current.y - dy;
            target.z = current.z - dz;
            FixedPoint tempX = (currentVelocity.x + omega * dx) * deltaTime;
            FixedPoint tempY = (currentVelocity.y + omega * dy) * deltaTime;
            FixedPoint tempZ = (currentVelocity.z + omega * dz) * deltaTime;
            currentVelocity.x = (currentVelocity.x - omega * tempX) * decayFactor;
            currentVelocity.y = (currentVelocity.y - omega * tempY) * decayFactor;
            currentVelocity.z = (currentVelocity.z - omega * tempZ) * decayFactor;
            FixedPoint outX = target.x + (dx + tempX) * decayFactor;
            FixedPoint outY = target.y + (dy + tempY) * decayFactor;
            FixedPoint outZ = target.z + (dz + tempZ) * decayFactor;
            FixedPoint origDx = original.x - current.x;
            FixedPoint origDy = original.y - current.y;
            FixedPoint origDz = original.z - current.z;
            FixedPoint outDx = outX - original.x;
            FixedPoint outDy = outY - original.y;
            FixedPoint outDz = outZ - original.z;
            if (origDx * outDx + origDy * outDy + origDz * outDz > FixedPoint.Zero)
            {
                outX = original.x;
                outY = original.y;
                outZ = original.z;
                currentVelocity.x = (outX - original.x) / deltaTime;
                currentVelocity.y = (outY - original.y) / deltaTime;
                currentVelocity.z = (outZ - original.z) / deltaTime;
            }

            return new Vector3(outX, outY, outZ);
        }

        /// <summary>
        /// 返回默认格式字符串
        /// </summary>
        public override string ToString() => ToString(null, null);

        /// <summary>
        /// 使用指定格式返回字符串
        /// </summary>
        /// <param name="format">格式字符串</param>
        public string ToString(string? format) => ToString(format, null);

        /// <summary>
        /// 使用指定格式和格式提供程序返回字符串
        /// </summary>
        /// <param name="format">格式字符串</param>
        /// <param name="formatProvider">格式提供程序</param>
        public string ToString(string? format, IFormatProvider? formatProvider)
        {
            string? outputFormat = format;
            if (string.IsNullOrEmpty(outputFormat))
            {
                outputFormat = "F2";
            }

            IFormatProvider? outputFormatProvider = formatProvider;
            if (outputFormatProvider == null)
            {
                outputFormatProvider = CultureInfo.InvariantCulture.NumberFormat;
            }

            return string.Format(outputFormatProvider,
                "({0}, {1}, {2})",
                x.RawDouble.ToString(outputFormat, outputFormatProvider),
                y.RawDouble.ToString(outputFormat, outputFormatProvider),
                z.RawDouble.ToString(outputFormat, outputFormatProvider));
        }

        /// <summary>
        /// 返回哈希码
        /// </summary>
        public override int GetHashCode() => x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2);

        /// <summary>
        /// 判断与任意对象是否相等
        /// </summary>
        /// <param name="other">要比较的对象</param>
        public override bool Equals(object? other)
        {
            return other is Vector3 v && Equals(v);
        }

        /// <summary>
        /// 判断与另一个向量是否相等
        /// </summary>
        /// <param name="other">要比较的向量</param>
        public bool Equals(Vector3 other) => x == other.x && y == other.y && z == other.z;

        #endregion

        #region 运算符

        /// <summary>
        /// 向量加法
        /// </summary>
        /// <param name="a">左操作数</param>
        /// <param name="b">右操作数</param>
        public static Vector3 operator +(Vector3 a, Vector3 b) => new Vector3(a.x + b.x, a.y + b.y, a.z + b.z);

        /// <summary>
        /// 向量减法
        /// </summary>
        /// <param name="a">左操作数</param>
        /// <param name="b">右操作数</param>
        public static Vector3 operator -(Vector3 a, Vector3 b) => new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);

        /// <summary>
        /// 取负
        /// </summary>
        /// <param name="a">输入向量</param>
        public static Vector3 operator -(Vector3 a) => new Vector3(-a.x, -a.y, -a.z);

        /// <summary>
        /// 分量相乘
        /// </summary>
        /// <param name="a">左操作数</param>
        /// <param name="b">右操作数</param>
        public static Vector3 operator *(Vector3 a, Vector3 b) => new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);

        /// <summary>
        /// 分量相除
        /// </summary>
        /// <param name="a">左操作数</param>
        /// <param name="b">右操作数</param>
        public static Vector3 operator /(Vector3 a, Vector3 b) => new Vector3(a.x / b.x, a.y / b.y, a.z / b.z);

        /// <summary>
        /// 标量右乘
        /// </summary>
        /// <param name="a">输入向量</param>
        /// <param name="d">标量</param>
        public static Vector3 operator *(Vector3 a, FixedPoint d) => new Vector3(a.x * d, a.y * d, a.z * d);

        /// <summary>
        /// 标量左乘
        /// </summary>
        /// <param name="d">标量</param>
        /// <param name="a">输入向量</param>
        public static Vector3 operator *(FixedPoint d, Vector3 a) => new Vector3(a.x * d, a.y * d, a.z * d);

        /// <summary>
        /// 标量除法
        /// </summary>
        /// <param name="a">输入向量</param>
        /// <param name="d">标量</param>
        public static Vector3 operator /(Vector3 a, FixedPoint d) => new Vector3(a.x / d, a.y / d, a.z / d);

        /// <summary>
        /// 精确相等比较 (与 Equals / GetHashCode 语义一致)
        /// </summary>
        /// <param name="lhs">左操作数</param>
        /// <param name="rhs">右操作数</param>
        public static bool operator ==(Vector3 lhs, Vector3 rhs)
        {
            return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z;
        }

        /// <summary>
        /// 不等比较
        /// </summary>
        /// <param name="lhs">左操作数</param>
        /// <param name="rhs">右操作数</param>
        public static bool operator !=(Vector3 lhs, Vector3 rhs) => !(lhs == rhs);

        #endregion
    }
}
