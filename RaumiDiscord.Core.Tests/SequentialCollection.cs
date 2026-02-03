using Xunit;

namespace RaumiDiscord.Core.Tests;

/// <summary>
/// テストを順次実行するためのコレクション定義
/// データベースの競合を防ぐため、テストは並列実行されません
/// </summary>
[CollectionDefinition("Sequential", DisableParallelization = true)]
public class SequentialCollection
{
    // このクラスはマーカーとして機能し、コード本体は不要です
}
