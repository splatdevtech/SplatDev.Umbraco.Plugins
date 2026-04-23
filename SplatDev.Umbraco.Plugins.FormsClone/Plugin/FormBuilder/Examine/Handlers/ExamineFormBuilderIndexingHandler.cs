using Examine;

using FormBuilder.Core.Persistence.Fields;
using FormBuilder.Examine.Interfaces;

using Microsoft.Extensions.Logging;

using System.Data;
using System.Globalization;

using Umbraco.Cms.Core.Logging;
using Umbraco.Cms.Core.Runtime;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Infrastructure;
using Umbraco.Cms.Infrastructure.Examine;
using Umbraco.Cms.Infrastructure.HostedServices;

namespace FormBuilder.Examine.Handlers
{
    internal sealed class ExamineFormBuilderIndexingHandler : IFormBuilderIndexingHandler
    {
        private readonly IMainDom _mainDom;
        private readonly ILogger<ExamineFormBuilderIndexingHandler> _logger;
        private readonly IProfilingLogger _profilingLogger;
        private readonly ICoreScopeProvider _scopeProvider;
        private readonly IExamineManager _examineManager;
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;
        private readonly IValueSetBuilder<Record> _recordValueSetBuilder;
        private readonly Lazy<bool> _enabled;

        public ExamineFormBuilderIndexingHandler(
          IMainDom mainDom,
          ILogger<ExamineFormBuilderIndexingHandler> logger,
          IProfilingLogger profilingLogger,
          ICoreScopeProvider scopeProvider,
          IExamineManager examineManager,
          IBackgroundTaskQueue backgroundTaskQueue,
          IValueSetBuilder<Record> recordValueSetBuilder)
        {
            _mainDom = mainDom;
            _logger = logger;
            _profilingLogger = profilingLogger;
            _scopeProvider = scopeProvider;
            _examineManager = examineManager;
            _backgroundTaskQueue = backgroundTaskQueue;
            _recordValueSetBuilder = recordValueSetBuilder;
            _enabled = new Lazy<bool>(new Func<bool>(IsEnabled));
        }

        private bool IsEnabled()
        {
            if (!_mainDom.Register(release: () =>
            {
                using (_profilingLogger.TraceDuration<ExamineFormBuilderIndexingHandler>("Examine shutting down"))
                    _examineManager.Dispose();
            }))
            {
                _logger.LogInformation("Examine shutdown not registered, this AppDomain is not the MainDom, Examine will be disabled");
                Suspendable.ExamineEvents.SuspendIndexers(_logger);
                return false;
            }
            _logger.LogDebug("Examine shutdown registered with MainDom");
            int num = _examineManager.Indexes.OfType<IUmbracoIndex>().Count(x => x.EnableDefaultEventHandler);
            _logger.LogInformation("Adding examine event handlers for {RegisteredIndexers} index providers.", num);
            return num != 0;
        }

        public bool Enabled => _enabled.Value;

        public void DeleteRecord(Record record)
        {
            DeferedActions? deferedActions = DeferedActions.Get(_scopeProvider);
            if (deferedActions is not null)
                deferedActions.Add(new DeferedDeleteIndex(this, record));
            else
                DeferedDeleteIndex.Execute(this, record);
        }

        public void ReIndexForRecord(Record record)
        {
            DeferedActions? deferedActions = DeferedActions.Get(_scopeProvider);
            if (deferedActions is not null)
                deferedActions.Add(new DeferedReIndexForRecord(_backgroundTaskQueue, this, record));
            else
                DeferedReIndexForRecord.Execute(_backgroundTaskQueue, this, record);
        }

        private sealed class DeferedActions
        {
            private readonly List<DeferedAction> _actions = [];

            public static DeferedActions? Get(
              ICoreScopeProvider scopeProvider)
            {
                return scopeProvider.Context?.Enlist("examineFormsEvents", () => new DeferedActions(), (completed, actions) =>
                {
                    if (!completed || actions is null)
                        return;
                    actions.Execute();
                }, 80);
            }

            public void Add(
              DeferedAction action)
            {
                _actions.Add(action);
            }

            private void Execute()
            {
                foreach (DeferedAction action in _actions)
                    action.Execute();
            }
        }

        private abstract class DeferedAction
        {
            public virtual void Execute()
            {
            }
        }

        private sealed class DeferedReIndexForRecord(
          IBackgroundTaskQueue backgroundTaskQueue,
          ExamineFormBuilderIndexingHandler examineFormBuildersIndexingHandler,
          Record record) : DeferedAction
        {
            private readonly ExamineFormBuilderIndexingHandler _examineFormBuildersIndexingHandler = examineFormBuildersIndexingHandler;
            private readonly Record _record = record;
            private readonly IBackgroundTaskQueue _backgroundTaskQueue = backgroundTaskQueue;

            public override void Execute() => Execute(_backgroundTaskQueue, _examineFormBuildersIndexingHandler, _record);

            public static void Execute(
              IBackgroundTaskQueue backgroundTaskQueue,
              ExamineFormBuilderIndexingHandler examineFormBuildersIndexingHandler,
              Record record)
            {
                backgroundTaskQueue.QueueBackgroundWorkItem(cancellationToken =>
                {
                    using (examineFormBuildersIndexingHandler._scopeProvider.CreateCoreScope(IsolationLevel.Unspecified, RepositoryCacheMode.Unspecified, null, null, new bool?(), false, true))
                    {
                        List<ValueSet> list = [.. examineFormBuildersIndexingHandler._recordValueSetBuilder.GetValueSets(
                        [
              record
                        ])];
                        foreach (IIndex index in examineFormBuildersIndexingHandler._examineManager.Indexes.OfType<IFormBuildersRecordIndex>().Cast<IIndex>())
                            index.IndexItems(list);
                        return Task.CompletedTask;
                    }
                });
            }
        }

        private sealed class DeferedDeleteIndex(
          ExamineFormBuilderIndexingHandler examineFormBuildersIndexingHandler,
          Record record) : DeferedAction
        {
            private readonly ExamineFormBuilderIndexingHandler _examineFormBuildersIndexingHandler = examineFormBuildersIndexingHandler;
            private readonly Record _record = record;

            public override void Execute() => Execute(_examineFormBuildersIndexingHandler, _record);

            public static void Execute(
              ExamineFormBuilderIndexingHandler examineFormBuildersIndexingHandler,
              Record record)
            {
                string itemId = record.Id.ToString(CultureInfo.InvariantCulture);
                foreach (IIndex index in examineFormBuildersIndexingHandler._examineManager.Indexes.OfType<IFormBuildersRecordIndex>().Cast<IIndex>())
                    index.DeleteFromIndex(itemId);
            }
        }
    }
}