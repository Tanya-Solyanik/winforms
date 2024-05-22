// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace ScratchProject;

// As we can't currently design in VS in the runtime solution, mark as "Default" so this opens in code view by default.
[DesignerCategory("Default")]
public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
        Button button = new()
        {
            Text = "Write",
            Location = new Point(10, 10),
            Size = new Size(100, 50)
        };

        button.Click += (sender, e) => Write();
        Controls.Add(button);

        button = new()
        {
            Text = "Read",
            Location = new Point(10, 70),
            Size = new Size(100, 50)
        };

        button.Click += (sender, e) => Read();
        Controls.Add(button);
    }

    private void Read()
    {
        IDataObject? iData = Clipboard.GetDataObject();
        if (iData?.GetDataPresent(DataFormats.Text) == true)
        {
            Text = (string?)iData.GetData(DataFormats.Text);
        }
        else  if (iData?.GetDataPresent(typeof(TestData).FullName!) == true)
        {
            TestData? data = iData.GetData<TestData>(typeof(TestData).FullName!);
            if (data is not null)
            {
                Text = $"{data._text1} {data._text2}";
            }
        }
        else
        {
            Text = "Could not retrieve data off the clipboard.";
        }
    }

    private void Write()
    {
        // Clipboard.SetDataObject<TestData>(new TestData("Hello", "World"));
        Clipboard.SetData(typeof(TestData).FullName!, new TestData("Hello", "World"));
    }

    public void Test()
    {
        Font f1 = new("Microsoft Sans Serif", 10);
        Clipboard.SetData(DataFormats.PenData, f1);
        object? o = Clipboard.GetData(DataFormats.PenData);
        Font? f = o as Font;
    }

    private void Set_Click(object sender, EventArgs e)
    {
        Clipboard.SetData(DataFormats.Bitmap, "C:\\Work\\photo1.jpg");
    }

    // Get the bitmap from clipboard in another application with targeting 4.8

    private void Get_Click(object sender, EventArgs e)
    {
        object? result = Clipboard.GetData(DataFormats.Bitmap);
    }

    public void DataObject_CustomFormats()
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
}

[Serializable]
internal class TestData
{
    public TestData(string text1, string text2)
    {
        _text1 = text1;
        _text2 = text2;
    }

    public string _text1;
    public string _text2;
}
