using System;
using System.Globalization;

namespace Tao.FixedPoint
{
    /// <summary>
    /// 四维定点数向量，保证跨平台确定性
    /// </summary>
    public struct Vector4 : IEquatable<Vector4>, IFormattable
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
        /// W 分量
        /// </summary>
        public FixedPoint w;

        /// <summary>
        /// 零向量缓存
        /// </summary>
        private static readonly Vector4 _zeroVector = new Vector4(FixedPoint.Zero, FixedPoint.Zero, FixedPoint.Zero, FixedPoint.Zero);

        /// <summary>
        /// 单位向量缓存
        /// </summary>
        private static readonly Vector4 _oneVector = new Vector4(FixedPoint.One, FixedPoint.One, FixedPoint.One, FixedPoint.One);

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
                _ => throw new IndexOutOfRangeException("Invalid Vector4 index!"),
            };
            set
            {
                switch (index)
                {
                    case 0: x = value; break;
                    case 1: y = value; break;
                    case 2: z = value; break;
                    case 3: w = value; break;
                    default: throw new IndexOutOfRangeException("Invalid Vector4 index!");
                }
            }
        }

        /// <summary>
        /// 单位化向量 (只读)
        /// </summary>
        public Vector4 Normalized
        {
            get
            {
                Vector4 result = new Vector4(x, y, z, w);
                result.Normalize();
                return result;
            }
        }

        /// <summary>
        /// 向量长度 (只读)
        /// </summary>
        public FixedPoint Magnitude => Math.Sqrt(x * x + y * y + z * z + w * w);

        /// <summary>
        /// 向量长度的平方 (只读，避免开方)
        /// </summary>
        public FixedPoint SqrMagnitude => x * x + y * y + z * z + w * w;

        /// <summary>
        /// 零向量 (0, 0, 0, 0)
        /// </summary>
        public static Vector4 Zero => _zeroVector;

        /// <summary>
        /// 单位向量 (1, 1, 1, 1)
        /// </summary>
        public static Vector4 One => _oneVector;

        #endregion

        #region 构造函数

        /// <summary>
        /// 从四个定点数构造
        /// </summary>
        /// <param name="x">X 分量</param>
        /// <param name="y">Y 分量</param>
        /// <param name="z">Z 分量</param>
        /// <param name="w">W 分量</param>
        public Vector4(FixedPoint x, FixedPoint y, FixedPoint z, FixedPoint w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        /// <summary>
        /// 从三个定点数构造 (w = 0)
        /// </summary>
        /// <param name="x">X 分量</param>
        /// <param name="y">Y 分量</param>
        /// <param name="z">Z 分量</param>
        public Vector4(FixedPoint x, FixedPoint y, FixedPoint z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            w = FixedPoint.Zero;
        }

        /// <summary>
        /// 从两个定点数构造 (z = w = 0)
        /// </summary>
        /// <param name="x">X 分量</param>
        /// <param name="y">Y 分量</param>
        public Vector4(FixedPoint x, FixedPoint y)
        {
            this.x = x;
            this.y = y;
            z = FixedPoint.Zero;
            w = FixedPoint.Zero;
        }

        /// <summary>
        /// 从四个浮点数构造 (仅用于初始化常量，运行时逻辑请使用 FixedPoint 构造)
        /// </summary>
        /// <param name="x">X 分量</param>
        /// <param name="y">Y 分量</param>
        /// <param name="z">Z 分量</param>
        /// <param name="w">W 分量</param>
        public Vector4(float x, float y, float z, float w)
        {
            this.x = new FixedPoint((long)System.Math.Round(x * FixedPoint.MULTIPLE));
            this.y = new FixedPoint((long)System.Math.Round(y * FixedPoint.MULTIPLE));
            this.z = new FixedPoint((long)System.Math.Round(z * FixedPoint.MULTIPLE));
            this.w = new FixedPoint((long)System.Math.Round(w * FixedPoint.MULTIPLE));
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
        public void Scale(Vector4 scale)
        {
            x *= scale.x;
            y *= scale.y;
            z *= scale.z;
            w *= scale.w;
        }

        /// <summary>
        /// 点积
        /// </summary>
        /// <param name="a">左向量</param>
        /// <param name="b">右向量</param>
        public static FixedPoint Dot(Vector4 a, Vector4 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
        }

        /// <summary>
        /// 两点之间的距离
        /// </summary>
        /// <param name="a">起点</param>
        /// <param name="b">终点</param>
        public static FixedPoint Distance(Vector4 a, Vector4 b)
        {
            FixedPoint dx = a.x - b.x;
            FixedPoint dy = a.y - b.y;
            FixedPoint dz = a.z - b.z;
            FixedPoint dw = a.w - b.w;
            return Math.Sqrt(dx * dx + dy * dy + dz * dz + dw * dw);
        }

        /// <summary>
        /// 线性插值 (t 限制在 [0,1])
        /// </summary>
        /// <param name="a">起始向量</param>
        /// <param name="b">目标向量</param>
        /// <param name="t">插值系数</param>
        public static Vector4 Lerp(Vector4 a, Vector4 b, FixedPoint t)
        {
            t = Math.Clamp01(t);
            return new Vector4(
                a.x + (b.x - a.x) * t,
                a.y + (b.y - a.y) * t,
                a.z + (b.z - a.z) * t,
                a.w + (b.w - a.w) * t);
        }

        /// <summary>
        /// 线性插值 (t 不限制范围)
        /// </summary>
        /// <param name="a">起始向量</param>
        /// <param name="b">目标向量</param>
        /// <param name="t">插值系数</param>
        public static Vector4 LerpUnclamped(Vector4 a, Vector4 b, FixedPoint t)
        {
            return new Vector4(
                a.x + (b.x - a.x) * t,
                a.y + (b.y - a.y) * t,
                a.z + (b.z - a.z) * t,
                a.w + (b.w - a.w) * t);
        }

        /// <summary>
        /// 向目标移动，每次最多移动 maxDistanceDelta
        /// </summary>
        /// <param name="current">当前向量</param>
        /// <param name="target">目标向量</param>
        /// <param name="maxDistanceDelta">最大移动距离</param>
        public static Vector4 MoveTowards(Vector4 current, Vector4 target, FixedPoint maxDistanceDelta)
        {
            FixedPoint dx = target.x - current.x;
            FixedPoint dy = target.y - current.y;
            FixedPoint dz = target.z - current.z;
            FixedPoint dw = target.w - current.w;
            FixedPoint sqrDist = dx * dx + dy * dy + dz * dz + dw * dw;
            // 已到达 或 剩余距离 ≤ 步长 → 直接返回目标（用平方比较避免开方）
            if (sqrDist == FixedPoint.Zero
                || (maxDistanceDelta >= FixedPoint.Zero && sqrDist <= maxDistanceDelta * maxDistanceDelta))
            {
                return target;
            }

            FixedPoint dist = Math.Sqrt(sqrDist);
            return new Vector4(
                current.x + dx / dist * maxDistanceDelta,
                current.y + dy / dist * maxDistanceDelta,
                current.z + dz / dist * maxDistanceDelta,
                current.w + dw / dist * maxDistanceDelta);
        }

        /// <summary>
        /// 单位化 (静态)
        /// </summary>
        /// <param name="value">输入向量</param>
        public static Vector4 Normalize(Vector4 value)
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
        public static Vector4 Scale(Vector4 a, Vector4 b)
        {
            return new Vector4(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);
        }

        /// <summary>
        /// 向量投影到另一个向量上
        /// </summary>
        /// <param name="vector">输入向量</param>
        /// <param name="onNormal">目标向量</param>
        public static Vector4 Project(Vector4 vector, Vector4 onNormal)
        {
            FixedPoint sqrMag = Dot(onNormal, onNormal);
            if (sqrMag <= FixedPoint.Zero)
            {
                return _zeroVector;
            }

            // 投影公式: proj = n × (v·n / |n|²)
            FixedPoint dot = Dot(vector, onNormal);
            return new Vector4(
                onNormal.x * dot / sqrMag,
                onNormal.y * dot / sqrMag,
                onNormal.z * dot / sqrMag,
                onNormal.w * dot / sqrMag);
        }

        /// <summary>
        /// 限制向量长度
        /// </summary>
        /// <param name="vector">输入向量</param>
        /// <param name="maxLength">最大长度</param>
        public static Vector4 ClampMagnitude(Vector4 vector, FixedPoint maxLength)
        {
            FixedPoint sqrMag = vector.SqrMagnitude;
            if (sqrMag > maxLength * maxLength)
            {
                FixedPoint mag = Math.Sqrt(sqrMag);
                return new Vector4(
                    vector.x / mag * maxLength,
                    vector.y / mag * maxLength,
                    vector.z / mag * maxLength,
                    vector.w / mag * maxLength);
            }

            return vector;
        }

        /// <summary>
        /// 返回分量最小值组合的向量
        /// </summary>
        /// <param name="lhs">左向量</param>
        /// <param name="rhs">右向量</param>
        public static Vector4 Min(Vector4 lhs, Vector4 rhs)
        {
            return new Vector4(
                Math.Min(lhs.x, rhs.x), Math.Min(lhs.y, rhs.y),
                Math.Min(lhs.z, rhs.z), Math.Min(lhs.w, rhs.w));
        }

        /// <summary>
        /// 返回分量最大值组合的向量
        /// </summary>
        /// <param name="lhs">左向量</param>
        /// <param name="rhs">右向量</param>
        public static Vector4 Max(Vector4 lhs, Vector4 rhs)
        {
            return new Vector4(
                Math.Max(lhs.x, rhs.x), Math.Max(lhs.y, rhs.y),
                Math.Max(lhs.z, rhs.z), Math.Max(lhs.w, rhs.w));
        }

        /// <summary>
        /// 返回默认格式字符串
        /// </summary>
        public override string ToString() => ToString(null, null);

        /// <summary>
        /// 使用指定格式返回字符串
        /// </summary>
        /// <param name="format">格式字符串</param>
        public string ToString(string format) => ToString(format, null);

        /// <summary>
        /// 使用指定格式和格式提供程序返回字符串
        /// </summary>
        /// <param name="format">格式字符串</param>
        /// <param name="formatProvider">格式提供程序</param>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            string outputFormat = format;
            if (string.IsNullOrEmpty(outputFormat))
            {
                outputFormat = "F2";
            }

            IFormatProvider outputFormatProvider = formatProvider;
            if (outputFormatProvider == null)
            {
                outputFormatProvider = CultureInfo.InvariantCulture.NumberFormat;
            }

            return string.Format(outputFormatProvider,
                "({0}, {1}, {2}, {3})",
                x.RawDouble.ToString(outputFormat, outputFormatProvider),
                y.RawDouble.ToString(outputFormat, outputFormatProvider),
                z.RawDouble.ToString(outputFormat, outputFormatProvider),
                w.RawDouble.ToString(outputFormat, outputFormatProvider));
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
        public override bool Equals(object other)
        {
            return other is Vector4 v && Equals(v);
        }

        /// <summary>
        /// 判断与另一个向量是否相等
        /// </summary>
        /// <param name="other">要比较的向量</param>
        public bool Equals(Vector4 other) => x == other.x && y == other.y && z == other.z && w == other.w;

        #endregion

        #region 运算符

        /// <summary>
        /// 向量加法
        /// </summary>
        /// <param name="a">左操作数</param>
        /// <param name="b">右操作数</param>
        public static Vector4 operator +(Vector4 a, Vector4 b) => new Vector4(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);

        /// <summary>
        /// 向量减法
        /// </summary>
        /// <param name="a">左操作数</param>
        /// <param name="b">右操作数</param>
        public static Vector4 operator -(Vector4 a, Vector4 b) => new Vector4(a.x - b.x, a.y - b.y, a.z - b.z, a.w - b.w);

        /// <summary>
        /// 取负
        /// </summary>
        /// <param name="a">输入向量</param>
        public static Vector4 operator -(Vector4 a) => new Vector4(-a.x, -a.y, -a.z, -a.w);

        /// <summary>
        /// 分量相乘
        /// </summary>
        /// <param name="a">左操作数</param>
        /// <param name="b">右操作数</param>
        public static Vector4 operator *(Vector4 a, Vector4 b) => new Vector4(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);

        /// <summary>
        /// 分量相除
        /// </summary>
        /// <param name="a">左操作数</param>
        /// <param name="b">右操作数</param>
        public static Vector4 operator /(Vector4 a, Vector4 b) => new Vector4(a.x / b.x, a.y / b.y, a.z / b.z, a.w / b.w);

        /// <summary>
        /// 标量右乘
        /// </summary>
        /// <param name="a">输入向量</param>
        /// <param name="d">标量</param>
        public static Vector4 operator *(Vector4 a, FixedPoint d) => new Vector4(a.x * d, a.y * d, a.z * d, a.w * d);

        /// <summary>
        /// 标量左乘
        /// </summary>
        /// <param name="d">标量</param>
        /// <param name="a">输入向量</param>
        public static Vector4 operator *(FixedPoint d, Vector4 a) => new Vector4(a.x * d, a.y * d, a.z * d, a.w * d);

        /// <summary>
        /// 标量除法
        /// </summary>
        /// <param name="a">输入向量</param>
        /// <param name="d">标量</param>
        public static Vector4 operator /(Vector4 a, FixedPoint d) => new Vector4(a.x / d, a.y / d, a.z / d, a.w / d);

        /// <summary>
        /// 精确相等比较 (与 Equals / GetHashCode 语义一致)
        /// </summary>
        /// <param name="lhs">左操作数</param>
        /// <param name="rhs">右操作数</param>
        public static bool operator ==(Vector4 lhs, Vector4 rhs)
        {
            return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z && lhs.w == rhs.w;
        }

        /// <summary>
        /// 不等比较
        /// </summary>
        /// <param name="lhs">左操作数</param>
        /// <param name="rhs">右操作数</param>
        public static bool operator !=(Vector4 lhs, Vector4 rhs) => !(lhs == rhs);

        #endregion

        #region 类型转换

        /// <summary>
        /// 从 Vector3 隐式转换 (w = 0)
        /// </summary>
        /// <param name="v">输入三维向量</param>
        public static implicit operator Vector4(Vector3 v) => new Vector4(v.x, v.y, v.z, FixedPoint.Zero);

        /// <summary>
        /// 从 Vector2 隐式转换 (z = w = 0)
        /// </summary>
        /// <param name="v">输入二维向量</param>
        public static implicit operator Vector4(Vector2 v) => new Vector4(v.x, v.y, FixedPoint.Zero, FixedPoint.Zero);

        /// <summary>
        /// 向 Vector3 隐式转换 (丢弃 w)
        /// </summary>
        /// <param name="v">输入四维向量</param>
        public static implicit operator Vector3(Vector4 v) => new Vector3(v.x, v.y, v.z);

        /// <summary>
        /// 向 Vector2 隐式转换 (丢弃 z, w)
        /// </summary>
        /// <param name="v">输入四维向量</param>
        public static implicit operator Vector2(Vector4 v) => new Vector2(v.x, v.y);

        #endregion
    }
}
