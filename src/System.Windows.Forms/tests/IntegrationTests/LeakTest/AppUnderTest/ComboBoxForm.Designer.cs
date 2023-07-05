// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace AppUnderTest;

partial class ComboBoxForm
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
            this._testComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // _testComboBox
            // 
            this._testComboBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._testComboBox.FormattingEnabled = true;
            this._testComboBox.Items.AddRange(new object[] {
            "aaa",
            "bbb",
            "ccc"});
            this._testComboBox.Location = new System.Drawing.Point(20, 58);
            this._testComboBox.Margin = new System.Windows.Forms.Padding(6);
            this._testComboBox.Name = "_testComboBox";
            this._testComboBox.Size = new System.Drawing.Size(238, 33);
            this._testComboBox.TabIndex = 0;
            this._testComboBox.Text = "original";
            // 
            // ComboBoxForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 577);
            this.Controls.Add(this._testComboBox);
            this.Margin = new System.Windows.Forms.Padding(6);
            this.Name = "ComboBoxForm";
            this.Text = "ComboBox";
            this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.ComboBox _testComboBox;
}
