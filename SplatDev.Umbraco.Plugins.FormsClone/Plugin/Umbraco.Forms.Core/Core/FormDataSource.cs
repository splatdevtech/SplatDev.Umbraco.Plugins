
// Type: Umbraco.Forms.Core.FormDataSource
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Umbraco.Forms.Core.Interfaces;
using Umbraco.Forms.Core.Providers;


#nullable enable
namespace Umbraco.Forms.Core
{
  [DataContract(Name = "formDataSource")]
  public class FormDataSource : ITypeWithEditorDetails, IType
  {
    private FormDataSourceType? _formDataSourceType;

    [DataMember(Name = "id")]
    public Guid Id { get; set; }

    [DataMember(Name = "unique")]
    public Guid Unique => this.Id;

    [DataMember(Name = "entityType")]
    public string EntityType => "datasource";

    [DataMember(Name = "name")]
    public string Name { get; set; } = string.Empty;

    [DataMember(Name = "created")]
    public DateTime Created { get; set; }

    [DataMember(Name = "createdBy")]
    public int? CreatedBy { get; set; }

    [DataMember(Name = "createdByName")]
    public string? CreatedByName { get; set; }

    [DataMember(Name = "updated")]
    public DateTime Updated { get; set; }

    [DataMember(Name = "updatedBy")]
    public int? UpdatedBy { get; set; }

    [DataMember(Name = "updatedByName")]
    public string? UpdatedByName { get; set; }

    [DataMember(Name = "settings")]
    public Dictionary<string, string> Settings { get; set; } = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    [DataMember(Name = "formDataSourceTypeId")]
    public Guid FormDataSourceTypeId { get; set; }

    [DataMember(Name = "valid")]
    public bool Valid { get; set; }

    public FormDataSourceType? GetFormDataSourceType(
      DataSourceTypeCollection dataSourceTypeCollection)
    {
      if (this._formDataSourceType != null || this.FormDataSourceTypeId == Guid.Empty)
        return this._formDataSourceType;
      this._formDataSourceType = dataSourceTypeCollection[this.FormDataSourceTypeId];
      return this._formDataSourceType;
    }

    public void SetFormDataSourceType(FormDataSourceType value)
    {
      this.FormDataSourceTypeId = value.Id;
      this._formDataSourceType = value;
    }
  }
}
