using System.Collections.Generic;

namespace UmbracoCms.Plugins
{
    public static partial class Default
    {
        public static class MediaTypes
        {
            public static class Dictionaries
            {
                public static Dictionary<string, int> IdByAlias = new Dictionary<string, int>
                {
                    {   Alias.Image, Ids.Image      },
                    {   Alias.Folder, Ids.Folder    },
                    {   Alias.File, Ids.File        },
                    {   Alias.Custom, Ids.Custom    },
                };
            }
            public static class Alias
            {
                public const string Image                       = "Image";
                public const string Folder                      = "Folder";
                public const string File                        = "File";
                public const string Custom                      = "";
            }

            public static class Ids
            {
                public const int Custom                         = -999;
                public const int Folder                         = 1031;
                public const int Image                          = 1032;
                public const int File                           = 1033;
            }
        }
    }
}
