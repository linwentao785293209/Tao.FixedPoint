using System;
using System.Globalization;

namespace Tao.FixedPoint
{
    /// <summary>
    /// 4×4 定点数矩阵，用于组合变换 (平移+旋转+缩放)
    /// 行优先存储: m[row, col]
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public struct Matrix4x4 : IEquatable<Matrix4x4>
    {
        #region 字段

        public FixedPoint m00, m01, m02, m03;
        public FixedPoint m10, m11, m12, m13;
        public FixedPoint m20, m21, m22, m23;
        public FixedPoint m30, m31, m32, m33;

        #endregion

        #region 属性

        /// <summary>
        /// 单位矩阵
        /// </summary>
        public static Matrix4x4 Identity
        {
            get
            {
                Matrix4x4 result = default;
                result.m00 = FixedPoint.One;
                result.m11 = FixedPoint.One;
                result.m22 = FixedPoint.One;
                result.m33 = FixedPoint.One;
                return result;
            }
        }

        /// <summary>
        /// 零矩阵
        /// </summary>
        public static Matrix4x4 Zero => default;

        /// <summary>
        /// 转置矩阵
        /// </summary>
        public Matrix4x4 Transpose
        {
            get
            {
                Matrix4x4 result = default;
                result.m00 = m00; result.m01 = m10; result.m02 = m20; result.m03 = m30;
                result.m10 = m01; result.m11 = m11; result.m12 = m21; result.m13 = m31;
                result.m20 = m02; result.m21 = m12; result.m22 = m22; result.m23 = m32;
                result.m30 = m03; result.m31 = m13; result.m32 = m23; result.m33 = m33;
                return result;
            }
        }

        /// <summary>
        /// 行列式
        /// </summary>
        public FixedPoint Determinant
        {
            get
            {
                // Laplace 展开: s0..s5 = 前两行(0,1)的 2×2 子行列式
                FixedPoint s0 = m00 * m11 - m01 * m10;
                FixedPoint s1 = m00 * m12 - m02 * m10;
                FixedPoint s2 = m00 * m13 - m03 * m10;
                FixedPoint s3 = m01 * m12 - m02 * m11;
                FixedPoint s4 = m01 * m13 - m03 * m11;
                FixedPoint s5 = m02 * m13 - m03 * m12;

                // c0..c5 = 后两行(2,3)的 2×2 子行列式
                FixedPoint c5 = m22 * m33 - m23 * m32;
                FixedPoint c4 = m21 * m33 - m23 * m31;
                FixedPoint c3 = m21 * m32 - m22 * m31;
                FixedPoint c2 = m20 * m33 - m23 * m30;
                FixedPoint c1 = m20 * m32 - m22 * m30;
                FixedPoint c0 = m20 * m31 - m21 * m30;

                return s0 * c5 - s1 * c4 + s2 * c3 + s3 * c2 - s4 * c1 + s5 * c0;
            }
        }

        /// <summary>
        /// 逆矩阵 (通过伴随矩阵法计算，行列式为零时抛出异常)
        /// </summary>
        public Matrix4x4 Inverse
        {
            get
            {
                // Laplace 展开: s0..s5 = 前两行(0,1)的 2×2 子行列式
                FixedPoint s0 = m00 * m11 - m01 * m10;
                FixedPoint s1 = m00 * m12 - m02 * m10;
                FixedPoint s2 = m00 * m13 - m03 * m10;
                FixedPoint s3 = m01 * m12 - m02 * m11;
                FixedPoint s4 = m01 * m13 - m03 * m11;
                FixedPoint s5 = m02 * m13 - m03 * m12;

                // c0..c5 = 后两行(2,3)的 2×2 子行列式
                FixedPoint c5 = m22 * m33 - m23 * m32;
                FixedPoint c4 = m21 * m33 - m23 * m31;
                FixedPoint c3 = m21 * m32 - m22 * m31;
                FixedPoint c2 = m20 * m33 - m23 * m30;
                FixedPoint c1 = m20 * m32 - m22 * m30;
                FixedPoint c0 = m20 * m31 - m21 * m30;

                FixedPoint det = s0 * c5 - s1 * c4 + s2 * c3 + s3 * c2 - s4 * c1 + s5 * c0;
                if (det == FixedPoint.Zero)
                {
                    throw new InvalidOperationException("矩阵不可逆 (行列式为零)");
                }

                // 伴随矩阵法: result = adj(M) / det(M)
                Matrix4x4 result = default;
                result.m00 = (m11 * c5 - m12 * c4 + m13 * c3) / det;
                result.m01 = (-m01 * c5 + m02 * c4 - m03 * c3) / det;
                result.m02 = (m31 * s5 - m32 * s4 + m33 * s3) / det;
                result.m03 = (-m21 * s5 + m22 * s4 - m23 * s3) / det;

                result.m10 = (-m10 * c5 + m12 * c2 - m13 * c1) / det;
                result.m11 = (m00 * c5 - m02 * c2 + m03 * c1) / det;
                result.m12 = (-m30 * s5 + m32 * s2 - m33 * s1) / det;
                result.m13 = (m20 * s5 - m22 * s2 + m23 * s1) / det;

                result.m20 = (m10 * c4 - m11 * c2 + m13 * c0) / det;
                result.m21 = (-m00 * c4 + m01 * c2 - m03 * c0) / det;
                result.m22 = (m30 * s4 - m31 * s2 + m33 * s0) / det;
                result.m23 = (-m20 * s4 + m21 * s2 - m23 * s0) / det;

                result.m30 = (-m10 * c3 + m11 * c1 - m12 * c0) / det;
                result.m31 = (m00 * c3 - m01 * c1 + m02 * c0) / det;
                result.m32 = (-m30 * s3 + m31 * s1 - m32 * s0) / det;
                result.m33 = (m20 * s3 - m21 * s1 + m22 * s0) / det;

                return result;
            }
        }

        /// <summary>
        /// 通过行列索引访问元素
        /// </summary>
        /// <param name="row">行索引 (0-3)</param>
        /// <param name="col">列索引 (0-3)</param>
        public FixedPoint this[int row, int col]
        {
            get => this[row * 4 + col];
            set => this[row * 4 + col] = value;
        }

        /// <summary>
        /// 通过线性索引访问元素 (按行展开: 0=m00, 1=m01, ..., 15=m33)
        /// </summary>
        /// <param name="index">线性索引 (0-15)</param>
        public FixedPoint this[int index]
        {
            get => index switch
            {
                0 => m00, 1 => m01, 2 => m02, 3 => m03,
                4 => m10, 5 => m11, 6 => m12, 7 => m13,
                8 => m20, 9 => m21, 10 => m22, 11 => m23,
                12 => m30, 13 => m31, 14 => m32, 15 => m33,
                _ => throw new IndexOutOfRangeException("Invalid Matrix4x4 index!"),
            };
            set
            {
                switch (index)
                {
                    case 0: m00 = value; break; case 1: m01 = value; break;
                    case 2: m02 = value; break; case 3: m03 = value; break;
                    case 4: m10 = value; break; case 5: m11 = value; break;
                    case 6: m12 = value; break; case 7: m13 = value; break;
                    case 8: m20 = value; break; case 9: m21 = value; break;
                    case 10: m22 = value; break; case 11: m23 = value; break;
                    case 12: m30 = value; break; case 13: m31 = value; break;
                    case 14: m32 = value; break; case 15: m33 = value; break;
                    default: throw new IndexOutOfRangeException("Invalid Matrix4x4 index!");
                }
            }
        }

        #endregion

        #region 静态构造方法

        /// <summary>
        /// 从平移、旋转、缩放创建变换矩阵 (TRS 顺序)
        /// </summary>
        /// <param name="pos">平移向量</param>
        /// <param name="q">旋转四元数</param>
        /// <param name="s">缩放向量</param>
        // ReSharper disable once InconsistentNaming
        public static Matrix4x4 TRS(Vector3 pos, Quaternion q, Vector3 s)
        {
            // 四元数 → 旋转矩阵元素 (2倍分量用于简化公式)
            FixedPoint doubleX = q.x + q.x;
            FixedPoint doubleY = q.y + q.y;
            FixedPoint doubleZ = q.z + q.z;
            FixedPoint xx = q.x * doubleX;
            FixedPoint xy = q.x * doubleY;
            FixedPoint xz = q.x * doubleZ;
            FixedPoint yy = q.y * doubleY;
            FixedPoint yz = q.y * doubleZ;
            FixedPoint zz = q.z * doubleZ;
            FixedPoint wx = q.w * doubleX;
            FixedPoint wy = q.w * doubleY;
            FixedPoint wz = q.w * doubleZ;

            // 组合旋转和缩放到前三列，平移到第四列
            Matrix4x4 result = default;
            result.m00 = (FixedPoint.One - yy - zz) * s.x;
            result.m01 = (xy - wz) * s.y;
            result.m02 = (xz + wy) * s.z;
            result.m03 = pos.x;

            result.m10 = (xy + wz) * s.x;
            result.m11 = (FixedPoint.One - xx - zz) * s.y;
            result.m12 = (yz - wx) * s.z;
            result.m13 = pos.y;

            result.m20 = (xz - wy) * s.x;
            result.m21 = (yz + wx) * s.y;
            result.m22 = (FixedPoint.One - xx - yy) * s.z;
            result.m23 = pos.z;

            result.m33 = FixedPoint.One;
            return result;
        }

        /// <summary>
        /// 创建缩放矩阵
        /// </summary>
        /// <param name="vector">缩放向量</param>
        public static Matrix4x4 Scale(Vector3 vector)
        {
            Matrix4x4 result = default;
            result.m00 = vector.x;
            result.m11 = vector.y;
            result.m22 = vector.z;
            result.m33 = FixedPoint.One;
            return result;
        }

        /// <summary>
        /// 创建平移矩阵
        /// </summary>
        /// <param name="vector">平移向量</param>
        public static Matrix4x4 Translate(Vector3 vector)
        {
            Matrix4x4 result = Identity;
            result.m03 = vector.x;
            result.m13 = vector.y;
            result.m23 = vector.z;
            return result;
        }

        /// <summary>
        /// 创建旋转矩阵 (从四元数)
        /// </summary>
        /// <param name="q">旋转四元数</param>
        public static Matrix4x4 Rotate(Quaternion q)
        {
            return TRS(Vector3.Zero, q, Vector3.One);
        }

        #endregion

        #region 公共方法

        /// <summary>
        /// 获取指定列 (0-3)
        /// </summary>
        /// <param name="index">列索引</param>
        public Vector4 GetColumn(int index)
        {
            return index switch
            {
                0 => new Vector4(m00, m10, m20, m30),
                1 => new Vector4(m01, m11, m21, m31),
                2 => new Vector4(m02, m12, m22, m32),
                3 => new Vector4(m03, m13, m23, m33),
                _ => throw new IndexOutOfRangeException("Invalid column index!"),
            };
        }

        /// <summary>
        /// 设置指定列 (0-3)
        /// </summary>
        /// <param name="index">列索引</param>
        /// <param name="column">列向量</param>
        public void SetColumn(int index, Vector4 column)
        {
            this[0, index] = column.x;
            this[1, index] = column.y;
            this[2, index] = column.z;
            this[3, index] = column.w;
        }

        /// <summary>
        /// 获取指定行 (0-3)
        /// </summary>
        /// <param name="index">行索引</param>
        public Vector4 GetRow(int index)
        {
            return index switch
            {
                0 => new Vector4(m00, m01, m02, m03),
                1 => new Vector4(m10, m11, m12, m13),
                2 => new Vector4(m20, m21, m22, m23),
                3 => new Vector4(m30, m31, m32, m33),
                _ => throw new IndexOutOfRangeException("Invalid row index!"),
            };
        }

        /// <summary>
        /// 设置指定行 (0-3)
        /// </summary>
        /// <param name="index">行索引</param>
        /// <param name="row">行向量</param>
        public void SetRow(int index, Vector4 row)
        {
            this[index, 0] = row.x;
            this[index, 1] = row.y;
            this[index, 2] = row.z;
            this[index, 3] = row.w;
        }

        /// <summary>
        /// 变换三维点 (应用完整 4×4 变换，含透视除法)
        /// </summary>
        /// <param name="point">输入点</param>
        public Vector3 MultiplyPoint(Vector3 point)
        {
            FixedPoint rx = m00 * point.x + m01 * point.y + m02 * point.z + m03;
            FixedPoint ry = m10 * point.x + m11 * point.y + m12 * point.z + m13;
            FixedPoint rz = m20 * point.x + m21 * point.y + m22 * point.z + m23;
            FixedPoint rw = m30 * point.x + m31 * point.y + m32 * point.z + m33;

            // 透视除法: w ≠ 0 且 w ≠ 1 时需要除以 w 还原齐次坐标
            if (rw != FixedPoint.Zero && rw != FixedPoint.One)
            {
                rx /= rw;
                ry /= rw;
                rz /= rw;
            }

            return new Vector3(rx, ry, rz);
        }

        /// <summary>
        /// 变换三维点 (忽略第四行，适用于仿射变换，比 MultiplyPoint 更快)
        /// </summary>
        /// <param name="point">输入点</param>
        // ReSharper disable once InconsistentNaming
        public Vector3 MultiplyPoint3x4(Vector3 point)
        {
            return new Vector3(
                m00 * point.x + m01 * point.y + m02 * point.z + m03,
                m10 * point.x + m11 * point.y + m12 * point.z + m13,
                m20 * point.x + m21 * point.y + m22 * point.z + m23);
        }

        /// <summary>
        /// 变换方向向量 (忽略平移，仅应用旋转和缩放)
        /// </summary>
        /// <param name="vector">输入方向</param>
        public Vector3 MultiplyVector(Vector3 vector)
        {
            return new Vector3(
                m00 * vector.x + m01 * vector.y + m02 * vector.z,
                m10 * vector.x + m11 * vector.y + m12 * vector.z,
                m20 * vector.x + m21 * vector.y + m22 * vector.z);
        }

        /// <summary>
        /// 返回字符串表示
        /// </summary>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture,
                "{0:F2}\t{1:F2}\t{2:F2}\t{3:F2}\n{4:F2}\t{5:F2}\t{6:F2}\t{7:F2}\n{8:F2}\t{9:F2}\t{10:F2}\t{11:F2}\n{12:F2}\t{13:F2}\t{14:F2}\t{15:F2}",
                m00.RawDouble, m01.RawDouble, m02.RawDouble, m03.RawDouble,
                m10.RawDouble, m11.RawDouble, m12.RawDouble, m13.RawDouble,
                m20.RawDouble, m21.RawDouble, m22.RawDouble, m23.RawDouble,
                m30.RawDouble, m31.RawDouble, m32.RawDouble, m33.RawDouble);
        }

        /// <summary>
        /// 返回哈希码
        /// </summary>
        public override int GetHashCode()
        {
            return GetColumn(0).GetHashCode() ^ (GetColumn(1).GetHashCode() << 2)
                   ^ (GetColumn(2).GetHashCode() >> 2) ^ (GetColumn(3).GetHashCode() >> 1);
        }

        /// <summary>
        /// 判断与任意对象是否相等
        /// </summary>
        /// <param name="other">要比较的对象</param>
        public override bool Equals(object other)
        {
            return other is Matrix4x4 mat && Equals(mat);
        }

        /// <summary>
        /// 判断与另一个矩阵是否相等
        /// </summary>
        /// <param name="other">要比较的矩阵</param>
        public bool Equals(Matrix4x4 other)
        {
            return GetColumn(0) == other.GetColumn(0) && GetColumn(1) == other.GetColumn(1)
                   && GetColumn(2) == other.GetColumn(2) && GetColumn(3) == other.GetColumn(3);
        }

        #endregion

        #region 运算符

        /// <summary>
        /// 矩阵乘法
        /// </summary>
        /// <param name="lhs">左矩阵</param>
        /// <param name="rhs">右矩阵</param>
        public static Matrix4x4 operator *(Matrix4x4 lhs, Matrix4x4 rhs)
        {
            // 标准 4×4 矩阵乘法: result[i,j] = Σ lhs[i,k] × rhs[k,j]
            Matrix4x4 result = default;
            result.m00 = lhs.m00 * rhs.m00 + lhs.m01 * rhs.m10 + lhs.m02 * rhs.m20 + lhs.m03 * rhs.m30;
            result.m01 = lhs.m00 * rhs.m01 + lhs.m01 * rhs.m11 + lhs.m02 * rhs.m21 + lhs.m03 * rhs.m31;
            result.m02 = lhs.m00 * rhs.m02 + lhs.m01 * rhs.m12 + lhs.m02 * rhs.m22 + lhs.m03 * rhs.m32;
            result.m03 = lhs.m00 * rhs.m03 + lhs.m01 * rhs.m13 + lhs.m02 * rhs.m23 + lhs.m03 * rhs.m33;

            result.m10 = lhs.m10 * rhs.m00 + lhs.m11 * rhs.m10 + lhs.m12 * rhs.m20 + lhs.m13 * rhs.m30;
            result.m11 = lhs.m10 * rhs.m01 + lhs.m11 * rhs.m11 + lhs.m12 * rhs.m21 + lhs.m13 * rhs.m31;
            result.m12 = lhs.m10 * rhs.m02 + lhs.m11 * rhs.m12 + lhs.m12 * rhs.m22 + lhs.m13 * rhs.m32;
            result.m13 = lhs.m10 * rhs.m03 + lhs.m11 * rhs.m13 + lhs.m12 * rhs.m23 + lhs.m13 * rhs.m33;

            result.m20 = lhs.m20 * rhs.m00 + lhs.m21 * rhs.m10 + lhs.m22 * rhs.m20 + lhs.m23 * rhs.m30;
            result.m21 = lhs.m20 * rhs.m01 + lhs.m21 * rhs.m11 + lhs.m22 * rhs.m21 + lhs.m23 * rhs.m31;
            result.m22 = lhs.m20 * rhs.m02 + lhs.m21 * rhs.m12 + lhs.m22 * rhs.m22 + lhs.m23 * rhs.m32;
            result.m23 = lhs.m20 * rhs.m03 + lhs.m21 * rhs.m13 + lhs.m22 * rhs.m23 + lhs.m23 * rhs.m33;

            result.m30 = lhs.m30 * rhs.m00 + lhs.m31 * rhs.m10 + lhs.m32 * rhs.m20 + lhs.m33 * rhs.m30;
            result.m31 = lhs.m30 * rhs.m01 + lhs.m31 * rhs.m11 + lhs.m32 * rhs.m21 + lhs.m33 * rhs.m31;
            result.m32 = lhs.m30 * rhs.m02 + lhs.m31 * rhs.m12 + lhs.m32 * rhs.m22 + lhs.m33 * rhs.m32;
            result.m33 = lhs.m30 * rhs.m03 + lhs.m31 * rhs.m13 + lhs.m32 * rhs.m23 + lhs.m33 * rhs.m33;

            return result;
        }

        /// <summary>
        /// 矩阵与四维向量相乘
        /// </summary>
        /// <param name="lhs">左矩阵</param>
        /// <param name="vector">右向量</param>
        public static Vector4 operator *(Matrix4x4 lhs, Vector4 vector)
        {
            return new Vector4(
                lhs.m00 * vector.x + lhs.m01 * vector.y + lhs.m02 * vector.z + lhs.m03 * vector.w,
                lhs.m10 * vector.x + lhs.m11 * vector.y + lhs.m12 * vector.z + lhs.m13 * vector.w,
                lhs.m20 * vector.x + lhs.m21 * vector.y + lhs.m22 * vector.z + lhs.m23 * vector.w,
                lhs.m30 * vector.x + lhs.m31 * vector.y + lhs.m32 * vector.z + lhs.m33 * vector.w);
        }

        /// <summary>
        /// 精确相等比较
        /// </summary>
        /// <param name="lhs">左操作数</param>
        /// <param name="rhs">右操作数</param>
        public static bool operator ==(Matrix4x4 lhs, Matrix4x4 rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// 不等比较
        /// </summary>
        /// <param name="lhs">左操作数</param>
        /// <param name="rhs">右操作数</param>
        public static bool operator !=(Matrix4x4 lhs, Matrix4x4 rhs) => !(lhs == rhs);

        #endregion
    }
}
