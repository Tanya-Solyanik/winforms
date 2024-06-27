// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;

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

    private static readonly TypeName s_stringNameNET = TypeName.Parse("System.String, System.Runtime");
    // private static readonly TypeName s_stringNameNETFX = TypeName.Parse("System.String, mscorlib");

    public static TypeName GetRootTypeName(this BinaryFormattedObject format)
    {
        return format.RootRecord switch
        {
            ClassRecord record => GetTypeNameFromRecord(record),
            BinaryObjectString => s_stringNameNET,
            ArraySingleString => GetTypeName<string[]>(),
            ArraySingleObject => GetTypeName<object[]>(),
            IPrimitiveTypeRecord primitiveArray => primitiveArray.GetTypeName(),
            // IBinaryArray binaryArray => binaryArray.,

            _ => throw new InvalidOperationException($"RootRecord should not be of type {format.RootRecord.GetType().Name}."),
        };

        static TypeName GetTypeName<T>() =>
           TypeName.Parse($"{typeof(T).FullName}, {typeof(T).Assembly.FullName}");

        TypeName GetTypeNameFromRecord(ClassRecord record)
        {
            (string typeName, string assemblyName) = format.GetTypeNameFromClassRecord(record);
            return TypeName.Parse($"{typeName}, {assemblyName}");
        }
    }

    private static (string typeName, string assemblyName) GetTypeNameFromClassRecord(this BinaryFormattedObject format, ClassRecord record)
    {
        string typeName = record.Name;
        string assemblyName = record.LibraryId.IsNull
            ? typeof(object).Assembly.FullName!
            : ((BinaryLibrary)format[record.LibraryId]).LibraryName;
        Debug.Assert(assemblyName is not null);
        assemblyName = assemblyName.Split(',')[0].Trim();

        return (record.Name, assemblyName);
    }

    /// <summary>
    ///  Verify if this binary formatter object contains an object of type <typeparamref name="T"/>,
    ///  if that type is supported by the binary format.
    /// </summary>
    public static bool Contains<T>(this BinaryFormattedObject format)
    {
        switch (format.RootRecord)
        {
            case ClassRecord record:
                var (recordTypeName, recordAssemblyName)= format.GetTypeNameFromClassRecord(record);

                Type requestedType = typeof(T);
                var (requestedTypeName, requestedAssemblyName) = GetNamesFromTypeSimpleNullable(requestedType);

                // Name of the root record could be a .NET Framework or .NET type, first compare to T as the .NET type,
                // then if T has a TypeForwardedFrom attribute, compare get assembly name type was forwarded from.
                if (string.Equals(requestedTypeName, recordTypeName, StringComparison.Ordinal)
                    && (string.Equals(requestedAssemblyName, recordAssemblyName, StringComparison.Ordinal)
                        || string.Equals(recordAssemblyName, GetAssemblyShortName(requestedType), StringComparison.Ordinal)))
                {
                    return true;
                }

                return false;

            case BinaryObjectString:
                return GetNamesFromTypeSimpleNullable(typeof(T)) == GetNamesFromTypeSimple(typeof(string));

            case ArraySingleString:
                // TanyaSo: Add handling of nullable element types.
                return GetNamesFromTypeSimpleNullable(typeof(T)) == GetNamesFromTypeSimple(typeof(string[]));

            case ArraySingleObject:
                return GetNamesFromTypeSimpleNullable(typeof(T)) == GetNamesFromTypeSimple(typeof(object[]));

            case IPrimitiveTypeRecord primitiveArray:
                return GetNamesFromTypeSimpleNullable(typeof(T)) == GetNamesFromTypeName(primitiveArray.GetTypeName());

            case IBinaryArray binaryArray:
                Type type = typeof(T);
                if (!typeof(T).IsArray || typeof(T).GetArrayRank() != binaryArray.Rank)
                {
                    return false;
                }

                Type elementType = typeof(T).GetElementType()!;
                // binaryArray.TypeInfo.
                return true;

            default:
                throw new InvalidOperationException($"RootRecord should not be of type {format.RootRecord.GetType().Name}.");
        }

        static (string typeName, string? assemblyName) GetNamesFromTypeSimpleNullable(Type type)
        {
            type = Formatter.NullableUnwrap(type);
            return (type.FullName!, type.Assembly.FullName!.Split(',')[0].Trim());
         }

        static (string typeName, string? assemblyName) GetNamesFromTypeSimple(Type type) =>
            (type.FullName!, type.Assembly.FullName!.Split(',')[0].Trim());

        static (string typeName, string? assemblyName) GetNamesFromTypeName(TypeName name) =>
            (name.FullName, name.AssemblyName?.Name);

        static string GetAssemblyShortName(Type type)
        {
            // Special case types like arrays.
            Type attributedType = type;
            while (attributedType.HasElementType)
            {
                attributedType = attributedType.GetElementType()!;
            }

            attributedType = Formatter.NullableUnwrap(attributedType);

            foreach (Attribute first in attributedType.GetCustomAttributes(typeof(TypeForwardedFromAttribute), inherit: false))
            {
                return ((TypeForwardedFromAttribute)first).AssemblyFullName.Split(',')[0].Trim();
            }

            return type.Assembly.FullName!;
        }
    }
}
