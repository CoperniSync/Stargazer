using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
static class WrapperOnProjectLoad
{
    static WrapperOnProjectLoad()
    {
        // Run once after the Editor finishes compiling & loading
        EditorApplication.delayCall += OnProjectLoad;
    }

    static void OnProjectLoad()
    {
        EditorApplication.delayCall -= OnProjectLoad;
        Debug.Log("[WrapperOnProjectLoad] Populating UPM wrappers…");

        WrapperGenerator.CleanGeneratedPackagesRuntime();

        if (WrapperGenerator.NeedsWrapOrBuild())
        {
            WrapperGenerator.WrapChangedLibraries();
        }
        else
        {
            Debug.Log("[WrapperOnProjectLoad] No changes detected; skipping wrap.");
        }

        AssetDatabase.Refresh();
        Debug.Log("[WrapperOnProjectLoad] Done.");
    }
}