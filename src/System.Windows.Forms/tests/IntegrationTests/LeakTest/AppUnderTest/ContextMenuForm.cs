// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace AppUnderTest;

[DesignerCategory("Default")]
public class ContextMenuForm : Form
{
    private readonly ContextMenuStrip _menu;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _menu.Dispose();
        }

        base.Dispose(disposing);
    }

    public ContextMenuForm()
    {
        Text = "ContextMenu";
        ClientSize = new Size(284, 261);
        MouseClick += ContextMenuForm_MouseClick;

        _menu = new ContextMenuStrip();
        for (int i = 0; i < 170; i++)
        {
            var menu = new ToolStripMenuItem($"Menu{i}");
            _menu.Items.Add(menu);
            var subMenu = new ToolStripMenuItem("SubMenu1");
            menu.DropDownItems.Add(subMenu);
            subMenu = new ToolStripMenuItem("SubMenu2");
            menu.DropDownItems.Add(subMenu);
        }
    }

    private void ContextMenuForm_MouseClick(object? sender, MouseEventArgs e)
    {
        _menu.Show(this, e.Location);
    }
}

// UIA inspect with FreeControlsForRefCountedAccessibleObjectsInLevel5 == true or == false
// 0:012> !DumpHeap -type Menu
//    <snip>
// Statistics:
//              MT Count    TotalSize Class Name
// 00007ffc37da1708        1           24 System.Windows.Forms.ToolStripDropDownMenu+ToolStripDropDownLayoutEngine
// 00007ffc37c791d8        1           32 System.Windows.Forms.Form+SecurityMenuItem
// 00007ffc37da3178        1           56 System.Windows.Forms.MenuTimer
// 00007ffc37dac090        1           96 System.Windows.Forms.ToolStripManager+ModalMenuFilter
// Total 4 objects
