using Discord;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.Services.EmbedFactory.Models;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.Helpers;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Bot.Services.EmbedFactory
{
    /// <summary>
    /// Factory service for creating standardized Discord embeds
    /// </summary>
    public class EmbedFactory : IEmbedFactory
    {
        private readonly ImprovedLoggingService _loggingService;
        private readonly Color _raumiMainColor = new Color(0x7bb3ee);
        private readonly Color _raumiSubColor = new Color(0xf02443);

        public EmbedFactory(ImprovedLoggingService loggingService)
        {
            _loggingService = loggingService;
        }

        /// <summary>
        /// Creates a base embed with common styling and user context
        /// </summary>
        /// <param name="user">The user context for the embed</param>
        /// <param name="color">Optional color override for the embed</param>
        /// <returns>A configured EmbedBuilder instance</returns>
        public EmbedBuilder CreateBaseEmbed(IUser user, Color? color = null)
        {
            try
            {
                var builder = new EmbedBuilder();
                
                // Set author to the user
                builder.WithAuthor(user);
                
                // Apply color scheme
                builder.WithColor(color ?? _raumiMainColor);
                
                // Add timestamp
                builder.WithCurrentTimestamp();
                
                return builder;
            }
            catch (Exception ex)
            {
                _loggingService?.Log($"Error creating base embed: {ex.Message}", "EmbedFactory", ImprovedLoggingService.LogLevel.Error);
                
                // Return a minimal fallback embed
                return new EmbedBuilder()
                    .WithAuthor(user)
                    .WithColor(_raumiMainColor)
                    .WithCurrentTimestamp();
            }
        }

        /// <summary>
        /// Creates an FAQ-style embed using structured data
        /// </summary>
        /// <param name="user">The user context for the embed</param>
        /// <param name="data">The FAQ data containing title, description, fields, etc.</param>
        /// <returns>A configured EmbedBuilder instance</returns>
        public EmbedBuilder CreateFAQEmbed(IUser user, FAQEmbedData data)
        {
            if (data == null)
            {
                _loggingService?.Log("FAQEmbedData is null", "EmbedFactory", ImprovedLoggingService.LogLevel.Warning);
                return CreateErrorEmbed(user, "FAQ data is not available");
            }

            try
            {
                var builder = CreateBaseEmbed(user, data.Color);
                
                // Set title and description
                if (!string.IsNullOrEmpty(data.Title))
                    builder.WithTitle(data.Title);
                
                if (!string.IsNullOrEmpty(data.Description))
                    builder.WithDescription(data.Description);
                
                // Add fields
                if (data.Fields != null)
                {
                    foreach (var field in data.Fields)
                    {
                        if (!string.IsNullOrEmpty(field.Key) && !string.IsNullOrEmpty(field.Value))
                        {
                            builder.AddField(field.Key, field.Value);
                        }
                    }
                }
                
                // Set URL if provided
                if (!string.IsNullOrEmpty(data.Url))
                    builder.WithUrl(data.Url);
                
                // Set footer
                if (!string.IsNullOrEmpty(data.FooterText))
                    builder.WithFooter(data.FooterText);
                else
                    builder.WithFooter("ヘルプを参照中");
                
                // Set image if provided
                if (!string.IsNullOrEmpty(data.ImageUrl))
                    builder.WithImageUrl(data.ImageUrl);
                
                // Set thumbnail if provided
                if (!string.IsNullOrEmpty(data.ThumbnailUrl))
                    builder.WithThumbnailUrl(data.ThumbnailUrl);
                
                return builder;
            }
            catch (Exception ex)
            {
                _loggingService?.Log($"Error creating FAQ embed: {ex.Message}", "EmbedFactory", ImprovedLoggingService.LogLevel.Error);
                return CreateErrorEmbed(user, "FAQ embed creation failed");
            }
        }

        /// <summary>
        /// Creates a standardized error embed
        /// </summary>
        /// <param name="user">The user context for the embed</param>
        /// <param name="message">The error message to display</param>
        /// <returns>A configured EmbedBuilder instance</returns>
        public EmbedBuilder CreateErrorEmbed(IUser user, string message)
        {
            try
            {
                var builder = CreateBaseEmbed(user, Color.DarkRed);
                
                builder.WithTitle("エラー");
                builder.WithDescription(message ?? "不明なエラーが発生しました");
                builder.WithFooter("DeltaRaumi");
                
                return builder;
            }
            catch (Exception ex)
            {
                _loggingService?.Log($"Error creating error embed: {ex.Message}", "EmbedFactory", ImprovedLoggingService.LogLevel.Error);
                
                // Return absolute minimal fallback
                return new EmbedBuilder()
                    .WithAuthor(user)
                    .WithTitle("エラー")
                    .WithDescription("システムエラーが発生しました")
                    .WithColor(Color.DarkRed)
                    .WithCurrentTimestamp();
            }
        }

        /// <summary>
        /// Creates a status embed with dynamic fields
        /// </summary>
        /// <param name="user">The user context for the embed</param>
        /// <param name="title">The title of the status embed</param>
        /// <param name="description">The description of the status embed</param>
        /// <param name="fields">Optional dictionary of field names and values</param>
        /// <returns>A configured EmbedBuilder instance</returns>
        public EmbedBuilder CreateStatusEmbed(IUser user, string title, string description, Dictionary<string, string>? fields = null)
        {
            try
            {
                var builder = CreateBaseEmbed(user);
                
                if (!string.IsNullOrEmpty(title))
                    builder.WithTitle(title);
                
                if (!string.IsNullOrEmpty(description))
                    builder.WithDescription(description);
                
                // Add fields if provided
                if (fields != null)
                {
                    foreach (var field in fields)
                    {
                        if (!string.IsNullOrEmpty(field.Key) && !string.IsNullOrEmpty(field.Value))
                        {
                            builder.AddField(field.Key, field.Value);
                        }
                    }
                }
                
                builder.WithFooter("DeltaRaumi");
                
                return builder;
            }
            catch (Exception ex)
            {
                _loggingService?.Log($"Error creating status embed: {ex.Message}", "EmbedFactory", ImprovedLoggingService.LogLevel.Error);
                return CreateErrorEmbed(user, "Status embed creation failed");
            }
        }
    }
}