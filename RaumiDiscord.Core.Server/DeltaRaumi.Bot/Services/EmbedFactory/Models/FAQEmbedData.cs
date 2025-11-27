using Discord;
using System.ComponentModel.DataAnnotations;

namespace RaumiDiscord.Core.Server.DeltaRaumi.Bot.Services.EmbedFactory.Models
{
    /// <summary>
    /// Data model for FAQ embed templates
    /// </summary>
    public class FAQEmbedData : EmbedTemplate
    {
        /// <summary>
        /// Dictionary of field names and values to be added to the embed
        /// </summary>
        public Dictionary<string, string>? Fields { get; set; }

        /// <summary>
        /// Creates a new instance of FAQEmbedData
        /// </summary>
        public FAQEmbedData()
        {
            Fields = new Dictionary<string, string>();
        }

        /// <summary>
        /// Creates a new instance of FAQEmbedData with specified properties
        /// </summary>
        /// <param name="title">The title of the FAQ embed</param>
        /// <param name="description">The description of the FAQ embed</param>
        /// <param name="url">Optional URL for the embed</param>
        /// <param name="footerText">Optional footer text</param>
        public FAQEmbedData(string title, string description, string? url = null, string? footerText = null)
        {
            Title = title;
            Description = description;
            Url = url;
            FooterText = footerText;
            Fields = new Dictionary<string, string>();
        }

        /// <summary>
        /// Adds a field to the FAQ embed
        /// </summary>
        /// <param name="name">The field name</param>
        /// <param name="value">The field value</param>
        /// <returns>This instance for method chaining</returns>
        public FAQEmbedData AddField(string name, string value)
        {
            Fields ??= new Dictionary<string, string>();
            Fields[name] = value;
            return this;
        }
    }
}