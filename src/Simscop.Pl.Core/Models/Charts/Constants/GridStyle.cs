// ReSharper disable once CheckNamespace
namespace Simscop.Pl.Core.Models.Charts;

[Flags]
public enum GridStyle
{
    /// <summary>
    /// 
    /// </summary>
    None = 0,

    /// <summary>
    /// 
    /// </summary>
    Vertical = 1,

    /// <summary>
    /// 
    /// </summary>
    Horizontal = 2,

    /// <summary>
    /// 
    /// </summary>
    All = Vertical | Horizontal
}