using FormBuilder.Core.Models;

namespace FormBuilder.Web.Behaviors.Interfaces
{
    /// <summary>
    /// Defines a behavior applying default workflows to new forms created by the editor.
    /// </summary>
    public interface IApplyDefaultWorkflowsBehavior
    {
        /// <summary>Applies workflows to a newly created form.</summary>
        void ApplyDefaultWorkflows(FormDesign form);
    }
}