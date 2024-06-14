// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Drawing;
using System.Windows.Forms;
using FluentAssertions;
using Microsoft.VisualBasic.Devices;
using DataFormats = System.Windows.Forms.DataFormats;
using TextDataFormat = System.Windows.Forms.TextDataFormat;

namespace Microsoft.VisualBasic.MyServices.Tests;

[Collection("Sequential")]
public class ClipboardProxyTests
{
    [WinFormsFact]
    public void Clear()
    {
        var clipboard = new Computer().Clipboard;
        string text = GetUniqueText();
        clipboard.SetText(text);
        Assert.True(Clipboard.ContainsText());
        clipboard.Clear();
        Assert.False(Clipboard.ContainsText());
    }

    [WinFormsFact]
    public void Text()
    {
        var clipboard = new Computer().Clipboard;
        string text = GetUniqueText();
        clipboard.SetText(text, TextDataFormat.UnicodeText);
        Assert.Equal(Clipboard.ContainsText(), clipboard.ContainsText());
        Assert.Equal(Clipboard.GetText(), clipboard.GetText());
        Assert.Equal(Clipboard.GetText(TextDataFormat.UnicodeText), clipboard.GetText(TextDataFormat.UnicodeText));
        Assert.Equal(text, clipboard.GetText(TextDataFormat.UnicodeText));
    }

    [WinFormsFact]
    public void Image()
    {
        var clipboard = new Computer().Clipboard;
        using Bitmap image = new(2, 2);
        Assert.Equal(Clipboard.ContainsImage(), clipboard.ContainsImage());
        Assert.Equal(Clipboard.GetImage(), clipboard.GetImage());
        clipboard.SetImage(image);
    }

    [WinFormsFact]
    public void Audio()
    {
        var clipboard = new Computer().Clipboard;
        Assert.Equal(Clipboard.ContainsAudio(), clipboard.ContainsAudio());
        // Not tested:
        //   Public Function GetAudioStream() As Stream
        //   Public Sub SetAudio(ByVal audioBytes As Byte())
        //   Public Sub SetAudio(ByVal audioStream As Stream)
    }

    [WinFormsFact]
    public void FileDropList()
    {
        var clipboard = new Computer().Clipboard;
        Assert.Equal(Clipboard.ContainsFileDropList(), clipboard.ContainsFileDropList());
        // Not tested:
        //   Public Function GetFileDropList() As StringCollection
        //   Public Sub SetFileDropList(ByVal filePaths As StringCollection)
    }

    [WinFormsFact]
    public void Data()
    {
        var clipboard = new Computer().Clipboard;
        string data = GetUniqueText();
        clipboard.SetData(DataFormats.UnicodeText, data);
        Assert.Equal(Clipboard.ContainsData(DataFormats.UnicodeText), clipboard.ContainsData(DataFormats.UnicodeText));
        Assert.Equal(Clipboard.GetData(DataFormats.UnicodeText), clipboard.GetData(DataFormats.UnicodeText));
    }

    [WinFormsFact]
    public void DataOfT_BinaryFormatterDisabled_Success()
    {
        var clipboard = new Computer().Clipboard;
        string data = GetUniqueText();
        clipboard.SetDataAsJson(data);

        clipboard.TryGetData(out string? result).Should().BeTrue();  // TanyaSo : fails
        result.Should().Be(data);
    }

    [WinFormsFact]
    public void DataOfT_BinaryFormatterDisabled_Fail()
    {
        var clipboard = new Computer().Clipboard;
        clipboard.SetDataAsJson(new Button());

        clipboard.TryGetData(out Button? result).Should().BeFalse();
    }

    [WinFormsFact]
    public void DataOfT_BinaryFormatterEnabled_Success()
    {
        using BinaryFormatterScope scope = new(enable: true);
        var clipboard = new Computer().Clipboard;
        TestData data = new("thing1", "thing2");
        clipboard.SetDataAsJson(data);

        clipboard.TryGetData(out TestData? result).Should().BeTrue(); // TanyaSo todo
        result.Should().Be(data);
    }

    [WinFormsFact]
    public void DataObject()
    {
        var clipboard = new Computer().Clipboard;
        string data = GetUniqueText();
        Assert.Equal(Clipboard.GetDataObject()!.GetData(DataFormats.UnicodeText), clipboard.GetDataObject().GetData(DataFormats.UnicodeText));
        clipboard.SetDataObject(new DataObject(data));
    }

    private static string GetUniqueText() => Guid.NewGuid().ToString("D");

    [Serializable]
    private class TestData
    {
        public TestData(string text1, string text2)
        {
            _text1 = text1;
            _text2 = text2;
        }

        public string _text1;
        public string _text2;
    }
}
