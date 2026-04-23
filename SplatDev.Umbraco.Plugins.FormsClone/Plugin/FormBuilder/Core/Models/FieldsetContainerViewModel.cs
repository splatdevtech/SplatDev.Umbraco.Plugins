namespace FormBuilder.Core.Models
{
    /// <summary>Defines a view model for a fieldset container.</summary>
    [Serializable]
    public class FieldsetContainerViewModel
    {
        /// <summary>Gets or sets the container's caption.</summary>
        public string Caption { get; set; } = string.Empty;

        /// <summary>Gets or sets the container's width.</summary>
        public int Width { get; set; }

        /// <summary>Gets or sets the container's fields.</summary>
        public IList<FieldViewModel> Fields { get; set; } = [];
    }
}