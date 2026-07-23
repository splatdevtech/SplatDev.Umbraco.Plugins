
// Type: Umbraco.Forms.Core.Extensions.RecordFieldExtensions
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Extensions;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;


#nullable enable
namespace Umbraco.Forms.Core.Extensions
{
  public static class RecordFieldExtensions
  {
    public static int FormFieldOrder(this RecordField recordField, IEnumerable<Field> formFields)
    {
      int index = formFields.FindIndex<Field>((Func<Field, bool>) (x => x.Id == recordField.FieldId));
      return index != -1 ? index : int.MaxValue;
    }

    public static IEnumerable<string> GetSelectedPrevalues(this RecordField recordField)
    {
      if (recordField.Field == null)
        return Enumerable.Empty<string>();
      IEnumerable<FieldPrevalue> preValues = recordField.Field.PreValues;
      string fieldValue = recordField.ValuesAsString();
      return RecordFieldExtensions.GetSelectedPrevalues(preValues.Select<FieldPrevalue, string>((Func<FieldPrevalue, string>) (x => x.Value)), fieldValue);
    }

    private static IEnumerable<string> GetSelectedPrevalues(
      IEnumerable<string> fieldPreValues,
      string fieldValue)
    {
      if (string.IsNullOrEmpty(fieldValue))
        return Enumerable.Empty<string>();
      IOrderedEnumerable<string> orderedEnumerable = fieldPreValues.OrderByDescending<string, int>((Func<string, int>) (x => x.Length));
      string str1 = fieldValue + ", ";
      foreach (string str2 in (IEnumerable<string>) orderedEnumerable)
        str1 = str1.Replace(str2 + ", ", str2.Replace(",", "|,") + "|||");
      string[] selectedPrevalues = str1.Split(new string[1]
      {
        "|||"
      }, StringSplitOptions.RemoveEmptyEntries);
      for (int index = 0; index < selectedPrevalues.Length; ++index)
        selectedPrevalues[index] = selectedPrevalues[index].Replace("|,", ",");
      return (IEnumerable<string>) selectedPrevalues;
    }
  }
}
