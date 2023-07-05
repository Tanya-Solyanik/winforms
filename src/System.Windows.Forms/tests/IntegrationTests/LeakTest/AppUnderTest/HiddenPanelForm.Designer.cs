// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace AppUnderTest;

partial class HiddenPanelForm
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
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        _showPanelButton = new System.Windows.Forms.Button();
        _hidePanelButton = new System.Windows.Forms.Button();
        _collectButton = new System.Windows.Forms.Button();
        SuspendLayout();
        // 
        // _showPanelButton
        // 
        _showPanelButton.Location = new System.Drawing.Point(40, 40);
        _showPanelButton.Name = "_showPanelButton";
        _showPanelButton.Size = new System.Drawing.Size(100, 23);
        _showPanelButton.TabIndex = 0;
        _showPanelButton.Text = "Show panel";
        _showPanelButton.UseVisualStyleBackColor = true;
        _showPanelButton.Click += new System.EventHandler(ClickShowPanelButton);
        // 
        // _hidePanelButton
        // 
        _hidePanelButton.Location = new System.Drawing.Point(40, 70);
        _hidePanelButton.Name = "_hidePanelButton";
        _hidePanelButton.Size = new System.Drawing.Size(100, 23);
        _hidePanelButton.TabIndex = 0;
        _hidePanelButton.Text = "Hide panel";
        _hidePanelButton.UseVisualStyleBackColor = true;
        _hidePanelButton.Click += new System.EventHandler(ClickHidePanelButton);
        // 
        // _collectButton
        // 
        _collectButton.Location = new System.Drawing.Point(40, 103);
        _collectButton.Name = "collectButton";
        _collectButton.Size = new System.Drawing.Size(100, 23);
        _collectButton.TabIndex = 1;
        _collectButton.Text = "GC.Collect";
        _collectButton.UseVisualStyleBackColor = true;
        _collectButton.Click += new System.EventHandler(ClickCollectButton);
        // 
        // PanelForm
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(180, 200);
        Controls.Add(_collectButton);
        Controls.Add(_showPanelButton);
        Controls.Add(_hidePanelButton);
        Name = "_panelForm";
        Text = "Panel";
        ResumeLayout(false);
    }

    #endregion

    private System.Windows.Forms.Button _showPanelButton;
    private System.Windows.Forms.Button _hidePanelButton;
    private System.Windows.Forms.Button _collectButton;
}
