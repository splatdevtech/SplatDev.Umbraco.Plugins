
// Type: Umbraco.Forms.Core.Interfaces.IFieldType
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using Umbraco.Forms.Core.Enums;


#nullable enable
namespace Umbraco.Forms.Core.Interfaces
{
  public interface IFieldType
  {
    string Name { get; set; }

    string Alias { get; set; }

    Guid Id { get; set; }

    string Icon { get; set; }

    string RenderView { get; set; }

    string EditView { get; set; }

    string Category { get; set; }

    int SortOrder { get; set; }

    FieldDataType DataType { get; set; }

    bool SupportsPreValues { get; set; }

    bool SupportsMandatory { get; set; }

    bool MandatoryByDefault { get; set; }

    bool SupportsRegex { get; set; }

    RenderInputType RenderInputType { get; set; }

    bool StoresData { get; }

    bool HideLabel { get; set; }

    Dictionary<FieldConditionRuleOperator, Func<string, string, bool>> ConditionCheckFunctions { get; }

    List<Exception> ValidateSettings();
  }
}
