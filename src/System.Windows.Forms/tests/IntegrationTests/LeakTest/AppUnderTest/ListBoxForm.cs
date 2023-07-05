// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace AppUnderTest;

[DesignerCategory("code")]
public partial class ListBoxForm : Form
{
    public ListBoxForm()
    {
        InitializeComponent();
    }
}

// UIA inspect with FreeControlsForRefCountedAccessibleObjectsInLevel5 == true
// 0:013> !GCRoot 0000018c000e56f0
// HandleTable:
//    0000018c71551808(ref counted handle)
//    -> 0000018c000eade0 System.Windows.Forms.ListBox+ListBoxAccessibleObject+ListBoxItemAccessibleObject
//    -> 0000018c000e56f0 System.Windows.Forms.ListBox
//
//    0000018c71551850 (ref counted handle)
//    -> 0000018c000ead90 System.Windows.Forms.ListBox+ListBoxAccessibleObject+ListBoxItemAccessibleObject
//    -> 0000018c000e56f0 System.Windows.Forms.ListBox
//
//    0000018c71551958 (ref counted handle)
//    -> 0000018c000e9bf8 System.Windows.Forms.ListBox+ListBoxAccessibleObject
//    -> 0000018c000e56f0 System.Windows.Forms.ListBox

// Found 3 unique roots (run '!GCRoot -all' to see all roots).
//
//    after:
// 0:012> !DumpHeap -type ListBox
// Statistics:
//              MT    Count    TotalSize Class Name
// 00007ffc37e102b8        2          160 System.Windows.Forms.ListBox+ListBoxAccessibleObject+ListBoxItemAccessibleObject
// 00007ffc37d59cb0        2          192 System.Windows.Forms.ListBox+ListBoxAccessibleObject
// Total 4 objects
