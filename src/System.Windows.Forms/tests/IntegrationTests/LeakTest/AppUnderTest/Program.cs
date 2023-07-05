// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace AppUnderTest;

internal static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main(string[] args)
    {
        if (args is null || args.Length < 1)
        {
            args = ["1"];
        }

        if (!int.TryParse(args[0], out int scenario))
        {
            return;
        }

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        Application.ThreadException += Application_ThreadException;
        // Set the unhandled exception mode to force all Windows Forms errors to go through
        // our handler.
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException, true);

        MainForm form = new(scenario);
        Application.Run(form);
    }

    private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
    {
        Debug.WriteLine(e.Exception.Message);
    }
}
