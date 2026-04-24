using FormBuilder.Core.Filters;

using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Core.Attributes
{
    public class ValidateFormsAntiForgeryTokenAttribute : TypeFilterAttribute
    {
        public ValidateFormsAntiForgeryTokenAttribute()
          : base(typeof(ValidateFormsAntiForgeryTokenFilter))
        {
        }
    }
}