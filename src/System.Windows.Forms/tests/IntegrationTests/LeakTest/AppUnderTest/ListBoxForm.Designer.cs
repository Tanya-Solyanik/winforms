// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace AppUnderTest;

partial class ListBoxForm
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
        listBox1 = new ListBox();
        SuspendLayout();
        // 
        // listBox1
        // 
        listBox1.FormattingEnabled = true;
        listBox1.ItemHeight = 15;
        listBox1.Items.AddRange(new object[] { "aaa", "bbb", "ccc" });
        listBox1.Location = new Point(57, 54);
        listBox1.Name = "listBox1";
        listBox1.Size = new Size(120, 94);
        listBox1.TabIndex = 0;
        // 
        // ListBoxForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(233, 346);
        Controls.Add(listBox1);
        Margin = new Padding(4, 3, 4, 3);
        Name = "_listBoxForm";
        Text = "ListBox";
        ResumeLayout(false);
    }

    #endregion

    private ListBox listBox1;
}
