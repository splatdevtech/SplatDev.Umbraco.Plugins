using Umbraco.Cms.Api.Common.OpenApi;

namespace FormBuilder.Core.Handlers
{
    internal sealed class FormsSchemaIdHandler : SchemaIdHandler
    {
        public override bool CanHandle(Type type) => type.Namespace?.StartsWith("FormBuilder.Web.Api.Management") ?? false;
    }
}