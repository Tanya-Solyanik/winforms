// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ScratchProject;

// As we can't currently design in VS in the runtime solution, mark as "Default" so this opens in code view by default.
[DesignerCategory("Default")]
public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
        int y = 20;
        Button button = new() { Text = "BF-Legacy", Size = new Size(200, 40), Location = new(20, y) };
        button.Click += (sender, e) => SetGetClipboardDataLegacy();
        Controls.Add(button);
        y += 40 + 20;

        button = new() { Text = "Json-Legacy", Size = new Size(200, 40), Location = new(20, y) };
        button.Click += (sender, e) => SetGetClipboardDataJson();
        Controls.Add(button);
        y += 40 + 20;

        button = new() { Text = "Json-new", Size = new Size(200, 40), Location = new(20, y) };
        button.Click += (sender, e) => SetGetClipboardDataAsJson();
        Controls.Add(button);
        y += 40 + 20;

        button = new() { Text = "NoJson-new", Size = new Size(200, 40), Location = new(20, y) };
        button.Click += (sender, e) => SetGetClipboardDataWithResolver();
        Controls.Add(button);
        y += 40 + 20;

        button = new() { Text = "Derived-new", Size = new Size(200, 40), Location = new(20, y) };
        button.Click += (sender, e) => SetGetClipboardDataDerived();
        Controls.Add(button);
    }

    private readonly WeatherForecast _weatherForecast = new()
    {
        Date = DateTime.Parse("2019-08-01"),
        TemperatureCelsius = 25,
        Summary = "Hot",
        Font = new("Consolas", emSize: 10)
    };

    public void SetGetClipboardDataLegacy()
    {
        // Old API would require the app to opt into the BinaryFormatter processing.
        Clipboard.SetData("myCustomFormat", _weatherForecast);

#pragma warning disable WFDEV005 // Type or member is obsolete
        object? result = Clipboard.GetData("myCustomFormat");
#pragma warning restore WFDEV005

        if (result is WeatherForecast forecast)
        {
            if (forecast.Equals(_weatherForecast))
            {
                Text = "Data round-tripped successfully!";
            }
        }

        Clipboard.Clear();
    }

    public void SetGetClipboardDataJson()
    {
        // Users could manually JSON serialize their data with old API, on .NET8.or 9
        // to avoid use of the BinaryFormatter.
        byte[] serialized = JsonSerializer.SerializeToUtf8Bytes(_weatherForecast);
        Clipboard.SetData("myCustomFormat", serialized);

#pragma warning disable WFDEV005 // Type or member is obsolete
        if (Clipboard.GetData("myCustomFormat") is byte[] byteData)
#pragma warning restore WFDEV005
        {
            using MemoryStream stream = new(byteData);
            if (JsonSerializer.Deserialize(stream, typeof(WeatherForecast)) is WeatherForecast forecast)
            {
                if (forecast.Equals(_weatherForecast))
                {
                    Text = "Manual JSON data round-tripped successfully!";
                }
            }
        }

        Clipboard.Clear();
    }

    public void SetGetClipboardDataAsJson()
    {
        Clipboard.SetDataAsJson("myCustomFormat", _weatherForecast);
        if (Clipboard.TryGetData("myCustomFormat", out WeatherForecast? forecast))
        {
            if (forecast.Equals(_weatherForecast))
            {
                Text = "JSON data round-tripped successfully!";
            }
        }

        Clipboard.Clear();
    }

    public void SetGetClipboardDataWithResolver()
    {
        Clipboard.SetData("myCustomFormat", _weatherForecast);
        if (Clipboard.TryGetData("myCustomFormat", resolver: WeatherForecast.Resolver, out WeatherForecast? forecast))
        {
            if (forecast?.Equals(_weatherForecast) == true)
            {
                Text = "BF+Resolver data round-tripped successfully!";
            }
        }

        Clipboard.Clear();
    }

    public void SetGetClipboardDataDerived()
    {
        DerivedForecast derived = new()
        {
            Date = DateTime.Parse("2019-08-01"),
            TemperatureCelsius = 25,
            Summary = "Hot",
            Font = new("Consolas", emSize: 10)
        };

        Clipboard.SetDataAsJson("myCustomFormat", derived);
        if (Clipboard.TryGetData("myCustomFormat", resolver: DerivedForecast.DerivedResolver, out WeatherForecast? forecast))
        {
            if (forecast?.Equals(derived) == true)
            {
                Text = "JSON+Resolver derived data round-tripped successfully!";
            }
        }

        Clipboard.Clear();
    }
}

public class DerivedForecast : WeatherForecast
{
#pragma warning disable CA1051 // Do not declare visible instance fields
    public List<int> Temperatures = new() { 21, 20, 23 };
#pragma warning restore CA1051
    public static Type DerivedResolver(TypeName typeName)
    {
        if (typeof(DerivedForecast).FullName! == typeName.FullName)
        {
            return typeof(DerivedForecast);
        }
        
        throw new NotSupportedException($"Can't resolve {typeName.FullName}");
       // return Resolver(typeName);
    }
}

// [Serializable]
public class WeatherForecast
{
    public DateTimeOffset Date { get; set; }
    public int TemperatureCelsius { get; set; }
    public string? Summary { get; set; }

    [JsonConverter(typeof(FontJsonConverter))]
    public Font? Font { get; set; }

    public bool Equals(WeatherForecast other) => Date.Equals(other.Date)
        && TemperatureCelsius == other.TemperatureCelsius
        && Summary == other.Summary
        && SameFont(other.Font);

    private bool SameFont(Font? otherFont) =>
        (Font is null && otherFont is null) ||
        (Font?.Name == otherFont?.Name && Font?.Size == otherFont?.Size);

    public static Type Resolver(TypeName typeName)
    {
        (string name, Type type)[] allowedTypes =
        [
            (typeof(DateTimeOffset).FullName!, typeof(DateTimeOffset)),
            (typeof(Font).FullName!, typeof(Font)),
            (typeof(FontStyle).FullName!, typeof(FontStyle)),
            (typeof(FontFamily).FullName!, typeof(FontFamily)),
            (typeof(GraphicsUnit).FullName!, typeof(GraphicsUnit)),
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

// Simplified for the sake of the example.
public class FontJsonConverter : JsonConverter<Font>
{
    public override Font? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        Font? result = null;
        string? fontFamily = null;
        int size = 0;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                if (fontFamily is not null && size != 0)
                {
                    result = new Font(fontFamily, size);
                }

                return result;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }

            if (reader.GetString() is not string propertyName)
            {
                throw new JsonException();
            }

            reader.Read();

            switch (propertyName)
            {
                case nameof(Font.FontFamily):
                    fontFamily = reader.GetString();
                    break;
                case nameof(Font.Size):
                    size = reader.GetInt32();
                    break;
            }
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, Font value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString(nameof(Font.FontFamily), value.FontFamily.Name);
        writer.WriteNumber(nameof(Font.Size), value.Size);
        writer.WriteEndObject();
    }
}

[Serializable]
public class TestData
{
    public TestData(string name, int age)
    {
        _name = name;
        _age = age;
    }

    private readonly string _name;
    private readonly int _age;

    public override string ToString() => $"{_name} {_age}";
}
