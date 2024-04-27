using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Simscop.Pl.Core.Converters;

public class EnumDescriptionConverter : EnumConverter
{
    public EnumDescriptionConverter(Type type)
        : base(type) { }

    /// <inheritdoc/>
    public override object ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type? destinationType)
    {
        if (destinationType != typeof(string)) return string.Empty;
        if (value == null) return string.Empty;
        
        var fieldInfo = value.GetType()!.GetField(value.ToString()!);
        
        if (fieldInfo == null) return value.ToString()!;
        
        var attribute = fieldInfo.GetCustomAttribute<DescriptionAttribute>(inherit: false);
        return attribute != null
            ? !string.IsNullOrEmpty(attribute.Description) ? attribute.Description : value.ToString()!
            : value.ToString()!;
    }
}