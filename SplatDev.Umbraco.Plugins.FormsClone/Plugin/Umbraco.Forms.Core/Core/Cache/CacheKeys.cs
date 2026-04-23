
// Type: Umbraco.Forms.Core.Cache.CacheKeys
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Runtime.CompilerServices;


#nullable enable
namespace Umbraco.Forms.Core.Cache
{
  public static class CacheKeys
  {
    public const string FoldersDbCacheRefresherId = "74318b85-f97d-49af-ba15-caf9e0ba4d5a";
    public static readonly Guid FoldersDbCacheRefresherGuid = new Guid("74318b85-f97d-49af-ba15-caf9e0ba4d5a");
    public const string FormsDbCacheRefresherId = "8ad0c841-02c9-4460-8627-562beba6a36a";
    public static readonly Guid FormsDbCacheRefresherGuid = new Guid("8ad0c841-02c9-4460-8627-562beba6a36a");
    public const string WorkDbflowCacheRefresherId = "bd86d2b0-d738-4dbc-be69-87a74b67760c";
    public static readonly Guid WorkflowDbCacheRefresherGuid = new Guid("bd86d2b0-d738-4dbc-be69-87a74b67760c");
    public const string PreValueDbCacheRefresherId = "628a5766-5823-49b1-9269-a1b1df7c798c";
    public static readonly Guid PreValueDbCacheRefresherGuid = new Guid("628a5766-5823-49b1-9269-a1b1df7c798c");
    public const string DataSourceDbCacheRefresherId = "174f7b86-0b49-43e6-8cee-fa28fdeb2fec";
    public static readonly Guid DataSourceDbCacheRefresherGuid = new Guid("174f7b86-0b49-43e6-8cee-fa28fdeb2fec");
    public const string FormStorageAllFormsKey = "Forms.FormStorage.All";
    public const string FolderPrefix = "Forms.Folder.";
    public const string PreValuePrefix = "Forms.PreValues.";
    public const string PreValueSourcePrevaluesFormat = "Forms.PreValues.{0}.{1}.{2}.{3}";
    public const string DataSourcePrefix = "Forms.DataSource.";
    public const string WorkflowPrefix = "Forms.Workflow.";
    public const string FormsVersion = "Forms.Version";
    public const string FormsSettingsAll = "Forms.Setting.All";

    public static string GetMemberCacheKey(Guid memberKey)
    {
      DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(12, 1);
      interpolatedStringHandler.AppendLiteral("Forms.Member");
      interpolatedStringHandler.AppendFormatted<Guid>(memberKey);
      return interpolatedStringHandler.ToStringAndClear();
    }

    public static string GetMemberValuesCacheKey(Guid memberKey)
    {
      DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(19, 1);
      interpolatedStringHandler.AppendLiteral("Forms.Member");
      interpolatedStringHandler.AppendFormatted<Guid>(memberKey);
      interpolatedStringHandler.AppendLiteral(".Values");
      return interpolatedStringHandler.ToStringAndClear();
    }
  }
}
