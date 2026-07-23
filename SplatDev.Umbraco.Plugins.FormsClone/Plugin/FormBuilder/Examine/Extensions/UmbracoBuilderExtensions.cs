using Examine;
using Examine.Lucene;

using FormBuilder.Core.Persistence.Fields;
using FormBuilder.Core.Searches;
using FormBuilder.Core.Searches.Interfaces;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Core.Services.Notifications;
using FormBuilder.Examine.Handlers;
using FormBuilder.Examine.IndexPopulators;
using FormBuilder.Examine.Interfaces;
using FormBuilder.Examine.Services;
using FormBuilder.Examine.ValueSetBuilders;

using Microsoft.Extensions.DependencyInjection;

using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Infrastructure.Examine;
using Umbraco.Extensions;

namespace FormBuilder.Examine.Extensions
{
    public static class UmbracoBuilderExtensions
    {
        public static IUmbracoBuilder AddFormBuilderExamine(this IUmbracoBuilder builder)
        {
            builder.Services.AddExamineLuceneIndex<FormBuilderRecordRecordIndex, ConfigurationEnabledDirectoryFactory>("FormBuilderRecordsIndex", null, null, null, (IReadOnlyDictionary<string, IFieldValueTypeFactory>?)null).ConfigureOptions<ConfigureFormBuilderIndexOptions>();
            builder.Services.AddSingleton<IIndexPopulator, FormsIndexPopulator>();
            builder.Services.AddSingleton<IFormBuilderIndexingHandler, ExamineFormBuilderIndexingHandler>();
            builder.Services.AddUnique<IValueSetBuilder<Record>, RecordValueSetBuilder>();
            builder.Services.AddUnique<IFormRecordSearcher, FormRecordSearcher>();
            builder.Services.AddUnique<IRecordReaderService, RecordReaderService>();
#pragma warning disable CS0618 // Type or member is obsolete
            builder.Services.AddSingleton<ExamineIndexRebuilder>();
#pragma warning restore CS0618 // Type or member is obsolete
            builder.AddNotificationHandler<RecordDeletingNotification, RecordStorageNotificationHandler>();
            builder.AddNotificationHandler<RecordSavingNotification, RecordStorageNotificationHandler>();
            return builder;
        }
    }
}