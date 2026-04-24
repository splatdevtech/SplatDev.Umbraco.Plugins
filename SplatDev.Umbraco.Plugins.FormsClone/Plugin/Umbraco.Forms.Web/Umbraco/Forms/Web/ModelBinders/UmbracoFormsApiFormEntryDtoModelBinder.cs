
// Type: Umbraco.Forms.Web.ModelBinders.UmbracoFormsApiFormEntryDtoModelBinder
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Umbraco.Forms.Core.Models.DeliveryApi;


#nullable enable
namespace Umbraco.Forms.Web.ModelBinders
{
  public class UmbracoFormsApiFormEntryDtoModelBinder : IModelBinder
  {
    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
      if (bindingContext == null)
        throw new ArgumentNullException(nameof (bindingContext));
      using (StreamReader reader = new StreamReader(bindingContext.HttpContext.Request.Body))
      {
        FormEntryDto model = JsonSerializer.Deserialize<FormEntryDto>(await reader.ReadToEndAsync(bindingContext.HttpContext.RequestAborted).ConfigureAwait(false));
        if (model != null)
          bindingContext.Result = ModelBindingResult.Success((object) model);
      }
    }
  }
}
