using Moq;
using Umbraco.Cms.Core.Models;

namespace SplatDev.Umbraco.Tools.Tests;

public class ContentTypeBuilder
{
    private readonly Mock<IContentType> _mock = new();
    private int _id = 1;
    private string _alias = "testContentType";
    private string _name = "Test Content Type";
    private string _icon = "icon-document";

    public ContentTypeBuilder WithId(int id) { _id = id; return this; }
    public ContentTypeBuilder WithAlias(string alias) { _alias = alias; return this; }
    public ContentTypeBuilder WithName(string name) { _name = name; return this; }
    public ContentTypeBuilder WithIcon(string icon) { _icon = icon; return this; }

    public IContentType Build()
    {
        _mock.Setup(c => c.Id).Returns(_id);
        _mock.Setup(c => c.Alias).Returns(_alias);
        _mock.Setup(c => c.Name).Returns(_name);
        _mock.Setup(c => c.Icon).Returns(_icon);
        return _mock.Object;
    }
}
