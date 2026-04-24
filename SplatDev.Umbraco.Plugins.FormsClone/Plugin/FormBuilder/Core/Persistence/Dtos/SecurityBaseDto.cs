using System.Runtime.Serialization;

using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace FormBuilder.Core.Persistence.Dtos
{
    [DataContract(Name = "securityBase")]
    public abstract class SecurityBaseDto
    {
        [DataMember(Name = "manageDataSources")]
        public bool ManageDataSources { get; set; }

        [DataMember(Name = "managePreValueSources")]
        public bool ManagePreValueSources { get; set; }

        [DataMember(Name = "manageWorkflows")]
        public bool ManageWorkflows { get; set; }

        [DataMember(Name = "manageForms")]
        public bool ManageForms { get; set; }

        [DataMember(Name = "viewEntries")]
        [Constraint(Default = "0")]
        public bool ViewEntries { get; set; }

        [DataMember(Name = "editEntries")]
        [Constraint(Default = "0")]
        public bool EditEntries { get; set; }

        [DataMember(Name = "deleteEntries")]
        [Constraint(Default = "0")]
        public bool DeleteEntries { get; set; }
    }
}