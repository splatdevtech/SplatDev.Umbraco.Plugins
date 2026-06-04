using Microsoft.Extensions.DependencyInjection;

using Umbraco.Cms.Core.Composing;

namespace SplatDev.Umbraco.DataTypes.USStates
{
    public class USStatesDataTypeComposer : ComponentComposer<USStatesDataTypeComponent> { }

    public class USStatesDataTypeComponent(IServiceScopeFactory scopeFactory) : IAsyncComponent
    {
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

        public Task InitializeAsync(CancellationToken cancellationToken)
        {
            new USStatesDataType(_scopeFactory).Install();
            return Task.CompletedTask;
        }

        public Task TerminateAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
