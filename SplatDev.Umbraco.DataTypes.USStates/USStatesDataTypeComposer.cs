using Microsoft.Extensions.DependencyInjection;

using Umbraco.Cms.Core.Composing;

namespace SplatDev.Umbraco.DataTypes.USStates
{
    public class USStatesDataTypeComposer : ComponentComposer<USStatesDataTypeComponent> { }

    public class USStatesDataTypeComponent(IServiceScopeFactory scopeFactory) : IComponent
    {
        private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

        public void Initialize()
        {
            new USStatesDataType(_scopeFactory).Install();
        }

        public void Terminate()
        {
        }
    }
}
