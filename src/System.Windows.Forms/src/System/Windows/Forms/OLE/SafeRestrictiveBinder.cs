// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization;

namespace System.Windows.Forms;

internal class SafeRestrictiveBinder : SerializationBinder
{
    // TanyaSo: We might unseal it to allow for composition of binders.
    private SafeRestrictiveBinder()
    {
    }

    internal static readonly SafeRestrictiveBinder s_instance = new();

    /// <devdoc>
    /// This binder is used in clipboard deserialization and is equivalent to not calling the BinaryFormatter at all.
    /// </devdoc>
    /// <exception cref="NotSupportedException"></exception>
    public override Type? BindToType(string assemblyName, string typeName)
    {
        throw new NotSupportedException("Using BinaryFormatter is not supported in clipboard data deserialization.");
    }
}
