using Hardware.Info;
using RuoYi.Data.Models;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace RuoYi.System.Services;
public class ServerService : ITransient
{
    private const string NO_DATA = "暂无";
    private readonly IHardwareInfo _hardwareInfo;
    public ServerService()
    {
        _hardwareInfo = new HardwareInfo(true);
    }

    public Server GetServerInfo()
    {
        _hardwareInfo.RefreshCPUList();
        _hardwareInfo.RefreshMemoryList();
        _hardwareInfo.RefreshMemoryStatus();
        _hardwareInfo.RefreshDriveList();
        var cpuUsed = Convert.ToDouble(_hardwareInfo.CpuList.FirstOrDefault()?.PercentProcessorTime ?? 0);
        var cpuFree = MathUtils.Round((1 - cpuUsed / 100) * 100, 0);
        var cpu = new Cpu
        {
            CpuNum = Convert.ToInt32(_hardwareInfo.CpuList.FirstOrDefault()?.NumberOfCores ?? 0),
            Total = cpuUsed,
            Free = cpuFree,
            Used = NO_DATA,
            Sys = NO_DATA
        };
        var memTotal = _hardwareInfo.MemoryStatus.TotalPhysical;
        var memFree = _hardwareInfo.MemoryStatus.AvailablePhysical;
        var mem = new Mem
        {
            Total = CalculateMem(memTotal),
            Used = CalculateMem(memTotal - memFree),
            Free = CalculateMem(memFree),
            Usage = MathUtils.Round(Convert.ToDecimal(memTotal - memFree) / memTotal, 4) * 100
        };
        var sys = new Sys
        {
            ComputerName = Environment.MachineName,
            ComputerIp = IpUtils.GetHostIpAddr(),
            UserDir = Environment.CurrentDirectory,
            OsName = RuntimeInformation.OSDescription + Environment.OSVersion.ToString(),
            OsArch = RuntimeInformation.OSArchitecture.ToString()
        };
        var process = Process.GetCurrentProcess();
        var clrUsed = CalculateClrMem(process.WorkingSet64);
        var clrVersion = Environment.Version.ToString();
        var clrHome = GetClrHome();
        var clr = new Clr
        {
            Name = RuntimeInformation.FrameworkDescription.Replace(clrVersion, ""),
            Version = clrVersion,
            Home = clrHome,
            Total = NO_DATA,
            Max = process.MaxWorkingSet,
            Used = clrUsed,
            Free = NO_DATA,
            StartTime = process.StartTime.To_YmdHms(),
            RunTime = GetRunTime(DateTime.Now, process.StartTime),
            InputArgs = string.Join(", ", Environment.GetCommandLineArgs()),
            Usage = NO_DATA
        };
        var sysFiles = _hardwareInfo.DriveList.Select(d =>
        {
            var total = d.Size;
            var free = GetDriveFreeSpace(d);
            var used = total - free;
            return new SysFile
            {
                DirName = d.Name,
                SysTypeName = d.Description,
                TypeName = GetFileSystem(d.PartitionList),
                Total = ConvertFileSize(total),
                Free = ConvertFileSize(free),
                Used = ConvertFileSize(used),
                Usage = MathUtils.Round(Convert.ToDecimal(used) / total, 2) * 100
            };
        }).ToList();
        return new Server
        {
            Cpu = cpu,
            Mem = mem,
            Sys = sys,
            Clr = clr,
            SysFiles = sysFiles
        };
    }

    private double CalculateMem(ulong mem)
    {
        return MathUtils.Round(mem / (1024 * 1024 * 1024.0), 2);
    }

    private double CalculateClrMem(long mem)
    {
        return MathUtils.Round(mem / (1024 * 1024.0), 2);
    }

    private string GetStartTime(long tickCount)
    {
        var now = DateTime.Now.ToUnixTimeMilliseconds();
        var startDateTime = (now - tickCount).FromUnixTimeMilliseconds();
        return startDateTime.To_YmdHms();
    }

    private string GetClrHome()
    {
        var clrSdks = CmdUtils.Run("dotnet", "--list-sdks");
        var sdks = clrSdks.Split(Environment.NewLine);
        var path = sdks.Where(info => info.StartsWith(Environment.Version.Major.ToString())).FirstOrDefault();
        return path ?? "";
    }

    private string GetRunTime(long tickCount)
    {
        var daySeconds = 3600 * 24;
        var hourSeconds = 3600;
        var totalSeconds = tickCount / 1000;
        var days = totalSeconds / daySeconds;
        var hours = (totalSeconds - days * daySeconds) / hourSeconds;
        var minutes = (totalSeconds - days * daySeconds - hours * hourSeconds) / 60;
        return $"{days}天{hours}小时{minutes}分钟";
    }

    private string GetRunTime(DateTime endTime, DateTime startTime)
    {
        var timeSpan = endTime - startTime;
        return $"{timeSpan.Days}天{timeSpan.Hours}小时{timeSpan.Minutes}分钟";
    }

    private string ConvertFileSize(ulong size)
    {
        ulong kb = 1024;
        ulong mb = kb * 1024;
        ulong gb = mb * 1024;
        if (size >= gb)
        {
            return string.Format("{0:#} GB", (float)size / gb);
        }
        else if (size >= mb)
        {
            float f = (float)size / mb;
            return string.Format(f > 100 ? "{0:#} MB" : "{0:##} MB", f);
        }
        else if (size >= kb)
        {
            float f = (float)size / kb;
            return string.Format(f > 100 ? "{0:#} KB" : "{0:##} KB", f);
        }
        else
        {
            return string.Format("{0} B", size);
        }
    }

    private string GetFileSystem(List<Partition> partitions)
    {
        string? fileSystem = "";
        foreach (var partition in partitions)
        {
            fileSystem = partition.VolumeList.Where(v => !string.IsNullOrEmpty(v.FileSystem)).FirstOrDefault()?.FileSystem;
            if (!string.IsNullOrEmpty(fileSystem))
                break;
        }

        return fileSystem ?? "";
    }

    private ulong GetDriveFreeSpace(Drive drive)
    {
        long freeSpace = 0;
        foreach (var partition in drive.PartitionList)
        {
            freeSpace += partition.VolumeList.Sum(v => (long)v.FreeSpace);
        }

        return Convert.ToUInt64(freeSpace);
    }
}