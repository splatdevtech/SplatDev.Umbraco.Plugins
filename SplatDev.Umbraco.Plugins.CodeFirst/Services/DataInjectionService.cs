namespace SplatDev.Umbraco.Plugins.CodeFirst.Services
{
    using NPoco;

    using SplatDev.Database.Helpers;

    using System.Collections.Generic;
    public abstract class DataInjectionService<T> where T : class
    {
        public string Table { get; set; }
        public List<T> Data { get; set; }
        public Database Database { get; set; }
        public bool SkipIfHasData { get; set; }

        public virtual void InsertData()
        {
            DatabaseHelper.InsertData<T>(Database, Data, SkipIfHasData, Table);
        }
    }
}
