// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Formats.Nrbf;

namespace System.Windows.Forms.Nrbf;

internal static class WinFormsSerializationRecordExtensions
{
    /// <summary>
    ///  Tries to get this object as a binary formatted <see cref="ImageListStreamer"/>.
    /// </summary>
    public static bool TryGetImageListStreamer(
        this SerializationRecord record,
        out object? imageListStreamer)
    {
        return SerializationRecordExtensions.TryGet(Get, record, out imageListStreamer);

        static bool Get(SerializationRecord record, [NotNullWhen(true)] out object? imageListStreamer)
        {
            imageListStreamer = null;

            if (record is not ClassRecord types
                || !types.TypeNameMatches(typeof(ImageListStreamer))
                || !types.HasMember("Data")
                || types.GetRawValue("Data") is not SZArrayRecord<byte> data)
            {
                return false;
            }

            imageListStreamer = new ImageListStreamer(data.GetArray());
            return true;
        }
    }

    /// <summary>
    ///  Tries to get this object as a binary formatted <see cref="Bitmap"/>.
    /// </summary>
    public static bool TryGetBitmap(this SerializationRecord record, out object? bitmap)
    {
        bitmap = null;

        if (record is not ClassRecord types
            || !types.TypeNameMatches(typeof(Bitmap))
            || !types.HasMember("Data")
            || types.GetRawValue("Data") is not SZArrayRecord<byte> data)
        {
            return false;
        }

        bitmap = new Bitmap(new MemoryStream(data.GetArray()));
        return true;
    }

    /// <summary>
    ///  Try to get a supported object. This supports common types used in WinForms that do not have type converters.
    /// </summary>
    public static bool TryGetResXObject(this SerializationRecord record, [NotNullWhen(true)] out object? value) =>
        record.TryGetFrameworkObject(out value)
        || record.TryGetBitmap(out value)
        || record.TryGetImageListStreamer(out value);

    public static bool TryGetCommonObject(this SerializationRecord record, [NotNullWhen(true)] out object? value) =>
        record.TryGetResXObject(out value)
        || record.TryGetDrawingPrimitivesObject(out value);

    private static bool IsNrbfPrimitiveType<T>() =>
        typeof(T) == typeof(bool)
        || typeof(T) == typeof(byte)
        || typeof(T) == typeof(sbyte)
        || typeof(T) == typeof(char)
        || typeof(T) == typeof(short)
        || typeof(T) == typeof(ushort)
        || typeof(T) == typeof(int)
        || typeof(T) == typeof(uint)
        || typeof(T) == typeof(long)
        || typeof(T) == typeof(ulong)
        || typeof(T) == typeof(float)
        || typeof(T) == typeof(double)
        || typeof(T) == typeof(decimal)
        || typeof(T) == typeof(DateTime)
        || typeof(T) == typeof(TimeSpan)
        || typeof(T) == typeof(nint)
        || typeof(T) == typeof(nuint);

    public static bool TryGetCommonObject<T>(this SerializationRecord record, [NotNullWhen(true)] out object? value)
    {
        Type? unwrapped;
        if (!SerializationRecordExtensions.TryGet(Get, record, out value))
        {
            unwrapped = Formatter.NullableUnwrap(typeof(T));
            return unwrapped is not null && SerializationRecordExtensions.TryGet(GetUnwrapped, record, out value);
        }

        return true;

        bool Get(SerializationRecord record, [NotNullWhen(true)] out object? value)
        {
            value = null;

            if (typeof(T) == typeof(object))
            {
                return record.TryGetCommonObject(out value);
            }

            if (typeof(T) == typeof(string))
            {
                if (record.RecordType is SerializationRecordType.BinaryObjectString)
                {
                    value = ((PrimitiveTypeRecord<string>)record).Value;
                    return true;
                }

                return false;
            }

            if (IsNrbfPrimitiveType<T>())
            {
                if (record.RecordType is SerializationRecordType.MemberPrimitiveTyped && record is PrimitiveTypeRecord<T> recordOfT)
                {
                    value = recordOfT.Value!;
                    return true;
                }

                return false;
            }

            if (typeof(T) == typeof(Point))
            {
                return record.TryGetPoint(out value);
            }

            if (typeof(T) == typeof(PointF))
            {
                return record.TryGetPointF(out value);
            }

            if (typeof(T) == typeof(Size))
            {
                return record.TryGetSize(out value);
            }

            if (typeof(T) == typeof(SizeF))
            {
                return record.TryGetSizeF(out value);
            }

            if (typeof(T) == typeof(Rectangle))
            {
                return record.TryGetRectangle(out value);
            }

            if (typeof(T) == typeof(RectangleF))
            {
                return record.TryGetRectangleF(out value);
            }

            if (typeof(T) == typeof(Color))
            {
                return record.TryGetColor(out value);
            }

            if (typeof(T) == typeof(Bitmap))
            {
                return record.TryGetBitmap(out value);
            }

            if (typeof(T) == typeof(ImageListStreamer))
            {
                return record.TryGetImageListStreamer(out value);
            }

            if (typeof(T) == typeof(NotSupportedException))
            {
                return record.TryGetNotSupportedException(out value);
            }

            return GetArrayOrList(typeof(T), out value);
        }

        bool GetUnwrapped(SerializationRecord record, [NotNullWhen(true)] out object? value)
        {
            value = null;

            if (unwrapped == typeof(object))
            {
                return record.TryGetCommonObject(out value);
            }

            if (unwrapped == typeof(string))
            {
                if (record.RecordType is SerializationRecordType.BinaryObjectString)
                {
                    value = ((PrimitiveTypeRecord<string>)record).Value;
                    return true;
                }

                return false;
            }

            if (Private.Windows.Core.BinaryFormat.TypeInfo.GetPrimitiveType(unwrapped) != default)
            {
                if (record.RecordType is SerializationRecordType.MemberPrimitiveTyped)
                {
                    PrimitiveTypeRecord? recordOfT = Type.GetTypeCode(unwrapped) switch
                    {
                        TypeCode.Boolean => record as PrimitiveTypeRecord<bool>,
                        TypeCode.Char => record as PrimitiveTypeRecord<char>,
                        TypeCode.SByte => record as PrimitiveTypeRecord<sbyte>,
                        TypeCode.Byte => record as PrimitiveTypeRecord<byte>,
                        TypeCode.Int16 => record as PrimitiveTypeRecord<short>,
                        TypeCode.UInt16 => record as PrimitiveTypeRecord<ushort>,
                        TypeCode.Int32 => record as PrimitiveTypeRecord<int>,
                        TypeCode.UInt32 => record as PrimitiveTypeRecord<uint>,
                        TypeCode.Int64 => record as PrimitiveTypeRecord<long>,
                        TypeCode.UInt64 => record as PrimitiveTypeRecord<ulong>,
                        TypeCode.Single => record as PrimitiveTypeRecord<float>,
                        TypeCode.Double => record as PrimitiveTypeRecord<double>,
                        TypeCode.Decimal => record as PrimitiveTypeRecord<decimal>,
                        TypeCode.DateTime => record as PrimitiveTypeRecord<DateTime>,
                        TypeCode.String => record as PrimitiveTypeRecord<string>,
                        _ => record as PrimitiveTypeRecord<TimeSpan>
                    };

                    value = recordOfT?.Value!;
                    return recordOfT is not null;
                }

                return false;
            }

            if (unwrapped == typeof(Point))
            {
                return record.TryGetPoint(out value);
            }

            if (unwrapped == typeof(PointF))
            {
                return record.TryGetPointF(out value);
            }

            if (unwrapped == typeof(Size))
            {
                return record.TryGetSize(out value);
            }

            if (unwrapped == typeof(SizeF))
            {
                return record.TryGetSizeF(out value);
            }

            if (unwrapped == typeof(Rectangle))
            {
                return record.TryGetRectangle(out value);
            }

            if (unwrapped == typeof(RectangleF))
            {
                return record.TryGetRectangleF(out value);
            }

            if (unwrapped == typeof(Color))
            {
                return record.TryGetColor(out value);
            }

            if (unwrapped == typeof(Bitmap))
            {
                return record.TryGetBitmap(out value);
            }

            if (unwrapped == typeof(ImageListStreamer))
            {
                return record.TryGetImageListStreamer(out value);
            }

            if (unwrapped == typeof(NotSupportedException))
            {
                return record.TryGetNotSupportedException(out value);
            }

            return GetArrayOrList(unwrapped, out value);
        }

        bool GetArrayOrList(Type type, [NotNullWhen(true)] out object? value)
        {
            value = null;

            if (type.IsGenericType)
            {
                var arguments = type.GenericTypeArguments;
                if (arguments.Length != 1
                    || !type.IsConstructedGenericType
                    || type.Name != typeof(List<>).Name
                    || Private.Windows.Core.BinaryFormat.TypeInfo.GetPrimitiveType(arguments[0]) == default)
                {
                    return false;
                }

                var recordArguments = record.TypeName.GetGenericArguments();
                if (recordArguments.Length != 1 || recordArguments[0].FullName != arguments[0].FullName)
                {
                    return false;
                }

                return record.TryGetPrimitiveList(out value);
            }

            if (type.IsArray)
            {
                if (type.GetArrayRank() != 1
                    || type.GetElementType() is not Type elementType
                    || Private.Windows.Core.BinaryFormat.TypeInfo.GetPrimitiveType(elementType) == default
                    || elementType.FullName != record.TypeName.GetElementType().FullName)
                {
                    return false;
                }

                return record.TryGetPrimitiveArray(out value);
            }

            return false;
        }
    }

    public static bool TypeNameMatches<T>(this SerializationRecord record)
    {
        Type type = typeof(T);
        if (record.TypeNameMatches(type))
        {
            return true;
        }

        if (Formatter.NullableUnwrap(type) is { } unwrapped && record.TypeNameMatches(unwrapped))
        {
            return true;
        }

        return false;
    }
}
