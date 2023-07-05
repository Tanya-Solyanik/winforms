// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace AppUnderTest;

[DesignerCategory("code")]
public partial class ToolStripForm : Form
{
    public ToolStripForm()
    {
        InitializeComponent();
        toolStripComboBox1.Items.AddRange([
            "aaa",
            "bbb",
            "ccc"]);
        toolStripTextBox1.Text = "original";
        toolStripProgressBar1.Value = 50;
    }
}

// ToolStrip ToolStripOverflowButton  ToolStripOverflowButton+ToolStripOverflowButtonAccessibleObject [RefCount Handle, Count: 5]
// ToolStripSplitButton+ToolStripSplitButtonUiaProvider [RefCount Handle, Count: 2]
// ToolStripSplitButton ToolStripSplitButton+ToolStripSplitButtonExAccessibleObject ToolStripSplitButton+ToolStripSplitButtonUiaProvider [RefCount Handle, Count: 2]
