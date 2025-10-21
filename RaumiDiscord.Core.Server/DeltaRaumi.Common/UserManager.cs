using Discord.WebSocket;
using Microsoft.EntityFrameworkCore;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.EventHandlers;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.Helpers;
using RaumiDiscord.Core.Server.DeltaRaumi.Database;
using RaumiDiscord.Core.Server.DeltaRaumi.Database.DataContext;
using RaumiDiscord.Core.Server.DeltaRaumi.Database.Models;
using System.Globalization;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Common
{
    /// <summary>
    /// ユーザーに関する管理クラスです。ユーザーのデータを管理し、更新や取得を行います。
    /// </summary>
    public class UserManager
    {
        private readonly ImprovedLoggingService _logger;
        private readonly IDbContextFactory<DeltaRaumiDbContext> _deltaRaumiDb;
        private readonly DiscordSocketClient _client;
        private readonly IDictionary<string, UserBaseDataModel> _users;

        /// <summary>
        /// UserManagerのコンストラクタ
        /// </summary>
        /// <param name="client"></param>
        /// <param name="logging"></param>
        /// <param name="deltaRaumiDb"></param>
        /// <param name="users"></param>
        public UserManager(
            DiscordSocketClient client,
            ImprovedLoggingService logging,
            IDbContextFactory<DeltaRaumiDbContext> deltaRaumiDb, IDictionary<string, UserBaseDataModel> users)
        {
            _client = client;
            _logger = logging;
            _deltaRaumiDb = deltaRaumiDb;
            _users = users;
        }
        /// <summary>
        /// ユーザーが更新されたときに発生します。
        /// </summary>
        public event EventHandler<GenericEventHandler<UserBaseDataModel>>? OnUserUpdated;

        /// <summary>
        /// UserBaseDataModelのコレクションを取得します。
        /// </summary>
        public IEnumerable<UserBaseDataModel> Users => _users.Values;


        /// <summary>
        /// UserBaseDataModelをIDで取得します。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public UserBaseDataModel? GetUserById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException(nameof(id));

            _users.TryGetValue(id, out var user);

            return user;
        }
        /// <summary>
        /// ユーザー名でUserBaseDataModelを取得します。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public UserBaseDataModel? GetUserByName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            return _users.Values.FirstOrDefault(u => string.Equals(u.UserName, name, StringComparison.OrdinalIgnoreCase));
        }

        internal async Task UpdateUserAsync(UserBaseDataModel user)
        {
            await UpdateUserInternalAsync(user);
        }

        private Task UpdateUserInternalAsync(UserBaseDataModel user)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 新しいユーザーを作成します。
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<UserBaseDataModel> CreateUserAsync(ulong userid, string username)
        {
            if (Users.Any(u => u.UserName.Equals(username, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ArgumentException(string.Format(
                    CultureInfo.InvariantCulture,
                    "A userid with the name '{0}' already exists.",
                    username));
            }
            if (Users.Any(u => u.UserId.Equals(userid)))
            {
                throw new ArgumentException(string.Format(
                    CultureInfo.InvariantCulture,
                    "A user ID already exists for ulong user ID '{0}'.",
                    username));
            }

            UserBaseDataModel newUser;

            var dbContext = await _deltaRaumiDb.CreateDbContextAsync();

            await using (dbContext)
            {
                var userId = _client.CurrentUser.Id;
                newUser = await CreateUserInternalAsync(userId, username);
                dbContext.UserBases.Add(newUser);
                await dbContext.SaveChangesAsync();
            }
            return newUser;
        }

        private Task<UserBaseDataModel> CreateUserInternalAsync(ulong userid, string username)
        {
            //throw new NotImplementedException();
            if (userid == 0)
                throw new ArgumentException("User ID cannot be zero.", nameof(userid));

            var discordUser = _client.GetUser(userid);


            var userBase = new UserBaseDataModel()
            {
                UserId = userid.ToString()

            };

            bool isBot = discordUser.IsBot;
            bool isWebhook = discordUser.IsWebhook;
            userBase.UserName = discordUser.Username;
            userBase.AvatarId = discordUser.AvatarId;
            userBase.CreatedAt = discordUser.CreatedAt.UtcDateTime;

            //user.AddDefaultPermissions();
            //user.AddDefaultPreferences();
            userBase.AddDefaultDiscordData(isBot, isWebhook);

            return Task.FromResult(userBase);
        }
    }
}
