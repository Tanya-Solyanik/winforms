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

    public ComposedBinder(Type type, Func<TypeName, Type> resolver)
    {
        _resolver = resolver.OrThrowIfNull();
        _type = type.OrThrowIfNull();
        if (!TypeName.TryParse($"{type.FullName}, {type.Assembly.FullName}", out TypeName? parsed))
        {
            throw new InvalidOperationException();
        }

        _typeName = parsed.FullName;
        // Ignore version, culture, and public key token and compare short names
        _assemblyName = parsed.AssemblyName!.Name;
    }

    public override Type? BindToType(string assemblyName, string typeName)
    {
        if (!TypeName.TryParse($"{typeName}, {assemblyName}", out TypeName? parsed))
        {
            throw new InvalidOperationException();
        }

        if (parsed.AssemblyName is { } assembly)
        {
            // FullName is a namespace-qualified name.
            if (_typeName == parsed.FullName && _assemblyName == assembly.Name)
            {
                return _type;
            }
        }

        return _resolver(parsed);
    }
}
