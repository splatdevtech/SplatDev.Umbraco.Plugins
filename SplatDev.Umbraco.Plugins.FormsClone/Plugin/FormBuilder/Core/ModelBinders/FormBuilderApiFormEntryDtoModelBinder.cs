using FormBuilder.Core.Dto;

using Microsoft.AspNetCore.Mvc.ModelBinding;

using System.Text.Json;

namespace FormBuilder.Core.ModelBinders
{
    /// <summary>
    /// Custom model binder to be used for handling posted form submissions to a type of     /// </summary>
    public class FormBuilderApiFormEntryDtoModelBinder : IModelBinder
    {
        /// <inheritdoc />
        public async Task BindModelAsync(ModelBindingContext bindingContext)
        {
            ArgumentNullException.ThrowIfNull(bindingContext);
            using StreamReader reader = new(bindingContext.HttpContext.Request.Body);
            FormEntryDto? model = JsonSerializer.Deserialize<FormEntryDto>(await reader.ReadToEndAsync(bindingContext.HttpContext.RequestAborted).ConfigureAwait(false));
            if (model is not null)
                bindingContext.Result = ModelBindingResult.Success(model);
        }
    }
}