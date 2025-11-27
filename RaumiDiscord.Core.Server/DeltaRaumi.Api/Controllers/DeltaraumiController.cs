using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using RaumiDiscord.Core.Server.DeltaRaumi.Common.Data;
using RaumiDiscord.Core.Server.DeltaRaumi.Configuration.Models;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Api.Controllers
{
    /// <summary>
    /// Deltaraumiの基本的な情報を提供するコントローラー
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DeltaraumiController : ControllerBase
    {
        private static readonly DateTime StartTime = new BotConfiguration().Setting.UpTime;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet("ping")]
        public Task<IActionResult> HttpPing()
        {
            return Task.FromResult<IActionResult>(Ok("pingConnection"));
        }
        /// <summary>
        /// サーバーの状態情報を JSON で返します。<br/>
        /// 返却されるオブジェクトには以下が含まれます：
        /// - serverVersion (string): サーバーのバージョン<br/>
        /// - apiVersion (string): API のバージョン<br/>
        /// - processCount (int): 現在実行中のプロセス数<br/>
        /// - availableMemoryMB (long): 利用可能メモリ（MB）<br/>
        /// - uptime (string): サーバー起動からの経過時間（DD:HH:mm:ss）<br/>
        /// - cpuUsagePercent (double): CPU 使用率（%）<br/>
        /// - network.bytesSent (long): 送信済みバイト数<br/>
        /// - network.bytesReceived (long): 受信済みバイト数<br/>
        /// - requestDurationMs (long): このリクエストの処理時間（ms）
        /// </summary>
        /// <returns>サーバー状態を示す JSON オブジェクト</returns>

        [HttpGet("status")]
        public async Task<IActionResult> ServerInfo()
        {
            var sw = Stopwatch.StartNew();

            Process.GetCurrentProcess();
            var uptime = DateTime.Now - StartTime;

            var cpuUsage = await GetCpuUsageAsync();
            var networkStats = GetNetworkStats();

            sw.Stop();

            var result = new
            {
                serverVersion = "0.1.3.18",
                apiVersion = "1.0",
                processCount = Process.GetProcesses().Length,
                availableMemoryMB = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes / 1024 / 1024,
                uptime = $"{(int)uptime.TotalDays:D2}:{uptime.Hours:D2}:{uptime.Minutes:D2}:{uptime.Seconds:D2}",
                cpuUsagePercent = Math.Round(cpuUsage, 2),
                network = new
                {
                    networkStats.bytesSent, networkStats.bytesReceived
                },
                requestDurationMs = sw.ElapsedMilliseconds
            };

            return Ok(result);
        }

        private static async Task<double> GetCpuUsageAsync()
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

        private static (long bytesSent, long bytesReceived) GetNetworkStats()
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
