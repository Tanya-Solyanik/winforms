// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace AppUnderTest;

partial class StatusStripForm
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
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StatusStripForm));
        this.statusStrip1 = new System.Windows.Forms.StatusStrip();
        this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
        this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
        this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
        this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
        this.toolStripComboBox1 = new System.Windows.Forms.ToolStripComboBox();
        this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
        this.toolStripTextBox1 = new System.Windows.Forms.ToolStripTextBox();
        this.toolStripSplitButton1 = new System.Windows.Forms.ToolStripSplitButton();
        this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
        this.aaaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.aaaaaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.bbbbbToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
        this.statusStrip1.SuspendLayout();
        this.SuspendLayout();
        // 
        // statusStrip1
        // 
        this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
        this.toolStripStatusLabel1,
        this.toolStripProgressBar1,
        this.toolStripDropDownButton1,
        this.toolStripSplitButton1,
        this.toolStripStatusLabel2});
        this.statusStrip1.Location = new System.Drawing.Point(0, 98);
        this.statusStrip1.Name = "statusStrip1";
        this.statusStrip1.Size = new System.Drawing.Size(301, 22);
        this.statusStrip1.TabIndex = 0;
        this.statusStrip1.Text = "statusStrip1";
        // 
        // toolStripStatusLabel1
        // 
        this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
        this.toolStripStatusLabel1.Size = new System.Drawing.Size(118, 17);
        this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
        // 
        // toolStripProgressBar1
        // 
        this.toolStripProgressBar1.Name = "toolStripProgressBar1";
        this.toolStripProgressBar1.Size = new System.Drawing.Size(100, 16);
        // 
        // toolStripDropDownButton1
        // 
        this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
        this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
        this.toolStripTextBox1,
        this.toolStripSeparator1,
        this.toolStripComboBox1,
        this.toolStripMenuItem1});
        this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
        this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
        this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
        this.toolStripDropDownButton1.Size = new System.Drawing.Size(29, 20);
        this.toolStripDropDownButton1.Text = "toolStripDropDownButton1";
        // 
        // toolStripMenuItem1
        // 
        this.toolStripMenuItem1.Name = "toolStripMenuItem1";
        this.toolStripMenuItem1.Size = new System.Drawing.Size(181, 22);
        this.toolStripMenuItem1.Text = "toolStripMenuItem1";
        // 
        // toolStripComboBox1
        // 
        this.toolStripComboBox1.Name = "toolStripComboBox1";
        this.toolStripComboBox1.Size = new System.Drawing.Size(121, 23);
        // 
        // toolStripSeparator1
        // 
        this.toolStripSeparator1.Name = "toolStripSeparator1";
        this.toolStripSeparator1.Size = new System.Drawing.Size(178, 6);
        // 
        // toolStripTextBox1
        // 
        this.toolStripTextBox1.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.toolStripTextBox1.Name = "toolStripTextBox1";
        this.toolStripTextBox1.Size = new System.Drawing.Size(100, 23);
        // 
        // toolStripSplitButton1
        // 
        this.toolStripSplitButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
        this.toolStripSplitButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
        this.aaaaaToolStripMenuItem,
        this.aaaToolStripMenuItem});
        this.toolStripSplitButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripSplitButton1.Image")));
        this.toolStripSplitButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
        this.toolStripSplitButton1.Name = "toolStripSplitButton1";
        this.toolStripSplitButton1.Size = new System.Drawing.Size(32, 20);
        this.toolStripSplitButton1.Text = "toolStripSplitButton1";
        // 
        // toolStripStatusLabel2
        // 
        this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
        this.toolStripStatusLabel2.Size = new System.Drawing.Size(118, 17);
        this.toolStripStatusLabel2.Text = "toolStripStatusLabel2";
        // 
        // aaaToolStripMenuItem
        // 
        this.aaaToolStripMenuItem.Name = "aaaToolStripMenuItem";
        this.aaaToolStripMenuItem.Size = new System.Drawing.Size(104, 22);
        this.aaaToolStripMenuItem.Text = "aaa";
        // 
        // aaaaaToolStripMenuItem
        // 
        this.aaaaaToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
        this.bbbbbToolStripMenuItem});
        this.aaaaaToolStripMenuItem.Name = "aaaaaToolStripMenuItem";
        this.aaaaaToolStripMenuItem.Size = new System.Drawing.Size(104, 22);
        this.aaaaaToolStripMenuItem.Text = "aaaaa";
        // 
        // bbbbbToolStripMenuItem
        // 
        this.bbbbbToolStripMenuItem.Name = "bbbbbToolStripMenuItem";
        this.bbbbbToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
        this.bbbbbToolStripMenuItem.Text = "bbbbb";
        // 
        // StatusStripForm
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(301, 120);
        this.Controls.Add(this.statusStrip1);
        this.Name = "StatusStripForm";
        this.Text = "StatusStripForm";
        this.statusStrip1.ResumeLayout(false);
        this.statusStrip1.PerformLayout();
        this.ResumeLayout(false);
        this.PerformLayout();

    }

    #endregion

    private StatusStrip statusStrip1;
    private ToolStripStatusLabel toolStripStatusLabel1;
    private ToolStripProgressBar toolStripProgressBar1;
    private ToolStripDropDownButton toolStripDropDownButton1;
    private ToolStripTextBox toolStripTextBox1;
    private ToolStripSeparator toolStripSeparator1;
    private ToolStripComboBox toolStripComboBox1;
    private ToolStripMenuItem toolStripMenuItem1;
    private ToolStripSplitButton toolStripSplitButton1;
    private ToolStripMenuItem aaaaaToolStripMenuItem;
    private ToolStripMenuItem bbbbbToolStripMenuItem;
    private ToolStripMenuItem aaaToolStripMenuItem;
    private ToolStripStatusLabel toolStripStatusLabel2;
    private BackgroundWorker backgroundWorker1;
}