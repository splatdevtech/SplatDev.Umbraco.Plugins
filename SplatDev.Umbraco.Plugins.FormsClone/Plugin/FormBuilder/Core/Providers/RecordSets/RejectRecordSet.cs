using FormBuilder.Core.Enums;
using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Fields;
using FormBuilder.Core.Services.Interfaces;

namespace FormBuilder.Core.Providers.RecordSets
{
    public class RejectRecordSet : RecordSetActionType
    {
        private readonly IRecordService _recordService;

        /// <summary>
        /// Initializes a new instance of the         /// </summary>
        /// <param name="recordService"></param>
        public RejectRecordSet(IRecordService recordService)
        {
            _recordService = recordService;
            Description = "Rejects a set of records";
            Icon = "icon-wrong";
            Id = new Guid("84cd75a7-d3d9-4551-9c1a-3f478b4ec9ed");
            Name = "Reject";
            Alias = "reject";
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
                    await _recordService.RejectAsync(record, form).ConfigureAwait(false);
            }
            return RecordActionStatus.Completed;
        }
    }
}