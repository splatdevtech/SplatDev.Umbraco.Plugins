namespace UmbracoCms.CodeFirst.Acl
{
    /// <summary>
    /// Implements <see cref="IAccessRule"/>.
    /// </summary>
    public class AccessRule : IAccessRule
    {
        /// <inheritdoc />
        public AccessRuleTypes Type { get; set; }

        /// <inheritdoc />
        public string Value { get; set; }
    }
}