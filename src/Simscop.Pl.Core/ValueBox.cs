using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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