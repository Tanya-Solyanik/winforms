// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;
using Microsoft.CodeAnalysis.Testing;
using Xunit;

namespace System.Windows.Forms.Analyzers.Tests;

public static class CurrentReferences
{
    public static string? Tfm { get; }
    public static string? NetCoreRefsVersion { get; }
    public static ReferenceAssemblies? ReferenceAssemblies { get; }
    public static string? WinFormsRefPath { get; }
    public static string? RepoRootPath { get; }

    private const string RefPackageName = "Microsoft.NETCore.App.Ref";

    static CurrentReferences()
    {
        if (!GetRootFolderPath(out string? rootFolderPath))
        {
            Assert.NotNull(rootFolderPath);
            return;
        }

        RepoRootPath = rootFolderPath;

        if (!TryGetNetCoreVersion(rootFolderPath, out string? tfm, out string? netCoreRefsVersion))
        {
            Assert.NotNull(tfm);
            Assert.NotNull(netCoreRefsVersion);
            return;
        }

        Tfm = tfm;

        string configuration =
#if DEBUG
            "Debug";
#else
            "Release";
#endif

        WinFormsRefPath = $@"{RepoRootPath}\artifacts\obj\System.Windows.Forms\{configuration}\{tfm}\ref\System.Windows.Forms.dll";
        Assert.True(File.Exists(WinFormsRefPath));

        // Specify absolute path to the reference assemblies because this version is not necessarily available in the nuget packages cache.
        string netCoreAppRefPath = Path.Combine(RepoRootPath, ".dotnet", "packs", RefPackageName);
        if (!Directory.Exists(Path.Combine(netCoreAppRefPath, netCoreRefsVersion)))
        {
            netCoreRefsVersion = GetAvailableVersion(netCoreAppRefPath, $"{netCoreRefsVersion.Split('.')[0]}.");
        }

        netCoreAppRefPath = Path.Combine(netCoreAppRefPath, netCoreRefsVersion, "ref", tfm);
        Assert.True(Directory.Exists(netCoreAppRefPath));

        NetCoreRefsVersion = netCoreRefsVersion;

        // Create ReferenceAssemblies from the specified path.
        ReferenceAssemblies = new ReferenceAssemblies(
            tfm,
            new PackageIdentity(RefPackageName, netCoreRefsVersion),
            Path.Combine("ref", tfm));
              // .WithNuGetConfigFilePath = Path.Combine(RepoRootPath, "NuGet.Config");
        // https://github.com/dotnet/aspnetcore/blob/029978cdc937b96da9b38e5ba3cf658da636b541/src/Framework/AspNetCoreAnalyzers/test/Verifiers/CSharpAnalyzerVerifier.cs#L49-#L62
    }

    private static string GetAvailableVersion(string netCoreAppRefPath, string major)
    {
        string[] versions = Directory.GetDirectories(netCoreAppRefPath);
        string? availableVersion = versions.FirstOrDefault(v =>
            Path.GetFileName(v).StartsWith(major, StringComparison.InvariantCultureIgnoreCase));
        Assert.NotNull(availableVersion);
        return availableVersion!;
    }

    private static bool TryGetNetCoreVersion(
        string rootFolderPath,
        [NotNullWhen(true)] out string? tfm,
        [NotNullWhen(true)] out string? netCoreRefsVersion)
    {
        tfm = default;
        netCoreRefsVersion = default;

        if (!TryGetSdkVersion(rootFolderPath, out string? version))
        {
            return false;
        }

        // First, try to use the local .NET SDK if it's there.
        string sdkFolderPath = Path.Combine(rootFolderPath, ".dotnet", "sdk", version);
        if (!Directory.Exists(sdkFolderPath))
        {
            return false;
        }

        return TryGetNetCoreVersionFromJson(sdkFolderPath, out tfm, out netCoreRefsVersion);
    }

    private static bool GetRootFolderPath([NotNullWhen(true)] out string? root, [CallerFilePath] string filePath = "")
    {
        root = default;
        // We walk the parent folder structure until we find our global.json.
        string? currentFolderPath = Path.GetDirectoryName(filePath);

        while (currentFolderPath is not null)
        {
            string globalJsonPath = Path.Combine(currentFolderPath, "global.json");
            if (File.Exists(globalJsonPath))
            {
                // We've found the repo root.
                root = currentFolderPath;
                return true;
            }

            currentFolderPath = Path.GetDirectoryName(currentFolderPath);
        }

        // Either CallerPathAttribute is didn't give us the path or global.json file had disappeared.
        return false;
    }

    private static bool TryGetSdkVersion(string rootFolderPath, [NotNullWhen(true)] out string? version)
    {
        string globalJsonPath = Path.Combine(rootFolderPath, "global.json");
        string globalJsonString = File.ReadAllText(globalJsonPath);
        JsonObject? jsonObject = JsonNode.Parse(globalJsonString)?.AsObject();
        version = (string?)jsonObject?["sdk"]?["version"];

        return version is not null;
    }

    private static bool TryGetNetCoreVersionFromJson(string sdkFolderPath, [NotNullWhen(true)] out string? tfm, [NotNullWhen(true)] out string? version)
    {
        string configJsonPath = Path.Combine(sdkFolderPath, "dotnet.runtimeconfig.json");
        string configJsonString = File.ReadAllText(configJsonPath);
        JsonObject? jsonObject = JsonNode.Parse(configJsonString)?.AsObject();
        JsonNode? runtimeOptions = jsonObject?["runtimeOptions"];
        tfm = (string?)runtimeOptions?["tfm"];
        if (tfm is null)
        {
            version = default;
            return false;
        }

        version = (string?)runtimeOptions?["framework"]?["version"];
        if (version is null)
        {
            tfm = null;
            return false;
        }

        return true;
    }
}
