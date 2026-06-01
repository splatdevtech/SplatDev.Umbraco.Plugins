using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.IO;

namespace SplatDev.Umbraco.Plugins.CharLimit;

public class CharLimitConfigurationEditor : ConfigurationEditor<CharLimitConfiguration>
{
    public CharLimitConfigurationEditor(IIOHelper ioHelper) : base(ioHelper)
    {
#if NET10_0_OR_GREATER
        Fields.Add(new ConfigurationField
        {
            Key = "maxChars",
            PropertyName = "Max Characters",
        });
        Fields.Add(new ConfigurationField
        {
            Key = "showCountdown",
            PropertyName = "Show Countdown",
        });
#else
        Fields.Add(new ConfigurationField { Key = "maxChars", Name = "Max Characters", View = "number" });
        Fields.Add(new ConfigurationField { Key = "showCountdown", Name = "Show Countdown", View = "boolean" });
#endif
    }
}
