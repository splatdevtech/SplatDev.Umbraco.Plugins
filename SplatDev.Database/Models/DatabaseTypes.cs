namespace SplatDev.Database.Models
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    public struct DatabaseTypes
    {
        private Dictionary<string, string> _type;
        public Dictionary<string, string> Type
        {
            get
            {
                _type = new Dictionary<string, string>
                {
                    { "byte", "TINYINT" },
                    { "int16", "SMALLINT" },
                    { "int32", "INT" },
                    { "int64", "BIGINT" },
                    { "string", "NVARCHAR(255)" },
                    { "boolean", "BIT" },
                    { "decimal", "DECIMAL" },
                    { "float", "FLOAT" },
                    { "single", "FLOAT" },
                    { "double", "FLOAT" },
                    { "datetime", "DATETIME" }
                };

                return _type;
            }
        }


        public string this[PropertyInfo property]
        {
            get
            {
                try
                {
                    return Type[property.PropertyType.Name.ToLower()];
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
        }
    }
}
