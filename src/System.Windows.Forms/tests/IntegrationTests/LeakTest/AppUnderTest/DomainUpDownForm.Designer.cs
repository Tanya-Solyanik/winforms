// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace AppUnderTest;

partial class DomainUpDownForm
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        domainUpDown1 = new DomainUpDown();
        SuspendLayout();
        // 
        // domainUpDown1
        // 
        domainUpDown1.Location = new Point(62, 78);
        domainUpDown1.Name = "domainUpDown1";
        domainUpDown1.Size = new Size(120, 23);
        domainUpDown1.TabIndex = 0;
        domainUpDown1.Text = "domainUpDown1";
        // 
        // DomainUpDownForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(233, 346);
        Controls.Add(domainUpDown1);
        Margin = new Padding(4, 3, 4, 3);
        Name = "_domainUpDownForm";
        Text = "DomainUpDown";
        ResumeLayout(false);
    }

    #endregion

    private DomainUpDown domainUpDown1;
}
