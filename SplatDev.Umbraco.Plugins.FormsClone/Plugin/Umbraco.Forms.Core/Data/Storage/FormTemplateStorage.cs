
// Type: Umbraco.Forms.Data.Storage.FormTemplateStorage
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading;
using Umbraco.Extensions;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Data.Storage;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Models;


#nullable enable
namespace Umbraco.Forms.Data.Storage
{
  internal sealed class FormTemplateStorage : IFormTemplateStorage
  {
    private readonly string _folder = "~/App_Plugins/UmbracoForms/Data/".TrimStart('~') + "Templates";
    private const string FileExtension = ".json";
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly FormDesignSettings _formDesignSettings;
    private static readonly object _fileLock = new object();

    public FormTemplateStorage(
      IWebHostEnvironment webHostEnvironment,
      IOptions<FormDesignSettings> formDesignSettings)
    {
      this._webHostEnvironment = webHostEnvironment;
      this._formDesignSettings = formDesignSettings.Value;
    }

    public IEnumerable<FormTemplateBase> GetAllTemplates()
    {
      IDictionary<string, Form> allAsDictionary = this.GetAllAsDictionary();
      List<FormTemplateBase> allTemplates = new List<FormTemplateBase>();
      foreach (KeyValuePair<string, Form> keyValuePair in (IEnumerable<KeyValuePair<string, Form>>) allAsDictionary)
      {
        FormTemplateBase formTemplateBase1 = new FormTemplateBase()
        {
          Name = keyValuePair.Value.Name,
          Alias = Path.GetFileNameWithoutExtension(keyValuePair.Key)
        };
        List<Field> allFields = keyValuePair.Value.AllFields;
        if (allFields.Count == 0)
          formTemplateBase1.Description = "0 fields";
        else if (allFields.Count < 5)
        {
          formTemplateBase1.Description = string.Join(", ", allFields.Select<Field, string>((Func<Field, string>) (x => x.Caption)).ToArray<string>());
        }
        else
        {
          formTemplateBase1.Description = string.Join(", ", allFields.Take<Field>(3).Select<Field, string>((Func<Field, string>) (x => x.Caption)).ToArray<string>());
          FormTemplateBase formTemplateBase2 = formTemplateBase1;
          formTemplateBase2.Description = formTemplateBase2.Description + " and " + (allFields.Count - 3).ToString() + " other fields";
        }
        allTemplates.Add(formTemplateBase1);
      }
      return (IEnumerable<FormTemplateBase>) allTemplates;
    }

    public Form? GetTemplate(string alias)
    {
      Form template;
      if (!this.TryGetFromFile(alias, out template, out string _))
        return (Form) null;
      this.PrepareTemplate(template);
      return template;
    }

    private void PrepareTemplate(Form template) => template.Id = Guid.Empty;

    public (Form? Form, IDictionary<FormState, IEnumerable<Workflow>> Workflows) GetTemplateWithWorkflows(
      string alias)
    {
      Form template;
      string templateAsJson;
      if (!this.TryGetFromFile(alias, out template, out templateAsJson))
        return ((Form) null, (IDictionary<FormState, IEnumerable<Workflow>>) new Dictionary<FormState, IEnumerable<Workflow>>());
      this.PrepareTemplate(template);
      return (template, FormDeserializationHelper.DeserializeWorkflows(templateAsJson));
    }

    private IDictionary<string, Form> GetAllAsDictionary()
    {
      Dictionary<string, Form> allAsDictionary = new Dictionary<string, Form>();
      foreach (IFileInfo fileInfo in this._webHostEnvironment.WebRootFileProvider.GetDirectoryContents(this._folder).Where<IFileInfo>((Func<IFileInfo, bool>) (x => !x.IsDirectory && x.Name.EndsWith(".json"))).ToList<IFileInfo>())
      {
        if (!this._formDesignSettings.RemoveProvidedFormTemplates || !((IEnumerable<string>) Constants.FormTemplates.CoreFormTemplates).Contains<string>(fileInfo.Name))
        {
          object fileLock = FormTemplateStorage._fileLock;
          bool lockTaken = false;
          try
          {
            Monitor.Enter(fileLock, ref lockTaken);
            using (StreamReader streamReader = new StreamReader(fileInfo.CreateReadStream()))
            {
              Form form = FormDeserializationHelper.DeserializeForm(streamReader.ReadToEnd());
              if (form != null)
                allAsDictionary.Add(fileInfo.Name, form);
            }
          }
          finally
          {
            if (lockTaken)
              Monitor.Exit(fileLock);
          }
        }
      }
      return (IDictionary<string, Form>) allAsDictionary;
    }

    private bool TryGetFromFile(string alias, [NotNullWhen(true)] out Form? template, [NotNullWhen(true)] out string? templateAsJson)
    {
      IFileInfo fileInfo = this.Find(alias);
      if (fileInfo != null)
      {
        object fileLock = FormTemplateStorage._fileLock;
        bool lockTaken = false;
        try
        {
          Monitor.Enter(fileLock, ref lockTaken);
          using (StreamReader streamReader = new StreamReader(fileInfo.CreateReadStream()))
          {
            templateAsJson = streamReader.ReadToEnd();
            template = FormDeserializationHelper.DeserializeForm(templateAsJson);
            return template != null;
          }
        }
        finally
        {
          if (lockTaken)
            Monitor.Exit(fileLock);
        }
      }
      else
      {
        templateAsJson = (string) null;
        template = (Form) null;
        return false;
      }
    }

    private IFileInfo? Find(string alias) => this._webHostEnvironment.WebRootFileProvider.GetDirectoryContents(this._folder).SingleOrDefault<IFileInfo>((Func<IFileInfo, bool>) (x => !x.IsDirectory && x.Name.InvariantEquals(alias + ".json"))) ?? throw new FileNotFoundException(string.Format("Could not find file '{0}{1}' in the folder {2}", (object) alias, (object) ".json", (object) this._folder));
  }
}
