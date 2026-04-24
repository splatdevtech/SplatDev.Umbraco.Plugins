using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Fields;

using System.Collections;

namespace FormBuilder.Core.Services.Interfaces
{
    public interface IPlaceholderParsingService
    {
        string ParsePlaceHolders(
          string value,
          bool htmlEncodeValues,
          Record? record = null,
          Form? form = null,
          Hashtable? pageElements = null,
          IDictionary<string, string?>? additionalData = null);
    }
}