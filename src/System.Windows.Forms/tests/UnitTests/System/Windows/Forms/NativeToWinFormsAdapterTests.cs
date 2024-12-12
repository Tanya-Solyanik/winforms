﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Drawing;
using System.Reflection.Metadata;
using Com = Windows.Win32.System.Com;

namespace System.Windows.Forms.Tests;

public unsafe class NativeToWinFormsAdapterTests
{
    public static TheoryData<string> UnboundedFormat() =>
    [
        DataFormats.Serializable,
        "something custom"
    ];

    // These formats contain only known types.
    public static TheoryData<string> UndefinedRestrictedFormat() =>
    [
        DataFormats.CommaSeparatedValue,
        DataFormats.Dib,
        DataFormats.Dif,
        DataFormats.PenData,
        DataFormats.Riff,
        DataFormats.Tiff,
        DataFormats.WaveAudio,
        DataFormats.SymbolicLink,
        DataFormats.EnhancedMetafile,
        DataFormats.MetafilePict,
        DataFormats.Palette
    ];

    public static TheoryData<string> BitmapFormat() =>
    [
        DataFormats.Bitmap,
        "System.Drawing.Bitmap"
    ];

    // These formats set and get strings by accessing HGLOBAL directly.
    public static TheoryData<string> StringFormat() =>
    [
         DataFormats.Text,
         DataFormats.UnicodeText,
         DataFormats.StringConstant,
         DataFormats.Rtf,
         DataFormats.Html,
         DataFormats.OemText,
         DataFormats.FileDrop,
         "FileName",
         "FileNameW"
    ];

    private const string InvalidTypeFormatCombinationMessage = "Type '*' is not compatible with the specified format '*'.";
    private const string RequiresResolverMessage =
        $"'*' is not a concrete type, and could allow for unbounded deserialization.  Use a concrete type or" +
        $" define a resolver function that supports types that you are retrieving from the Clipboard or being dragged and dropped.";
    private const string UseTryGetDataWithResolver =
        "Use 'TryGetData<T>' method with a 'resolver' function that defines a set of allowed types, to deserialize '*'.";
    private const string FormatterDisabledMessage =
        "BinaryFormatter serialization and deserialization are disabled within this application. See https://aka.ms/binaryformatter for more information.";

    [WinFormsTheory]
    [MemberData(nameof(UndefinedRestrictedFormat))]
    public void TryGetData_AsObject_Primitive_Success(string format)
    {
        DataObject native = new();
        // Primitive type is serialized by generating the record field by field.
        native.SetData(format, 1);
        DataObject dataObject = new(ComHelpers.GetComPointer<Com.IDataObject>(native));

        // Format is compatible with `object` type, validation passed.
        // The `int` type can be assigned to an `object`, thus record type matched the requested type.
        // The primitive type is read from the serialization record field by field.
        dataObject.TryGetData(format, out object? value).Should().BeTrue();
        value.Should().Be(1);
    }

    [WinFormsTheory]
    [MemberData(nameof(UnboundedFormat))]
    public void TryGetData_AsObject_Primitive_RequiresResolver(string format)
    {
        DataObject native = new();
        // Primitive type is serialized by generating the record field by field.
        native.SetData(format, 1);
        DataObject dataObject = new(ComHelpers.GetComPointer<Com.IDataObject>(native));

        Action tryGetData = () => dataObject.TryGetData(format, out object? value);
        tryGetData.Should().Throw<NotSupportedException>().WithMessage(expectedWildcardPattern: RequiresResolverMessage);

        dataObject.TryGetData(format, Resolver, autoConvert: false, out object? value).Should().BeTrue();
        value.Should().Be(1);

        static Type Resolver(TypeName typeName) => typeof(int);
    }

    [WinFormsTheory]
    [MemberData(nameof(StringFormat))]
    [MemberData(nameof(BitmapFormat))]
    public void TryGetData_AsObject_Primitive_InvalidTypeFormatCombination(string format)
    {
        DataObject native = new();
        native.SetData(format, 1);
        DataObject dataObject = new(ComHelpers.GetComPointer<Com.IDataObject>(native));

        // Throw when validating arguments, as these formats allow exactly strings or bitmaps only.
        Action tryGetData = () => dataObject.TryGetData(format, out object? _);
        tryGetData.Should().Throw<NotSupportedException>()
            .WithMessage(expectedWildcardPattern: InvalidTypeFormatCombinationMessage);
    }

    private static (DataObject dataObject, TestData value) SetDataObject(string format)
    {
        DataObject native = new();
        TestData value = new(new(6, 7));
        // This code does not flush the data.
        native.SetData(format, value);
        DataObject dataObject = new(ComHelpers.GetComPointer<Com.IDataObject>(native));

        return (dataObject, value);
    }

    [WinFormsTheory]
    [MemberData(nameof(UnboundedFormat))]
    public void TryGetData_AsObject_Custom_RequiresResolver(string format)
    {
        (DataObject dataObject, TestData _) = SetDataObject(format);
        Action tryGetData = () => dataObject.TryGetData(format, out object? _);

        tryGetData.Should().Throw<NotSupportedException>().WithMessage(expectedWildcardPattern: RequiresResolverMessage);
    }

    [WinFormsTheory]
    [MemberData(nameof(UnboundedFormat))]
    public void TryGetData_AsObject_Custom_FormatterEnabled_RequiresResolver(string format)
    {
        (DataObject dataObject, TestData _) = SetDataObject(format);
        Action tryGetData = () => dataObject.TryGetData(format, out object? _);

        using BinaryFormatterScope scope = new(enable: true);
        using BinaryFormatterInClipboardDragDropScope clipboardScope = new(enable: true);

        tryGetData.Should().Throw<NotSupportedException>().WithMessage(expectedWildcardPattern: RequiresResolverMessage);
    }

    [WinFormsTheory]
    [MemberData(nameof(StringFormat))]
    [MemberData(nameof(BitmapFormat))]
    public void TryGetData_AsObject_Custom_InvalidTypeFormatCombination(string format)
    {
        (DataObject dataObject, TestData _) = SetDataObject(format);
        Action tryGetData = () => dataObject.TryGetData(format, out object? _);

        // Type-Format combination is validated before the we attempt to serialize data.
        tryGetData.Should().Throw<NotSupportedException>()
            .WithMessage(expectedWildcardPattern: InvalidTypeFormatCombinationMessage);
    }

    [WinFormsTheory]
    [MemberData(nameof(UndefinedRestrictedFormat))]
    public void TryGetData_AsObject_Custom_ReturnsNotSupportedException(string format)
    {
        (DataObject dataObject, TestData _) = SetDataObject(format);

        // SetData writes NotSupportedException to HGLOBAL to indicate that formatters are disabled,
        // but the restricted format can't read it.
        dataObject.TryGetData(format, out object? result).Should().BeFalse();
        result.Should().BeNull();
    }

    [WinFormsTheory]
    [MemberData(nameof(UndefinedRestrictedFormat))]
    public void TryGetData_AsObject_Custom_FormatterEnabled_ReturnsFalse(string format)
    {
        (DataObject dataObject, TestData _) = SetDataObject(format);

        using BinaryFormatterScope scope = new(enable: true);
        using BinaryFormatterInClipboardDragDropScope clipboardScope = new(enable: true);

        dataObject.TryGetData(format, out object? result).Should().BeFalse();
        result.Should().BeNull();
    }

    [WinFormsTheory]
    [MemberData(nameof(UndefinedRestrictedFormat))]
    public void TryGetData_AsInterface_ListOfPrimitives_Success(string format)
    {
        DataObject native = new();
        List<int> value = [1];
        native.SetData(format, value);
        DataObject dataObject = new(ComHelpers.GetComPointer<Com.IDataObject>(native));

        dataObject.TryGetData(format, out IList<int>? list).Should().BeTrue();
        list.Should().BeEquivalentTo(value);
    }

    [WinFormsTheory]
    [MemberData(nameof(UnboundedFormat))]
    public void TryGetData_AsInterface_ListOfPrimitives_RequiresResolver(string format)
    {
        DataObject native = new();
        List<int> value = [1];
        native.SetData(format, value);
        DataObject dataObject = new(ComHelpers.GetComPointer<Com.IDataObject>(native));

        // Theoretically we don't require a resolver here, but this is an exception. In the more common cases resolver
        // is required to instantiate non-concrete types.
        Action tryGetData = () => dataObject.TryGetData(format, out IList<int>? _);
        tryGetData.Should().Throw<NotSupportedException>().WithMessage(expectedWildcardPattern: RequiresResolverMessage);
    }

    [WinFormsTheory]
    [MemberData(nameof(StringFormat))]
    [MemberData(nameof(BitmapFormat))]
    public void TryGetData_AsInterface_ListOfPrimitives_InvalidTypeFormatCombination(string format)
    {
        DataObject native = new();
        List<int> value = [1];
        native.SetData(format, value);
        DataObject dataObject = new(ComHelpers.GetComPointer<Com.IDataObject>(native));

        Action tryGetData = () => dataObject.TryGetData(format, out IList<int>? _);
        tryGetData.Should().Throw<NotSupportedException>()
            .WithMessage(expectedWildcardPattern: InvalidTypeFormatCombinationMessage);
    }

    [WinFormsTheory]
    [MemberData(nameof(UnboundedFormat))]
    [MemberData(nameof(UndefinedRestrictedFormat))]
    public void TryGetData_AsConcreteType_ListOfPrimitives_Success(string format)
    {
        DataObject native = new();
        List<int> value = [1];
        native.SetData(format, value);
        DataObject dataObject = new(ComHelpers.GetComPointer<Com.IDataObject>(native));

        dataObject.TryGetData(format, out List<int>? list).Should().BeTrue();
        list.Should().BeEquivalentTo(value);
    }

    [WinFormsTheory]
    [MemberData(nameof(StringFormat))]
    [MemberData(nameof(BitmapFormat))]
    public void TryGetData_AsConcreteType_ListOfPrimitives_InvalidTypeFormatCombination(string format)
    {
        DataObject native = new();
        List<int> value = [1];
        native.SetData(format, value);
        DataObject dataObject = new(ComHelpers.GetComPointer<Com.IDataObject>(native));

        Action tryGetData = () => dataObject.TryGetData(format, out List<int>? _);
        tryGetData.Should().Throw<NotSupportedException>()
            .WithMessage(expectedWildcardPattern: InvalidTypeFormatCombinationMessage);
    }

    [WinFormsTheory]
    [MemberData(nameof(UnboundedFormat))]
    public void TryGetData_AsConcreteType_Custom_FormatterEnabled_RequiresResolver(string format)
    {
        (DataObject dataObject, TestData _) = SetDataObject(format);
        Action tryGetData = () => dataObject.TryGetData(format, out TestData? _);

        using BinaryFormatterScope scope = new(enable: true);
        using BinaryFormatterInClipboardDragDropScope clipboardScope = new(enable: true);

        tryGetData.Should().Throw<NotSupportedException>().WithMessage(expectedWildcardPattern: UseTryGetDataWithResolver);
    }

    [WinFormsTheory]
    [MemberData(nameof(UndefinedRestrictedFormat))]
    public void TryGetData_AsConcreteType_Custom_FormatterEnabled_ReturnsFalse(string format)
    {
        (DataObject dataObject, TestData _) = SetDataObject(format);

        using BinaryFormatterScope scope = new(enable: true);
        using BinaryFormatterInClipboardDragDropScope clipboardScope = new(enable: true);

        // Format-type combination is invalid, serialization threw an exception.
        dataObject.TryGetData(format, out TestData? testData).Should().BeFalse();
        testData.Should().BeNull();
    }

    [WinFormsTheory]
    [MemberData(nameof(UndefinedRestrictedFormat))]
    public void TryGetData_AsConcreteType_Custom_FormattersDisabled_ReturnFalse(string format)
    {
        (DataObject dataObject, TestData _) = SetDataObject(format);

        // Formatters are not supported, HGLOBAL contains NotSupportedException
        // but these formats are not compatible with this type on the clipboard.
        dataObject.TryGetData(format, out TestData? testData).Should().BeFalse();
        testData.Should().BeNull();
    }

    [WinFormsTheory]
    [MemberData(nameof(StringFormat))]
    [MemberData(nameof(BitmapFormat))]
    public void TryGetData_AsConcreteType_Custom_InvalidTypeFormatCombination(string format)
    {
        (DataObject dataObject, TestData _) = SetDataObject(format);

        Action tryGetData = () => dataObject.TryGetData(format, out TestData? _);
        tryGetData.Should().Throw<NotSupportedException>()
            .WithMessage(expectedWildcardPattern: InvalidTypeFormatCombinationMessage);
    }

    [WinFormsTheory]
    [MemberData(nameof(UnboundedFormat))]
    public void TryGetData_WithResolver_AsConcreteType_Custom_FormatterEnabled_Success(string format)
    {
        (DataObject dataObject, TestData value) = SetDataObject(format);

        using BinaryFormatterScope scope = new(enable: true);
        using BinaryFormatterInClipboardDragDropScope clipboardScope = new(enable: true);

        dataObject.TryGetData(format, TestData.Resolver, autoConvert: true, out TestData? testData).Should().BeTrue();
        testData.Should().BeEquivalentTo(value);
    }

    [WinFormsTheory]
    [MemberData(nameof(StringFormat))]
    [MemberData(nameof(BitmapFormat))]
    public void TryGetData_WithResolver_AsConcreteType_Custom_InvalidTypeFormatCombination(string format)
    {
        (DataObject dataObject, TestData _) = SetDataObject(format);
        Action tryGetData = () => dataObject.TryGetData(format, TestData.Resolver, autoConvert: true, out TestData? _);

        tryGetData.Should().Throw<NotSupportedException>()
            .WithMessage(expectedWildcardPattern: InvalidTypeFormatCombinationMessage);
    }

    [WinFormsTheory]
    [MemberData(nameof(StringFormat))]
    [MemberData(nameof(BitmapFormat))]
    public void TryGetData_WithResolver_AsConcreteType_Custom_FormatterEnabled_InvalidTypeFormatCombination(string format)
    {
        (DataObject dataObject, TestData _) = SetDataObject(format);
        Action tryGetData = () => dataObject.TryGetData(format, TestData.Resolver, autoConvert: true, out TestData? _);

        using BinaryFormatterScope scope = new(enable: true);
        using BinaryFormatterInClipboardDragDropScope clipboardScope = new(enable: true);

        tryGetData.Should().Throw<NotSupportedException>()
            .WithMessage(expectedWildcardPattern: InvalidTypeFormatCombinationMessage);
    }

    [WinFormsTheory]
    [MemberData(nameof(UndefinedRestrictedFormat))]
    public void TryGetData_WithResolver_AsConcreteType_Custom_FormatterDisabledException(string format)
    {
        (DataObject dataObject, TestData _) = SetDataObject(format);

        // Formatter is not enabled, HGLOBAL contains NotSupportedException, we can't read it, assume wrong type on the clipboard.
        dataObject.TryGetData(format, TestData.Resolver, autoConvert: true, out TestData? testData).Should().BeFalse();
        testData.Should().BeNull();
    }

    [WinFormsTheory]
    [MemberData(nameof(UndefinedRestrictedFormat))]
    public void TryGetData_AsAbstract_Custom_FormatterEnabled_ReturnFalse(string format)
    {
        (DataObject dataObject, TestData _) = SetDataObject(format);

        using BinaryFormatterScope scope = new(enable: true);
        using BinaryFormatterInClipboardDragDropScope clipboardScope = new(enable: true);

        // Format-type combination is invalid, serialization threw an exception.
        dataObject.TryGetData(format, out AbstractBase? testData).Should().BeFalse();
        testData.Should().BeNull();
    }

    [WinFormsTheory]
    [MemberData(nameof(StringFormat))]
    [MemberData(nameof(BitmapFormat))]
    public void TryGetData_AsAbstract_Custom_InvalidTypeFormatCombination(string format)
    {
        (DataObject dataObject, TestData _) = SetDataObject(format);

        Action tryGetData = () => dataObject.TryGetData(format, out AbstractBase? _);
        tryGetData.Should().Throw<NotSupportedException>()
            .WithMessage(expectedWildcardPattern: InvalidTypeFormatCombinationMessage);
    }

    [WinFormsTheory]
    [MemberData(nameof(UnboundedFormat))]
    public void TryGetData_AsAbstract_Custom_RequiresResolver(string format)
    {
        (DataObject dataObject, TestData _) = SetDataObject(format);
        Action tryGetData = () => dataObject.TryGetData(format, out AbstractBase? _);

        tryGetData.Should().Throw<NotSupportedException>().WithMessage(expectedWildcardPattern: RequiresResolverMessage);

        dataObject.TryGetData(format, out NotSupportedException? ex).Should().BeTrue();
        ex.Should().BeOfType<NotSupportedException>().Which.Message.Should().Be(FormatterDisabledMessage);
    }

    [WinFormsTheory]
    [MemberData(nameof(UnboundedFormat))]
    public void TryGetData_AsAbstract_Custom_FormatterEnabled_RequiresResolver(string format)
    {
        (DataObject dataObject, TestData _) = SetDataObject(format);
        Action tryGetData = () => dataObject.TryGetData(format, out AbstractBase? _);

        using BinaryFormatterScope scope = new(enable: true);
        using BinaryFormatterInClipboardDragDropScope clipboardScope = new(enable: true);

        tryGetData.Should().Throw<NotSupportedException>().WithMessage(expectedWildcardPattern: RequiresResolverMessage);
    }

    [WinFormsTheory]
    [MemberData(nameof(UndefinedRestrictedFormat))]
    public void TryGetData_WithResolver_AsAbstract_Custom_FormatterEnabled_ReturnFalse(string format)
    {
        (DataObject dataObject, TestData _) = SetDataObject(format);

        using BinaryFormatterScope scope = new(enable: true);
        using BinaryFormatterInClipboardDragDropScope clipboardScope = new(enable: true);

        // Format-type combination is validated, but serialization threw an exception
        dataObject.TryGetData(format, TestData.Resolver, autoConvert: true, out AbstractBase? testData).Should().BeFalse();
        testData.Should().BeNull();
    }

    [WinFormsTheory]
    [MemberData(nameof(UnboundedFormat))]
    public void TryGetData_WithResolver_AsAbstract_Custom_FormatterEnabled_Success(string format)
    {
        (DataObject dataObject, TestData value) = SetDataObject(format);

        using BinaryFormatterScope scope = new(enable: true);
        using BinaryFormatterInClipboardDragDropScope clipboardScope = new(enable: true);

        dataObject.TryGetData(format, TestData.Resolver, autoConvert: true, out AbstractBase? testData).Should().BeTrue();
        testData.Should().BeEquivalentTo(value);
    }

    [WinFormsTheory]
    [MemberData(nameof(StringFormat))]
    [MemberData(nameof(BitmapFormat))]
    public void TryGetData_WithResolver_AsAbstract_Custom_InvalidTypeFormatCombination(string format)
    {
        (DataObject dataObject, TestData _) = SetDataObject(format);

        // Nothing is written to HGLOBAL in this test because format-type combination is invalid.
        Action tryGetData = () => dataObject.TryGetData(format, TestData.Resolver, autoConvert: true, out AbstractBase? _);
        tryGetData.Should().Throw<NotSupportedException>()
            .WithMessage(expectedWildcardPattern: InvalidTypeFormatCombinationMessage);
    }

    [WinFormsTheory]
    [MemberData(nameof(UnboundedFormat))]
    public void TryGetData_AsConcrete_NotSerializable_FormatterEnabled_ReturnFalse(string format)
    {
        DataObject native = new();
        NotSerializableData value = new(1);
        native.SetData(format, value);
        DataObject dataObject = new(ComHelpers.GetComPointer<Com.IDataObject>(native));

        using BinaryFormatterScope scope = new(enable: true);
        using BinaryFormatterInClipboardDragDropScope clipboardScope = new(enable: true);

        // E_UNEXPECTED and a NULL HGLOBAL is returned from the COM GetData, we have no stream to deserialize.
        dataObject.TryGetData(format, out NotSerializableData? data).Should().BeFalse();
        data.Should().BeNull();
    }

    // This class does not have [Serializable] attribute, serialization stream will be corrupt.
    private class NotSerializableData
    {
        public NotSerializableData(int value)
        {
            Value = value;
        }

        public int Value;
    }

    [Serializable]
    private class TestData : AbstractBase
    {
        public TestData(Point point)
        {
            Location = point;
            Inner = new InnerData("inner");
        }

        public Point Location;
        public InnerData? Inner;

        public override void DoStuff() { }

        [Serializable]
        internal class InnerData
        {
            public InnerData(string text)
            {
                Text = text;
            }

            public string Text;
        }

        public static Type Resolver(TypeName typeName)
        {
            (string name, Type type)[] allowedTypes =
            [
                (typeof(TestData).FullName!, typeof(TestData)),
                (typeof(InnerData).FullName!, typeof(InnerData)),
                (typeof(AbstractBase).FullName!, typeof(AbstractBase)),
            ];

            string fullName = typeName.FullName;
            foreach (var (name, type) in allowedTypes)
            {
                // Namespace-qualified type name.
                if (name == fullName)
                {
                    return type;
                }
            }

            throw new NotSupportedException($"Can't resolve {fullName}");
        }
    }

    [Serializable]
    internal abstract class AbstractBase
    {
        public abstract void DoStuff();
    }
}
