using System;
using System.Collections.Generic;

namespace UmbracoCms.Plugins
{
    public static partial class Default
    {
        //from https://our.umbraco.org/forum/developers/api-questions/10081-Getting-Datatype-ID
        public static class DataTypes
        {
            public static class DataTypesDictionaries
            {
                public static Dictionary<string, int> IdByAlias = new Dictionary<string, int>
                {
                    {   Alias.ApprovedColor                       , Ids.ApprovedColor	    },
                    {   Alias.CheckboxList                        , Ids.CheckboxList	    },
                    {   Alias.ContentPicker                       , Ids.ContentPicker	    },
                    {   Alias.Custom                              , Ids.Custom	            },
                    {   Alias.DatePicker                          , Ids.DatePicker	        },
                    {   Alias.DatePickerWithTime                  , Ids.DatePickerWithTime	},
                    {   Alias.Dropdown                            , Ids.Dropdown	        },
                    {   Alias.DropdownMultiple                    , Ids.DropdownMultiple	},
                    {   Alias.ImageCropper                        , Ids.ImageCropper	    },
                    {   Alias.Label                               , Ids.Label	            },
                    {   Alias.ListViewContent                     , Ids.ListViewContent	    },
                    {   Alias.ListViewMedia                       , Ids.ListViewMedia	    },
                    {   Alias.ListViewMember                      , Ids.ListViewMember	    },
                    {   Alias.MediaPicker                         , Ids.MediaPicker	        },
                    {   Alias.MemberPicker                        , Ids.MemberPicker	    },
                    {   Alias.MultipleMediaPicker                 , Ids.MultipleMediaPicker	},
                    {   Alias.NestedContent                       , Ids.NestedContent	    },
                    {   Alias.Numeric                             , Ids.Numeric	            },
                    {   Alias.Radiobox                            , Ids.Radiobox	        },
                    {   Alias.RelatedLinks2                       , Ids.RelatedLinks2	    },
                    {   Alias.RichtextEditor                      , Ids.RichtextEditor	    },
                    {   Alias.Tags                                , Ids.Tags	            },
                    {   Alias.TextArea                            , Ids.TextArea	        },
                    {   Alias.Textstring                          , Ids.Textstring	        },
                    {   Alias.TrueFalse                           , Ids.TrueFalse	        },
                    {   Alias.Upload                              , Ids.Upload	            },
                };

                public static Dictionary<string, int> IdByName  = new Dictionary<string, int>
                {
                    {   Names.ApprovedColor                       , Ids.ApprovedColor       },
                    {   Names.CheckboxList                        , Ids.CheckboxList        },
                    {   Names.ContentPicker                       , Ids.ContentPicker       },
                    {   Names.Custom                              , Ids.Custom              },
                    {   Names.DatePicker                          , Ids.DatePicker          },
                    {   Names.DatePickerWithTime                  , Ids.DatePickerWithTime  },
                    {   Names.Dropdown                            , Ids.Dropdown            },
                    {   Names.DropdownMultiple                    , Ids.DropdownMultiple    },
                    {   Names.ImageCropper                        , Ids.ImageCropper        },
                    {   Names.Label                               , Ids.Label               },
                    {   Names.ListViewContent                     , Ids.ListViewContent     },
                    {   Names.ListViewMedia                       , Ids.ListViewMedia       },
                    {   Names.ListViewMember                      , Ids.ListViewMember      },
                    {   Names.MediaPicker                         , Ids.MediaPicker         },
                    {   Names.MemberPicker                        , Ids.MemberPicker        },
                    {   Names.MultipleMediaPicker                 , Ids.MultipleMediaPicker },
                    {   Names.NestedContent                       , Ids.NestedContent       },
                    {   Names.Numeric                             , Ids.Numeric             },
                    {   Names.Radiobox                            , Ids.Radiobox            },
                    {   Names.RelatedLinks2                       , Ids.RelatedLinks2       },
                    {   Names.RichtextEditor                      , Ids.RichtextEditor      },
                    {   Names.Tags                                , Ids.Tags                },
                    {   Names.TextArea                            , Ids.TextArea            },
                    {   Names.Textstring                          , Ids.Textstring          },
                    {   Names.TrueFalse                           , Ids.TrueFalse           },
                    {   Names.Upload                              , Ids.Upload              },
                };
            }

            public static class Alias
            {
                public const string ApprovedColor               = "Umbraco.ColorPickerAlias";
                public const string CheckboxList                = "Umbraco.CheckBoxList";
                public const string ContentPicker               = "Umbraco.ContentPicker2";
                public const string Custom                      = "";
                public const string DatePicker                  = "Umbraco.Date";
                public const string DatePickerWithTime          = "Umbraco.DateTime";
                public const string Dropdown                    = "Umbraco.DropDown";
                public const string DropdownMultiple            = "Umbraco.DropDownMultiple";
                public const string ImageCropper                = "Umbraco.ImageCropper";
                public const string Label                       = "Umbraco.NoEdit";
                public const string ListViewContent             = "Umbraco.ListView";
                public const string ListViewMedia               = "Umbraco.ListView";
                public const string ListViewMember              = "Umbraco.ListView";
                public const string MediaPicker                 = "Umbraco.MediaPicker2";
                public const string MemberPicker                = "Umbraco.MemberPicker2";
                public const string MultipleMediaPicker         = "Umbraco.MediaPicker2";
                public const string NestedContent               = "Umbraco.NestedContent";
                public const string Numeric                     = "Umbraco.Integer";
                public const string Radiobox                    = "Umbraco.RadioButtonList";
                public const string RelatedLinks2               = "Umbraco.RelatedLinks2";
                public const string RichtextEditor              = "Umbraco.TinyMCEv3";
                public const string Tags                        = "Umbraco.Tags";
                public const string TextArea                    = "Umbraco.TextboxMultiple";
                public const string Textstring                  = "Umbraco.Textbox";
                public const string TrueFalse                   = "Umbraco.TrueFalse";
                public const string Upload                      = "Umbraco.UploadField";
            }

            public static class Ids
            {
                /// <summary>
                /// Adds a list of approved colours which can be selected by clicking. The approved colours need to be added as hex values (without the #) in the prevalues field. i.e. cccccc
                /// </summary>
                public const int ApprovedColor                  = -37;

                /// <summary>
                /// Displays a list of preset values as a list of checkbox controls. The preset values are modified in the developer section under "data types" / checkbox list where new items can be added. The value saved is a comma-separated string of prevalue IDs; which is easiliest processed with xslt. (umbraco.library:GetPrevalue())
                /// </summary>
                public const int CheckboxList                   = -43;

                /// <summary>
                /// The content picker opens a simple modal to pick a specific page from the content structure. The value saved is the selected page's ID. This ID can be used in xslt with umbraco.library:GetXmlNodeById(ID) to get the page's content
                /// </summary>
                public const int ContentPicker                  = 1046;

                /// <summary>
                /// A Custom Control (Normally a Custom Data Type with a Custom Property Editor)
                /// </summary>
                public const int Custom                         = 0;

                /// <summary>
                /// Displays a calendar UI for selecting dates and time; the value saved is a standard dateTime value; but with no time information.
                /// </summary>
                public const int DatePicker                     = -41;

                /// <summary>
                /// Displays a calendar UI for selecting dates and time; the value saved is a standard dateTime value
                /// </summary>
                public const int DatePickerWithTime             = -36;

                /// <summary>
                /// Displays a list of preset values as a list where only a single can be selected. The preset values are modified in the developer section under "data types" / Dropdown multple where new items can be added. The value saved is the selected value as a string.
                /// </summary>
                public const int Dropdown                       = -42;

                /// <summary>
                /// Displays a list of preset values as a list where multiple values can be selected. The preset values are modified in the developer section under "data types" / Dropdown multple where new items can be added. The value saved is a commasepareted string of prevalue IDs; which is easiliest processed with xslt. (umbraco.library:GetPrevalue())
                /// </summary>
                public const int DropdownMultiple               = -39;

                /// <summary>
                /// Used mainly with container Media Types; the Folder Browser displays a list of thumbnail images. Every Media item contained within the folder that has a generated thumbnail will be listed.
                /// </summary>
                public const int FolderBrowser                  = -38;

                /// <summary>
                /// An Image Cropper that allows setting the focus point and several crop sizes
                /// </summary>
                public const int ImageCropper                   = 1043;

                /// <summary>
                /// Is a non-editable control; can only be used to display a present text. It can also be used in the media section to load in values related to the node; such as width; height and file size.
                public const int    /// </summary>
                    Label                                       = -92;

                /// <summary>
                /// List View (Content)
                /// </summary>
                public const int ListViewContent                = -95;

                /// <summary>
                /// List View (Media)
                /// </summary>
                public const int ListViewMedia                  = -96;

                /// <summary>
                /// List View (Members)
                /// </summary>
                public const int ListViewMember                 = -97;

                /// <summary>
                /// The content picker opens a simple modal to pick a specific media item from the media tree. The value saved is the selected media node ID. This ID can be used in xslt with umbraco.library:GetMedia(ID) to get the media items xml data
                /// </summary>
                public const int MediaPicker                    = 1048;

                /// <summary>
                /// An Umbraco Member (not user)
                /// </summary>
                public const int Member                         = 1044;

                /// <summary>
                /// Displays a simple dropdown with all available members in. A single member can be selected. The value saved is the ID of the member
                /// </summary>
                public const int MemberPicker                   = 1047;

                /// <summary>
                /// A dropdown that allows selection of multiple media items; including folders
                /// </summary>
                public const int MultipleMediaPicker            = 1049;

                //data types from https://our.umbraco.org/documentation/getting-started/data/data-types/default-data-types
                /// <summary>
                /// New in 7.7.7+ nested content
                /// </summary>
                public const int NestedContent                  = -999;
                /// <summary>
                /// A simple textbox to input a numeric value.
                /// </summary>
                public const int Numeric                        = -51;

                /// <summary>
                /// this Data type enables editors to choose from list of radiobuttons. Options for the Radiobox need to be set in the Developer section by adding prevalues to the Data type
                /// </summary>
                public const int Radiobox                       = -40;

                /// <summary>
                /// This datatype allows an editor to easily add an array of links. These can either be internal Umbraco pages or external URLs.
                /// </summary>
                [Obsolete("Use RelatedLinks2 for better results")]
                public const int RelatedLinks                   = 1040;

                /// <summary>
                /// This datatype allows an editor to easily add an array of links. These can either be internal Umbraco pages or external URLs.
                /// </summary>
                public const int RelatedLinks2                  = 1050;

                /// <summary>
                /// <para>
                /// The TinyMCE based wysiwyg editor. This is the standard editor used to edit any larger amount of text. The editor has a lot of settings; which can be changed under the developer section in "data types" / Richtext editor. The editor also supports TinyMCE plugins which can be controlled in the configuration file located at /config/tinyMce.config
                ///In the default settings some tags such as bullet list can be used. If you want to use other tags like h1 or h2; you need to add stylesheets.
                ///Create child stylesheets for each tag(h1 or h2) under a base one. Go to "Back office->Developer->Data Types->Richtext editor" and associate rich text editor with the base. Also turn on "styleselect" in the toolbar section. You can find a new button in the toolbar of the content editor.
                /// </para>
                /// <para>An example of the style sheet tree is as follows.</para>
                /// <para>
                /// Stylesheets
                ///-IE7
                ///-IE8
                ///-Style
                ///-RichEdit
                ///--h1
                ///--h2
                /// </para>
                /// </summary>
                public const int RichtextEditor                 = -87;

                /// <summary>
                /// A textbox that allows you to use use multiple tags on a docType - This is what is used on Blog4Umbraco and is perfect if you need to categorise data. You can specify a TAG Group when creating new versions of this datatype; in case you need to use TAGS on different sections of your site (i.e News; Article; Events).
                /// </summary>
                public const int Tags                           = 1041;

                /// <summary>
                /// A simple textarea control to import text
                /// </summary>
                public const int TextArea                       = -89;

                /// <summary>
                /// A normal html input text field
                /// </summary>
                public const int Textstring                     = -88;

                /// <summary>
                /// A simple checkbox which saves either 0 or 1; depending on the checkbox being checked or not. A common use for instance is to create a property with the special alias 'umbracoNaviHide' and the Data-Type True/False to enable editors to hide nodes from appearing in a navigation menu
                /// </summary>
                public const int TrueFalse                      = -49;

                /// <summary>
                /// Adds an upload field; which allows documents or images to be uploaded to umbraco. This does nto add them to the media library; they are simply added to the document data.
                /// </summary>
                public const int Upload                         = -90;
            }

            public static class Names
            {
                public const string ApprovedColor               = "Approved Color";
                public const string CheckboxList                = "Checkbox list";
                public const string ContentPicker               = "Content Picker";
                public const string Custom                      = "Custom";
                public const string DatePicker                  = "Date Picker";
                public const string DatePickerWithTime          = "Date Picker with Time";
                public const string Dropdown                    = "Dropdown";
                public const string DropdownMultiple            = "Dropdown Multiple";
                public const string FolderBrowser               = "Folder Browser";
                public const string ImageCropper                = "Image Cropper";
                public const string Label                       = "Label";
                public const string ListViewContent             = "Content List View";
                public const string ListViewMedia               = "Media List View";
                public const string ListViewMember              = "Member List View";
                public const string MediaPicker                 = "Media Picker";
                public const string MemberPicker                = "Member Picker";
                public const string MultipleMediaPicker         = "Multiple Media Picker";
                public const string NestedContent               = "Nested Content";
                public const string Numeric                     = "Numeric";
                public const string Radiobox                    = "Radiobox";
                public const string RelatedLinks2               = "Related Links";
                public const string RichtextEditor              = "Richtext Editor";
                public const string Tags                        = "Tags";
                public const string TextArea                    = "Textbox multiple";
                public const string Textstring                  = "Textbox";
                public const string TrueFalse                   = "True/False";
                public const string Upload                      = "Upload";
            }
        }
    }
}