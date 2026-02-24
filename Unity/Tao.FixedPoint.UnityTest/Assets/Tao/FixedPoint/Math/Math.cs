using System;
using System.Runtime.CompilerServices;

namespace Tao.FixedPoint
{
    /// <summary>
    /// 确定性定点数学函数库，参考 Unity Mathf / System.Math
    /// 所有运算基于整数，保证跨平台确定性
    /// </summary>
    public static class Math
    {
        #region 常量

        /// <summary>
        /// 角度转弧度系数 (π/180)
        /// </summary>
        public static readonly FixedPoint Deg2Rad = new FixedPoint(18L);

        /// <summary>
        /// 弧度转角度系数 (180/π)
        /// </summary>
        public static readonly FixedPoint Rad2Deg = new FixedPoint(58671L);

        /// <summary>
        /// 查找表输出精度因子 (表内值 = 真实值 × LOOKUP_FACTOR)
        /// </summary>
        private const int LOOKUP_FACTOR = 10000;

        /// <summary>
        /// 三角函数查表周期 (= MULTIPLE × 2π × LOOKUP_FACTOR)
        /// </summary>
        private const long TRIG_TABLE_PERIOD = FixedPoint.MULTIPLE * 62832L;

        /// <summary>
        /// 自然对数 ln(2) ≈ 0.693 (Q10 原始值 710)
        /// </summary>
        internal static readonly FixedPoint Ln2 = new FixedPoint(710L);

        /// <summary>
        /// 常用对数 log10(2) ≈ 0.301 (Q10 原始值 308)
        /// </summary>
        internal static readonly FixedPoint Log10Of2 = new FixedPoint(308L);

        /// <summary>
        /// SmoothDamp 二次衰减系数 (≈0.48, 预计算: round(0.48 × 1024) = 492)
        /// </summary>
        internal static readonly FixedPoint SmoothDampC2 = new FixedPoint(492L);

        /// <summary>
        /// SmoothDamp 三次衰减系数 (≈0.235, 预计算: round(0.235 × 1024) = 241)
        /// </summary>
        internal static readonly FixedPoint SmoothDampC3 = new FixedPoint(241L);

        /// <summary>
        /// 小数部分掩码 (MULTIPLE - 1 = 0x3FF)，用于 Floor/Ceiling/Truncate 位操作
        /// </summary>
        internal const long FRAC_MASK = FixedPoint.MULTIPLE - 1;

        /// <summary>
        /// 预缓存定点数 3 (SmoothStep 与 Horner 展开系数)
        /// </summary>
        internal static readonly FixedPoint Three = new FixedPoint(3);

        /// <summary>
        /// 预缓存定点数 4 (Horner 展开系数)
        /// </summary>
        internal static readonly FixedPoint Four = new FixedPoint(4);

        /// <summary>
        /// 预缓存定点数 5 (Horner 展开系数)
        /// </summary>
        internal static readonly FixedPoint Five = new FixedPoint(5);

        /// <summary>
        /// 预缓存定点数 180 (角度-弧度转换)
        /// </summary>
        internal static readonly FixedPoint Deg180 = new FixedPoint(180);

        /// <summary>
        /// 预缓存定点数 360 (角度回绕)
        /// </summary>
        internal static readonly FixedPoint Deg360 = new FixedPoint(360);

        #endregion

        #region 基础数学

        /// <summary>
        /// 绝对值
        /// </summary>
        /// <param name="value">输入值</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPoint Abs(FixedPoint value)
        {
            if (value.FixedValue >= 0)
            {
                return value;
            }

            return new FixedPoint(-value.FixedValue);
        }

        /// <summary>
        /// 返回两个值中的较大值
        /// </summary>
        /// <param name="a">第一个值</param>
        /// <param name="b">第二个值</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPoint Max(FixedPoint a, FixedPoint b)
        {
            return a > b ? a : b;
        }

        /// <summary>
        /// 返回两个值中的较小值
        /// </summary>
        /// <param name="a">第一个值</param>
        /// <param name="b">第二个值</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPoint Min(FixedPoint a, FixedPoint b)
        {
            return a < b ? a : b;
        }

        /// <summary>
        /// 符号函数：非负返回 1，负数返回 -1 (与 Unity Mathf.Sign 行为一致)
        /// </summary>
        /// <param name="value">输入值</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPoint Sign(FixedPoint value)
        {
            return value >= FixedPoint.Zero ? FixedPoint.One : FixedPoint.NegativeOne;
        }

        /// <summary>
        /// 幂运算 (快速幂算法，仅支持整数指数)
        /// </summary>
        /// <param name="value">底数</param>
        /// <param name="exponent">整数指数</param>
        public static FixedPoint Pow(FixedPoint value, int exponent)
        {
            if (exponent == 0)
            {
                return FixedPoint.One;
            }

            if (exponent == 1)
            {
                return value;
            }

            if (exponent < 0)
            {
                if (exponent == int.MinValue)
                {
                    return FixedPoint.One / (Pow(value, int.MaxValue) * value);
                }

                return FixedPoint.One / Pow(value, -exponent);
            }

            FixedPoint half = Pow(value, exponent >> 1);
            return (exponent & 1) == 1 ? value * half * half : half * half;
        }

        /// <summary>
        /// 幂运算 (支持定点指数，底数必须为正)
        /// 通过恒等式 b^e = e^(e·ln(b)) 实现
        /// </summary>
        /// <param name="value">底数 (必须为正)</param>
        /// <param name="exponent">定点数指数</param>
        public static FixedPoint Pow(FixedPoint value, FixedPoint exponent)
        {
            if (exponent == FixedPoint.Zero)
            {
                return FixedPoint.One;
            }

            if (value == FixedPoint.One)
            {
                return FixedPoint.One;
            }

            if (value <= FixedPoint.Zero)
            {
                if (value == FixedPoint.Zero && exponent > FixedPoint.Zero)
                {
                    return FixedPoint.Zero;
                }

                throw new ArgumentOutOfRangeException(nameof(value), "定点指数幂的底数必须为正");
            }

            return Exp(exponent * Ln(value));
        }

        /// <summary>
        /// 平方根 (牛顿迭代法，位长度初始估值 + 固定 8 次迭代)
        /// 直接对原始 long 值运算，避免 FixedPoint 运算符开销
        /// </summary>
        /// <param name="value">输入值</param>
        public static FixedPoint Sqrt(FixedPoint value)
        {
            if (value.FixedValue < 0)
            {
                throw new ArithmeticException("不能对负数求平方根");
            }

            if (value.FixedValue == 0)
            {
                return FixedPoint.Zero;
            }

            long v = value.FixedValue;

            // 用位长度估算初始值: sqrt(v × MULTIPLE) ≈ 2^((bits + BIT_MOVE_COUNT) / 2)
            // 使任意输入的初始偏差控制在 2 倍以内，保证快速收敛
            int bits = 0;
            long temp = v;
            while (temp > 0) { bits++; temp >>= 1; }
            long estimate = 1L << ((bits + FixedPoint.BIT_MOVE_COUNT) / 2);

            // 牛顿迭代: e(n+1) = (e(n) + v × MULTIPLE / e(n)) / 2
            // 二次收敛下 8 次迭代远超 Q10 的 10 位精度需求
            for (int i = 0; i < 8; i++)
            {
                estimate = (estimate + v * FixedPoint.MULTIPLE / estimate) >> 1;
            }

            return new FixedPoint(estimate);
        }

        /// <summary>
        /// 平方根 (牛顿迭代法，指定迭代次数)
        /// </summary>
        /// <param name="value">输入值</param>
        /// <param name="iterations">迭代次数</param>
        public static FixedPoint Sqrt(FixedPoint value, int iterations)
        {
            if (value.FixedValue < 0)
            {
                throw new ArithmeticException("不能对负数求平方根");
            }

            if (value.FixedValue == 0)
            {
                return FixedPoint.Zero;
            }

            long v = value.FixedValue;

            int bits = 0;
            long temp = v;
            while (temp > 0) { bits++; temp >>= 1; }
            long estimate = 1L << ((bits + FixedPoint.BIT_MOVE_COUNT) / 2);

            for (int i = 0; i < iterations; i++)
            {
                estimate = (estimate + v * FixedPoint.MULTIPLE / estimate) >> 1;
            }

            return new FixedPoint(estimate);
        }

        /// <summary>
        /// 倒数平方根 1/√x
        /// </summary>
        /// <param name="value">输入值 (必须为正)</param>
        public static FixedPoint RSqrt(FixedPoint value)
        {
            return FixedPoint.One / Sqrt(value);
        }

        /// <summary>
        /// 倒数 1/x
        /// </summary>
        /// <param name="value">输入值 (不为零)</param>
        public static FixedPoint Rcp(FixedPoint value)
        {
            return FixedPoint.One / value;
        }

        #endregion

        #region 对数与指数函数

        /// <summary>
        /// 以 2 为底的对数 (纯整数迭代平方算法)
        /// </summary>
        /// <param name="value">输入值 (必须为正)</param>
        public static FixedPoint Log2(FixedPoint value)
        {
            if (value.FixedValue <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "对数的输入必须为正数");
            }

            // 将值归一化到 [MULTIPLE, 2*MULTIPLE) 范围，即实际值 [1.0, 2.0)
            long normalizedValue = value.FixedValue;
            int integerPart = 0;
            while (normalizedValue >= 2 * FixedPoint.MULTIPLE)
            {
                normalizedValue >>= 1;
                integerPart++;
            }

            while (normalizedValue < FixedPoint.MULTIPLE)
            {
                normalizedValue <<= 1;
                integerPart--;
            }

            // 迭代平方法求小数部分：每次平方后检查是否 ≥ 2.0
            long fractionalBits = 0;
            for (int i = 0; i < FixedPoint.BIT_MOVE_COUNT; i++)
            {
                normalizedValue = normalizedValue * normalizedValue / FixedPoint.MULTIPLE;
                fractionalBits <<= 1;
                if (normalizedValue >= 2 * FixedPoint.MULTIPLE)
                {
                    normalizedValue >>= 1;
                    fractionalBits |= 1;
                }
            }

            return new FixedPoint((long)integerPart * FixedPoint.MULTIPLE + fractionalBits);
        }

        /// <summary>
        /// 自然对数 ln(x)
        /// </summary>
        /// <param name="value">输入值 (必须为正)</param>
        public static FixedPoint Ln(FixedPoint value)
        {
            return Log2(value) * Ln2;
        }

        /// <summary>
        /// 常用对数 log10(x)
        /// </summary>
        /// <param name="value">输入值 (必须为正)</param>
        public static FixedPoint Log10(FixedPoint value)
        {
            return Log2(value) * Log10Of2;
        }

        /// <summary>
        /// 指数函数 e^x (范围约缩 + Horner 展开，纯整数算法)
        /// </summary>
        /// <param name="value">指数值</param>
        public static FixedPoint Exp(FixedPoint value)
        {
            if (value == FixedPoint.Zero)
            {
                return FixedPoint.One;
            }

            // 范围约缩: x = shiftCount·ln2 + remainder，使 |remainder| ≤ ln2/2 ≈ 0.347
            const long LN2_RAW = 710;
            long inputRaw = value.FixedValue;
            long halfLn2 = LN2_RAW / 2;
            long shiftCount = inputRaw >= 0
                ? (inputRaw + halfLn2) / LN2_RAW
                : (inputRaw - halfLn2) / LN2_RAW;

            FixedPoint remainder = new FixedPoint(inputRaw - shiftCount * LN2_RAW);

            // Horner 展开: e^r ≈ 1 + r(1 + r/2(1 + r/3(1 + r/4(1 + r/5))))
            FixedPoint taylorResult = FixedPoint.One + remainder / Five;
            taylorResult = FixedPoint.One + remainder * taylorResult / Four;
            taylorResult = FixedPoint.One + remainder * taylorResult / Three;
            taylorResult = FixedPoint.One + remainder * taylorResult / FixedPoint.Two;
            taylorResult = FixedPoint.One + remainder * taylorResult;

            // 乘以 2^shiftCount：正数左移，负数右移
            if (shiftCount >= 0)
            {
                int shift = (int)shiftCount;
                if (shift >= 63 || taylorResult.FixedValue > (long.MaxValue >> shift))
                {
                    return FixedPoint.MaxValue;
                }

                long shiftedResult = taylorResult.FixedValue << shift;
                return shiftedResult > FixedPoint.MAX_VALUE
                    ? FixedPoint.MaxValue
                    : new FixedPoint(shiftedResult);
            }
            else
            {
                int shift = (int)(-shiftCount);
                if (shift >= 63)
                {
                    return FixedPoint.Zero;
                }

                return new FixedPoint(taylorResult.FixedValue >> shift);
            }
        }

        /// <summary>
        /// 以 2 为底的指数函数 2^x (通过 Exp(x·ln2) 实现)
        /// </summary>
        /// <param name="value">指数值</param>
        public static FixedPoint Exp2(FixedPoint value)
        {
            return Exp(value * Ln2);
        }

        #endregion

        #region 取整函数

        /// <summary>
        /// 四舍五入
        /// </summary>
        /// <param name="value">输入值</param>
        public static FixedPoint Round(FixedPoint value)
        {
            // 分离整数部分和小数部分，小数绝对值 ≥ 0.5 时向远离零方向进位
            long integerPart = value.FixedValue / FixedPoint.MULTIPLE;
            long remainder = value.FixedValue % FixedPoint.MULTIPLE;
            if (System.Math.Abs(remainder) * 2 >= FixedPoint.MULTIPLE)
            {
                integerPart += System.Math.Sign(remainder);
            }

            return new FixedPoint(integerPart * FixedPoint.MULTIPLE);
        }

        /// <summary>
        /// 向下取整 (向负无穷方向)
        /// 利用二补码特性: 位与 ~FRAC_MASK 天然向负无穷取整
        /// </summary>
        /// <param name="value">输入值</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPoint Floor(FixedPoint value)
        {
            return new FixedPoint(value.FixedValue & ~FRAC_MASK);
        }

        /// <summary>
        /// 向上取整 (向正无穷方向)
        /// </summary>
        /// <param name="value">输入值</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPoint Ceiling(FixedPoint value)
        {
            long v = value.FixedValue;
            return (v & FRAC_MASK) == 0
                ? value
                : new FixedPoint((v & ~FRAC_MASK) + FixedPoint.MULTIPLE);
        }

        /// <summary>
        /// 向零取整 (截断小数部分)
        /// </summary>
        /// <param name="value">输入值</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPoint Truncate(FixedPoint value)
        {
            return new FixedPoint(value.FixedValue / FixedPoint.MULTIPLE * FixedPoint.MULTIPLE);
        }

        /// <summary>
        /// 取小数部分 (返回值始终在 [0, 1) 范围内)
        /// </summary>
        /// <param name="value">输入值</param>
        public static FixedPoint Fract(FixedPoint value)
        {
            return value - Floor(value);
        }

        /// <summary>
        /// 四舍五入为 int
        /// </summary>
        /// <param name="value">输入值</param>
        public static int RoundToInt(FixedPoint value) => Round(value).RawInt;

        /// <summary>
        /// 向下取整为 int
        /// </summary>
        /// <param name="value">输入值</param>
        public static int FloorToInt(FixedPoint value) => Floor(value).RawInt;

        /// <summary>
        /// 向上取整为 int
        /// </summary>
        /// <param name="value">输入值</param>
        public static int CeilToInt(FixedPoint value) => Ceiling(value).RawInt;

        #endregion

        #region 钳制函数

        /// <summary>
        /// 将值限制在 [min, max] 范围内
        /// </summary>
        /// <param name="value">输入值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPoint Clamp(FixedPoint value, FixedPoint min, FixedPoint max)
        {
            return value < min ? min : value > max ? max : value;
        }

        /// <summary>
        /// 将 int 值限制在 [min, max] 范围内
        /// </summary>
        /// <param name="value">输入值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Clamp(int value, int min, int max)
        {
            if (value < min)
            {
                value = min;
            }
            else if (value > max)
            {
                value = max;
            }

            return value;
        }

        /// <summary>
        /// 将值限制在 [0, 1] 范围内
        /// </summary>
        /// <param name="value">输入值</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FixedPoint Clamp01(FixedPoint value)
        {
            if (value < FixedPoint.Zero)
            {
                return FixedPoint.Zero;
            }

            if (value > FixedPoint.One)
            {
                return FixedPoint.One;
            }

            return value;
        }

        #endregion

        #region 三角函数

        /// <summary>
        /// 正弦 (查找表 + 线性插值，输入弧度)
        /// </summary>
        /// <param name="radians">弧度值</param>
        public static FixedPoint Sin(FixedPoint radians)
        {
            long tableValue = InterpolateTable(SinCosLookupTable.SinTable, radians.FixedValue);
            return new FixedPoint(Divide(tableValue * FixedPoint.MULTIPLE, LOOKUP_FACTOR));
        }

        /// <summary>
        /// 余弦 (查找表 + 线性插值，输入弧度)
        /// </summary>
        /// <param name="radians">弧度值</param>
        public static FixedPoint Cos(FixedPoint radians)
        {
            long tableValue = InterpolateTable(SinCosLookupTable.CosTable, radians.FixedValue);
            return new FixedPoint(Divide(tableValue * FixedPoint.MULTIPLE, LOOKUP_FACTOR));
        }

        /// <summary>
        /// 同时计算正弦和余弦 (共享插值位置计算，比分别调用 Sin 和 Cos 更高效)
        /// </summary>
        /// <param name="radians">弧度值</param>
        /// <param name="sin">输出正弦值</param>
        /// <param name="cos">输出余弦值</param>
        public static void SinCos(FixedPoint radians, out FixedPoint sin, out FixedPoint cos)
        {
            // 归约弧度到 [0, TRIG_TABLE_PERIOD)，防止乘 NomMul 时 long 溢出
            long scaledRadians = radians.FixedValue % TRIG_TABLE_PERIOD;
            if (scaledRadians < 0) scaledRadians += TRIG_TABLE_PERIOD;

            // 将弧度值映射到查找表索引空间
            scaledRadians *= SinCosLookupTable.NomMul;

            long tableIndex = scaledRadians / TRIG_TABLE_PERIOD;
            long interpolationRemainder = scaledRadians - tableIndex * TRIG_TABLE_PERIOD;

            int currentIndex = (int)tableIndex & SinCosLookupTable.Mask;
            int nextIndex = (currentIndex + 1) & SinCosLookupTable.Mask;

            // 线性插值: table[i] + (table[i+1] − table[i]) × remainder / period
            long sinValue = SinCosLookupTable.SinTable[currentIndex]
                + ((long)SinCosLookupTable.SinTable[nextIndex] - SinCosLookupTable.SinTable[currentIndex])
                * interpolationRemainder / TRIG_TABLE_PERIOD;
            long cosValue = SinCosLookupTable.CosTable[currentIndex]
                + ((long)SinCosLookupTable.CosTable[nextIndex] - SinCosLookupTable.CosTable[currentIndex])
                * interpolationRemainder / TRIG_TABLE_PERIOD;

            sin = new FixedPoint(Divide(sinValue * FixedPoint.MULTIPLE, LOOKUP_FACTOR));
            cos = new FixedPoint(Divide(cosValue * FixedPoint.MULTIPLE, LOOKUP_FACTOR));
        }

        /// <summary>
        /// 正切 (Sin / Cos，cos 为零时除法自动饱和)
        /// </summary>
        /// <param name="radians">弧度值</param>
        public static FixedPoint Tan(FixedPoint radians)
        {
            return Sin(radians) / Cos(radians);
        }

        /// <summary>
        /// 反余弦 (查找表 + 线性插值，与 Sin/Cos 精度一致)
        /// </summary>
        /// <param name="value">输入值</param>
        public static FixedPoint Acos(FixedPoint value)
        {
            return AcosInterpolated(
                value.FixedValue * AcosLookupTable.HalfCount, FixedPoint.MULTIPLE);
        }

        /// <summary>
        /// 反余弦 (查找表 + 线性插值，指定分母)
        /// </summary>
        /// <param name="value">输入值</param>
        /// <param name="denominator">分母值</param>
        public static FixedPoint Acos(FixedPoint value, long denominator)
        {
            return AcosInterpolated(
                value.FixedValue * AcosLookupTable.HalfCount, denominator);
        }

        /// <summary>
        /// 反正弦 (通过恒等式 Asin(x) = π/2 − Acos(x) 计算)
        /// </summary>
        /// <param name="value">输入值 (范围 [-1, 1])</param>
        public static FixedPoint Asin(FixedPoint value)
        {
            return FixedPoint.PiOver2 - Acos(value);
        }

        /// <summary>
        /// 反正切 (通过 Atan2(value, 1) 实现)
        /// </summary>
        /// <param name="value">输入值</param>
        public static FixedPoint Atan(FixedPoint value)
        {
            return Atan2(value, FixedPoint.One);
        }

        /// <summary>
        /// 四象限反正切 (查找表，输入定点数，确定性安全)
        /// </summary>
        /// <param name="fy">Y 分量（定点值）</param>
        /// <param name="fx">X 分量（定点值）</param>
        public static FixedPoint Atan2(FixedPoint fy, FixedPoint fx)
        {
            return Atan2Internal(fy.FixedValue, fx.FixedValue);
        }

        /// <summary>
        /// 四象限反正切 (查找表，输入浮点数，仅用于编辑器工具或初始化，运行时逻辑请使用 Atan2(FixedPoint, FixedPoint))
        /// </summary>
        /// <param name="fy">Y 分量（浮点值）</param>
        /// <param name="fx">X 分量（浮点值）</param>
        public static FixedPoint Atan2(float fy, float fx)
        {
            return Atan2Internal((int)(fy * FixedPoint.MULTIPLE), (int)(fx * FixedPoint.MULTIPLE));
        }

        /// <summary>
        /// Atan2 内部实现，通过象限映射查找表计算角度
        /// </summary>
        /// <param name="yValue">Y 分量（定点原始值）</param>
        /// <param name="xValue">X 分量（定点原始值）</param>
        private static FixedPoint Atan2Internal(long yValue, long xValue)
        {
            long absX = xValue;
            long absY = yValue;
            if (absX == 0 && absY == 0)
            {
                return FixedPoint.Zero;
            }

            // 将任意象限映射到第一象限，记录符号和偏移量用于还原
            int signMultiplier;
            int angleOffset;
            if (absX < 0)
            {
                if (absY < 0)
                {
                    // 第三象限 → 第一象限，角度 = -(π - θ)
                    absX = unchecked(-absX);
                    absY = unchecked(-absY);
                    signMultiplier = 1;
                }
                else
                {
                    // 第二象限 → 第一象限，角度 = π - θ
                    absX = unchecked(-absX);
                    signMultiplier = -1;
                }

                angleOffset = -31416;
            }
            else
            {
                if (absY < 0)
                {
                    // 第四象限 → 第一象限，角度 = -θ
                    absY = unchecked(-absY);
                    signMultiplier = -1;
                }
                else
                {
                    // 已在第一象限
                    signMultiplier = 1;
                }

                angleOffset = 0;
            }

            // 将 (absX, absY) 归一化到查找表索引范围 [0, dim-1]
            int tableDimension = Atan2LookupTable.Dim;
            long indexRange = tableDimension - 1L;
            long maxComponent = absX >= absY ? absX : absY;
            int indexX = (int)Divide(absX * indexRange, maxComponent);
            int indexY = (int)Divide(absY * indexRange, maxComponent);
            int lookupAngle = Atan2LookupTable.Table[indexY * tableDimension + indexX];
            return new FixedPoint(
                Divide((long)(lookupAngle + angleOffset) * signMultiplier * FixedPoint.MULTIPLE, LOOKUP_FACTOR));
        }

        #endregion

        #region 插值函数

        /// <summary>
        /// 线性插值 (t 被限制在 [0, 1])
        /// </summary>
        /// <param name="a">起始值</param>
        /// <param name="b">目标值</param>
        /// <param name="t">插值系数</param>
        public static FixedPoint Lerp(FixedPoint a, FixedPoint b, FixedPoint t)
        {
            t = Clamp01(t);
            return a + (b - a) * t;
        }

        /// <summary>
        /// 线性插值 (t 不限制范围)
        /// </summary>
        /// <param name="a">起始值</param>
        /// <param name="b">目标值</param>
        /// <param name="t">插值系数</param>
        public static FixedPoint LerpUnclamped(FixedPoint a, FixedPoint b, FixedPoint t)
        {
            return a + (b - a) * t;
        }

        /// <summary>
        /// 反向线性插值，返回 value 在 [a, b] 中的归一化位置
        /// </summary>
        /// <param name="a">区间起点</param>
        /// <param name="b">区间终点</param>
        /// <param name="value">目标值</param>
        public static FixedPoint InverseLerp(FixedPoint a, FixedPoint b, FixedPoint value)
        {
            if (a != b)
            {
                return Clamp01((value - a) / (b - a));
            }

            return FixedPoint.Zero;
        }

        /// <summary>
        /// 将 current 向 target 移动，每次最多移动 maxDelta
        /// </summary>
        /// <param name="current">当前值</param>
        /// <param name="target">目标值</param>
        /// <param name="maxDelta">最大移动量</param>
        public static FixedPoint MoveTowards(FixedPoint current, FixedPoint target, FixedPoint maxDelta)
        {
            if (Abs(target - current) <= maxDelta)
            {
                return target;
            }

            return current + Sign(target - current) * maxDelta;
        }

        /// <summary>
        /// 平滑阶梯插值 (Hermite)
        /// </summary>
        /// <param name="from">起始值</param>
        /// <param name="to">目标值</param>
        /// <param name="t">插值系数</param>
        public static FixedPoint SmoothStep(FixedPoint from, FixedPoint to, FixedPoint t)
        {
            t = Clamp01(t);
            t = t * t * (Three - FixedPoint.Two * t);
            return from + (to - from) * t;
        }

        /// <summary>
        /// 标量平滑阻尼
        /// </summary>
        /// <param name="current">当前值</param>
        /// <param name="target">目标值</param>
        /// <param name="currentVelocity">当前速度 (由函数修改)</param>
        /// <param name="smoothTime">到达目标的近似时间</param>
        /// <param name="deltaTime">帧同步时间增量 (不要使用 Time.deltaTime)</param>
        /// <param name="maxSpeed">最大速度限制</param>
        public static FixedPoint SmoothDamp(FixedPoint current, FixedPoint target,
            ref FixedPoint currentVelocity, FixedPoint smoothTime, FixedPoint deltaTime,
            FixedPoint maxSpeed)
        {
            smoothTime = Max(FixedPoint.Epsilon, smoothTime);
            // 弹簧阻尼系统: omega = 2/smoothTime 为角频率
            FixedPoint omega = FixedPoint.Two / smoothTime;
            FixedPoint dampingInput = omega * deltaTime;
            FixedPoint decayFactor = FixedPoint.One /
                             (FixedPoint.One + dampingInput + SmoothDampC2 * dampingInput * dampingInput
                              + SmoothDampC3 * dampingInput * dampingInput * dampingInput);
            FixedPoint change = current - target;
            FixedPoint originalTo = target;
            FixedPoint maxChange = maxSpeed * smoothTime;
            change = Clamp(change, -maxChange, maxChange);
            target = current - change;
            FixedPoint temp = (currentVelocity + omega * change) * deltaTime;
            currentVelocity = (currentVelocity - omega * temp) * decayFactor;
            FixedPoint output = target + (change + temp) * decayFactor;
            if ((originalTo - current > FixedPoint.Zero) == (output > originalTo))
            {
                output = originalTo;
                currentVelocity = (output - originalTo) / deltaTime;
            }

            return output;
        }

        #endregion

        #region 角度相关

        /// <summary>
        /// 将角度转换为弧度 (比常量 Deg2Rad 精度更高)
        /// </summary>
        /// <param name="degrees">角度值</param>
        public static FixedPoint DegreesToRadians(FixedPoint degrees)
        {
            return degrees * FixedPoint.Pi / Deg180;
        }

        /// <summary>
        /// 将弧度转换为角度 (比常量 Rad2Deg 精度更高)
        /// </summary>
        /// <param name="radians">弧度值</param>
        public static FixedPoint RadiansToDegrees(FixedPoint radians)
        {
            return radians * Deg180 / FixedPoint.Pi;
        }

        /// <summary>
        /// 计算两个角度之间的最短差值 (考虑 360° 回绕)
        /// </summary>
        /// <param name="current">当前角度</param>
        /// <param name="target">目标角度</param>
        public static FixedPoint DeltaAngle(FixedPoint current, FixedPoint target)
        {
            FixedPoint delta = Repeat(target - current, Deg360);
            if (delta > Deg180)
            {
                delta -= Deg360;
            }

            return delta;
        }

        /// <summary>
        /// 角度线性插值 (考虑 360° 回绕)
        /// </summary>
        /// <param name="a">起始角度</param>
        /// <param name="b">目标角度</param>
        /// <param name="t">插值系数</param>
        public static FixedPoint LerpAngle(FixedPoint a, FixedPoint b, FixedPoint t)
        {
            FixedPoint delta = Repeat(b - a, Deg360);
            if (delta > Deg180)
            {
                delta -= Deg360;
            }

            return a + delta * Clamp01(t);
        }

        /// <summary>
        /// 将角度向目标移动，每次最多移动 maxDelta (考虑 360° 回绕)
        /// </summary>
        /// <param name="current">当前角度</param>
        /// <param name="target">目标角度</param>
        /// <param name="maxDelta">最大移动角度</param>
        public static FixedPoint MoveTowardsAngle(FixedPoint current, FixedPoint target, FixedPoint maxDelta)
        {
            FixedPoint delta = DeltaAngle(current, target);
            if (-maxDelta < delta && delta < maxDelta)
            {
                return target;
            }

            target = current + delta;
            return MoveTowards(current, target, maxDelta);
        }

        #endregion

        #region 周期函数

        /// <summary>
        /// 循环取值 (类似取模，但结果始终非负)
        /// </summary>
        /// <param name="t">输入值</param>
        /// <param name="length">循环长度</param>
        public static FixedPoint Repeat(FixedPoint t, FixedPoint length)
        {
            return Clamp(t - Floor(t / length) * length, FixedPoint.Zero, length);
        }

        /// <summary>
        /// 乒乓循环取值 (在 0 和 length 之间来回)
        /// </summary>
        /// <param name="t">输入值</param>
        /// <param name="length">循环长度</param>
        public static FixedPoint PingPong(FixedPoint t, FixedPoint length)
        {
            t = Repeat(t, length * FixedPoint.Two);
            return length - Abs(t - length);
        }

        #endregion

        #region 比较函数

        /// <summary>
        /// 近似相等判断 (差值不超过定点数最小精度)
        /// </summary>
        /// <param name="a">第一个值</param>
        /// <param name="b">第二个值</param>
        public static bool Approximately(FixedPoint a, FixedPoint b)
        {
            return Abs(b - a) <= FixedPoint.Epsilon;
        }

        #endregion

        #region 内部辅助

        /// <summary>
        /// 对反余弦查找表进行线性插值查询
        /// 向下取整确定基准索引，余数作为插值权重，与 Sin/Cos 精度一致
        /// </summary>
        /// <param name="scaledNumerator">value.FixedValue × HalfCount</param>
        /// <param name="divisor">除数 (MULTIPLE 或自定义分母)</param>
        private static FixedPoint AcosInterpolated(long scaledNumerator, long divisor)
        {
            // 向下取整除法，确保余数 r ∈ [0, divisor)
            long q = scaledNumerator / divisor;
            long r = scaledNumerator - q * divisor;
            if (r < 0) { q--; r += divisor; }

            int rawIndex = (int)q + AcosLookupTable.HalfCount;

            // 边界外直接返回端点值
            if (rawIndex < 0)
            {
                return new FixedPoint(
                    Divide((long)AcosLookupTable.Table[0] * FixedPoint.MULTIPLE, LOOKUP_FACTOR));
            }

            if (rawIndex >= AcosLookupTable.Count)
            {
                return new FixedPoint(
                    Divide((long)AcosLookupTable.Table[AcosLookupTable.Count] * FixedPoint.MULTIPLE,
                        LOOKUP_FACTOR));
            }

            // 线性插值: table[i] + (table[i+1] − table[i]) × r / divisor
            long val0 = AcosLookupTable.Table[rawIndex];
            long val1 = AcosLookupTable.Table[rawIndex + 1];
            long interpolated = val0 + (val1 - val0) * r / divisor;

            return new FixedPoint(
                Divide(interpolated * FixedPoint.MULTIPLE, LOOKUP_FACTOR));
        }

        /// <summary>
        /// 对正余弦查找表进行线性插值查询 (消除表索引量化误差，纯整数运算保证确定性)
        /// </summary>
        /// <param name="table">查找表 (SinTable 或 CosTable)</param>
        /// <param name="numerator">弧度定点原始值</param>
        private static long InterpolateTable(int[] table, long numerator)
        {
            // 归约分子到 [0, TRIG_TABLE_PERIOD)，防止乘 NomMul 时 long 溢出
            numerator %= TRIG_TABLE_PERIOD;
            if (numerator < 0) numerator += TRIG_TABLE_PERIOD;

            // 将弧度值映射到查找表索引空间
            numerator *= SinCosLookupTable.NomMul;

            long tableIndex = numerator / TRIG_TABLE_PERIOD;
            long interpolationRemainder = numerator - tableIndex * TRIG_TABLE_PERIOD;

            int currentIndex = (int)tableIndex & SinCosLookupTable.Mask;
            int nextIndex = (currentIndex + 1) & SinCosLookupTable.Mask;

            // 线性插值: table[i] + (table[i+1] − table[i]) × remainder / period
            return table[currentIndex]
                + ((long)table[nextIndex] - table[currentIndex]) * interpolationRemainder / TRIG_TABLE_PERIOD;
        }

        /// <summary>
        /// 四舍五入整数除法 (long)，保证确定性
        /// </summary>
        /// <param name="dividend">被除数</param>
        /// <param name="divisor">除数</param>
        internal static long Divide(long dividend, long divisor)
        {
            if (divisor == 0)
            {
                throw new DivideByZeroException("除数不能为0");
            }

            // 提取符号位：异号为1，同号为0
            long signBit = (long)((ulong)((dividend ^ divisor) & long.MinValue) >> 63);
            // 异号时 signCorrection = -1 (向零舍入)，同号时 signCorrection = 1 (远离零舍入)
            long signCorrection = signBit * -2L + 1L;
            long roundingOffset = unchecked((divisor / 2L) * signCorrection);
            return unchecked(dividend + roundingOffset) / divisor;
        }

        /// <summary>
        /// 四舍五入整数除法 (int)，保证确定性
        /// </summary>
        /// <param name="dividend">被除数</param>
        /// <param name="divisor">除数</param>
        internal static int Divide(int dividend, int divisor)
        {
            if (divisor == 0)
            {
                throw new DivideByZeroException("除数不能为0");
            }

            // 提取符号位：异号为1，同号为0
            int signBit = (int)((uint)((dividend ^ divisor) & int.MinValue) >> 31);
            // 异号时 signCorrection = -1 (向零舍入)，同号时 signCorrection = 1 (远离零舍入)
            int signCorrection = signBit * -2 + 1;
            int roundingOffset = unchecked((divisor / 2) * signCorrection);
            return unchecked(dividend + roundingOffset) / divisor;
        }

        #endregion
    }
}