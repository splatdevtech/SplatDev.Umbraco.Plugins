using FormBuilder.Core.Models;

namespace FormBuilder.Core.Services
{
    public interface IFormThemeResolver
    {
        string? GetDatePicker(string? theme);

        string? GetFieldView(FormViewModel form, FieldViewModel field);

        string? GetReadOnlyFieldView(FormViewModel form, FieldViewModel field);

        string? GetFormRender(FormViewModel form);

        string? GetFormSubmittedView(FormViewModel form);

        string? GetFormView(FormViewModel form);

        string? GetScriptView(FormViewModel form);

        string? GetMultiPageFormPagingDetailsView(FormViewModel form);

        string? GetMultiPageFormSummaryView(FormViewModel form);

        string? GetGenericReadOnlyFieldView(FormViewModel form);
    }
}