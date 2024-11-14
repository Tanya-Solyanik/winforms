// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System;

public readonly ref struct NrbfSerializerInClipboardDragDropScope
{
    private readonly WinFormsAppContextSwitchScope _switchScope;

    public NrbfSerializerInClipboardDragDropScope(bool enable)
    {
        Monitor.Enter(typeof(NrbfSerializerInClipboardDragDropScope));
        _switchScope = new(WinFormsAppContextSwitchNames.ClipboardDragDropEnableNrbfSerializationSwitchName, enable);
    }

    public void Dispose()
    {
        try
        {
            _switchScope.Dispose();
        }
        finally
        {
            Monitor.Exit(typeof(NrbfSerializerInClipboardDragDropScope));
        }
    }
}
