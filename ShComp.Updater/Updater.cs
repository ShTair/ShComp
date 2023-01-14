using System.Diagnostics;
using System.Reflection;

namespace ShComp.Updater;

public sealed class Updater : IDisposable
{
    private readonly UpdaterConfiguration? _configuration;
    private readonly Func<Task> _stop;

    private ChangeDetector? _detector;

    public Updater(UpdaterConfiguration? configuration, Func<Task> stop)
    {
        _configuration = configuration;
        _stop = stop;
    }

    public void Dispose()
    {
        _detector?.Dispose();
    }

    public async Task<bool> RunAsync()
    {
        if (_configuration is null or { IsEnabled: false }) return true;

        if (_configuration.SourcePath is not { } sourcePath) throw new Exception();

        var exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        if (sourcePath == exePath)
        {
            Console.WriteLine($"配置ディレクトリで実行されています。");

            await Task.Delay(5000);

            Console.WriteLine($"本番ディレクトリにファイルをコピーします。");

            if (_configuration.ProductionPath is not { } productionPath) throw new Exception();
            Copy(sourcePath, productionPath);

            Console.WriteLine($"本体を実行します。");

            if (_configuration.ExeFileName is not { } exeFileName) throw new Exception();
            Process.Start(new ProcessStartInfo
            {
                WorkingDirectory = productionPath,
                FileName = exeFileName,
                CreateNoWindow = false,
                UseShellExecute = true,
            });

            Console.WriteLine($"終了します。");

            return false;
        }
        else
        {
            Console.WriteLine($"本番ディレクトリで実行されています。");
            Console.WriteLine($"配置ディレクトリのチェックを開始します。");

            _detector = new ChangeDetector(sourcePath, TimeSpan.FromSeconds(10));

            _ = Task.Run(async () =>
            {
                await _detector.WaitAsync();
                Console.WriteLine($"配置ディレクトリが更新されました。");
                Console.WriteLine($"プログラムを終了しています。");
                await _stop();
            });

            return true;
        }
    }

    public async Task UpdateAsync()
    {
        if (_detector is null) return;

        await _detector.WaitAsync();

        Console.WriteLine($"配置ディレクトリを実行します。");
        if (_configuration!.SourcePath is not { } sourcePath) throw new Exception();
        if (_configuration.ExeFileName is not { } exeFileName) throw new Exception();

        Process.Start(new ProcessStartInfo
        {
            WorkingDirectory = sourcePath,
            FileName = exeFileName,
            CreateNoWindow = false,
            UseShellExecute = true,
        });
    }

    private void Copy(string srcDir, string dstDir)
    {
        if (File.Exists(dstDir)) File.Delete(dstDir);
        if (!Directory.Exists(dstDir)) Directory.CreateDirectory(dstDir);

        foreach (var filePath in Directory.EnumerateFiles(srcDir))
        {
            var fileName = Path.GetFileName(filePath);
            var dstFilePath = Path.Combine(dstDir, fileName);
            Console.WriteLine($"F {fileName}");

            if (Directory.Exists(dstFilePath)) Directory.Delete(dstFilePath, true);
            File.Copy(filePath, dstFilePath, true);
        }

        foreach (var dirPath in Directory.EnumerateDirectories(srcDir))
        {
            var dirName = Path.GetFileName(dirPath);
            var dstDirPath = Path.Combine(dstDir, dirName);
            Console.WriteLine($"D {dirName}");

            Copy(dirPath, dstDirPath);
        }
    }
}
