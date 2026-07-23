using FormBuilder.Core.Enums;

namespace FormBuilder.Core.Interfaces
{
    public interface IFieldType
    {
        string Name { get; set; }

        string Alias { get; set; }

        Guid Id { get; set; }

        string Icon { get; set; }

        string RenderView { get; set; }

        string EditView { get; set; }

        string Category { get; set; }

        int SortOrder { get; set; }

        FieldDataType DataType { get; set; }

        bool SupportsPreValues { get; set; }

        bool SupportsMandatory { get; set; }

        bool MandatoryByDefault { get; set; }

        bool SupportsRegex { get; set; }

        RenderInputType RenderInputType { get; set; }

        bool StoresData { get; }

        bool HideLabel { get; set; }

        Dictionary<FieldConditionRuleOperator, Func<string, string, bool>> ConditionCheckFunctions { get; }

        List<Exception> ValidateSettings();
    }
}