using System;

namespace Tao.FixedPoint
{
    /// <summary>
    /// 确定性定点数随机数生成器 (Xorshift32 算法，纯整数运算，保证跨平台确定性)
    /// </summary>
    public class RandomSeed
    {
        #region 字段和属性

        /// <summary>
        /// 随机种子
        /// </summary>
        public int SeedId { get; private set; }

        /// <summary>
        /// Xorshift32 内部状态 (必须非零)
        /// </summary>
        private uint _state;

        #endregion

        #region 构造函数

        /// <summary>
        /// 使用指定种子创建确定性随机数生成器
        /// </summary>
        /// <param name="seedId">随机种子</param>
        public RandomSeed(int seedId)
        {
            SeedId = seedId;
            _state = seedId != 0 ? (uint)seedId : 2463534242u;
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 在 [min, max) 范围内随机一个整数
        /// </summary>
        /// <param name="min">最小值（包含）</param>
        /// <param name="max">最大值（不包含）</param>
        public int Range(int min, int max)
        {
            if (min >= max)
            {
                return min;
            }

            uint range = (uint)(max - min);
            return min + (int)(NextUInt() % range);
        }

        /// <summary>
        /// 在 [min, max) 范围内随机一个定点数的整数部分
        /// </summary>
        /// <param name="min">最小定点值（包含）</param>
        /// <param name="max">最大定点值（不包含）</param>
        public int Range(FixedPoint min, FixedPoint max)
        {
            long minFixValue = min.FixedValue;
            long maxFixValue = max.FixedValue;
            if (minFixValue < int.MinValue || minFixValue > int.MaxValue
                                           || maxFixValue < int.MinValue || maxFixValue > int.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(min), "定点范围超出 int 支持区间，无法安全转换为整数随机范围。");
            }

            return Range((int)minFixValue, (int)maxFixValue) / FixedPoint.MULTIPLE;
        }

        #endregion

        #region 内部辅助

        /// <summary>
        /// Xorshift32 生成下一个随机 uint (Marsaglia, 2003)
        /// </summary>
        private uint NextUInt()
        {
            _state ^= _state << 13;
            _state ^= _state >> 17;
            _state ^= _state << 5;
            return _state;
        }

        #endregion
    }
}