// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace AppUnderTest;

partial class TextBoxForm
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
        _testTextBox = new System.Windows.Forms.TextBox();
        SuspendLayout();
        // 
        // _testTextBox
        // 
        _testTextBox.Location = new System.Drawing.Point(10, 30);
        _testTextBox.Name = "testTextBox";
        _testTextBox.Size = new System.Drawing.Size(121, 21);
        _testTextBox.TabIndex = 0;
        // 
        // emptyComboBoxForm
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(200, 300);
        Controls.Add(_testTextBox);
        Name = "_textBoxForm";
        Text = "TextBox";
        ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TextBox _testTextBox;
}
