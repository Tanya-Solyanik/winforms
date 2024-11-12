// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Immutable;
using System.Reflection.Metadata;

namespace System.Windows.Forms;
public unsafe partial class DataObject
{
    internal unsafe partial class Composition
    {
        internal class TypeNameComparer : IEqualityComparer<TypeName>
        {
            private TypeNameComparer()
            {
            }

            internal static IEqualityComparer<TypeName> Default { get; } = new TypeNameComparer();

            public bool Equals(TypeName? x, TypeName? y)
            {
                if (x is null && y is null)
                {
                    return true;
                }

                if (x is null || y is null)
                {
                    return false;
                }

                return Matches(x, y);
            }

            public int GetHashCode(TypeName obj)
            {
                if (obj is null)
                {
                    return 0;
                }

                if (obj.IsArray)
                {
                    return HashCode.Combine(true, obj.GetArrayRank(), obj.GetElementType());
                }

                if (obj.IsConstructedGenericType)
                {
                    int hashCode = HashCode.Combine("constructed", obj.GetGenericTypeDefinition());
                    foreach (TypeName genericName in obj.GetGenericArguments())
                    {
                        hashCode ^= GetHashCode(genericName);
                    }

                    return hashCode;
                }

                return obj.FullName.GetHashCode();
            }

            // Based on https://github.com/dotnet/runtime/blob/5d69e2dca30524a93b00cd613be218144b5f95d1/src/libraries/System.Formats.Nrbf/src/System/Formats/Nrbf/SerializationRecord.cs#L54
            private static bool Matches(TypeName x, TypeName y)
            {
                if (x.IsArray != y.IsArray
                    || x.IsConstructedGenericType != y.IsConstructedGenericType
                    || x.IsNested != y.IsNested
                    || (x.IsArray && x.GetArrayRank() != y.GetArrayRank())
                    || x.IsSZArray != y.IsSZArray // int[] vs int[*]
                    )
                {
                    return false;
                }

                if (x.FullName == y.FullName)
                {
                    return true;
                }

                if (y.IsArray)
                {
                    return Matches(x.GetElementType(), y.GetElementType());
                }

                if (x.IsConstructedGenericType)
                {
                    if (!Matches(x.GetGenericTypeDefinition(), y.GetGenericTypeDefinition()))
                    {
                        return false;
                    }

                    ImmutableArray<TypeName> genericNames2 = y.GetGenericArguments();
                    ImmutableArray<TypeName> genericNames1 = x.GetGenericArguments();

                    if (genericNames1.Length != genericNames2.Length)
                    {
                        return false;
                    }

                    for (int i = 0; i < genericNames1.Length; i++)
                    {
                        if (!Matches(genericNames1[i], genericNames2[i]))
                        {
                            return false;
                        }
                    }

                    return true;
                }

                return false;
            }
        }
    }
}
