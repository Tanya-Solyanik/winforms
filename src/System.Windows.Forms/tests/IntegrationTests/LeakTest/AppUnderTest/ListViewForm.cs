// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace AppUnderTest;

[DesignerCategory("code")]
public partial class ListViewForm : Form
{
    public ListViewForm()
    {
        InitializeComponent();
        listView1.LabelEdit = true;
    }
}

// UIA inspect with FreeControlsForRefCountedAccessibleObjectsInLevel5 == true
// 0:014> !GCRoot 00000215cf73b040
// HandleTable:
//    00000215cdc71988(ref counted handle)
//    -> 00000215cf73d3a0 System.Windows.Forms.ListView+ListViewAccessibleObject
//    -> 00000215cf73b040 System.Windows.Forms.ListView
// Found 1 unique roots (run '!GCRoot -all' to see all roots).

// after
// 0:012> !DumpHeap -type ListView

// Statistics:
//              MT    Count    TotalSize Class Name
// 00007ffc37d496f0        1           24 System.Windows.Forms.ListViewItem[]
// 00007ffc37d4c3b0        2          192 System.Windows.Forms.ListView+ListViewAccessibleObject
// Total 3 objects
