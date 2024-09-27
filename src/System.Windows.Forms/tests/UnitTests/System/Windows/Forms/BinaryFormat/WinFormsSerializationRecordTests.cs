// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Collections;
using System.Drawing;
using System.Formats.Nrbf;
using System.Windows.Forms.Nrbf;

namespace System.Private.Windows.Core.BinaryFormat.Tests;

public class WinFormsSerializationRecordTests
{
    private static void RoundTripCommonObject<T>(T data)
    {
        SerializationRecord record = data.SerializeAndDecode();
        record.TryGetCommonObject<T>(out object? value).Should().BeTrue();
        value.Should().Be(data);
    }

    private static void RoundTripEquivalentCommonObject<T>(T data)
    {
        SerializationRecord record = data.SerializeAndDecode();
        record.TryGetCommonObject<T>(out object? value).Should().BeTrue();
        value.Should().BeEquivalentTo(data);
    }

    [Fact]
    public void TryGetCommonObject_AsObject() =>
        RoundTripEquivalentCommonObject<object>(new List<int> { 1, 2, 3 });

    [Fact]
    public void TryGetCommonObject_AsObject_Nullable() =>
        RoundTripEquivalentCommonObject<object?>(new List<int> { 1, 2, 3 });

    [Fact]
    public void TryGetCommonObject_string() =>
        RoundTripCommonObject("text");

    [Fact]
    public void TryGetCommonObject_int() =>
        RoundTripCommonObject(int.MaxValue);

    [Fact]
    public void TryGetCommonObject_uint() =>
        RoundTripCommonObject(uint.MaxValue);

    [Fact]
    public void TryGetCommonObject_long() =>
        RoundTripCommonObject(long.MaxValue);

    [Fact]
    public void TryGetCommonObject_ulong() =>
        RoundTripCommonObject(ulong.MaxValue);

    [Fact]
    public void TryGetCommonObject_short() =>
        RoundTripCommonObject(short.MaxValue);

    [Fact]
    public void TryGetCommonObject_ushort() =>
        RoundTripCommonObject(ushort.MaxValue);

    [Fact]
    public void TryGetCommonObject_sbyte() =>
        RoundTripCommonObject(sbyte.MaxValue);

    [Fact]
    public void TryGetCommonObject_byte() =>
        RoundTripCommonObject(byte.MaxValue);

    [Fact]
    public void TryGetCommonObject_bool() =>
        RoundTripCommonObject(true);

    [Fact]
    public void TryGetCommonObject_float() =>
        RoundTripCommonObject(float.MaxValue);

    [Fact]
    public void TryGetCommonObject_double() =>
        RoundTripCommonObject(double.MaxValue);

    [Fact]
    public void TryGetCommonObject_char() =>
        RoundTripCommonObject(char.MaxValue);

    [Fact]
    public void TryGetCommonObject_TimeSpan() =>
        RoundTripCommonObject(TimeSpan.MaxValue);

    [Fact]
    public void TryGetCommonObject_DateTime() =>
        RoundTripCommonObject(DateTime.MaxValue);

    [Fact]
    public void TryGetCommonObject_decimal() =>
        RoundTripCommonObject(decimal.MaxValue);

    [Fact]
    public void TryGetCommonObject_nint() =>
        RoundTripCommonObject((nint)(-1));

    [Fact]
    public void TryGetCommonObject_nuint() =>
        RoundTripCommonObject((nuint)1);

    [Fact]
    public void TryGetCommonObject_Point() =>
        RoundTripCommonObject(new Point(1, 1));

    [Fact]
    public void TryGetCommonObject_PointF() =>
        RoundTripCommonObject(new PointF(1.1f, 1.1f));

    [Fact]
    public void TryGetCommonObject_Size() =>
        RoundTripCommonObject(new Size(1, 1));

    [Fact]
    public void TryGetCommonObject_SizeF() =>
        RoundTripCommonObject(new SizeF(1.1f, 1.1f));

    [Fact]
    public void TryGetCommonObject_Rectangle() =>
        RoundTripCommonObject(new Rectangle(1, 2, 3, 4));

    [Fact]
    public void TryGetCommonObject_RectangleF() =>
        RoundTripCommonObject(new RectangleF(1.1f, 2.1f, 3.1f, 4.1f));

    [Fact]
    public void TryGetCommonObject_Color() =>
        RoundTripCommonObject(new Color());

    [Fact]
    public void TryGetCommonObject_Bitmap()
    {
        using Bitmap data = new(1, 1);
        SerializationRecord record = data.SerializeAndDecode();
        record.TryGetCommonObject<Bitmap>(out object? value).Should().BeTrue();
        value.Should().BeOfType<Bitmap>().Which.Size.Should().Be(new Size(1, 1));
    }

    [Fact]
    public void TryGetCommonObject_ImageListStreamer()
    {
        using ImageList sourceList = new();
        using Bitmap image = new(1, 1);
        sourceList.Images.Add(image);
        using ImageListStreamer data = sourceList.ImageStream!;

        SerializationRecord record = data.SerializeAndDecode();
        record.TryGetCommonObject<ImageListStreamer>(out object? value).Should().BeTrue();

        using ImageList newList = new();
        newList.ImageStream = value.Should().BeOfType<ImageListStreamer>().Subject;
        newList.Images.Count.Should().Be(1);
        Bitmap newImage = (Bitmap)newList.Images[0];
        newImage.Size.Should().Be(sourceList.Images[0].Size);
    }

    [Fact]
    public void TryGetCommonObject_NotSupportedException() =>
        RoundTripEquivalentCommonObject(new NotSupportedException());

    [Fact]
    public void TryGetCommonObject_List_Of_int() =>
        RoundTripEquivalentCommonObject(new List<int> { 1, 2, 3 });

    [Fact]
    public void TryGetCommonObject_List_Of_int_ReadAsWrongType()
    {
        List<int> data = [1, 2, 3];
        SerializationRecord record = data.SerializeAndDecode();

        record.TryGetCommonObject<List<(int, int)>>(out object? value).Should().BeFalse();
        value.Should().BeNull();

        record.TryGetCommonObject<List<string>>(out value).Should().BeFalse();
        value.Should().BeNull();

        record.TryGetCommonObject<IList>(out value).Should().BeFalse();
        value.Should().BeNull();

        record.TryGetCommonObject<ArrayList>(out value).Should().BeFalse();
        value.Should().BeNull();

        record.TryGetCommonObject<List<uint>>(out value).Should().BeFalse();
        value.Should().BeNull();
    }

    [Fact]
    public void TryGetCommonObject_Mismatch()
    {
        FileNotFoundException data = new();
        SerializationRecord record = data.SerializeAndDecode();
        record.TryGetCommonObject<Control>(out object? value).Should().BeFalse();
        value.Should().BeNull();
    }

    [Fact]
    public void TryGetCommonObject_Object()
    {
        object @object = new();
        SerializationRecord record = @object.SerializeAndDecode();
        record.TryGetCommonObject<object>(out object? value).Should().BeFalse();
        value.Should().BeNull();
    }

    [Fact]
    public void TryGetCommonObject_Array_Int() =>
        RoundTripEquivalentCommonObject(new int[] { 1, 2, 3 });

    [Fact]
    public void TryGetCommonObject_Array_String() =>
        RoundTripEquivalentCommonObject(new string?[] { "thing1", "thing2", null });

    [Fact]
    public void TryGetCommonObject_Array_ReadAsWrongType()
    {
        string[] data = ["1", "2", "3"];
        SerializationRecord record = data.SerializeAndDecode();

        record.TryGetCommonObject<List<string>>(out object? value).Should().BeFalse();
        value.Should().BeNull();

        record.TryGetCommonObject<string[][]>(out value).Should().BeFalse();
        value.Should().BeNull();

        record.TryGetCommonObject<int[]>(out value).Should().BeFalse();
        value.Should().BeNull();

        record.TryGetCommonObject<Control[]>(out value).Should().BeFalse();
        value.Should().BeNull();

        record.TryGetCommonObject<DayOfWeek>(out value).Should().BeFalse();
        value.Should().BeNull();
    }

    [Fact]
    public void TryGetCommonObject_Array_NotPrimitive()
    {
        Point[] data = [new Point(1, 2)];

        SerializationRecord record = data.SerializeAndDecode();
        record.TryGetCommonObject<Point[]>(out object? value).Should().BeFalse();
        value.Should().BeNull();
    }

    [Fact]
    public void TryGetCommonObject_string_Nullable() =>
        RoundTripCommonObject<string?>("text");

    [Fact]
    public void TryGetCommonObject_int_Nullable() =>
        RoundTripCommonObject<int?>(int.MaxValue);

    [Fact]
    public void TryGetCommonObject_uint_Nullable() =>
        RoundTripCommonObject<uint?>(uint.MaxValue);

    [Fact]
    public void TryGetCommonObject_long_Nullable() =>
        RoundTripCommonObject<long?>(long.MaxValue);

    [Fact]
    public void TryGetCommonObject_ulong_Nullable() =>
        RoundTripCommonObject<ulong?>(ulong.MaxValue);

    [Fact]
    public void TryGetCommonObject_short_Nullable() =>
        RoundTripCommonObject<short?>(short.MaxValue);

    [Fact]
    public void TryGetCommonObject_ushort_Nullable() =>
        RoundTripCommonObject<ushort?>(ushort.MaxValue);

    [Fact]
    public void TryGetCommonObject_sbyte_Nullable() =>
        RoundTripCommonObject<sbyte?>(sbyte.MaxValue);

    [Fact]
    public void TryGetCommonObject_byte_Nullable() =>
        RoundTripCommonObject<byte?>(byte.MaxValue);

    [Fact]
    public void TryGetCommonObject_bool_Nullable() =>
        RoundTripCommonObject<bool?>(false);

    [Fact]
    public void TryGetCommonObject_float_Nullable() =>
        RoundTripCommonObject<float?>(float.MaxValue);

    [Fact]
    public void TryGetCommonObject_double_Nullable() =>
        RoundTripCommonObject<double?>(double.MaxValue);

    [Fact]
    public void TryGetCommonObject_char_Nullable() =>
        RoundTripCommonObject<char?>('1');

    [Fact]
    public void TryGetCommonObject_TimeSpan_Nullable() =>
        RoundTripCommonObject<TimeSpan?>(TimeSpan.MaxValue);

    [Fact]
    public void TryGetCommonObject_DateTime_Nullable() =>
        RoundTripCommonObject<DateTime?>(DateTime.MaxValue);

    [Fact]
    public void TryGetCommonObject_decimal_Nullable() =>
        RoundTripCommonObject<decimal?>(decimal.MaxValue);

    [Fact]
    public void TryGetCommonObject_Point_Nullable() =>
        RoundTripCommonObject<Point?>(new Point(1, 1));

    [Fact]
    public void TryGetCommonObject_PointF_Nullable() =>
        RoundTripCommonObject<PointF?>(new PointF(1.1f, 1.1f));

    [Fact]
    public void TryGetCommonObject_Size_Nullable() =>
        RoundTripCommonObject<Size?>(new Size(1, 1));

    [Fact]
    public void TryGetCommonObject_SizeF_Nullable() =>
        RoundTripCommonObject<SizeF?>(new SizeF(1.1f, 1.1f));

    [Fact]
    public void TryGetCommonObject_Rectangle_Nullable() =>
        RoundTripCommonObject<Rectangle?>(new Rectangle(1, 2, 3, 4));

    [Fact]
    public void TryGetCommonObject_RectangleF_Nullable() =>
        RoundTripCommonObject<RectangleF?>(new RectangleF(1.1f, 2.1f, 3.1f, 4.1f));

    [Fact]
    public void TryGetCommonObject_Color_Nullable() =>
        RoundTripCommonObject<Color?>(new Color());

    [Fact]
    public void TryGetCommonObject_Bitmap_Nullable()
    {
        using Bitmap data = new(1, 1);
        SerializationRecord record = data.SerializeAndDecode();
        record.TryGetCommonObject<Bitmap?>(out object? value).Should().BeTrue();
        value.Should().BeOfType<Bitmap>().Which.Size.Should().Be(new Size(1, 1));
    }

    [Fact]
    public void TryGetCommonObject_ImageListStreamer_Nullable()
    {
        using ImageList sourceList = new();
        using Bitmap image = new(1, 1);
        sourceList.Images.Add(image);
        using ImageListStreamer data = sourceList.ImageStream!;

        SerializationRecord record = data.SerializeAndDecode();
        record.TryGetCommonObject<ImageListStreamer?>(out object? value).Should().BeTrue();

        using ImageList newList = new();
        newList.ImageStream = value.Should().BeOfType<ImageListStreamer>().Subject;
        newList.Images.Count.Should().Be(1);
        Bitmap newImage = (Bitmap)newList.Images[0];
        newImage.Size.Should().Be(sourceList.Images[0].Size);
    }

    [Fact]
    public void TryGetCommonObject_NotSupportedException_Nullable() =>
        RoundTripEquivalentCommonObject<NotSupportedException?>(new NotSupportedException());

    [Fact]
    public void TryGetCommonObject_List_Of_int_Nullable() =>
        RoundTripEquivalentCommonObject<List<int>?>(new List<int> { 1, 2, 3 });

    [Fact]
    public void TryGetCommonObject_Array_Of_int_Nullable() =>
         RoundTripEquivalentCommonObject<int[]?>([1, 2, 3]);

    [Fact]
    public void TryGetCommonObject_ArrayList_Fail()
    {
        ArrayList arrayList = new ArrayList { 1, "string", true };

        SerializationRecord record = arrayList.SerializeAndDecode();
        record.TryGetCommonObject<ArrayList>(out object? value).Should().BeFalse();
        value.Should().BeNull();
    }

    [Fact]
    public void TryGetCommonObject_Hashtable_Fail()
    {
        Hashtable hashtable = new Hashtable
        {
            { "key1", 1 },
            { "key2", "value2" },
            { "key3", true }
        };

        SerializationRecord record = hashtable.SerializeAndDecode();
        record.TryGetCommonObject<Hashtable>(out object? value).Should().BeFalse();
        value.Should().BeNull();
    }
}
