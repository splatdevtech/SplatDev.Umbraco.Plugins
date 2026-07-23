namespace SplatDev.Messaging.Models
{
    using SplatDev.Messaging.Interfaces;

    using System.Collections.Generic;
    using System.Linq;
    public static class CannedMessageTemplates
    {
        private static List<ICannedMessageTemplate> templates;

        public static List<CannedMessagePlaceholder> DefaultPlaceholders { get; set; }

        public static List<ICannedMessageTemplate> Templates
        {
            get
            {
                templates = templates ?? new List<ICannedMessageTemplate>();
                return templates;
            }
        }

        public static ICannedMessageTemplate Template(string key)
        {
            return templates.Find(x => x.TemplateName == key);
        }

        public static CannedMessagePlaceholder TemplatePlaceholder(string template, string key)
        {
            return Template(template).Placeholders.ToList().Find(x => x.Key == key);
        }

        public static CannedMessagePlaceholder Placeholder(string key)
        {
            if (DefaultPlaceholders == null) DefaultPlaceholders = new List<CannedMessagePlaceholder>();
            return DefaultPlaceholders.Find(x => x.Key == key);
        }
    }
}
