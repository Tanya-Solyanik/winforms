// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Specialized;
using System.Drawing;
using System.Formats.Nrbf;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using Windows.Win32.System.Com;
using Com = Windows.Win32.System.Com;

namespace System.Windows.Forms;

/// <summary>
///  Provides methods to place data on and retrieve data from the system clipboard. This class cannot be inherited.
/// </summary>
public static class Clipboard
{
    /// <summary>
    ///  Places non-persistent data on the system <see cref="Clipboard"/>.
    /// </summary>
    public static void SetDataObject(object data) => SetDataObject(data, copy: false);

    /// <summary>
    ///  Overload that uses default values for retryTimes and retryDelay.
    /// </summary>
    public static void SetDataObject(object data, bool copy) =>
        SetDataObject(data, copy, retryTimes: 10, retryDelay: 100);

    /// <summary>
    ///  Places data on the system <see cref="Clipboard"/> and uses copy to specify whether the data
    ///  should remain on the <see cref="Clipboard"/> after the application exits.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   See remarks for <see cref="DataObject(object)"/> for recommendations on how to implement custom <paramref name="data"/>.
    ///  </para>
    /// </remarks>
    public static unsafe void SetDataObject(object data, bool copy, int retryTimes, int retryDelay)
    {
        if (Application.OleRequired() != ApartmentState.STA)
        {
            throw new ThreadStateException(SR.ThreadMustBeSTA);
        }

        ArgumentNullException.ThrowIfNull(data);
        ArgumentOutOfRangeException.ThrowIfNegative(retryTimes);
        ArgumentOutOfRangeException.ThrowIfNegative(retryDelay);

        // Always wrap the data in our DataObject since we know how to retrieve our DataObject from the proxy OleGetClipboard returns.
        DataObject wrappedData = data is DataObject { IsWrappedForClipboard: true } alreadyWrapped
            ? alreadyWrapped
            : new DataObject(data) { IsWrappedForClipboard = true };
        using var dataObject = ComHelpers.GetComScope<Com.IDataObject>(wrappedData);

        HRESULT hr;
        int retry = retryTimes;
        while ((hr = PInvoke.OleSetClipboard(dataObject)).Failed)
        {
            if (--retry < 0)
            {
                throw new ExternalException(SR.ClipboardOperationFailed, (int)hr);
            }

            Thread.Sleep(millisecondsTimeout: retryDelay);
        }

        if (copy)
        {
            retry = retryTimes;
            while ((hr = PInvoke.OleFlushClipboard()).Failed)
            {
                if (--retry < 0)
                {
                    throw new ExternalException(SR.ClipboardOperationFailed, (int)hr);
                }

                Thread.Sleep(millisecondsTimeout: retryDelay);
            }
        }
    }

    /// <summary>
    ///  Retrieves the data that is currently on the system <see cref="Clipboard"/>.
    /// </summary>
    public static unsafe IDataObject? GetDataObject()
    {
        if (Application.OleRequired() != ApartmentState.STA)
        {
            // Only throw if a message loop was started. This makes the case of trying to query the clipboard from the
            // finalizer or non-UI MTA thread silently fail, instead of making the application die.
            return Application.MessageLoop ? throw new ThreadStateException(SR.ThreadMustBeSTA) : null;
        }

        int retryTimes = 10;
        using ComScope<Com.IDataObject> proxyDataObject = new(null);
        HRESULT hr;
        while ((hr = PInvoke.OleGetClipboard(proxyDataObject)).Failed)
        {
            if (--retryTimes < 0)
            {
                throw new ExternalException(SR.ClipboardOperationFailed, (int)hr);
            }

            Thread.Sleep(millisecondsTimeout: 100);
        }

        // OleGetClipboard always returns a proxy. The proxy forwards all IDataObject method calls to the real data object,
        // without giving out the real data object. If the data placed on the clipboard is not one of our CCWs or the clipboard
        // has been flushed, marshal will create a wrapper around the proxy for us to use. However, if the data placed on
        // the clipboard is one of our own and the clipboard has not been flushed, we need to retrieve the real data object
        // pointer in order to retrieve the original managed object via ComWrappers. To do this, we must query for an
        // interface that is not known to the proxy e.g. IComCallableWrapper. If we are able to query for IComCallableWrapper
        // it means that the real data object is one of our CCWs and we've retrieved it successfully,
        // otherwise it is not ours and we will use the wrapped proxy.
        IUnknown* target = default;
        var realDataObject = proxyDataObject.TryQuery<IComCallableWrapper>(out hr);
        if (hr.Succeeded)
        {
            target = realDataObject.AsUnknown;
        }
        else
        {
            target = proxyDataObject.AsUnknown;
        }

        if (!ComHelpers.TryGetObjectForIUnknown(target, out object? managedDataObject))
        {
            target->Release();
            return null;
        }

        if (managedDataObject is not Com.IDataObject.Interface dataObject)
        {
            // We always wrap data set on the Clipboard in a DataObject, so if we do not have
            // a IDataObject.Interface this means built-in com support is turned off and
            // we have a proxy where there is no way to retrieve the original data object
            // pointer from it likely because either the clipboard was flushed or the data on the
            // clipboard is from another process. We need to mimic built-in com behavior and wrap the proxy ourselves.
            // DataObject will ref count proxyDataObject properly to take ownership.
            return new DataObject(proxyDataObject.Value);
        }

        if (dataObject is DataObject { IsWrappedForClipboard: true } wrappedData)
        {
            // There is a DataObject on the clipboard that we placed there. If the real data object
            // implements IDataObject, we want to unwrap it and return it. Otherwise return
            // the DataObject as is.
            return wrappedData.TryUnwrapInnerIDataObject();
        }

        // We did not place the data on the clipboard. Fall back to old behavior.
        return dataObject is IDataObject ido && !Marshal.IsComObject(dataObject)
            ? ido
            : new DataObject(dataObject);
    }

    /// <summary>
    ///  Removes all data from the Clipboard.
    /// </summary>
    public static unsafe void Clear()
    {
        if (Application.OleRequired() != ApartmentState.STA)
        {
            throw new ThreadStateException(SR.ThreadMustBeSTA);
        }

        HRESULT hr;
        int retry = 10;
        while ((hr = PInvoke.OleSetClipboard(null)).Failed)
        {
            if (--retry < 0)
            {
#pragma warning disable CA2201 // Do not raise reserved exception types
                throw new ExternalException(SR.ClipboardOperationFailed, (int)hr);
#pragma warning restore CA2201
            }

            Thread.Sleep(millisecondsTimeout: 100);
        }
    }

    /// <summary>
    ///  Indicates whether there is data on the Clipboard in the <see cref="DataFormats.WaveAudio"/> format.
    /// </summary>
    public static bool ContainsAudio() => ContainsData(DataFormats.WaveAudioConstant);

    /// <summary>
    ///  Indicates whether there is data on the Clipboard that is in the specified format
    ///  or can be converted to that format.
    /// </summary>
    public static bool ContainsData(string? format) =>
        !string.IsNullOrWhiteSpace(format) && ContainsData(format, autoConvert: false);

    private static bool ContainsData(string format, bool autoConvert) =>
        GetDataObject() is IDataObject dataObject && dataObject.GetDataPresent(format, autoConvert: autoConvert);

    /// <summary>
    ///  Indicates whether there is data on the Clipboard that is in the <see cref="DataFormats.FileDrop"/> format
    ///  or can be converted to that format.
    /// </summary>
    public static bool ContainsFileDropList() => ContainsData(DataFormats.FileDrop, autoConvert: true);

    /// <summary>
    ///  Indicates whether there is data on the Clipboard that is in the <see cref="DataFormats.Bitmap"/> format
    ///  or can be converted to that format.
    /// </summary>
    public static bool ContainsImage() => ContainsData(DataFormats.Bitmap, autoConvert: true);

    /// <summary>
    ///  Indicates whether there is text data on the Clipboard in <see cref="TextDataFormat.UnicodeText"/> format.
    /// </summary>
    public static bool ContainsText() => ContainsText(TextDataFormat.UnicodeText);

    /// <summary>
    ///  Indicates whether there is text data on the Clipboard in the format indicated by the specified
    ///  <see cref="TextDataFormat"/> value.
    /// </summary>
    public static bool ContainsText(TextDataFormat format)
    {
        SourceGenerated.EnumValidator.Validate(format, nameof(format));
        return ContainsData(ConvertToDataFormats(format));
    }

    /// <summary>
    ///  Retrieves an audio stream from the <see cref="Clipboard"/>.
    /// </summary>
    public static Stream? GetAudioStream() =>
        GetLegacyData<Stream>(DataFormats.WaveAudioConstant);

    /// <summary>
    ///  Retrieves data from the <see cref="Clipboard"/> in the specified format.
    /// </summary>
    /// <exception cref="ThreadStateException">
    ///  The current thread is not in single-threaded apartment (STA) mode.
    /// </exception>
    [Obsolete(
        Obsoletions.ClipboardGetDataMessage,
        error: false,
        DiagnosticId = Obsoletions.ClipboardGetDataDiagnosticId,
        UrlFormat = Obsoletions.SharedUrlFormat)]
    public static object? GetData(string format) =>
        string.IsNullOrWhiteSpace(format) ? null : GetData(format, autoConvert: false);

    private static object? GetData(string format, bool autoConvert) =>
        GetDataObject() is IDataObject dataObject ? dataObject.GetData(format, autoConvert) : null;

    /// <inheritdoc cref="TryGetData{T}(string, Func{TypeName, Type}, out T)"/>
    public static bool TryGetData<T>(
        string format,
        [NotNullWhen(true), MaybeNullWhen(false)] out T data)
    {
        data = default;
        var dataObject = GetDataObject();
        if (dataObject is null)
        {
            return false;
        }

        if (dataObject is ITypedDataObject typed)
        {
            // Custom IDataObjects should handle their own validation.
            if (typed is DataObject && !DataObject.ValidateTryGetDataArguments<T>(format))
            {
                return false;
            }

            return typed.TryGetData(format, DataObject.NotSupportedResolver, autoConvert: false, out data);
        }

        throw new NotSupportedException($"{nameof(IDataObject)} on the {nameof(Clipboard)} does not" +
            $" support {nameof(ITypedDataObject)} and can't be read using `TryGetData<T>(string, out T)` method.");
    }

    /// <summary>
    ///  Retrieves data from the <see cref="Clipboard"/> in the specified format if that data is of type <typeparamref name="T"/>.
    ///  This is an alternative to <see cref="GetData(string)"/> that uses <see cref="BinaryFormatter"/> only when application
    ///  opts-in into <see cref="AppContext"/> switch named "Windows.ClipboardDragDrop.EnableUnsafeBinaryFormatterSerialization".
    ///  By default the NRBF deserializer attempts to deserialize the stream.
    /// </summary>
    /// <param name="format">
    ///  A string that specifies what format to retrieve the data as.
    ///  See the <see cref="DataFormats"/> class for a set of predefined data formats.
    /// </param>
    /// <param name="resolver">
    ///  Resolver is used only when deserializing non-OLE formats. It returns the type if <see cref="TypeName"/> is allowed or
    ///  throws a <see cref="NotSupportedException"/> if type is not expected. It should not return a <see langword="null"/>.
    ///  Type requested by the user is resolved automatically, it does not have to be resolved by this function.
    ///  The following types are resolved automatically:
    ///  1.NRBF primitive types <see href="https://learn.microsoft.com/openspecs/windows_protocols/ms-nrbf/4e77849f-89e3-49db-8fb9-e77ee4bc7214"/>
    ///  (bool, byte, char, decimal, double, short, int, long, sbyte, ushort, uint, ulong, float, string, TimeSpan, DateTime).
    ///  2.System.Drawing.Primitive.dll exchange types (PointF, RectangleF, Point, Rectangle, SizeF, Size, Color).
    ///  3.Types commonly used in WinForms applications (System.Drawing.Bitmap, System.Windows.Forms.ImageListStreamer,
    ///    System.NotSupportedException(only the message is re-hydrated), List{T} where T is an NRBF primitive type, arrays of NRBF primitive types).
    /// </param>
    /// <param name="data">
    ///  An object that contains the data in the specified format, or <see lanfword="null"/> if the data is unavailable in the specified format,
    ///  or is of a wrong <see cref="Type"/>.
    /// </param>
    /// <returns>
    ///  <see langword="true"/> if the data of this format is present and the value is
    ///  of a matching type and that value can be successfully retrieved, or <see langword="false"/>
    ///  if the format is not present or the value is of a wrong type.
    /// </returns>
    /// <exception cref="NotSupportedException">
    ///  If application does not support <see cref="BinaryFormatter"/> and the object can't be deserialized otherwise, or
    ///  application supports <see cref="BinaryFormatter"/> but <typeparamref name="T"/> is an <see cref="object"/>, or not a concrete type,
    ///  or if <paramref name="resolver"/> does not resolve the actual payload type.  Or the <see cref="IDataObject"/> on
    ///  the <see cref="Clipboard"/> does not implement <see cref="ITypedDataObject"/> interface.
    ///  </exception>
    /// <example>
    /// <![CDATA[
    ///  using System.Reflection.Metadata;
    ///
    ///  internal static Type MyResolver(TypeName typeName)
    ///  {
    ///      Type[] allowedTypes =
    ///      [
    ///          typeof(MyClass1),
    ///          typeof(MyClass2),
    ///      ];
    ///
    ///      foreach (Type type in allowedTypes)
    ///      {
    ///          if (type.FullName == typeName.FullName)
    ///          {
    ///              return type;
    ///          }
    ///      }
    ///
    ///      throw new NotSupportedException($"Unexpected type in the payload {typeName.FullName}");
    ///  }
    ///  ]]>
    /// </example>
    [CLSCompliant(false)]
    public static bool TryGetData<T>(
        string format,
        Func<TypeName, Type> resolver,
        [NotNullWhen(true), MaybeNullWhen(false)] out T data)
    {
        data = default;
        var dataObject = GetDataObject();
        if (dataObject is ITypedDataObject typed)
        {
            return typed.TryGetData(format, resolver, autoConvert: false, out data);
        }

        throw new NotSupportedException($"{nameof(IDataObject)} on the {nameof(Clipboard)} does not" +
            $" support {nameof(ITypedDataObject)} and can't be read using" +
            $" `TryGetData<T>(string, Func<TypeName, Type>, out T)` method.");
    }

    /// <summary>
    ///  Retrieves a collection of file names from the <see cref="Clipboard"/>.
    /// </summary>
    public static StringCollection GetFileDropList()
    {
        StringCollection result = [];

        if (GetLegacyData<string[]?>(DataFormats.FileDropConstant) is string[] strings)
        {
            result.AddRange(strings);
        }

        return result;
    }

    /// <summary>
    ///  Retrieves a <see cref="Bitmap"/> from the <see cref="Clipboard"/>.
    /// </summary>
    /// <devdoc>
    ///  <see cref="Bitmap"/>s are re-hydrated from a <see cref="SerializationRecord"/> by reading a byte array
    ///  but if that fails, <see cref="BinaryFormatter"/> is restricted by the <see cref="DataObject.BitmapBinder"/>.
    /// </devdoc>
    public static Image? GetImage() => GetLegacyData<Image>(DataFormats.Bitmap);

    /// <summary>
    ///  Retrieves text data from the <see cref="Clipboard"/> in the <see cref="TextDataFormat.UnicodeText"/> format.
    /// </summary>
    public static string GetText() => GetText(TextDataFormat.UnicodeText);

    /// <summary>
    ///  Retrieves text data from the <see cref="Clipboard"/> in the format indicated by the specified
    ///  <see cref="TextDataFormat"/> value.
    /// </summary>
    public static string GetText(TextDataFormat format)
    {
        SourceGenerated.EnumValidator.Validate(format, nameof(format));

        return GetLegacyData<string>(ConvertToDataFormats(format)) is string text ? text : string.Empty;
    }

    private static T? GetLegacyData<T>(string format)
    {
        IDataObject? data = GetDataObject();
        if (data is ITypedDataObject typed)
        {
            return typed.TryGetData(format, autoConvert: true, out T? value) ? value : default;
        }

        if (data is IDataObject dataObject)
        {
            return dataObject.GetData(format, autoConvert: true) is T value ? value : default;
        }

        return default;
    }

    /// <summary>
    ///  Clears the <see cref="Clipboard"/> and then adds data in the <see cref="DataFormats.WaveAudio"/> format.
    /// </summary>
    public static void SetAudio(byte[] audioBytes) => SetAudio(new MemoryStream(audioBytes.OrThrowIfNull()));

    /// <summary>
    ///  Clears the <see cref="Clipboard"/> and then adds data in the <see cref="DataFormats.WaveAudio"/> format.
    /// </summary>
    public static void SetAudio(Stream audioStream) =>
        SetDataObject(new DataObject(DataFormats.WaveAudioConstant, audioStream.OrThrowIfNull()), copy: true);

    /// <summary>
    ///  Clears the Clipboard and then adds data in the specified format.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   See remarks for <see cref="DataObject(object)"/> for recommendations on how to implement custom <paramref name="data"/>.
    ///  </para>
    /// </remarks>
    public static void SetData(string format, object data)
    {
        if (string.IsNullOrWhiteSpace(format.OrThrowIfNull()))
        {
            throw new ArgumentException(SR.DataObjectWhitespaceEmptyFormatNotAllowed, nameof(format));
        }

        // Note: We delegate argument checking to IDataObject.SetData, if it wants to do so.
        SetDataObject(new DataObject(format, data), copy: true);
    }

    /// <summary>
    ///  Clears the Clipboard and then adds a collection of file names in the <see cref="DataFormats.FileDrop"/> format.
    /// </summary>
    public static void SetFileDropList(StringCollection filePaths)
    {
        if (filePaths.OrThrowIfNull().Count == 0)
        {
            throw new ArgumentException(SR.CollectionEmptyException);
        }

        // Validate the paths to make sure they don't contain invalid characters
        string[] filePathsArray = new string[filePaths.Count];
        filePaths.CopyTo(filePathsArray, 0);

        foreach (string path in filePathsArray)
        {
            // These are the only error states for Path.GetFullPath
            if (string.IsNullOrEmpty(path) || path.Contains('\0'))
            {
                throw new ArgumentException(string.Format(SR.Clipboard_InvalidPath, path, nameof(filePaths)));
            }
        }

        SetDataObject(new DataObject(DataFormats.FileDropConstant, autoConvert: true, filePathsArray), copy: true);
    }

    /// <summary>
    ///  Clears the Clipboard and then adds an <see cref="Image"/> in the <see cref="DataFormats.Bitmap"/> format.
    /// </summary>
    public static void SetImage(Image image) =>
        SetDataObject(new DataObject(DataFormats.BitmapConstant, autoConvert: true, image.OrThrowIfNull()), copy: true);

    /// <summary>
    ///  Clears the Clipboard and then adds text data in the <see cref="TextDataFormat.UnicodeText"/> format.
    /// </summary>
    public static void SetText(string text) => SetText(text, TextDataFormat.UnicodeText);

    /// <summary>
    ///  Clears the Clipboard and then adds text data in the format indicated by the specified
    ///  <see cref="TextDataFormat"/> value.
    /// </summary>
    public static void SetText(string text, TextDataFormat format)
    {
        text.ThrowIfNullOrEmpty();
        SourceGenerated.EnumValidator.Validate(format, nameof(format));
        SetDataObject(new DataObject(ConvertToDataFormats(format), text), copy: true);
    }

    private static string ConvertToDataFormats(TextDataFormat format) => format switch
    {
        TextDataFormat.Text => DataFormats.Text,
        TextDataFormat.UnicodeText => DataFormats.UnicodeText,
        TextDataFormat.Rtf => DataFormats.Rtf,
        TextDataFormat.Html => DataFormats.Html,
        TextDataFormat.CommaSeparatedValue => DataFormats.CommaSeparatedValue,
        _ => DataFormats.UnicodeText,
    };
}
