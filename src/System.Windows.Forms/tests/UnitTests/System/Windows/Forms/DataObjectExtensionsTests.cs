// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Reflection.Metadata;

namespace System.Windows.Forms.Tests;

public class DataObjectExtensionsTests
{
    [Fact]
    public void TryGetData_Throws_ArgumentNull()
    {
        ((Action)(() => DataObjectExtensions.TryGetData<string>(null!, out _))).Should().Throw<ArgumentNullException>();
        ((Action)(() => DataObjectExtensions.TryGetData<string>(null!, DataFormats.Text, out _))).Should().Throw<ArgumentNullException>();
        ((Action)(() => DataObjectExtensions.TryGetData<string>(null!, DataFormats.CommaSeparatedValue, autoConvert: true, out _))).Should().Throw<ArgumentNullException>();
        ((Action)(() => DataObjectExtensions.TryGetData<string>(null!, DataFormats.Serializable, autoConvert: false, out _))).Should().Throw<ArgumentNullException>();
        ((Action)(() => DataObjectExtensions.TryGetData<string>(null!, DataFormats.UnicodeText, Resolver, autoConvert: true, out _))).Should().Throw<ArgumentNullException>();
        ((Action)(() => DataObjectExtensions.TryGetData<string>(null!, DataFormats.Bitmap, Resolver, autoConvert: false, out _))).Should().Throw<ArgumentNullException>();
    }

    private static Type Resolver(TypeName typeName) => typeof(string);

    [Fact]
    public void TryGetData_Throws_Argument()
    {
        UntypedDataObject dataObject = new();

        ((Action)(() => dataObject.TryGetData<string>(out _))).Should().Throw<ArgumentException>();
        dataObject.VerifyGetDataWasNotCalled();
    }

    [Fact]
    public void TryGetData_String_Throws_Argument()
    {
        UntypedDataObject dataObject = new();
        ((Action)(() => dataObject.TryGetData<string>(DataFormats.Text, out _))).Should().Throw<ArgumentException>();
        dataObject.VerifyGetDataWasNotCalled();
    }

    [Theory]
    [BoolData]
    public void TryGetData_StringBool_Throws_Argument(bool autoConvert)
    {
        UntypedDataObject dataObject = new();
        ((Action)(() => dataObject.TryGetData<string>(DataFormats.CommaSeparatedValue, autoConvert, out _))).Should().Throw<ArgumentException>();
        dataObject.VerifyGetDataWasNotCalled();
    }

    [Theory]
    [BoolData]
    public void TryGetData_StringFuncBool_Throws_Argument(bool autoConvert)
    {
        UntypedDataObject dataObject = new();
        ((Action)(() => dataObject.TryGetData<string>(DataFormats.UnicodeText, Resolver, autoConvert, out _))).Should().Throw<ArgumentException>();
        dataObject.VerifyGetDataWasNotCalled();
    }

    [Fact]
    public void DataObject_ReturnFalse()
    {
        DataObject dataObject = new();
        ((IDataObject)dataObject).TryGetData(out string? text).Should().BeFalse();
        text.Should().BeNull();
    }

    [Fact]
    public void DataObject_String_ReturnsFalse()
    {
        DataObject dataObject = new();
        ((IDataObject)dataObject).TryGetData(DataFormats.Dib, out Bitmap? bitmap).Should().BeFalse();
        bitmap.Should().BeNull();
    }

    [Theory]
    [BoolData]
    public void DataObject_StringBool_ReturnFalse(bool autoConvert)
    {
        DataObject dataObject = new();
        ((IDataObject)dataObject).TryGetData(DataFormats.Serializable, autoConvert, out Font? font).Should().BeFalse();
        font.Should().BeNull();
    }

    [Theory]
    [BoolData]
    public void DataObject_StringFuncBool_ReturnFalse(bool autoConvert)
    {
        DataObject dataObject = new();
        ((IDataObject)dataObject).TryGetData(DataFormats.SymbolicLink, Resolver, autoConvert, out DateTime? date).Should().BeFalse();
        date.Should().BeNull();
    }

    [Fact]
    public void TypedDataObject_CallsITypedDataObject()
    {
        TypedDataObject dataObject = new();
        ((IDataObject)dataObject).TryGetData(out string? _).Should().BeFalse();
        dataObject.VerifyTryGetDataCalled();
    }

    [Fact]
    public void TypedDataObject_String_CallsITypedDataObject()
    {
        TypedDataObject dataObject = new();
        ((IDataObject)dataObject).TryGetData(DataFormats.Dib, out Bitmap? _).Should().BeFalse();
        dataObject.VerifyTryGetDataStringCalled();
    }

    [Theory]
    [BoolData]
    public void TypedDataObject_StringBool_CallsITypedDataObject(bool autoConvert)
    {
        TypedDataObject dataObject = new();
        ((IDataObject)dataObject).TryGetData(DataFormats.FileDrop, autoConvert, out int? _).Should().BeFalse();
        dataObject.VerifyTryGetDataStringBoolCalled();
    }

    [Theory]
    [BoolData]
    public void TypedDataObject_StringFuncBool_CallsITypedDataObject(bool autoConvert)
    {
        TypedDataObject dataObject = new();
        ((IDataObject)dataObject).TryGetData(DataFormats.SymbolicLink, Resolver, autoConvert, out DateTime? date).Should().BeFalse();
        dataObject.VerifyTryGetDataStringFuncBoolCalled();
    }

    internal class UntypedDataObject : IDataObject
    {
        public void VerifyGetDataWasNotCalled()
        {
            GetDataType_Count.Should().Be(0);
            GetDataString_Count.Should().Be(0);
            GetDataStringBool_Count.Should().Be(0);
        }

        private int GetDataStringBool_Count { get; set; }
        public object? GetData(string format, bool autoConvert)
        {
            GetDataStringBool_Count++;
            return null;
        }

        private int GetDataString_Count { get; set; }
        public object? GetData(string format)
        {
            GetDataString_Count++;
            return null;
        }

        private int GetDataType_Count { get; set; }
        public object? GetData(Type format)
        {
            GetDataType_Count++;
            return null;
        }

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

    internal class TypedDataObject : IDataObject, ITypedDataObject
    {
        public object? GetData(string format, bool autoConvert) => throw new NotImplementedException();
        public object? GetData(string format) => throw new NotImplementedException();
        public object? GetData(Type format) => throw new NotImplementedException();
        public bool GetDataPresent(string format, bool autoConvert) => throw new NotImplementedException();
        public bool GetDataPresent(string format) => throw new NotImplementedException();
        public bool GetDataPresent(Type format) => throw new NotImplementedException();
        public string[] GetFormats(bool autoConvert) => throw new NotImplementedException();
        public string[] GetFormats() => throw new NotImplementedException();
        public void SetData(string format, bool autoConvert, object? data) => throw new NotImplementedException();
        public void SetData(string format, object? data) => throw new NotImplementedException();
        public void SetData(Type format, object? data) => throw new NotImplementedException();
        public void SetData(object? data) => throw new NotImplementedException();

        private int _tryGetDataCalledCount;
        private int _tryGetDataStringCalledCount;
        private int _tryGetDataStringBoolCalledCount;
        private int _tryGetDataStringFuncBoolCalledCount;

        public void VerifyTryGetDataCalled()
        {
            _tryGetDataCalledCount.Should().Be(1);
            _tryGetDataStringCalledCount.Should().Be(0);
            _tryGetDataStringBoolCalledCount.Should().Be(0);
            _tryGetDataStringFuncBoolCalledCount.Should().Be(0);
        }

        public void VerifyTryGetDataStringCalled()
        {
            _tryGetDataCalledCount.Should().Be(0);
            _tryGetDataStringCalledCount.Should().Be(1);
            _tryGetDataStringBoolCalledCount.Should().Be(0);
            _tryGetDataStringFuncBoolCalledCount.Should().Be(0);
        }

        public void VerifyTryGetDataStringBoolCalled()
        {
            _tryGetDataCalledCount.Should().Be(0);
            _tryGetDataStringCalledCount.Should().Be(0);
            _tryGetDataStringBoolCalledCount.Should().Be(1);
            _tryGetDataStringFuncBoolCalledCount.Should().Be(0);
        }

        public void VerifyTryGetDataStringFuncBoolCalled()
        {
            _tryGetDataCalledCount.Should().Be(0);
            _tryGetDataStringCalledCount.Should().Be(0);
            _tryGetDataStringBoolCalledCount.Should().Be(0);
            _tryGetDataStringFuncBoolCalledCount.Should().Be(1);
        }

        public bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>([MaybeNullWhen(false), NotNullWhen(true)] out T data)
        {
            _tryGetDataCalledCount++;
            data = default;
            return false;
        }

        public bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(string format, [MaybeNullWhen(false), NotNullWhen(true)] out T data)
        {
            _tryGetDataStringCalledCount++;
            data = default;
            return false;
        }

        public bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(string format, bool autoConvert, [MaybeNullWhen(false), NotNullWhen(true)] out T data)
        {
            _tryGetDataStringBoolCalledCount++;
            data = default;
            return false;
        }

        public bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(string format, Func<TypeName, Type> resolver, bool autoConvert, [MaybeNullWhen(false), NotNullWhen(true)] out T data)
        {
            _tryGetDataStringFuncBoolCalledCount++;
            data = default;
            return false;
        }
    }
}
