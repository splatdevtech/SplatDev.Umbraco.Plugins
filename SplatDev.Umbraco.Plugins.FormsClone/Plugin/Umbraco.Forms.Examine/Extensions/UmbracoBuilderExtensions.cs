
// Type: Umbraco.Forms.Examine.Extensions.UmbracoBuilderExtensions
// Assembly: Umbraco.Forms.Examine, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: EDF5A33E-94A1-42C9-B681-695454D27A51

using Examine;
using Examine.Lucene;
using Lucene.Net.Analysis;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Infrastructure.Examine;
using Umbraco.Extensions;
using Umbraco.Forms.Core.Persistence.Dtos;
using Umbraco.Forms.Core.Searchers;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Core.Services.Notifications;
using Umbraco.Forms.Examine.Indexes;
using Umbraco.Forms.Examine.NotificationHandlers;
using Umbraco.Forms.Examine.Services;


#nullable enable
namespace Umbraco.Forms.Examine.Extensions
{
  public static class UmbracoBuilderExtensions
  {
    public static IUmbracoBuilder AddUmbracoFormsExamine(this IUmbracoBuilder builder)
    {
      ServicesCollectionExtensions.AddExamineLuceneIndex<UmbracoFormsRecordRecordIndex, ConfigurationEnabledDirectoryFactory>(builder.Services, "UmbracoFormsRecordsIndex", (FieldDefinitionCollection) null, (Analyzer) null, (IValueSetValidator) null, (IReadOnlyDictionary<string, IFieldValueTypeFactory>) null).ConfigureOptions<ConfigureUmbracoFormsIndexOptions>();
      builder.Services.AddSingleton<IIndexPopulator, FormsIndexPopulator>();
      builder.Services.AddSingleton<IUmbracoFormsIndexingHandler, ExamineUmbracoFormsIndexingHandler>();
      builder.Services.AddUnique<IValueSetBuilder<Record>, RecordValueSetBuilder>();
      builder.Services.AddUnique<IFormRecordSearcher, FormRecordSearcher>();
      builder.Services.AddUnique<IRecordReaderService, RecordReaderService>();
      builder.Services.AddSingleton<ExamineIndexRebuilder>();
      builder.AddNotificationHandler<RecordDeletingNotification, RecordStorageNotificationHandler>();
      builder.AddNotificationHandler<RecordSavingNotification, RecordStorageNotificationHandler>();
      return builder;
    }
  }
}
