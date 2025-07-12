using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using RaumiDiscord.Core.Server.DeltaRaumi.Common.Data;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeltaraumiController : ControllerBase
    {
        private static readonly DateTime _startTime = new Configuration().GetConfig().Setting.UpTime;

        [HttpGet("ping")]
        public async Task<IActionResult> HttpPing()
        {
            return Ok("pingConnection");
        }
        [HttpGet("status")]
        public async Task<IActionResult> ServerInfo()
        {
            var sw = Stopwatch.StartNew();

            var process = Process.GetCurrentProcess();
            var uptime = DateTime.UtcNow - _startTime;

            var cpuUsage = await GetCpuUsageAsync();
            var networkStats = GetNetworkStats();

            sw.Stop();

            var result = new
            {
                serverVersion = "0.1.3.10",
                apiVersion= "1.0",
                processCount = Process.GetProcesses().Length,
                availableMemoryMB = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes / 1024 / 1024,
                uptime = $"{(int)uptime.TotalDays:D2}:{uptime.Hours:D2}:{uptime.Minutes:D2}:{uptime.Seconds:D2}",
                cpuUsagePercent = Math.Round(cpuUsage, 2),
                network = new
                {
                    bytesSent = networkStats.bytesSent,
                    bytesReceived = networkStats.bytesReceived
                },
                requestDurationMs = sw.ElapsedMilliseconds
            };

            return Ok(result);
        }

        private async Task<double> GetCpuUsageAsync()
        {
            var process = Process.GetCurrentProcess();
            var startCpuTime = process.TotalProcessorTime;
            var startTime = DateTime.UtcNow;

            await Task.Delay(500); // 少し待ってCPU時間の差を見る

            var endCpuTime = process.TotalProcessorTime;
            var endTime = DateTime.UtcNow;

            var cpuUsedMs = (endCpuTime - startCpuTime).TotalMilliseconds;
            var totalMsPassed = (endTime - startTime).TotalMilliseconds;
            var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed) * 100;

            return cpuUsageTotal;
        }

        private (long bytesSent, long bytesReceived) GetNetworkStats()
        {
            long sent = 0;
            long received = 0;

            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.OperationalStatus != OperationalStatus.Up)
                    continue;

                var stats = ni.GetIPStatistics();
                sent += stats.BytesSent;
                received += stats.BytesReceived;
            }

            return (sent, received);
        }
    }
}
