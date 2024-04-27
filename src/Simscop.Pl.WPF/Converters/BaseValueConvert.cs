using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace Simscop.Pl.WPF.Converters;


public abstract class BaseValueConvert<T> : MarkupExtension, IValueConverter
    where T : class, new()
{
    /// <summary>
    /// 将输入的值转换成控件需要的值
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public abstract object Convert(object? value, Type targetType, object? parameter, CultureInfo culture);

    /// <summary>
    /// 反过来转换，一般不会用，但是RadioButton的时候经常会用到
    /// </summary>
    /// <param name="value"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public virtual object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();

    private static T? _instance;

    public override object ProvideValue(IServiceProvider serviceProvider)
        => _instance ??= new T();
}