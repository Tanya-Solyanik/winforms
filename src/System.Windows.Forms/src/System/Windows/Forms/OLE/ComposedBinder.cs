// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection.Metadata;
using System.Runtime.Serialization;

namespace System.Windows.Forms;

internal sealed class ComposedBinder : SerializationBinder
{
    private readonly Func<TypeName, Type> _resolver;
    private readonly string _typeName;
    private readonly string _assemblyName;
    private readonly Type _type;
    private readonly bool _legacyMode;

    public ComposedBinder(Type type, Func<TypeName, Type> resolver, bool legacyMode)
    {
        _resolver = resolver.OrThrowIfNull();
        _type = type.OrThrowIfNull();
        _typeName = _type.FullName!;
        // Ignore version, culture, and public key token and compare the short names.
        _assemblyName = _type.Assembly.FullName!;
        _legacyMode = legacyMode;
    }

    public override Type? BindToType(string assemblyName, string typeName)
    {
        if (string.IsNullOrWhiteSpace(assemblyName))
        {
            throw new ArgumentException(nameof(assemblyName));
        }

        if (string.IsNullOrWhiteSpace(typeName))
        {
            throw new ArgumentException(nameof(typeName));
        }

        // Ignore version, culture, and public key token and compare the short names.
        assemblyName = assemblyName.Split(',')[0].Trim();

        if (string.Equals(_typeName, typeName, StringComparison.Ordinal)
            && string.Equals(_assemblyName, assemblyName, StringComparison.Ordinal))
        {
            return _type;
        }

        Type type = _resolver(TypeName.Parse($"{typeName}, {assemblyName}"));
        if (!_legacyMode && type is null)
        {
            throw new NotSupportedException($"'resolver' function provided in '{nameof(Clipboard.TryGetData)}'" +
                $" method should never return a null.  It should throw a '{nameof(NotSupportedException)}' when encountering unsupported types.");
        }

        return type;
    }
}
