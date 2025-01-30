// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Windows.Forms.Analyzers;
using System.Windows.Forms.CSharp.Analyzers.Diagnostics;

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
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterSyntaxNodeAction(AnalyzeClassDeclaration, SyntaxKind.ClassDeclaration);
    }

    private void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not ClassDeclarationSyntax classDeclaration)
        {
            return;
        }

        if (classDeclaration.BaseList is null)
        {
            return;
        }

        var semanticModel = context.SemanticModel;
        var compilation = semanticModel.Compilation;

        // ITypedDataObject was introduced in NET10.0.
        if (!compilation.IsNet100OrAbove())
        {
            return;
        }

        // Check if the System.Windows.Forms assembly is referenced
        if (!compilation.ReferencedAssemblyNames.Any(assembly => assembly.Name == Namespace))
        {
            return;
        }

        var classSymbol = semanticModel.GetDeclaredSymbol(classDeclaration);
        if (classSymbol is null)
        {
            return;
        }

        bool implementsITypedDataObject = classSymbol.AllInterfaces
           .Any(i => i.ContainingNamespace.ToDisplayString() == Namespace && i.Name == ITypedDataObject);
        if (implementsITypedDataObject)
        {
            return;
        }

        bool implementsIDataObject = classSymbol.AllInterfaces
            .Any(i => i.ContainingNamespace.ToDisplayString() == Namespace && i.Name == IDataObject);
        if (!implementsIDataObject)
        {
            return;
        }

        // Report if it implements IDataObject but NOT ITypedDataObject.
        var diagnostic = Diagnostic.Create(
            CSharpDiagnosticDescriptors.s_implementITypedDataObjectInAdditionToIDataObject,
            classDeclaration.Identifier.GetLocation());

        context.ReportDiagnostic(diagnostic);
    }
}
