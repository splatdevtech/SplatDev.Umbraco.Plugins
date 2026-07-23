using Examine;

using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Fields;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Examine.Handlers;

using Umbraco.Cms.Infrastructure.Examine;

namespace FormBuilder.Examine.IndexPopulators
{
    internal sealed class FormsIndexPopulator(
      IRecordService recordService,
      IFormService formService,
      IValueSetBuilder<Record> valueSetBuilder) : IndexPopulator<IFormBuildersRecordIndex>
    {
        private readonly IFormService _formService = formService;
        private readonly IRecordService _recordService = recordService;
        private readonly IValueSetBuilder<Record> _valueSetBuilder = valueSetBuilder;

        protected override void PopulateIndexes(IReadOnlyList<IIndex> indexes)
        {
            if (indexes.Count == 0)
                return;
            IEnumerable<Form> forms = _formService.Get();
            bool flag = false;
            foreach (Form form in forms)
            {
                Record[] array1 = [.. _recordService.GetAllRecords(form)];
                if (array1.Length != 0)
                {
                    ValueSet[] array2 = [.. _valueSetBuilder.GetValueSets(array1)];
                    foreach (IIndex index in (IEnumerable<IIndex>)indexes)
                        index.IndexItems((IEnumerable<ValueSet>)array2);
                    flag = true;
                }
            }
            if (flag)
                return;
            foreach (IIndex index in (IEnumerable<IIndex>)indexes)
                index.IndexItems((IEnumerable<ValueSet>)[]);
        }
    }
}