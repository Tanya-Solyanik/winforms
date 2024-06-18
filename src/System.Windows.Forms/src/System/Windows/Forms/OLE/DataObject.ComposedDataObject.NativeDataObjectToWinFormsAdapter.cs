// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms.BinaryFormat;
using Com = Windows.Win32.System.Com;

namespace System.Windows.Forms;

public unsafe partial class DataObject
{
    internal unsafe partial class ComposedDataObject
    {
        // Feature switch, when set to false, BinaryFormatter is not supported in trimmed applications.
        // This field, using the default BinaryFormatter switch, is used to control trim warnings related to using BinaryFormatter in WinForms trimming.
        // The trimmer will generate a warning when set to true and will not generate a warning when set to false.
        [FeatureSwitchDefinition("System.Runtime.Serialization.EnableUnsafeBinaryFormatterSerialization")]
#pragma warning disable IDE0075 // Simplify conditional expression - the simpler expression is hard to read
        internal static bool EnableUnsafeBinaryFormatterInNativeObjectSerialization { get; } =
            AppContext.TryGetSwitch("System.Runtime.Serialization.EnableUnsafeBinaryFormatterSerialization", out bool isEnabled)
                ? isEnabled
                : true;
#pragma warning restore IDE0075

        /// <summary>
        ///  Maps native pointer <see cref="Com.IDataObject"/> to <see cref="IDataObject"/>.
        /// </summary>
        private unsafe class NativeDataObjectToWinFormsAdapter : IDataObject, Com.IDataObject.Interface
        {
            private readonly AgileComPointer<Com.IDataObject> _nativeDataObject;

            public NativeDataObjectToWinFormsAdapter(Com.IDataObject* dataObject)
            {
#if DEBUG
                _nativeDataObject = new(dataObject, takeOwnership: true, trackDisposal: false);
#else
                _nativeDataObject = new(dataObject, takeOwnership: true);
#endif
            }

            #region Com.IDataObject.Interface

            HRESULT Com.IDataObject.Interface.DAdvise(Com.FORMATETC* pformatetc, uint advf, Com.IAdviseSink* pAdvSink, uint* pdwConnection)
            {
                using var nativeDataObject = _nativeDataObject.GetInterface();
                return nativeDataObject.Value->DAdvise(pformatetc, advf, pAdvSink, pdwConnection);
            }

            HRESULT Com.IDataObject.Interface.DUnadvise(uint dwConnection)
            {
                using var nativeDataObject = _nativeDataObject.GetInterface();
                return nativeDataObject.Value->DUnadvise(dwConnection);
            }

            HRESULT Com.IDataObject.Interface.EnumDAdvise(Com.IEnumSTATDATA** ppenumAdvise)
            {
                using var nativeDataObject = _nativeDataObject.GetInterface();
                return nativeDataObject.Value->EnumDAdvise(ppenumAdvise);
            }

            HRESULT Com.IDataObject.Interface.EnumFormatEtc(uint dwDirection, Com.IEnumFORMATETC** ppenumFormatEtc)
            {
                using var nativeDataObject = _nativeDataObject.GetInterface();
                return nativeDataObject.Value->EnumFormatEtc(dwDirection, ppenumFormatEtc);
            }

            HRESULT Com.IDataObject.Interface.GetData(Com.FORMATETC* pformatetcIn, Com.STGMEDIUM* pmedium)
            {
                using var nativeDataObject = _nativeDataObject.GetInterface();
                return nativeDataObject.Value->GetData(pformatetcIn, pmedium);
            }

            HRESULT Com.IDataObject.Interface.GetDataHere(Com.FORMATETC* pformatetc, Com.STGMEDIUM* pmedium)
            {
                using var nativeDataObject = _nativeDataObject.GetInterface();
                return nativeDataObject.Value->GetDataHere(pformatetc, pmedium);
            }

            HRESULT Com.IDataObject.Interface.QueryGetData(Com.FORMATETC* pformatetc)
            {
                using var nativeDataObject = _nativeDataObject.GetInterface();
                return nativeDataObject.Value->QueryGetData(pformatetc);
            }

            HRESULT Com.IDataObject.Interface.GetCanonicalFormatEtc(Com.FORMATETC* pformatectIn, Com.FORMATETC* pformatetcOut)
            {
                using var nativeDataObject = _nativeDataObject.GetInterface();
                return nativeDataObject.Value->GetCanonicalFormatEtc(pformatectIn, pformatetcOut);
            }

            HRESULT Com.IDataObject.Interface.SetData(Com.FORMATETC* pformatetc, Com.STGMEDIUM* pmedium, BOOL fRelease)
            {
                using var nativeDataObject = _nativeDataObject.GetInterface();
                return nativeDataObject.Value->SetData(pformatetc, pmedium, fRelease);
            }

            #endregion

            /// <summary>
            ///  Retrieves the specified format from the specified <paramref name="hglobal"/>.
            /// </summary>
            private static T? GetDataFromHGLOBAL<T>(HGLOBAL hglobal, string format, Func<TypeName, Type> resolver, out MemoryStream? stream)
            {
                stream = null;

                if (hglobal == 0)
                {
                    return default;
                }

                return format switch
                {
                    DataFormats.TextConstant or DataFormats.RtfConstant or DataFormats.OemTextConstant => IsString()
                        ? CastOrDefault(ReadStringFromHGLOBAL(hglobal, unicode: false))
                        : default,
                    DataFormats.HtmlConstant => IsString()
                        ? CastOrDefault(ReadUtf8StringFromHGLOBAL(hglobal))
                        : default,
                    DataFormats.UnicodeTextConstant => IsString()
                        ? CastOrDefault(ReadStringFromHGLOBAL(hglobal, unicode: true))
                        : default,
                    DataFormats.FileDropConstant => IsStringArray()
                        ? CastOrDefault(ReadFileListFromHDROP((HDROP)(nint)hglobal))
                        : default,
                    CF_DEPRECATED_FILENAME => IsStringArray()
                        ? CastOrDefault(new string[] { ReadStringFromHGLOBAL(hglobal, unicode: false) })
                        : default,
                    CF_DEPRECATED_FILENAMEW => IsStringArray()
                        ? CastOrDefault(new string[] { ReadStringFromHGLOBAL(hglobal, unicode: true) })
                        : default,
                    _ => ReadObjectFromHGLOBAL(hglobal, RestrictDeserializationToSafeTypes(format), out stream)
                };

                static T? CastOrDefault(object? value) => value is T t ? t : default;

                static bool IsString() => typeof(T).IsAssignableTo(typeof(string));

                static bool IsStringArray() => typeof(T).IsAssignableTo(typeof(string[]));

                T? ReadObjectFromHGLOBAL(HGLOBAL hglobal, bool restrictDeserialization, out MemoryStream? stream)
                {
                    stream = ReadByteStreamFromHGLOBAL(hglobal, out bool isSerializedObject);
                    if (isSerializedObject)
                    {
                        T? value = ReadObjectFromHandleDeserializer(stream, restrictDeserialization);
                        stream = null;
                        return value;
                    }

                    return default;

                    T? ReadObjectFromHandleDeserializer(MemoryStream stream, bool restrictDeserialization)
                    {
                        long startPosition = stream.Position;
                        try
                        {
                            BinaryFormattedObject binaryFormattedObject = new(stream);
                            if (binaryFormattedObject.TryGetObject(out object? value))
                            {
                                // I'm ignoring the "trimming not supported" message here???????
                                return CastOrDefault(value);
                            }

                            if (!binaryFormattedObject.Contains<T>())
                            {
                                if (resolver is null)
                                {
                                    return default;
                                }

                                if (resolver != Clipboard.CompatibleResolver && binaryFormattedObject.TryGetRootTypeName(out TypeName? name))
                                {
                                    Type? type;
                                    try
                                    {
                                        type = resolver(name);
                                    }
                                    catch (NotSupportedException)
                                    {
                                        type = null;
                                    }

                                    if (type is null || !type.IsAssignableTo(typeof(T)))
                                    {
                                        return default;
                                    }
                                }
                            }

                            // TanyaSo: when do I get an exception and when do I get a null?
                            // I'm assuming that if contains succeeded and Deserialize succeeded, then BF will not do any better.
                            value = binaryFormattedObject.Deserialize();
                            return CastOrDefault(value);
                        }
                        catch (NotSupportedException)
                        {
                            // try BinaryFormatter
                        }
                        catch (SerializationException)
                        {
                        }
                        catch (Exception ex) when (!ex.IsCriticalException())
                        {
                            // Couldn't parse for some reason, let the BinaryFormatter try to handle it,
                            // assuming developer had opted in into BinaryFormatter and provided a resolver.
                        }

                        if (resolver is null)
                        {
                            return default;
                        }

                        // This check is to help in trimming scenarios with a trim warning on a call to BinaryFormatter.Deserialize(), which has a RequiresUnreferencedCode annotation.
                        // If the flag is false, the trimmer will not generate a warning, since BinaryFormatter.Deserialize() will not be called,
                        // If the flag is true, the trimmer will generate a warning for calling a method that has a RequiresUnreferencedCode annotation.
                        if (!EnableUnsafeBinaryFormatterInNativeObjectSerialization)
                        {
                            throw new NotSupportedException(SR.BinaryFormatterNotSupported);
                        }

                        stream.Position = startPosition;

#pragma warning disable SYSLIB0011 // Type or member is obsolete
#pragma warning disable SYSLIB0050 // Type or member is obsolete
                        object result = new BinaryFormatter()
                        {
                            Binder = restrictDeserialization ? new BitmapBinder() : new ComposedBinder(typeof(T), resolver),
                            AssemblyFormat = FormatterAssemblyStyle.Simple
                        }.Deserialize(stream);
#pragma warning restore SYSLIB0050
#pragma warning restore SYSLIB0011

                        return CastOrDefault(result);
                    }
                }
            }

            private static unsafe MemoryStream ReadByteStreamFromHGLOBAL(HGLOBAL hglobal, out bool isSerializedObject)
            {
                void* buffer = PInvokeCore.GlobalLock(hglobal);
                if (buffer is null)
                {
                    throw new ExternalException(SR.ExternalException, (int)HRESULT.E_OUTOFMEMORY);
                }

                try
                {
                    int size = (int)PInvokeCore.GlobalSize(hglobal);
                    byte[] bytes = new byte[size];
                    Marshal.Copy((nint)buffer, bytes, 0, size);
                    int index = 0;

                    // The object here can either be a stream or a serialized object. We identify a serialized object
                    // by writing the bytes for the guid serializedObjectID at the front of the stream.

                    if (isSerializedObject = bytes.AsSpan().StartsWith(s_serializedObjectID))
                    {
                        index = s_serializedObjectID.Length;
                    }

                    return new MemoryStream(bytes, index, bytes.Length - index);
                }
                finally
                {
                    PInvokeCore.GlobalUnlock(hglobal);
                }
            }

            private static unsafe string ReadStringFromHGLOBAL(HGLOBAL hglobal, bool unicode)
            {
                string? stringData = null;

                void* buffer = PInvokeCore.GlobalLock(hglobal);
                try
                {
                    stringData = unicode ? new string((char*)buffer) : new string((sbyte*)buffer);
                }
                finally
                {
                    PInvokeCore.GlobalUnlock(hglobal);
                }

                return stringData;
            }

            private static unsafe string ReadUtf8StringFromHGLOBAL(HGLOBAL hglobal)
            {
                void* buffer = PInvokeCore.GlobalLock(hglobal);
                try
                {
                    int size = (int)PInvokeCore.GlobalSize(hglobal);
                    return Encoding.UTF8.GetString((byte*)buffer, size - 1);
                }
                finally
                {
                    PInvokeCore.GlobalUnlock(hglobal);
                }
            }

            private static unsafe string[]? ReadFileListFromHDROP(HDROP hdrop)
            {
                uint count = PInvoke.DragQueryFile(hdrop, iFile: 0xFFFFFFFF, lpszFile: null, cch: 0);
                if (count == 0)
                {
                    return null;
                }

                Span<char> fileName = stackalloc char[PInvoke.MAX_PATH + 1];
                string[] files = new string[count];

                fixed (char* buffer = fileName)
                {
                    for (uint i = 0; i < count; i++)
                    {
                        uint charactersCopied = PInvoke.DragQueryFile(hdrop, i, buffer, (uint)fileName.Length);
                        if (charactersCopied == 0)
                        {
                            continue;
                        }

                        string s = fileName[..(int)charactersCopied].ToString();
                        files[i] = s;
                    }
                }

                return files;
            }

            /// <summary>
            ///  Extracts a managed object from <see cref="Com.IDataObject"/> of the specified format.
            /// </summary>
            /// <param name="doNotContinue">
            ///  A restricted type was encountered, do not continue trying to deserialize.
            /// </param>
            private static T? GetObjectFromDataObject<T>(Com.IDataObject* dataObject, string format, Func<TypeName, Type> resolver, out bool doNotContinue, out MemoryStream? stream)
            {
                T? data = default;
                stream = null;
                doNotContinue = false;
                try
                {
                    // Try to get the data as a bitmap first.
                    if (typeof(Bitmap).IsAssignableTo(typeof(T)))
                    {
                        Bitmap? bitmap = TryGetBitmapData(dataObject, format);
                        if (bitmap is object)
                        {
                            return bitmap is T t ? t : default;
                        }
                    }

                    // Check for one of our standard data types.
                    data = TryGetHGLOBALData(dataObject, format, resolver, out doNotContinue, out stream);

                    if (data is null && stream is null && !doNotContinue)
                    {
                        // Lastly check to see if the data is an IStream.
                        data = TryGetIStreamData(dataObject, format, resolver, out stream);
                    }
                }
                catch (Exception e)
                {
                    Debug.Fail(e.ToString());
                }

                return data;

                static T? TryGetHGLOBALData(Com.IDataObject* dataObject, string format, Func<TypeName, Type> resolver, out bool doNotContinue, out MemoryStream? stream)
                {
                    doNotContinue = false;
                    stream = null;

                    Com.FORMATETC formatetc = new()
                    {
                        cfFormat = (ushort)DataFormats.GetFormat(format).Id,
                        dwAspect = (uint)Com.DVASPECT.DVASPECT_CONTENT,
                        lindex = -1,
                        tymed = (uint)Com.TYMED.TYMED_HGLOBAL
                    };

                    if (dataObject->QueryGetData(formatetc).Failed)
                    {
                        return default;
                    }

                    T? data = default;
                    HRESULT result = dataObject->GetData(formatetc, out Com.STGMEDIUM medium);

                    // One of the ways this can happen is when we attempt to put binary formatted data onto the
                    // clipboard, which will succeed as Windows ignores all errors when putting data on the clipboard.
                    // The data state, however, is not good, and this error will be returned by Windows when asking to
                    // get the data out.
                    Debug.WriteLineIf(result == HRESULT.CLIPBRD_E_BAD_DATA, "CLIPBRD_E_BAD_DATA returned when trying to get clipboard data.");

                    try
                    {
                        if (medium.tymed == Com.TYMED.TYMED_HGLOBAL && !medium.hGlobal.IsNull)
                        {
                            data = GetDataFromHGLOBAL<T>(medium.hGlobal, format, resolver, out stream);
                        }
                    }
                    catch (RestrictedTypeDeserializationException)
                    {
                        doNotContinue = true;
                    }
                    catch
                    {
                    }
                    finally
                    {
                        PInvoke.ReleaseStgMedium(ref medium);
                    }

                    return data;
                }

                static unsafe T? TryGetIStreamData(Com.IDataObject* dataObject, string format, Func<TypeName, Type> resolver, out MemoryStream? stream)
                {
                    stream = null;
                    Com.FORMATETC formatEtc = new()
                    {
                        cfFormat = (ushort)DataFormats.GetFormat(format).Id,
                        dwAspect = (uint)Com.DVASPECT.DVASPECT_CONTENT,
                        lindex = -1,
                        tymed = (uint)Com.TYMED.TYMED_ISTREAM
                    };

                    // Limit the # of exceptions we may throw below.
                    if (dataObject->QueryGetData(formatEtc).Failed
                        || dataObject->GetData(formatEtc, out Com.STGMEDIUM medium).Failed)
                    {
                        return default;
                    }

                    HGLOBAL hglobal = default;
                    try
                    {
                        if (medium.tymed != Com.TYMED.TYMED_ISTREAM || medium.hGlobal.IsNull)
                        {
                            return default;
                        }

                        using ComScope<Com.IStream> pStream = new((Com.IStream*)medium.hGlobal);
                        pStream.Value->Stat(out Com.STATSTG sstg, (uint)Com.STATFLAG.STATFLAG_DEFAULT);

                        hglobal = PInvokeCore.GlobalAlloc(GLOBAL_ALLOC_FLAGS.GMEM_MOVEABLE | GLOBAL_ALLOC_FLAGS.GMEM_ZEROINIT, (uint)sstg.cbSize);

                        // Not throwing here because the other out of memory condition on GlobalAlloc
                        // happens inside innerData.GetData and gets turned into a null return value.
                        if (hglobal.IsNull)
                        {
                            return default;
                        }

                        void* ptr = PInvokeCore.GlobalLock(hglobal);
                        pStream.Value->Read((byte*)ptr, (uint)sstg.cbSize, null);
                        PInvokeCore.GlobalUnlock(hglobal);

                        return GetDataFromHGLOBAL<T>(hglobal, format, resolver, out stream);
                    }
                    finally
                    {
                        if (!hglobal.IsNull)
                        {
                            PInvokeCore.GlobalFree(hglobal);
                        }

                        PInvoke.ReleaseStgMedium(ref medium);
                    }
                }
            }

            private static Bitmap? TryGetBitmapData(Com.IDataObject* dataObject, string format)
            {
                if (format != DataFormats.BitmapConstant)
                {
                    return null;
                }

                Com.FORMATETC formatEtc = new()
                {
                    cfFormat = (ushort)DataFormats.GetFormat(format).Id,
                    dwAspect = (uint)Com.DVASPECT.DVASPECT_CONTENT,
                    lindex = -1,
                    tymed = (uint)Com.TYMED.TYMED_GDI
                };

                Com.STGMEDIUM medium = default;

                if (dataObject->QueryGetData(formatEtc).Succeeded)
                {
                    HRESULT hr = dataObject->GetData(formatEtc, out medium);
                    // One of the ways this can happen is when we attempt to put binary formatted data onto the
                    // clipboard, which will succeed as Windows ignores all errors when putting data on the clipboard.
                    // The data state, however, is not good, and this error will be returned by Windows when asking to
                    // get the data out.
                    Debug.WriteLineIf(hr == HRESULT.CLIPBRD_E_BAD_DATA, "CLIPBRD_E_BAD_DATA returned when trying to get clipboard data.");
                }

                Bitmap? data = null;

                try
                {
                    // GDI+ doesn't own this HBITMAP, but we can't delete it while the object is still around. So we
                    // have to do the really expensive thing of cloning the image so we can release the HBITMAP.
                    if ((uint)medium.tymed == (uint)TYMED.TYMED_GDI
                        && !medium.hGlobal.IsNull
                        && Image.FromHbitmap(medium.hGlobal) is Bitmap clipboardBitmap)
                    {
                        data = (Bitmap)clipboardBitmap.Clone();
                        clipboardBitmap.Dispose();
                    }
                }
                finally
                {
                    PInvoke.ReleaseStgMedium(ref medium);
                }

                return data;
            }

            private bool TryGetDataInternal<T>(string format, Func<TypeName, Type> resolver, bool autoConvert, out T? data)
            {
                using var nativeDataObject = _nativeDataObject.GetInterface();
                data = GetObjectFromDataObject<T>(nativeDataObject, format, resolver, out bool doNotContinue, out MemoryStream? stream);

                if (doNotContinue
                    || !autoConvert
                    || stream is null
                    || GetMappedFormats(format) is not { } mappedFormats)
                {
                    if (data is null && stream is T t)
                    {
                        data = t;
                    }

                    return data is object;
                }

                T? originalData = data;
                // Try to find a mapped format that works better.
                foreach (string mappedFormat in mappedFormats)
                {
                    if (!format.Equals(mappedFormat))
                    {
                        data = GetObjectFromDataObject<T>(nativeDataObject, mappedFormat, resolver, out doNotContinue, out stream);
                        if (doNotContinue)
                        {
                            break;
                        }

                        if (data is not null and not MemoryStream)
                        {
                            return true;
                        }
                    }
                }

                data = originalData ?? data;
                return data is object;
            }

            private static bool IsUnbounded(Type type) => type.IsInterface || type.IsAbstract || type == typeof(object);

            #region IDataObject

            bool IDataObject.TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
                string format,
                Func<TypeName, Type> resolver,
                bool autoConvert,
                [NotNullWhen(true), MaybeNullWhen(false)] out T data)
            {
                data = default;
                if (Clipboard.CompatibleResolver != resolver && IsUnbounded(typeof(T)))
                {
                    // TanyaSo - should this be a not supported exception?
                    return false;
                }

                return TryGetDataInternal(format, resolver, autoConvert, out data);
            }

            object? IDataObject.GetData(string format, bool autoConvert)
            {
                TryGetDataInternal(format, Clipboard.Resolver, autoConvert, out object? data);
                return data;
            }

            object? IDataObject.GetData(string format) => ((IDataObject)this).GetData(format, autoConvert: true);

            object? IDataObject.GetData(Type format) => ((IDataObject)this).GetData(format.FullName!);

            bool IDataObject.GetDataPresent(Type format) => GetDataPresent(format.FullName!);

            public bool GetDataPresent(string format, bool autoConvert)
            {
                bool dataPresent = GetDataPresentInner(format);

                if (dataPresent || !autoConvert || GetMappedFormats(format) is not { } mappedFormats)
                {
                    return dataPresent;
                }

                foreach (string mappedFormat in mappedFormats)
                {
                    if (!format.Equals(mappedFormat) && (dataPresent = GetDataPresentInner(mappedFormat)))
                    {
                        break;
                    }
                }

                return dataPresent;
            }

            public bool GetDataPresent(string format) => GetDataPresent(format, autoConvert: true);

            public string[] GetFormats(bool autoConvert)
            {
                using var nativeDataObject = _nativeDataObject.GetInterface();
                Debug.Assert(!nativeDataObject.IsNull, "You must have an innerData on all DataObjects");

                using ComScope<Com.IEnumFORMATETC> enumFORMATETC = new(null);
                nativeDataObject.Value->EnumFormatEtc((uint)DATADIR.DATADIR_GET, enumFORMATETC).AssertSuccess();

                if (enumFORMATETC.IsNull)
                {
                    return [];
                }

                // Since we are only adding elements to the HashSet, the order will be preserved.
                HashSet<string> distinctFormats = [];

                enumFORMATETC.Value->Reset();

                Com.FORMATETC[] formatEtc = [default];
                HRESULT hr;

                fixed (Com.FORMATETC* pFormatEtc = formatEtc)
                {
                    hr = enumFORMATETC.Value->Next(1, pFormatEtc);
                }

                if (hr == HRESULT.S_OK)
                {
                    string name = DataFormats.GetFormat(formatEtc[0].cfFormat).Name;
                    if (autoConvert)
                    {
                        string[] mappedFormats = GetMappedFormats(name)!;
                        for (int i = 0; i < mappedFormats.Length; i++)
                        {
                            distinctFormats.Add(mappedFormats[i]);
                        }
                    }
                    else
                    {
                        distinctFormats.Add(name);
                    }
                }

                return [.. distinctFormats];
            }

            public string[] GetFormats() => GetFormats(autoConvert: true);

            void IDataObject.SetData(string format, bool autoConvert, object? data) { }
            void IDataObject.SetData(string format, object? data) { }
            void IDataObject.SetData(Type format, object? data) { }
            void IDataObject.SetData(object? data) { }

            #endregion

            private bool GetDataPresentInner(string format)
            {
                Com.FORMATETC formatEtc = new()
                {
                    cfFormat = (ushort)(DataFormats.GetFormat(format).Id),
                    dwAspect = (uint)Com.DVASPECT.DVASPECT_CONTENT,
                    lindex = -1,
                    tymed = (uint)AllowedTymeds
                };

                using var nativeDataObject = _nativeDataObject.GetInterface();
                HRESULT hr = nativeDataObject.Value->QueryGetData(formatEtc);
                return hr.Succeeded;
            }
        }
    }
}
