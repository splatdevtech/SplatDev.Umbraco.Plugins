using FormBuilder.Core.Configuration;
using FormBuilder.Core.Enums;
using FormBuilder.Core.Helpers;
using FormBuilder.Core.Models;
using FormBuilder.Core.Storage.Interfaces;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

using System.Diagnostics.CodeAnalysis;

using Umbraco.Extensions;

namespace FormBuilder.Core.Storage
{
    internal sealed class FormTemplateStorage(
      IWebHostEnvironment webHostEnvironment,
      IOptions<FormDesignSettings> formDesignSettings) : IFormTemplateStorage
    {
        private readonly string _folder = "~/App_Plugins/FormBuilder/Data/".TrimStart('~') + "Templates";
        private const string FileExtension = ".json";
        private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;
        private readonly FormDesignSettings _formDesignSettings = formDesignSettings.Value;
        private static readonly object _fileLock = new();

        public IEnumerable<FormTemplateBase> GetAllTemplates()
        {
            IDictionary<string, Form> allAsDictionary = GetAllAsDictionary();
            List<FormTemplateBase> allTemplates = [];
            foreach (KeyValuePair<string, Form> keyValuePair in allAsDictionary)
            {
                FormTemplateBase formTemplateBase1 = new()
                {
                    Name = keyValuePair.Value.Name,
                    Alias = Path.GetFileNameWithoutExtension(keyValuePair.Key)
                };
                List<Field> allFields = keyValuePair.Value.AllFields;
                if (allFields.Count == 0)
                    formTemplateBase1.Description = "0 fields";
                else if (allFields.Count < 5)
                {
                    formTemplateBase1.Description = string.Join(", ", allFields.Select(x => x.Caption).ToArray());
                }
                else
                {
                    formTemplateBase1.Description = string.Join(", ", allFields.Take(3).Select(x => x.Caption).ToArray());
                    FormTemplateBase formTemplateBase2 = formTemplateBase1;
                    formTemplateBase2.Description = formTemplateBase2.Description + " and " + (allFields.Count - 3).ToString() + " other fields";
                }
                allTemplates.Add(formTemplateBase1);
            }
            return allTemplates;
        }

        public Form? GetTemplate(string alias)
        {
            if (!TryGetFromFile(alias, out Form? template, out var _))
                return null;
            PrepareTemplate(template);
            return template;
        }

        private static void PrepareTemplate(Form template) => template.Id = Guid.Empty;

        public (Form? Form, IDictionary<FormState, IEnumerable<Workflow>> Workflows) GetTemplateWithWorkflows(
          string alias)
        {
            if (!TryGetFromFile(alias, out Form? template, out string? templateAsJson))
                return (null, new Dictionary<FormState, IEnumerable<Workflow>>());
            PrepareTemplate(template);

            var json = FormDeserializationHelper.DeserializeWorkflows(templateAsJson);
            return (template, json);
        }

        private Dictionary<string, Form> GetAllAsDictionary()
        {
            Dictionary<string, Form> allAsDictionary = [];
            foreach (IFileInfo fileInfo in _webHostEnvironment.WebRootFileProvider.GetDirectoryContents(_folder).Where(x => !x.IsDirectory && x.Name.EndsWith(".json")).ToList())
            {
                if (!_formDesignSettings.RemoveProvidedFormTemplates || !Constants.FormTemplates.CoreFormTemplates.Contains(fileInfo.Name))
                {
                    object fileLock = _fileLock;
                    bool lockTaken = false;
                    try
                    {
                        Monitor.Enter(fileLock, ref lockTaken);
                        using StreamReader streamReader = new(fileInfo.CreateReadStream());
                        Form? form = FormDeserializationHelper.DeserializeForm(streamReader.ReadToEnd());
                        if (form is not null)
                            allAsDictionary.Add(fileInfo.Name, form);
                    }
                    finally
                    {
                        if (lockTaken)
                            Monitor.Exit(fileLock);
                    }
                }
            }
            return allAsDictionary;
        }

        private bool TryGetFromFile(string alias, [NotNullWhen(true)] out Form? template, [NotNullWhen(true)] out string? templateAsJson)
        {
            IFileInfo? fileInfo = Find(alias);
            if (fileInfo is not null)
            {
                object fileLock = _fileLock;
                bool lockTaken = false;
                try
                {
                    Monitor.Enter(fileLock, ref lockTaken);
                    using StreamReader streamReader = new(fileInfo.CreateReadStream());
                    templateAsJson = streamReader.ReadToEnd();
                    template = FormDeserializationHelper.DeserializeForm(templateAsJson);
                    return template is not null;
                }
                finally
                {
                    if (lockTaken)
                        Monitor.Exit(fileLock);
                }
            }
            else
            {
                templateAsJson = null;
                template = null;
                return false;
            }
        }

        private IFileInfo? Find(string alias) => _webHostEnvironment.WebRootFileProvider.GetDirectoryContents(_folder).SingleOrDefault(x => !x.IsDirectory && x.Name.InvariantEquals(alias + ".json")) ?? throw new FileNotFoundException(string.Format("Could not find file '{0}{1}' in the folder {2}", alias, ".json", _folder));
    }
}