// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization;
using System.Windows.Forms.Primitives;

namespace System.Windows.Forms;

internal class UnsafeCompatibleBinder : SerializationBinder
{
    // TanyaSo: We might unseal it to allow for composition of binders.
    private UnsafeCompatibleBinder()
    {
    }

    internal static readonly UnsafeCompatibleBinder s_instance = new ();

    // This binder is used in clipboard deserialization and is equivalent to not having a binder on BinaryFormatter at all.
    public override Type? BindToType(string assemblyName, string typeName)
    {
        if (!LocalAppContextSwitches.ClipboardEnableUnsafeBinaryFormatterDeserialization)
        {
            throw new NotSupportedException("Using BinaryFormatter is not supported in clipboard data deserialization.");
        }

        // cs/deserialization/nullbindtotype
        return null; // CodeQL [SM04225] : This binder is required for compatibility scenarios, user has to opt-in the application into this codepath.
    }
}
