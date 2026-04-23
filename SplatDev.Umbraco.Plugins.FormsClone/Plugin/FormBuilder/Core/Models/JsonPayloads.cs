using FormBuilder.Core.DataSources;
using FormBuilder.Core.Prevalues;

namespace FormBuilder.Core.Models
{
    public class JsonPayloads
    {
        public class FolderPayload
        {
            public Folder? Folder { get; set; }

            public bool DeleteFolder { get; set; }
        }

        public class FormPayload
        {
            public Form? Form { get; set; }

            public bool DeleteForm { get; set; }
        }

        public class WorkflowPayload
        {
            public Workflow? Workflow { get; set; }

            public bool DeleteWorkflow { get; set; }
        }

        public class DataSourcePayload
        {
            public FormDataSource? DataSource { get; set; }

            public bool DeleteDataSource { get; set; }
        }

        public class PreValuePayload
        {
            public FieldPrevalueSource? Prevalue { get; set; }

            public bool DeletePreValue { get; set; }
        }
    }
}