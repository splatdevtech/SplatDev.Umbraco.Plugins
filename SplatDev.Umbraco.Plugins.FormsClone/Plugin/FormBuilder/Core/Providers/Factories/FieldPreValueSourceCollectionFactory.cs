using FormBuilder.Core.Providers.Collections;

using Microsoft.Extensions.DependencyInjection;

namespace FormBuilder.Core.Providers.Factories
{
    public class FieldPrevalueSourceCollectionFactory(IServiceProvider serviceProvider)
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        public FieldPrevalueSourceCollection GetPreValueSourceCollection() => _serviceProvider.GetRequiredService<FieldPrevalueSourceCollection>();
    }
}