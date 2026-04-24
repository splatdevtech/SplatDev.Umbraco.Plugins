
// Type: Umbraco.Forms.Web.Api.Configuration.ApiConstants
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73


#nullable enable
namespace Umbraco.Forms.Web.Api.Configuration
{
  internal static class ApiConstants
  {
    public static class DeliveryApi
    {
      public const string RootPath = "/umbraco/forms/delivery/api";
      public const string ApiTitle = "Umbraco Forms Delivery API";
      public const string ApiName = "forms-delivery";
      public const string ApiGroupName = "Forms";
      public const string ApiDocumentationArticleLink = "https://docs.umbraco.com/umbraco-forms/v/12.forms.latest/developer/ajaxforms";
    }

    public static class ManagementApi
    {
      public const string RootPath = "/umbraco/forms/management/api";
      public const string ApiTitle = "Umbraco Forms Management API";
      public const string ApiName = "forms-management";
      public const string ApiNamespacePrefix = "Umbraco.Forms.Web.Api.ManagementApi";
    }
  }
}
