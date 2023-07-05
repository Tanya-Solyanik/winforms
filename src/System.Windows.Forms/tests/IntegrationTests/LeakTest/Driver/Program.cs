// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Common;
using System.Diagnostics;
using System.IO;
using System.Windows.Automation;

namespace Driver;

public class Program
{
    internal static void Main(string[]? args)
    {
        string exePath;

        if (args is null || args.Length < 1)
        {
            string configuration = "Release";
#if DEBUG
            configuration = "Debug";
#endif
            exePath = $"..\\..\\..\\AppUnderTest\\{configuration}\\net9.0\\AppUnderTest.exe";

            exePath = Path.Combine(Directory.GetCurrentDirectory(), exePath);
        }
        else
        {
            exePath = args[0];
        }

        if (!File.Exists(exePath))
        {
            return;
        }

        RunAppUnderTest(exePath, 1);

        // for (int i = 1; i <= 23; i++)
        // {
        //    RunAppUnderTest(exePath, i);
        // }
    }

    private static void RunAppUnderTest(string exePath, int scenario)
    {
        using var testApp = Process.Start(exePath, scenario.ToString());

        testApp.WaitForInputIdle();
        // .NET app takes much longer to start.
        IntPtr formHandle = testApp.MainWindowHandle;
        int attempt = 5;
        while (formHandle == IntPtr.Zero)
        {
            if (attempt-- == 0)
            {
                return;
            }

            Thread.Sleep(1000);
        }

        var form = AutomationElement.FromHandle(formHandle);
        if (form is null)
        {
            return;
        }

        for (int i = 0; i < 10; i++)
        {
            if (ClickButton(form, CommonNames.ShowTestForm) != 0)
            {
                return;
            }

            var testForm = FindNamedElement(form, CommonNames.TestForm);
            if (testForm is null)
            {
                return;
            }

            WalkEnabledElements(testForm);

            object pattern = testForm.GetCurrentPattern(WindowPattern.Pattern);
            if (pattern is WindowPattern windowPattern)
            {
                windowPattern.Close();
            }

            // Test form is a child of the main form, wait until it notifies the parent that it's closed,
            // or we would hit it while enumerating children or refresh the Automation element that corresponds
            // to the main form. 
            // Thread.Sleep(1000);
            form = AutomationElement.FromHandle(formHandle);
            if (form is null)
            {
                return;
            }

            if (ClickButton(form, CommonNames.GCCollect) != 0)
            {
                return;
            }
        }

        testApp.CloseMainWindow();
    }

    private static AutomationElement? FindNamedElement(AutomationElement rootElement, string target)
    {
        System.Windows.Automation.Condition condition1 = new PropertyCondition(AutomationElement.IsControlElementProperty, true);
        TreeWalker walker = new(condition1);

        AutomationElement? next = walker.GetFirstChild(rootElement);

        while (next is not null)
        {
            string? name = next.GetCurrentPropertyValue(
                AutomationElement.NameProperty,
                ignoreDefaultValue: true) as string;

            if (name == target)
            {
                return next;
            }

            var element = FindNamedElement(next, target);
            if (element is not null)
            {
                return element;
            }

            next = walker.GetNextSibling(next);
        }

        return null;
    }

    internal static int ClickButton(string buttonName)
    {
        var button = AutomationElement.RootElement.FindFirst(
            TreeScope.Descendants,
            new PropertyCondition(AutomationElement.NameProperty, buttonName));
        if (button is null)
        {
            return 1;
        }

        if (button.GetCurrentPattern(InvokePattern.Pattern) is not InvokePattern click)
        {
            return 2;
        }

        click.Invoke();

        return 0;
    }

    internal static int ClickButton(AutomationElement invokable)
    {
        if (invokable.GetCurrentPattern(InvokePattern.Pattern) is not InvokePattern click)
        {
            return 2;
        }

        click.Invoke();

        return 0;
    }

    private static int ClickButton(AutomationElement parent, string name)
    {
        AutomationElement? button = FindNamedElement(parent, name);
        if (button is null)
        {
            return 4;
        }

        return ClickButton(button);
    }

    internal static void WalkEnabledElements(AutomationElement rootElement)
    {
        System.Windows.Automation.Condition condition1 = new PropertyCondition(AutomationElement.IsControlElementProperty, true);
        System.Windows.Automation.Condition condition2 = new PropertyCondition(AutomationElement.IsEnabledProperty, true);
        TreeWalker walker = new(new AndCondition(condition1, condition2));

        Debug.WriteLine($"WalkEnabledElements rootElement: {rootElement.Current.Name}");
        ReadElementData(rootElement);
        AutomationElement? next = walker.GetFirstChild(rootElement);
        while (next is not null)
        {
            WalkEnabledElements(next);

            Debug.WriteLine($"Calling GetNextSibling for {next.Current.Name}");
            next = walker.GetNextSibling(next);
        }
    }

    internal static void ReadElementData(AutomationElement element)
    {
        int[] id = element.GetRuntimeId();
        foreach (int i in id)
        {
            Debug.Write($"{i} ");
        }

        Debug.WriteLine("");

        var properties = element.GetSupportedProperties();
        foreach (AutomationProperty property in properties)
        {
            object value = element.GetCurrentPropertyValue(
                property,
                ignoreDefaultValue: true);
            Debug.WriteLine($"{Automation.PropertyName(property)} {value}");
        }

        var patterns = element.GetSupportedPatterns();
        foreach (AutomationPattern pattern in patterns)
        {
            object patternValue = element.GetCurrentPattern(pattern);
            Debug.WriteLine($"{Automation.PatternName(pattern)} {patternValue}");
        }
    }

    #if !NET
    private static Process? StartToolIfNeeded(string toolName)
    {
        Process[] narrators = Process.GetProcessesByName(toolName);
        if (narrators is not null && narrators.Length != 0)
        {
            return narrators[0];
        }

        return Process.Start($"{toolName}.exe");
    }
    #endif
}

