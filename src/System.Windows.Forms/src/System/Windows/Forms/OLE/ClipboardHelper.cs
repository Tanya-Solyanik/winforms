// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection.Metadata;
using Switches = System.Windows.Forms.Primitives.LocalAppContextSwitches;

namespace System.Windows.Forms;

internal static class ClipboardHelper
{
    /// <devdoc>
    ///  This binder is used in clipboard deserialization and is equivalent to not calling the BinaryFormatter at all.
    /// </devdoc>
    /// <exception cref="NotSupportedException"></exception>
    internal static Func<TypeName, Type> SafeResolver { get; } = (typeName) =>
        throw new NotSupportedException("Using BinaryFormatter is not supported in WinForms Clipboard data deserialization.");

    internal static Func<TypeName, Type> UnsafeResolver { get; } = (typeName) =>
    {
        if (!Switches.ClipboardEnableUnsafeBinaryFormatterDeserialization)
        {
            throw new NotSupportedException("Using BinaryFormatter is not supported in WinForms Clipboard data deserialization.");
        }

        // The resolver should not return null unless the application had explicitly opted into compatible behavior because it will
        // result in BinaryFormatter deserializing the data.
        // For types that it does not resolve, it should throw a SerializationException.
        return null!;
    };
}
