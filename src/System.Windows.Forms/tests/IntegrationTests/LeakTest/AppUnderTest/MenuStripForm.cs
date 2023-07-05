// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace AppUnderTest;

[DesignerCategory("code")]
public partial class MenuStripForm : Form
{
    public MenuStripForm()
    {
        InitializeComponent();
        toolStripComboBox1.Items.AddRange([
            "aaa",
            "bbb",
            "ccc"]);
    }
}

// ToolStripDropDownMenu ToolStripMenuItem ToolStripMenuItem+ToolStripMenuItemAccessibleObject [RefCount Handle, Count: 5]
// ToolStripMenuItemAccessibleObject  ToolStripDropDownItemAccessibleObject  ToolStripItemAccessibleObject
