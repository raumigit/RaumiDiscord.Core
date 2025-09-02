namespace RaumiDiscord.Core.Server.DeltaRaumi.Database.Models
{
    /// <summary>
    /// 天気予報のモデル
    /// </summary>
    public class WeatherForecast
    {
        /// <summary>
        /// 日付
        /// </summary>
        public DateOnly Date { get; set; }
        /// <summary>
        /// 気温(C)
        /// </summary>
        public int TemperatureC { get; set; }
        /// <summary>
        /// 気温(F)
        /// </summary>
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
        /// <summary>
        /// 天気の概要
        /// </summary>
        public string? Summary { get; set; }
    }
}
