using System.Text.RegularExpressions;

namespace FormBuilder.Extension.Forms.Web.Management
{
    internal static partial class ManagementApiRegexes
    {
        private static readonly Lazy<Regex> _controllerTypeToNameRegex = new(() =>
            ControllerTypeToNameRegexPattern());

        public static Regex ControllerTypeToNameRegex() => _controllerTypeToNameRegex.Value;

        [GeneratedRegex("(Controller)$", RegexOptions.Compiled)]
        public static partial Regex ControllerTypeToNameRegexPattern();
    }
}