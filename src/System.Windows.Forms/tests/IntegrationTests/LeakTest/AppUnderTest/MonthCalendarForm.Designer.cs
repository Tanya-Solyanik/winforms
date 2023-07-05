// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace AppUnderTest;

partial class MonthCalendarForm
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
        monthCalendar1 = new MonthCalendar();
        SuspendLayout();
        // 
        // monthCalendar1
        // 
        monthCalendar1.Location = new Point(0, 0);
        monthCalendar1.Name = "monthCalendar1";
        monthCalendar1.TabIndex = 0;
        // 
        // MonthCalendarForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(233, 346);
        Controls.Add(monthCalendar1);
        Margin = new Padding(4, 3, 4, 3);
        Name = "_monthCalendarForm";
        Text = "MonthCalendar";
        ResumeLayout(false);
    }

    #endregion

    private MonthCalendar monthCalendar1;
}
