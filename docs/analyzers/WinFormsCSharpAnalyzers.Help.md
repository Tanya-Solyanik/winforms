# How to use System.Windows.Forms.Analyzers

System.Windows.Forms.Analyzers analyzers and source generators are shipped inbox with Windows Desktop .NET SDK, and
are automatically referenced for Window Forms .NET applications.

## `AppManifestAnalyzer`

`AppManifestAnalyzer` is automatically invoked when a Windows Forms application (`OutputType=Exe` or `OutputType=WinExe`) has a custom app.manifest.

### [WFO0003](https://aka.ms/winforms-warnings/WFAC010): Unsupported high DPI configuration.

Windows Forms applications should specify application DPI-awareness via the [application configuration](https://aka.ms/applicationconfiguration) or
[`Application.SetHighDpiMode` API](https://docs.microsoft.com/dotnet/api/system.windows.forms.application.sethighdpimode).

|Item|Value|
|-|-|
| Category | ApplicationConfiguration |
| Enabled | True |
| Severity | Warning |
| CodeFix | False |
---

## `MissingPropertySerializationConfiguration`

`MissingPropertySerializationConfiguration` checks for missing `DesignerSerializationVisibilityAttribute` on properties of classes which are 
derived from `Control` and could potentially serialize design-time data by the designer without the user being aware of it.

### [WFO1000](https://aka.ms/winforms-warnings/WFO1000): Missing property serialization configuration.

Properties of classes derived from `Control` should have `DesignerSerializationVisibilityAttribute` 
set to `DesignerSerializationVisibility.Content` or `DesignerSerializationVisibility.Visible`.

|Item|Value|
|-|-|
| Category | WinForms Security |
| Enabled | True |
| Severity | Warning |
| CodeFix | False |
---

## `ImplementITypedDataObjectInAdditionToIDataObject`

`ImplementITypedDataObjectInAdditionToIDataObject` checks for custom implementations of the managed `IDataObject` interface and suggests to also implement the `ITypedDataObject` interface.

### [WFO1001](https://aka.ms/winforms-warnings/WFO1001): Implement ITypedDataObject interface in addition to IDataObject.

Custom data objects must implement `ITypedDataObject` interface in order to support the best practices in reading binary formatted data from the Clipboard or data being dragged and dropped.

|Item|Value|
|-|-|
| Category | WinForms Security |
| Enabled | True |
| Severity | Warning |
| CodeFix | False |
---
