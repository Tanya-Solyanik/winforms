// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Reflection;

namespace ScratchProject;

// As we can't currently design in VS in the runtime solution, mark as "Default" so this opens in code view by default.
[DesignerCategory("Default")]
public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();

        Action[] writers =
        {
            ClipboardSetDataTestData,
            WriteAsPersistentObject,
            WriteBitmapWithStandardFormat
        };

        Action[] readers =
        {
            ClipboardTryGetDataOfTestData,
            ClipboardTryGetDataOfTestDataByTypeName,
            ClipboardTryGetDataOfTestDataAsPersistentObj,
            ReadFromDataObject,
            ReadOfBitmap
        };

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
        {
            RoundTripCustomFormats,
            RoundTripPenData
        };

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

    #region Readers
    private void ClipboardTryGetDataOfTestData()
    {
        if (Clipboard.TryGetData<TestData>(out var data))
        {
            Text = $"{data._text1} {data._text2}";
        }
        else
        {
            Text = "Could not retrieve data off the clipboard.";
        }

        Clipboard.Clear();
    }

    private void ClipboardTryGetDataOfTestDataByTypeName()
    {
        if (Clipboard.TryGetData<TestData>(typeof(TestData).FullName!, out var data))
        {
            Text = $"{data._text1} {data._text2}";
        }
        else
        {
            Text = "Could not retrieve data off the clipboard.";
        }

        Clipboard.Clear();
    }

    private void ClipboardTryGetDataOfTestDataAsPersistentObj()
    {
        if (Clipboard.TryGetData<TestData>("WindowsForms10PersistentObject", out var data))
        {
            Text = $"{data._text1} {data._text2}";
        }
        else
        {
            Text = "Could not retrieve data off the clipboard.";
        }

        Clipboard.Clear();
    }

    // Get the bitmap from clipboard in another application with targeting 4.8
    private void ReadOfBitmap()
    {
        // if (Clipboard.TryGetData<Bitmap>(out Bitmap data, DataFormats.Bitmap))
        if (Clipboard.TryGetData<Bitmap>(out var data))
        {
            Text = $"got bitmap";
        }
        else
        {
            Text = "Could not retrieve data off the clipboard.";
        }

        Clipboard.Clear();
    }

    private void ReadFromDataObject()
    {
        IDataObject? iData = Clipboard.GetDataObject();
        if (iData?.TryGetData<TestData>(out var data) == true)
        {
            Text = $"{data._text1} {data._text2}";
        }
        else
        {
            Text = "Could not retrieve data off the clipboard.";
        }

        Clipboard.Clear();
    }

    #endregion

    #region Writers
    private void ClipboardSetDataTestData()
    {
        var data = new TestData("Hello", "World");
        // Clipboard.SetData(typeof(TestData).FullName!, new TestData("Hello", "World"));
        Clipboard.SetDataAsJson(data); // this helper method avoids specifying the format
        Text = $"format {typeof(TestData).FullName!}";
    }

    private void WriteAsPersistentObject()
    {
        Clipboard.SetData("WindowsForms10PersistentObject", new TestData("Hello", "World"));
        Text = $"format WindowsForms10PersistentObject";
    }

    private void WriteBitmapWithStandardFormat()
    {
        Clipboard.SetData(
            DataFormats.Bitmap, "D:\\winforms\\src\\System.Windows.Forms\\tests\\UnitTests\\bitmaps\\nature24bits.jpg");
        Text = "Bitmap";
    }

    #endregion

    #region RoundTrips
    public void RoundTripPenData()
    {
        Font f1 = new("Microsoft Sans Serif", 10);
        Clipboard.SetData(DataFormats.PenData, f1);
        object? o = Clipboard.GetData(DataFormats.PenData);
        Font? f = o as Font;
    }

    public void RoundTripCustomFormats()
    {
        string myTextFormat = "MyTextFormat";
        string myBlobFormat = "MyBlobFormat";
        Guid guid = Guid.NewGuid();
        string myText = guid.ToString();
        MemoryStream myBlob = new(guid.ToByteArray());
        string unicodeText = "Euro char: \u20AC";
        DataObject data = new();
        data.SetData(myTextFormat, myText);
        data.SetData(myBlobFormat, myBlob);
        data.SetText(unicodeText);
        Clipboard.SetDataObject(data);
        IDataObject? copiedDataObject = Clipboard.GetDataObject();
        string? copiedText = copiedDataObject?.GetData(myTextFormat) as string;
        MemoryStream? copiedBlob = copiedDataObject?.GetData(myBlobFormat) as MemoryStream;
        string? copiedUnicodeText = Clipboard.GetText();
        // Assert.NotNull(copiedText);
        // Assert.Equal(copiedText, myText);
        // Assert.NotNull(copiedBlob);
        // Assert.Equal(copiedBlob.Length, myBlob.Length);
        // Assert.Equal(new Guid(copiedBlob.ToArray()), guid);
        // Assert.NotNull(copiedUnicodeText);
        // Assert.Equal(copiedUnicodeText, unicodeText);
    }
    #endregion
}
