using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Tao.FixedPoint
{
    /// <summary>
    /// Q10 定点数结构体，所有运算基于整数位移，保证跨平台确定性
    /// 内部以 long 存储放大 MULTIPLE(1024) 倍后的值
    /// </summary>
    public struct FixedPoint : IEquatable<FixedPoint>, IComparable<FixedPoint>
    {
        #region 常量

        /// <summary>
        /// 内部值上限 (对应外部值 int.MaxValue，正负对称)
        /// </summary>
        public const long MAX_VALUE = (long)int.MaxValue << BIT_MOVE_COUNT;

        /// <summary>
        /// 内部值下限 (= -MAX_VALUE，正负对称)
        /// </summary>
        public const long MIN_VALUE = -MAX_VALUE;

        /// <summary>
        /// 位移次数 (Q10 格式)
        /// </summary>
        internal const int BIT_MOVE_COUNT = 10;

        /// <summary>
        /// 放大倍率 2^10 = 1024
        /// </summary>
        public const int MULTIPLE = 1 << BIT_MOVE_COUNT;

        /// <summary>
        /// 圆周率 π ≈ 3.142 (Q10 原始值 3217)
        /// </summary>
        public static readonly FixedPoint Pi = new FixedPoint(3217L);

        /// <summary>
        /// 双圆周率 2π ≈ 6.283 (Q10 原始值 6434)
        /// </summary>
        public static readonly FixedPoint TwoPi = new FixedPoint(6434L);

        /// <summary>
        /// 半圆周率 π/2 ≈ 1.571 (Q10 原始值 1609，与 Acos 查找表对齐)
        /// </summary>
        public static readonly FixedPoint PiOver2 = new FixedPoint(1609L);

        /// <summary>
        /// 单位一
        /// </summary>
        public static readonly FixedPoint One = new FixedPoint(1);

        /// <summary>
        /// 二
        /// </summary>
        public static readonly FixedPoint Two = new FixedPoint(2);

        /// <summary>
        /// 负一
        /// </summary>
        public static readonly FixedPoint NegativeOne = new FixedPoint(-1);

        /// <summary>
        /// 零
        /// </summary>
        public static readonly FixedPoint Zero = new FixedPoint(0);

        /// <summary>
        /// 最小精度值 (1/MULTIPLE ≈ 0.001)
        /// </summary>
        public static readonly FixedPoint Epsilon = new FixedPoint(1L);

        /// <summary>
        /// 可表示的最大值 (= int.MaxValue ≈ 2.147×10⁹)
        /// </summary>
        public static readonly FixedPoint MaxValue = new FixedPoint(MAX_VALUE);

        /// <summary>
        /// 可表示的最小值 (= -int.MaxValue ≈ -2.147×10⁹)
        /// </summary>
        public static readonly FixedPoint MinValue = new FixedPoint(MIN_VALUE);

        #endregion

        #region 字段和属性

        /// <summary>
        /// 内部存储的放大后数值
        /// </summary>
        private readonly long _fixedValue;

        /// <summary>
        /// 放大后的原始数值 (用于定点运算)
        /// </summary>
        public long FixedValue => _fixedValue;

        /// <summary>
        /// 还原后的 double 值 (仅用于表现层，保留3位小数)
        /// </summary>
        public double RawDouble => System.Math.Round(_fixedValue / (double)MULTIPLE, 3);

        /// <summary>
        /// 还原后的 float 值 (仅用于表现层，保留3位小数)
        /// </summary>
        public float RawFloat => (float)System.Math.Round(_fixedValue / (double)MULTIPLE, 3);

        /// <summary>
        /// 还原后的 int 值 (纯整数四舍五入，不经过浮点运算)
        /// </summary>
        public int RawInt
        {
            get
            {
                long abs = _fixedValue >= 0 ? _fixedValue : -_fixedValue;
                int result = (int)((abs + (MULTIPLE >> 1)) / MULTIPLE);
                return _fixedValue >= 0 ? result : -result;
            }
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 从 double 构造 (仅用于初始化常量，运行时逻辑请使用 int 构造)
        /// </summary>
        /// <param name="value">输入的 double 值</param>
        public FixedPoint(double value)
        {
            _fixedValue = Saturate((long)System.Math.Round(value * MULTIPLE));
        }

        /// <summary>
        /// 从 float 构造 (仅用于初始化常量，运行时逻辑请使用 int 构造)
        /// </summary>
        /// <param name="value">输入的 float 值</param>
        public FixedPoint(float value)
        {
            _fixedValue = Saturate((long)System.Math.Round(value * MULTIPLE));
        }

        /// <summary>
        /// 从 int 构造，自动放大 MULTIPLE 倍
        /// </summary>
        /// <param name="value">输入的 int 值</param>
        public FixedPoint(int value)
        {
            _fixedValue = Saturate((long)value << BIT_MOVE_COUNT);
        }

        /// <summary>
        /// 从已放大的 long 构造，不再缩放 (内部运算专用)
        /// </summary>
        /// <param name="scaledValue">必须是已乘以 MULTIPLE 的值</param>
        public FixedPoint(long scaledValue)
        {
            _fixedValue = Saturate(scaledValue);
        }

        #endregion

        #region 类型转换

        /// <summary>
        /// 显式转换为 double (还原后的值)
        /// </summary>
        /// <param name="value">定点数值</param>
        public static explicit operator double(FixedPoint value) => value.RawDouble;

        /// <summary>
        /// 显式转换为 float (还原后的值)
        /// </summary>
        /// <param name="value">定点数值</param>
        public static explicit operator float(FixedPoint value) => value.RawFloat;

        /// <summary>
        /// 显式转换为 int (四舍五入还原后的值)
        /// </summary>
        /// <param name="value">定点数值</param>
        public static explicit operator int(FixedPoint value) => value.RawInt;

        /// <summary>
        /// 显式转换为 long (返回内部放大值)
        /// </summary>
        /// <param name="value">定点数值</param>
        public static explicit operator long(FixedPoint value) => value._fixedValue;

        /// <summary>
        /// double 显式转换为定点数 (防止逻辑层误用运行时浮点值)
        /// </summary>
        /// <param name="value">double 数值</param>
        public static explicit operator FixedPoint(double value) => new FixedPoint(value);

        /// <summary>
        /// float 显式转换为定点数 (防止逻辑层误用运行时浮点值)
        /// </summary>
        /// <param name="value">float 数值</param>
        public static explicit operator FixedPoint(float value) => new FixedPoint(value);

        /// <summary>
        /// int 隐式转换为定点数 (自动放大)
        /// </summary>
        /// <param name="value">int 数值</param>
        public static implicit operator FixedPoint(int value) => new FixedPoint(value);

        /// <summary>
        /// long 隐式转换，传入值视为已放大的原始值 (与 int 行为不同!)
        /// </summary>
        /// <param name="scaledValue">已放大的 long 原始值</param>
        public static implicit operator FixedPoint(long scaledValue) => new FixedPoint(scaledValue);

        #endregion

        #region 运算符

        /// <summary>
        /// 定点数加法
        /// </summary>
        /// <param name="a">左操作数</param>
        /// <param name="b">右操作数</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPoint operator +(FixedPoint a, FixedPoint b)
        {
            return new FixedPoint(Saturate(a._fixedValue + b._fixedValue));
        }

        /// <summary>
        /// 定点数减法
        /// </summary>
        /// <param name="a">左操作数</param>
        /// <param name="b">右操作数</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPoint operator -(FixedPoint a, FixedPoint b)
        {
            return new FixedPoint(Saturate(a._fixedValue - b._fixedValue));
        }

        /// <summary>
        /// 取负
        /// </summary>
        /// <param name="a">输入值</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPoint operator -(FixedPoint a)
        {
            return new FixedPoint(Saturate(-a._fixedValue));
        }

        /// <summary>
        /// 定点数乘法
        /// </summary>
        /// <param name="a">左操作数</param>
        /// <param name="b">右操作数</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPoint operator *(FixedPoint a, FixedPoint b)
        {
            long leftValue = a._fixedValue;
            long rightValue = b._fixedValue;

            if (leftValue == 0 || rightValue == 0)
            {
                return Zero;
            }

            // 溢出检测：|left × right| 是否超出 long 范围
            long absLeft = leftValue > 0 ? leftValue : -leftValue;
            long absRight = rightValue > 0 ? rightValue : -rightValue;
            if (absLeft > long.MaxValue / absRight)
            {
                // 中间乘积溢出 long → 结果必然超出有效范围 → 按符号饱和
                return new FixedPoint((leftValue ^ rightValue) >= 0 ? MAX_VALUE : MIN_VALUE);
            }

            // 乘法结果放大了两次 MULTIPLE，需缩回一次
            return new FixedPoint(Saturate(leftValue * rightValue / MULTIPLE));
        }

        /// <summary>
        /// 定点数除法 (除零和溢出均做饱和处理)
        /// </summary>
        /// <param name="a">被除数</param>
        /// <param name="b">除数</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPoint operator /(FixedPoint a, FixedPoint b)
        {
            if (b._fixedValue == 0)
            {
                // 除零饱和：0/0 → 0，非零/0 → 按被除数符号饱和
                if (a._fixedValue == 0)
                {
                    return Zero;
                }

                return new FixedPoint(a._fixedValue > 0 ? MAX_VALUE : MIN_VALUE);
            }

            // 有效范围内的值左移 BIT_MOVE_COUNT 后不会溢出 long
            // (MAX_VALUE << 10 ≈ 2.25×10¹⁵ << long.MaxValue ≈ 9.2×10¹⁸)
            long shifted = a._fixedValue << BIT_MOVE_COUNT;
            return new FixedPoint(Saturate(shifted / b._fixedValue));
        }

        /// <summary>
        /// 定点数取模 (除零返回零)
        /// </summary>
        /// <param name="a">被除数</param>
        /// <param name="b">除数</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPoint operator %(FixedPoint a, FixedPoint b)
        {
            if (b._fixedValue == 0)
            {
                return Zero;
            }

            return new FixedPoint(a._fixedValue % b._fixedValue);
        }

        /// <summary>
        /// 左移
        /// </summary>
        /// <param name="a">输入值</param>
        /// <param name="count">位移位数</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPoint operator <<(FixedPoint a, int count)
        {
            long fixedValue = a._fixedValue;
            if (fixedValue == 0)
            {
                return a;
            }

            // 防止 long 溢出导致符号翻转
            if (count >= 63
                || (fixedValue > 0 && fixedValue > MAX_VALUE >> count)
                || (fixedValue < 0 && fixedValue < MIN_VALUE >> count))
            {
                return new FixedPoint(fixedValue > 0 ? MAX_VALUE : MIN_VALUE);
            }

            return new FixedPoint(fixedValue << count);
        }

        /// <summary>
        /// 右移
        /// </summary>
        /// <param name="a">输入值</param>
        /// <param name="count">位移位数</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPoint operator >> (FixedPoint a, int count)
        {
            return new FixedPoint(a._fixedValue >> count);
        }

        /// <summary>
        /// 相等比较
        /// </summary>
        /// <param name="a">左操作数</param>
        /// <param name="b">右操作数</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(FixedPoint a, FixedPoint b) => a._fixedValue == b._fixedValue;

        /// <summary>
        /// 不等比较
        /// </summary>
        /// <param name="a">左操作数</param>
        /// <param name="b">右操作数</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(FixedPoint a, FixedPoint b) => a._fixedValue != b._fixedValue;

        /// <summary>
        /// 大于比较
        /// </summary>
        /// <param name="a">左操作数</param>
        /// <param name="b">右操作数</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >(FixedPoint a, FixedPoint b) => a._fixedValue > b._fixedValue;

        /// <summary>
        /// 小于比较
        /// </summary>
        /// <param name="a">左操作数</param>
        /// <param name="b">右操作数</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <(FixedPoint a, FixedPoint b) => a._fixedValue < b._fixedValue;

        /// <summary>
        /// 大于等于比较
        /// </summary>
        /// <param name="a">左操作数</param>
        /// <param name="b">右操作数</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator >=(FixedPoint a, FixedPoint b) => a._fixedValue >= b._fixedValue;

        /// <summary>
        /// 小于等于比较
        /// </summary>
        /// <param name="a">左操作数</param>
        /// <param name="b">右操作数</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator <=(FixedPoint a, FixedPoint b) => a._fixedValue <= b._fixedValue;

        #endregion

        #region 公共方法

        /// <summary>
        /// 判断与另一个定点数是否相等
        /// </summary>
        /// <param name="other">要比较的定点数</param>
        public readonly bool Equals(FixedPoint other) => _fixedValue == other._fixedValue;

        /// <summary>
        /// 判断与任意对象是否相等
        /// </summary>
        /// <param name="obj">要比较的对象</param>
        public override bool Equals(object obj) => obj is FixedPoint other && _fixedValue == other._fixedValue;

        /// <summary>
        /// 返回哈希码
        /// </summary>
        public override int GetHashCode() => _fixedValue.GetHashCode();

        /// <summary>
        /// 返回字符串表示
        /// </summary>
        public override string ToString() => RawFloat.ToString(CultureInfo.InvariantCulture);

        /// <summary>
        /// 与另一个定点数比较大小
        /// </summary>
        /// <param name="other">要比较的定点数</param>
        public readonly int CompareTo(FixedPoint other) => _fixedValue.CompareTo(other._fixedValue);

        /// <summary>
        /// 与任意对象比较大小
        /// </summary>
        /// <param name="obj">要比较的对象</param>
        public readonly int CompareTo(object obj)
        {
            if (obj is FixedPoint other)
            {
                return CompareTo(other);
            }

            throw new ArgumentException("对象不是 FixedPoint 类型", nameof(obj));
        }

        #endregion

        #region 内部辅助

        /// <summary>
        /// 饱和钳位：超出有效范围时钳位到边界值 (保证不回绕、不崩溃)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static long Saturate(long value)
        {
            if (value > MAX_VALUE)
            {
                return MAX_VALUE;
            }

            if (value < MIN_VALUE)
            {
                return MIN_VALUE;
            }

            return value;
        }

        #endregion
    }
}