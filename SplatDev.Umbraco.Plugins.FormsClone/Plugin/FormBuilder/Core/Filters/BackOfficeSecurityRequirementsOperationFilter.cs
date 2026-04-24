using Umbraco.Cms.Api.Management.OpenApi;

namespace FormBuilder.Core.Filters
{
    public class BackOfficeSecurityRequirementsOperationFilter :
      BackOfficeSecurityRequirementsOperationFilterBase
    {
        protected override string ApiName => "formBuilder-management";
    }
}