// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace AppUnderTest;

[DesignerCategory("code")]
public partial class StatusStripForm : Form
{
    public StatusStripForm()
    {
        InitializeComponent();
        backgroundWorker1.WorkerReportsProgress = true;
        backgroundWorker1.DoWork += backgroundWorker1_DoWork;
        backgroundWorker1.ProgressChanged += backgroundWorker1_ProgressChanged;
        if (!backgroundWorker1.IsBusy)
        {
            // Start the asynchronous operation
            backgroundWorker1.RunWorkerAsync();
        }
    }

    private void backgroundWorker1_DoWork(object? sender, DoWorkEventArgs e)
    {
        for (int i = 0; i <= 100; i++)
        {
            Thread.Sleep(50);

            backgroundWorker1.ReportProgress(i);
        }
    }

    private void backgroundWorker1_ProgressChanged(object? sender, ProgressChangedEventArgs e)
    {
        toolStripProgressBar1.Value = e.ProgressPercentage;
    }
}
