namespace FormBuilder.Core.Extensions
{
    /// <summary>
    /// Defines constants related to the Razor rendering of forms.
    /// </summary>
    public static class WebConstants
    {
        /// <summary>Defines constant related to TempData keys.</summary>
        public static class TempDataKeys
        {
            /// <summary>
            /// The TempData key used for the submitted record (form entry).
            /// </summary>
            public const string SubmittedRecordId = "Forms_Current_Record_id";

            /// <summary>
            /// The TempData or HttpContext.Items key used for tracking rendered forms for the purpose of
            /// rendering of associated scripts.
            /// </summary>
            public const string RenderedFormIds = "FormBuilder";
        }

        /// <summary>Defines constant related to querystring keys.</summary>
        public static class QueryStringKeys
        {
            /// <summary>Record Id.</summary>
            public const string RecordId = "recordId";
        }
    }
}