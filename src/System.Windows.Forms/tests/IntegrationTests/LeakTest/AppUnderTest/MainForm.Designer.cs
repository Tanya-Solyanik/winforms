// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace AppUnderTest;

partial class MainForm
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
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
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
            this._showTestFormButton = new System.Windows.Forms.Button();
            this._collectButton = new System.Windows.Forms.Button();
            this.scenarioIdTextBox = new System.Windows.Forms.TextBox();
            this.scenarioIdLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // _showTestFormButton
            // 
            this._showTestFormButton.Location = new System.Drawing.Point(40, 40);
            this._showTestFormButton.Name = "_showTestFormButton";
            this._showTestFormButton.Size = new System.Drawing.Size(100, 23);
            this._showTestFormButton.TabIndex = 0;
            this._showTestFormButton.Text = "Show Test Form";
            this._showTestFormButton.UseVisualStyleBackColor = true;
            this._showTestFormButton.Click += new System.EventHandler(this.ClickShowFormButton);
            // 
            // _collectButton
            // 
            this._collectButton.Location = new System.Drawing.Point(40, 103);
            this._collectButton.Name = "_collectButton";
            this._collectButton.Size = new System.Drawing.Size(100, 23);
            this._collectButton.TabIndex = 1;
            this._collectButton.Text = "GC.Collect";
            this._collectButton.UseVisualStyleBackColor = true;
            this._collectButton.Click += new System.EventHandler(this.ClickCollectButton);
            // 
            // scenarioIdTextBox
            // 
            this.scenarioIdTextBox.Location = new System.Drawing.Point(40, 153);
            this.scenarioIdTextBox.Name = "scenarioIdTextBox";
            this.scenarioIdTextBox.Size = new System.Drawing.Size(100, 20);
            this.scenarioIdTextBox.TabIndex = 2;
            // 
            // scenarioIdLabel
            // 
            this.scenarioIdLabel.AutoSize = true;
            this.scenarioIdLabel.Location = new System.Drawing.Point(37, 129);
            this.scenarioIdLabel.Name = "scenarioIdLabel";
            this.scenarioIdLabel.Size = new System.Drawing.Size(61, 13);
            this.scenarioIdLabel.TabIndex = 3;
            this.scenarioIdLabel.Text = "Scenario Id";
            // 
            // MainForm
            // 
            this.AcceptButton = this._showTestFormButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(232, 200);
            this.Controls.Add(this.scenarioIdLabel);
            this.Controls.Add(this.scenarioIdTextBox);
            this.Controls.Add(this._collectButton);
            this.Controls.Add(this._showTestFormButton);
            this.Name = "MainForm";
            this.Text = "Start Form";
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button _showTestFormButton;
    private System.Windows.Forms.Button _collectButton;
    private TextBox scenarioIdTextBox;
    private Label scenarioIdLabel;
}
