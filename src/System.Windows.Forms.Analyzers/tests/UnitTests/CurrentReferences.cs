// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;
using Microsoft.CodeAnalysis.Testing;

namespace System.Windows.Forms.Analyzers.Tests;

public static class CurrentReferences
{
    public static string? Tfm { get; }
    public static string? NetCoreRefsVersion { get; }
    public static ReferenceAssemblies? ReferenceAssemblies { get; }
    public static string? WinFormsRefPath { get; }

    static CurrentReferences()
    {
        if (!TryGetNetCoreVersion(out string? tfm, out string? netCoreRefsVersion))
        {
            return;
        }

        Tfm = tfm;
        NetCoreRefsVersion = netCoreRefsVersion;

        string configuration =
#if DEBUG
            "Debug";
#else
            "Release";
#endif

        WinFormsRefPath = $@"..\..\..\..\..\artifacts\obj\System.Windows.Forms\{configuration}\{tfm}\ref\System.Windows.Forms.dll";

        // Specify the absolute paths to the reference assemblies because this version is not necessarily available in the nuget packages cache.
        string netCoreAppRefPath = $@"..\..\..\..\..\.dotnet\packs\Microsoft.NETCore.App.Ref\{netCoreRefsVersion}\ref\{tfm}";

        // Create ReferenceAssemblies from the specified path.
        ReferenceAssemblies = new ReferenceAssemblies(
            tfm,
            new PackageIdentity("Microsoft.NETCore.App.Ref", netCoreRefsVersion),
            netCoreAppRefPath);
    }

    private static bool TryGetNetCoreVersion(
        [NotNullWhen(true)] out string? tfm,
        [NotNullWhen(true)] out string? netCoreRefsVersion)
    {
        tfm = default;
        netCoreRefsVersion = default;

        if (!GetRootFolderPath(out string? rootFolderPath))
        {
            return false;
        }

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
