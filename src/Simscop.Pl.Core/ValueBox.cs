using System.ComponentModel;
using Simscop.Pl.Core.Converters;

namespace Simscop.Pl.Core;

/// <summary>
/// 灰度等级范围
/// </summary>
/// <param name="Start"></param>
/// <param name="End"></param>
public record LevelRange(int Start, int End);

/// <summary>
/// 图像的尺寸
/// </summary>
/// <param name="Width"></param>
/// <param name="Height"></param>
public record Size(int Width, int Height);

/// <summary>
/// 顺序为左上右下
/// </summary>
[TypeConverter(typeof(ThicknessConverter))]
public struct Vector4d
{
    public double V1 { get; set; }

    public double V2 { get; set; }

    public double V3 { get; set; }

    public double V4 { get; set; }

    public Vector4d(double v1, double v2, double v3, double v4)
    {
        V1 = v1;
        V2 = v2;
        V3 = v3;
        V4 = v4;
    }

    public Vector4d(double v)
    {
        V1 = v;
        V2 = v;
        V3 = v;
        V4 = v;
    }

    public double this[int index]
    {
        readonly get => index switch
        {
            0 => V1,
            1 => V2,
            2 => V3,
            3 => V4,
            _ => throw new IndexOutOfRangeException()
        };
        set => _ = index switch
        {
            0 => V1 = value,
            1 => V2 = value,
            2 => V3 = value,
            3 => V4 = value,
            _ => throw new IndexOutOfRangeException()
        };
    }

    public bool IsNegative() => V1 <= 0 || V2 <= 0 || V3 <= 0 || V4 <= 0;

    public override string ToString() => $"{V1},{V2},{V3},{V4}";
}