namespace FormBuilder.Core.Configuration
{
    public class DatePickerSettings
    {
        public int DatePickerYearRange { get; set; } = 10;

        public string DatePickerFormat { get; set; } = "LL";

        public string DatePickerFormatForValidation { get; set; } = string.Empty;
    }
}