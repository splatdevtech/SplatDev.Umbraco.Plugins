// Type: Umbraco.Forms.Examine.Indexes.FormsIndexPopulator
// Assembly: Umbraco.Forms.Examine, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: EDF5A33E-94A1-42C9-B681-695454D27A51


#nullable enable
using Examine;

using Umbraco.Cms.Infrastructure.Examine;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;
using Umbraco.Forms.Core.Services;

namespace Umbraco.Forms.Examine.Indexes
{
    internal sealed class FormsIndexPopulator(
      IRecordService recordService,
      IFormService formService,
      IValueSetBuilder<Record> valueSetBuilder) : IndexPopulator<IUmbracoFormsRecordIndex>
    {
        private readonly IFormService _formService = formService;
        private readonly IRecordService _recordService = recordService;
        private readonly IValueSetBuilder<Record> _valueSetBuilder = valueSetBuilder;

        protected override void PopulateIndexes(IReadOnlyList<IIndex> indexes)
        {
            if (indexes.Count == 0)
                return;
            IEnumerable<Form> forms = this._formService.Get();
            bool flag = false;
            foreach (Form form in forms)
            {
                Record[] array1 = this._recordService.GetAllRecords(form).ToArray<Record>();
                if (array1.Length != 0)
                {
                    ValueSet[] array2 = this._valueSetBuilder.GetValueSets(array1).ToArray<ValueSet>();
                    foreach (IIndex index in (IEnumerable<IIndex>)indexes)
                        index.IndexItems((IEnumerable<ValueSet>)array2);
                    flag = true;
                }
            }
            if (flag)
                return;
            foreach (IIndex index in (IEnumerable<IIndex>)indexes)
                index.IndexItems((IEnumerable<ValueSet>)Array.Empty<ValueSet>());
        }
    }
}
