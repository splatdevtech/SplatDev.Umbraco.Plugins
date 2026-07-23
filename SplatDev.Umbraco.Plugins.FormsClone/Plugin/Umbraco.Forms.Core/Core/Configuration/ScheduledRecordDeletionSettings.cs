
// Type: Umbraco.Forms.Core.Configuration.ScheduledRecordDeletionSettings
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.ComponentModel;


#nullable enable
namespace Umbraco.Forms.Core.Configuration
{
  public class ScheduledRecordDeletionSettings
  {
    internal const bool StaticEnabled = false;
    internal const string StaticPeriod = "1.00:00:00";

    [DefaultValue(false)]
    public bool Enabled { get; set; }

    public string FirstRunTime { get; set; } = string.Empty;

    [DefaultValue("1.00:00:00")]
    public TimeSpan Period { get; set; } = TimeSpan.Parse("1.00:00:00");
  }
}
