// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace AppUnderTest;

[DesignerCategory("code")]
public partial class DateTimePickerForm : Form
{
    public DateTimePickerForm()
    {
        InitializeComponent();
    }
}

// UIA inspect with FreeControlsForRefCountedAccessibleObjectsInLevel5 == true
// 0:012> !DumpHeap -type Date
// Statistics:
//              MT Count    TotalSize Class Name
// 00007ffc92fbdbf8        6          144 System.DateTime
// 00007ffc37d57b80        2          176 System.Windows.Forms.DateTimePicker+DateTimePickerAccessibleObject
// 00007ffc92fc51e8        2          736 System.Globalization.DateTimeFormatInfo
// Total 10 objects
