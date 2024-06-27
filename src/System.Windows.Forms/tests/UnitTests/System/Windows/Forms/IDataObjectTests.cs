// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection.Metadata;

namespace System.Windows.Forms.Tests;

#nullable enable

// Test default implementation of IDataObject.TryGetData<T> overloads.
public partial class IDataObjectTests
{
    [Fact]
    public void IDataObject_TryGetData_Invoke_ReturnsTrue()
    {
        IDataObject dataObject = new DefaultTryGetMethodsDataObject();
        dataObject.TryGetData(out TypeNameFalse? _).Should().BeTrue();
        dataObject.TryGetData("format", out FormatFalse? _).Should().BeTrue();
        dataObject.TryGetData("format", autoConvert: false, out FormatFalse? _).Should().BeTrue();
        dataObject.TryGetData("format", autoConvert: true, out FormatTrue? _).Should().BeTrue();
        dataObject.TryGetData("format", Resolver, autoConvert: false, out FormatFalse? _).Should().BeTrue();
        dataObject.TryGetData("format", Resolver, autoConvert: true, out FormatTrue? _).Should().BeTrue();
    }

    [Fact]
    public void IDataObject_TryGetData_Invoke_ReturnsFalse()
    {
        IDataObject dataObject = new DefaultTryGetMethodsDataObject();
        dataObject.TryGetData(out DateTime? _).Should().BeFalse();
        dataObject.TryGetData(format: null!, out Stream? _).Should().BeFalse();
        dataObject.TryGetData("format", out Button? _).Should().BeFalse();
        dataObject.TryGetData("format", autoConvert: false, out Button? _).Should().BeFalse();
        dataObject.TryGetData("format", autoConvert: true, out Button? _).Should().BeFalse();
        dataObject.TryGetData("format", Resolver, autoConvert: false, out Button? _).Should().BeFalse();
        dataObject.TryGetData("format", Resolver, autoConvert: true, out Button? _).Should().BeFalse();
    }

    private static Type Resolver(TypeName typeName) => throw new NotImplementedException();

    private class FormatTrue() { }
    private class FormatFalse() { }
    private class TypeNameFalse() { }
    private class DefaultTryGetMethodsDataObject : IDataObject
    {
        private readonly Dictionary<(string, bool), object> _data = new()
        {
            [("format", true)] = new FormatTrue(),
            [("format", false)] = new FormatFalse(),
            [(typeof(TypeNameFalse).FullName!, false)] = new TypeNameFalse(),
            [(typeof(MemoryStream).FullName!, false)] = new MemoryStream(),
        };

        private object? GetDataInternal(string format, bool autoConvert)
        {
            if (_data.TryGetValue((format, autoConvert), out object? value))
            {
                return value;
            }

            return null;
        }

        public object? GetData(string format, bool autoConvert) => GetDataInternal(format, autoConvert);
        public object? GetData(string format) => GetDataInternal(format, autoConvert: false);
        public object? GetData(Type format) => GetDataInternal(format.FullName!, autoConvert: false);
        public bool GetDataPresent(string format, bool autoConvert) => throw new NotImplementedException();
        public bool GetDataPresent(string format) => throw new NotImplementedException();
        public bool GetDataPresent(Type format) => throw new NotImplementedException();
        public string[] GetFormats(bool autoConvert) => throw new NotImplementedException();
        public string[] GetFormats() => throw new NotImplementedException();
        public void SetData(string format, bool autoConvert, object? data) => throw new NotImplementedException();
        public void SetData(string format, object? data) => throw new NotImplementedException();
        public void SetData(Type format, object? data) => throw new NotImplementedException();
        public void SetData(object? data) => throw new NotImplementedException();
    }
}
