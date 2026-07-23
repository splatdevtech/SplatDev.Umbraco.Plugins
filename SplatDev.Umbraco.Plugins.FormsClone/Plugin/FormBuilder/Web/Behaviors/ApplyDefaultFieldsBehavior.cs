using FormBuilder.Core.Configuration;
using FormBuilder.Core.Models;
using FormBuilder.Core.Providers.Collections;
using FormBuilder.Web.Behaviors.Interfaces;
using FormBuilder.Web.Extensions;

using Microsoft.Extensions.Options;

namespace FormBuilder.Web.Behaviors
{
    /// <summary>
    /// Default implementation of the     /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    internal class ApplyDefaultFieldsBehavior(
      IOptions<FormDesignSettings> formDesignSettings,
      FieldCollection fieldCollection) : IApplyDefaultFieldsBehavior
    {
        private readonly FormDesignSettings _formDesignSettings = formDesignSettings.Value;
        private readonly FieldCollection _fieldCollection = fieldCollection;

        /// <inheritdoc />
        public virtual void ApplyDefaultFields(FormDesign form)
        {
            Page page = new();
            form.Pages.Add(page);
            FieldSet fieldSet = new()
            {
                Id = Guid.NewGuid()
            };
            page.FieldSets.Add(fieldSet);
            FieldsetContainer container = new()
            {
                Width = 12
            };
            fieldSet.Containers.Add(container);
            if (_formDesignSettings.DisableAutomaticAdditionOfDataConsentField)
                return;
            container.AddDataConsentField(_formDesignSettings, _fieldCollection);
        }
    }
}