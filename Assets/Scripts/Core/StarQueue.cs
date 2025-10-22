using Assets.Scripts.CelestialBodies;
using ChargerAstronomyEngine.Data;
using ChargerAstronomyEngine.Streaming;
using ChargerAstronomyShared.Contracts.Models;
using ChargerAstronomyShared.Domain.Equatorial;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;


public class StarQueue
{
    private CsvStarRepository starRepo;
    private BoundedInitializationQueue<PageResult<EquatorialStar>> queue;

    /// <summary>
    /// Construct a new instance of StarQueue and have it start pulling data from the designated repo
    /// </summary>
    public StarQueue(int amountToTake ,string fileName = "AllStars" + ".csv")
    {
        starRepo = new(FindCsvPath(fileName));
        BoundedInitializationQueue<PageResult<EquatorialStar>> queue = new(capacity: 5);
        fillQueue(amountToTake, queue, starRepo);
    }

    /// <summary>
    /// The method that stars pulling data from the repo using BoundedInitialization Queue
    /// </summary>
    private async Task fillQueue( int amountToTake, BoundedInitializationQueue<PageResult<EquatorialStar>> queue, CsvStarRepository starRepo)
    {
        _ = Task.Run(() => starRepo.ProducePagesAsync(queue, new PageRequest(0, amountToTake), CancellationToken.None));
        await Task.Delay(1);
    }

    /// <summary>
    /// Gets the file for the designated repo at fileName
    /// </summary>
    private static string FindCsvPath(string fileName = "AllStars" + ".csv")
    {
        var direct = Path.Combine(AppContext.BaseDirectory, fileName);
        if (File.Exists(direct)) return direct;

        var dir = new DirectoryInfo(AppContext.BaseDirectory);
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
        if (queue.TryDequeue(out var pr))
        {
            List<EquatorialStar> equatorialList = (List<EquatorialStar>)pr.Items;
            foreach( EquatorialStar equatorialStar in equatorialList)
            {
                //Star newstar = new(new HorizontalStar(equatorialStar)));
            }
            return true;
        }
        else
        {
            return false;
        }
    }
}
