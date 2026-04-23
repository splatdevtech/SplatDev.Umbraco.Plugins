
// Type: Umbraco.Forms.Core.Extensions.TypeWithEditorDetailsExtensions
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Forms.Core.Interfaces;


#nullable enable
namespace Umbraco.Forms.Core.Extensions
{
  internal static class TypeWithEditorDetailsExtensions
  {
    public static void PopulateCreatedDetails(
      this ITypeWithEditorDetails item,
      IBackOfficeSecurityAccessor backOfficeSecurityAccessor)
    {
      int? id = backOfficeSecurityAccessor.BackOfficeSecurity?.CurrentUser?.Id;
      if (id.HasValue)
      {
        if (!item.CreatedBy.HasValue)
          item.CreatedBy = id;
        if (!item.UpdatedBy.HasValue)
          item.UpdatedBy = id;
      }
      item.Created = item.Updated = DateTime.Now;
    }

    public static void PopulateUpdatedDetails(
      this ITypeWithEditorDetails item,
      IBackOfficeSecurityAccessor backOfficeSecurityAccessor)
    {
      int? id = backOfficeSecurityAccessor.BackOfficeSecurity?.CurrentUser?.Id;
      if (id.HasValue && !item.UpdatedBy.HasValue)
        item.UpdatedBy = id;
      item.Updated = DateTime.Now;
    }

    public static void PopulateEditorDetails(
      this ITypeWithEditorDetails item,
      IUserService userService)
    {
      if (item == null)
        return;
      int? nullable;
      if (item.CreatedBy.HasValue)
      {
        ITypeWithEditorDetails withEditorDetails = item;
        IUserService userService1 = userService;
        nullable = item.CreatedBy;
        int id = nullable.Value;
        string name = userService1.GetUserById(id)?.Name;
        withEditorDetails.CreatedByName = name;
      }
      nullable = item.UpdatedBy;
      if (!nullable.HasValue)
        return;
      ITypeWithEditorDetails withEditorDetails1 = item;
      IUserService userService2 = userService;
      nullable = item.UpdatedBy;
      int id1 = nullable.Value;
      string name1 = userService2.GetUserById(id1)?.Name;
      withEditorDetails1.UpdatedByName = name1;
    }
  }
}
