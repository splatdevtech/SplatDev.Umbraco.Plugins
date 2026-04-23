using Moq;
using Umbraco.Cms.Core.Models.PublishedContent;

namespace SplatDev.Umbraco.Tools.Tests;

public class PublishedContentBuilder
{
    private readonly Mock<IPublishedContent> _mock = new();
    private int _id = 1;
    private string _name = "Test Content";
    private string _urlSegment = "test-content";
    private readonly Dictionary<string, object?> _properties = new();

    public PublishedContentBuilder WithId(int id) { _id = id; return this; }
    public PublishedContentBuilder WithName(string name) { _name = name; return this; }
    public PublishedContentBuilder WithUrlSegment(string segment) { _urlSegment = segment; return this; }
    public PublishedContentBuilder WithProperty(string alias, object? value) { _properties[alias] = value; return this; }

    public IPublishedContent Build()
    {
        _mock.Setup(c => c.Id).Returns(_id);
        _mock.Setup(c => c.Name).Returns(_name);
        _mock.Setup(c => c.UrlSegment).Returns(_urlSegment);

        foreach (var (alias, value) in _properties)
        {
            var propMock = new Mock<IPublishedProperty>();
            propMock.Setup(p => p.Alias).Returns(alias);
            propMock.Setup(p => p.GetValue(It.IsAny<string?>(), It.IsAny<string?>())).Returns(value);
            _mock.Setup(c => c.GetProperty(alias)).Returns(propMock.Object);
        }

        return _mock.Object;
    }
}
