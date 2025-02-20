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
        MenuStrip menuStrip1 = new MenuStrip();
        ToolStripMenuItem fileToolStripMenuItem = new ToolStripMenuItem();
        fileToolStripMenuItem.Text = "File";
        menuStrip1.Items.AddRange(new ToolStripMenuItem[] {fileToolStripMenuItem});
        Controls.Add(menuStrip1);
        PropertyGrid propertyGrid1 = new PropertyGrid();
        propertyGrid1.Dock = DockStyle.Right;
        propertyGrid1.Width = 400;
        Controls.Add(propertyGrid1);
        propertyGrid1.SelectedObject = fileToolStripMenuItem;
    }
}
