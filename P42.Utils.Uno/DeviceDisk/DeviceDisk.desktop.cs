using P42.Serilog.QuickLog;

namespace P42.Utils.Uno;

// ReSharper disable once UnusedType.Global
public partial class DeviceDisk
{
    public static Task<Dictionary<string, ulong>> NativeGetAppFolderPropertiesAsync(string[] properties)
    {
        return Task.Run(() =>
        {
            var result = new Dictionary<string, ulong>();

            if (OperatingSystem.IsWindows())
            {
                var data = Platform
                    .ExecuteShellCommand("wmic /locale:ms_409 logicaldisk get name,size,freespace /format:csv").Trim();
                var lines = data.Split('\n');
                if (lines.Length < 2)
                    return result;

                var headers = lines[0].RemoveExtraWhitespace().Split(',');
                var freeColumn = -1;
                var capacityColumn = -1;
                var nameColumn = -1;
                for (var i = 0; i < headers.Length; i++)
                {
                    var header = headers[i];
                    Console.WriteLine($"header: {header}");
                    if (header.Equals("Size", StringComparison.OrdinalIgnoreCase))
                        capacityColumn = i;
                    if (header.Equals("FreeSpace", StringComparison.OrdinalIgnoreCase))
                        freeColumn = i;
                    if (header.Equals("Name", StringComparison.OrdinalIgnoreCase))
                        nameColumn = i;
                }

                if (freeColumn < 0)
                {
                    Console.Error.WriteLine("Free column not found");
                    return result;
                }

                if (capacityColumn < 0)
                {
                    Console.Error.WriteLine("Capacity column not found");
                    return result;
                }

                if (nameColumn < 0)
                {
                    Console.Error.WriteLine("Name column not found");
                    return result;
                }

                var currentDir = Directory.GetCurrentDirectory();
                var currentDrive = Path.GetPathRoot(currentDir)?.Trim('\\') ?? string.Empty;

                for (var i = 1; i < lines.Length; i++)
                {
                    var line = lines[i];
                    var values = line.RemoveExtraWhitespace().Split(',');
                    if (!values[nameColumn].Equals(currentDrive, StringComparison.OrdinalIgnoreCase))
                        continue;

                    result["System.FreeSpace"] = ulong.Parse(values[freeColumn]);
                    result["System.Capacity"] = ulong.Parse(values[capacityColumn]);
                    break;
                }

                return result;
            }


            if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
            {
                var data = Platform.ExecuteShellCommand("df -P .");
                var lines = data.Split('\n');
                if (lines.Length < 2)
                    return result;

                var headers = lines[0].RemoveExtraWhitespace().Split(' ');
                ulong blockSize = 0;
                var freeColumn = -1;
                var capacityColumn = -1;
                for (var i = 0; i < headers.Length; i++)
                {
                    var header = headers[i];
                    if (header.EndsWith("-blocks"))
                    {
                        capacityColumn = i;
                        blockSize = ulong.Parse(header.Split('-')[0]);
                    }

                    if (header.Equals("Available", StringComparison.OrdinalIgnoreCase))
                        freeColumn = i;
                }

                if (blockSize <= 0)
                {
                    QLog.Error("Block size not found", "NativeGetAppFolderProperties Error");
                    return result;
                }

                if (freeColumn < 0)
                {
                    QLog.Error("Free column not found", "NativeGetAppFolderProperties Error");
                    return result;
                }

                if (capacityColumn < 0)
                {
                    QLog.Error("Capacity column not found", "NativeGetAppFolderProperties Error");
                    return result;
                }


                var values = lines[1].RemoveExtraWhitespace().Split(' ');

                result[FreeSpace] = ulong.Parse(values[freeColumn]) * blockSize;
                result[Capacity] = ulong.Parse(values[capacityColumn]) * blockSize;
            }


            throw new NotSupportedException("Current OS is not supported by NativeGetAppFolderProperties");
        });
    }
}
