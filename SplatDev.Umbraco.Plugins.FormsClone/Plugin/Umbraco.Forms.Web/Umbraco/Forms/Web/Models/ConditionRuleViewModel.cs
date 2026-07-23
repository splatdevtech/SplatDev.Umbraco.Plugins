
// Type: Umbraco.Forms.Web.Models.ConditionRuleViewModel
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Web.Models
{
  [Serializable]
  public class ConditionRuleViewModel
  {
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("fieldsetId")]
    public Guid FieldsetId { get; set; }

    [JsonPropertyName("field")]
    public Guid Field { get; set; }

    [JsonPropertyName("operator")]
    public FieldConditionRuleOperator Operator { get; set; }

    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;

    public static ConditionRuleViewModel Build(
      Form form,
      FieldConditionRule rule,
      IPlaceholderParsingService placeholderParsingService)
    {
      return new ConditionRuleViewModel()
      {
        Id = rule.Id,
        Field = rule.Field,
        FieldsetId = ConditionRuleViewModel.FindFieldsetId(form, rule),
        Operator = rule.Operator,
        Value = placeholderParsingService.ParsePlaceHolders(rule.Value, false, form: form)
      };
    }

    private static Guid FindFieldsetId(Form form, FieldConditionRule rule) => form.Pages.SelectMany<Page, FieldSet>((Func<Page, IEnumerable<FieldSet>>) (p => (IEnumerable<FieldSet>) p.FieldSets)).Where<FieldSet>((Func<FieldSet, bool>) (fs => fs.Containers.Any<FieldsetContainer>((Func<FieldsetContainer, bool>) (c => c.Fields.Any<Umbraco.Forms.Core.Models.Field>((Func<Umbraco.Forms.Core.Models.Field, bool>) (f => f.Id == rule.Field)))))).Select<FieldSet, Guid>((Func<FieldSet, Guid>) (fs => fs.Id)).FirstOrDefault<Guid>();
  }
}
