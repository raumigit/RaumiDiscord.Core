using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Net;
using System.Net.NetworkInformation;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeltaraumiController : ControllerBase
    {
        [HttpGet("ping")]
        public async Task<IActionResult> HttpPing()
        {
            return Ok("pingConnection");
        }
        [HttpGet("status")]
        public async Task<IActionResult> ServerInfo()
        {
            return Ok(new
            {
                ServerName = Environment.MachineName,
                OSVersion = Environment.OSVersion.ToString(),
                ProcessorCount = Environment.ProcessorCount,
                MemoryAvailable = GC.GetTotalMemory(false),
                Uptime = TimeSpan.FromMilliseconds(Environment.TickCount64),
                NetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces()
                    .Select(ni => new
                    {
                        Name = ni.Name,
                        Status = ni.OperationalStatus.ToString(),
                        Speed = ni.Speed
                    })
            });
        }
    }
}
