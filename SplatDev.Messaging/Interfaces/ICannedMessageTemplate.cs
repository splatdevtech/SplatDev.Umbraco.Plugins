namespace SplatDev.Messaging.Interfaces
{
    using SplatDev.Messaging.Models;

    using System.Collections.Generic;
    public interface ICannedMessageTemplate
    {
        string TemplateName { get; set; }
        string Body { get; set; }
        string FormattedBody { get; set; }
        IEnumerable<CannedMessagePlaceholder> Placeholders { get; set; }
    }
}
