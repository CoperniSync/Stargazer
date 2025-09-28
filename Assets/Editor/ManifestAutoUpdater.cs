// ChargerAstronomyLab/Stargazer/Assets/Editor/ManifestAutoUpdater.cs
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

[InitializeOnLoad]
static class ManifestAutoUpdater
{
    static readonly List<AddRequest> s_Requests = new List<AddRequest>();

    static ManifestAutoUpdater()
    {
        EditorApplication.delayCall += EnqueueLocalAdds;
        EditorApplication.update += ProgressRequests;
    }

    static void EnqueueLocalAdds()
    {
        EditorApplication.delayCall -= EnqueueLocalAdds;
        Debug.Log("[ManifestAutoUpdater] Adding local UPM packages to manifest...");

        foreach (var lib in WrapperGenerator.LoadEntries())
        {
            var localRef = $"file:GeneratedPackages/{lib.id}";
            Debug.Log($"[ManifestAutoUpdater] Enqueueing add for '{localRef}'");
            var req = Client.Add(localRef);
            s_Requests.Add(req);
        }
    }

    static void ProgressRequests()
    {
        if (s_Requests.Count == 0)
        {
            EditorApplication.update -= ProgressRequests;
            return;
        }

        for (int i = s_Requests.Count - 1; i >= 0; --i)
        {
            var req = s_Requests[i];
            if (!req.IsCompleted)
                continue;

            if (req.Status == StatusCode.Success && req.Result != null)
            {
                var info = req.Result;
                Debug.Log($"[ManifestAutoUpdater] Successfully added {info.name}@{info.version}");
            }
            else
            {
                // req.Error is guaranteed non-null on failure
                var pkgRef = req.Result.packageId ?? "(unknown)";
                var msg = req.Error != null ? req.Error.message : "No error message";
                Debug.LogError($"[ManifestAutoUpdater] Failed to add '{pkgRef}': {msg}");
            }

            s_Requests.RemoveAt(i);
        }
    }
}