using Umbraco.Cms.Core.PropertyEditors;

namespace UmbracoCms.Plugins.CharLimit;

public class CharLimitConfigurationEditor : ConfigurationEditor<CharLimitConfiguration>
{
    public CharLimitConfigurationEditor() : base()
    {
        Fields.Add(new ConfigurationField { Key = "maxChars", Name = "Max Characters", View = "number" });
        Fields.Add(new ConfigurationField { Key = "showCountdown", Name = "Show Countdown", View = "boolean" });
    }
}
