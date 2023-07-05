// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace AppUnderTest;

[DesignerCategory("code")]
public partial class LinkLabelForm : Form
{
    public LinkLabelForm()
    {
        InitializeComponent();
    }
}

// UIA inspect with FreeControlsForRefCountedAccessibleObjectsInLevel5
// 0:011> !DumpHeap -type LinkLabel
//         Address MT     Size
// 000002580215cf98 00007ffc37d5a7e8       88     
// 000002580215d1c8 00007ffc37d5abc0       72     
// 0000025802160c10 00007ffc37d583c0       24     
// 0000025802166718 00007ffc37d5a7e8       88     
// 00000258021667b0 00007ffc37d5abc0       72     

// Statistics:
//              MT Count    TotalSize Class Name
// 00007ffc37d583c0        1           24 System.Windows.Forms.LinkLabel+LinkComparer
// 00007ffc37d5abc0        2          144 System.Windows.Forms.LinkLabel+LinkAccessibleObject
// 00007ffc37d5a7e8        2          176 System.Windows.Forms.LinkLabel+LinkLabelAccessibleObject
// Total 5 objects
