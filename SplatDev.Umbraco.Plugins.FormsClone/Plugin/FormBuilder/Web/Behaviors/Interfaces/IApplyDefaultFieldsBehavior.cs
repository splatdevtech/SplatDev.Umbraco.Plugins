using FormBuilder.Core.Models;

namespace FormBuilder.Web.Behaviors.Interfaces
{
    /// <summary>
    /// Defines a behavior applying default fields to new forms created by the editor.
    /// </summary>
    public interface IApplyDefaultFieldsBehavior
    {
        /// <summary>Applies fields to a newly created form.</summary>
        void ApplyDefaultFields(FormDesign form);
    }
}