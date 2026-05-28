#if !NET10_0_OR_GREATER

namespace FormBuilder.Extension.Composers;

public class FormBuilderSection : global::Umbraco.Cms.Core.Models.Section
{
    public override string Alias => "formBuilder";

    public override string Name => "Form Builder";
}

#endif
