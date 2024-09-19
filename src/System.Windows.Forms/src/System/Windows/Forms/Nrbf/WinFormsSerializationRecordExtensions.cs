// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Drawing;
using System.Formats.Nrbf;
using System.Reflection.Metadata;

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
        return SerializationRecordExtensions.TryGet(Get, record, out value);

        static bool Get(SerializationRecord record, [NotNullWhen(true)] out object? value)
        {
            value = null;

            if (typeof(T) == typeof(object))
            {
                return record.TryGetCommonObject(out value);
            }

            if (typeof(string) == typeof(T))
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

            Type type = typeof(T);
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

                return record.TryGetPrimitiveList(out value);
            }

            if (typeof(T) == typeof(ArrayList))
            {
                return record.TryGetPrimitiveArrayList(out value);
            }

            if (typeof(T) == typeof(Hashtable))
            {
                return record.TryGetPrimitiveHashtable(out value);
            }

            if (type.IsArray)
            {
                if (type.GetArrayRank() != 1
                    || type.GetElementType() is not Type elementType
                    || Private.Windows.Core.BinaryFormat.TypeInfo.GetPrimitiveType(elementType) == default)
                {
                    return false;
                }

                return record.TryGetPrimitiveArray(out value);
            }

            return false;
        }
    }

    /// <summary>
    ///  Verify if this <paramref name="record"/> contains an object that can be assigned to <typeparamref name="T"/>.
    /// </summary>
    public static bool IsAssignableTo<T>(this SerializationRecord record)
    {
        Type type = typeof(T); // Type forwarding is taken care of
        if (record.TypeNameMatches(type))
        {
            return true;
        }

        Type unwrapped = Formatter.NullableUnwrap(type);
        if (unwrapped != type && record.TypeNameMatches(unwrapped))
        {
            return true;
        }

        switch (record)
        {
            case ClassRecord classRecord:
                {
                    return false;
                    // TanyaSo
                    // Type recordType = record.TypeResolver.GetType(classRecord.Name, classRecord.LibraryId);
                    // return recordType.IsAssignableTo(typeof(T));
                }

            case PrimitiveTypeRecord primitiveTypeRecord:
                return false;

            case ArrayRecord arrayRecord:
                Type requestedType = typeof(T);
                if (!type.IsArray)
                {
                    return typeof(T) == typeof(object) || typeof(T) == typeof(Array);
                }

                if (type != unwrapped && !unwrapped.IsArray)
                {
                    return unwrapped == typeof(object) || unwrapped == typeof(Array);
                }

                return false;
        }

        return false;
    }

    public static bool TryGetObject<T>(this SerializationRecord record, Func<TypeName, Type> resolver, out object? value)
    {
        Type? type = null;
        if (record.TypeNameMatches(typeof(T)))
        {
            type = typeof(T);
        }
        else
        {
         type =  resolver(record.TypeName) is Type type && !type.IsAssignableTo(typeof(T)))
        if (record is not ClassRecord classRecord)
        {
            value = null;
            return false;
        }

        TypeResolver typeResolver = new TypeResolver(resolver, classRecord);

        classRecord.TypeResolver.GetType(classRecord.Name, classRecord.LibraryId);

        value = null;
        return false;
    }
}
