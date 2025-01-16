// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

#nullable disable

[Obsolete(
    Obsoletions.StatusBarMessage,
    error: false,
    DiagnosticId = Obsoletions.UnsupportedControlsDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
[EditorBrowsable(EditorBrowsableState.Never)]
[Browsable(false)]
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[DefaultEvent(nameof(PanelClick))]
[DefaultProperty(nameof(Text))]
[Designer($"System.Windows.Forms.Design.StatusBarDesigner, {AssemblyRef.SystemDesign}")]
/// <summary>
///  This type is provided for binary compatibility with the .NET Framework. You should not use it.
/// </summary>
public partial class StatusBar : Control
{
    public StatusBar() => throw new PlatformNotSupportedException();

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [Localizable(true)]
    [MergableProperty(false)]
    public StatusBarPanelCollection Panels => throw null;

    [DefaultValue(false)]
    public bool ShowPanels
    {
        get => throw null;
        set { }
    }

    [DefaultValue(true)]
    public bool SizingGrip
    {
        get => throw null;
        set { }
    }

    public event StatusBarDrawItemEventHandler DrawItem
    {
        add { }
        remove { }
    }

    public event StatusBarPanelClickEventHandler PanelClick
    {
        add { }
        remove { }
    }

    protected virtual void OnPanelClick(StatusBarPanelClickEventArgs e) { }

    protected virtual void OnDrawItem(StatusBarDrawItemEventArgs sbdievent) { }
}
