using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

static class WrapperGenerator
{
    // Where we emit UPM wrappers under Packages/
    const string GeneratedRoot = "Packages/GeneratedPackages";
    // Drives which DLLs to wrap + their remote URLs
    const string LibrariesJson = "Assets/Editor/Libraries.json";
    const string ManifestPath = "Packages/manifest.json";


    // ----- Data types -----

    [Serializable]
    public class LibEntry
    {
        public string id;
        public string dllPath;
        public string remoteRef;
    }

    [Serializable]
    class WrapperList { public LibEntry[] items; }

    [Serializable]
    class ManifestData
    {
        // JsonUtility can only serialize fields, not properties
        public Dictionary<string, string> dependencies = new Dictionary<string, string>();
    }

    [Serializable]
    class PackageJson
    {
        public string name;
        public string version;
        public string displayName;
        public string unity;
        public string description;
    }

    // ----- Load configuration -----

    public static LibEntry[] LoadEntries()
    {
        var txt = File.ReadAllText(LibrariesJson);
        // wrap in an object so JsonUtility can parse arrays
        return JsonUtility
            .FromJson<WrapperList>($"{{\"items\":{txt}}}")
            .items;
    }

    // ----- Manifest mutation -----

    // call this with useLocal=true or false
    public static void UpdateManifest(bool useLocal)
    {
        // 1. Load the entire manifest.json text
        var fullPath = Path.Combine(Application.dataPath, "../", ManifestPath);
        var text = File.ReadAllText(fullPath);

        // 2. Extract the existing dependencies block
        //    We capture everything from the opening { to the matching }.
        var depsPattern = new Regex(
            @"(""dependencies""\s*:\s*)\{(?<body>[\s\S]*?)\}",
            RegexOptions.Multiline);
        var match = depsPattern.Match(text);
        if (!match.Success)
        {
            UnityEngine.Debug.LogError("[WrapperGenerator] Could not find a dependencies block in manifest.json");
            return;
        }

        var prefix = match.Groups[1].Value;       // the '"dependencies": ' part
        var existingBody = match.Groups["body"].Value;  // everything inside the { … }

        // 3. Parse out existing entries into a dictionary
        var deps = new Dictionary<string, string>();
        var linePattern = new Regex(@"""(?<key>[^""]+)""\s*:\s*""(?<val>[^""]+)""");
        foreach (Match line in linePattern.Matches(existingBody))
        {
            deps[line.Groups["key"].Value] = line.Groups["val"].Value;
        }

        // 4. Inject or override with your CoperniSync entries
        foreach (var lib in LoadEntries())
        {
            var reference = useLocal
                ? $"file:GeneratedPackages/{lib.id}"
                : lib.remoteRef;
            deps[lib.id] = reference;
        }

        // 5. Rebuild the dependencies JSON block
        var sb = new StringBuilder();
        sb.Append(prefix).AppendLine("{");
        int count = 0, total = deps.Count;
        foreach (var kv in deps)
        {
            count++;
            var comma = count < total ? "," : "";
            sb.AppendFormat("    \"{0}\": \"{1}\"{2}\n", kv.Key, kv.Value, comma);
        }
        sb.Append("}");

        // 6. Replace the old block with the new one
        var updated = depsPattern.Replace(text, sb.ToString());

        // 7. Write it back
        File.WriteAllText(fullPath, updated);
        UnityEngine.Debug.Log($"[WrapperGenerator] manifest.json merged (useLocal={useLocal})");
    }

   
    // ----- Build / Wrap logic -----

    public static bool NeedsWrapOrBuild()
    {
        foreach (var lib in LoadEntries())
        {
            var src = Path.GetFullPath(Path.Combine(
                Application.dataPath, "../" + lib.dllPath));
            var dst = Path.Combine(
                GeneratedRoot, lib.id,
                "Runtime", "lib", "netstandard2.1",
                Path.GetFileName(src));

            if (!File.Exists(dst)) return true;
            if (File.GetLastWriteTimeUtc(src) > File.GetLastWriteTimeUtc(dst))
                return true;
        }
        return false;
    }

 


    // Inside WrapperGenerator.cs
    public static void WrapChangedLibraries()
    {
        foreach (var lib in LoadEntries())
        {
            // Find the build output folder
            var dllFull = Path.GetFullPath(
                Path.Combine(Application.dataPath, "../" + lib.dllPath));
            var binDir = Path.GetDirectoryName(dllFull); // …/bin/Debug/netstandard2.1

            if (!Directory.Exists(binDir))
            {
                UnityEngine.Debug.LogWarning($"[Wrapper] Build folder not found for {lib.id}: {binDir}");
                continue;
            }

            // Prepare the UPM package runtime folder
            var pkgRoot = Path.Combine(GeneratedRoot, lib.id);
            var runtime = Path.Combine(pkgRoot, "Runtime", "lib", "netstandard2.1");
            Directory.CreateDirectory(runtime);

            // Copy every DLL (engine + all its NuGet deps)
            foreach (var srcFile in Directory.GetFiles(binDir, "*.dll"))
            {
                var dest = Path.Combine(runtime, Path.GetFileName(srcFile));
                var srcTime = File.GetLastWriteTimeUtc(srcFile);
                var dstTime = File.Exists(dest)
                    ? File.GetLastWriteTimeUtc(dest)
                    : DateTime.MinValue;

                if (srcTime <= dstTime)
                    continue;

                File.Copy(srcFile, dest, overwrite: true);
                UnityEngine.Debug.Log($"[Wrapper] Copied {Path.GetFileName(srcFile)} for {lib.id}");
            }

            // Ensure package.json is emitted at pkgRoot
            WritePackageJson(lib, pkgRoot);
        }
    }

    static void WritePackageJson(LibEntry lib, string pkgRoot)
    {
        // Extract major.minor from Application.unityVersion
        // e.g. "2021.3.5f1" → "2021.3"
        var match = Regex.Match(Application.unityVersion, @"^(?<maj>\d+)\.(?<min>\d+)");
        var unityVer = match.Success
            ? $"{match.Groups["maj"].Value}.{match.Groups["min"].Value}"
            : "0.0";

        var pkg = new PackageJson
        {
            name = lib.id,
            version = "0.1.0",
            displayName = lib.id,
            unity = unityVer,
            description = "Auto-wrapped local DLL"
        };

        var json = JsonUtility.ToJson(pkg, prettyPrint: true);
        File.WriteAllText(Path.Combine(pkgRoot, "package.json"), json);
    }


    public static void CleanGeneratedPackagesRuntime()
    {
        foreach (var lib in LoadEntries())
        {
            var runtime = Path.Combine(GeneratedRoot, lib.id, "Runtime");
            if (Directory.Exists(runtime))
                Directory.Delete(runtime, true);
        }
    }
}