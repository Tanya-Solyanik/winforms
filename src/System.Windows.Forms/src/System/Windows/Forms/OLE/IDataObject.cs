// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Provides a format-independent mechanism for transferring data.
/// </summary>
public interface IDataObject
{
    /// <summary>
    ///  Retrieves the data associated with the specified data format, using
    ///  <paramref name="autoConvert"/> to determine whether to convert the data to the  format.
    /// </summary>
    object? GetData(string format, bool autoConvert);

    /// <summary>
    ///  Retrieves the data associated with the specified data format.
    /// </summary>
    object? GetData(string format);

    /// <summary>
    ///  Retrieves the data associated with the specified class type format.
    /// </summary>
    object? GetData(Type format);

    /// <summary>
    ///  Retrieves the data associated with the specified data format if that data is of type <typeparamref name="T"/>.
    /// </summary>
    T? GetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(string format) where T : class => GetData(format) as T;

    /// <summary>
    ///  Retrieves the data associated with the specified class type format if that data is of type <typeparamref name="T"/>.
    /// </summary>
    T? GetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>() where T : class => GetData<T>(typeof(T).FullName!);

    /// <summary>
    ///  Determines whether data stored in this instance is  associated with the
    ///  specified format, using autoConvert to determine whether to convert the
    ///  data to the format.
    /// </summary>
    bool GetDataPresent(string format, bool autoConvert);

    /// <summary>
    ///  Determines whether data stored in this instance is associated with, or
    ///  can be converted to, the specified format.
    /// </summary>
    bool GetDataPresent(string format);

    /// <summary>
    ///  Determines whether data stored in this instance is associated with, or
    ///  can be converted to, the specified format.
    /// </summary>
    bool GetDataPresent(Type format);

    /// <summary>
    ///  Determines whether data stored in this instance is associated with, or
    ///  can be converted to format <typeparamref name="T" />.
    /// </summary>
    bool GetDataPresent<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>() where T : class => GetDataPresent(typeof(T));

    /// <summary>
    ///  Gets a list of all formats that data stored in this instance is
    ///  associated with or can be converted to, using <paramref name="autoConvert"/> to determine
    ///  whether to retrieve all formats that the data can be converted to, or
    ///  only native data formats.
    /// </summary>
    string[] GetFormats(bool autoConvert);

    /// <summary>
    ///  Gets a list of all formats that data stored in this instance is
    ///  associated with or can be converted to.
    /// </summary>
    string[] GetFormats();

    /// <summary>
    ///  Stores the specified data and its associated format in this instance,
    ///  using autoConvert to specify whether the data can be converted to
    ///  another format.
    /// </summary>
    void SetData(string format, bool autoConvert, object? data);

    /// <summary>
    ///  Stores the specified data and its associated format in this instance.
    /// </summary>
    void SetData(string format, object? data);

    /// <summary>
    ///  Stores the specified data and its associated class type in this instance.
    /// </summary>
    void SetData(Type format, object? data);

    /// <summary>
    ///  Stores the specified data in this instance, using the class of the
    ///  data for the format.
    /// </summary>
    void SetData(object? data);

    /// <summary>
    ///  Stores the specified data and its associated format in this instance,
    ///  if <paramref name="format"/> supports <typeparamref name="T"/>.
    /// </summary>
    void SetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(string format, T data) where T : class =>
        SetData(format, (object)data);

    /// <inheritdoc cref="SetData(Type, object?)"/>
    void SetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(T data) where T : class =>
        SetData(typeof(T).FullName!, data);
}
