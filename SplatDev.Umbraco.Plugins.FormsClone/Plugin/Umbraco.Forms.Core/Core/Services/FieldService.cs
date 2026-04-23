
// Type: Umbraco.Forms.Core.Services.FieldService
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Forms.Core.Models;


#nullable enable
namespace Umbraco.Forms.Core.Services
{
  internal sealed class FieldService : IFieldService
  {
    public IEnumerable<string> GetDuplicates(List<Field> fields) => fields.GroupBy<Field, string>((Func<Field, string>) (x => x.Alias)).Where<IGrouping<string, Field>>((Func<IGrouping<string, Field>, bool>) (x => x.Count<Field>() > 1)).Select<IGrouping<string, Field>, string>((Func<IGrouping<string, Field>, string>) (k => k.Key));

    public bool ContainsSensitiveData(List<Field> fields) => fields.Any<Field>((Func<Field, bool>) (field => field.ContainsSensitiveData));
  }
}
