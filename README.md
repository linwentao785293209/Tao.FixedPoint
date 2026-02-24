# Tao.FixedPoint

C# 确定性定点数数学库，为帧同步游戏提供跨平台一致的数值运算。

## 解决什么问题

帧同步架构要求所有客户端独立模拟，只同步输入，因此每一步计算的结果必须在所有平台上**完全相同**——不是"差不多"，而是逐位一致。

浮点数（`float` / `double`）本身精度够用，但不同 CPU、不同编译器、甚至同一平台的 Debug 和 Release 模式都可能产生微小差异。这些差异经过上百帧的累积放大，最终导致各端逻辑完全分叉。

本库将所有运算建立在整数之上（Q10 定点格式，放大 1024 倍），从根本上消除跨平台不一致问题。

## 特性

- **Q10 定点格式**：精度约 0.001，有效范围约 ±21 亿，以 `long` 存储
- **完整的数学函数**：四则运算、取整、幂运算、对数、指数、平方根（牛顿迭代法）
- **查找表三角函数**：Sin / Cos（线性插值）、Acos（线性插值）、Atan2（双线性插值），不依赖 `System.Math`
- **向量与矩阵**：Vector2、Vector3、Vector4、Quaternion、Matrix4x4，API 对齐 Unity
- **确定性随机数**：Xorshift32 算法，不依赖 `System.Random`，跨运行时结果一致
- **饱和算术**：溢出和除零返回饱和值，不抛异常、不回绕
- **运算符重载**：`+` `-` `*` `/` `%` `<<` `>>` 全部重载，写法与原生数值类型一致
- **性能优化**：所有运算符标记 `AggressiveInlining`，核心类型均为 `struct`，零 GC

## 项目结构

```text
Tao.FixedPoint/
├── DotNet/
│   ├── Tao.FixedPoint/                    ← 源码库（.NET Standard 2.0）
│   │   └── Tao/FixedPoint/
│   │       ├── Core/FixedPoint.cs         ← 定点数核心结构体
│   │       ├── Math/Math.cs               ← 数学函数
│   │       ├── Math/LookupTable/          ← Sin/Cos、Acos、Atan2 查找表
│   │       ├── Vector/                    ← Vector2、Vector3、Vector4
│   │       ├── Quaternion/                ← 四元数
│   │       ├── Matrix/                    ← Matrix4x4
│   │       └── Random/                    ← 确定性随机数
│   └── Tao.FixedPoint.DotNetTest/         ← .NET 测试（MSTest）
├── Unity/
│   └── Tao.FixedPoint.UnityTest/          ← Unity 测试项目（NUnit）
│       └── Assets/Tao/FixedPoint/
│           ├── [源码副本]                 ← 与 DotNet 版本保持同步
│           └── UnityTest/                 ← Unity 测试用例
├── Document/
│   ├── Markdown/
│   │   ├── 1.基础知识必备.md              ← 二进制、补码、移位等前置知识
│   │   ├── 2.定点数实现原理.md            ← 设计思路与实现细节
│   │   ├── 3.API快速参考.md              ← 日常开发速查手册
│   │   └── 4.规范合规说明.md              ← 编码规范与帧同步约束
│   └── Image/                             ← 文档配图
└── Tao.FixedPoint.sln                     ← 解决方案（包含 .NET 和 Unity 项目）
```

## 快速上手

```csharp
using Tao.FixedPoint;

// 创建定点数（int 隐式转换）
FixedPoint hp = 100;
FixedPoint speed = 6;
FixedPoint damage = hp - speed * 3;    // 82

// 向量运算
Vector3 pos = new Vector3(10, 0, 5);
Vector3 target = new Vector3(30, 0, 8);
Vector3 next = Vector3.MoveTowards(pos, target, speed);

// 三角函数（查找表实现，跨平台确定）
FixedPoint angle = Math.Atan2(new FixedPoint(1), new FixedPoint(1));
// angle ≈ π/4

// 确定性随机数
RandomSeed rng = new RandomSeed(42);
int roll = rng.Range(0, 100);    // [0, 100) 范围内的随机整数

// 显示给 UI（必须显式转换，提醒开发者离开定点世界）
float uiHp = (float)hp;
```

## 核心使用原则

```text
策划配置 (float)  ──→  逻辑核心 (FixedPoint)  ──→  网络传输 (long)
                                                ──→  UI 显示 (float)
                                                ←── 禁止回流
```

- 逻辑层全程使用 `FixedPoint` / `Math` / `Vector*`，禁止 `float`、`System.Math`、`System.Random`
- 网络传输使用 `FixedValue`（`long` 整数），不传 `float` 或字符串
- 逻辑层和表现层各自持有独立的 `RandomSeed` 实例

## 文档

| 文档 | 适用场景 |
|---|---|
| [1.基础知识必备](Document/Markdown/1.基础知识必备.md) | 刚接触定点数，需要补前置知识 |
| [2.定点数实现原理](Document/Markdown/2.定点数实现原理.md) | 想了解为什么这样设计、每步解决了什么问题 |
| [3.API快速参考](Document/Markdown/3.API快速参考.md) | 日常开发时查接口用法 |
| [4.规范合规说明](Document/Markdown/4.规范合规说明.md) | 维护库代码或做 Code Review |
