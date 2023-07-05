// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace AppUnderTest;

[DesignerCategory("code")]
public partial class LabelForm : Form
{
    public LabelForm()
    {
        InitializeComponent();
    }
}

// UIA inspect with FreeControlsForRefCountedAccessibleObjectsInLevel5 == true
// 0:014> !DumpHeap -type Label
//         Address MT     Size

// Statistics:
//              MT Count    TotalSize Class Name
// Total 0 objects
