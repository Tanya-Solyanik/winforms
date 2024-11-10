// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public static class DataObjectExtensions
{
    /// <inheritdoc cref="ITypedDataObject.TryGetData{T}(out T)"/>
    /// <exception cref="ArgumentException">if the <paramref name="dataObject"/> does not implement <see cref="ITypedDataObject" />.</exception>
    /// <exception cref="ArgumentNullException">if the <paramref name="dataObject"/> is <see langword="null"/></exception>
    public static bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        this IDataObject dataObject,
        [NotNullWhen(true), MaybeNullWhen(false)] out T data)
    {
        ArgumentNullException.ThrowIfNull(dataObject);

        if (dataObject is not ITypedDataObject typed)
        {
            throw new ArgumentException($"DataObject should implement {nameof(ITypedDataObject)} interface.", nameof(dataObject));
        }

        return typed.TryGetData(out data);
    }

    /// <inheritdoc cref="ITypedDataObject.TryGetData{T}(string, out T)"/>
    /// <exception cref="ArgumentException">if the <paramref name="dataObject"/> does not implement <see cref="ITypedDataObject" />.</exception>
    /// <exception cref="ArgumentNullException">if the <paramref name="dataObject"/> is <see langword="null"/></exception>
    public static bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        this IDataObject dataObject,
        string format,
        [NotNullWhen(true), MaybeNullWhen(false)] out T data)
    {
        ArgumentNullException.ThrowIfNull(dataObject);

        if (dataObject is not ITypedDataObject typed)
        {
            throw new ArgumentException($"DataObject should implement {nameof(ITypedDataObject)} interface.", nameof(dataObject));
        }

        return typed.TryGetData(format, out data);
    }

    /// <inheritdoc cref="ITypedDataObject.TryGetData{T}(string, bool, out T)"/>
    /// <exception cref="ArgumentException">if the <paramref name="dataObject"/> does not implement <see cref="ITypedDataObject" />.</exception>
    /// <exception cref="ArgumentNullException">if the <paramref name="dataObject"/> is <see langword="null"/></exception>
    public static bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        this IDataObject dataObject,
        string format,
        bool autoConvert,
        [NotNullWhen(true), MaybeNullWhen(false)] out T data)
    {
        ArgumentNullException.ThrowIfNull(dataObject);

        if (dataObject is not ITypedDataObject typed)
        {
            throw new ArgumentException($"DataObject should implement {nameof(ITypedDataObject)} interface.", nameof(dataObject));
        }

        return typed.TryGetData(format, autoConvert, out data);
    }

    /// <inheritdoc cref="ITypedDataObject.TryGetData{T}(string, Func{Reflection.Metadata.TypeName, Type}, bool, out T)"/>
    /// <exception cref="ArgumentException">if the <paramref name="dataObject"/> does not implement <see cref="ITypedDataObject" />.</exception>
    /// <exception cref="ArgumentNullException">if the <paramref name="dataObject"/> is <see langword="null"/></exception>
    public static bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        this IDataObject dataObject,
        string format,
#pragma warning disable CS3001 // Argument type is not CLS-compliant
        Func<Reflection.Metadata.TypeName, Type> resolver,
#pragma warning restore CS3001
        bool autoConvert,
        [NotNullWhen(true), MaybeNullWhen(false)] out T data)
    {
        ArgumentNullException.ThrowIfNull(dataObject);

        if (dataObject is not ITypedDataObject typed)
        {
            throw new ArgumentException($"DataObject should implement {nameof(ITypedDataObject)} interface.", nameof(dataObject));
        }

        return typed.TryGetData(format, resolver, autoConvert, out data);
    }
}
