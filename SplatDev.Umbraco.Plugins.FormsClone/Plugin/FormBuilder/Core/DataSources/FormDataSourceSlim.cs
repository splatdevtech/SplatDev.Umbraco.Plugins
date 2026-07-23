using FormBuilder.Core.Models;

using System.Runtime.Serialization;

namespace FormBuilder.Core.DataSources
{
    [DataContract(Name = "formDataSource", Namespace = "")]
    [Serializable]
    public class FormDataSourceSlim : BaseSlim
    {
    }
}