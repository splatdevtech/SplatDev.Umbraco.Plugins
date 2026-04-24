namespace FormBuilder.Core.Providers.FieldTypes
{
    public static class DatePickerHelpers
    {
        public static IEnumerable<object> ProcessFieldValues(
          IEnumerable<object> postedValues)
        {
            List<object> objectList = [];
            postedValues = [.. postedValues];
            if (!postedValues.Any())
                return objectList;
            if (DateTime.TryParse(postedValues.First().ToString(), out DateTime result))
                objectList.Add(result);
            return objectList;
        }
    }
}