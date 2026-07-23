
// Type: Umbraco.Forms.Examine.Indexes.ExamineUmbracoFormsIndexingHandler
// Assembly: Umbraco.Forms.Examine, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: EDF5A33E-94A1-42C9-B681-695454D27A51

using Examine;

using Microsoft.Extensions.Logging;

using System.Data;
using System.Globalization;

using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Runtime;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Infrastructure;
using Umbraco.Cms.Infrastructure.Examine;
using Umbraco.Cms.Infrastructure.HostedServices;
using Umbraco.Forms.Core.Persistence.Dtos;

using IScopeProvider = Umbraco.Cms.Infrastructure.Scoping.IScopeProvider;

#nullable enable
namespace Umbraco.Forms.Examine.Indexes
{
    internal sealed class ExamineUmbracoFormsIndexingHandler : IUmbracoFormsIndexingHandler
    {
        private const int EnlistPriority = 80;
        private readonly IMainDom _mainDom;
        private readonly ILogger<ExamineUmbracoFormsIndexingHandler> _logger;
        private readonly IProfilingLogger _profilingLogger;
        private readonly IScopeProvider _scopeProvider;
        private readonly IExamineManager _examineManager;
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;
        private readonly IPublishedContentValueSetBuilder _publishedContentValueSetBuilder;
        private readonly IValueSetBuilder<Record> _recordValueSetBuilder;
        private readonly Lazy<bool> _enabled;

        public ExamineUmbracoFormsIndexingHandler(
          IMainDom mainDom,
          ILogger<ExamineUmbracoFormsIndexingHandler> logger,
          IProfilingLogger profilingLogger,
          IScopeProvider scopeProvider,
          IExamineManager examineManager,
          IBackgroundTaskQueue backgroundTaskQueue,
          IPublishedContentValueSetBuilder publishedContentValueSetBuilder,
          IValueSetBuilder<Record> recordValueSetBuilder)
        {
            this._mainDom = mainDom;
            this._logger = logger;
            this._profilingLogger = profilingLogger;
            this._scopeProvider = scopeProvider;
            this._examineManager = examineManager;
            this._backgroundTaskQueue = backgroundTaskQueue;
            this._recordValueSetBuilder = recordValueSetBuilder;
            this._publishedContentValueSetBuilder = publishedContentValueSetBuilder;
            this._enabled = new Lazy<bool>(new Func<bool>(this.IsEnabled));
        }

        private bool IsEnabled()
        {
            if (!this._mainDom.Register(release: () =>
            {
                using (this._profilingLogger.TraceDuration<ExamineUmbracoFormsIndexingHandler>("Examine shutting down"))
                    this._examineManager.Dispose();
            }))
            {
                this._logger.LogInformation("Examine shutdown not registered, this AppDomain is not the MainDom, Examine will be disabled");
                Suspendable.ExamineEvents.SuspendIndexers(_logger);
                return false;
            }
            this._logger.LogDebug("Examine shutdown registered with MainDom");
            int num = this._examineManager.Indexes.OfType<IUmbracoIndex>().Count<IUmbracoIndex>(x => x.EnableDefaultEventHandler);
            this._logger.LogInformation("Adding examine event handlers for {RegisteredIndexers} index providers.", num);
            return num != 0;
        }

        public bool Enabled => this._enabled.Value;

        public void DeleteRecord(Record record)
        {
            ExamineUmbracoFormsIndexingHandler.DeferedActions deferedActions = ExamineUmbracoFormsIndexingHandler.DeferedActions.Get(this._scopeProvider);
            if (deferedActions != null)
                deferedActions.Add(new ExamineUmbracoFormsIndexingHandler.DeferedDeleteIndex(this, record));
            else
                ExamineUmbracoFormsIndexingHandler.DeferedDeleteIndex.Execute(this, record);
        }

        public void ReIndexForRecord(Record record)
        {
            ExamineUmbracoFormsIndexingHandler.DeferedActions deferedActions = ExamineUmbracoFormsIndexingHandler.DeferedActions.Get(this._scopeProvider);
            if (deferedActions != null)
                deferedActions.Add(new ExamineUmbracoFormsIndexingHandler.DeferedReIndexForRecord(this._backgroundTaskQueue, this, record));
            else
                ExamineUmbracoFormsIndexingHandler.DeferedReIndexForRecord.Execute(this._backgroundTaskQueue, this, record);
        }

        private sealed class DeferedActions
        {
            private readonly List<ExamineUmbracoFormsIndexingHandler.DeferedAction> _actions = new List<ExamineUmbracoFormsIndexingHandler.DeferedAction>();

            public static ExamineUmbracoFormsIndexingHandler.DeferedActions? Get(
              IScopeProvider scopeProvider)
            {
                return scopeProvider.Context?.Enlist<ExamineUmbracoFormsIndexingHandler.DeferedActions>("examineFormsEvents", () => new ExamineUmbracoFormsIndexingHandler.DeferedActions(), (completed, actions) =>
                {
                    if (!completed || actions == null)
                        return;
                    actions.Execute();
                }, 80);
            }

            public void Add(
              ExamineUmbracoFormsIndexingHandler.DeferedAction action)
            {
                this._actions.Add(action);
            }

            private void Execute()
            {
                foreach (ExamineUmbracoFormsIndexingHandler.DeferedAction action in this._actions)
                    action.Execute();
            }
        }

        private abstract class DeferedAction
        {
            public virtual void Execute()
            {
            }
        }

        private sealed class DeferedReIndexForRecord : ExamineUmbracoFormsIndexingHandler.DeferedAction
        {
            private readonly ExamineUmbracoFormsIndexingHandler _examineUmbracoFormsIndexingHandler;
            private readonly Record _record;
            private readonly IBackgroundTaskQueue _backgroundTaskQueue;

            public DeferedReIndexForRecord(
              IBackgroundTaskQueue backgroundTaskQueue,
              ExamineUmbracoFormsIndexingHandler examineUmbracoFormsIndexingHandler,
              Record record)
            {
                this._examineUmbracoFormsIndexingHandler = examineUmbracoFormsIndexingHandler;
                this._record = record;
                this._backgroundTaskQueue = backgroundTaskQueue;
            }

            public override void Execute() => ExamineUmbracoFormsIndexingHandler.DeferedReIndexForRecord.Execute(this._backgroundTaskQueue, this._examineUmbracoFormsIndexingHandler, this._record);

            public static void Execute(
              IBackgroundTaskQueue backgroundTaskQueue,
              ExamineUmbracoFormsIndexingHandler examineUmbracoFormsIndexingHandler,
              Record record)
            {
                backgroundTaskQueue.QueueBackgroundWorkItem(cancellationToken =>
                {
                    using (examineUmbracoFormsIndexingHandler._scopeProvider.CreateScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, null, false, true))
                    {
                        List<ValueSet> list = examineUmbracoFormsIndexingHandler._recordValueSetBuilder.GetValueSets(new Record[1]
                        {
              record
                        }).ToList<ValueSet>();
                        foreach (IIndex index in examineUmbracoFormsIndexingHandler._examineManager.Indexes.OfType<IUmbracoFormsRecordIndex>())
                            index.IndexItems((IEnumerable<ValueSet>)list);
                        return Task.CompletedTask;
                    }
                });
            }
        }

        private sealed class DeferedDeleteIndex : ExamineUmbracoFormsIndexingHandler.DeferedAction
        {
            private readonly ExamineUmbracoFormsIndexingHandler _examineUmbracoFormsIndexingHandler;
            private readonly Record _record;

            public DeferedDeleteIndex(
              ExamineUmbracoFormsIndexingHandler examineUmbracoFormsIndexingHandler,
              Record record)
            {
                this._examineUmbracoFormsIndexingHandler = examineUmbracoFormsIndexingHandler;
                this._record = record;
            }

            public override void Execute() => ExamineUmbracoFormsIndexingHandler.DeferedDeleteIndex.Execute(this._examineUmbracoFormsIndexingHandler, this._record);

            public static void Execute(
              ExamineUmbracoFormsIndexingHandler examineUmbracoFormsIndexingHandler,
              Record record)
            {
                string itemId = record.Id.ToString(CultureInfo.InvariantCulture);
                foreach (IIndex index in examineUmbracoFormsIndexingHandler._examineManager.Indexes.OfType<IUmbracoFormsRecordIndex>())
                    index.DeleteFromIndex(itemId);
            }
        }
    }
}
