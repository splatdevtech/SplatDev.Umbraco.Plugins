
// Type: Umbraco.Forms.Core.Providers.PreValues.NodePreValues
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68

using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Core.Providers.PreValues
{
    public class NodePreValues : FieldPreValueSourceType
    {
        private readonly IContentService _contentService;
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;
        private readonly IDynamicRootContentLocator _dynamicRootContentLocator;

        public NodePreValues(
          IContentService contentService,
          IUmbracoContextAccessor umbracoContextAccessor,
          IDynamicRootContentLocator dynamicRootContentLocator)
        {
            this.Id = new Guid("DE996870-C45A-11DE-8A39-0800200C9A66");
            this.Name = "Umbraco Documents";
            this.Alias = "umbracoDocuments";
            this.Description = "Uses nodes from a specific source as preValues";
            this.Icon = "icon-documents";
            this._contentService = contentService;
            this._umbracoContextAccessor = umbracoContextAccessor;
            this._dynamicRootContentLocator = dynamicRootContentLocator;
        }

        [Setting("Root node", Description = "Source to fetch nodes from", DisplayOrder = 10, View = "Umb.PropertyEditorUi.ContentPicker.Source")]
        public virtual string RootNode { get; set; } = string.Empty;

        [Setting("Use current page as root", Description = "Does not work in preview mode", DisplayOrder = 20, View = "Umb.PropertyEditorUi.Toggle")]
        public virtual string UseCurrentPage { get; set; } = string.Empty;

        [Setting("Document type", Description = "The type of nodes you would like to use", DisplayOrder = 30, View = "Forms.PropertyEditorUi.DocumentTypePicker")]
        public virtual string DocType { get; set; } = string.Empty;

        [Setting("Value field", Description = "Select which field should be used for the value of the prevalue", DisplayOrder = 35, PreValues = "DocType", View = "Forms.PropertyEditorUi.DocumentTypeFieldPicker")]
        public virtual string ValueField { get; set; } = string.Empty;

        [Setting("Caption field", Description = "Select which field should be used for the caption of the prevalue", DisplayOrder = 36, PreValues = "DocType", View = "Forms.PropertyEditorUi.DocumentTypeFieldPicker")]
        public virtual string CaptionField { get; set; } = string.Empty;

        [Setting("List all descendants", Description = "Don't limit to children but also include other levels", DisplayOrder = 40, View = "Umb.PropertyEditorUi.Toggle")]
        public virtual string ListGrandChildren { get; set; } = string.Empty;

        [Setting("Order by", Description = "Select how the prevalue list should be ordered", DisplayOrder = 50, PreValues = "Node order,Alphabetical", View = "Umb.PropertyEditorUi.Dropdown")]
        public virtual string OrderBy { get; set; } = string.Empty;

        public override Task<List<PreValue>> GetPreValuesAsync(Field? field, Form? form)
        {
            bool result1;
            bool.TryParse(this.UseCurrentPage, out result1);
            bool result2;
            bool.TryParse(this.ListGrandChildren, out result2);
            List<PreValue> preValueList = new List<PreValue>();
            IUmbracoContext umbracoContext;
            this._umbracoContextAccessor.TryGetUmbracoContext(out umbracoContext);
            int? id = umbracoContext?.PublishedRequest?.PublishedContent?.Id;
            if (!id.HasValue || id.Value == 0)
            {
                id = this._contentService.GetRootContent().FirstOrDefault<IContent>()?.Id;
                if (!id.HasValue || id.Value == 0)
                    return Task.FromResult<List<PreValue>>(preValueList);
            }
            IContent content = !result1 || umbracoContext == null ? this._dynamicRootContentLocator.GetContent(this.RootNode, id.Value).GetAwaiter().GetResult() : this._contentService.GetById(id.Value);
            if (content == null)
                return Task.FromResult<List<PreValue>>(preValueList);
            if (this._contentService.HasChildren(content.Id))
            {
                foreach (IContent pagedChild in this._contentService.GetPagedChildren(content.Id, 0L, int.MaxValue, out long _))
                {
                    if (pagedChild.Published)
                    {
                        if (pagedChild.ContentType.Alias == this.DocType)
                        {
                            PreValue preValue = new PreValue()
                            {
                                Id = pagedChild.Id.ToString(),
                                Value = NodePreValues.GetFieldValue(pagedChild, this.ValueField),
                                Caption = NodePreValues.GetFieldValue(pagedChild, this.CaptionField)
                            };
                            preValueList.Add(preValue);
                        }
                        if (result2)
                            this.LoopChildren(pagedChild, preValueList);
                    }
                }
            }
            return this.OrderBy == "Alphabetical" ? Task.FromResult<List<PreValue>>(preValueList.OrderBy<PreValue, string>(x => x.Caption).ToList<PreValue>()) : Task.FromResult<List<PreValue>>(preValueList);
        }

        private static string GetFieldValue(IContent content, string field)
        {
            if (field == "Id")
                return content.Id.ToString();
            if (field == "Key")
                return content.Key.ToString();
            switch (field)
            {
                case "":
                case "Name":
                    return content.Name ?? string.Empty;
                default:
                    return (content.HasProperty(field) ? content.GetValue(field)?.ToString() : content.Name) ?? string.Empty;
            }
        }

        private void LoopChildren(IContent parent, ICollection<PreValue> preValues)
        {
            foreach (IContent content in this._contentService.GetPagedChildren(parent.Id, 0L, int.MaxValue, out long _).ToArray<IContent>())
            {
                if (content.Published)
                {
                    if (content.ContentType.Alias == this.DocType)
                    {
                        PreValue preValue = new PreValue()
                        {
                            Id = content.Id.ToString(),
                            Value = NodePreValues.GetFieldValue(content, this.ValueField),
                            Caption = NodePreValues.GetFieldValue(content, this.CaptionField)
                        };
                        preValues.Add(preValue);
                    }
                    this.LoopChildren(content, preValues);
                }
            }
        }

        public override List<Exception> ValidateSettings()
        {
            List<Exception> exceptionList = new List<Exception>();
            bool result;
            bool.TryParse(this.UseCurrentPage, out result);
            if (!result && string.IsNullOrEmpty(this.RootNode))
                exceptionList.Add(new Exception("'Root node' setting has not been set and is required unless `Use current page as root' is selected."));
            return exceptionList;
        }
    }
}
