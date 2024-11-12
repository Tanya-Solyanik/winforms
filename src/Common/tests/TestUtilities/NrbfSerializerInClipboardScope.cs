// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System;

public readonly ref struct NrbfSerializerInClipboardScope
{
    private readonly AppContextSwitchScope _switchScope;

    public NrbfSerializerInClipboardScope(bool enable)
    {
        Monitor.Enter(typeof(NrbfSerializerInClipboardScope));
        _switchScope = new(AppContextSwitchNames.ClipboardDragDropEnableNrbfSerializationSwitchName, enable);
    }

    public void Dispose()
    {
        try
        {
            _switchScope.Dispose();
        }
        finally
        {
            Monitor.Exit(typeof(NrbfSerializerInClipboardScope));
        }
    }
}
