// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace AppUnderTest;

[DesignerCategory("code")]
public partial class DomainUpDownForm : Form
{
    public DomainUpDownForm()
    {
        InitializeComponent();
        domainUpDown1.Items.AddRange(new object[] { "One", "Two", "Three", "Four", "Five" });
    }
}

// Still leaks if clicking buttons
// 0:013> !GCRoot 000001ff54545018
// HandleTable:
//    000001ff528c1948(ref counted handle)
//    -> 000001ff54547118 System.Windows.Forms.UpDownBase+UpDownEdit+UpDownEditAccessibleObjectLevel5
//    -> 000001ff54545018 System.Windows.Forms.DomainUpDown
//
// DomainUpDown UpDownBase+UpDownEdit TextBoxBase+TextBoxBaseUiaTextProvider UiaTextRange [RefCount Handle, Count: 1]
