using FormBuilder.Core.Configuration;

using System.Runtime.Serialization;

namespace FormBuilder.Core.Models
{
    /// <summary>
    /// Defines a representation of the presented form designer.
    /// </summary>
    /// <remarks>
    /// This is a view model for the new Form Designer
    /// Which includes the same form properties but with addition of workflows
    /// We will use this in our API/Editor controller and then massage
    /// back into the Form and workflow model.
    /// Inherits from     /// </remarks>
    [Serializable]
    public class FormDesign : Form
    {
        /// <summary>
        /// Initializes a new instance of the         /// </summary>
        public FormDesign()
          : this(new DefaultFormSettings())
        {
        }

        /// <summary>
        /// Initializes a new instance of the         /// </summary>
        public FormDesign(DefaultFormSettings defaultSettings)
        {
            Pages = [];
            StoreRecordsLocally = true;
            SetFromDefaults(defaultSettings);
        }

        /// <summary>Gets or sets the form workflows.</summary>
        /// <remarks>
        /// An instance of         /// </remarks>
        [DataMember(Name = "formWorkflows")]
        public FormWorkflows FormWorkflows { get; set; } = new FormWorkflows();

        /// <summary>
        /// Gets or sets the path to the form in the tree as a comma separated list of values.
        /// </summary>
        [DataMember(Name = "path")]
        public string Path { get; set; } = string.Empty;
    }
}