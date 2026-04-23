using FormBuilder.Core.Enums;
using FormBuilder.Core.FieldTypes;

using Umbraco.Cms.Core.Hosting;

namespace FormBuilder.Core.Models
{
    /// <summary>Defines a view model for a page.</summary>
    [Serializable]
    public class PageViewModel
    {
        /// <summary>Gets or sets the page's caption.</summary>
        public string Caption { get; set; } = string.Empty;

        /// <summary>Gets or sets the page's fieldsets.</summary>
        public IList<FieldsetViewModel> Fieldsets { get; set; } = [];

        /// <summary>Gets or sets the page's Id.</summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the page has a condition for button display.
        /// </summary>
        public bool HasButtonCondition { get; set; }

        /// <summary>Gets or sets the page's button condition's action type.</summary>
        public FieldConditionActionType ButtonConditionActionType { get; set; }

        /// <summary>Gets or sets the page's button condition's logic type.</summary>
        public FieldConditionLogicType ButtonConditionLogicType { get; set; }

        /// <summary>Gets or sets the page's button condition's rules.</summary>
        public IEnumerable<FieldConditionRule> ButtonConditionRules { get; set; } = [];

        /// <summary>Gets or sets the page's button condition.</summary>
        public FieldCondition? ButtonCondition { get; set; }

        /// <summary>Gets or sets the page's JavaScript files.</summary>
        public Dictionary<string, string> JavascriptFiles { get; set; } = [];

        /// <summary>Gets or sets the page's CSS files.</summary>
        public Dictionary<string, string> CssFiles { get; set; } = [];

        /// <summary>Gets or sets the page's JavaScript commands.</summary>
        public List<string> JavascriptCommands { get; set; } = [];

        /// <summary>Gets or sets the page's partial view files.</summary>
        public Dictionary<string, string> PartialViewFiles { get; set; } = [];

        /// <summary>
        /// Registers the JavaSscript assets for a field on the page.
        /// </summary>
        public void RegisterFieldJavascriptAssets(
          Field field,
          FieldType type,
          IHostingEnvironment hostEnvironment)
        {
            RegisterFieldJavascriptAssets(() => string.Empty, field, type, hostEnvironment);
        }

        /// <summary>
        /// Registers the themed JavaSscript assets for a field on the page.
        /// </summary>
        public void RegisterFieldJavascriptAssets(
          Func<string?> themeAccessor,
          Field field,
          FieldType type,
          IHostingEnvironment hostingEnvironment)
        {
            string[] array1 = [.. type.RequiredJavascriptFiles(field)];
            string[] array2 = [.. type.RequiredCssFiles(field)];
            string[] array3 = [.. type.RequiredPartialViews(themeAccessor, field).Union(type.RequiredPartialViews(field))];
            if (array3.Length != 0)
            {
                foreach (string str in array3)
                {
                    string key = str.ToLower().Replace("/", string.Empty).Replace(".", string.Empty);
                    PartialViewFiles.TryAdd(key, str);
                }
            }
            if (array1.Length != 0)
            {
                foreach (string str in array1)
                {
                    string key = str.ToLower().Replace("/", string.Empty).Replace(".", string.Empty);
                    JavascriptFiles.TryAdd(key, str);
                }
            }
            if (array2.Length != 0)
            {
                foreach (string virtualPath in array2)
                {
                    string key = virtualPath.ToLower().Replace("/", string.Empty).Replace(".", string.Empty);
                    if (!CssFiles.ContainsKey(key))
                        CssFiles.Add(key, hostingEnvironment.ToAbsolute(virtualPath));
                }
            }
            string str1 = type.RequiredJavascriptInitialization(field);
            if (string.IsNullOrEmpty(str1) || JavascriptCommands.Contains(str1))
                return;
            JavascriptCommands.Add(str1);
        }
    }
}