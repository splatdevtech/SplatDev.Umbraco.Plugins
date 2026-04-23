
// Type: Umbraco.Forms.Web.Api.Configuration.FormsSchemaIdHandler
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using System;
using Umbraco.Cms.Api.Common.OpenApi;


#nullable enable
namespace Umbraco.Forms.Web.Api.Configuration
{
  internal sealed class FormsSchemaIdHandler : SchemaIdHandler
  {
    public override bool CanHandle(Type type) => type.Namespace?.StartsWith("Umbraco.Forms.Web.Api.ManagementApi") ?? false;
  }
}
