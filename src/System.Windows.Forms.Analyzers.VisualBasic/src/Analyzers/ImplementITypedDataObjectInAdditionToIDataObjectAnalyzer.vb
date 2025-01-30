' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Collections.Immutable
Imports System.Windows.Forms.Analyzers
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Diagnostics
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax

Namespace System.Windows.Forms.VisualBasic.Analyzers.ImplementITypedDataObjectInAdditionToIDataObject

    <DiagnosticAnalyzer(LanguageNames.VisualBasic)>
    Public NotInheritable Class ImplementITypedDataObjectInAdditionToIDataObjectAnalyzer
        Inherits DiagnosticAnalyzer

        Private Const IDataObject As String = NameOf(IDataObject)
        Private Const ITypedDataObject As String = NameOf(ITypedDataObject)
        Private Const NamespaceName As String = "System.Windows.Forms"

        Public Overrides ReadOnly Property SupportedDiagnostics As ImmutableArray(Of DiagnosticDescriptor) _
            = ImmutableArray.Create(s_implementITypedDataObjectInAdditionToIDataObject)

        Public Overrides Sub Initialize(context As AnalysisContext)
            context.EnableConcurrentExecution()
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None)
            context.RegisterSyntaxNodeAction(AddressOf AnalyzeClassBlock, SyntaxKind.ClassBlock)
        End Sub

        Private Sub AnalyzeClassBlock(context As SyntaxNodeAnalysisContext)
            Dim classBlock = TryCast(context.Node, ClassBlockSyntax)
            If classBlock Is Nothing Then
                Return
            End If

            If classBlock.Implements.Count = 0 Then
                Return
            End If

            Dim semanticModel = context.SemanticModel
            Dim compilation = semanticModel.Compilation

            ' ITypedDataObject was introduced in NET10.0.
            If Not compilation.IsNet100OrAbove() Then
                Return
            End If

            ' Check if the System.Windows.Forms assembly is referenced
            If Not compilation.ReferencedAssemblyNames.Any(Function(assembly) assembly.Name = NamespaceName) Then
                Return
            End If

            Dim classSymbol = semanticModel.GetDeclaredSymbol(classBlock)
            If classSymbol Is Nothing Then
                Return
            End If

            Dim implementsITypedDataObject = classSymbol.AllInterfaces _
                .Any(Function(i) i.ContainingNamespace.ToDisplayString() = NamespaceName AndAlso i.Name = ITypedDataObject)
            If implementsITypedDataObject Then
                Return
            End If

            Dim implementsIDataObject = classSymbol.AllInterfaces _
                .Any(Function(i) i.ContainingNamespace.ToDisplayString() = NamespaceName AndAlso i.Name = IDataObject)
            If Not implementsIDataObject Then
                Return
            End If

            ' Report if it implements IDataObject but NOT ITypedDataObject.
            Dim diagnostic As Diagnostic = Diagnostic.Create(
                s_implementITypedDataObjectInAdditionToIDataObject,
                classBlock.BlockStatement.Identifier.GetLocation())

            context.ReportDiagnostic(diagnostic)
        End Sub
    End Class

End Namespace
