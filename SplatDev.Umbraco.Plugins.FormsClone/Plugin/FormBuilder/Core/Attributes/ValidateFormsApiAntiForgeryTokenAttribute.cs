using FormBuilder.Core.Filters;

using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Core.Attributes
{
    public class ValidateFormsApiAntiForgeryTokenAttribute : TypeFilterAttribute
    {
        public ValidateFormsApiAntiForgeryTokenAttribute()
          : base(typeof(ValidateFormsApiAntiForgeryTokenFilter))
        {
        }
    }
}