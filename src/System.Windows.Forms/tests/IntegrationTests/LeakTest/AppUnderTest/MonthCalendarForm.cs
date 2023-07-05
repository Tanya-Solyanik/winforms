// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace AppUnderTest;

[DesignerCategory("code")]
public partial class MonthCalendarForm : Form
{
    public MonthCalendarForm()
    {
        InitializeComponent();
    }
}

// UIA inspect with FreeControlsForRefCountedAccessibleObjectsInLevel5 == true
// 0:012> !DumpHeap -type MonthCalendar
//         Address               MT     Size

// Statistics:
//              MT    Count    TotalSize Class Name
// Total 0 objects
