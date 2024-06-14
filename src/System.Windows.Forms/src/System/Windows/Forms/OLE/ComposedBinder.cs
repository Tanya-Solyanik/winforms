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
        _typeName = type.FullName!;
        _assemblyName = type.Assembly.FullName!;
    }

    public override Type? BindToType(string assemblyName, string typeName)
    {
        if (!TypeName.TryParse($"{typeName}, {assemblyName}".AsSpan(), out TypeName? parsed))
        {
            throw new InvalidOperationException();
        }

        if (parsed.FullName == _typeName && parsed.AssemblyName?.FullName == _assemblyName)
        {
            return _type;
        }

        return _resolver(parsed);
    }
}
