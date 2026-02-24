using System;
using System.Globalization;

namespace Tao.FixedPoint
{
    /// <summary>
    /// 二维定点数向量，保证跨平台确定性
    /// </summary>
    public struct Vector2 : IEquatable<Vector2>, IFormattable
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
        /// 零向量缓存
        /// </summary>
        private static readonly Vector2 _zeroVector = new Vector2(FixedPoint.Zero, FixedPoint.Zero);

        /// <summary>
        /// 单位向量缓存
        /// </summary>
        private static readonly Vector2 _oneVector = new Vector2(FixedPoint.One, FixedPoint.One);

        /// <summary>
        /// 上方向缓存
        /// </summary>
        private static readonly Vector2 _upVector = new Vector2(FixedPoint.Zero, FixedPoint.One);

        /// <summary>
        /// 下方向缓存
        /// </summary>
        private static readonly Vector2 _downVector = new Vector2(FixedPoint.Zero, FixedPoint.NegativeOne);

        /// <summary>
        /// 左方向缓存
        /// </summary>
        private static readonly Vector2 _leftVector = new Vector2(FixedPoint.NegativeOne, FixedPoint.Zero);

        /// <summary>
        /// 右方向缓存
        /// </summary>
        private static readonly Vector2 _rightVector = new Vector2(FixedPoint.One, FixedPoint.Zero);

        /// <summary>
        /// 通过索引访问分量 (0=x, 1=y)
        /// </summary>
        /// <param name="index">分量索引</param>
        public FixedPoint this[int index]
        {
            get => index switch
            {
                0 => x,
                1 => y,
                _ => throw new IndexOutOfRangeException("Invalid Vector2 index!"),
            };
            set
            {
                switch (index)
                {
                    case 0: x = value; break;
                    case 1: y = value; break;
                    default: throw new IndexOutOfRangeException("Invalid Vector2 index!");
                }
            }
        }

        /// <summary>
        /// 单位化向量 (只读)
        /// </summary>
        public Vector2 Normalized
        {
            get
            {
                Vector2 result = new Vector2(x, y);
                result.Normalize();
                return result;
            }
        }

        /// <summary>
        /// 向量长度 (只读)
        /// </summary>
        public FixedPoint Magnitude => Math.Sqrt(x * x + y * y);

        /// <summary>
        /// 向量长度的平方 (只读，避免开方)
        /// </summary>
        public FixedPoint SqrMagnitude => x * x + y * y;

        /// <summary>
        /// 零向量 (0, 0)
        /// </summary>
        public static Vector2 Zero => _zeroVector;

        /// <summary>
        /// 单位向量 (1, 1)
        /// </summary>
        public static Vector2 One => _oneVector;

        /// <summary>
        /// 上方向 (0, 1)
        /// </summary>
        public static Vector2 Up => _upVector;

        /// <summary>
        /// 下方向 (0, -1)
        /// </summary>
        public static Vector2 Down => _downVector;

        /// <summary>
        /// 左方向 (-1, 0)
        /// </summary>
        public static Vector2 Left => _leftVector;

        /// <summary>
        /// 右方向 (1, 0)
        /// </summary>
        public static Vector2 Right => _rightVector;

        #endregion

        #region 构造函数

        /// <summary>
        /// 从定点数构造
        /// </summary>
        /// <param name="x">X 分量</param>
        /// <param name="y">Y 分量</param>
        public Vector2(FixedPoint x, FixedPoint y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// 从浮点数构造 (仅用于初始化常量，运行时逻辑请使用 FixedPoint 构造)
        /// </summary>
        /// <param name="x">X 分量</param>
        /// <param name="y">Y 分量</param>
        public Vector2(float x, float y)
        {
            this.x = new FixedPoint((long)System.Math.Round(x * FixedPoint.MULTIPLE));
            this.y = new FixedPoint((long)System.Math.Round(y * FixedPoint.MULTIPLE));
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 设置分量值
        /// </summary>
        /// <param name="newX">新的 X 分量</param>
        /// <param name="newY">新的 Y 分量</param>
        public void Set(FixedPoint newX, FixedPoint newY)
        {
            x = newX;
            y = newY;
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
        public void Scale(Vector2 scale)
        {
            x *= scale.x;
            y *= scale.y;
        }

        /// <summary>
        /// 点积
        /// </summary>
        /// <param name="lhs">左向量</param>
        /// <param name="rhs">右向量</param>
        public static FixedPoint Dot(Vector2 lhs, Vector2 rhs)
        {
            return lhs.x * rhs.x + lhs.y * rhs.y;
        }

        /// <summary>
        /// 两点之间的距离
        /// </summary>
        /// <param name="a">起点</param>
        /// <param name="b">终点</param>
        public static FixedPoint Distance(Vector2 a, Vector2 b)
        {
            FixedPoint dx = a.x - b.x;
            FixedPoint dy = a.y - b.y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// 两个向量之间的无符号角度 (度)
        /// </summary>
        /// <param name="from">起始向量</param>
        /// <param name="to">目标向量</param>
        public static FixedPoint Angle(Vector2 from, Vector2 to)
        {
            FixedPoint denominator = Math.Sqrt(from.SqrMagnitude * to.SqrMagnitude);
            if (denominator <= FixedPoint.Zero)
            {
                return FixedPoint.Zero;
            }

            FixedPoint dot = Math.Clamp(Dot(from, to) / denominator, FixedPoint.NegativeOne, FixedPoint.One);
            return Math.Acos(dot) * Math.Rad2Deg;
        }

        /// <summary>
        /// 两个向量之间的有符号角度 (度)
        /// </summary>
        /// <param name="from">起始向量</param>
        /// <param name="to">目标向量</param>
        public static FixedPoint SignedAngle(Vector2 from, Vector2 to)
        {
            FixedPoint angle = Angle(from, to);
            FixedPoint sign = Math.Sign(from.x * to.y - from.y * to.x);
            return angle * sign;
        }

        /// <summary>
        /// 线性插值 (t 限制在 [0,1])
        /// </summary>
        /// <param name="a">起始向量</param>
        /// <param name="b">目标向量</param>
        /// <param name="t">插值系数</param>
        public static Vector2 Lerp(Vector2 a, Vector2 b, FixedPoint t)
        {
            t = Math.Clamp01(t);
            return new Vector2(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t);
        }

        /// <summary>
        /// 线性插值 (t 不限制范围)
        /// </summary>
        /// <param name="a">起始向量</param>
        /// <param name="b">目标向量</param>
        /// <param name="t">插值系数</param>
        public static Vector2 LerpUnclamped(Vector2 a, Vector2 b, FixedPoint t)
        {
            return new Vector2(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t);
        }

        /// <summary>
        /// 向目标移动，每次最多移动 maxDistanceDelta
        /// </summary>
        /// <param name="current">当前向量</param>
        /// <param name="target">目标向量</param>
        /// <param name="maxDistanceDelta">最大移动距离</param>
        public static Vector2 MoveTowards(Vector2 current, Vector2 target, FixedPoint maxDistanceDelta)
        {
            FixedPoint dx = target.x - current.x;
            FixedPoint dy = target.y - current.y;
            FixedPoint sqrDist = dx * dx + dy * dy;
            if (sqrDist == FixedPoint.Zero
                || (maxDistanceDelta >= FixedPoint.Zero && sqrDist <= maxDistanceDelta * maxDistanceDelta))
            {
                return target;
            }

            FixedPoint dist = Math.Sqrt(sqrDist);
            return new Vector2(
                current.x + dx / dist * maxDistanceDelta,
                current.y + dy / dist * maxDistanceDelta);
        }

        /// <summary>
        /// 分量相乘 (静态)
        /// </summary>
        /// <param name="a">第一个向量</param>
        /// <param name="b">第二个向量</param>
        public static Vector2 Scale(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x * b.x, a.y * b.y);
        }

        /// <summary>
        /// 反射向量
        /// </summary>
        /// <param name="inDirection">入射方向</param>
        /// <param name="inNormal">法线方向</param>
        public static Vector2 Reflect(Vector2 inDirection, Vector2 inNormal)
        {
            FixedPoint factor = -FixedPoint.Two * Dot(inNormal, inDirection);
            return new Vector2(factor * inNormal.x + inDirection.x, factor * inNormal.y + inDirection.y);
        }

        /// <summary>
        /// 返回逆时针旋转 90° 的垂直向量
        /// </summary>
        /// <param name="inDirection">输入方向</param>
        public static Vector2 Perpendicular(Vector2 inDirection)
        {
            return new Vector2(-inDirection.y, inDirection.x);
        }

        /// <summary>
        /// 限制向量长度
        /// </summary>
        /// <param name="vector">输入向量</param>
        /// <param name="maxLength">最大长度</param>
        public static Vector2 ClampMagnitude(Vector2 vector, FixedPoint maxLength)
        {
            FixedPoint sqrMag = vector.SqrMagnitude;
            if (sqrMag > maxLength * maxLength)
            {
                FixedPoint mag = Math.Sqrt(sqrMag);
                return new Vector2(vector.x / mag * maxLength, vector.y / mag * maxLength);
            }
            return vector;
        }

        /// <summary>
        /// 返回分量最小值组合的向量
        /// </summary>
        /// <param name="lhs">左向量</param>
        /// <param name="rhs">右向量</param>
        public static Vector2 Min(Vector2 lhs, Vector2 rhs)
        {
            return new Vector2(Math.Min(lhs.x, rhs.x), Math.Min(lhs.y, rhs.y));
        }

        /// <summary>
        /// 返回分量最大值组合的向量
        /// </summary>
        /// <param name="lhs">左向量</param>
        /// <param name="rhs">右向量</param>
        public static Vector2 Max(Vector2 lhs, Vector2 rhs)
        {
            return new Vector2(Math.Max(lhs.x, rhs.x), Math.Max(lhs.y, rhs.y));
        }

        /// <summary>
        /// 绕原点旋转指定角度 (度)
        /// </summary>
        /// <param name="v">输入向量</param>
        /// <param name="degrees">旋转角度</param>
        public static Vector2 Rotate(Vector2 v, FixedPoint degrees)
        {
            FixedPoint rad = Math.DegreesToRadians(degrees);
            FixedPoint cos = Math.Cos(rad);
            FixedPoint sin = Math.Sin(rad);
            return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
        }

        /// <summary>
        /// 平滑阻尼 (无速度限制)
        /// </summary>
        /// <param name="current">当前位置</param>
        /// <param name="target">目标位置</param>
        /// <param name="currentVelocity">当前速度 (由函数修改)</param>
        /// <param name="smoothTime">到达目标的近似时间</param>
        /// <param name="fixedDeltaTime">帧同步时间增量 (不要使用 Time.deltaTime)</param>
        public static Vector2 SmoothDamp(Vector2 current, Vector2 target,
            ref Vector2 currentVelocity, FixedPoint smoothTime, FixedPoint fixedDeltaTime)
        {
            return SmoothDamp(current, target, ref currentVelocity, smoothTime, fixedDeltaTime,
                new FixedPoint(999999));
        }

        /// <summary>
        /// 平滑阻尼
        /// </summary>
        /// <param name="current">当前位置</param>
        /// <param name="target">目标位置</param>
        /// <param name="currentVelocity">当前速度 (由函数修改)</param>
        /// <param name="smoothTime">到达目标的近似时间</param>
        /// <param name="fixedDeltaTime">帧同步时间增量 (不要使用 Time.deltaTime)</param>
        /// <param name="maxSpeed">最大速度限制</param>
        public static Vector2 SmoothDamp(Vector2 current, Vector2 target,
            ref Vector2 currentVelocity, FixedPoint smoothTime, FixedPoint fixedDeltaTime,
            FixedPoint maxSpeed)
        {
            // 弹簧阻尼系统: omega = 2/smoothTime 为角频率
            smoothTime = Math.Max(FixedPoint.Epsilon, smoothTime);
            FixedPoint omega = FixedPoint.Two / smoothTime;
            FixedPoint dampingInput = omega * fixedDeltaTime;
            FixedPoint decayFactor = FixedPoint.One /
                             (FixedPoint.One + dampingInput + Math.SmoothDampC2 * dampingInput * dampingInput
                              + Math.SmoothDampC3 * dampingInput * dampingInput * dampingInput);
            FixedPoint dx = current.x - target.x;
            FixedPoint dy = current.y - target.y;
            Vector2 original = target;
            FixedPoint maxChange = maxSpeed * smoothTime;
            FixedPoint maxChangeSqr = maxChange * maxChange;
            FixedPoint sqrDist = dx * dx + dy * dy;
            if (sqrDist > maxChangeSqr)
            {
                FixedPoint dist = Math.Sqrt(sqrDist);
                dx = dx / dist * maxChange;
                dy = dy / dist * maxChange;
            }

            target.x = current.x - dx;
            target.y = current.y - dy;
            FixedPoint tempX = (currentVelocity.x + omega * dx) * fixedDeltaTime;
            FixedPoint tempY = (currentVelocity.y + omega * dy) * fixedDeltaTime;
            currentVelocity.x = (currentVelocity.x - omega * tempX) * decayFactor;
            currentVelocity.y = (currentVelocity.y - omega * tempY) * decayFactor;
            FixedPoint outX = target.x + (dx + tempX) * decayFactor;
            FixedPoint outY = target.y + (dy + tempY) * decayFactor;
            FixedPoint origDx = original.x - current.x;
            FixedPoint origDy = original.y - current.y;
            FixedPoint outDx = outX - original.x;
            FixedPoint outDy = outY - original.y;
            if (origDx * outDx + origDy * outDy > FixedPoint.Zero)
            {
                outX = original.x;
                outY = original.y;
                currentVelocity.x = (outX - original.x) / fixedDeltaTime;
                currentVelocity.y = (outY - original.y) / fixedDeltaTime;
            }

            return new Vector2(outX, outY);
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
                "({0}, {1})",
                x.RawDouble.ToString(outputFormat, outputFormatProvider),
                y.RawDouble.ToString(outputFormat, outputFormatProvider));
        }

        /// <summary>
        /// 返回哈希码
        /// </summary>
        public override int GetHashCode() => x.GetHashCode() ^ (y.GetHashCode() << 2);

        /// <summary>
        /// 判断与任意对象是否相等
        /// </summary>
        /// <param name="other">要比较的对象</param>
        public override bool Equals(object? other)
        {
            return other is Vector2 v && Equals(v);
        }

        /// <summary>
        /// 判断与另一个向量是否相等
        /// </summary>
        /// <param name="other">要比较的向量</param>
        public bool Equals(Vector2 other) => x == other.x && y == other.y;

        #endregion

        #region 运算符

        /// <summary>
        /// 向量加法
        /// </summary>
        /// <param name="a">左操作数</param>
        /// <param name="b">右操作数</param>
        public static Vector2 operator +(Vector2 a, Vector2 b) => new Vector2(a.x + b.x, a.y + b.y);

        /// <summary>
        /// 向量减法
        /// </summary>
        /// <param name="a">左操作数</param>
        /// <param name="b">右操作数</param>
        public static Vector2 operator -(Vector2 a, Vector2 b) => new Vector2(a.x - b.x, a.y - b.y);

        /// <summary>
        /// 分量相乘
        /// </summary>
        /// <param name="a">左操作数</param>
        /// <param name="b">右操作数</param>
        public static Vector2 operator *(Vector2 a, Vector2 b) => new Vector2(a.x * b.x, a.y * b.y);

        /// <summary>
        /// 分量相除
        /// </summary>
        /// <param name="a">左操作数</param>
        /// <param name="b">右操作数</param>
        public static Vector2 operator /(Vector2 a, Vector2 b) => new Vector2(a.x / b.x, a.y / b.y);

        /// <summary>
        /// 取负
        /// </summary>
        /// <param name="a">输入向量</param>
        public static Vector2 operator -(Vector2 a) => new Vector2(-a.x, -a.y);

        /// <summary>
        /// 标量右乘
        /// </summary>
        /// <param name="a">输入向量</param>
        /// <param name="d">标量</param>
        public static Vector2 operator *(Vector2 a, FixedPoint d) => new Vector2(a.x * d, a.y * d);

        /// <summary>
        /// 标量左乘
        /// </summary>
        /// <param name="d">标量</param>
        /// <param name="a">输入向量</param>
        public static Vector2 operator *(FixedPoint d, Vector2 a) => new Vector2(a.x * d, a.y * d);

        /// <summary>
        /// 标量除法
        /// </summary>
        /// <param name="a">输入向量</param>
        /// <param name="d">标量</param>
        public static Vector2 operator /(Vector2 a, FixedPoint d) => new Vector2(a.x / d, a.y / d);

        /// <summary>
        /// 精确相等比较 (与 Equals / GetHashCode 语义一致)
        /// </summary>
        /// <param name="lhs">左操作数</param>
        /// <param name="rhs">右操作数</param>
        public static bool operator ==(Vector2 lhs, Vector2 rhs)
        {
            return lhs.x == rhs.x && lhs.y == rhs.y;
        }

        /// <summary>
        /// 不等比较
        /// </summary>
        /// <param name="lhs">左操作数</param>
        /// <param name="rhs">右操作数</param>
        public static bool operator !=(Vector2 lhs, Vector2 rhs) => !(lhs == rhs);

        #endregion

        #region 类型转换

        /// <summary>
        /// 隐式转换为 Vector3 (z = 0)
        /// </summary>
        /// <param name="v">输入向量</param>
        public static implicit operator Vector3(Vector2 v) => new Vector3(v.x, v.y, FixedPoint.Zero);

        #endregion
    }
}
