using Umbraco.Cms.Core.PropertyEditors;

namespace SplatDev.Umbraco.Plugins.CharLimit;

#if NET10_0_OR_GREATER
public class CharLimitConfigurationEditor(global::Umbraco.Cms.Core.IO.IIOHelper ioHelper) : ConfigurationEditor<CharLimitConfiguration>(ioHelper)
{
}
#else
public class CharLimitConfigurationEditor : ConfigurationEditor<CharLimitConfiguration>
{
    public CharLimitConfigurationEditor()
    {
        Fields.Add(new ConfigurationField { Key = "maxChars", Name = "Max Characters", View = "number" });
        Fields.Add(new ConfigurationField { Key = "showCountdown", Name = "Show Countdown", View = "boolean" });
    }
}
#endif
