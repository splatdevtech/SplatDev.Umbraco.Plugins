using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplatDev.Umbraco.Plugins.PropertiesReport
{
    public class Class1
    {
        /*
        @inherits Umbraco.Web.Mvc.UmbracoViewPage
 @using ContentModels = Umbraco.Web.PublishedModels;
 @using System.Linq
 @{
     Layout = null;
     @* Walk up the tree from the current page to get the root node *@
     var documentTypes = Services.ContentTypeService.GetAll();
 }


 <!DOCTYPE html>
 <html>
 <head>
     <title>Property List Vanilla</title>
     <style>
         body {
             font-family: 'Lucida Sans', 'Lucida Sans Regular', 'Lucida Grande', 'Lucida Sans Unicode', Geneva, Verdana, sans-serif;
         }

         table {
             width: 100%;
             border: 1px solid #ccc;
         }

         tr:nth-child(odd) {
             background: #fff;
         }

         tr:nth-child(even) {
             background: #f2f2f2;
         }

         th {
             text-align: left;
             padding: 5px 10px;
             color: #111;
             background: #f9f9f9;
         }
     </style>
 </head>
 <body>
     <h1>Property List Vanilla</h1>
     @foreach (var documentType in documentTypes)
     {
         <h1>@documentType.Name (@documentType.Alias)</h1>
         <div class="list">
             <table>
                 <thead>
                     <tr>
                         <th>Property Group</th>
                         <th>Properties</th>
                     </tr>
                 </thead>
                 <tbody>
                     @foreach (var propertyGroup in documentType.PropertyGroups)
                     {
                         <tr>
                             <td>@propertyGroup.Name (@propertyGroup.PropertyTypes.Count)</td>
                             <td>
                                 <table>
                                     <thead>
                                         <tr>
                                             <th>Property Name</th>
                                             <th>Property Alias</th>
                                             <th>Property Type</th>
                                         </tr>
                                     </thead>
                                     <tbody>

                                         @foreach (var property in propertyGroup.PropertyTypes)
                                         {
                                             var dataType = Services.DataTypeService.GetAll(property.DataTypeId).FirstOrDefault();
                                             <tr>
                                                 <td>@property.Name</td>
                                                 <td>@property.Alias</td>
                                                 <td>@dataType.Name</td>
                                             </tr>
                                         }
                                     </tbody>
                                 </table>
                             </td>
                         </tr>
                     }
                 </tbody>
             </table>
         </div>
     }
 </body>
 </html>
         */
    }
}
