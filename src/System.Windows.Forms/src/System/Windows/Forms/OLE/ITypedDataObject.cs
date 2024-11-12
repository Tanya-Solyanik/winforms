// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection.Metadata;

namespace System.Windows.Forms;

public interface ITypedDataObject
{
    /// <summary>
    ///  Retrieves data associated with data format named after <typeparamref name="T"/>,
    ///  if that data is of type <typeparamref name="T"/>.
    /// </summary>
    /// <returns>
    ///  <see langword="true"/> if the data of this format is present and the value is
    ///  of a matching type and that value can be successfully retrieved, or <see langword="false"/>
    ///  if the format is not present or the value is not of the right type.
    /// </returns>
    bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        [NotNullWhen(true), MaybeNullWhen(false)] out T data);

    /// <summary>
    ///  Retrieves data associated with the specified data format if that data is of type <typeparamref name="T"/>.
    /// </summary>
    /// <returns>
    ///  <see langword="true"/> if the data of this format is present and the value is
    ///  of a matching type and that value can be successfully retrieved, or <see langword="false"/>
    ///  if the format is not present or the value is not of the right type.
    /// </returns>
    bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        string format,
        [NotNullWhen(true), MaybeNullWhen(false)] out T data);

    /// <summary>
    ///  Retrieves data associated with the specified data format, using
    ///  <paramref name="autoConvert"/> to determine whether to convert the data to another format,
    ///  if that data is of type <typeparamref name="T"/>.
    /// </summary>
    /// <returns>
    ///  <see langword="true"/> if the data of this format is present and the value is
    ///  of a matching type and that value can be successfully retrieved, or <see langword="false"/>
    ///  if the format is not present or the value is not of the right type.
    /// </returns>
    bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        string format,
        bool autoConvert,
        [NotNullWhen(true), MaybeNullWhen(false)] out T data);

    /// <summary>
    ///  Retrieves data associated with the specified data format, using
    ///  <paramref name="autoConvert"/> to determine whether to convert the data to the  format,
    ///  if that data is assignable to <typeparamref name="T"/>.
    ///  Will use <paramref name="resolver"/> with the binary formatter if needed.
    ///  <paramref name="resolver"/> is implemented by the user and should return the allowed types or
    ///  throw a <see cref="NotSupportedException"/>.
    /// </summary>
    /// <returns>
    ///  <see langword="true"/> if the data of this format is present and the value is
    ///  of a matching type and that value can be successfully retrieved, or <see langword="false"/>
    ///  if the format is not present or the value is not of the right type.
    /// </returns>
    bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        string format,
        Func<TypeName, Type> resolver,
        bool autoConvert,
        [NotNullWhen(true), MaybeNullWhen(false)] out T data);
}
