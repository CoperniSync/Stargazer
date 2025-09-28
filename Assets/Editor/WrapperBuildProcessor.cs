using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using System.IO;
using UnityEngine;

class WrapperBuildProcessor : IPostprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPostprocessBuild(BuildReport report)
    {
        Debug.Log("[WrapperBuildProcessor] Reverting to remote package refs…");

        var manifestPath = "Packages/manifest.json";
        var manifestText = File.ReadAllText(manifestPath);

        foreach (var lib in WrapperGenerator.LoadEntries())
        {
            var localRef = $"file:GeneratedPackages/{lib.id}";
            manifestText = manifestText.Replace(localRef, lib.remoteRef);
        }

        File.WriteAllText(manifestPath, manifestText);

        WrapperGenerator.CleanGeneratedPackagesRuntime();
        AssetDatabase.ImportAsset("Packages/GeneratedPackages", ImportAssetOptions.ForceUpdate);
        Debug.Log("[WrapperBuildProcessor] Done.");
    }
}