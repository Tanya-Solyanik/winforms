// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Reflection.Metadata;

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

    public static bool TryGetRootTypeName(this BinaryFormattedObject format, [NotNullWhen(true)] out TypeName? name)
    {
        name = null;

        if (format.RootRecord is not ClassRecord record)
        {
            return false;
        }

        string typeName = record.Name;
        string? assemblyName = record.LibraryId.IsNull
            ? typeof(object).Assembly.FullName
            : format[record.LibraryId] is BinaryLibrary library ? library.LibraryName : null;

        return TypeName.TryParse($"{typeName}, {assemblyName}", out name);
    }

    /// <summary>
    ///  Verify if this binary formatter object contains an object of <typeparamref name="T"/> type, if that type is supported by the binary format.
    /// </summary>
    public static bool Contains<T>(this BinaryFormattedObject format)
    {
        // TanyaSo: What happens with the TypeForwardedFrom  - test bitmap
        (string typeName, string? assemblyName) = GetNamesFromType(typeof(T));
        // format.RootRecord.GetType().Name "BinaryObjectString"
        switch (format.RootRecord)
        {
            case ClassRecord record:
                if (!format.TryGetRootTypeName(out TypeName? name))
                {
                    return false;
                }

                return (typeName, assemblyName) == GetNamesFromTypeName(name);

             default:
                return false;
        }

        static (string typeName, string? assemblyName) GetNamesFromType(Type type)
        {
            if (!TypeName.TryParse($"{type.FullName}, {type.Assembly.FullName}", out TypeName? parsed))
            {
                throw new InvalidOperationException();
            }

            string typeName = parsed.FullName;
            string? assemblyName = parsed.AssemblyName?.Name;

            return (typeName, assemblyName);
        }

        static (string typeName, string? assemblyName) GetNamesFromTypeName(TypeName name) => (name.FullName, name.AssemblyName?.Name);
    }
}
