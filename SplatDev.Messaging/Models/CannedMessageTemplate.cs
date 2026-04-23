namespace SplatDev.Messaging.Models
{
    using SplatDev.Messaging.Interfaces;

    using System.Collections.Generic;
    public class CannedMessageTemplate : ICannedMessageTemplate
    {
        public string TemplateName { get; set; }

        public string Body { get; set; }

        public string FormattedBody { get; set; }

        public IEnumerable<CannedMessagePlaceholder> Placeholders { get; set; }
    }
}
