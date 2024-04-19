using System.Collections;
using OpenCvSharp;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Simscop.Pl.Core.Converters;

public class ThicknessConverter : TypeConverter
{
    /// <inheritdoc/>
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) =>
        Type.GetTypeCode(sourceType) switch
        {
            TypeCode.Int16 => true,
            TypeCode.UInt16 => true,
            TypeCode.Int32 => true,
            TypeCode.UInt32 => true,
            TypeCode.Int64 => true,
            TypeCode.UInt64 => true,
            TypeCode.Single => true,
            TypeCode.Double => true,
            TypeCode.Decimal => true,
            TypeCode.String => true,
            _ => false
        };

    /// <inheritdoc/>
    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
        => destinationType == typeof(InstanceDescriptor) || destinationType == typeof(string);

    /// <inheritdoc/>
    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object source) =>
        source switch
        {
            null => throw this.GetConvertFromException(source),
            string s => String2Vector4d(s),
            double uniformLength => (object)new Vector4d(uniformLength),
            _ => (object)new Vector4d(Convert.ToDouble(source, (IFormatProvider)culture!))
        };

    private Vector4d? String2Vector4d(string s)
    {
        var str = s.Trim();
        var value = s.Split(new char[] { ' ', ',' });


        var vec = new Vector4d();

        switch (value.Length)
        {
            case 1:
            {
                return double.TryParse(value[0], out var v) ? new Vector4d(v) : null;
            }
            case 2:
            {
                for (var i = 0; i < 2; i++)
                {
                    if (double.TryParse(value[i], out var v))
                    {
                        vec[i] = v;
                        vec[i + 1] = v;
                    }
                    else
                        return null;
                }

                break;
            }
            case 4:
            {
                for (var i = 0; i < 4; i++)
                {
                    if (double.TryParse(value[i], out var v))
                        vec[i] = v;
                    else
                        return null;
                }

                break;
            }
            default:
                return null;
        }

        return vec;
    }

    /// <inheritdoc/>
    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
        var c = context;
        var v = value;


        throw new NotImplementedException();
    }
}