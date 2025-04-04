// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Xunit.Abstractions;

namespace System.Windows.Forms.UITests;

public class ErrorProviderTests : ControlTestBase
{
    public ErrorProviderTests(ITestOutputHelper output) : base(output)
    {
    }

    [WinFormsFact]
    public async Task ErrorProvider_SetIcon_ShowsIconOnForm()
    {
        await RunFormAsync(
            () =>
            {
                Form form = new();
                TextBox textBox = new() { Location = new Point(10, 10) };
                form.Controls.Add(textBox);

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

                return (form, textBox);
            },
            async (form, textBox) =>
            {
                form.Show();
                await Task.Delay(1000); // Wait for the form to render

                // Verify the icon is set correctly
                TextBox control = Assert.IsType<TextBox>(form.Controls[0]);
                object? tag = control.Tag;
                Assert.NotNull(tag);
                ErrorProvider errorProvider = Assert.IsType<ErrorProvider>(tag);
                Assert.NotNull(errorProvider.Icon);
                Assert.Equal(256, errorProvider.Icon.Width);
                Assert.Equal(256, errorProvider.Icon.Height);
            });
    }
}
