// ChargerAstronomyLab/Stargazer/Assets/Editor/WrapperMenu.cs
using UnityEditor;
using UnityEngine;

static class WrapperMenu
{
    [MenuItem("Tools/CoperniSync/Refresh Local Packages")]
    static void RefreshLocalPackages()
    {
        Debug.Log("[WrapperMenu] Refreshing CoperniSync local packages…");

        // 1. Clean out only the old Runtime/ folders
        WrapperGenerator.CleanGeneratedPackagesRuntime();

        // 2. Rebuild & wrap changed DLLs (dotnet build is incremental)
        if (WrapperGenerator.NeedsWrapOrBuild())
        {
            WrapperGenerator.WrapChangedLibraries();
            Debug.Log("[WrapperMenu] Built & wrapped updated libraries.");
        }
        else
        {
            Debug.Log("[WrapperMenu] No changes detected; skipping wrap.");
        }

        // 3. Update Packages/manifest.json to point at local file: refs
        WrapperGenerator.UpdateManifest(useLocal: true);
        Debug.Log("[WrapperMenu] manifest.json now references local packages.");

        // 4. Refresh so Unity picks up the new DLLs and updated manifest
        AssetDatabase.Refresh();
        Debug.Log("[WrapperMenu] Local UPM wrappers are up to date.");
    }
}