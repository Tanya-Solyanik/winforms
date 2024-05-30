// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using System.Runtime.Serialization;

namespace ScratchProject;

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
