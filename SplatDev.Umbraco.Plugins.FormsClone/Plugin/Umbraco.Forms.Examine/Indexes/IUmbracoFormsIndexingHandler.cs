
// Type: Umbraco.Forms.Examine.Indexes.IUmbracoFormsIndexingHandler
// Assembly: Umbraco.Forms.Examine, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: EDF5A33E-94A1-42C9-B681-695454D27A51

using Umbraco.Forms.Core.Persistence.Dtos;


#nullable enable
namespace Umbraco.Forms.Examine.Indexes
{
  public interface IUmbracoFormsIndexingHandler
  {
    bool Enabled { get; }

    void ReIndexForRecord(Record record);

    void DeleteRecord(Record record);
  }
}
