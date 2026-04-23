namespace SplatDev.Messaging.Helpers
{
    using SplatDev.Messaging.Interfaces;
    using SplatDev.Messaging.Models;

    using System.Linq;
    using System.Reflection;
    public static class CannedMessageHelpers
    {
        /// <summary>
        /// Whether to use A templated mail message
        /// </summary>
        /// <param name="template">The text to be used in the message</param>
        /// <param name="values">The values to be replaced in the template</param>
        /// <returns>returns the full template with the replaced text</returns>
        /// <remarks>replaceable values should be surrounded by # (pound) i.e. #VALUE#</remarks>
        public static string UsingTemplate(string template, object values)
        {
            foreach (var prop in values.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
                template = template.Replace($"#{prop.Name}#", values.GetType().GetProperty(prop.Name).GetValue(values, null).ToString());
            return template;
        }

        /// <summary>
        /// Generates a message with replaced values
        /// </summary>
        /// <param name="templateName">The name of the template (from the list of templates)</param>
        /// <returns>The message formatted and with values replaced</returns>
        public static string GenerateMessageFromTemplate(string templateName, ICannedMessageTemplate canned = null)
        {
            ICannedMessageTemplate cannedMessage = canned ?? CannedMessageTemplates.Templates.Find(x => x.TemplateName == templateName);

            foreach (var val in cannedMessage.Placeholders)
            {
                cannedMessage.FormattedBody = cannedMessage.Body.Replace(val.Key, val.Value);
            }
            return cannedMessage.FormattedBody;
        }

        /// <summary>
        /// Generates the message from a canned message.
        /// </summary>
        /// <param name="canned">The canned message.</param>
        /// <returns>The canned message formatted and with values replaced</returns>
        public static string GenerateMessageFromCanned(ICannedMessageTemplate canned)
        {
            canned.FormattedBody = canned.Body;

            if (canned.Placeholders?.Any() != true)
            {
                return canned.Body;
            }
            else
            {
                foreach (var val in canned.Placeholders)
                {
                    canned.FormattedBody = canned.FormattedBody.Replace(val.Key, val.Value);
                }
                return canned.FormattedBody;
            }
        }
    }
}
