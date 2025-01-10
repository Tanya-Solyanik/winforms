// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection.Metadata;

namespace System.Windows.Forms;

internal sealed class TypedDataObject : ITypedDataObject
{
    private readonly IDataObject _dataObject;

    public TypedDataObject(IDataObject dataObject)
    {
        Debug.Assert(dataObject is not ITypedDataObject, "dataObject implements its own ITypedDataObject methods.");
        ArgumentNullException.ThrowIfNull(dataObject);
        _dataObject = dataObject;
    }

    public object? GetData(string format, bool autoConvert) => _dataObject.GetData(format, autoConvert);
    public object? GetData(string format) => _dataObject.GetData(format);
    public object? GetData(Type format) => _dataObject.GetData(format);
    public bool GetDataPresent(string format, bool autoConvert) => _dataObject.GetDataPresent(format, autoConvert);
    public bool GetDataPresent(string format) => _dataObject.GetDataPresent(format);
    public bool GetDataPresent(Type format) => _dataObject.GetDataPresent(format);
    public string[] GetFormats(bool autoConvert) => _dataObject.GetFormats(autoConvert);
    public string[] GetFormats() => _dataObject.GetFormats();
    public void SetData(string format, bool autoConvert, object? data) => _dataObject.SetData(format, autoConvert, data);
    public void SetData(string format, object? data) => _dataObject.SetData(format, data);
    public void SetData(Type format, object? data) => _dataObject.SetData(format, data);
    public void SetData(object? data) => _dataObject.SetData(data);

    public bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>([NotNullWhen(true)] out T? data) =>
        TryGetData(typeof(T).FullName!, out data);

    public bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(string format, [NotNullWhen(true)] out T? data)
    {
        if (_dataObject.GetData(format) is T t)
        {
            data = t;
            return true;
        }

        data = default;
        return false;
    }

    public bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(string format, bool autoConvert, [NotNullWhen(true)] out T? data)
    {
        if (_dataObject.GetData(format, autoConvert) is T t)
        {
            data = t;
            return true;
        }

        data = default;
        return false;
    }

    public bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(string format, Func<TypeName, Type> typeResolver, bool autoConvert, [NotNullWhen(true)] out T? data) =>
        TryGetData(format, autoConvert, out data);
}
