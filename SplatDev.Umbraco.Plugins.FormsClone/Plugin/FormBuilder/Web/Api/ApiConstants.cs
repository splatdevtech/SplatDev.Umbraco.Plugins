namespace FormBuilder.Web.Api
{
    internal static class ApiConstants
    {
        /// <summary>
        /// Defines constants for the Form Builder Delivery (AJAX/Headless) API.
        /// </summary>
        public static class DeliveryApi
        {
            /// <summary>The API root path.</summary>
            public const string RootPath = "/formBuilder/delivery/api";

            /// <summary>The API title.</summary>
            public const string ApiTitle = "Form Builder Delivery API";

            /// <summary>The API name.</summary>
            public const string ApiName = "forms-delivery";

            /// <summary>The API group name.</summary>
            public const string ApiGroupName = "Forms";

            /// <summary>The API documentation link.</summary>
            public const string ApiDocumentationArticleLink = "";
        }

        /// <summary>Defines constants for the Forms Management API.</summary>
        public static class ManagementApi
        {
            /// <summary>The API root path.</summary>
            public const string RootPath = "/formBuilder/management/api";

            /// <summary>The API title.</summary>
            public const string ApiTitle = "Form Builder Management API";

            /// <summary>The API name.</summary>
            public const string ApiName = "formBuilder-management";

            /// <summary>The namespace prefix for Forms Management API.</summary>
            public const string ApiNamespacePrefix = "FormBuilder.Web.Api.Management";
        }
    }
}