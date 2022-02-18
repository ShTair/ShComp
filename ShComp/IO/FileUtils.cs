namespace ShComp.IO;

public static class FileUtils
{
    public static IEnumerable<PathInfo> GetPathInfos(string targetDirectory)
    {
        targetDirectory = Path.GetFullPath(targetDirectory);
        if (!Directory.Exists(targetDirectory)) throw new DirectoryNotFoundException();
        return GetPathInfos(targetDirectory, "");
    }

    private static IEnumerable<PathInfo> GetPathInfos(string currentDir, string currentSub)
    {
        yield return new PathInfo { IsDirectory = true, Path = currentDir, SubPath = currentSub };

        foreach (var dir in Directory.EnumerateDirectories(currentDir))
        {
            var dirName = Path.GetFileName(dir);
            var sub = currentSub is null ? dirName : Path.Combine(currentSub, dirName);
            foreach (var pathInfo in GetPathInfos(dir, sub)) yield return pathInfo;
        }

        foreach (var file in Directory.EnumerateFiles(currentDir))
        {
            var fileName = Path.GetFileName(file);
            var sub = currentSub is null ? fileName : Path.Combine(currentSub, fileName);
            yield return new PathInfo { IsDirectory = false, Path = file, SubPath = sub };
        }
    }
}

#pragma warning disable CS8618

public class PathInfo
{
    public bool IsDirectory { get; set; }

    public string Path { get; set; }

    public string SubPath { get; set; }
}

#pragma warning restore CS8618
