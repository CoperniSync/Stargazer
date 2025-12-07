using Assets.Scripts.CelestialBodies;
using ChargerAstronomyEngine.Data.Star;
using ChargerAstronomyEngine.Streaming;
using ChargerAstronomyShared.Contracts.Models;
using ChargerAstronomyShared.Contracts.Repositories;
using ChargerAstronomyShared.Domain.Equatorial;
using ChargerAstronomyShared.Domain.Horizontal;
using ChargerAstronomyShared.Domain.SpatialIndex;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Assets.Scripts.Core
{

    // Author: Morgan Hendon FA 2025

    public class StarQueue
    {


        private SpatialStarIndex<IHorizontal> starIndex;

        private CsvStarRepository starRepo; // repository to pull stars from

        private BoundedInitializationQueue<PageResult<EquatorialStar>> queue;

        /// <summary>
        /// Construct a new instance of StarQueue and have it start pulling data from the designated repo
        /// </summary>
        public StarQueue(IEngineService<IHorizontal> engineService, int amountToTake, string fileName = "AllStars" + ".csv")
        {
            starRepo = new(FindCsvPath(fileName));
            queue = new(capacity: 5);

            starIndex = engineService.SpatialStarIndex;
            FillQueue(amountToTake, queue, starRepo);
        }
        /// <summary>
        /// The method that stars pulling data from the repo using BoundedInitialization Queue
        /// </summary>
        private async Task FillQueue(int amountToTake, BoundedInitializationQueue<PageResult<EquatorialStar>> queue, CsvStarRepository starRepo)
        {
            _ = Task.Run(() => starRepo.ProducePagesAsync(queue, new PageRequest(0, amountToTake), CancellationToken.None));
            await Task.Delay(0);
        }

        /// <summary>
        /// Gets the file for the designated repo at fileName
        /// </summary>
        private static string FindCsvPath(string fileName = "AllStars" + ".csv")
        {

            var direct = Path.Combine(GameLoop.GetProjectPath(), fileName);
            if (File.Exists(direct)) return direct;

            var dir = new DirectoryInfo(GameLoop.GetProjectPath());
            for (int i = 0; i < 8 && dir != null; i++, dir = dir.Parent)
            {
                var candidate = Directory.EnumerateFiles(dir.FullName, fileName, SearchOption.AllDirectories)
                    .FirstOrDefault();

                if (candidate != null) return candidate;
            }

            throw new FileNotFoundException($"Could not locate '{fileName}'. " +
                "Mark it as Content -> Copy if newer, or place it next to the test binaries.");

        }

        /// <summary>
        /// A star located by the Horizontal Coordinate method
        /// </summary>
        public bool TryDequeue(ref List<Star> starList)
        {
            if (queue != null && queue.TryDequeue(out var pr))
            {
                IReadOnlyList<EquatorialStar> equatorialList = pr.Items;
                foreach (EquatorialStar equatorialStar in equatorialList)
                {
                    HorizontalStar hstar = new HorizontalStar(equatorialStar);
                    Star newStar = new(hstar, 150f);
                    starIndex.AddStar(newStar);
                    starList.Add(newStar);
                    newStar.UpdatePosition();

                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsCompleted()
        {
            return queue.IsCompleted;
        }

        public void Dispose()
        {
            queue.Dispose();
        }
    }
}