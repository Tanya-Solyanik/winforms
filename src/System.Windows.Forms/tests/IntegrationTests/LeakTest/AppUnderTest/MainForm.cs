// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace AppUnderTest;

[DesignerCategory("code")]
public partial class MainForm : Form
{
    private long _allocated;
    private readonly string _fileName;
    private int _scenario;

    public MainForm(int scenario)
    {
        InitializeComponent();

        _scenario = scenario;
        scenarioIdTextBox.Text = _scenario.ToString();

        _fileName = Path.Combine(Application.StartupPath, $"deltas{scenario}.log");

        _showTestFormButton.AccessibleName = CommonNames.ShowTestForm;
        _collectButton.AccessibleName = CommonNames.GCCollect;

        if (File.Exists(_fileName))
        {
            File.Delete(_fileName);
        }

        // Requires elevation in .NET.
#if !NET
        // var process = StartInspectIfNeeded();
        // var process = StartToolIfNeeded("AccessibilityInsights");
        var process = StartToolIfNeeded("Narrator");
        if (process is null)
        {
            return;
        }

        process.Exited += (sender, e) =>
        {
            process = null;
        };

        FormClosed += (sender, e) =>
        {
            try
            {
                process?.Kill();
            }
            catch
            {
            }

            if (File.Exists(_fileName))
            {
                // Process.Start($"{_fileName}");
            }
        };
#endif
    }

    private void Scenario<T>() where T : Form, new()
    {
        using var testForm = new T();
        testForm.AccessibleName = CommonNames.TestForm;
        testForm.ShowDialog(this);
    }

    private static void CleanUp()
    { 
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        GC.WaitForPendingFinalizers();
    }

    private void ClickShowFormButton(object sender, EventArgs e)
    {
        CleanUp();

        _allocated = GC.GetTotalMemory(forceFullCollection: true);
        string? text = scenarioIdTextBox.Text;
        if (!string.IsNullOrEmpty(text) && int.TryParse(text, out int result))
        {
            _scenario = result;
        }

        switch(_scenario)
        {
        case 1:
            Scenario<BindingNavigatorForm>(); // 25 kb
            break;
        case 2:
            Scenario<CheckedListBoxForm>();
            break;
        case 3:
            Scenario<ComboBoxForm>();
            break;
        case 4:
            Scenario<ContextMenuForm>();
            break;
        case 5:
            Scenario<DataGridViewForm>();   // 30 kb
            break;
        case 6:
            Scenario<DateTimePickerForm>();
            break;
        case 7:
            Scenario<DomainUpDownForm>();
            break;
        case 8:
            Scenario<GroupBoxForm>();
            break;
        case 9:
            Scenario<HiddenPanelForm>();
            break;
        case 10:
            Scenario<LabelForm>();
            break;
        case 11:
            Scenario<LinkLabelForm>();
            break;
        case 12:
            Scenario<ListBoxForm>();
            break;
        case 13:
            Scenario<ListViewForm>();
            break;
        case 14:
            Scenario<MenuStripForm>();
            break;
        case 15:
            Scenario<MonthCalendarForm>();
            break;
        case 16:
            Scenario<NumericUpDownForm>();
            break;
        case 17:
            Scenario<ProgressBarForm>();
            break;
        case 18:
            Scenario<PropertyGridForm>();
            break;
        case 19:
            Scenario<TextBoxForm>();
            break;
        case 20:
            Scenario<ToolStripForm>();
            break;
        case 21:
            Scenario<RichTextBoxForm>();
            break;
        case 22:
            Scenario<TreeViewForm>();
            break;
        case 23:
            Scenario<StatusStripForm>();
            break;
        default:
            Scenario<ToolStripForm>();
            break;
        }
    }

    private void ClickCollectButton(object sender, EventArgs e)
    {
        CleanUp();

        // bytes
        long leakSize = _allocated;
        _allocated = GC.GetTotalMemory(forceFullCollection: true);
        leakSize = _allocated - leakSize;

        Text = leakSize.ToString();

        File.AppendAllLines(_fileName, [Text]);
    }

    private static Process? StartInspectIfNeeded()
    {
        // Windows kit version should match OS version this app runs on if debugging into inspect process.
        const string inspectPath = @"C:\Program Files (x86)\Windows Kits\10\bin\10.0.19041.0\x64\inspect.exe";
        if (!File.Exists(inspectPath))
        {
            return null;
        }

        Process[] inspects = Process.GetProcessesByName("inspect");
        if (inspects is not null && inspects.Length != 0)
        {
            return inspects[0];
        }

        return Process.Start(inspectPath);
    }

    private static Process? StartToolIfNeeded(string toolName)
    {
        Process[] narrators = Process.GetProcessesByName(toolName);
        if (narrators is not null && narrators.Length != 0)
        {
            return narrators[0];
        }

        return Process.Start($"{toolName}.exe");
    }
}
