// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.Serialization;

namespace System.Windows.Forms;

internal sealed class ComposedBinder : SerializationBinder
{
    private readonly Func<TypeName, Type> _resolver;
    private Dictionary<(string, string?), Type>? _nameToType;
    private readonly Type _type;

    public ComposedBinder(Type type, Func<TypeName, Type> resolver)
    {
        _resolver = resolver.OrThrowIfNull();
        _type = type.OrThrowIfNull();
        if (!TypeName.TryParse($"{type.FullName}, {type.Assembly.FullName}", out TypeName? parsed))
        {
            throw new InvalidOperationException();
        }
    }

    private static (string typeName, string assemblyName) GetNames(Type type)
    {
        if (!TypeName.TryParse($"{type.FullName}, {type.Assembly.FullName}", out TypeName? parsed))
        {
            throw new InvalidOperationException();
        }

        string typeName = parsed.FullName;
        // Ignore version, culture, and public key token and compare short names
        string assemblyName = parsed.AssemblyName!.Name;

        return (typeName, assemblyName);
    }

    [MemberNotNull(nameof(_nameToType))]
    private void Initialize()
    {
        if (_nameToType is object)
        {
            return;
        }

        (string typeName, string assemblyName) = GetNames(_type);
        _nameToType = new()
        {
            { (typeName, assemblyName), _type}
        };

        // this does not handle ISerializable
        BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public;
        var fields = _type.GetFields(flags);
        foreach (FieldInfo field in fields)
        {
            Type type = field.FieldType;
            (typeName, assemblyName) = GetNames(type);
            _nameToType.Add((typeName, assemblyName), type);
        }
    }

    public override Type? BindToType(string assemblyName, string typeName)
    {
        Initialize();

        if (!TypeName.TryParse($"{typeName}, {assemblyName}", out TypeName? parsed))
        {
            throw new InvalidOperationException();
        }

        if (parsed.AssemblyName is { } assembly)
        {
            // FullName is a namespace-qualified name.
            if (_nameToType.TryGetValue((parsed.FullName, assembly.Name), out Type? type))
            {
                return type;
            }
        }

        return _resolver(parsed);
    }
}
