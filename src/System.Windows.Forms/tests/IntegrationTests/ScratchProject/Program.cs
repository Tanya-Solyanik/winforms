// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;

namespace ScratchProject;

// This project is meant for temporary testing and experimenting and should be kept as simple as possible.

internal static class Program
{
    [STAThread]
    public static void Main()
    {
        CultureInfo cultureInfo = new ("de-DE");
        Thread.CurrentThread.CurrentUICulture = cultureInfo;
        Thread.CurrentThread.CurrentCulture = cultureInfo;
        Application.EnableVisualStyles();
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
        Application.Run(new Form1());
    }
}
