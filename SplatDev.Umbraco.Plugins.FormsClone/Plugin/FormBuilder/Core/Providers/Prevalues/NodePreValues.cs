using FormBuilder.Core.Attributes;
using FormBuilder.Core.Models;
using FormBuilder.Core.Prevalues;
using FormBuilder.Core.Services.Interfaces;

using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;

namespace FormBuilder.Core.Providers.Prevalues
{
    /// <summary>
    /// Defines a     /// </summary>
    public class NodePreValues : FieldPrevalueSourceType
    {
        private readonly IContentService _contentService;
        private readonly IUmbracoContextAccessor _umbracoContextAccessor;
        private readonly IDynamicRootContentLocator _dynamicRootContentLocator;

        /// <summary>
        /// Initializes a new instance of the         /// </summary>
        public NodePreValues(
          IContentService contentService,
          IUmbracoContextAccessor umbracoContextAccessor,
          IDynamicRootContentLocator dynamicRootContentLocator)
        {
            Id = new Guid("DE996870-C45A-11DE-8A39-0800200C9A66");
            Name = "Umbraco Documents";
            Alias = "umbracoDocuments";
            Description = "Uses nodes from a specific source as preValues";
            Icon = "icon-documents";
            _contentService = contentService;
            _umbracoContextAccessor = umbracoContextAccessor;
            _dynamicRootContentLocator = dynamicRootContentLocator;
        }

        /// <summary>Gets or sets the root node to fetch nodes from.</summary>
        [Setting("Root node", Description = "Source to fetch nodes from", DisplayOrder = 10, View = "Umb.PropertyEditorUi.ContentPicker.Source")]
        public virtual string RootNode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether to use the current page as the node source.
        /// </summary>
        [Setting("Use current page as root", Description = "Does not work in preview mode", DisplayOrder = 20, View = "Umb.PropertyEditorUi.Toggle")]
        public virtual string UseCurrentPage { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the document type alias of the nodes to use.
        /// </summary>
        [Setting("Document type", Description = "The type of nodes you would like to use", DisplayOrder = 30, View = "Forms.PropertyEditorUi.DocumentTypePicker")]
        public virtual string DocType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the field to use for the value of the prevalue.
        /// </summary>
        [Setting("Value field", Description = "Select which field should be used for the value of the prevalue", DisplayOrder = 35, PreValues = "DocType", View = "Forms.PropertyEditorUi.DocumentTypeFieldPicker")]
        public virtual string ValueField { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the field to use for the caption of the prevalue.
        /// </summary>
        [Setting("Caption field", Description = "Select which field should be used for the caption of the prevalue", DisplayOrder = 36, PreValues = "DocType", View = "Forms.PropertyEditorUi.DocumentTypeFieldPicker")]
        public virtual string CaptionField { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether to not limit to children but also to include other levels.
        /// </summary>
        [Setting("List all descendants", Description = "Don't limit to children but also include other levels", DisplayOrder = 40, View = "Umb.PropertyEditorUi.Toggle")]
        public virtual string ListGrandChildren { get; set; } = string.Empty;

        /// <summary>Gets or sets the ordering options for the prevalues.</summary>
        [Setting("Order by", Description = "Select how the prevalue list should be ordered", DisplayOrder = 50, PreValues = "Node order,Alphabetical", View = "Umb.PropertyEditorUi.Dropdown")]
        public virtual string OrderBy { get; set; } = string.Empty;

        /// <inheritdoc />
        public override Task<List<Prevalue>> GetPreValuesAsync(Field? field, Form? form)
        {
            _ = bool.TryParse(UseCurrentPage, out bool result1);
            _ = bool.TryParse(ListGrandChildren, out bool result2);
            List<Prevalue> preValueList = [];
            _umbracoContextAccessor.TryGetUmbracoContext(out IUmbracoContext? umbracoContext);
            int? id = umbracoContext?.PublishedRequest?.PublishedContent?.Id;
            if (!id.HasValue || id.Value == 0)
            {
                id = _contentService.GetRootContent().FirstOrDefault()?.Id;
                if (!id.HasValue || id.Value == 0)
                    return Task.FromResult(preValueList);
            }
            IContent? content = !result1 || umbracoContext == null ? _dynamicRootContentLocator.GetContent(RootNode, id.Value).GetAwaiter().GetResult() : _contentService.GetById(id.Value);
            if (content == null)
                return Task.FromResult(preValueList);
            if (_contentService.HasChildren(content.Id))
            {
                foreach (IContent pagedChild in _contentService.GetPagedChildren(content.Id, 0L, int.MaxValue, out _))
                {
                    if (pagedChild.Published)
                    {
                        if (pagedChild.ContentType.Alias == DocType)
                        {
                            Prevalue preValue = new()
                            {
                                Id = pagedChild.Id.ToString(),
                                Value = GetFieldValue(pagedChild, ValueField),
                                Caption = GetFieldValue(pagedChild, CaptionField)
                            };
                            preValueList.Add(preValue);
                        }
                        if (result2)
                            LoopChildren(pagedChild, preValueList);
                    }
                }
            }
            return OrderBy == "Alphabetical" ? Task.FromResult<List<Prevalue>>([.. preValueList.OrderBy(x => x.Caption)]) : Task.FromResult(preValueList);
        }

        private static string GetFieldValue(IContent content, string field)
        {
            if (field == "Id")
                return content.Id.ToString();
            if (field == "Key")
                return content.Key.ToString();
            return field switch
            {
                "" or "Name" => content.Name ?? string.Empty,
                _ => (content.HasProperty(field) ? content.GetValue(field)?.ToString() : content.Name) ?? string.Empty,
            };
        }

        private void LoopChildren(IContent parent, ICollection<Prevalue> preValues)
        {
            foreach (IContent content in _contentService.GetPagedChildren(parent.Id, 0L, int.MaxValue, out long _).ToArray())
            {
                if (content.Published)
                {
                    if (content.ContentType.Alias == DocType)
                    {
                        Prevalue preValue = new()
                        {
                            Id = content.Id.ToString(),
                            Value = GetFieldValue(content, ValueField),
                            Caption = GetFieldValue(content, CaptionField)
                        };
                        preValues.Add(preValue);
                    }
                    LoopChildren(content, preValues);
                }
            }
        }

        /// <inheritdoc />
        public override List<Exception> ValidateSettings()
        {
            List<Exception> exceptionList = [];
            _ = bool.TryParse(UseCurrentPage, out bool result);
            if (!result && string.IsNullOrEmpty(RootNode))
                exceptionList.Add(new Exception("'Root node' setting has not been set and is required unless `Use current page as root' is selected."));
            return exceptionList;
        }
    }
}