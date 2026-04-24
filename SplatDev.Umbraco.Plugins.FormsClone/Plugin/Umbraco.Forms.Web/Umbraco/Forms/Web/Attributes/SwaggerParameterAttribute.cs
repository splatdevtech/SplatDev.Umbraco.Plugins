
// Type: Umbraco.Forms.Web.Attributes.SwaggerParameterAttribute
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using System;


#nullable enable
namespace Umbraco.Forms.Web.Attributes
{
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
  public class SwaggerParameterAttribute : Attribute
  {
    public SwaggerParameterAttribute(string name, string description)
    {
      this.Name = name;
      this.Description = description;
    }

    public string Name { get; private set; }

    public string Description { get; private set; }
  }
}
