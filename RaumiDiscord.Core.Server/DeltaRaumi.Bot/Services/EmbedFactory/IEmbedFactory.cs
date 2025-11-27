using Discord;
using RaumiDiscord.Core.Server.DeltaRaumi.Bot.Services.EmbedFactory.Models;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Bot.Services.EmbedFactory
{
    /// <summary>
    /// Interface for creating standardized Discord embeds
    /// </summary>
    public interface IEmbedFactory
    {
        /// <summary>
        /// Creates a base embed with common styling and user context
        /// </summary>
        /// <param name="user">The user context for the embed</param>
        /// <param name="color">Optional color override for the embed</param>
        /// <returns>A configured EmbedBuilder instance</returns>
        EmbedBuilder CreateBaseEmbed(IUser user, Color? color = null);

        /// <summary>
        /// Creates an FAQ-style embed using structured data
        /// </summary>
        /// <param name="user">The user context for the embed</param>
        /// <param name="data">The FAQ data containing title, description, fields, etc.</param>
        /// <returns>A configured EmbedBuilder instance</returns>
        EmbedBuilder CreateFAQEmbed(IUser user, FAQEmbedData data);

        /// <summary>
        /// Creates a standardized error embed
        /// </summary>
        /// <param name="user">The user context for the embed</param>
        /// <param name="message">The error message to display</param>
        /// <returns>A configured EmbedBuilder instance</returns>
        EmbedBuilder CreateErrorEmbed(IUser user, string message);

        /// <summary>
        /// Creates a status embed with dynamic fields
        /// </summary>
        /// <param name="user">The user context for the embed</param>
        /// <param name="title">The title of the status embed</param>
        /// <param name="description">The description of the status embed</param>
        /// <param name="fields">Optional dictionary of field names and values</param>
        /// <returns>A configured EmbedBuilder instance</returns>
        EmbedBuilder CreateStatusEmbed(IUser user, string title, string description, Dictionary<string, string>? fields = null);
    }
}