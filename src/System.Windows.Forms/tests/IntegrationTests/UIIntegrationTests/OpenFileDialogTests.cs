// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Xunit.Abstractions;

namespace System.Windows.Forms.UITests;

public class OpenFileDialogTests : ControlTestBase
{
    public OpenFileDialogTests(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    // Regression test for https://github.com/dotnet/winforms/issues/8108
    [WinFormsFact]
    public void OpenWithNonExistingInitDirectory_Success()
    {
        using DialogHostForm dialogOwnerForm = new();
        using OpenFileDialog dialog = new();
        dialog.InitialDirectory = Guid.NewGuid().ToString();
        Assert.Equal(DialogResult.Cancel, dialog.ShowDialog(dialogOwnerForm));
    }

    [WinFormsFact]
    public void OpenWithExistingInitDirectory_Success()
    {
        using DialogHostForm dialogOwnerForm = new();
        using OpenFileDialog dialog = new();
        dialog.InitialDirectory = Path.GetTempPath();
        Assert.Equal(DialogResult.Cancel, dialog.ShowDialog(dialogOwnerForm));
    }

    // Regression test for https://github.com/dotnet/winforms/issues/8414
    [WinFormsFact]
    public void ShowDialog_ResultWithMultiselect()
    {
        using var tempFile = TempFile.Create(0);
        using AcceptDialogForm dialogOwnerForm = new();
        using OpenFileDialog dialog = new();
        dialog.Multiselect = true;
        dialog.InitialDirectory = Path.GetDirectoryName(tempFile.Path);
        dialog.FileName = tempFile.Path;
        Assert.Equal(DialogResult.OK, dialog.ShowDialog(dialogOwnerForm));
        Assert.Equal(tempFile.Path, dialog.FileName);
    }

    // Regression test for https://github.com/dotnet/winforms/issues/12847
    [WinFormsFact]
    public void ShowDialog_Twice_DoesNotCauseStackOverflow()
    {
        bool done = false;

        using OpenTwiceHostForm dialogOwnerForm = new(() => done);
        using OpenFileDialog openFileDialog = new();
        using ToolStrip toolStrip = new();
        using ToolStripButton toolStripButton = new()
        {
            Text = "Double Click Me!"
        };
        toolStrip.Items.Add(toolStripButton);
        dialogOwnerForm.Controls.Add(toolStrip);

        toolStripButton.Click += (sender, e) =>
        {
            openFileDialog.ShowDialog(dialogOwnerForm);
        };

        // Simulate two clicks on the ToolStripButton to open the OpenFileDialog.
        toolStripButton.PerformClick();
        toolStripButton.PerformClick();

        // Verify that the dialog was shown and no stack overflow occurred
        done = true;
    }

    private class OpenTwiceHostForm : DialogHostForm
    {
        private readonly Func<bool> _done;

        public OpenTwiceHostForm(Func<bool> done)
        {
            _done = done;
        }

        protected override void OnDialogIdle(HWND dialogHandle)
        {
            if (_done())
            {
                base.OnDialogIdle(dialogHandle);
            }
        }
    }

    private class AcceptDialogForm : DialogHostForm
    {
        protected override void OnDialogIdle(HWND dialogHandle)
        {
            Accept(dialogHandle);
        }
    }
}
