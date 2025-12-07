using Assets.Scripts.CelestialBodies;
using ChargerAstronomyEngine.Data;
using ChargerAstronomyEngine.Data.Messier;
using ChargerAstronomyShared.Contracts.Models;
using ChargerAstronomyShared.Contracts.Repositories;
using ChargerAstronomyShared.Domain.Equatorial;
using ChargerAstronomyShared.Domain.Horizontal;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ChargerAstronomyShared.Domain.SpatialIndex;

namespace Assets.Scripts.Core
{

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


        public static List<MessierObject> GetMessier(IEngineService<IHorizontal> engineService, string fileName = "messier-catalog" + ".csv")
        {
            List<MessierObject> messierList = new();
            IMessierRepository messierRepo = new StarLustMessierCsvRepository(FindCsvPath(fileName));
            SpatialStarIndex<IHorizontal> starIndex = engineService.SpatialStarIndex;
            foreach (EquatorialMessierObject eMessier in messierRepo.GetMessierObjects())
            {
                HorizontalMessierObject hMessier = new(eMessier);
                MessierObject messier = new(hMessier);
                messier.ApplyHorizontal(hMessier);
                messierList.Add(messier);
                //starIndex.AddStar(messier);
            }
            return messierList;
        }

        private static string FindCsvPath(string fileName = "messier-catalog" + ".csv")
        {

            var direct = Path.Combine(GameLoop.GetProjectPath(), fileName);
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
}