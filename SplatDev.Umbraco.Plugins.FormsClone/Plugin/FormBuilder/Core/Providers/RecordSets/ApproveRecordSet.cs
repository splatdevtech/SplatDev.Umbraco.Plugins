using FormBuilder.Core.Enums;
using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Fields;
using FormBuilder.Core.Services.Interfaces;

namespace FormBuilder.Core.Providers.RecordSets
{
    /// <summary>
    /// Provides a     /// </summary>
    public class ApproveRecordSet : RecordSetActionType
    {
        private readonly IRecordService _recordService;

        /// <summary>
        /// Initializes a new instance of the         /// </summary>
        public ApproveRecordSet(IRecordService recordService)
        {
            _recordService = recordService;
            Description = "Approves a set of records";
            Icon = "icon-check";
            Id = new Guid("CB126B79-9011-11DF-A4EE-0800200C9A66");
            Name = "Approve";
            Alias = "approve";
            IsAvailableForApprovedRecords = false;
        }

        /// <inheritdoc />
        public override List<Exception> ValidateSettings() => [];

        /// <inheritdoc />
        public override async Task<RecordActionStatus> ExecuteAsync(
          List<Record> records,
          Form form)
        {
            foreach (Record record in records)
            {
                if (form.Id == record.Form)
                    await _recordService.ApproveAsync(record, form).ConfigureAwait(false);
            }
            return RecordActionStatus.Completed;
        }
    }
}