// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using System.Windows.Forms.Analyzers.Diagnostics;
using System.Windows.Forms.Analyzers.Tests;
using System.Windows.Forms.CSharp.Analyzers.ImplementITypedDataObjectInAdditionToIDataObject;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;

namespace System.Windows.Forms.Analyzers.CSharp.Tests;

public sealed class ImplementITypedDataObjectInAdditionToIDataObjectAnalyzerTests
{
    private const string DiagnosticId = DiagnosticIDs.ImplementITypedDataObjectInAdditionToIDataObject;

    [Fact]
    public async Task UntypedInterface()
    {
        // internal class UntypedInterface :IDataObject
        string input = await GetTestCodeAsync();
        await RaiseTheWarning(input, [DiagnosticResult.CompilerWarning(DiagnosticId).WithSpan(10, 16, 10, 16 + nameof(UntypedInterface).Length)]);
    }

    [Fact]
    public async Task UntypedWithAlias()
    {
        // internal class UntypedWithAlias : IManagedDataObject
        string input = await GetTestCodeAsync();
        await RaiseTheWarning(input, [DiagnosticResult.CompilerWarning(DiagnosticId).WithSpan(10, 16, 10, 16 + nameof(UntypedWithAlias).Length)]);
    }

    [Fact]
    public async Task UntypedWithNamespace()
    {
        // internal class UntypedWithNamespace :Forms.IDataObject
        string input = await GetTestCodeAsync();
        await RaiseTheWarning(input, [DiagnosticResult.CompilerWarning(DiagnosticId).WithSpan(10, 16, 10, 16 + nameof(UntypedWithNamespace).Length)]);
    }

    [Fact]
    public async Task UntypedUnimplemented()
    {
        // internal class UntypedUnimplemented :IDataObject
        string input = await GetTestCodeAsync();
        await RaiseTheWarning(input,
            [
                DiagnosticResult.CompilerWarning(DiagnosticId).WithSpan(10, 16, 10, 16 + nameof(UntypedUnimplemented).Length),
                DiagnosticResult.CompilerError("CS0535")
                    .WithSpan(10, 39, 10, 39 + "IDataObject".Length)
                    .WithArguments($"System.Windows.Forms.Analyzers.CSharp.Tests.{nameof(UntypedUnimplemented)}", "System.Windows.Forms.IDataObject.GetData(string, bool)"),
            ]);
    }

    [Fact]
    public async Task Downlevel()
    {
        // Targeting NET9.
        string input = await GetTestCodeAsync();
        await NoWarning(input);
    }

    [Fact]
    public async Task NoTargetAttribute()
    {
        string input = await GetTestCodeAsync();
        await NoWarning(input);
    }

    [Fact]
    public async Task TypedInterface()
    {
        // internal class TypedInterface :ITypedDataObject
        string input = await GetTestCodeAsync();
        await NoWarning(input);
    }

    [Fact]
    public async Task TypedWithNamespace()
    {
        // internal class TypedWithNamespace : Forms.ITypedDataObject
        string input = await GetTestCodeAsync();
        await NoWarning(input);
    }

    [Fact]
    public async Task TypedWithAlias()
    {
        // internal class TypedWithAlias : IManagedDataObject, System.Windows.Forms.IDataObject
        string input = await GetTestCodeAsync();
        await NoWarning(input);
    }

    [Fact]
    public async Task TwoInterfaces()
    {
        // internal class TwoInterfaces :IDataObject, ITypedDataObject
        string input = await GetTestCodeAsync();
        await NoWarning(input);
    }

    [Fact]
    public async Task UnrelatedIDataObject()
    {
        // Name collision, this analyzer is not applicable
        string input = await GetTestCodeAsync();
        await NoWarning(input);
    }

    private static async Task RaiseTheWarning(string input, List<DiagnosticResult> diagnostics)
    {
        var context = CreateContext(input);
        context.TestState.ExpectedDiagnostics.AddRange(diagnostics);

        await context.RunAsync().ConfigureAwait(false);
    }

    private static async Task NoWarning(string input) => await CreateContext(input).RunAsync().ConfigureAwait(false);

    private static CSharpAnalyzerTest<ImplementITypedDataObjectInAdditionToIDataObjectAnalyzer, DefaultVerifier> CreateContext(string input)
    {
        ReferenceAssemblies netCore = new ReferenceAssemblies(
            "net10.0",
            new PackageIdentity("Microsoft.NETCore.App.Ref", "10.0.0-alpha.1.25073.13"),
            Path.Combine("ref", "net10.0"));
        ReferenceAssemblies referenceAssemblies = netCore
            .AddPackages([new PackageIdentity("Microsoft.WindowsDesktop.App.Ref", "10.0.0-alpha.1.25066.2")]);

        CSharpAnalyzerTest<ImplementITypedDataObjectInAdditionToIDataObjectAnalyzer, DefaultVerifier> context = new()
        {
            TestCode = input,
            TestState =
            {
                OutputKind = OutputKind.DynamicallyLinkedLibrary,
            },
            ReferenceAssemblies = referenceAssemblies
        };

        return context;
    }

    private static async Task<string> GetTestCodeAsync(
        [CallerMemberName] string testName = "",
        [CallerFilePath] string filePath = "")
    {
        string toolName = Path.GetFileName(Path.GetDirectoryName(filePath))!;
        return await TestFileLoader.LoadTestFileAsync(toolName, testName).ConfigureAwait(false);
    }
}
