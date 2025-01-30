// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;

namespace System.Windows.Forms.Analyzers.Tests;

public static class CurrentReferences
{
    public static string GetNetCoreRefVersion()
    {
        return "10.0.0-alpha.1.25073.13";
    }

    public static string GetDesktopRefVersion()
    {
        return "10.0.0-alpha.1.25073.1";
    }

    public static string GetDotNetVersion()
    {
        return "net10.0";
    }

    public static bool TryGetSdkPath([NotNullWhen(true)] out string? sdkFolderPath)
    {
        sdkFolderPath = default;
        string rootFolderPath = GetRootFolderPath();

        if (!TryGetSdkVersion(rootFolderPath, out string? version))
        {
            return false;
        }

        // First, try to use the local .NET SDK if it's there.
        sdkFolderPath = Path.Combine(rootFolderPath, ".dotnet", "sdk", version);

        if (Directory.Exists(sdkFolderPath))
        {
            return true;
        }

        sdkFolderPath = default;
        return false;
    }

    public static string GetRootFolderPath([CallerFilePath] string filePath = "")
    {
        // We walk the parent folder structure until we find our global.json.
        string? currentFolderPath = Path.GetDirectoryName(filePath);

        while (currentFolderPath is not null)
        {
            string globalJsonPath = Path.Combine(currentFolderPath, "global.json");
            if (File.Exists(globalJsonPath))
            {
                // We've found the repo root.
                return currentFolderPath;
            }

            currentFolderPath = Path.GetDirectoryName(currentFolderPath);
        }

        throw new InvalidOperationException("Either CallerPathAttribute is didn't give us the path or global.json file had disappeared.");
    }

    public static bool TryGetSdkVersion(string rootFolderPath, [NotNullWhen(true)] out string? version)
    {
        string globalJsonPath = Path.Combine(rootFolderPath, "global.json");
        string globalJsonString = File.ReadAllText(globalJsonPath);
        JsonObject? jsonObject = JsonNode.Parse(globalJsonString)?.AsObject();
        version = (string?)jsonObject?["sdk"]?["version"];

        return version is not null;
    }
}
