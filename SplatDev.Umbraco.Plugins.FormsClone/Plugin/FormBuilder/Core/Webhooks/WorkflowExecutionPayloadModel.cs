using FormBuilder.Core.Enums;

using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FormBuilder.Core.Webhooks
{
    public sealed record WorkflowExecutionPayloadModel
    {
        [Required]
        public required Guid WorkflowId { get; init; }

        [Required]
        public required string WorkflowName { get; init; }

        [Required]
        public required Guid FormId { get; init; }

        [Required]
        public required FormState RecordState { get; init; }

        [Required]
        public required Guid RecordId { get; init; }

        public WorkflowExecutionStatus Result { get; init; }

        [SetsRequiredMembers]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public WorkflowExecutionPayloadModel() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

        [SetsRequiredMembers]
        public WorkflowExecutionPayloadModel(
            Guid workflowId,
            string workflowName,
            Guid formId,
            FormState recordState,
            Guid recordId,
            WorkflowExecutionStatus result)
        {
            WorkflowId = workflowId;
            WorkflowName = workflowName;
            FormId = formId;
            RecordState = recordState;
            RecordId = recordId;
            Result = result;
        }
    }
}