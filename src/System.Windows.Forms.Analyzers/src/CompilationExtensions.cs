// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Versioning;
using Microsoft.CodeAnalysis;

namespace System.Windows.Forms.Analyzers
{
    public static class CompilationExtensions
    {
        public static bool IsNet100OrAbove(this Compilation compilation) =>
            IsSameOrAbove(compilation, ".NETCoreApp,Version=v10.0");

        public static bool IsNet90OrAbove(this Compilation compilation) =>
            IsSameOrAbove(compilation, ".NETCoreApp,Version=v9.0");

        private static bool IsSameOrAbove(Compilation compilation, string expectedFramework)
        {
            if (GetFrameworkName(compilation) is not FrameworkName name)
            {
                return false;
            }

            FrameworkName expected = new(expectedFramework);
            return name.Identifier == expected.Identifier && name.Version >= expected.Version;
        }

        private static FrameworkName? GetFrameworkName(Compilation compilation)
        {
            var targetFrameworkAttribute = compilation.Assembly
                .GetAttributes()
                .FirstOrDefault(attr => attr.AttributeClass?.Name == "TargetFrameworkAttribute");

            if (targetFrameworkAttribute is null)
            {
                return null;
            }

            if (targetFrameworkAttribute.ConstructorArguments.FirstOrDefault().Value is not string frameworkName)
            {
                return null;
            }

            return new(frameworkName);
        }
    }
}
