// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using osu.Framework.Platform;
using osu.Game;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.Formats;
using osu.Game.Configuration;
using osu.Game.Database;
using osu.Game.Rulesets;

namespace OsuStableToLazer;

internal static class Program
{
    private const string lazer_data_option = "--lazer-data";

    public static async Task<int> Main(string[] args)
    {
#if DEBUG
        Console.Error.WriteLine("This tool must be built and run in Release configuration. Debug builds intentionally target a versioned development Realm.");
        return 2;
#endif

        if (!tryParseArguments(args, out string stableFolder, out string? lazerDataPath))
        {
            printUsage();
            return 2;
        }

        stableFolder = Path.GetFullPath(stableFolder);

        if (!Directory.Exists(stableFolder))
            return fail($"Beatmap set directory does not exist: {stableFolder}");

        if (!Directory.EnumerateFiles(stableFolder, "*.osu", SearchOption.TopDirectoryOnly).Any())
            return fail("The directory must contain at least one top-level .osu file.");

        string? dataPath = lazerDataPath == null ? findDefaultLazerDataPath() : Path.GetFullPath(lazerDataPath);

        if (dataPath == null)
            return fail("Could not locate lazer data. Pass --lazer-data <path>.");

        if (!File.Exists(Path.Combine(dataPath, OsuGameBase.CLIENT_DATABASE_FILENAME)))
            return fail($"No {OsuGameBase.CLIENT_DATABASE_FILENAME} was found in: {dataPath}");

        if (isLazerRunning())
            return fail("osu!lazer appears to be running. Close it completely before importing.");

        if (!canCreateHardLinks(stableFolder, dataPath))
            return fail("The stable set and lazer data directories must be on writable NTFS volumes on the same drive for hard-link import.");

        Console.WriteLine($"Stable set: {stableFolder}");
        Console.WriteLine($"lazer data: {dataPath}");
        Console.WriteLine("Opening lazer database and importing with NTFS hard links...");

        var storage = new NativeStorage(dataPath);

        try
        {
            using var realm = new RealmAccess(storage, OsuGameBase.CLIENT_DATABASE_FILENAME);
            using var rulesets = new RealmRulesetStore(realm, storage);

            Decoder.RegisterDependencies(rulesets);

            var importer = new BeatmapImporter(storage, realm);
            var imported = await importer.Import(new ImportTask(stableFolder), new ImportParameters { PreferHardLinks = true }).ConfigureAwait(false);

            if (imported == null)
                return fail("The folder was not imported. See lazer logs for parser or database errors.");

            var result = imported.PerformRead(set => new { set.ID, BeatmapCount = set.Beatmaps.Count, set.Hash });
            Console.WriteLine($"Imported set {result.ID} ({result.BeatmapCount} difficulties, {result.Hash}).");
            return 0;
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"Import failed: {e.Message}");
            return 1;
        }
    }

    private static bool tryParseArguments(string[] args, out string stableFolder, out string? lazerDataPath)
    {
        stableFolder = string.Empty;
        lazerDataPath = null;

        if (args.Length == 1)
        {
            stableFolder = args[0];
            return true;
        }

        if (args.Length == 3 && args[1] == lazer_data_option)
        {
            stableFolder = args[0];
            lazerDataPath = args[2];
            return true;
        }

        return false;
    }

    private static string? findDefaultLazerDataPath()
    {
        string[] candidates =
        {
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "osu"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "osu")
        };

        foreach (string candidate in candidates)
        {
            if (!Directory.Exists(candidate))
                continue;

            string configuredPath = getConfiguredStoragePath(candidate);
            string dataPath = string.IsNullOrEmpty(configuredPath) ? candidate : configuredPath;

            if (File.Exists(Path.Combine(dataPath, OsuGameBase.CLIENT_DATABASE_FILENAME)))
                return dataPath;
        }

        return null;
    }

    private static string getConfiguredStoragePath(string defaultDataPath)
    {
        try
        {
            var config = new StorageConfigManager(new NativeStorage(defaultDataPath));
            return config.Get<string>(StorageConfig.FullPath);
        }
        catch
        {
            return string.Empty;
        }
    }

    private static bool isLazerRunning()
    {
        string[] names = { "osu", "osu!" };
        return names.SelectMany(Process.GetProcessesByName).Any(p => p.Id != Environment.ProcessId);
    }

    private static bool canCreateHardLinks(string sourceDirectory, string destinationDirectory)
    {
        try
        {
            var source = new DriveInfo(Path.GetPathRoot(sourceDirectory)!);
            var destination = new DriveInfo(Path.GetPathRoot(destinationDirectory)!);
            return source.IsReady && destination.IsReady
                   && source.Name.Equals(destination.Name, StringComparison.OrdinalIgnoreCase)
                   && source.DriveFormat.Equals("NTFS", StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }

    private static int fail(string message)
    {
        Console.Error.WriteLine($"Error: {message}");
        return 1;
    }

    private static void printUsage()
    {
        Console.Error.WriteLine("Usage: osu-stable-to-lazer.exe <stable beatmap set folder> [--lazer-data <lazer data folder>]");
    }
}
