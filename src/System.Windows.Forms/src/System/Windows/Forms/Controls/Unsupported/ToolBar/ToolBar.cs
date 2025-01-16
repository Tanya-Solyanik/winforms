// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

#nullable disable

[Obsolete(
    Obsoletions.ToolBarMessage,
    error: false,
    DiagnosticId = Obsoletions.UnsupportedControlsDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
[EditorBrowsable(EditorBrowsableState.Never)]
[Browsable(false)]
[ComVisible(true)]
[ClassInterface(ClassInterfaceType.AutoDispatch)]
[DefaultEvent(nameof(ButtonClick))]
[Designer($"System.Windows.Forms.Design.ToolBarDesigner, {AssemblyRef.SystemDesign}")]
[DefaultProperty(nameof(Buttons))]
/// <summary>
///  This type is provided for binary compatibility with the .NET Framework. You should not use it.
/// </summary>
public partial class ToolBar : Control
{
    // Suppress creation of a default constructor by the compiler. This class should not be "constructable".
    public ToolBar() => throw new PlatformNotSupportedException();

    [DefaultValue(ToolBarAppearance.Normal)]
    [Localizable(true)]
    public ToolBarAppearance Appearance
    {
        get => throw null;
        set { }
    }

    [DefaultValue(BorderStyle.None)]
    [Runtime.InteropServices.DispId(-504)]
    public BorderStyle BorderStyle
    {
        get => throw null;
        set { }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [Localizable(true)]
    [MergableProperty(false)]
    public ToolBarButtonCollection Buttons => throw null;

    [RefreshProperties(RefreshProperties.All)]
    [Localizable(true)]
    public Size ButtonSize
    {
        get => throw null;
        set { }
    }

    [DefaultValue(true)]
    public bool Divider
    {
        get => throw null;
        set { }
    }

    [DefaultValue(false)]
    [Localizable(true)]
    public bool DropDownArrows
    {
        get => throw null;
        set { }
    }

    [DefaultValue(null)]
    public ImageList ImageList
    {
        get => throw null;
        set { }
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Size ImageSize => throw null;

    [DefaultValue(false)]
    [Localizable(true)]
    public bool ShowToolTips
    {
        get => throw null;
        set { }
    }

    [DefaultValue(ToolBarTextAlign.Underneath)]
    [Localizable(true)]
    public ToolBarTextAlign TextAlign
    {
        get => throw null;
        set { }
    }

    [DefaultValue(true)]
    [Localizable(true)]
    public bool Wrappable
    {
        get => throw null;
        set { }
    }

    public event ToolBarButtonClickEventHandler ButtonClick
    {
        add { }
        remove { }
    }

    public event ToolBarButtonClickEventHandler ButtonDropDown
    {
        add { }
        remove { }
    }

    protected virtual void OnButtonClick(ToolBarButtonClickEventArgs e) { }

    protected virtual void OnButtonDropDown(ToolBarButtonClickEventArgs e) { }
}
