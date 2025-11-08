using Assets.Scripts.CelestialBodies;
using ChargerAstronomyEngine.Data;
using ChargerAstronomyEngine.Data.Messier;
using ChargerAstronomyShared.Contracts.Repositories;
using ChargerAstronomyShared.Domain.Equatorial;
using ChargerAstronomyShared.Domain.Horizontal;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// Author: Morgan Hendon FA 2025
public class MessierRetrieval
{
    /// <summary>
    /// Gets the messier object data from the file 
    /// </summary>
    /// <param name="filename">
    /// The filepath to a csv to get the data from
    /// </param>
    /// <returns> a list of all the Messier Objects in the file in type MessierObject </returns> 
    public static List<MessierObject> GetMessier(string fileName = "messier-catalog" + ".csv")
    {
        List<MessierObject> messierList = new();
        IMessierRepository messierRepo = new StarLustMessierCsvRepository(FindCsvPath(fileName));
        foreach(EquatorialMessierObject eMessier in messierRepo.GetMessierObjects())
        {
            MessierObject messier = new();
            HorizontalMessierObject hMessier = new(eMessier);
            messier.ApplyHorizontal(hMessier);
            messierList.Add(messier);
        }
        return messierList;
    }

    private static string FindCsvPath(string fileName = "AllStars" + ".csv")
    {

        var direct = Path.Combine(Path.Combine(GameLoop.GetProjectPath(), "ChargerAstronomyEngine", "ChargerAstronomyEngine", "Data"), "Star", fileName);
        if (File.Exists(direct)) return direct;

        var dir = new DirectoryInfo(GameLoop.GetProjectPath());
        for (int i = 0; i < 8 && dir != null; i++, dir = dir.Parent)
        {
            var candidate = Directory.EnumerateFiles(dir.FullName, fileName, SearchOption.AllDirectories)
                .FirstOrDefault();

            if (candidate != null) return Directory.GetParent(candidate).ToString();
        }

        throw new FileNotFoundException($"Could not locate '{fileName}'. " +
            "Mark it as Content -> Copy if newer, or place it next to the test binaries.");

    }
}
