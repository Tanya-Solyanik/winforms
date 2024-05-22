// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.BinaryFormat;

internal static class WinFormsBinaryFormattedObjectExtensions
{
    /// <summary>
    ///  Tries to get this object as a binary formatted <see cref="ImageListStreamer"/>.
    /// </summary>
    public static bool TryGetImageListStreamer(
        this BinaryFormattedObject format,
        out object? imageListStreamer)
    {
        return BinaryFormattedObjectExtensions.TryGet(Get, format, out imageListStreamer);

        static bool Get(BinaryFormattedObject format, [NotNullWhen(true)] out object? imageListStreamer)
        {
            imageListStreamer = null;

            if (format.RootRecord is not ClassWithMembersAndTypes types
                || types.ClassInfo.Name != typeof(ImageListStreamer).FullName
                || format[3] is not ArraySinglePrimitive<byte> data)
            {
                return false;
            }

            Debug.Assert(data.ArrayObjects is byte[]);
            imageListStreamer = new ImageListStreamer((byte[])data.ArrayObjects);
            return true;
        }
    }

    /// <summary>
    ///  Tries to get this object as a binary formatted <see cref="Bitmap"/>.
    /// </summary>
    public static bool TryGetBitmap(this BinaryFormattedObject format, out object? bitmap)
    {
        bitmap = null;

        if (format.RootRecord is not ClassWithMembersAndTypes types
            || types.ClassInfo.Name != typeof(Bitmap).FullName
            || format[3] is not ArraySinglePrimitive<byte> data)
        {
            return false;
        }

        Debug.Assert(data.ArrayObjects is byte[]);
        bitmap = new Bitmap(new MemoryStream((byte[])data.ArrayObjects));
        return true;
    }

    /// <summary>
    ///  Try to get a supported object.
    /// </summary>
    public static bool TryGetObject(this BinaryFormattedObject format, [NotNullWhen(true)] out object? value) =>
        format.TryGetFrameworkObject(out value)
        || format.TryGetBitmap(out value)
        || format.TryGetImageListStreamer(out value);

    /// <summary>
    ///  Try to get an object of <typeparamref name="T"/> type, if that type is supported by the binary format.
    /// </summary>
    public static bool TryGetObject<T>(this BinaryFormattedObject format, [NotNullWhen(true)] out T? value) where T : class
    {
        value = null;
        if (Contains(format.RootRecord as ClassRecord))
        {
            value = format.Deserialize() as T;
            return value is not null;
        }

        return false;

        bool Contains(ClassRecord? record)
        {
            if (record is null)
            {
                return false;
            }

            string typeName = typeof(T).FullName!;
            string assemblyName = typeof(T).Assembly.FullName!;

            // This class is in System.Runtime.dll
            if (assemblyName == typeof(object).Assembly.FullName)
            {
                return record.LibraryId.IsNull && record.Name == typeName;
            }

            return record.ClassInfo.Name == typeName
                && format[record.LibraryId] is BinaryLibrary library
                && library.LibraryName == assemblyName;
        }
    }
}
