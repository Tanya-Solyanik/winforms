// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Windows.Forms.Analyzers;

namespace System.Windows.Forms.CSharp.Analyzers.ImplementITypedDataObjectInAdditionToIDataObject;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ImplementITypedDataObjectInAdditionToIDataObjectAnalyzer : DiagnosticAnalyzer
{
    private const string IDataObject = nameof(IDataObject);
    private const string ITypedDataObject = nameof(ITypedDataObject);
    private const string Namespace = "System.Windows.Forms";

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        [CSharpDiagnosticDescriptors.s_implementITypedDataObjectInAdditionToIDataObject];

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.ClassDeclaration);
    }

    private void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not ClassDeclarationSyntax classDeclaration)
        {
            return;
        }

        if (classDeclaration.BaseList is not BaseListSyntax bases)
        {
            return;
        }

        var semanticModel = context.SemanticModel;
        var compilation = semanticModel.Compilation;

        if (!Utilities.IsNet100OrAbove(compilation))
        {
            return;
        }

        bool assemblyFound = compilation.ReferencedAssemblyNames
            .Any(assembly => assembly.Name == Namespace);

        if (!assemblyFound)
        {
            return;
        }

        bool foundIDataObject = false;
        foreach (BaseTypeSyntax baseType in bases.Types)
        {
            if (baseType is not SimpleBaseTypeSyntax simpleBaseType)
            {
                continue;
            }

            TypeSyntax type = simpleBaseType.Type;
            TypeInfo typeInfo = semanticModel.GetTypeInfo(type);
            if (typeInfo.Type is INamedTypeSymbol namedTypeSymbol
                && namedTypeSymbol.ContainingNamespace.ToDisplayString() == Namespace
                && IsTypedDataObjectImplemented(namedTypeSymbol, ref foundIDataObject))
            {
                return;
            }
        }

        if (!foundIDataObject)
        {
            return;
        }

        // Report the warning if IDataObject is in the base list but ITypedDataObject is not.
        var diagnostic = Diagnostic.Create(
            CSharpDiagnosticDescriptors.s_implementITypedDataObjectInAdditionToIDataObject,
            classDeclaration.Identifier.GetLocation());
        context.ReportDiagnostic(diagnostic);

        static bool IsTypedDataObjectImplemented(INamedTypeSymbol typeSymbol, ref bool foundIDataObject)
        {
            string typeName = typeSymbol.Name;
            if (!foundIDataObject && typeName == IDataObject)
            {
                foundIDataObject = true;
                return false;
            }

            return typeName == ITypedDataObject;
        }
    }
}
