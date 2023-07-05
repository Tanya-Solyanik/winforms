// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace AppUnderTest;

[DesignerCategory("code")]
public partial class HiddenPanelForm : Form
{
    public HiddenPanelForm()
    {
        InitializeComponent();
    }

    private void ClickShowPanelButton(object sender, EventArgs e)
    {
        var view = new Panel()
        {
            BackColor = Color.Blue,
            Name = "View",
            Bounds = new Rectangle(100, 100, 100, 100),
            Parent = this,
            Dock = DockStyle.Left,
        };

        Controls.Add(view);
    }

    private void ClickHidePanelButton(object sender, EventArgs e)
    {
        SuspendLayout();

        var view = Controls.Find("View", false)?.FirstOrDefault() as Panel;
        if (view is not null)
        {
            Controls.Remove(view);
            view.Dispose();
        }

        ResumeLayout(false);
    }

    private void ClickCollectButton(object sender, EventArgs e)
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
    }

    // Before
    //        0:016> !GCRoot -nostacks 18874c52a28
    // HandleTable:
    //    0000018872581290 (strong handle)
    //          -> 018874c50da8 AppUnderTest.HiddenPanelForm
    //          -> 018874c53da8 System.Windows.Forms.LayoutEventArgs
    //          -> 018874c52a28 System.Windows.Forms.Panel

    // Found 1 unique roots.
}
