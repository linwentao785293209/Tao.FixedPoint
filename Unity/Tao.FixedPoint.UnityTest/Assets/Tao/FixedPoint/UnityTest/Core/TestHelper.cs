using NUnit.Framework;

namespace Tao.FixedPoint.UnityTest
{
    /// <summary>
    /// 单元测试通用断言辅助
    /// </summary>
    internal static class TestHelper
    {
        /// <summary>
        /// 将定点数转换为 double (不使用 RawDouble，避免其内部四舍五入)
        /// </summary>
        internal static double ToDouble(FixedPoint fp)
        {
            return fp.FixedValue / (double)FixedPoint.MULTIPLE;
        }

        /// <summary>
        /// 断言定点数近似等于预期 double 值
        /// </summary>
        internal static void AssertApprox(FixedPoint actual, double expected, double tolerance = 0.002)
        {
            double actualDouble = ToDouble(actual);
            Assert.IsTrue(System.Math.Abs(actualDouble - expected) <= tolerance,
                $"Expected ≈{expected}, got {actualDouble} (FixedValue={actual.FixedValue})");
        }

        /// <summary>
        /// 断言向量各分量近似等于预期值
        /// </summary>
        internal static void AssertApprox(Vector3 actual, double ex, double ey, double ez, double tolerance = 0.01)
        {
            AssertApprox(actual.x, ex, tolerance);
            AssertApprox(actual.y, ey, tolerance);
            AssertApprox(actual.z, ez, tolerance);
        }
    }
}
