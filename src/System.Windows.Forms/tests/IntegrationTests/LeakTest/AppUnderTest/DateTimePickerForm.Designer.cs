// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace AppUnderTest;

partial class DateTimePickerForm
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
        dateTimePicker1 = new DateTimePicker();
        SuspendLayout();
        // 
        // dateTimePicker1
        // 
        dateTimePicker1.Location = new Point(12, 43);
        dateTimePicker1.Name = "dateTimePicker1";
        dateTimePicker1.Size = new Size(200, 23);
        dateTimePicker1.TabIndex = 0;
        // 
        // DateTimePickerForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(233, 346);
        Controls.Add(dateTimePicker1);
        Margin = new Padding(4, 3, 4, 3);
        Name = "_dateTimePickerForm";
        Text = "DateTimePicker";
        ResumeLayout(false);
    }

    #endregion

    private DateTimePicker dateTimePicker1;
}
