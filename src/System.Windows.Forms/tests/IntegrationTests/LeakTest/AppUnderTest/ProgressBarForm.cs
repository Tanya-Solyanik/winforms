// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace AppUnderTest;

[DesignerCategory("code")]
public partial class ProgressBarForm : Form
{
    public ProgressBarForm()
    {
        InitializeComponent();

        _backgroundWorker1.DoWork += backgroundWorker1_DoWork;
        _backgroundWorker1.ProgressChanged += backgroundWorker1_ProgressChanged;
        if (!_backgroundWorker1.IsBusy)
        {
            // Start the asynchronous operation
            _backgroundWorker1.RunWorkerAsync();
        }
    }

    private readonly BackgroundWorker _backgroundWorker1 = new()
    {
        WorkerReportsProgress = true,
    };

    private void backgroundWorker1_DoWork(object? sender, DoWorkEventArgs e)
    {
        for (int i = 0; i <= 100; i++)
        {
            Thread.Sleep(50);

            _backgroundWorker1.ReportProgress(i);
        }
    }

    private void backgroundWorker1_ProgressChanged(object? sender, ProgressChangedEventArgs e)
    {
        progressBar1.Value = e.ProgressPercentage;
    } 
}
