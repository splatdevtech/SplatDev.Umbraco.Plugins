namespace SplatDev.Logger
{
    public static class Helpers
    {
        public static string TypeString(this LogType type)
        {
            switch (type)
            {
                case LogType.Error: return "Error";
                case LogType.Info: return "Information";
                case LogType.Warning: return "Warning";
                default: return "";
            }
        }
    }
}