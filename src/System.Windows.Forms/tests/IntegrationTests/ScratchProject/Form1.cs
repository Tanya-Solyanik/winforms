// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;

namespace ScratchProject;

// As we can't currently design in VS in the runtime solution, mark as "Default" so this opens in code view by default.
[DesignerCategory("Default")]
public partial class Form1 : Form
{
#pragma warning disable WFDEV005

    public Form1()
    {
        InitializeComponent();
        string f = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        Action[] writers =
        [
            ClipboardSetData
        ];

        Action[] readers =
        [
            ClipboardGetData
        ];

        int x = 40;
        int y = 10;
        Button button;
        foreach (var write in writers)
        {
            button = new()
            {
                Text = write.GetMethodInfo().Name,
                Location = new Point(x, y),
                Size = new Size(160, 50)
            };

            button.Click += (sender, e) => write();
            Controls.Add(button);

            y += 60;
        }

        x += 200;
        y = 10;
        foreach (var read in readers)
        {
            button = new()
            {
                Text = read.GetMethodInfo().Name,
                Location = new Point(x, y),
                Size = new Size(160, 50)
            };

            button.Click += (sender, e) => read();
            Controls.Add(button);

            y += 60;
        }

        Action[] roundtrips =
        [
            RoundTripCustomFormat_GenericList,
            RoundTripCustomFormat_Known,
            RoundTripCustomFormat_Primitive,
            RoundTripCustomFormat_OffsetArray,
            RoundTripCustomFormat_String,
            RoundTripCustomFormat_Array,
            RoundTripPenData
        ];

        x += 200;
        y = 10;
        foreach (var roundtrip in roundtrips)
        {
            button = new()
            {
                Text = roundtrip.GetMethodInfo().Name,
                Location = new Point(x, y),
                Size = new Size(160, 50)
            };

            button.Click += (sender, e) => roundtrip();
            Controls.Add(button);

            y += 60;
        }
    }

    private void ClipboardSetData()
    {
        // DayOfWeek data = DayOfWeek.Friday;
        // Clipboard.SetData(DataFormats.Serializable, data);
        // Text = typeof(DayOfWeek).AssemblyQualifiedName;

        // int[] data = { 1, 2, 3, 4, 5 };
        // Clipboard.SetData("Test format", data);
        // Text = "int[]";

        // Clipboard.SetData("Test format", new BinaryFormatReadableData());
        // Text = "BinaryFormatReadableData";

        // Clipboard.SetData(DataFormats.Text, "Hello World");
        // Text = "Hello world!";

        // Clipboard.SetData(
        //    DataFormats.Bitmap, new Bitmap("D:\\winforms\\src\\System.Windows.Forms\\tests\\UnitTests\\bitmaps\\nature24bits.jpg"));
        // Text = typeof(Bitmap).AssemblyQualifiedName;

        Clipboard.SetData(DataFormats.Serializable, Color.AliceBlue);
        Text = typeof(Color).AssemblyQualifiedName;
    }

    private void ClipboardGetData()
    {
        // object? data = Clipboard.GetData(DataFormats.Bitmap);
        // if (data is not null)
        // if (Clipboard.TryGetData<Bitmap>(DataFormats.Bitmap, out Bitmap? data))
        // if (Clipboard.TryGetData<string>(DataFormats.Text, out string? data))
        // if (Clipboard.TryGetData<BinaryFormatReadableData>("Test format", out BinaryFormatReadableData? data))
        // if (Clipboard.TryGetData("Test format", out int[]? data))
        // if (Clipboard.TryGetData(DataFormats.Serializable, out DayOfWeek? data))
        if (Clipboard.TryGetData(DataFormats.Serializable, out Color data))
        {
            Text = $"got {data.GetType().AssemblyQualifiedName}";
        }
        else
        {
            Text = "Could not retrieve data off the clipboard.";
        }

        Clipboard.Clear();
    }

    // Get the bitmap from clipboard in another application with targeting 4.8

    #region RoundTrips
    public void RoundTripPenData()
    {
        Font f1 = new("Microsoft Sans Serif", 10);
        Clipboard.SetData(DataFormats.PenData, f1);
        object? o = Clipboard.GetData(DataFormats.PenData);
        if (o is Font f)
        {
            Text = f.Name;
        }
        else
        {
            Text = "null";
        }
    }

    // private static Type WaveAudioResolver(TypeName typeName)
    // {
    //    Type[] knownTypes = [typeof(MemoryStream), typeof(Stream)];
    //    foreach (Type type in knownTypes)
    //    {
    //        TypeName parsed = TypeName.Parse($"{type.FullName}, {type.Assembly.FullName}");
    //        if (typeName.FullName == parsed.FullName && typeName.AssemblyName?.Name == parsed.AssemblyName?.Name)
    //        {
    //            return type;
    //        }
    //    }
    //    throw new NotSupportedException();
    // }

    public static void RoundTripCustomFormat_String()
    {
        Clipboard.SetData("MyFormat", "MyData");
        object? value = Clipboard.GetData("MyFormat");
    }

    public static void RoundTripCustomFormat_Array()
    {
        int[][] jagged =
        [
            [1, 2, 3, 4],
            [5, 6, 7, 8, 9],
            [10, 11, 12],
        ];
        Clipboard.SetData("MyFormat", jagged);
        object? value = Clipboard.GetData("MyFormat"); // returns MemoryStream

        Bitmap[] bitmaps =
        [
            new("D:\\winforms\\src\\System.Windows.Forms\\tests\\UnitTests\\bitmaps\\nature24bits.jpg"),
            new("D:\\winforms\\src\\System.Windows.Forms\\tests\\UnitTests\\bitmaps\\nature24bits.jpg")
        ];
        Clipboard.SetData("MyFormat", bitmaps);
        value = Clipboard.GetData("MyFormat");

        int[] simple = [101, 202, 303];
        Clipboard.SetData("MyFormat", simple);
        value = Clipboard.GetData("MyFormat");

        if (simple.GetType().IsAssignableTo(typeof(long[])))
        {
            Debug.WriteLine($"{simple.GetType().Name} is an Array");
        }

        int?[] nullableElements = [101, 202, 303];
        Clipboard.SetData("MyFormat", nullableElements);
        value = Clipboard.GetData("MyFormat");

        int[,] multidimensional = new int[3, 2]
        {
            {1,2},
            {2,3},
            {4,5}
        };
        Clipboard.SetData("MyFormat", multidimensional);
        value = Clipboard.GetData("MyFormat");

        int?[,] multidimensionalNullable = new int?[3, 2]
        {
            {1,2},
            {2,3},
            {4,5}
        };
        Clipboard.SetData("MyFormat", multidimensionalNullable);
        value = Clipboard.GetData("MyFormat");

        int?[][] jaggedNullable =
        [
            [1, 2, 3, 4],
            [5, 6, 7, 8, 9],
            [10, 11, 12],
        ];
        Clipboard.SetData("MyFormat", jaggedNullable);
        value = Clipboard.GetData("MyFormat");

        object[] objects = [jagged, bitmaps, simple, nullableElements, multidimensional, multidimensionalNullable, jaggedNullable];
        foreach (object obj in objects)
        {
            Debug.WriteLine(obj.GetType().AssemblyQualifiedName);
            if (obj.GetType().IsAssignableTo(typeof(Array)))
            {
                Debug.WriteLine($"{obj.GetType().Name} is an Array");
            }
        }
    }

    public static void RoundTripCustomFormat_OffsetArray()
    {
        // does not work with either BFO or BF, return a null
        Array array = Array.CreateInstance(typeof(uint[]), [5], [1]);

        Clipboard.SetData("MyFormat", array);
        object? value = Clipboard.GetData("MyFormat");
    }

    public static void RoundTripCustomFormat_Primitive()
    {
        // these were resolved through TryGetObject
        Clipboard.SetData("MyFormat", true);
        object? value = Clipboard.GetData("MyFormat");
        Clipboard.Clear();

        Clipboard.SetData("MyFormat", 2);
        value = Clipboard.GetData("MyFormat");
        Clipboard.Clear();

        byte data = 2;
        Clipboard.SetData("MyFormat", data);
        value = Clipboard.GetData("MyFormat");
        Clipboard.Clear();

        sbyte data_1 = 2;
        Clipboard.SetData("MyFormat", data_1);
        value = Clipboard.GetData("MyFormat");
        Clipboard.Clear();

        short data_2 = 2;
        Clipboard.SetData("MyFormat", data_2);
        value = Clipboard.GetData("MyFormat");
        Clipboard.Clear();

        ushort data_3 = 2;
        Clipboard.SetData("MyFormat", data_3);
        value = Clipboard.GetData("MyFormat");
        Clipboard.Clear();

        uint data_4 = 2;
        Clipboard.SetData("MyFormat", data_4);
        value = Clipboard.GetData("MyFormat");
        Clipboard.Clear();

        Clipboard.SetData("MyFormat", 2L);
        value = Clipboard.GetData("MyFormat");
        Clipboard.Clear();

        ulong data_5 = 2;
        Clipboard.SetData("MyFormat", data_5);
        value = Clipboard.GetData("MyFormat");
        Clipboard.Clear();

        IntPtr data_6 = new(2);
        Clipboard.SetData("MyFormat", data_6);
        value = Clipboard.GetData("MyFormat");
        Clipboard.Clear();

        UIntPtr data_7 = new(2);
        Clipboard.SetData("MyFormat", data_7);
        value = Clipboard.GetData("MyFormat");
        Clipboard.Clear();

        char data_8 = 'c';
        Clipboard.SetData("MyFormat", data_8);
        value = Clipboard.GetData("MyFormat");
        Clipboard.Clear();

        double data_9 = 2.2;
        Clipboard.SetData("MyFormat", data_9);
        value = Clipboard.GetData("MyFormat");
        Clipboard.Clear();

        float data_10 = 2.2f;
        Clipboard.SetData("MyFormat", data_10);
        value = Clipboard.GetData("MyFormat");
        Clipboard.Clear();
    }

    public static void RoundTripCustomFormat_GenericList()
    {
        List<int> list = new() { 1, 2, 3, 4, 5 };
        ListHelper(list);

        List<uint> list_15 = new() { 1, 2, 3, 4, 5 };
        ListHelper(list_15);

        List<string> list_1 = new() { "1", "2", "3", "4", "5" };
        ListHelper(list_1);

        List<byte> list_2 = new() { 1, 2, 3, 4, 5 };
        ListHelper(list_2);

        List<sbyte> list_14 = new() { 1, 2, 3, 4, 5 };
        ListHelper(list_14);

        List<short> list_3 = new() { 1, 2, 3, 4, 5 };
        ListHelper(list_3);

        List<long> list_4 = new() { 1, 2, 3, 4, 5 };
        ListHelper(list_4);

        List<float> list_5 = new() { 1.1f, 2.2f, 3.3f, 4.4f, 5.5f };
        ListHelper(list_5);

        List<ushort> list_6 = new() { 1, 2, 3, 4, 5 };
        ListHelper(list_6);

        List<ulong> list_7 = new() { 1, 2, 3, 4, 5 };
        ListHelper(list_7);

        List<double> list_8 = new() { 1.1, 2.2, 3.3, 4.4, 5.5 };
        ListHelper(list_8);

        List<decimal> list_9 = new() { 1.1m, 2.2m, 3.3m, 4.4m, 5.5m };
        ListHelper(list_9);

        List<char> list_10 = new() { 'a', 's', 'd', 'f', 'g' };
        ListHelper(list_10);

        List<bool> list_11 = new() { true, false, true, false, true };
        ListHelper(list_11);

        List<DateTime> list_12 = new() { new(2021, 10, 10), new(2021, 10, 11), new(2021, 10, 12) };
        ListHelper(list_12);

        List<TimeSpan> list_13 = new() { TimeSpan.FromHours(1), TimeSpan.FromHours(2), TimeSpan.FromHours(3) };
        ListHelper(list_13);
    }

    private static void ListHelper<T>(List<T> list)
    {
        Clipboard.SetData("MyFormat", list);
        object? value = Clipboard.GetData("MyFormat");
        // Clipboard.Clear();
    }

    public static void RoundTripCustomFormat_Known()
    {
        Bitmap data = new("D:\\winforms\\src\\System.Windows.Forms\\tests\\UnitTests\\bitmaps\\nature24bits.jpg");
        Clipboard.SetData("MyFormat", data);
        object? value = Clipboard.GetData("MyFormat");
        Clipboard.Clear();

        Clipboard.SetData("MyFormat", "text");
        value = Clipboard.GetData("MyFormat");
        Clipboard.Clear();

        int[][] array = [[1, 2], [3, 4]];
        Clipboard.SetData("MyFormat", array);
        value = Clipboard.GetData("MyFormat");
        Clipboard.Clear();

        decimal dec = 2.2m;
        Clipboard.SetData("MyFormat", dec);
        value = Clipboard.GetData("MyFormat");
        Clipboard.Clear();

        DateTime date = new(2021, 10, 10);
        Clipboard.SetData("MyFormat", date);
        value = Clipboard.GetData("MyFormat");
        Clipboard.Clear();

        TimeSpan time = TimeSpan.FromHours(2);
        Clipboard.SetData("MyFormat", time);
        value = Clipboard.GetData("MyFormat");
        Clipboard.Clear();

        PointF pointF = new(1, 2);
        Clipboard.SetData("MyFormat", pointF);
        value = Clipboard.GetData("MyFormat");
        Clipboard.Clear();

        RectangleF rectangle = new(1, 2, 3, 4);
        Clipboard.SetData("MyFormat", rectangle);
        value = Clipboard.GetData("MyFormat");
        Clipboard.Clear();

        Hashtable table = new()
        {
            { "ID1", "Value1" },
            { "ID2", "Value2" },
            { "ID3", "Value3" }
        };
        Clipboard.SetData("MyFormat", table);
        value = Clipboard.GetData("MyFormat");
        Clipboard.Clear();

        ArrayList arrayList = new()
        {
            1, "Two", 3.0
        };
        Clipboard.SetData("MyFormat", arrayList);
        value = Clipboard.GetData("MyFormat");
        Clipboard.Clear();

        // NotSupportedException
    }

    // public void RoundTripCustomFormats()
    // {
    // byte[] audioBytes = [1, 2, 3];
    // Clipboard.SetAudio(audioBytes);

    // bool result = Clipboard.TryGetData(DataFormats.WaveAudio, WaveAudioResolver, out Stream? unboundedData);
    // Text = result.ToString();

    // string myTextFormat = "MyTextFormat";
    // string myBlobFormat = "MyBlobFormat";
    // Guid guid = Guid.NewGuid();
    // string myText = guid.ToString();
    // MemoryStream myBlob = new(guid.ToByteArray());
    // string unicodeText = "Euro char: \u20AC";
    // DataObject data = new();
    // data.SetData(myTextFormat, myText);
    // data.SetData(myBlobFormat, myBlob);
    // data.SetText(unicodeText);
    // Clipboard.SetDataObject(data);
    // IDataObject? copiedDataObject = Clipboard.GetDataObject();
    // string? copiedText = copiedDataObject?.GetData(myTextFormat) as string;
    // MemoryStream? copiedBlob = copiedDataObject?.GetData(myBlobFormat) as MemoryStream;
    // string? copiedUnicodeText = Clipboard.GetText();
    // Assert.NotNull(copiedText);
    // Assert.Equal(copiedText, myText);
    // Assert.NotNull(copiedBlob);
    // Assert.Equal(copiedBlob.Length, myBlob.Length);
    // Assert.Equal(new Guid(copiedBlob.ToArray()), guid);
    // Assert.NotNull(copiedUnicodeText);
    // Assert.Equal(copiedUnicodeText, unicodeText);
    // }
    #endregion
}

[Serializable]
internal sealed class TestData
{
    public TestData(string text1, string text2)
    {
        _text1 = text1;
        _text2 = text2;
    }

    public string _text1;
    public string _text2;
}

[Serializable]
internal sealed class TestData1
{
    public TestData1(PointF point)
    {
        _point = point;
    }

    public PointF _point;
}

[Serializable]
internal sealed class TestData2
{
    public TestData2(ImageListStreamer streamer)
    {
        _streamer = streamer;
    }

    public ImageListStreamer _streamer;
}

[Serializable]
internal sealed class TestData3
{
    public TestData3(Bitmap bitmap)
    {
        _bitmap = bitmap;
    }

    public Bitmap _bitmap;

    internal class InnerData
    {
        public InnerData(string text)
        {
            _text = text;
        }

        public string _text;
    }
}

[Serializable]
public sealed class TestData4 : ISerializable
{
    private readonly int _n1;

    public TestData4()
    {
        _n1 = -1;
    }

    private TestData4(SerializationInfo info, StreamingContext context)
    {
        _n1 = (info.GetValue(nameof(_n1), typeof(int)) is object)
            ? (int)info.GetValue(nameof(_n1), typeof(int))!
            : -1;
    }

    // The following method serializes the instance.
    void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
    {
        info.AddValue(nameof(_n1), _n1);
    }
}

internal class BinaryFormatReadableData : IEquatable<BinaryFormatReadableData>, IDisposable
{
    public BinaryFormatReadableData()
    {
        _point = new PointF(1.2F, 1.2F);
        // _bitmap = new(10, 10);
        // _imageList = new();
        _id = ++s_counter;
    }

    // public Bitmap _bitmap;
    public PointF _point;
    // private readonly ImageList _imageList;
    private readonly int _id;
    private static int s_counter;

    public bool Equals(BinaryFormatReadableData? other)
        => other is not null && _point == other._point /* && _bitmap.Size == other._bitmap.Size */ && _id == other._id;
    public override bool Equals(object? obj) => obj is BinaryFormatReadableData other && Equals(other);
    public override int GetHashCode() => _point.GetHashCode();
    public void Dispose()
    {
        // _bitmap?.Dispose();
        // _imageList?.Dispose();
    }
}

internal class MyException : NotSupportedException
{
    public MyException(string message) : base(message) { }
}

