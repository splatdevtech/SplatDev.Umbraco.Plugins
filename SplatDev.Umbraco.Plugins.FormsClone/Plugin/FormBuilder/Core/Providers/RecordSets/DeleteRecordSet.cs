using FormBuilder.Core.Enums;
using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Fields;
using FormBuilder.Core.Services.Interfaces;

namespace FormBuilder.Core.Providers.RecordSets
{
    /// <summary>
    /// Provides a     /// </summary>
    public class DeleteRecordSet : RecordSetActionType
    {
        private readonly IRecordService _recordService;

        /// <summary>
        /// Initializes a new instance of the         /// </summary>
        public DeleteRecordSet(IRecordService recordService)
        {
            _recordService = recordService;
            Description = "Deletes a set of records";
            Icon = "icon-trash";
            Id = new Guid("CB126B70-9011-11DF-A4EE-0800200C9A66");
            Name = "Delete";
            Alias = "delete";
        }

        /// <inheritdoc />
        public override bool NeedsConfirm => true;

        /// <inheritdoc />
        public override string ConfirmMessage => "@formRecordSetActions_deleteConfirm";

        /// <inheritdoc />
        public override List<Exception> ValidateSettings() => [];

        /// <inheritdoc />
        public override async Task<RecordActionStatus> ExecuteAsync(
          List<Record> records,
          Form form)
        {
            foreach (Record record in records)
                await _recordService.DeleteAsync(record, form).ConfigureAwait(false);
            return RecordActionStatus.Completed;
        }
    }
}