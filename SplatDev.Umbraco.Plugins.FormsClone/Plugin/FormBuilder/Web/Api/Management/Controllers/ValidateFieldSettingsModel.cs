using FormBuilder.Core.Models;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>Model POSTed to validate field settings.</summary>
    public class ValidateFieldSettingsModel
    {
        /// <summary>Gets or sets the field's caption.</summary>
        public string Caption { get; set; } = string.Empty;

        /// <summary>Gets or sets the field's alias.</summary>
        public string Alias { get; set; } = string.Empty;

        /// <summary>Gets or sets the field's settings.</summary>
        public IDictionary<string, string> Settings { get; set; } = new Dictionary<string, string>();

        /// <summary>Gets or sets the allowed upload types for the field.</summary>
        public IEnumerable<AllowedUploadType>? AllowedUploadTypes { get; set; }
    }
}