
// Type: Umbraco.Forms.Core.Security.IFormsSecurity
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using Umbraco.Cms.Core.Models.Membership;


#nullable enable
namespace Umbraco.Forms.Core.Security
{
  public interface IFormsSecurity
  {
    bool HasAccessToForm(Guid form);

    bool HasAccessToForm(Guid form, IUser user);

    bool CanCurrentUserManageWorkflows();

    bool CanCurrentUserManageForms();

    bool CanUserManageForms(IUser user);

    bool CanCurrentUserManageDataSources();

    bool CanCurrentUserManagePreValues();

    bool CanCurrentUserViewEntries();

    bool CanUserViewEntries(IUser user);

    bool CanCurrentUserEditEntries();

    bool CanCurrentUserDeleteEntries();

    IEnumerable<Guid> FilterFormIdsForCurrentUser(IEnumerable<Guid> formIds);

    IEnumerable<Guid> FilterFormIdsForUser(IEnumerable<Guid> formIds, IUser user);

    IEnumerable<Guid> GetStartFolderKeysForCurrentUser();

    IEnumerable<Guid> GetStartFolderKeysForUser(IUser user);
  }
}
