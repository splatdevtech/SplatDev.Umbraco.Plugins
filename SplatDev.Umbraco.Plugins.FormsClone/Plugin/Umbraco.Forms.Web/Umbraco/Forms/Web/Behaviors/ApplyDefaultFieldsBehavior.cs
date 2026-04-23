
// Type: Umbraco.Forms.Web.Behaviors.ApplyDefaultFieldsBehavior
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.Extensions.Options;

using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Providers;
using Umbraco.Forms.Web.Extensions;
using Umbraco.Forms.Web.Models.Backoffice;


#nullable enable
namespace Umbraco.Forms.Web.Behaviors
{
    internal class ApplyDefaultFieldsBehavior : IApplyDefaultFieldsBehavior
    {
        private readonly FormDesignSettings _formDesignSettings;
        private readonly FieldCollection _fieldCollection;

        public ApplyDefaultFieldsBehavior(
          IOptions<FormDesignSettings> formDesignSettings,
          FieldCollection fieldCollection)
        {
            this._formDesignSettings = formDesignSettings.Value;
            this._fieldCollection = fieldCollection;
        }

        public virtual void ApplyDefaultFields(FormDesign form)
        {
            Page page = new Page();
            form.Pages.Add(page);
            FieldSet fieldSet = new FieldSet()
            {
                Id = Guid.NewGuid()
            };
            page.FieldSets.Add(fieldSet);
            FieldsetContainer container = new FieldsetContainer()
            {
                Width = 12
            };
            fieldSet.Containers.Add(container);
            if (this._formDesignSettings.DisableAutomaticAdditionOfDataConsentField)
                return;
            container.AddDataConsentField(this._formDesignSettings, this._fieldCollection);
        }
    }
}
