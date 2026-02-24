using System;
using NUnit.Framework;

namespace Tao.FixedPoint.UnityTest
{
    /// <summary>
    /// FixedPoint 核心结构体测试：构造函数、类型转换、运算符、饱和钳位、相等比较
    /// </summary>
    [TestFixture]
    public class FixedPointTests
    {
        #region 常量

        [Test]
        public void Constant_Multiple_Is1024()
        {
            Assert.AreEqual(1024, FixedPoint.MULTIPLE);
        }

        [Test]
        public void Constant_MaxValue_EqualsIntMaxShifted()
        {
            Assert.AreEqual((long)int.MaxValue << 10, FixedPoint.MAX_VALUE);
        }

        [Test]
        public void Constant_MinValue_IsNegativeMaxValue()
        {
            Assert.AreEqual(-FixedPoint.MAX_VALUE, FixedPoint.MIN_VALUE);
        }

        [Test]
        public void Constant_StaticReadonly_CorrectValues()
        {
            Assert.AreEqual(0L, FixedPoint.Zero.FixedValue);
            Assert.AreEqual(1024L, FixedPoint.One.FixedValue);
            Assert.AreEqual(2048L, FixedPoint.Two.FixedValue);
            Assert.AreEqual(-1024L, FixedPoint.NegativeOne.FixedValue);
            Assert.AreEqual(1L, FixedPoint.Epsilon.FixedValue);
            Assert.AreEqual(FixedPoint.MAX_VALUE, FixedPoint.MaxValue.FixedValue);
            Assert.AreEqual(FixedPoint.MIN_VALUE, FixedPoint.MinValue.FixedValue);
        }

        [Test]
        public void Constant_Pi_ApproximatelyCorrect()
        {
            TestHelper.AssertApprox(FixedPoint.Pi, System.Math.PI, 0.001);
            TestHelper.AssertApprox(FixedPoint.TwoPi, 2 * System.Math.PI, 0.002);
            TestHelper.AssertApprox(FixedPoint.PiOver2, System.Math.PI / 2, 0.002);
        }

        #endregion

        #region 构造函数

        [TestCase(0, 0L)]
        [TestCase(1, 1024L)]
        [TestCase(5, 5120L)]
        [TestCase(-3, -3072L)]
        public void Constructor_Int_ScalesUp(int input, long expectedFixed)
        {
            FixedPoint fp = new FixedPoint(input);
            Assert.AreEqual(expectedFixed, fp.FixedValue);
        }

        [TestCase(0L, 0L)]
        [TestCase(5L, 5L)]
        [TestCase(1024L, 1024L)]
        [TestCase(-512L, -512L)]
        public void Constructor_Long_TreatsAsRaw(long input, long expectedFixed)
        {
            FixedPoint fp = new FixedPoint(input);
            Assert.AreEqual(expectedFixed, fp.FixedValue);
        }

        [Test]
        public void Constructor_Long_SaturatesOnOverflow()
        {
            FixedPoint fpMax = new FixedPoint(long.MaxValue);
            Assert.AreEqual(FixedPoint.MAX_VALUE, fpMax.FixedValue);

            FixedPoint fpMin = new FixedPoint(long.MinValue + 1);
            Assert.AreEqual(FixedPoint.MIN_VALUE, fpMin.FixedValue);
        }

        [Test]
        public void Constructor_Double_RoundsAndScales()
        {
            FixedPoint fp = new FixedPoint(1.5);
            Assert.AreEqual(1536L, fp.FixedValue);

            FixedPoint fpNeg = new FixedPoint(-2.25);
            Assert.AreEqual(-2304L, fpNeg.FixedValue);
        }

        [Test]
        public void Constructor_Float_RoundsAndScales()
        {
            FixedPoint fp = new FixedPoint(1.5f);
            Assert.AreEqual(1536L, fp.FixedValue);
        }

        #endregion

        #region 隐式/显式转换

        [Test]
        public void ImplicitConversion_Int_ScalesUp()
        {
            FixedPoint fp = 5;
            Assert.AreEqual(5120L, fp.FixedValue);
        }

        [Test]
        public void ImplicitConversion_Long_TreatsAsRaw()
        {
            FixedPoint fp = 5L;
            Assert.AreEqual(5L, fp.FixedValue);
        }

        [Test]
        public void ExplicitConversion_ToNumeric_ReturnsExpected()
        {
            FixedPoint fp = new FixedPoint(3);
            Assert.AreEqual(3.0, (double)fp, 0.001);
            Assert.AreEqual(3.0f, (float)fp, 0.001f);
            Assert.AreEqual(3, (int)fp);
            Assert.AreEqual(3072L, (long)fp);
        }

        [Test]
        public void ExplicitConversion_FromFloat_CreatesFixedPoint()
        {
            FixedPoint fromDouble = (FixedPoint)3.14;
            TestHelper.AssertApprox(fromDouble, 3.14, 0.001);

            FixedPoint fromFloat = (FixedPoint)3.14f;
            TestHelper.AssertApprox(fromFloat, 3.14, 0.001);
        }

        #endregion

        #region 属性

        [Test]
        public void Property_Raw_ReturnsRestoredValue()
        {
            FixedPoint fp = new FixedPoint(3);
            Assert.AreEqual(3.0, fp.RawDouble, 0.001);
            Assert.AreEqual(3.0f, fp.RawFloat, 0.001f);
            Assert.AreEqual(3, fp.RawInt);
        }

        #endregion

        #region 算术运算符

        [Test]
        public void Operator_Add_ReturnsSum()
        {
            FixedPoint a = new FixedPoint(3);
            FixedPoint b = new FixedPoint(5);
            FixedPoint result = a + b;
            Assert.AreEqual(new FixedPoint(8).FixedValue, result.FixedValue);
        }

        [Test]
        public void Operator_Add_Overflow_Saturates()
        {
            FixedPoint result = FixedPoint.MaxValue + FixedPoint.One;
            Assert.AreEqual(FixedPoint.MaxValue, result);
        }

        [Test]
        public void Operator_Sub_ReturnsDifference()
        {
            FixedPoint a = new FixedPoint(10);
            FixedPoint b = new FixedPoint(3);
            FixedPoint result = a - b;
            Assert.AreEqual(new FixedPoint(7).FixedValue, result.FixedValue);
        }

        [Test]
        public void Operator_Sub_Underflow_Saturates()
        {
            FixedPoint result = FixedPoint.MinValue - FixedPoint.One;
            Assert.AreEqual(FixedPoint.MinValue, result);
        }

        [Test]
        public void Operator_UnaryNegate_ReturnsNegated()
        {
            FixedPoint pos = new FixedPoint(5);
            FixedPoint neg = -pos;
            Assert.AreEqual(-5120L, neg.FixedValue);

            Assert.AreEqual(FixedPoint.Zero, -FixedPoint.Zero);
        }

        [Test]
        public void Operator_Mul_ReturnsProduct()
        {
            FixedPoint a = new FixedPoint(3);
            FixedPoint b = new FixedPoint(4);
            Assert.AreEqual(new FixedPoint(12).FixedValue, (a * b).FixedValue);
        }

        [Test]
        public void Operator_Mul_WithFraction()
        {
            FixedPoint a = new FixedPoint(1.5);
            FixedPoint b = new FixedPoint(4);
            TestHelper.AssertApprox(a * b, 6.0, 0.002);
        }

        [Test]
        public void Operator_Mul_Zero_ReturnsZero()
        {
            Assert.AreEqual(FixedPoint.Zero, FixedPoint.Zero * new FixedPoint(999));
            Assert.AreEqual(FixedPoint.Zero, new FixedPoint(999) * FixedPoint.Zero);
        }

        [Test]
        public void Operator_Mul_Overflow_Saturates()
        {
            FixedPoint large = new FixedPoint(1000000);
            FixedPoint result = large * large;
            Assert.AreEqual(FixedPoint.MaxValue, result);
        }

        [Test]
        public void Operator_Mul_NegativeOverflow_SaturatesToMin()
        {
            FixedPoint large = new FixedPoint(1000000);
            FixedPoint negLarge = new FixedPoint(-1000000);
            FixedPoint result = large * negLarge;
            Assert.AreEqual(FixedPoint.MinValue, result);
        }

        [Test]
        public void Operator_Div_ReturnsQuotient()
        {
            FixedPoint a = new FixedPoint(12);
            FixedPoint b = new FixedPoint(4);
            Assert.AreEqual(new FixedPoint(3).FixedValue, (a / b).FixedValue);
        }

        [Test]
        public void Operator_Div_ByZero_Throws()
        {
            Assert.Throws<DivideByZeroException>(() =>
            {
                FixedPoint _ = new FixedPoint(1) / FixedPoint.Zero;
            });
        }

        [Test]
        public void Operator_Mod_ReturnsRemainder()
        {
            FixedPoint a = new FixedPoint(7);
            FixedPoint b = new FixedPoint(3);
            TestHelper.AssertApprox(a % b, 1.0, 0.001);
        }

        [Test]
        public void Operator_Mod_ByZero_Throws()
        {
            Assert.Throws<DivideByZeroException>(() =>
            {
                FixedPoint _ = new FixedPoint(1) % FixedPoint.Zero;
            });
        }

        [Test]
        public void Operator_LeftShift_DoublesValue()
        {
            FixedPoint fp = new FixedPoint(3);
            FixedPoint shifted = fp << 1;
            Assert.AreEqual(new FixedPoint(6).FixedValue, shifted.FixedValue);
        }

        [Test]
        public void Operator_LeftShift_Overflow_Saturates()
        {
            FixedPoint fp = new FixedPoint(1000000);
            FixedPoint shifted = fp << 30;
            Assert.AreEqual(FixedPoint.MaxValue, shifted);
        }

        [Test]
        public void Operator_RightShift_HalvesValue()
        {
            FixedPoint fp = new FixedPoint(8);
            FixedPoint shifted = fp >> 1;
            Assert.AreEqual(new FixedPoint(4).FixedValue, shifted.FixedValue);
        }

        #endregion

        #region 比较运算符

        [Test]
        public void Operator_Equality_ComparesFixedValue()
        {
            FixedPoint a = new FixedPoint(5);
            FixedPoint b = new FixedPoint(5);
            FixedPoint c = new FixedPoint(3);

            Assert.IsTrue(a == b);
            Assert.IsFalse(a == c);
            Assert.IsFalse(a != b);
            Assert.IsTrue(a != c);
        }

        [Test]
        public void Operator_Comparison_CorrectOrdering()
        {
            FixedPoint three = new FixedPoint(3);
            FixedPoint five = new FixedPoint(5);

            Assert.IsTrue(five > three);
            Assert.IsFalse(three > five);
            Assert.IsTrue(three < five);
            Assert.IsFalse(five < three);
            Assert.IsTrue(five >= new FixedPoint(5));
            Assert.IsTrue(three <= new FixedPoint(3));
            Assert.IsTrue(five >= three);
            Assert.IsTrue(three <= five);
        }

        #endregion

        #region Equals / GetHashCode / CompareTo / ToString

        [Test]
        public void Equals_SameValue_ReturnsTrue()
        {
            FixedPoint a = new FixedPoint(5);
            FixedPoint b = new FixedPoint(5);

            Assert.IsTrue(a.Equals(b));
            Assert.IsTrue(a.Equals((object)b));
            Assert.IsFalse(a.Equals((object)42));
            Assert.IsFalse(a.Equals(null));
        }

        [Test]
        public void GetHashCode_EqualValues_SameHash()
        {
            FixedPoint a = new FixedPoint(5);
            FixedPoint b = new FixedPoint(5);
            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
        }

        [Test]
        public void CompareTo_CorrectOrdering()
        {
            FixedPoint three = new FixedPoint(3);
            FixedPoint five = new FixedPoint(5);

            Assert.IsTrue(three.CompareTo(five) < 0);
            Assert.IsTrue(five.CompareTo(three) > 0);
            Assert.AreEqual(0, three.CompareTo(three));
        }

        [Test]
        public void CompareTo_NonFixedPoint_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                FixedPoint fp = new FixedPoint(1);
                fp.CompareTo((object)"not a FixedPoint");
            });
        }

        [Test]
        public void ToString_ReturnsNonEmpty()
        {
            FixedPoint fp = new FixedPoint(3);
            string str = fp.ToString();
            Assert.IsFalse(string.IsNullOrEmpty(str));
        }

        #endregion
    }
}
