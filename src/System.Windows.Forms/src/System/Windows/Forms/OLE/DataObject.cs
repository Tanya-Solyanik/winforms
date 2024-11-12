// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Specialized;
using System.Drawing;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Com = Windows.Win32.System.Com;
using ComTypes = System.Runtime.InteropServices.ComTypes;

namespace System.Windows.Forms;

/// <summary>
///  Implements a basic data transfer mechanism.
/// </summary>
[ClassInterface(ClassInterfaceType.None)]
public unsafe partial class DataObject :
    IDataObject,
    ITypedDataObject,
    Com.IDataObject.Interface,
    ComTypes.IDataObject,
    Com.IManagedWrapper<Com.IDataObject>
{
    private const string CF_DEPRECATED_FILENAME = "FileName";
    private const string CF_DEPRECATED_FILENAMEW = "FileNameW";
    private const string BitmapFullName = "System.Drawing.Bitmap";

    private readonly Composition _innerData;

    internal static Type NotSupportedResolver(TypeName typeName) =>
       throw new NotSupportedException($"Using BinaryFormatter is not supported in WinForms Clipboard data deserialization." +
           $"  Can't resolve {typeName.AssemblyQualifiedName}.");

    /// <summary>
    ///  Initializes a new instance of the <see cref="DataObject"/> class, with the raw <see cref="Com.IDataObject"/>
    ///  and the managed data object the raw pointer is associated with.
    /// </summary>
    internal DataObject(Com.IDataObject* data) => _innerData = Composition.CreateFromNativeDataObject(data);

    /// <summary>
    ///  Initializes a new instance of the <see cref="DataObject"/> class, which can store arbitrary data.
    /// </summary>
    public DataObject()
    {
        _innerData = Composition.CreateFromWinFormsDataObject(new DataStore());
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref="DataObject"/> class, containing the specified data.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   If <paramref name="data"/> implements an <see cref="IDataObject"/> interface,
    ///   we strongly recommend implementing <see cref="ITypedDataObject"/> to support the
    ///   `TryGetData` API family that restricts deserialization to the requested and known types.
    ///   <see cref="Clipboard.TryGetData{T}(string, out T)"/> will throw <see cref="NotSupportedException"/>
    ///   if <see cref="ITypedDataObject"/> is not implemented.
    ///  </para>
    /// </remarks>
    public DataObject(object data)
    {
        if (data is DataObject dataObject)
        {
            _innerData = dataObject._innerData;
        }
        else if (data is IDataObject iDataObject)
        {
            _innerData = Composition.CreateFromWinFormsDataObject(iDataObject);
        }
        else if (data is ComTypes.IDataObject comDataObject)
        {
            _innerData = Composition.CreateFromRuntimeDataObject(comDataObject);
        }
        else
        {
            _innerData = Composition.CreateFromWinFormsDataObject(new DataStore());
            SetData(data);
        }
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref="DataObject"/> class, containing the specified data and its
    ///  associated format.
    /// </summary>
    public DataObject(string format, object data) : this() => SetData(format, data);

    internal DataObject(string format, bool autoConvert, object data) : this() => SetData(format, autoConvert, data);

    /// <summary>
    ///  Flags that the original data was wrapped for clipboard purposes.
    /// </summary>
    internal bool IsWrappedForClipboard { get; init; }

    /// <summary>
    ///  Returns the inner data that the <see cref="DataObject"/> was created with if the original data implemented
    ///  <see cref="IDataObject"/>. Otherwise, returns this.
    ///  This method should only be used if the <see cref="DataObject"/> was created for clipboard purposes.
    /// </summary>
    internal IDataObject TryUnwrapInnerIDataObject()
    {
        Debug.Assert(IsWrappedForClipboard, "This method should only be used for clipboard purposes.");
        return _innerData.OriginalIDataObject is { } original ? original : this;
    }

    /// <inheritdoc cref="Composition.OriginalIDataObject"/>
    internal IDataObject? OriginalIDataObject => _innerData.OriginalIDataObject;

    #region IDataObject
    [Obsolete(
        Obsoletions.DataObjectGetDataMessage,
        error: false,
        DiagnosticId = Obsoletions.ClipboardGetDataDiagnosticId,
        UrlFormat = Obsoletions.SharedUrlFormat)]
    public virtual object? GetData(string format, bool autoConvert) =>
        ((IDataObject)_innerData).GetData(format, autoConvert);

    [Obsolete(
        Obsoletions.DataObjectGetDataMessage,
        error: false,
        DiagnosticId = Obsoletions.ClipboardGetDataDiagnosticId,
        UrlFormat = Obsoletions.SharedUrlFormat)]
    public virtual object? GetData(string format) => GetData(format, autoConvert: true);

    [Obsolete(
        Obsoletions.DataObjectGetDataMessage,
        error: false,
        DiagnosticId = Obsoletions.ClipboardGetDataDiagnosticId,
        UrlFormat = Obsoletions.SharedUrlFormat)]
    public virtual object? GetData(Type format) => format is null ? null : GetData(format.FullName!);

    public virtual bool GetDataPresent(string format, bool autoConvert) =>
        ((IDataObject)_innerData).GetDataPresent(format, autoConvert);

    public virtual bool GetDataPresent(string format) => GetDataPresent(format, autoConvert: true);

    public virtual bool GetDataPresent(Type format) => format is not null && GetDataPresent(format.FullName!);

    public virtual string[] GetFormats(bool autoConvert) => ((IDataObject)_innerData).GetFormats(autoConvert);

    public virtual string[] GetFormats() => GetFormats(autoConvert: true);

    public virtual void SetData(string format, bool autoConvert, object? data)
        => ((IDataObject)_innerData).SetData(format, autoConvert, data);

    public virtual void SetData(string format, object? data) => ((IDataObject)_innerData).SetData(format, data);

    public virtual void SetData(Type format, object? data) => ((IDataObject)_innerData).SetData(format, data);

    public virtual void SetData(object? data) => ((IDataObject)_innerData).SetData(data);
    #endregion

    #region ITypedDataObject
    [CLSCompliant(false)]
    public bool TryGetData<T>(
        string format,
        Func<TypeName, Type> resolver,
        bool autoConvert,
        [NotNullWhen(true), MaybeNullWhen(false)] out T data) =>
            // TODO (TanyaSo) argument validation here??
            TryGetDataCore(format, resolver, autoConvert, out data);

    public bool TryGetData<T>(
        string format,
        bool autoConvert,
        [NotNullWhen(true), MaybeNullWhen(false)] out T data) =>
            TryGetData(format, NotSupportedResolver, autoConvert, out data);

    public bool TryGetData<T>(
        string format,
        [NotNullWhen(true), MaybeNullWhen(false)] out T data) =>
        TryGetData(format, autoConvert: false, out data);

    public bool TryGetData<T>(
        [NotNullWhen(true), MaybeNullWhen(false)] out T data) =>
            TryGetData(typeof(T).FullName!, out data);
    #endregion

    public virtual bool ContainsAudio() => GetDataPresent(DataFormats.WaveAudioConstant, autoConvert: false);

    public virtual bool ContainsFileDropList() => GetDataPresent(DataFormats.FileDropConstant, autoConvert: true);

    public virtual bool ContainsImage() => GetDataPresent(DataFormats.BitmapConstant, autoConvert: true);

    public virtual bool ContainsText() => ContainsText(TextDataFormat.UnicodeText);

    public virtual bool ContainsText(TextDataFormat format)
    {
        // Valid values are 0x0 to 0x4
        SourceGenerated.EnumValidator.Validate(format, nameof(format));
        return GetDataPresent(ConvertToDataFormats(format), autoConvert: false);
    }

#pragma warning disable WFDEV005 // Type or member is obsolete
    public virtual Stream? GetAudioStream() => GetData(DataFormats.WaveAudio, autoConvert: false) as Stream;

    public virtual StringCollection GetFileDropList()
    {
        StringCollection dropList = [];
        if (GetData(DataFormats.FileDropConstant, autoConvert: true) is string[] strings)
        {
            dropList.AddRange(strings);
        }

        return dropList;
    }

    public virtual Image? GetImage() => GetData(DataFormats.Bitmap, autoConvert: true) as Image;

    public virtual string GetText() => GetText(TextDataFormat.UnicodeText);

    public virtual string GetText(TextDataFormat format)
    {
        // Valid values are 0x0 to 0x4
        SourceGenerated.EnumValidator.Validate(format, nameof(format));
        return GetData(ConvertToDataFormats(format), false) is string text ? text : string.Empty;
    }
#pragma warning restore WFDEV005

    public virtual void SetAudio(byte[] audioBytes) => SetAudio(new MemoryStream(audioBytes.OrThrowIfNull()));

    public virtual void SetAudio(Stream audioStream) =>
        SetData(DataFormats.WaveAudioConstant, autoConvert: false, audioStream.OrThrowIfNull());

    public virtual void SetFileDropList(StringCollection filePaths)
    {
        string[] strings = new string[filePaths.OrThrowIfNull().Count];
        filePaths.CopyTo(strings, 0);
        SetData(DataFormats.FileDropConstant, true, strings);
    }

    public virtual void SetImage(Image image) => SetData(DataFormats.BitmapConstant, true, image.OrThrowIfNull());

    public virtual void SetText(string textData) => SetText(textData, TextDataFormat.UnicodeText);

    public virtual void SetText(string textData, TextDataFormat format)
    {
        textData.ThrowIfNullOrEmpty();

        // Valid values are 0x0 to 0x4
        SourceGenerated.EnumValidator.Validate(format, nameof(format));

        SetData(ConvertToDataFormats(format), false, textData);
    }

    [CLSCompliant(false)]
    protected virtual bool TryGetDataCore<T>(
        string format,
        Func<TypeName, Type> resolver,
        bool autoConvert,
        [NotNullWhen(true), MaybeNullWhen(false)] out T data) =>
            ((ITypedDataObject)_innerData).TryGetData(format, resolver, autoConvert, out data);

    /// <summary>
    ///  Verify if the requested format is valid and compatible with the requested type <typeparamref name="T"/>.
    ///  If a custom <paramref name="resolver"/> is provided, it will be validated at the resolution time.
    /// </summary>
    internal static bool ValidateTryGetDataArguments<T>(string format, Func<TypeName, Type>? resolver = default)
    {
        if (string.IsNullOrWhiteSpace(format))
        {
            return false;
        }

        if (IsInvalidPredefinedFormatType(format))
        {
            throw new NotSupportedException(
                $"'{typeof(T).Name}' is not compatible with the specified format `{format}`.");
        }

        if (resolver is null
            && !IsRestrictedFormat(format)
            // Check is a convenience for simple usages where user isn't passing a resolver explicitly.
            && IsUnboundedType())
        {
            // Tanyaso TODO: localize string
            throw new NotSupportedException(
                $"'{typeof(T).Name}' is not a concrete type, and could allow for " +
                $"unbounded deserialization.  Use a concrete type or define a resolver " +
                $"function that supports types that you are retrieving from the Clipboard.");
        }

        return true;

        static bool IsUnboundedType()
        {
            if (typeof(T) == typeof(object))
            {
                return true;
            }

            Type type = typeof(T);
            return type.IsInterface || type.IsAbstract;
        }

        static bool IsInvalidPredefinedFormatType(string format) => format switch
        {
            DataFormats.TextConstant
                or DataFormats.UnicodeTextConstant
                or DataFormats.StringConstant
                or DataFormats.RtfConstant
                or DataFormats.HtmlConstant
                or DataFormats.OemTextConstant => typeof(string) != typeof(T),

            DataFormats.FileDropConstant
                or CF_DEPRECATED_FILENAME
                or CF_DEPRECATED_FILENAMEW => typeof(string[]) != typeof(T),

            DataFormats.BitmapConstant or BitmapFullName =>
                typeof(Bitmap) != typeof(T) && typeof(Image) != typeof(T),
            _ => false
        };

        static bool IsRestrictedFormat(string format) =>
            format is DataFormats.StringConstant
                or BitmapFullName
                or DataFormats.CsvConstant
                or DataFormats.DibConstant
                or DataFormats.DifConstant
                or DataFormats.LocaleConstant
                or DataFormats.PenDataConstant
                or DataFormats.RiffConstant
                or DataFormats.SymbolicLinkConstant
                or DataFormats.TiffConstant
                or DataFormats.WaveAudioConstant
                or DataFormats.BitmapConstant
                or DataFormats.EmfConstant
                or DataFormats.PaletteConstant
                or DataFormats.WmfConstant
                or DataFormats.TextConstant
                or DataFormats.UnicodeTextConstant
                or DataFormats.RtfConstant
                or DataFormats.HtmlConstant
                or DataFormats.OemTextConstant
                or DataFormats.FileDropConstant
                or CF_DEPRECATED_FILENAME
                or CF_DEPRECATED_FILENAMEW;
    }

    private static string ConvertToDataFormats(TextDataFormat format) => format switch
    {
        TextDataFormat.UnicodeText => DataFormats.UnicodeTextConstant,
        TextDataFormat.Rtf => DataFormats.RtfConstant,
        TextDataFormat.Html => DataFormats.HtmlConstant,
        TextDataFormat.CommaSeparatedValue => DataFormats.CsvConstant,
        _ => DataFormats.UnicodeTextConstant,
    };

    /// <summary>
    ///  Returns all the "synonyms" for the specified format.
    /// </summary>
    private static string[]? GetMappedFormats(string format) => format switch
    {
        null => null,
        DataFormats.TextConstant or DataFormats.UnicodeTextConstant or DataFormats.StringConstant =>
            [
                DataFormats.StringConstant,
                DataFormats.UnicodeTextConstant,
                DataFormats.TextConstant
            ],
        DataFormats.FileDropConstant or CF_DEPRECATED_FILENAME or CF_DEPRECATED_FILENAMEW =>
            [
                DataFormats.FileDropConstant,
                CF_DEPRECATED_FILENAMEW,
                CF_DEPRECATED_FILENAME
            ],
        DataFormats.BitmapConstant or BitmapFullName =>
            [
                BitmapFullName,
                DataFormats.BitmapConstant
            ],
        _ => [format]
    };

    #region ComTypes.IDataObject
    int ComTypes.IDataObject.DAdvise(ref FORMATETC pFormatetc, ADVF advf, IAdviseSink pAdvSink, out int pdwConnection) =>
        ((ComTypes.IDataObject)_innerData).DAdvise(ref pFormatetc, advf, pAdvSink, out pdwConnection);

    void ComTypes.IDataObject.DUnadvise(int dwConnection) => ((ComTypes.IDataObject)_innerData).DUnadvise(dwConnection);

    int ComTypes.IDataObject.EnumDAdvise(out IEnumSTATDATA? enumAdvise) =>
        ((ComTypes.IDataObject)_innerData).EnumDAdvise(out enumAdvise);

    IEnumFORMATETC ComTypes.IDataObject.EnumFormatEtc(DATADIR dwDirection) =>
        ((ComTypes.IDataObject)_innerData).EnumFormatEtc(dwDirection);

    int ComTypes.IDataObject.GetCanonicalFormatEtc(ref FORMATETC pformatetcIn, out FORMATETC pformatetcOut) =>
        ((ComTypes.IDataObject)_innerData).GetCanonicalFormatEtc(ref pformatetcIn, out pformatetcOut);

    void ComTypes.IDataObject.GetData(ref FORMATETC formatetc, out STGMEDIUM medium) =>
        ((ComTypes.IDataObject)_innerData).GetData(ref formatetc, out medium);

    void ComTypes.IDataObject.GetDataHere(ref FORMATETC formatetc, ref STGMEDIUM medium) =>
        ((ComTypes.IDataObject)_innerData).GetDataHere(ref formatetc, ref medium);

    int ComTypes.IDataObject.QueryGetData(ref FORMATETC formatetc) =>
        ((ComTypes.IDataObject)_innerData).QueryGetData(ref formatetc);

    void ComTypes.IDataObject.SetData(ref FORMATETC pFormatetcIn, ref STGMEDIUM pmedium, bool fRelease) =>
        ((ComTypes.IDataObject)_innerData).SetData(ref pFormatetcIn, ref pmedium, fRelease);

    #endregion

    #region Com.IDataObject

    HRESULT Com.IDataObject.Interface.DAdvise(Com.FORMATETC* pformatetc, uint advf, Com.IAdviseSink* pAdvSink, uint* pdwConnection) =>
        ((Com.IDataObject.Interface)_innerData).DAdvise(pformatetc, advf, pAdvSink, pdwConnection);

    HRESULT Com.IDataObject.Interface.DUnadvise(uint dwConnection) =>
        ((Com.IDataObject.Interface)_innerData).DUnadvise(dwConnection);

    HRESULT Com.IDataObject.Interface.EnumDAdvise(Com.IEnumSTATDATA** ppenumAdvise) =>
        ((Com.IDataObject.Interface)_innerData).EnumDAdvise(ppenumAdvise);

    HRESULT Com.IDataObject.Interface.EnumFormatEtc(uint dwDirection, Com.IEnumFORMATETC** ppenumFormatEtc) =>
        ((Com.IDataObject.Interface)_innerData).EnumFormatEtc(dwDirection, ppenumFormatEtc);

    HRESULT Com.IDataObject.Interface.GetData(Com.FORMATETC* pformatetcIn, Com.STGMEDIUM* pmedium) =>
        ((Com.IDataObject.Interface)_innerData).GetData(pformatetcIn, pmedium);

    HRESULT Com.IDataObject.Interface.GetDataHere(Com.FORMATETC* pformatetc, Com.STGMEDIUM* pmedium) =>
        ((Com.IDataObject.Interface)_innerData).GetDataHere(pformatetc, pmedium);

    HRESULT Com.IDataObject.Interface.QueryGetData(Com.FORMATETC* pformatetc) =>
        ((Com.IDataObject.Interface)_innerData).QueryGetData(pformatetc);

    HRESULT Com.IDataObject.Interface.GetCanonicalFormatEtc(Com.FORMATETC* pformatectIn, Com.FORMATETC* pformatetcOut) =>
        ((Com.IDataObject.Interface)_innerData).GetCanonicalFormatEtc(pformatectIn, pformatetcOut);

    HRESULT Com.IDataObject.Interface.SetData(Com.FORMATETC* pformatetc, Com.STGMEDIUM* pmedium, BOOL fRelease) =>
        ((Com.IDataObject.Interface)_innerData).SetData(pformatetc, pmedium, fRelease);

    #endregion
}
