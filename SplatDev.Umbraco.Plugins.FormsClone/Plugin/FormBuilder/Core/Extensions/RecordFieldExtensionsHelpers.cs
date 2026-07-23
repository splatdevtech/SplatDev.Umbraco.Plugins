using Umbraco.Extensions;

namespace FormBuilder.Core.Extensions
{
    internal static class RecordFieldExtensionsHelpers
    {
        public static IEnumerable<string> GetSelectedPrevalues(
          IEnumerable<string> fieldPreValues,
          string fieldValue)
        {
            if (string.IsNullOrEmpty(fieldValue))
                return [];
            IOrderedEnumerable<string> orderedEnumerable = fieldPreValues.OrderByDescending(x => x.Length);
            string str1 = fieldValue + ", ";
            foreach (string str2 in (IEnumerable<string>)orderedEnumerable)
                str1 = str1.Replace(str2 + ", ", str2.Replace(",", "|,") + "|||");
            string[] selectedPrevalues = str1.Split(
            [
                "|||"
            ], StringSplitOptions.RemoveEmptyEntries);
            for (int index = 0; index < selectedPrevalues.Length; ++index)
                selectedPrevalues[index] = selectedPrevalues[index].Replace("|,", ",");
            return selectedPrevalues;
        }
    }
}