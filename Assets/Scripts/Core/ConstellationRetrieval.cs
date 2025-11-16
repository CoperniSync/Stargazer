using Assets.Scripts.CelestialBodies;
using ChargerAstronomyEngine.Data.Messier;
using ChargerAstronomyEngine.Streaming;
using ChargerAstronomyShared.Contracts.Models;
using ChargerAstronomyShared.Contracts.Repositories;
using ChargerAstronomyShared.Domain.Equatorial;
using ChargerAstronomyShared.Domain.Horizontal;
using ChargerAstronomyShared.Domain.SpatialIndex;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChargerAstronomyEngine.Data.Constellations;

namespace Assets.Scripts.Core
{
    public class ConstellationRetrieval
    {
        public static void GetConstellations(ref List<UnityConstellation> constellationList, List<Star> starList, string path = "constellations" + ".json")
        {
            IConstellationRepository constRepo = new CsvConstellationRepository(FindCsvPath(path));
            foreach (Constellation constellation in constRepo.GetConstellations())
            {
                UnityConstellation uConst = new(constellation, starList );
                constellationList.Add(uConst);
            }
        }

        private static string FindCsvPath(string fileName = "constellations" + ".json")
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
}
