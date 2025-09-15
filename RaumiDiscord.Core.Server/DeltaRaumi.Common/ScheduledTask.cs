using RaumiDiscord.Core.Server.DeltaRaumi.Bot.Helpers;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Common
{
    /// <summary>
    /// ScheduledTaskは、定期的に実行されるタスクを管理します。
    /// </summary>
    public class ScheduledTask
    {
        private readonly ImprovedLoggingService _logger;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// ScheduledTaskのコンストラクタ
        /// </summary>
        /// <param name="loggingService"></param>
        /// <param name="serviceProvider"></param>
        public ScheduledTask(ImprovedLoggingService loggingService, IServiceProvider serviceProvider)
        {
            _logger = loggingService;
            _serviceProvider = serviceProvider;
        }

/*
        private Task ScanDatabaseAsync()
        {
            // TODO: DBとキャッシュを比較して不足データを補う処理を書く
            _logger.Log("DBのスキャンを実行中...", "スケジュールタスク");
            return Task.CompletedTask;
        }
*/

/*
        private void CleanUpLogs(string logDirectory, TimeSpan maxAge)
        {
            _logger.Log("ログのクリーンアップを開始: {Path}", logDirectory);
            CleanOldFiles(logDirectory, maxAge);
        }
*/

/*
        private void CleanUpCache(string cacheDirectory, TimeSpan maxAge)
        {
            _logger.Log("キャッシュのクリーンアップを開始: {Path}", cacheDirectory);
            CleanOldFiles(cacheDirectory, maxAge);
        }
*/

/*
        private void CleanOldFiles(string path, TimeSpan maxAge)
        {
            if (!Directory.Exists(path)) return;

            var files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                try
                {
                    var lastWriteTime = File.GetLastWriteTime(file);
                    if (DateTime.Now - lastWriteTime > maxAge)
                    {
                        File.Delete(file);
                        _logger.Log("削除: {File}", file);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Log($"ファイル削除中にエラーが発生: {file}\n{ex}", "スケジュールタスク");
                }
            }
        }
*/
    }
}
