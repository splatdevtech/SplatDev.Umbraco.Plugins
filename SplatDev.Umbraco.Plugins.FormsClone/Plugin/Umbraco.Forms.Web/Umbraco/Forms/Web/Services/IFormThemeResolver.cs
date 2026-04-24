
// Type: Umbraco.Forms.Web.Services.IFormThemeResolver
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Umbraco.Forms.Web.Models;


#nullable enable
namespace Umbraco.Forms.Web.Services
{
  public interface IFormThemeResolver
  {
    string GetDatePicker(string? theme);

    string GetFieldView(FormViewModel form, FieldViewModel field);

    string GetReadOnlyFieldView(FormViewModel form, FieldViewModel field);

    string GetFormRender(FormViewModel form);

    string GetFormSubmittedView(FormViewModel form);

    string GetFormView(FormViewModel form);

    string GetScriptView(FormViewModel form);

    string GetMultiPageFormPagingDetailsView(FormViewModel form);

    string GetMultiPageFormSummaryView(FormViewModel form);

    string GetGenericReadOnlyFieldView(FormViewModel form);
  }
}
