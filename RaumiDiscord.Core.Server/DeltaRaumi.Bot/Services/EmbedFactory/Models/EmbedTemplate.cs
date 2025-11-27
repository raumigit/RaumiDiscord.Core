using Discord;
using System.ComponentModel.DataAnnotations;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Bot.Services.EmbedFactory.Models
{
    /// <summary>
    /// Base class for embed templates containing common properties
    /// </summary>
    public abstract class EmbedTemplate
    {
        /// <summary>
        /// The title of the embed
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// The description text of the embed
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// The color of the embed
        /// </summary>
        public Color? Color { get; set; }

        /// <summary>
        /// The URL associated with the embed
        /// </summary>
        public string? Url { get; set; }

        /// <summary>
        /// The footer text for the embed
        /// </summary>
        public string? FooterText { get; set; }

        /// <summary>
        /// Whether to include a timestamp in the embed
        /// </summary>
        public bool IncludeTimestamp { get; set; } = true;

        /// <summary>
        /// The image URL for the embed
        /// </summary>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// The thumbnail URL for the embed
        /// </summary>
        public string? ThumbnailUrl { get; set; }
    }
}