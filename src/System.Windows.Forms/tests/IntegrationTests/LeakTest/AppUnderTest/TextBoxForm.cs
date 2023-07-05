// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace AppUnderTest;

[DesignerCategory("code")]
public partial class TextBoxForm : Form
{
    public TextBoxForm()
    {
        InitializeComponent();
    }

    // Navigate in the textbox using arrows -
    //            0:015> !GCRoot 000001f436805b28
    // HandleTable:
    //    000001F4341D1DA0(ref counted handle)
    //    -> 000001F436807DE8 System.Windows.Forms.TextBoxBase+TextBoxBaseAccessibleObject
    //    -> 000001F436805B28 System.Windows.Forms.TextBox

    // Found 1 unique roots (run '!gcroot -all' to see all roots).

    // UIA inspect with FreeControlsForRefCountedAccessibleObjectsInLevel5
    //    0:012> !DumpHeap -type TextBox
    //         Address MT     Size
    // 00000219a21d9f88 00007ffc37d584a0      104     
    // 00000219a21e98c0 00007ffc37d584a0      104     

    // Statistics:
    //              MT Count    TotalSize Class Name
    // 00007ffc37d584a0        2          208 System.Windows.Forms.TextBoxBase+TextBoxBaseAccessibleObject
    // Total 2 objects
}
