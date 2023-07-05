// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace AppUnderTest;

partial class LinkLabelForm
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
        linkLabel1 = new LinkLabel();
        SuspendLayout();
        // 
        // linkLabel1
        // 
        linkLabel1.AutoSize = true;
        linkLabel1.Location = new Point(69, 52);
        linkLabel1.Name = "linkLabel1";
        linkLabel1.Size = new Size(60, 15);
        linkLabel1.TabIndex = 0;
        linkLabel1.TabStop = true;
        linkLabel1.Text = "linkLabel1";
        // 
        // LinkLabelForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(233, 346);
        Controls.Add(linkLabel1);
        Margin = new Padding(4, 3, 4, 3);
        Name = "_linkLabelForm";
        Text = "LinkLabel";
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private LinkLabel linkLabel1;
}
