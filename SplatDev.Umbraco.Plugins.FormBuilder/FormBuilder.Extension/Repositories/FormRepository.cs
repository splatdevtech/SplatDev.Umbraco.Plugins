using FormBuilder.Extension.Entities;
using FormBuilder.Extension.Interfaces;
using FormBuilder.Extension.Models;
using FormBuilder.Extension.Workflows;

using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Infrastructure.Persistence.Repositories.Implement;
using Umbraco.Cms.Infrastructure.Scoping;
using Umbraco.Extensions;

namespace FormBuilder.Extension.Repositories
{
    public class FormRepository(IScopeAccessor scopeAccessor, AppCaches appCaches) : RepositoryBase(scopeAccessor, appCaches), IFormRepository
    {

        public async Task<Form> CreateSubmissionAsync(FormSubmissionModel model)
        {
            // 1. Load the form definition (including fields and workflows)
            var form = await Database.SingleOrDefaultByIdAsync<Form>(model.FormId) ?? throw new InvalidOperationException("Form not found.");

            // 2. Load fields
            var fieldsQuery = Sql().Select<FormField>().From<FormField>().Where<FormField>(f => f.FormId == form.Id);
            var fields = await Database.FetchAsync<FormField>(fieldsQuery);
            form.Fields = fields;

            // 3. Load workflows
            var wfQuery = Sql().Select<Workflow>().From<Workflow>().Where<Workflow>(w => w.FormId == form.Id);
            List<Workflow> workflows = await Database.FetchAsync<Workflow>(wfQuery);
            form.Workflows = workflows;

            return form;
        }

        public async Task<Form> CreateAsync(Form form)
        {
            using var transaction = Database.GetTransaction();
            try
            {
                await Database.InsertAsync(form);
                await ProcessRelations(form);
                transaction.Complete();
                return form;
            }
            catch
            {
                transaction.Dispose();
                throw;
            }
        }

        public async Task DeleteAsync(int formId)
        {
            if (await HasSubmissionsAsync(formId))
                throw new InvalidOperationException("Form has existing submissions and cannot be deleted");

            using var transaction = Database.GetTransaction();
            try
            {
                await CleanRelations(formId);
                await Database.DeleteAsync(formId);
                transaction.Complete();
            }
            catch
            {
                transaction.Dispose();
                throw;
            }
        }

        public async Task<IEnumerable<Form>> GetAllAsync()
        {
            var query = Sql()
                .Select<Form>()
                .From<Form>()
                .OrderBy<Form>(f => f.Name);

            return await Database.FetchAsync<Form>(query);
        }

        public async Task<Form?> GetByIdAsync(int formId)
        {
            // 1. Get the form
            var form = await Database.SingleOrDefaultByIdAsync<Form>(formId);
            if (form == null)
                return null;

            // 2. Get fields
            var fieldsQuery2 = Sql().Select<FormField>().From<FormField>().Where<FormField>(f => f.FormId == formId).OrderBy<FormField>(f => f.SortOrder);
            var fields = await Database.FetchAsync<FormField>(fieldsQuery2);
            form.Fields = fields;

            // 3. Get workflows
            var wfQuery2 = Sql().Select<Workflow>().From<Workflow>().Where<Workflow>(w => w.FormId == formId);
            var workflows = await Database.FetchAsync<Workflow>(wfQuery2);
            form.Workflows = workflows;

            return form;
        }

        public async Task<bool> HasSubmissionsAsync(int formId)
        {
            var query = Sql()
                .Select("COUNT(*)")
                .From<FormSubmission>()
                .Where<FormSubmission>(s => s.FormId == formId);

            return await Database.ExecuteScalarAsync<int>(query) > 0;
        }

        public async Task<Form> UpdateAsync(Form form)
        {
            using var transaction = Database.GetTransaction();
            try
            {
                await Database.UpdateAsync(form);
                await CleanRelations(form.Id);
                await ProcessRelations(form);
                transaction.Complete();
                return form;
            }
            catch
            {
                transaction.Dispose();
                throw;
            }
        }
        public async Task UpdateOrderAsync(int formId, Dictionary<int, int> fieldOrder)
        {
            foreach (var kvp in fieldOrder)
            {
                var fieldId = kvp.Key;
                var sortOrder = kvp.Value;

                var field = await Database.SingleByIdAsync<FormField>(fieldId);
                field.SortOrder = sortOrder;
                await Database.UpdateAsync(field, ["SortOrder"]);
            }
        }

        private async Task CleanRelations(int formId)
        {
            await Database.DeleteAsync($"WHERE FormId = {formId}");
            await Database.DeleteAsync($"WHERE FormId = {formId}");
        }

        private async Task ProcessRelations(Form form)
        {
            foreach (var field in form.Fields)
            {
                field.FormId = form.Id;
                await Database.SaveAsync(field);
            }

            foreach (var workflow in form.Workflows)
            {
                workflow.FormId = form.Id;
                await Database.SaveAsync(workflow);
            }
        }
    }
}
