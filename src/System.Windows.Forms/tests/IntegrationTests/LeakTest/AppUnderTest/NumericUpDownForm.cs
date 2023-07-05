// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace AppUnderTest;

[DesignerCategory("code")]
public partial class NumericUpDownForm : Form
{
    public NumericUpDownForm()
    {
        InitializeComponent();
    }
}

// UIA inspect with FreeControlsForRefCountedAccessibleObjectsInLevel5 == true
// 0:012> !DumpHeap -type UpDown
// Statistics:
//              MT    Count    TotalSize Class Name
// 00007ffc37d4d2c8        2          176 System.Windows.Forms.NumericUpDown+NumericUpDownAccessibleObject
// 00007ffc37e03df0        2          208 System.Windows.Forms.UpDownBase+UpDownButtons+UpDownButtonsAccessibleObject
// 00007ffc37d4d928        2          224 System.Windows.Forms.UpDownBase+UpDownEdit+UpDownEditAccessibleObjectLevel5
// 00007ffc37e04af0        4          288 System.Windows.Forms.UpDownBase+UpDownButtons+UpDownButtonsAccessibleObject+DirectionButtonAccessibleObject
// Total 10 objects
