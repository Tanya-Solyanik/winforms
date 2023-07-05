// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace AppUnderTest;

[DesignerCategory("code")]
public partial class PropertyGridForm : Form
{
    public PropertyGridForm()
    {
        InitializeComponent();
        propertyGrid1.SelectedObject = this;
    }

    private void PropertyGridForm_FormClosed(object sender, FormClosedEventArgs e)
    {
        TypeDescriptor.Refresh(propertyGrid1);
        TypeDescriptor.Refresh(propertyGrid1.GetType().Assembly);
        TypeDescriptor.Refresh(typeof(Color).Assembly);
        // TypeDescriptor.Refresh(typeof(ColorEditor).Assembly);
        TypeDescriptor.Refresh(GetType().Assembly);
    }
}

// UIA inspect with FreeControlsForRefCountedAccessibleObjectsInLevel5 == true

// 0:014> !DumpHeap -type PropertyGrid
//         Address MT     Size
// 0000027b000f9d80 00007ffc37e50790       24     
// 0000027b000f9d98 00007ffc37e50868       24     

// Statistics:
//              MT Count    TotalSize Class Name
// 00007ffc37e50868        1           24 System.Windows.Forms.PropertyGridInternal.GridEntry+DisplayNameSortComparer
// 00007ffc37e50790        1           24 System.Windows.Forms.PropertyGridInternal.AttributeTypeSorter
// Total 2 objects
//
// PropertyGrid PropertyDescriptorGridEntry PropertyDescriptorGridEntry+PropertyDescriptorGridEntryAccessibleObject [RefCount Handle, Count: 17]
//
// AppUnderTest.PropertyGridForm SingleSelectRootGridEntry CategoryGridEntry PropertyDescriptorGridEntry PropertyDescriptorGridEntry+PropertyDescriptorGridEntryAccessibleObject [RefCount Handle, Count: 3]	1
