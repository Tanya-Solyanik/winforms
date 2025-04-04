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

        TextBox textBox = new() { Location = new Point(10, 10) };
        Controls.Add(textBox);
        // Create a 256x256 icon for testing
        using Bitmap bitmap = new(256, 256);
        using Graphics graphics = Graphics.FromImage(bitmap);
        graphics.FillRectangle(Brushes.Red, 0, 0, 256, 256);
        Icon icon = Icon.FromHandle(bitmap.GetHicon());

        ErrorProvider errorProvider = new()
        {
            Icon = icon
        };

        errorProvider.SetError(textBox, "Test error");
    }
}
