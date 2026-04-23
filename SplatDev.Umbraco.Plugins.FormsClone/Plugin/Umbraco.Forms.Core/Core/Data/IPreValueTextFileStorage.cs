
// Type: Umbraco.Forms.Core.Data.IPreValueTextFileStorage
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using System.IO;
using Umbraco.Forms.Core.Models;


#nullable enable
namespace Umbraco.Forms.Core.Data
{
  public interface IPreValueTextFileStorage
  {
    void DeleteTextFile(string filePath);

    List<PreValue> GetTextFilePreValues(string filePath);

    void SaveValuesIntoFile(List<string> values, string filePath);

    string GenerateFilePath(string filename, Guid preValueId);

    void SaveTextFile(Stream fileContentStream, string filename, Guid preValueId);
  }
}
