using SplatDev.Umbraco.Plugins.Schema2Yaml.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace SplatDev.Umbraco.Plugins.Schema2Yaml.Tests.Services;

public class ExportModelsTests
{
    private static ISerializer BuildSerializer() =>
        new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

    private static IDeserializer BuildDeserializer() =>
        new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();

    [Fact]
    public void ExportRoot_Serializes_WithUmbracoKey()
    {
        var root = new ExportRoot();
        var yaml = BuildSerializer().Serialize(root);
        Assert.Contains("umbraco:", yaml);
    }

    [Fact]
    public void ExportDataType_Serializes_AllFields()
    {
        var dataType = new ExportDataType
        {
            Alias = "textstring",
            Name = "Textstring",
            EditorUiAlias = "Umb.PropertyEditorUi.TextBox",
            ValueType = "String",
            Config = new Dictionary<string, object> { ["maxChars"] = 0 }
        };

        var yaml = BuildSerializer().Serialize(dataType);

        Assert.Contains("alias: textstring", yaml);
        Assert.Contains("name: Textstring", yaml);
        Assert.Contains("editorUiAlias:", yaml);
        Assert.Contains("valueType: String", yaml);
    }

    [Fact]
    public void ExportDocumentType_Serializes_TabsAndProperties()
    {
        var docType = new ExportDocumentType
        {
            Alias = "blogPost",
            Name = "Blog Post",
            IsElement = false,
            AllowAsRoot = true,
            Tabs =
            [
                new ExportTab
                {
                    Name = "Content",
                    SortOrder = 0,
                    Properties =
                    [
                        new ExportProperty
                        {
                            Alias = "title",
                            Name = "Title",
                            DataType = "Textstring",
                            Required = true,
                            SortOrder = 0
                        }
                    ]
                }
            ]
        };

        var yaml = BuildSerializer().Serialize(docType);

        Assert.Contains("alias: blogPost", yaml);
        Assert.Contains("name: Blog Post", yaml);
        Assert.Contains("isElement: false", yaml);
        Assert.Contains("allowAsRoot: true", yaml);
        Assert.Contains("tabs:", yaml);
        Assert.Contains("properties:", yaml);
        Assert.Contains("alias: title", yaml);
        Assert.Contains("required: true", yaml);
    }

    [Fact]
    public void ExportLanguage_Serializes_IsoCodeAndFlags()
    {
        var language = new ExportLanguage
        {
            IsoCode = "en-US",
            CultureName = "English (United States)",
            IsDefault = true,
            IsMandatory = false
        };

        var yaml = BuildSerializer().Serialize(language);

        Assert.Contains("isoCode: en-US", yaml);
        Assert.Contains("cultureName: English (United States)", yaml);
        Assert.Contains("isDefault: true", yaml);
        Assert.Contains("isMandatory: false", yaml);
    }

    [Fact]
    public void ExportTemplate_Serializes_AliasAndMasterTemplate()
    {
        var template = new ExportTemplate
        {
            Alias = "home",
            Name = "Home",
            MasterTemplate = "master",
            Content = "@inherits Umbraco.Web.Mvc.UmbracoViewPage"
        };

        var yaml = BuildSerializer().Serialize(template);

        Assert.Contains("alias: home", yaml);
        Assert.Contains("masterTemplate: master", yaml);
    }

    [Fact]
    public void ExportDictionaryItem_Serializes_TranslationsDictionary()
    {
        var item = new ExportDictionaryItem
        {
            Key = "general.welcome",
            Translations = new Dictionary<string, string>
            {
                ["en-US"] = "Welcome",
                ["pt-BR"] = "Bem-vindo"
            }
        };

        var yaml = BuildSerializer().Serialize(item);

        Assert.Contains("key: general.welcome", yaml);
        Assert.Contains("translations:", yaml);
        Assert.Contains("en-US: Welcome", yaml);
        Assert.Contains("pt-BR: Bem-vindo", yaml);
    }

    [Fact]
    public void ExportContent_Serializes_ChildrenRecursively()
    {
        var root = new ExportContent
        {
            Name = "Home",
            DocumentType = "home",
            IsPublished = true,
            Children =
            [
                new ExportContent
                {
                    Name = "About",
                    DocumentType = "textPage",
                    IsPublished = true
                }
            ]
        };

        var yaml = BuildSerializer().Serialize(root);

        Assert.Contains("name: Home", yaml);
        Assert.Contains("documentType: home", yaml);
        Assert.Contains("isPublished: true", yaml);
        Assert.Contains("children:", yaml);
        Assert.Contains("name: About", yaml);
    }

    [Fact]
    public void ExportMedia_Serializes_UrlAndFolder()
    {
        var media = new ExportMedia
        {
            Name = "hero.jpg",
            MediaType = "Image",
            Folder = "Banners",
            Url = "/media/1234/hero.jpg"
        };

        var yaml = BuildSerializer().Serialize(media);

        Assert.Contains("name: hero.jpg", yaml);
        Assert.Contains("mediaType: Image", yaml);
        Assert.Contains("folder: Banners", yaml);
        Assert.Contains("url: /media/1234/hero.jpg", yaml);
    }

    [Fact]
    public void ExportMember_Serializes_WithoutPasswordField()
    {
        var member = new ExportMember
        {
            Name = "John Doe",
            Email = "john@example.com",
            Username = "johndoe",
            MemberType = "Member",
            IsApproved = true
        };

        var yaml = BuildSerializer().Serialize(member);

        Assert.Contains("name: John Doe", yaml);
        Assert.Contains("email: john@example.com", yaml);
        Assert.Contains("username: johndoe", yaml);
        Assert.DoesNotContain("password:", yaml);
    }

    [Fact]
    public void ExportUser_Serializes_GroupsList()
    {
        var user = new ExportUser
        {
            Name = "Admin",
            Email = "admin@example.com",
            Username = "admin",
            UserGroups = ["Administrators", "Editors"]
        };

        var yaml = BuildSerializer().Serialize(user);

        Assert.Contains("name: Admin", yaml);
        Assert.Contains("userGroups:", yaml);
        Assert.Contains("Administrators", yaml);
        Assert.Contains("Editors", yaml);
    }

    [Fact]
    public void UmbracoExport_Serializes_AllSectionKeys()
    {
        var export = new UmbracoExport();
        var yaml = BuildSerializer().Serialize(export);

        Assert.Contains("languages:", yaml);
        Assert.Contains("dataTypes:", yaml);
        Assert.Contains("documentTypes:", yaml);
        Assert.Contains("mediaTypes:", yaml);
        Assert.Contains("templates:", yaml);
        Assert.Contains("media:", yaml);
        Assert.Contains("content:", yaml);
        Assert.Contains("dictionaryItems:", yaml);
        Assert.Contains("members:", yaml);
        Assert.Contains("users:", yaml);
    }

    [Fact]
    public void ExportRoot_RoundTrip_PreservesData()
    {
        var root = new ExportRoot
        {
            Umbraco = new UmbracoExport
            {
                Languages =
                [
                    new ExportLanguage { IsoCode = "en-US", IsDefault = true }
                ],
                DataTypes =
                [
                    new ExportDataType { Alias = "textstring", Name = "Textstring", EditorUiAlias = "Umb.PropertyEditorUi.TextBox" }
                ]
            }
        };

        var serializer = BuildSerializer();
        var deserializer = BuildDeserializer();

        var yaml = serializer.Serialize(root);
        var restored = deserializer.Deserialize<ExportRoot>(yaml);

        Assert.Single(restored.Umbraco.Languages);
        Assert.Equal("en-US", restored.Umbraco.Languages[0].IsoCode);
        Assert.True(restored.Umbraco.Languages[0].IsDefault);
        Assert.Single(restored.Umbraco.DataTypes);
        Assert.Equal("textstring", restored.Umbraco.DataTypes[0].Alias);
    }
}
