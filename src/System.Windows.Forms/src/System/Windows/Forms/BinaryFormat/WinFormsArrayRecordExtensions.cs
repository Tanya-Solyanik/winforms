// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection.Metadata;

namespace System.Windows.Forms.BinaryFormat;

internal static class WinFormsArrayRecordExtensions
{
    // Use the .NET assembly names, even if we store the .NET Framework type name on the clipboard,
    // this will be used to match against the .NET type.
    private readonly static TypeName s_booleanArrayTypeName = TypeName.Parse("System.Boolean[], System.Private.CoreLib");
    private readonly static TypeName s_byteArrayTypeName = TypeName.Parse("System.Byte[], System.Private.CoreLib");
    private readonly static TypeName s_charArrayTypeName = TypeName.Parse("System.Char[], System.Private.CoreLib");
    private readonly static TypeName s_decimalArrayTypeName = TypeName.Parse("System.Decimal[], System.Private.CoreLib");
    private readonly static TypeName s_doubleArrayTypeName = TypeName.Parse("System.Double[], System.Private.CoreLib");
    private readonly static TypeName s_int16ArrayTypeName = TypeName.Parse("System.Int16[], System.Private.CoreLib");
    private readonly static TypeName s_int32ArrayTypeName = TypeName.Parse("System.Int32[], System.Private.CoreLib");
    private readonly static TypeName s_int64ArrayTypeName = TypeName.Parse("System.Int64[], System.Private.CoreLib");
    private readonly static TypeName s_sbyteArrayTypeName = TypeName.Parse("System.SByte[], System.Private.CoreLib");
    private readonly static TypeName s_singleArrayTypeName = TypeName.Parse("System.Single[], System.Private.CoreLib");
    private readonly static TypeName s_timeSpanArrayTypeName = TypeName.Parse("System.TimeSpan[], System.Private.CoreLib");
    private readonly static TypeName s_dateTimeArrayTypeName = TypeName.Parse("System.DateTime[], System.Private.CoreLib");
    private readonly static TypeName s_uint16ArrayTypeName = TypeName.Parse("System.UInt16[], System.Private.CoreLib");
    private readonly static TypeName s_uint32ArrayTypeName = TypeName.Parse("System.UInt32[], System.Private.CoreLib");
    private readonly static TypeName s_uint64ArrayTypeName = TypeName.Parse("System.UInt64[], System.Private.CoreLib");

    internal static bool IsElementType<T>(this PrimitiveType primitiveType)
    {
        return primitiveType switch
        {
            PrimitiveType.Boolean => typeof(T) == typeof(bool[]),
            PrimitiveType.Byte => typeof(T) == typeof(byte[]),
            PrimitiveType.Char => typeof(T) == typeof(char[]),
            PrimitiveType.Decimal => typeof(T) == typeof(decimal[]),
            PrimitiveType.Double => typeof(T) == typeof(double[]),
            PrimitiveType.Int16 => typeof(T) == typeof(short[]),
            PrimitiveType.Int32 => typeof(T) == typeof(int[]),
            PrimitiveType.Int64 => typeof(T) == typeof(long[]),
            PrimitiveType.SByte => typeof(T) == typeof(sbyte[]),
            PrimitiveType.Single => typeof(T) == typeof(float[]),
            PrimitiveType.TimeSpan => typeof(T) == typeof(TimeSpan[]),
            PrimitiveType.DateTime => typeof(T) == typeof(DateTime[]),
            PrimitiveType.UInt16 => typeof(T) == typeof(ushort[]),
            PrimitiveType.UInt32 => typeof(T) == typeof(uint[]),
            PrimitiveType.UInt64 => typeof(T) == typeof(ulong[]),
            _ => throw new InvalidOperationException($"Unexpected primitive array type: '{primitiveType}'")
        };
    }

    internal static TypeName GetTypeName(this PrimitiveType primitiveType)
    {
        return primitiveType switch
        {
            PrimitiveType.Boolean => s_booleanArrayTypeName,
            PrimitiveType.Byte => s_byteArrayTypeName,
            PrimitiveType.Char => s_charArrayTypeName,
            PrimitiveType.Decimal => s_decimalArrayTypeName,
            PrimitiveType.Double => s_doubleArrayTypeName,
            PrimitiveType.Int16 => s_int16ArrayTypeName,
            PrimitiveType.Int32 => s_int32ArrayTypeName,
            PrimitiveType.Int64 => s_int64ArrayTypeName,
            PrimitiveType.SByte => s_sbyteArrayTypeName,
            PrimitiveType.Single => s_singleArrayTypeName,
            PrimitiveType.TimeSpan => s_timeSpanArrayTypeName,
            PrimitiveType.DateTime => s_dateTimeArrayTypeName,
            PrimitiveType.UInt16 => s_uint16ArrayTypeName,
            PrimitiveType.UInt32 => s_uint32ArrayTypeName,
            PrimitiveType.UInt64 => s_uint64ArrayTypeName,
            _ => throw new InvalidOperationException($"Unexpected primitive array type: '{primitiveType}'")
        };
    }
}
