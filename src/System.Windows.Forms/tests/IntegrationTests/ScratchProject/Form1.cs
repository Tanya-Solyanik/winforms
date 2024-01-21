﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace ScratchProject;

// As we can't currently design in VS in the runtime solution, mark as "Default" so this opens in code view by default.
[DesignerCategory("Default")]
public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
        propertyGrid1.SelectedObject = new Class1();
    }

    internal class Class1
    {
        //[SRCategory(nameof(SR.CatAccessibility))]
        //public int Thing1 { get; set; }

        public int Thing2 { get; set; }
    }
}
