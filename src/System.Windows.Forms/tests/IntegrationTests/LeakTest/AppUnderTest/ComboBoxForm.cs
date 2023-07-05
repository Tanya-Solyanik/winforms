// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace AppUnderTest;

[DesignerCategory("code")]
public partial class ComboBoxForm : Form
{
    public ComboBoxForm()
    {
        InitializeComponent();
        _testComboBox.SelectedIndex = 0;
        _testComboBox.SelectionStart = 0;
        _testComboBox.SelectionLength = _testComboBox.Text.Length;
    }

    // 0:018> !GCRoot 000001540d830770
    // HandleTable:
    //    000001540B461FA8(ref counted handle)
    //    -> 000001540D8334B8 System.Windows.Forms.ComboBox+ComboBoxChildDropDownButtonUiaProvider
    //    -> 000001540D830770 System.Windows.Forms.ComboBox
    //    000001540B461FB8 (ref counted handle)
    //    -> 000001540D832068 System.Windows.Forms.ComboBox+ComboBoxChildEditUiaProvider
    //    -> 000001540D830770 System.Windows.Forms.ComboBox

    // Before FreeControlsForRefCountedAccessibleObjectsInLevel5==false
    //    Statistics:
    //              MT Count    TotalSize Class Name
    // 00007ffc37d69338        1           24 System.Windows.Forms.ComboBox+AutoCompleteDropDownFinder
    // 00007ffc37d96288        1           72 System.Windows.Forms.ComboBox+ComboBoxChildDropDownButtonUiaProvider
    // 00007ffc37d6c738        1          120 System.Windows.Forms.ComboBox+ComboBoxUiaProvider
    // 00007ffc37d92ee0        2          160 System.Windows.Forms.ComboBox+ComboBoxChildEditUiaProvider
    // 00007ffc37d684a8        1          408 System.Windows.Forms.ComboBox

    // UIA inspect with FreeControlsForRefCountedAccessibleObjectsInLevel5==true
    // Statistics:
    //          MT Count    TotalSize Class Name
    // 00007ffc37e03d68        2          160 System.Windows.Forms.ComboBox+ComboBoxChildEditUiaProvider
    // Total 2 objects

    // with items:
    //       ComboBox -> ComboBox+ComboBoxChildDropDownButtonUiaProvider -> InternalAccessibleObject [RefCount Handle, Count: 1]
    //
    // ComboBox  ComboBox+ComboBoxItemAccessibleObject InternalAccessibleObject [RefCount Handle, Count: 1]
}
